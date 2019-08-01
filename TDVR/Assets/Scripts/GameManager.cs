using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using NetworkedInteractions;
using UnityEngine.XR;
public class GameManager : NetworkManager {
    // Singleton
    private static GameManager instance = null;
    public static GameManager Get() {
        return instance;
    }

    // Class stuff
    [Header("Game Info")]
    public Color[] hoverColors;
    public Player humanPlayer = null;
    public Player mousePlayer = null;
    public float mouseScale = 0.15f;
    [HideInInspector]
    public List<Player> players;

    private uint colorIndex = 0;

    // Start is called before the first frame update
    void Start() {
        // Check if instance already exists
        if (instance == null)
            // if not, set instance to this
            instance = this;
        // If instance already exists and it's not this:
        else if (instance != this)
            // Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        players = new List<Player>();
    }

    public override void OnStartServer() {
        NetworkServer.RegisterHandler(CustomMessages.VoiceChat, OnReceiveVoicechatServer);
    }

    public override void OnStartClient(NetworkClient client) {
        client.RegisterHandler(CustomMessages.VoiceChat, OnReceiveVoicechatClient);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, NetworkMessage extraMessage) {
        AddPlayer(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn) {
        AddPlayer(conn);
    }

    private void AddPlayer(NetworkConnection conn) {
        if (!XRDevice.isPresent)
        {
            GameObject player = GameObject.Instantiate(godviewPrefab, new Vector3(0, 15, 0),  Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player);
        }
        else
        {
            Transform startPos = GetStartPosition();
            GameObject player = GameObject.Instantiate(playerPrefab, startPos.position, startPos.rotation);
            Player p = player.GetComponent<Player>();
            p.conn = conn;

            if (humanPlayer == null)
            {
                humanPlayer = p;
                p.isMouse = false;
            }
            else
            {
                mousePlayer = p;
                p.isMouse = true;
                // player.transform.localScale = new Vector3(mouseScale, mouseScale, mouseScale);
            }

            p.id = (byte)players.Count;
            players.Add(p);

            NetworkInteractor[] interactors = player.GetComponentsInChildren<NetworkInteractor>();
            foreach (NetworkInteractor inter in interactors)
            {
                inter.highlightColor = hoverColors[colorIndex];
                colorIndex = (colorIndex + 1) % (uint)hoverColors.Length;
            }

            NetworkServer.AddPlayerForConnection(conn, player);
        }

    }

    // Voice chat stuff

    private void OnReceiveVoicechatServer(NetworkMessage message) {
        VoiceChat.VoiceMessage voiceMsg = message.ReadMessage<VoiceChat.VoiceMessage>();

        foreach(KeyValuePair<int, NetworkConnection> conn in NetworkServer.connections) {
            if(conn.Value != message.conn) {
                conn.Value.Send(CustomMessages.VoiceChat, voiceMsg);
            }
        }
    }

    private void OnReceiveVoicechatClient(NetworkMessage message) {
        VoiceChat.VoiceMessage voiceMsg = message.ReadMessage<VoiceChat.VoiceMessage>();

        players[voiceMsg.playerId].voiceChat.OnReceiveVoiceChat(voiceMsg.data);
    }
}
