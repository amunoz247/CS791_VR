using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
namespace NetworkedInteractions
{


    public class collisionSpawn : NetworkBehaviour
    {
        //public UnityEvent spawn;
        // Start is called before the first frame update
        public UnityEvent resetPositionUnit1;
        public UnityEvent resetPositionUnit2;
        public GameObject unit1;
        public GameObject unit2;
        public MiniBoardSpawn mbs;
        public int playerID = 0;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (this.tag == "player1")
            {
                playerID = 0;
            }
            else if (this.tag == "player2")
            {
                playerID = 1;
            }
        }


        public void OnCollisionEnter(Collision collision)
        {
            //ContactPoint contact = collision.contacts[0];
            //Debug.Log(contact.point);
            //spawn.Invoke();
            if(hasAuthority)
            {
                Debug.Log("trying to spawn");
                Vector3 spawnLocation = new Vector3(0, 0, 0);
                string name = this.gameObject.name;
                if (name == "LeftSpawn")
                {
                    if (playerID == 0)
                    {
                        spawnLocation = new Vector3(5, 0, -8);
                    }
                    else if (playerID == 1)
                    {
                        spawnLocation = new Vector3(-5, 0, -8);
                    }

                }
                else if (name == "RightSpawn")
                {
                    if (playerID == 0)
                    {
                        spawnLocation = new Vector3(5, 0, 8);

                    }
                    else if (playerID == 1)
                    {
                        spawnLocation = new Vector3(-5, 0, 8);
                    }

                }
                else if (name == "MidSpawn")
                {
                    if (playerID == 0)
                    {
                        spawnLocation = new Vector3(5, 0, 0);

                    }
                    else if (playerID == 1)
                    {
                        spawnLocation = new Vector3(-5, 0, 0);
                    }

                }
                if (collision.gameObject.name == "GreenUnit")
                {
                    if (unit1 == null)
                    {
                        Debug.Log("it is null in the collision");
                    }
                    mbs.spawnUnit(unit1, spawnLocation, tag);
                    resetPositionUnit1.Invoke();

                }
                else if (collision.gameObject.name == "RedUnit")
                {
                    if (unit2 == null)
                    {
                        Debug.Log("it is null in the collision");
                    }
                    mbs.spawnUnit(unit2, spawnLocation, tag);
                    resetPositionUnit2.Invoke();

                }
            }
                
        }

            //Debug.Log(name);
            //Debug.Log(collision.gameObject.name);
        
        //public void Spawn(GameObject unit)
        //{
        //CmdSpawn(unit);
        //}
        /*
        [Command]
        public void CmdSpawn(GameObject unit, Vector3 spawnLocation)
        {
            if(unit != null)
            {
                GameObject serverUnit = Instantiate(unit, spawnLocation, new Quaternion(0,90,0,1)) as GameObject;
                serverUnit.SetActive(true);
                NetworkServer.Spawn(serverUnit);
            }

        }
        */
    }
}