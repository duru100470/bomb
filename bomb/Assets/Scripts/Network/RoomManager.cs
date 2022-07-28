using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using System;

public class RoomManager : NetworkRoomManager
{
    public string hostIP;
    public string playerNickname;
    [SerializeField] private List<RoomPlayer> roomPlayerList = new List<RoomPlayer>();
    [SerializeField] private List<GameObject> gamePlayer = new List<GameObject>();

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        GameObject obj = Instantiate(gamePlayer[UnityEngine.Random.Range(0, gamePlayer.Count)]);
        
        return obj;
    }

    public override void OnRoomServerPlayersReady()
    {
        PlayerSetting.playerNum = roomSlots.Count;
        base.OnRoomServerPlayersReady();
    }
    public string Encrypt(string str)
    {
        string ret = String.Empty;
        string[] strings = str.Split('.');
        foreach(var strng in strings)
        {
            int cur = Int32.Parse(strng);
            if(cur < 17) ret += "0";
            ret += Int32.Parse(strng).ToString("X");
        }
        return ret;
    }
}