using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Account
{
    public string userLogin;
    public string userPassword;

    public Account(string user, string pass)
    {
        this.userLogin = user;
        this.userPassword = pass;
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
