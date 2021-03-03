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
using TMPro;
using static Mirror.Authenticators.MMOAuthenticator;
using Mirror.Authenticators;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class NetworkManagerMMO : NetworkManager
{
        public string accountsPath;
        public string streamPath;

        public string chunkPath;
        
        public string charPath;
    
        public static NetworkIdentity playerNet;
        public string json = string.Empty;
        public static NetworkManagerMMO instance;
        public GameObject loginCamera;
        public GameObject player;
        public string accountName;


    public enum NetworkManagerMode { Offline, ServerOnly, ClientOnly, Host }

    public struct LoginMessage : NetworkMessage
    {
        public string account;

    }

 
    private void Awake()
    {
      
        instance = this;
    }

    public override void OnStartServer()
    {
        Debug.Log("Manager: Server is Starting");
        NetworkServer.RegisterHandler<CharacterCreateMessage>(OnCharacterCreate);
        NetworkServer.RegisterHandler<CharacterSelectMessage>(OnCharacterSelect);

        //   NetworkServer.RegisterHandler<LoginMessage>(OnClientLogin);
        //   NetworkServer.RegisterHandler<AccountInfoMessage>(SetAccountName, false);

    }

    private void OnCharacterSelect(NetworkConnection conn, CharacterSelectMessage msg)
    {
        SelectCharacter(msg.character, msg.username);

        GameObject go = Instantiate(player);
        Player playerComponent = go.GetComponent<Player>();

        playerComponent.playerName = msg.character;

        // call this to use this gameobject as the primary controller

        NetworkServer.AddPlayerForConnection(conn, go);
        conn.Send(new CharacterSelectedMessage());


    }

    private void OnCharacterCreate(NetworkConnection conn, CharacterCreateMessage msg)
    {
        //Debug.Log("Character to be created: " + msg.name + msg.username);
        conn.Send(new CharacterCreatedMessage());

        CreateCharacter(msg.name, msg.username);

        GameObject go = Instantiate(player);
        //    Player playerComponent = go.GetComponent<Player>();
        //  playerComponent.playerName = msg.name;
        // call this to use this gameobject as the primary controller
  
        NetworkServer.AddPlayerForConnection(conn, go);

    }

    public void CreateCharacter(string name, string account)
    {
        Character newChar = new Character(name, account);
        using (StreamWriter stream = new StreamWriter(charPath + account+ "_"+ name + ".json"))
        {
            string json = JsonUtility.ToJson(newChar, false);
            stream.Write(json);

        }

        Account acc;
        using (StreamReader stream = new StreamReader(accountsPath + account + ".json"))
        {
            string json = stream.ReadToEnd();
            acc = JsonUtility.FromJson<Account>(json);
        }
        
        Array.Resize(ref acc.characters, acc.characters.Length + 1);
        acc.characters[acc.characters.GetUpperBound(0)] = name;

        using (StreamWriter stream = new StreamWriter(accountsPath + account + ".json"))
        {
            string json = JsonUtility.ToJson(acc, false);
            stream.Write(json);

        }

    }

    public void SelectCharacter(string name, string account)
    {
        Character character;
        using (StreamReader stream = new StreamReader(charPath + account + "_"+ name +  ".json"))
        {
            string json = stream.ReadToEnd();
            character = JsonUtility.FromJson<Character>(json);
        }


    }

    public override void OnStopServer()
    {
        Debug.Log("Manager: Server is Stopping");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Manager: Client has started");
       // NetworkClient.RegisterHandler<CharacterCreatedMessage>(OnCharacterCreated, false);


    }

    private void OnCharacterCreated(NetworkConnection conn, CharacterCreatedMessage msg)
    {
     //   ClientScene.Ready(conn);
     //   authenticator.OnClientAuthenticated.Invoke(conn);
    }

    //Change to character
    public void SetAccountName(NetworkConnection conn, AccountInfoMessage msg) {
        Debug.Log(msg.name);
      //  accountName = msg.name;
  
    }

    //Client tells server that it connected
    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("Manager: Client has Connected");

        //LoginMessage msg = new LoginMessage {
        //    account = accountName
        //};
        //conn.Send(msg);

    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("Disconnected to the server!");
    }

    void OnServerLogin(NetworkConnection conn, string message) {
          
        Debug.Log("Logging in!");
        
    }

    void OnClientLogin(NetworkConnection conn, LoginMessage message)
    {
        Debug.Log("Client has logged in");
       // Character character;
        
        
            //using (StreamReader stream = new StreamReader(charPath + message.account + ".json"))
            //{

            //    string json = stream.ReadToEnd();
            //    character = JsonUtility.FromJson<Character>(json);
            //}

          

        
       
        // playerPrefab is the one assigned in the inspector in Network
        // Manager but you can use different prefabs per race for example
        GameObject go = Instantiate(player);
        Player playerComponent = go.GetComponent<Player>();

        playerComponent.playerName = accountName;

        // call this to use this gameobject as the primary controller

         NetworkServer.AddPlayerForConnection(conn, go);
         Debug.Log(conn.connectionId);
        

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
