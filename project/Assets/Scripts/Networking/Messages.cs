using UnityEngine.Networking;

#pragma warning disable 618

public static class Messages
{
    private const short Base = MsgType.Highest;
    public const short ClientId = Base + 1;
    public const short MemberCount = Base + 2;
    public const short Control = Base + 3;
    public const short StartGame = Base + 4;
    public const short PuzzleReady = Base + 4;
    public const short ClearPuzzle = Base + 5;
    public const short EndGame = Base + 6;
    public const short RestartGame = Base + 7;
}

public class ControlMessage : MessageBase
{
    public readonly float Value;
    public readonly ControlType Type;

    public ControlMessage(float value, ControlType type)
    {
        Value = value;
        Type = type;
    }

    public ControlMessage()
    {
    }
}

public enum ControlType
{
    Vertical,
    Horizontal,
    CannonAngle,
    CannonLaunch
}