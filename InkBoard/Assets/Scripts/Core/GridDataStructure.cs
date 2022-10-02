using System;
using UnityEngine;

public class MyGrid<T> where T : class
{
    public MyGrid(GridLayout layout)
    {
        m_Layout = layout;
        m_Array = new Cell<T>[layout.GetTotalCount()];
        for(uint i = 0; i < m_Layout.GetTotalCount(); i++)
        {
            uint z = i / (layout.yCount * layout.xCount);
            uint countInZDimension = z * layout.yCount * layout.xCount;
            uint y = (i - countInZDimension) / layout.xCount;
            uint countInYDimension = y * layout.xCount;
            uint x = i - countInZDimension - countInYDimension;

            Cell<T> cell = new Cell<T>(x,y,z);
            m_Array[i] = cell;
        }
    }
    ~MyGrid()
    {
    }
    public virtual void ForEach(Action<Cell<T>> action)
    {
        for(uint i = 0; i < m_Array.Length; i++)
            action(m_Array[i]);
    }
    public virtual void DebugLog()
    {
        Debug.Log("******GRID DEBUG LOG******");
        Debug.Log("Layout: " + m_Layout.ToString());
        Debug.Log("Total count: " + m_Layout.GetTotalCount());
        Debug.Log("Displaying cells ...");
        for (uint i = 0; i < m_Array.Length; i++)
        {
            Debug.Log(m_Array[i].ToString());
        }
        Debug.Log("Finished!");
    }
    public Cell<T>[] GetArray() { return m_Array; }
    public T GetValueAt(GridPosition position)
    {
        bool valid = IsValidPosition(position, m_Layout);
        if (!valid) { Debug.LogWarning($"Grid position {position.ToString()} is not valid in layout {m_Layout.ToString()}"); return null; }
        uint index = (uint)(position.z * (m_Layout.xCount * m_Layout.yCount) + position.y * m_Layout.xCount + position.x);
        return m_Array[index].Value;
    }
    public void SetValueAt(GridPosition position, T value)
    {
        bool valid = IsValidPosition(position, m_Layout);
        if (!valid) { Debug.LogWarning($"Grid position {position.ToString()} is not valid in layout {m_Layout.ToString()}"); return; }
        uint index = (uint)(position.z * (m_Layout.xCount * m_Layout.yCount) + position.y * m_Layout.xCount + position.x);
        m_Array[index].SetValue(value);
    }
    public static bool IsValidPosition(GridPosition position, GridLayout layout)
    {
        bool valid =    position.x < layout.xCount && position.y < layout.yCount && position.z < layout.zCount && 
                        position.x > -1 && position.y > -1 && position.z > -1;
        return valid;
    }
    public GridLayout Layout { get { return m_Layout; } }
    protected GridLayout m_Layout;
    protected Cell<T>[] m_Array;

}

// Grid layout order when used in initializing an array is Z-Y-X (height-length-width)
// The count of each dimension should be at least 1
public struct GridLayout
{
    public GridLayout(uint xCount, uint yCount, uint zCount)
    {
        Debug.Assert(xCount > 0 && yCount > 0 && zCount > 0);
        this.xCount = xCount;
        this.yCount = yCount;
        this.zCount = zCount;
    }
    public override string ToString()
    {
        return string.Format("XYZ {0} {1} {2}", xCount, yCount, zCount);
    }
    public uint GetTotalCount() { return xCount * yCount * zCount; }
    public uint xCount;
    public uint yCount;
    public uint zCount;
}

public class Cell<T> where T : class
{
    public Cell(T value = null)
    {
        m_GridPosition = new GridPosition(0,0,0);
        m_Value = value;
    }
    public Cell(GridPosition gridPosition, T value = null)
    {
        m_GridPosition = gridPosition;
        m_Value = value;
    }
    public Cell(uint x, uint y, uint z, T value = null)
    {
        m_GridPosition = new GridPosition(x,y,z);
        m_Value = value;
    }
    public Cell(uint x, uint y, T value = null)
    {
        m_GridPosition = new GridPosition(x, y);
        m_Value = value;
    }
    public Cell(Vector3Int vec3, T value = null)
    {
        uint x = vec3.x < 0 ? 0 : (uint)vec3.x;
        uint y = vec3.y < 0 ? 0 : (uint)vec3.y;
        uint z = vec3.z < 0 ? 0 : (uint)vec3.z;
        m_GridPosition = new GridPosition(x, y, z);
        m_Value = value;
    }
    public Cell(Vector2Int vec2, T value = null)
    {
        uint x = vec2.x < 0 ? 0 : (uint)vec2.x;
        uint y = vec2.y < 0 ? 0 : (uint)vec2.y;
        m_GridPosition = new GridPosition(x, y);
        m_Value = value;
    }
    ~Cell()
    {
    }
    public Vector3Int GridPosition { get { return m_GridPosition; } }
    public GridPosition GetGridPosition() { return m_GridPosition; }
    public T Value { get { return m_Value; } }
    public void SetValue(T val) { m_Value = val; }
    public override string ToString()
    {
        return "Cell " + m_GridPosition.ToString();
    }
    protected GridPosition m_GridPosition;
    protected T m_Value;
}

public struct GridPosition
{
    public GridPosition(uint x, uint y, uint z)
    {
        this.x = (int)x;
        this.y = (int)y;
        this.z = (int)z;
    }
    public GridPosition(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public GridPosition(uint x, uint y)
    {
        this.x = (int)x;
        this.y = (int)y;
        this.z = 0;
    }
    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.z = 0;
    }
    public GridPosition(Vector3Int vec3)
    {
        this.x = vec3.x;
        this.y = vec3.y;
        this.z = vec3.z;
    }
    public GridPosition(Vector2Int vec2)
    {
        this.x = vec2.x;
        this.y = vec2.y;
        this.z = 0;
    }
    public static implicit operator Vector3Int(GridPosition pos) { return new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z); }
    public static implicit operator Vector3(GridPosition pos) { return new Vector3((float)pos.x, (float)pos.y, (float)pos.z); }
    public static implicit operator Vector2Int(GridPosition pos) { return new Vector2Int((int)pos.x, (int)pos.y); }
    public static implicit operator Vector2(GridPosition pos) { return new Vector2((float)pos.x, (float)pos.y); }
    public override string ToString()
    {
        return string.Format("{0} {1} {2}", x,y,z);
    }
    public int x;
    public int y;
    public int z;
}