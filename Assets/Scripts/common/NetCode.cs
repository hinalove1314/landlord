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

    public const int REQ_SHOW_POKER = 35;
    public const int RSP_SHOW_POKER = 36;

    public const int REQ_SHOT_POKER = 37;
    public const int RSP_SHOT_POKER = 38;

    public const int REQ_GAME_OVER = 41;
    public const int RSP_GAME_OVER = 42;

    public const int REQ_CHAT = 43;
    public const int RSP_CHAT = 44;

    public const int REQ_RESTART = 45;
    public const int RSP_RESTART = 46;
}
