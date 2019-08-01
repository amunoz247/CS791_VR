using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;
using Valve.VR;

[RequireComponent( typeof( NetworkIdentity ) )]
public class VoiceChat : NetworkBehaviour
{
    // Custom Voicechat message payload
     public class VoiceMessage : MessageBase {
         // Compressed audio data
         public byte[] data;
         // The player who generated the chat (for playing through correct AudioSource)
         public byte playerId;
    }

    [Tooltip("The SteamVR action used to enable talking")]
    public SteamVR_Action_Boolean pushToTalk;
    [Tooltip("A list of controllers which can push to talk")]
    public List<SteamVR_Behaviour_Pose> controls;
    [Tooltip("The AudioSource to play this voice through")]
    public AudioSource audioSource;
    [Tooltip("The audio sample rate. If 0, will be automatically chosen.")]
    public uint sampleRate = 0;
    [Tooltip("The size of the internal buffer (in audio frames) used to store the received audio data.")]
    public uint voiceBufferSize = 22528;

    // The number of push-to-talks being held. Un-Comment if you are using a hold-to-talk scheme.
    // Also search "Un-Comment if you are using a hold-to-talk scheme" to completely enable this feature.
    // private uint numPushing = 0;

    // Whether or not the user is currently recording (i.e. has at least one push to talk held)
    private bool isRecording = false;
    // The internal voice buffer. Voice data is received and put into this buffer, then taken out to supply the Audio Source.
    // Cyclic in nature - once you reach the end, resume again from the beginning.
    private short[] voiceData;
    // Keeps track of the next frame which has yet to be played (as an index in voiceData). Increases as OnAudioRead() is called.
    // Once it reaches the end, resets from the beginning.
    private uint voicePosition = 0;
    // Keeps track of the next frame where we will store new audio data (as an index in vocieData). Increases as OnReceiveCoiceChat() is called.
    private uint voiceEnd = 0;
    private AudioClip voiceClip;
    private Player player;

    private void Awake() {
        // Steam gives us most optimal sample rate for this CPU
        if(sampleRate == 0) sampleRate = SteamUser.GetVoiceOptimalSampleRate();

        player = GetComponent<Player>();

        if(!isLocalPlayer) {
            // Stored as a short, as Steam gives us 16-bit audio
            voiceData = new short[voiceBufferSize];

            // The length of the clip needs to be small, so that Unity doesn't cache a large amount of blank audio data.
            // Otherwise, this results in large delays until the audio data reaches the AudioSource and potentially overflowing your temporary buffer.
            // As sampleRate / 50, this means the internal buffer of the AudioClip can only store 1/50 a second of audio. This means that OnAudioRead()
            // gets called more often (and less efficiently), but that we don't have a large delay in voice chat. Fiddle with it if you aren't happy with performance.
            voiceClip = AudioClip.Create("Voice Clip", (int) sampleRate / 50, 1, (int) sampleRate, true, OnAudioRead);
            audioSource.clip = voiceClip;
            audioSource.Play();
        }
    }

    public override void OnStartLocalPlayer() {
        Setup();
    }

    private void OnEnable() {
        if(isLocalPlayer) {
            Setup();
        }
    }

    private void Setup() {
        // Un-Comment if you are using a hold-to-talk scheme
        // numPushing = 0;

        foreach(SteamVR_Behaviour_Pose pose in controls) {
            pushToTalk.AddOnChangeListener(PushedToTalk, pose.inputSource);

            // There's potential for the user to already be holding their push to talk
            if(pushToTalk.GetState(pose.inputSource)) {
                // Un-Comment if you are using a hold-to-talk scheme
                // numPushing++;

                if(!isRecording) {
                    SteamUser.StartVoiceRecording();
                    isRecording = true;
                    StartCoroutine(SendVoice());
                }
            }
        }
    }

    private void OnDisable() {
        if(isLocalPlayer) {
            foreach(SteamVR_Behaviour_Pose pose in controls) {
                pushToTalk.RemoveOnChangeListener(PushedToTalk, pose.inputSource);
            }

            // Stop recording
            if(isRecording) {
                SteamUser.StopVoiceRecording();
                isRecording = false;
            }

            // Un-Comment if you are using a hold-to-talk scheme
            // numPushing = 0;
        }
    }

    private void PushedToTalk(SteamVR_Action_Boolean actionIn, SteamVR_Input_Sources inputSource, bool newValue) {
        // Un-Comment if you are using a hold-to-talk scheme
        // if(newValue) numPushing++;
        // else numPushing--;

        // Un-Comment if you are using a hold-to-talk scheme
        if(/*numPushing == 0 && */isRecording) {
            SteamUser.StopVoiceRecording();
            isRecording = false;
        } else if(!isRecording) {
            SteamUser.StartVoiceRecording();
            isRecording = true;
            StartCoroutine(SendVoice());
        }
    }

