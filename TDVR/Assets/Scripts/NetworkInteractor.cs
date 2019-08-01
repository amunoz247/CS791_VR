using System.Collections;
using System.Collections.Generic;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Mirror;

using UnityEngine;

namespace NetworkedInteractions {
    [RequireComponent( typeof( SteamVR_Behaviour_Pose ) )]
    [RequireComponent( typeof( Collider ) )]
    [RequireComponent( typeof( Rigidbody ) )]
    public class NetworkInteractor : NetworkBehaviour {
        struct Holding {
            public GameObject obj;
            public Joint joint;
            public NetworkInteractable inter;
        }

        [Tooltip("The action used to grab Interactables")]
        public SteamVR_Action_Boolean grabAction;
        // [Tooltip("The hub used to ")]
        // public NetworkInteractorHub hub;
        [Tooltip("The color to hightlight objects with")]
        [SyncVar]
        public Color highlightColor;

        // List of objects currently being hovered
        private List<GameObject> hovered;
        // List of objects being held in this interactor
        private List<Holding> holding;
        private SteamVR_Behaviour_Pose pose;

        void Awake() {
            pose = GetComponent<SteamVR_Behaviour_Pose>();

            hovered = new List<GameObject>();
            holding = new List<Holding>();
        }

        public override void OnStartLocalPlayer() {
            grabAction.AddOnChangeListener(OnGrab, pose.inputSource);
        }

        private void OnEnable() {
            if(isLocalPlayer) grabAction.AddOnChangeListener(OnGrab, pose.inputSource);
        }

        private void OnDisable() {
            if(isLocalPlayer) grabAction.RemoveOnChangeListener(OnGrab, pose.inputSource);
        }


        private void OnTriggerEnter(Collider other) {
            NetworkInteractable interact = other.GetComponent<NetworkInteractable>();
            if(interact != null && !hovered.Contains(other.gameObject)) {
                hovered.Add(other.gameObject);
                interact.OnHoverStart(this);
            }
        }

        private void OnTriggerExit(Collider other) {
            NetworkInteractable interact = other.GetComponent<NetworkInteractable>();
            if(interact != null && hovered.Contains(other.gameObject)) {
                hovered.Remove(other.gameObject);
                interact.OnHoverEnd(this);
            }
        }

        private void OnGrab(SteamVR_Action_Boolean actionIn, SteamVR_Input_Sources inputSource, bool newValue) {
            NetworkInteractable interact;
            
            if(newValue) {
                // If we've just started grabbing, grab everything being hovered
                foreach(GameObject hov in hovered) {
                    interact = hov.GetComponent<NetworkInteractable>();

                    // If we dont have authority, request it from the server
                    if(!interact.hasAuthority) CmdTransferAuthority(interact.netIdentity);

                    // If this object is being grabbed by something else, force them to drop it
                    if(interact.grabbedBy != null) {
                        interact.grabbedBy.Drop(interact);
                    }

                    // Add a joint, and then conenct it to the object
                    Holding hold;
                    hold.joint = gameObject.AddComponent<FixedJoint>();
                    hold.joint.connectedBody = hov.GetComponent<Rigidbody>();
                    hold.obj = hov;
                    hold.inter = interact;

                    holding.Add(hold);

                    interact.OnGrabStart(this);
                }
            } else {
                // Let go of everything
                foreach(Holding hold in holding) {
                    Destroy(hold.joint);
                    // if(hold.inter.hasAuthority) CmdRemoveAuthority(hold.inter.netIdentity);
                    hold.inter.OnGrabEnd(this);
                }
                holding.Clear();
            }
        }

        public void Drop(NetworkInteractable obj) {
            for(int i = 0; i < holding.Count; i++) {
                if(holding[i].inter == obj) {
                    Destroy(holding[i].joint);
                    // if(holding[i].inter.hasAuthority) CmdRemoveAuthority(holding[i].inter.netIdentity);
                    obj.OnGrabEnd(this);
                    holding.RemoveAt(i);
                    break;
                }
            }
        }

        [Command]
        private void CmdTransferAuthority(NetworkIdentity inter) {
            if(inter.clientAuthorityOwner != null) inter.RemoveClientAuthority(inter.clientAuthorityOwner);
            inter.AssignClientAuthority(connectionToClient);
        }

        [Command]
        private void CmdRemoveAuthority(NetworkIdentity inter) {
            if(inter.clientAuthorityOwner != null) inter.RemoveClientAuthority(inter.clientAuthorityOwner);
        }

        
    }
}