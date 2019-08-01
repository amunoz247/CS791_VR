using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace NetworkedInteractions
{
    public class SlingshotManager : NetworkBehaviour
    {
        public Transform leftAnchor;

        public Transform rightAnchor;

        public Transform ObjectHolderResetPos;

        public Transform ObjectHolder;

        public GameObject ball;

        public GameObject ObjectHolderGameObject;

        public LineRenderer[] lines;

        public static SlingshotManager instance;

        private Vector3 initialPos;

        private Quaternion initialRot;

        public Player play;

        public NetworkInteractable NetInt;

        
        private Vector3 tempHolderPos;
        // Use this for initialization

        void Start()
        {
            lines[0].material.color = Color.white;
            lines[1].material.color = Color.white;
            lines[0].SetPosition(0, leftAnchor.position);
            lines[1].SetPosition(0, rightAnchor.position);
            initialPos = ObjectHolder.transform.position;
            initialRot = ObjectHolder.transform.rotation;
        }

        // Update is called once per frame

        void Update()

        {
            lines[0].SetPosition(0, leftAnchor.position);
            lines[1].SetPosition(0, rightAnchor.position);
            for (int i = 0; i < lines.Length; i++)
            {

                lines[i].SetPosition(1, ObjectHolder.position);

            }
            if(hasAuthority)
            {
                if (NetInt.grabbedBy == null)
                {
                    ObjectHolder.position = ObjectHolderResetPos.position;
                    ObjectHolder.rotation = ObjectHolderResetPos.rotation;
                }
            }




        }
        public void AssignPlayer(Player player)
        {
            play = player;
        }

        public void shoot()
        {

            //Destroy(clone, 5f);
            if (isServer)
            {
                tempHolderPos = ObjectHolder.position;

                GameObject clone = Instantiate(ball, ObjectHolderResetPos.position, Quaternion.identity) as GameObject;
                //clone.tag = "Slingshot";
                clone.tag = "player1ball";
                var heading = ObjectHolderResetPos.position - tempHolderPos;
                //Debug.Log(tempHolderPos + "on server");

                clone.SetActive(true);
                Destroy(clone, 5f);

                clone.GetComponent<Rigidbody>().AddForce(heading * 20f / 1000, ForceMode.Impulse);

                NetworkServer.Spawn(clone);
                ObjectHolder.position = Vector3.MoveTowards(ObjectHolder.position, ObjectHolderResetPos.position, 1 * Time.deltaTime);
                // will need to be the rotation of the slingshot as a whole, might just put in update
                ObjectHolder.rotation = ObjectHolderResetPos.rotation;
                ObjectHolderGameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                ObjectHolderGameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
            }
            else
            {
                //Debug.Log(tempHolderPos + "before command");
                //ObjectHolder.GetComponent<Collider>().enabled = false;
                CmdShoot();

            }

        }

        [Command]
        public void CmdShoot()
        {
            GameObject clone = Instantiate(ball, ObjectHolderResetPos.position, Quaternion.identity) as GameObject;
            //clone.tag = "Slingshot";

            var heading = ObjectHolderResetPos.position - ObjectHolder.position;
            //Debug.Log(tempHolderPos + "after command");
            clone.tag = "player2ball";

            clone.SetActive(true);
            clone.GetComponent<Rigidbody>().AddForce(heading * 20f/1000, ForceMode.Impulse);
            Destroy(clone, 5f);
            NetworkServer.Spawn(clone);
            //ObjectHolderGameObject.GetComponent<Rigidbody>().isKinematic = true;
            ObjectHolder.position = Vector3.MoveTowards(ObjectHolder.position,ObjectHolderResetPos.position,1*Time.deltaTime);
            // will need to be the rotation of the slingshot as a whole, might just put in update
            ObjectHolder.rotation = ObjectHolderResetPos.rotation;
            //ObjectHolderGameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            //ObjectHolderGameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);

        }
    }
}