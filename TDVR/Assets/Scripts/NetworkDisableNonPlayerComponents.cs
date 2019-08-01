using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkDisableNonPlayerComponents : NetworkBehaviour {
    public List<Behaviour> disable;
    public List<Collider> disableColliders;
    public List<Behaviour> enable;

    // Start is called before the first frame update
    void Start()
    {
        if(!isLocalPlayer) {
            foreach(Behaviour obj in disable) {
                obj.enabled = false;
            }

            foreach(Collider obj in disableColliders) {
                obj.enabled = false;
            }
        } else {
            foreach(Behaviour obj in enable) {
                obj.enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
