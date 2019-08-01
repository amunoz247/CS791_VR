using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.Events;
namespace NetworkedInteractions
{
    public class UnitController : NetworkBehaviour
    {
        public Slider slider; 
        // Start is called before the first frame update
        [SyncVar]
        private Transform target = null;
        [SyncVar]
        public int health = 100;
        public int damagePerSecond = 10;
        public float moveSpeed = 1f;
        public float attackRange = 1.5f;
        public Animation anim;
        public UnityEvent walkEvent;
        public UnityEvent attackEvent;
        [SyncVar]
        public float nearDist = 1000f;
        [SyncVar]
        bool isAttacking = false;

        private float timeTilNextAttack = 1;

        public Image Fill;
        private void Awake()
        {
        }
        void Start()
        {
            slider.maxValue = health;

            anim = gameObject.GetComponent<Animation>();
            if(anim == null)
            {
                walkEvent.Invoke();
            }
            if (isServer)
                InvokeRepeating("getNearestEnemy", 0.0f, 0.1f);

            if(isServer)
            {
                if (tag == "player2")
                {
                    Fill.color = Color.red;
                }
            }
            else
            {
                if (gameObject.transform.position.x > 0)
                {
                    Fill.color = Color.red;
                }
            }

        }

        // Update is called once per frame
        void Update()
        {
            timeTilNextAttack -= Time.deltaTime;
            slider.value = health;
            if (nearDist < attackRange)
            {
                isAttacking = true;
                if(timeTilNextAttack < 0)
                {
                    attackNearestEnemy(target);
                    timeTilNextAttack = 1;
                }
            }
            else
            {
                isAttacking = false;
            }
            if (target != null)
            {
                if(!isAttacking)
                {
                    Vector3 temp = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                    temp.y = transform.position.y;
                    transform.position = temp;
                    walkEvent.Invoke();
                }
                transform.LookAt(new Vector3(target.position.x, this.transform.position.y, target.position.z));
            }
            if(isServer)
            {
                slider.transform.LookAt(new Vector3(15, 5, 0));
            }
            else
            {
                slider.transform.LookAt(new Vector3(-15,5,0));
            }
            if (health <= 0)
            {
                Debug.Log("Destroying game object");
                Destroy(this.gameObject);
            }


        }
        void getNearestEnemy()
        {
            GameObject[] objs = null;
            if (tag == "player1")
            {
                objs = GameObject.FindGameObjectsWithTag("player2");
            }
            if (tag == "player2")
            {
                objs = GameObject.FindGameObjectsWithTag("player1");

            }
            float nearestDistance = 1000f;
            foreach (GameObject obj in objs)
            {
                float dist = Vector3.Distance(transform.position, obj.transform.position);
                if (dist < nearestDistance)
                {
                    nearestDistance = dist;
                    target = obj.transform;
                }
            }
            nearDist = nearestDistance;

        }
        private void attackNearestEnemy(Transform target)
        {
            if (anim != null)
                anim.Play("attack_short_001");
            else
            {
                //Debug.Log("no animation, using animator");
                attackEvent.Invoke();
            }
            if (target != null && target.gameObject.GetComponent<UnitController>() != null)
            {
                transform.LookAt(new Vector3(target.position.x, this.transform.position.y, target.position.z));
                target.gameObject.GetComponent<UnitController>().health -= damagePerSecond;
            }
            else if(target != null && target.gameObject.transform.parent.gameObject.GetComponent<CastleHealth>() != null)
            {
                Debug.Log("attacking enemy castle");
                target.gameObject.transform.parent.gameObject.GetComponent<CastleHealth>().health -= damagePerSecond;

            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "player1ball" && tag == "player2") 
            {
                //Debug.Log(health);
                health -= 40;
                //Debug.Log(health);
                if(collision.gameObject != this.gameObject)
                    Destroy(collision.gameObject);
            }
            else if(collision.gameObject.tag == "player2ball" && tag == "player1")
            {
                //Debug.Log(health);
                health -= 40;
                //Debug.Log(health);
                if (collision.gameObject != this.gameObject)
                    Destroy(collision.gameObject);
            }
        }
    }
}
