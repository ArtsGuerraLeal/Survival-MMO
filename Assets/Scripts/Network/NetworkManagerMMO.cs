// We use a custom NetworkManager that also takes care of login, character
// selection, character creation and more.
//
// We don't use the playerPrefab, instead all available player classes should be
// dragged into the spawnable objects property.
//
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Mirror;
using System.IO;
using static NetworkTerrainRenderer;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class NetworkManagerMMO : NetworkManager
{
        public string accountsPath;
        public string streamPath;
        public string chunkPath;
        
        public static NetworkIdentity player;
        public string json = string.Empty;
        public static NetworkManagerMMO instance;
        public GameObject loginCamera;

        public enum NetworkManagerMode { Offline, ServerOnly, ClientOnly, Host }


    private void Awake()
    {
      
        instance = this;
    }

    public override void OnStartServer()
    {
        Debug.Log("Server is Starting");
    }

    public override void OnStopServer()
    {
        Debug.Log("Server is Stopping");
    }

    public override void OnStartClient()
    {
        Debug.Log("Client has started");

    }


    //public override void OnClientConnect(NetworkConnection conn)
    //{
    //    Debug.Log("Connected to the server!");
    //}

    //public override void OnClientDisconnect(NetworkConnection conn)
    //{
    //    Debug.Log("Disconnected to the server!");
    //}

    void OnServerLogin(NetworkConnection conn, string message) {
          
            Debug.Log("Logging in!");
        
    }

    //    //Get Json
    //   [ContextMenu("Send Terrain Datas")]
    //   public void SendTerrainData(NetworkIdentity identity) {
    //   // c.Send(new TerrainMessage { content = json });
    //  //  NetworkClient.connection.identity.GetInstanceID;
    //    NetworkServer.SendToClientOfPlayer(identity, new TerrainMessage { content = json });
    //  //  NetworkServer.SendToAll(new TerrainMessage { content = json });
      
    //}







}
