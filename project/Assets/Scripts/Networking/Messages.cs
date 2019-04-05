using UnityEngine.Networking;

public class Messages
{
    public const short Base = MsgType.Highest;
    public const short MessageGiveClientId = Base + 1;
    public const short MessageGiveMembersJoined = Base + 2;
    public const short ControlMessage = Base + 3;
}