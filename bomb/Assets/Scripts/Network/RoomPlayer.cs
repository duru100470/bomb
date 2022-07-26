using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayer : NetworkRoomPlayer
{
    [SyncVar]
    public string nickname;
    RoomManager manager = NetworkManager.singleton as RoomManager;
    LobbyPlayerMover lobbyPlayer;

    public override void OnStartClient()
    {   
        if(isServer) 
        {
            SpawnLobbyPlayer();
        }
        else
        {
            CmdSetNickname(PlayerSetting.playerNickname);
        }
    }

    [Command]
    public void CmdSetNickname(string name)
    {
        nickname = name;
        lobbyPlayer.playerNickname = name;
    }

    private void SpawnLobbyPlayer()
    {
        var player = Instantiate(manager.spawnPrefabs[0]);
        lobbyPlayer = player.GetComponent<LobbyPlayerMover>();
        NetworkServer.Spawn(player, connectionToClient);
        CmdSetNickname(PlayerSetting.playerNickname);
    }

}