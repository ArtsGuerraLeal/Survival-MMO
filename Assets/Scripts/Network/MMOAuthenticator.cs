using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace Mirror.Authenticators
{
    [AddComponentMenu("Network/Authenticators/BasicAuthenticator")]
    public class MMOAuthenticator : NetworkAuthenticator
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(MMOAuthenticator));

        [Header("Custom Properties")]

        // set these in the inspector
        public string username;
        public string password;
        public string accountStreamPath;
        public string charStreamPath;
        public static MMOAuthenticator instance;

        public Text helpText;
        public GameObject loginUI;
        public GameObject characterUI;
        public GameObject characterSelectUI;


        public string charName;

        public GameObject player;

        private void Awake()
        {
            instance = this;
        }


        public override void OnStartServer()
        {

            // register a handler for the authentication request we expect from client
            NetworkServer.RegisterHandler<AuthRequestMessage>(OnServerLogin, false);
          //  NetworkServer.RegisterHandler<CharacterCreateMessage>(OnCharacterCreate, false);
        //    NetworkServer.RegisterHandler<CharacterSelectMessage>(OnCharacterSelect, false);

            Debug.Log("Basic Auth: Server Started");

        }

        public override void OnStartClient()
        {

            // register a handler for the authentication response we expect from server
            NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
            NetworkClient.RegisterHandler<CharacterCreatedMessage>(OnCharacterCreated, false);
       //     NetworkClient.RegisterHandler<CharacterSelectedMessage>(OnCharacterSelected, false);

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
            Debug.Log("Login Message Was Sent");
            conn.Send(authRequestMessage);
        }

        //Server Request to check if account exists or if it needs to be created
        public void OnServerLogin(NetworkConnection conn, AuthRequestMessage msg)
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
                    Debug.Log("Successful authentication");

                    AuthResponseMessage authResponseMessage = new AuthResponseMessage
                    {
                        code = 100,
                        message = "Success",
                        id = acc.id,
                        username = acc.userLogin,
                        characters = acc.characters
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
                string[] chars = { }; 
                Account newAccount = new Account(msg.authUsername, msg.authPassword, Directory.GetFiles(accountStreamPath, "*", SearchOption.TopDirectoryOnly).Length, chars);
                using (StreamWriter stream = new StreamWriter(accountStreamPath + msg.authUsername + ".json"))
                {
                    string json = JsonUtility.ToJson(newAccount, false);
                    stream.Write(json);

                }

                Debug.Log("Successful Creation");

                AuthResponseMessage authResponseMessage = new AuthResponseMessage
                {
                    code = 100,
                    message = "Success",
                    id = newAccount.id,
                    username = newAccount.userLogin,
                    characters = newAccount.characters


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

        public void OnAuthResponseMessage2(NetworkConnection conn, AuthResponseMessage msg)
        {
            if (msg.code == 100)
            {
                if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "Authentication Response: {0}", msg.message);
                helpText.text = "Logging in!";

                if (File.Exists(charStreamPath + msg.username + ".json"))
                {
                    Character character;
                    using (StreamReader stream = new StreamReader(charStreamPath + msg.username + ".json"))
                    {

                        string json = stream.ReadToEnd();
                        character = JsonUtility.FromJson<Character>(json);

                    }

                    List<string> m_DropOptions = new List<string> { character.name };


                    Debug.Log(character.name);
                    string n = character.name;
                    loginUI.GetComponent<UILogin>().characterSelectUI.SetActive(true);
                    loginUI.GetComponent<UILogin>().characterList.AddOptions(m_DropOptions);

                }
                else
                {
                    Debug.Log("No Character for account");
                    characterUI.SetActive(true);

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

        //What to do in client after being authenticated and recieving a response
        public void OnAuthResponseMessage(NetworkConnection conn, AuthResponseMessage msg)
        {
            if (msg.code == 100)
            {
                if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "Authentication Response: {0}", msg.message);            
                Debug.Log("Success Login");

                //OnClientAuthenticated.Invoke(conn);

                if (msg.characters.Length == 0)
                {
                    Debug.Log("No Characters");
                    loginUI.SetActive(false);
                    characterUI.SetActive(true);
                }
                else
                {
                    Debug.Log("Characters Found");
                    loginUI.SetActive(false);
                    characterSelectUI.SetActive(true);
                    List<string> m_DropOptions = new List<string>(msg.characters);       
                    characterSelectUI.GetComponent<UICharacterSelect>().characterList.AddOptions(m_DropOptions);
                }

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

        public void OnCharacterCreate(NetworkConnection conn, CharacterCreateMessage msg)
        {
            Debug.Log("OnCharactercreate");

            
            CreateCharacter(msg.name, msg.username);

            AccountInfoMessage accMessage = new AccountInfoMessage
            {
                name = msg.name
            };

            conn.Send(accMessage);


            CharacterCreatedMessage characterCreatedMessage = new CharacterCreatedMessage
            {
              name = msg.name

            };

            conn.Send(characterCreatedMessage);

        }

        public void OnCharacterSelect(NetworkConnection conn, CharacterSelectMessage msg)
        {
            Debug.Log("OnCharacterSelect");



            AccountInfoMessage accMessage = new AccountInfoMessage
            {
                name = msg.username
            };

            conn.Send(accMessage);


            CharacterSelectedMessage characterSelectMessage = new CharacterSelectedMessage
            {
                name = msg.username

            };

            conn.Send(characterSelectMessage);

        }

        public void OnCharacterCreated(NetworkConnection conn, CharacterCreatedMessage msg)
        {
            Debug.Log("Character created");
            ClientScene.Ready(conn);
            OnClientAuthenticated.Invoke(conn);
        }

        public void OnCharacterCreated2(NetworkConnection conn, CharacterCreatedMessage msg)
        {
            AccountInfoMessage accMessage = new AccountInfoMessage
            {
                name = msg.name
            };

            conn.Send(accMessage);

            Debug.Log("Character create");
            characterUI.SetActive(false);
            loginUI.SetActive(false);
            ClientScene.Ready(conn);
            base.OnClientAuthenticated.Invoke(conn);
        }

        public void OnCharacterSelected(NetworkConnection conn, CharacterSelectedMessage msg)
        {
            AccountInfoMessage accMessage = new AccountInfoMessage
            {
                name = msg.name
            };

            conn.Send(accMessage);

            Debug.Log("Character Selected");
            loginUI.GetComponent<UILogin>().characterSelectUI.SetActive(false);
            loginUI.SetActive(false);
            ClientScene.Ready(conn);
            base.OnClientAuthenticated.Invoke(conn);
        }


        public void SetCharacter(string name)
        {
            charName = name;
        }

        public void CreateCharacter(string name, string account)
        {
            Character newChar = new Character(name, account);
            using (StreamWriter stream = new StreamWriter(charStreamPath + account + ".json"))
            {
                string json = JsonUtility.ToJson(newChar, false);
                stream.Write(json);

            }
        }



    }




}
