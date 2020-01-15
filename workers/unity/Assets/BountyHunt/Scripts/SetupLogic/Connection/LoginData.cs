using System;

[Serializable]
public struct LoginData
{
    public string PlayerName;
    public string AuthToken;
    public int RequestedWeapon;

    public LoginData(string playerName, string authToken, int requestedWeapon)
    {
        PlayerName = playerName;
        AuthToken = authToken;
        RequestedWeapon = requestedWeapon;
    }
}
