using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;
public class DisplaySteamFriends : MonoBehaviour
{
    public GameObject menuItem;
    private Canvas canvas;
    public float spacing;
    public float bottom;
    // Start is called before the first frame update
    void Start()
    {
        if (canvas == null) {
            canvas = GetComponent<Canvas>();
        }

        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);  
        for (int i = 0; i < friendCount; i++) {
            CSteamID friendSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            string friendName = SteamFriends.GetFriendPersonaName(friendSteamId);
            EPersonaState friendState = SteamFriends.GetFriendPersonaState(friendSteamId);

            GameObject child = Instantiate(menuItem, canvas.transform);
            child.transform.localPosition = child.transform.localPosition + new Vector3(0, bottom + (0.25f + spacing) * i, 0);
            TextMeshProUGUI text = child.GetComponentInChildren<TextMeshProUGUI>();
            text.text = friendName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
