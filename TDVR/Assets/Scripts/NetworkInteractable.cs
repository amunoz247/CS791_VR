using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;

namespace NetworkedInteractions {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(VelocityEstimator))]
    public class NetworkInteractable : NetworkBehaviour
    {
        public struct Hoverer {
            public NetworkInteractor inter;
            public GameObject highlight;
        }
        public List<Hoverer> hoveredBy;
        public NetworkInteractor grabbedBy = null;
        public float highlightThickness = 0.005f;
        public UnityEvent GrabStart;
        public UnityEvent GrabEnd;

        private VelocityEstimator vel;
        private Rigidbody body;
        public Material hoverMaterial;
        public NetworkInteractor previousGrabbedBy = null;
        private void Awake() {
            vel = GetComponent<VelocityEstimator>();
            body = GetComponent<Rigidbody>();

            body.isKinematic = true;

            hoveredBy = new List<Hoverer>();
            // hoverMaterial = Resources.Load<Material>("Highlight");
        }

        // private void Update() {
        //     body.isKinematic = !hasAuthority;
        // }

        public void OnHoverStart(NetworkInteractor inter) {
            Hoverer hov;
            hov.inter = inter;

            MeshFilter filter = GetComponentInChildren<MeshFilter>();
            MeshRenderer render = filter.GetComponent<MeshRenderer>();

            hov.highlight = new GameObject("Highlight");
            hov.highlight.transform.parent = filter.transform;
            hov.highlight.transform.localPosition = Vector3.zero;
            hov.highlight.transform.localRotation = Quaternion.identity;
            hov.highlight.transform.localScale = Vector3.one;

            MeshFilter newFilter = hov.highlight.AddComponent<MeshFilter>();
            MeshRenderer newRender = hov.highlight.AddComponent<MeshRenderer>();
            Material highlightMaterial = new Material(hoverMaterial);
            float highlightDistance = hoveredBy.Count * highlightThickness;

            highlightMaterial.SetColor("g_vOutlineColor", inter.highlightColor);
            highlightMaterial.SetFloat("g_flOutlineWidth", highlightThickness);
            highlightMaterial.SetFloat("g_flExtrudeDistance", highlightDistance);
            highlightMaterial.renderQueue = highlightMaterial.shader.renderQueue + hoveredBy.Count;

            newFilter.sharedMesh = filter.sharedMesh;
            newRender.material = highlightMaterial;
            hoveredBy.Add(hov);
        }

        public void OnHoverEnd(NetworkInteractor inter) {
            int index;
            for(index = 0; index < hoveredBy.Count; index++) {
                if(hoveredBy[index].inter == inter) {
                    break;
                }
            }
            if (hoveredBy.Count > 0)
            {
                Destroy(hoveredBy[index].highlight);
                hoveredBy.RemoveAt(index);
            }

            for(; index < hoveredBy.Count; index++) {
                MeshRenderer render = hoveredBy[index].highlight.GetComponent<MeshRenderer>();
                render.material.renderQueue--;
                render.material.SetFloat("g_flExtrudeDistance", highlightThickness * index);
            }
        }

        public void OnGrabStart(NetworkInteractor inter) {
            vel.BeginEstimatingVelocity();
            grabbedBy = inter;
            
            GrabStart.Invoke();

        }

        public void OnGrabEnd(NetworkInteractor inter) {
            vel.FinishEstimatingVelocity();
            body.velocity = vel.GetVelocityEstimate();
            body.angularVelocity = vel.GetAngularVelocityEstimate();
            //OnHoverEnd(grabbedBy);
            grabbedBy = null;
            GrabEnd.Invoke();

        }

        public override void OnStartAuthority() {
            body.isKinematic = false;
        }

        public override void OnStopAuthority() {
            body.isKinematic = true;
        }
    }
}
