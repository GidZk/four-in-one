using UnityEngine.Networking;

#pragma warning disable 618

public class Messages
{
    public const short Base = MsgType.Highest;
    public const short MessageGiveClientId = Base + 1;
    public const short MessageGiveMembersJoined = Base + 2;
    public const short ControlMessage = Base + 3;
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
    VERTICAL,
    HORIZONTAL,
    CANNON_ANGLE,
    CANNON_LAUNCH
}