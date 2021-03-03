using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelect : MonoBehaviour
{
    public Dropdown characterList;
    public Button selectButton;
    public GameObject loginUI;

    // Start is called before the first frame update
    void Start()
    {
        selectButton.onClick.AddListener(() => {

            CharacterSelectMessage message = new CharacterSelectMessage
            {
                character = characterList.options[characterList.value].text,
                username = loginUI.GetComponent<UILogin>().auth.username
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
