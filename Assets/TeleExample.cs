using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TeleExample : NetworkBehaviour
{
    public string streamPath;
    public string terrainData;
    public class TerraintDataMessage : MessageBase
    {
        public int x;
        public int y;
        public string content; 
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnStartServer()
    {
        Debug.Log("Server HI");
        NetworkServer.RegisterHandler<TerraintDataMessage>(OnRequestTerrain, false);

    }

    public override void OnStartClient() {
        Debug.Log("Client HI");
        NetworkClient.RegisterHandler<TerraintDataMessage>(OnResponseTerrain, false);

    }



    //Server gets data from client and responds with file...
    public void OnRequestTerrain(NetworkConnection conn, TerraintDataMessage msg)
    {
        Debug.Log(msg.x);

        string data;
        Debug.Log("Terrain Requested by client");
        if (File.Exists(NetworkManagerMMO.instance.streamPath + "10_10.json"))
        {
            Debug.Log("File Exists " + NetworkManagerMMO.instance.streamPath + "10_10.json");
            using (StreamReader stream = new StreamReader(NetworkManagerMMO.instance.streamPath + "10_10.json"))
            {
                string json = stream.ReadToEnd();
                Debug.Log(json);
                data = json; 
                    //JsonUtility.FromJson<string>(json);
            }

          


            TerraintDataMessage ResponseMessage = new TerraintDataMessage
            {
                content = data

            };

            conn.Send(ResponseMessage);


        }
    }

    //Client gets file content from server;
    public void OnResponseTerrain(NetworkConnection conn, TerraintDataMessage msg)
    {

        Debug.Log("Terrain Responded by server");
        Debug.Log(msg.content);
       
        //    conn.Disconnect();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isClient) {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                NetworkClient.Send(new TerraintDataMessage { x=10,y=10 });

                //  NetworkServer.(NetworkClient.connection.identity, new TerraintDataMessage { content = json });
                Debug.Log("Sent Data to Server");
            }
        }

        if (isServer)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                NetworkServer.SendToClientOfPlayer(NetworkClient.connection.identity, new TerraintDataMessage { content = "Response" });
                //   NetworkServer.SendToClientOfPlayer(NetworkClient.connection,)
                Debug.Log("Recieved Data from Client");
            }
        }






    }
}
