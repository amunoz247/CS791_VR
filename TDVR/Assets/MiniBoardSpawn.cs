using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Mirror;
namespace NetworkedInteractions
{


    public class MiniBoardSpawn : NetworkBehaviour
    {
        public GameObject unit1;
        public GameObject unit2;
        public Text resourceText;
        public int totalResources = 20;
        public int currentResources = 10;
        private float time = 0;
        public int timeTilNextResource = 3;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            time += Time.deltaTime;
            resourceText.text = currentResources + "/" + totalResources;

            if (time > timeTilNextResource)
            {
                time = 0;
                if (currentResources < totalResources)
                {
                    currentResources += 1;
                }
            }
        }
        public void spawnUnit(GameObject unit, Vector3 spawnLocation, string unitTag)
        {
            Debug.Log("In miniboard spawn");
            if(isServer)
            {
                if (unit.name == "wizard_weapon_legacy DEMO")
                {
                    if (currentResources >= 5)
                    {
                        if (currentResources == totalResources)
                            time = 0;
                        currentResources -= 5;
                        Debug.Log("Is server");
                        GameObject serverUnit = Instantiate(unit, spawnLocation, new Quaternion(0, 90, 0, 1)) as GameObject;
                        serverUnit.tag = unitTag;
                        NetworkServer.Spawn(serverUnit);
                    }
                }
                else if (unit.name == "ZolrikMain")
                {
                    if (currentResources >= 3)
                    {
                        if (currentResources == totalResources)
                            time = 0;
                        currentResources -= 3;
                        Debug.Log("Is server");
                        GameObject serverUnit = Instantiate(unit, spawnLocation, new Quaternion(0, 90, 0, 1)) as GameObject;
                        serverUnit.tag = unitTag;
                        NetworkServer.Spawn(serverUnit);
                    }
                }

                return;
            }
            if (unit == null)
            {
                Debug.Log("it is null in the spawnUnit");
            }
            if (unit != null)
            {
                Debug.Log("is client");
                if (unit.name == "wizard_weapon_legacy DEMO")
                {

                    if(currentResources >= 5)
                    {
                        if (currentResources == totalResources)
                            time = 0;
                        currentResources -= 5;
                        CmdSpawnUnit(0, spawnLocation, unitTag);
                    }
                }
                else if (unit.name == "ZolrikMain")
                {
                    if(currentResources >= 3)
                    {
                        if (currentResources == totalResources)
                            time = 0;
                        currentResources -= 3;
                        CmdSpawnUnit(1, spawnLocation, unitTag);

                    }
                }
            }

        }
        [Command]
        public void CmdSpawnUnit(int unitNum, Vector3 spawnLocation, string unitTag)
        {
            /*if(unit == null)
            {
                Debug.Log("it is null in the command" + "spawn loc" + spawnLocation);
            }
            if (unit != null)
            {
                GameObject serverUnit = Instantiate(unit, spawnLocation, new Quaternion(0, 90, 0, 1)) as GameObject;
                serverUnit.tag = tag;
                NetworkServer.Spawn(serverUnit);
            }*/
            spawnLocation.x = -spawnLocation.x;
            if(unitNum == 0)
            {
                GameObject serverUnit = Instantiate(unit1, spawnLocation, new Quaternion(0, 90, 0, 1)) as GameObject;
                serverUnit.tag = "player2";
                NetworkServer.Spawn(serverUnit);
            }
            else if(unitNum == 1)
            {
                GameObject serverUnit = Instantiate(unit2, spawnLocation, new Quaternion(0, 90, 0, 1)) as GameObject;
                serverUnit.tag = "player2";
                NetworkServer.Spawn(serverUnit);
            }

        }
    }
}
