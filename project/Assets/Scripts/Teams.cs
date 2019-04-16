using System;
using UnityEngine;

public enum Team
{
    None,
    Green,
    Teal,
    Red
}

class TeamUtil
{
    private const string GreenIdent = "gr";
    private const string TealIdent = "te";
    private const string RedIdent = "re";

    public static Color GetTeamColor(Team t)
    {
        switch (t)
        {
            case Team.Green: return Color.green;
            case Team.Teal: return Color.cyan;
            case Team.Red: return Color.red;
        }

        return Color.white;
    }

    public static string ToIdent(Team t)
    {
        // TODO plz dont have this either
        switch (t)
        {
            case Team.Green: return GreenIdent;
            case Team.Red: return RedIdent;
            case Team.Teal: return TealIdent;
            default:
                throw new ArgumentOutOfRangeException(nameof(t), t, null);
        }
    }

    public static Team FromIdent(string s)
    {
        // TODO plz dont have this
        switch (s.ToLower())
        {
            case GreenIdent: return Team.Green;
            case RedIdent: return Team.Red;
            case TealIdent: return Team.Teal;
            default:
                var err = new ArgumentOutOfRangeException(nameof(s), s, "Something's fucky");
                Debug.LogError(err);
                throw err;
        }
    }
}