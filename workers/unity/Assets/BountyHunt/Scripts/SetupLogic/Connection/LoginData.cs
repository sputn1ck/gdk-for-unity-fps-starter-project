using System;

[Serializable]
public struct LoginData
{
    public string PlayerName;
    public string Pubkey;
    public int RequestedWeapon;

    public LoginData(string playerName, string pubkey, int requestedWeapon)
    {
        PlayerName = playerName;
        Pubkey = pubkey;
        RequestedWeapon = requestedWeapon;
    }
}
