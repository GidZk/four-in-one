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

    public static int ToInt(Team t)
    {
        // TODO plz dont have this either
        switch (t)
        {
            case Team.Green: return 0;
            case Team.Red: return 1;
            case Team.Teal: return 2;
            default:
                throw new ArgumentOutOfRangeException(nameof(t), t, null);
        }
    }

    public static Team FromInt(int d)
    {
        // TODO plz dont have this
        switch (d)
        {
            case 0: return Team.Green;
            case 1: return Team.Red;
            case 2: return Team.Teal;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}