    private IEnumerator SendVoice() {
        while(isRecording) {
            uint size, bytesWritten;
            // Test the waters for voice data - this function tells us how much voice data there is waiting in the buffer,
            // as well as the status of the voice service
            EVoiceResult result = SteamUser.GetAvailableVoice(out size);

            switch(result) {
                case EVoiceResult.k_EVoiceResultOK:
                    break;
                // If there's no data - don't bother sending anything
                case EVoiceResult.k_EVoiceResultNoData:
                    yield return null;
                    continue;
                default:
                    Debug.LogError("Failed to get any voice availability data: " + result);
                    yield return null;
                    continue;
            }

            // Custom VoiceChat payload - the player id will be used on the other client to play through correct audio source
            VoiceMessage msg = new VoiceMessage() {
                data = new byte[size],
                playerId = player.id
            };

            // Put voice data into message. This has potential to fail like GetAvailableVoice, but it's unlikely.
            result = SteamUser.GetVoice(true, msg.data, size, out bytesWritten);

            if(result != EVoiceResult.k_EVoiceResultOK) {
                Debug.LogError("Failed to get any voice data: " + result);
                yield return null;
                continue;
            }

            // Send to the server
            connectionToServer.Send(CustomMessages.VoiceChat, msg);

            // Wait for next frame
            yield return null;
        }
    }

    public void OnReceiveVoiceChat(byte[] data) {
        // Allocate a temporary buffer to get data from Steam
        byte[] tempBuffer = new byte[voiceBufferSize];
        uint bytesWritten;
        short singleFrame;

        // NOTE: DecompressVoice returns an array of bytes, but the data is 16-bit audio!
        // Meaning for each **2** bytes written, there is **1** audio frame
        // Also note that the data is SIGNED (i.e. not from 0 to 65535 but from -32768 to 32767)
        EVoiceResult result = SteamUser.DecompressVoice(data, (uint) data.Length, tempBuffer, voiceBufferSize, out bytesWritten, sampleRate);
        switch(result) {
            case EVoiceResult.k_EVoiceResultOK:
                break;
            default:
                Debug.LogError("Failure decompressing voice data: " + result + ", " + bytesWritten);
                return;
        }

        // Now format that data correctly and put it into our main buffer
        for(uint i = 0; i < bytesWritten; i += 2) {
            // High order byte first
            singleFrame = tempBuffer[i + 1];
            // Shift over
            singleFrame <<= 8;
            // Then lower order byte
            singleFrame |= tempBuffer[i];

            // Important that we do i/2, as discussed above (two bytes is one audio frame). As well % voiceBufferSize wraps automatically for us.
            voiceData[(voiceEnd + i / 2) % voiceBufferSize] = singleFrame;
        }

        // Make certain we don't overlap. Empty space in between voice end and voice position. Complex to calculate due to wrapping nature of buffer.
        // Note that emptySpace is in no. of frames, not no. of bytes. Also note the -1 -- this guarantees at leaste one empty byte between
        // the end of the buffer and the beginning (so we always know how it wraps).
        uint emptySpace = ((voicePosition > voiceEnd) ? (voicePosition - voiceEnd) : (voicePosition + (voiceBufferSize - voiceEnd))) - 1;

        // If our data has made the end of the buffer and where we are currently at overlap.
        // Then the data we're currently working with has already been overwritten and we need to increase the position.
        if(bytesWritten / 2 > emptySpace) {
            // A helpful log to let the developer know to increase the buffer a bit
            Debug.LogWarning("Data overlapped! Consider increasing buffer size! Overlapped by " + (bytesWritten / 2 - emptySpace) + " bytes. Increase to " + (voiceBufferSize + bytesWritten / 2 - emptySpace));

            // Then shuffle along the current position - the data has already been overwritten.
            voicePosition = (voicePosition + bytesWritten / 2 - emptySpace) % voiceBufferSize;
        }

        voiceEnd = (voiceEnd + (uint)bytesWritten / 2) % voiceBufferSize;
    }

    private void OnAudioRead(float[] outData) {
        // voicePosition only equals voiceEnd if there's no data left
        // (see emptySpace above and note that if voicePosition == voiceEnd, then emptySpace = voiceBufferSize)
        if(voicePosition == voiceEnd) {
            for(uint count = 0; count < outData.Length; count++) {
                outData[count] = 0.0f;
            }
        } else {
            uint count;
            // Write out as much meaningful data as possible
            for(count = 0; count < outData.Length && voicePosition != voiceEnd; count++) {
                // Unity expects a float in the range of [-1,1]
                outData[count] = voiceData[voicePosition] / 32768.0f;
                voicePosition = (voicePosition + 1) % voiceBufferSize;
            }

            // Then if we have any space left over, write nothing
            for(; count < outData.Length; count++) {
                outData[count] = 0.0f;
            }
        }
    }
}
