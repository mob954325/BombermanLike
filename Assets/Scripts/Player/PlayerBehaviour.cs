using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerBehaviour : NetworkBehaviour
{
    [Networked] 
    public int playerId { get; set; }

    [Networked]
    public Color playerColor { get; set; }
}
