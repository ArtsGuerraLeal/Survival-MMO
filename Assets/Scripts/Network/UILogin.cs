using Mirror;
using Mirror.Authenticators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Mirror.Authenticators.MMOAuthenticator;

public class UILogin : MonoBehaviour
{
    public NetworkManagerMMO manager; // singleton=null in Start/Awake
    public MMOAuthenticator auth;
    public GameObject panel;
    public GameObject characterUI;
    public GameObject characterSelectUI;

    public InputField accountInput;
    public InputField passwordInput;
    public InputField nameInput;

    public Button loginButton;
    public Button createButton;
    public Button selectButton;

    public Dropdown characterList;

    [TextArea(1, 30)] public string registerMessage = "First time? Just log in and an account\n will create automatically.";


    // Start is called before the first frame update
    void Start()
    {
        loginButton.onClick.AddListener(() => { manager.StartClient(); });
    }

    // Update is called once per frame
    void Update()
    {
        
        if (accountInput.text != "" && passwordInput.text != "") {
            auth.username = accountInput.text;
            auth.password = passwordInput.text;
        }

        if (nameInput.text != "")
        {
            auth.charName = nameInput.text;
        }

    }
}
