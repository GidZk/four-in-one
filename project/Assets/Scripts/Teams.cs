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

    // Hastily implemented because TryParse doesn't seem to work
    public static Team FromString(String s)
    {
        switch (s.ToLower().Remove(s.IndexOf('\0')))
        {
            case "green": return Team.Green;
            case "teal": return Team.Teal;
            case "red": return Team.Red;
        }

        throw new Exception($"Could not parse {s}");
    }
}