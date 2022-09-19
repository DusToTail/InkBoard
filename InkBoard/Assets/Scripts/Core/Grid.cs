using UnityEngine;

public class MyGrid
{
    public MyGrid(GridLayout layout)
    {
        m_Layout = layout;
        m_Array = new Cell[layout.GetTotalCount()];
        for(uint i = 0; i < m_Layout.GetTotalCount(); i++)
        {
            uint z = i / (layout.yCount * layout.xCount);
            uint countInZDimension = z * layout.yCount * layout.xCount;
            uint y = (i - countInZDimension) / layout.xCount;
            uint countInYDimension = y * layout.xCount;
            uint x = i - countInZDimension - countInYDimension;

            Cell cell = new Cell(x,y,z);
            m_Array[i] = cell;
        }

        Debug.Log("Grid constructed with layout " + layout.ToString());
    }

    ~MyGrid()
    {
    }

    public void DebugLog()
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
    public Cell[] GetArray() { return m_Array; }
    protected GridLayout m_Layout;
    protected Cell[] m_Array;

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
        return string.Format("XYZ layout {0} {1} {2}", xCount, yCount, zCount);
    }
    public uint GetTotalCount() { return xCount * yCount * zCount; }
    public uint xCount;
    public uint yCount;
    public uint zCount;
}

public class Cell
{
    public Cell()
    {
        m_GridPosition = new GridPosition(0,0,0);
    }
    public Cell(GridPosition gridPosition)
    {
        m_GridPosition = gridPosition;
    }
    public Cell(uint x, uint y, uint z)
    {
        m_GridPosition = new GridPosition(x,y,z);
    }
    public Cell(uint x, uint y)
    {
        m_GridPosition = new GridPosition(x, y);
    }
    public Cell(Vector3Int vec3)
    {
        uint x = vec3.x < 0 ? 0 : (uint)vec3.x;
        uint y = vec3.y < 0 ? 0 : (uint)vec3.y;
        uint z = vec3.z < 0 ? 0 : (uint)vec3.z;
        m_GridPosition = new GridPosition(x, y, z);
    }
    public Cell(Vector2Int vec2)
    {
        uint x = vec2.x < 0 ? 0 : (uint)vec2.x;
        uint y = vec2.y < 0 ? 0 : (uint)vec2.y;
        m_GridPosition = new GridPosition(x, y);
    }

    ~Cell()
    {
    }
    public override string ToString()
    {
        return "Cell " + m_GridPosition.ToString();
    }
    protected GridPosition m_GridPosition;
}

public struct GridPosition
{
    public GridPosition(uint x, uint y, uint z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public GridPosition(uint x, uint y)
    {
        this.x = x;
        this.y = y;
        this.z = 0;
    }
    public GridPosition(Vector3Int vec3)
    {
        this.x = vec3.x < 0 ? 0 : (uint)vec3.x;
        this.y = vec3.y < 0 ? 0 : (uint)vec3.y;
        this.z = vec3.z < 0 ? 0 : (uint)vec3.z;
    }
    public GridPosition(Vector2Int vec2)
    {
        this.x = vec2.x < 0 ? 0 : (uint)vec2.x;
        this.y = vec2.y < 0 ? 0 : (uint)vec2.y;
        this.z = 0;
    }
    public static implicit operator Vector3Int(GridPosition pos) { return new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z); }
    public static implicit operator Vector2Int(GridPosition pos) { return new Vector2Int((int)pos.x, (int)pos.y); }
    public override string ToString()
    {
        return string.Format("{0} {1} {2}", x,y,z);
    }
    public uint x;
    public uint y;
    public uint z;
}