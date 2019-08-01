using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
namespace NetworkedInteractions
{

    public class CastleHealth : NetworkBehaviour
    {
        [SyncVar]
        public int health = 300;
        public Slider slider;
        public Text myGameOverText;
        public Text enemyGameOverText;
        // Start is called before the first frame update
        void Start()
        {
            slider.maxValue = health;
        }

        // Update is called once per frame
        void Update()
        {
            slider.value = health;
            if (health < 0)
            {
                enemyGameOverText.text = "You Lose";

                myGameOverText.text = "You Win";
                //game over
            }
        }
    }
}
//-15, 2.5, 0
