using System;
using UnityEngine;

public enum Team
{
    Green,
    Teal,
    Red
}

class TeamUtil
{
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

    public static Team FromString(String s)
    {
        if (Enum.TryParse(s, true, out Teamx team))
        {
            return team;
        }

        throw new Exception($"Could not parse {s}");
    }
}