using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(VoiceChat))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    public bool isMouse = false;
    [SyncVar]
    public byte id;
    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public VoiceChat voiceChat;
    public GameObject SlingShot;
    public GameObject Table;
    public GameObject Castle;
    public NetworkConnection conn;
    private void Awake() {
        audioSource = GetComponentInChildren<AudioSource>();
        voiceChat = GetComponent<VoiceChat>();
    }
    private void Start() {
        // id stuff needs to be done in Start()
        // Everything else is too early

        if(isClientOnly) {
            while(GameManager.Get().players.Count <= id) GameManager.Get().players.Add(null);
            GameManager.Get().players[id] = this;
            Debug.Log("Registered player " + id);
        }
        if(isServer)
        {
            if(id == 0)
            {
                GameObject slingshot = Instantiate(SlingShot, new Vector3(14, 6, 1), Quaternion.identity);
                //slingshot.GetComponent<NetworkedInteractions.SlingshotManager>().;
                NetworkServer.SpawnWithClientAuthority(slingshot, conn);
                GameObject table = Instantiate(Table, new Vector3(15, 6, 0), Quaternion.identity);
                table.tag = "player1";
                foreach(Transform t in table.transform)
                {
                    t.gameObject.tag = "player1";
                }
                //table.GetComponent<collisionSpawn>().playerID = 0;
                NetworkServer.SpawnWithClientAuthority(table, conn);
                GameObject castle = Instantiate(Castle, new Vector3(15, 2.5f, 0), new Quaternion(0, 180, 0, 1));
                castle.tag = "player1";
                foreach (Transform t in castle.transform)
                {
                    t.gameObject.tag = "player1";
                }
                NetworkServer.SpawnWithClientAuthority(castle, conn);

            }
            else if (id == 1)
            {
                GameObject slingshot1 = Instantiate(SlingShot, new Vector3(-14, 6, 0), Quaternion.identity);
                NetworkServer.SpawnWithClientAuthority(slingshot1, conn);
                GameObject table1 = Instantiate(Table, new Vector3(-15, 6, 0), new Quaternion(0, -90, 0, 1));
                //table1.GetComponent<collisionSpawn>().playerID = 1;
                table1.tag = "player2";
                foreach (Transform t in table1.transform)
                {
                    t.gameObject.tag = "player2";
                }
                NetworkServer.SpawnWithClientAuthority(table1, conn);
                GameObject castle = Instantiate(Castle, new Vector3(-15, 2.5f, 0),Quaternion.identity);
                castle.tag = "player2";
                foreach (Transform t in castle.transform)
                {
                    t.gameObject.tag = "player2";
                }
                NetworkServer.SpawnWithClientAuthority(castle, conn);
            }


        }



    }
    /*
    [Command]
    public void CmdShoot(GameObject ball, Vector3 initialPos, Vector3 holderPosition)
    {
        GameObject clone = Instantiate(ball, holderPosition, Quaternion.identity) as GameObject;

        var heading = initialPos - holderPosition;

        clone.SetActive(true);
        clone.GetComponent<Rigidbody>().AddForce(heading * 20, ForceMode.Impulse);
        Destroy(clone, 3f);
        NetworkServer.Spawn(clone);
    }*/
}
