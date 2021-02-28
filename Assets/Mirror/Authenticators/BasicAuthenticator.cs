using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace Mirror.Authenticators
{
    [AddComponentMenu("Network/Authenticators/BasicAuthenticator")]
    public class BasicAuthenticator : NetworkAuthenticator
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(BasicAuthenticator));
        
        [Header("Custom Properties")]

        // set these in the inspector
        public string username;
        public string password;
        public string accountStreamPath;
        public string charStreamPath;

        public Text helpText;
        public GameObject loginUI;
        public GameObject characterUI;

        public GameObject player;
        public class AuthRequestMessage : MessageBase
        {
            // use whatever credentials make sense for your game
            // for example, you might want to pass the accessToken if using oauth
            public string authUsername;
            public string authPassword;
        }

        public class AuthResponseMessage : MessageBase
        {
            public byte code;
            public string message;
            public int id;
            public string name;
        }

        public override void OnStartServer()
        {

            // register a handler for the authentication request we expect from client
            NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
            Debug.Log("Basic Auth: Server Started");

        }

        public override void OnStartClient()
        {

            // register a handler for the authentication response we expect from server
            NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
            Debug.Log("Basic Auth: Client Started");
        }

        public override void OnServerAuthenticate(NetworkConnection conn)
        {
            // do nothing...wait for AuthRequestMessage from client
            Debug.Log("Basic Auth: Server Authenticated");

        }

        public override void OnClientAuthenticate(NetworkConnection conn)
        {
            Debug.Log("Basic Auth: On Client Authenticate");

            AuthRequestMessage authRequestMessage = new AuthRequestMessage
            {
                authUsername = username,
                authPassword = password
            };

            conn.Send(authRequestMessage);
        }

        //Server Request
        public void OnAuthRequestMessage(NetworkConnection conn, AuthRequestMessage msg)
        {
            if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "Authentication Request: {0} {1}", msg.authUsername, msg.authPassword);

            // check the credentials by calling your web server, database table, playfab api, or any method appropriate.
            if (File.Exists(accountStreamPath + msg.authUsername + ".json"))
            {
                Account acc;
                using (StreamReader stream = new StreamReader(accountStreamPath + msg.authUsername + ".json"))
                {

                    string json = stream.ReadToEnd();
                    acc = JsonUtility.FromJson<Account>(json);
                   
                }

                if (msg.authUsername == acc.userLogin && msg.authPassword == acc.userPassword)
                {
                    Debug.Log("Success");


                    AuthResponseMessage authResponseMessage = new AuthResponseMessage
                    {
                        code = 100,
                        message = "Success",
                        id = acc.id,
                        name = acc.userLogin
                    };

                    conn.Send(authResponseMessage);

                    // Invoke the event to complete a successful authentication
                    OnServerAuthenticated.Invoke(conn);
                    

                }
                else
                {
                    Debug.Log("Wrong password");
                    AuthResponseMessage authResponseMessage = new AuthResponseMessage
                    {
                        code = 200,
                        message = "Invalid Credentials"
                    };

                    conn.Send(authResponseMessage);

                    // must set NetworkConnection isAuthenticated = false
                    conn.isAuthenticated = false;

                    // disconnect the client after 1 second so that response message gets delivered
                    StartCoroutine(DelayedDisconnect(conn, 1));
                }


            }
            else
            {
                Debug.Log("File not Exists...Creating file");

                Account newAccount = new Account(msg.authUsername, msg.authPassword,Directory.GetFiles(accountStreamPath, "*", SearchOption.TopDirectoryOnly).Length);
                using (StreamWriter stream = new StreamWriter(accountStreamPath + msg.authUsername + ".json"))
                {
                    string json = JsonUtility.ToJson(newAccount, false);
                    stream.Write(json);
                 
                }

                Debug.Log("Success");

                AuthResponseMessage authResponseMessage = new AuthResponseMessage
                {
                    code = 100,
                    message = "Success",
                    id = newAccount.id,
                    name = newAccount.userLogin

                };

                conn.Send(authResponseMessage);

                // Invoke the event to complete a successful authentication
                OnServerAuthenticated.Invoke(conn);

            }
            
        }

        public IEnumerator DelayedDisconnect(NetworkConnection conn, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            conn.Disconnect();
        }

        //Client Response
        public void OnAuthResponseMessage(NetworkConnection conn, AuthResponseMessage msg)
        {
            if (msg.code == 100)
            {
                if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "Authentication Response: {0}", msg.message);
                helpText.text = "Logging in!";
                // Invoke the event to complete a successful authentication
              //  
                loginUI.SetActive(false);
                //  characterUI.SetActive(true);

                // Debug.Log(NetworkClient.connection.identity.gameObject.name);
                //foreach (Transform child in NetworkClient.connection.identity.gameObject.transform)
                //{
                //    if (child.tag == "NameTag")
                //        child.GetComponent<TextMesh>().text = msg.name;
                //}
                // NetworkClient.connection.identity.gameObject.transform.position = new Vector3(199, 199, 0);


                //These need to be ran on the server, not the client!!!

                //    OnClientAuthenticated.Invoke(conn);
             //   GameObject go = Instantiate(player);
             //   Debug.Log(conn);
            //    Debug.Log(go);

             //   NetworkServer.AddPlayerForConnection(conn, go);

                if (File.Exists(charStreamPath + msg.name + ".json"))
                {
                    Debug.Log("PLayer exists");
                  
                   
                //    OnClientAuthenticated.Invoke(conn);

                }
                else {
                    Debug.Log("No Character for account");
               //     CreateCharacter(msg.name);
                //    OnClientAuthenticated.Invoke(conn);
                //    GameObject gameobject = Instantiate(player);

                //    NetworkServer.AddPlayerForConnection(conn, gameobject);
                    

                }

                Debug.Log("Success Login");
            }
            else
            {
                // logger.LogFormat(LogType.Error, "Authentication Response: {0}", msg.message);
                helpText.text = "Wrong Password!";
                // Set this on the client for local reference
                conn.isAuthenticated = false;

                // disconnect the client
                conn.Disconnect();
            }
        }

        public void CreateCharacter( string name)
        {
            Character newChar = new Character(name);
            using (StreamWriter stream = new StreamWriter(charStreamPath + name + ".json"))
            {
                string json = JsonUtility.ToJson(newChar, false);
                stream.Write(json);

            }
        }

       

    }



    [Serializable]
    internal class Account
    {
        public string userLogin;
        public string userPassword;
        public int id;

        public Account(string user, string pass, int id)
        {
            this.userLogin = user;
            this.userPassword = pass;
            this.id = id;

        }

        public override string ToString()
        {
            return base.ToString();
        }

    }

    [Serializable]
    internal class Character
    {
        public string name;
   
        

        public Character(string name)
        {
            this.name = name;


        }

        public override string ToString()
        {
            return base.ToString();
        }

    }
}
