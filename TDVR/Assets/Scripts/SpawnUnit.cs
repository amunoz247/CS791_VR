using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SpawnUnit : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(Transform trans)
    {

    }

    public void Spawn(GameObject unit)
    {
        CmdSpawn(unit);
    }
    [Command]
    private void CmdSpawn(GameObject unit)
    {
        var serverUnit = Instantiate(unit, this.transform);
        NetworkServer.Spawn(serverUnit);
    }
}
