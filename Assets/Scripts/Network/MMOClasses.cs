using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

[Serializable]
internal class Account
{
    public string userLogin;
    public string userPassword;
    public int id;
    public string[] characters;


    public Account(string user, string pass, int id, string[] characters)
    {
        this.userLogin = user;
        this.userPassword = pass;
        this.id = id;
        this.characters = characters;

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
    public string account;
    public Vector3 position;
    public int hp;


    public Character(string name, string account)
    {
        this.name = name;
        this.account = account;
        this.position = new Vector3(168, 55);
        this.hp = 100;

    }

    public override string ToString()
    {
        return base.ToString();
    }

}
