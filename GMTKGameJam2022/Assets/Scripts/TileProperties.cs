using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileProperties : ScriptableObject
{
    public bool isWalkable;
    public bool isWall;
    public bool isSpawner = false;
}