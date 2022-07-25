using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using System;

public class RoomManager : NetworkRoomManager
{
    public string hostIP;
    [SerializeField] private List<RoomPlayer> roomPlayerList = new List<RoomPlayer>();
    
    public void AddPlayer(RoomPlayer player)
    {
        if (!roomPlayerList.Contains(player))
        {
            roomPlayerList.Add(player);
            Debug.Log("roomPlayer Added");
        }
    }

    public List<RoomPlayer> GetPlayerList()
    {
        return roomPlayerList;
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