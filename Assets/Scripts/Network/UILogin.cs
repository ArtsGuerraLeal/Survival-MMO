using Mirror;
using Mirror.Authenticators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    public NetworkManagerMMO manager; // singleton=null in Start/Awake
    public BasicAuthenticator auth;
    public GameObject panel;

    public InputField accountInput;
    public InputField passwordInput;
    public InputField nameInput;

    public Button loginButton;
    public Button createButton;

    [TextArea(1, 30)] public string registerMessage = "First time? Just log in and we will\ncreate an account automatically.";


    // Start is called before the first frame update
    void Start()
    {

        loginButton.onClick.AddListener(() => { manager.StartClient(); });
      //  createButton.onClick.AddListener(() => { auth.CreateCharacter(); });

    }

    // Update is called once per frame
    void Update()
    {
        
        if (accountInput.text != "" && passwordInput.text != "") {
            auth.username = accountInput.text;
            auth.password = passwordInput.text;
        }
    
    }
}
