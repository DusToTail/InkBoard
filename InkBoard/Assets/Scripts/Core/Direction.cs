using UnityEngine;
using System.Collections.Generic;

public class Direction
{
    public Direction(Vector3Int vec3)
    {
        m_Vector = vec3;
    }
    ~Direction()
    {
    }
    
    public static readonly Direction None = new Direction(Vector3Int.zero);
    public static readonly Direction Front = new Direction(Vector3Int.forward);
    public static readonly Direction Back = new Direction(Vector3Int.back);
    public static readonly Direction Left = new Direction(Vector3Int.left);
    public static readonly Direction Right = new Direction(Vector3Int.right);
    public static readonly Direction Up = new Direction(Vector3Int.up);
    public static readonly Direction Down = new Direction(Vector3Int.down);
    public static readonly Direction[] Directions = new Direction[]
    {
        None,
        Front,
        Back,
        Left,
        Right,
        Up,
        Down,
    };
    public static readonly Dictionary<int, Direction> IntDictionary = new Dictionary<int, Direction>()
    {
        [0] = None,
        [1] = Front,
        [2] = Back,
        [3] = Left,
        [4] = Right,
        [5] = Up,
        [6] = Down,
    };
    public static readonly Dictionary<string, Direction> StringDictionary = new Dictionary<string, Direction>()
    {
        ["None"]    = None,
        ["Front"]   = Front,
        ["Back"]    = Back,
        ["Left"]    = Left,
        ["Right"]   = Right,
        ["Up"]      = Up,
        ["Down"]    = Down,
    };
    public static Direction GetDirection(int index)
    {
        return Directions[index];
    }
    public static implicit operator Vector3Int(Direction dir) { return dir.m_Vector; }
    public static implicit operator Vector3(Direction dir) { return dir.m_Vector; }
    private Vector3Int m_Vector;
}
