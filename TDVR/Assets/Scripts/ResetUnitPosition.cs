using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NetworkedInteractions
{
    public class ResetUnitPosition : MonoBehaviour
    {
        // Start is called before the first frame update
        private Vector3 initialPos;
        public NetworkInteractable NI;
        private Quaternion initialRot;

        void Start()
        {
            initialPos = gameObject.transform.position;
            initialRot = gameObject.transform.rotation;
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void disableCollider()
        {
            GetComponent<Collider>().enabled = false;
        }
        public void enableCollider()
        {
            GetComponent<Collider>().enabled = true;
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        }
        public void resetOnRelease()
        {
            if (NI.previousGrabbedBy != null)
            {
                NI.OnHoverEnd(NI.previousGrabbedBy);
                //NI.OnGrabEnd(NI.grabbedBy);
            }
            //gameObject.SetActive(false);
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
            gameObject.transform.position = initialPos;
            gameObject.transform.rotation = initialRot;
            //gameObject.SetActive(true);
        }
    }

}