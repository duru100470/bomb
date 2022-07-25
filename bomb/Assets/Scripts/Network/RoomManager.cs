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
    
    public override void OnStartHost()
    {
        //hostIP = Encrypt(PlayerSetting.hostIP);
        //Debug.Log(hostIP);
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

    public string Decrypt(string str)
    {
        string ret = String.Empty;
        for(int i=0; i< str.Length/2; i++)
        {
            string cur = str.Substring(i*2, 2);
            Debug.Log(cur);
            int first = cur[0] >= 'A' ? cur[0] - 'A' + 10 : cur[0] - '0';
            int second = cur[1] >= 'A' ? cur[1] - 'A' + 10 : cur[1] - '0';
            int intValue = first * 16 + second;
            Debug.Log(intValue);
            ret += intValue;
            if(i != str.Length/2 -1) ret += ".";
        }
        return ret;
    }
}