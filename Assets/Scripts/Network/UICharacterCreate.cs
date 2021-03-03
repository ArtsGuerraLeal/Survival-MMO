using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterCreate : MonoBehaviour
{
    public InputField nameInput;
    public Button createButton;
    public GameObject UIlogin;

    // Start is called before the first frame update
    void Start()
    {
        createButton.onClick.AddListener(() => {

            CharacterCreateMessage message = new CharacterCreateMessage
            {
                name = nameInput.text,
                username = UIlogin.GetComponent<UILogin>().auth.username
            };

            NetworkClient.Send(message);
            gameObject.SetActive(false);

        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
