using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class PrintSteamID : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        Debug.Log("[STEAM-FRIENDS] Listing " + friendCount + " Friends.");
        for (int i = 0; i < friendCount; ++i) {
            CSteamID friendSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            string friendName = SteamFriends.GetFriendPersonaName(friendSteamId);
            EPersonaState friendState = SteamFriends.GetFriendPersonaState(friendSteamId);

            Debug.Log(friendName + "(" + friendSteamId + ")" + " is " + friendState);
        }
    }
}
