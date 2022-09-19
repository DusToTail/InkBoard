using UnityEngine;

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

    public static implicit operator Vector3Int(Direction dir) { return dir.m_Vector; }

    private Vector3Int m_Vector;
}
