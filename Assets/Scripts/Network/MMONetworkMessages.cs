// Contains all the network messages that we need.
using System.Collections.Generic;
using System.Linq;
using Mirror;

// client to server ////////////////////////////////////////////////////////////
public struct AuthRequestMessage : NetworkMessage
{
    // use whatever credentials make sense for your game
    // for example, you might want to pass the accessToken if using oauth
    public string authUsername;
    public string authPassword;
}

public struct AccountInfoMessage : NetworkMessage
{

    public string name;
}

public struct AuthResponseMessage : NetworkMessage
{
    public byte code;
    public string message;
    public int id;
    public string username;
    public string[] characters;
}

public struct CharacterCreateMessage : NetworkMessage
{
    public string name;
    public string username;

}

public struct CharacterSelectMessage : NetworkMessage
{
    public string username;
    public string character;

}

public struct CharacterCreatedMessage : NetworkMessage
{
    public string name;

}

public struct CharacterSelectedMessage : NetworkMessage
{
    public string name;

}

public struct LoginMessage : NetworkMessage
{

}
