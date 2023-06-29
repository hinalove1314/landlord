using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
    public int SeatNum { get; set; }
    public int CardNum { get; set; }
    public List<Card> PlayCards { get; set; }
}
