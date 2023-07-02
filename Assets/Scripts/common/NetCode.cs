using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetCode : MonoBehaviour
{
    public const int REQ_CREAT = 1;
    public const int RSP_CREAT = 2;

    public const int REQ_LOGIN = 11;
    public const int RSP_LOGIN = 12;

    public const int REQ_ROOM_LIST = 13;
    public const int RSP_ROOM_LIST = 14;

    public const int REQ_TABLE_LIST = 15;
    public const int RSP_TABLE_LIST = 16;

    public const int REQ_JOIN_ROOM = 17;
    public const int RSP_JOIN_ROOM = 18;

    public const int REQ_JOIN_TABLE = 19;
    public const int RSP_JOIN_TABLE = 20;

    public const int REQ_SEAT_NUM = 21;
    public const int RSP_SEAT_NUM = 22;

    public const int REQ_DEAL_POKER = 31;
    public const int RSP_DEAL_POKER = 32;

    public const int REQ_CALL_SCORE = 33;
    public const int RSP_CALL_SCORE = 34;

    public const int REQ_DEAL_LORD = 35;
    public const int RSP_DEAL_LORD = 36;

    public const int REQ_DEAL_LORD_CARD = 37;
    public const int RSP_DEAL_LORD_CARD = 38;

    public const int REQ_PLAY_CARD = 41;
    public const int RSP_PLAY_CARD = 42;

    public const int REQ_UNPLAY_CARD = 43;
    public const int RSP_UNPLAY_CARD = 44;

    public const int REQ_SHOW_OTHER_CARD = 45;
    public const int RSP_SHOW_OTHER_CARD = 46;
}
