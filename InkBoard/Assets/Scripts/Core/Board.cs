using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour, ISimulate<GameState>
{
    public static Board Instance { get; private set; }

    public GridLayout Layout { get { return m_Layout; } }

    [Header("Configurations")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private float betweenDistance;
    [SerializeField] private Vector3Int gridLayout;
    [Header("Gizmos")]
    [SerializeField] private Color color;
    [SerializeField] private bool displayGizmos;
    private MyGrid<Block> m_Grid;
    private BoardData m_Data;
    private int m_BlockInitIndex;
    private GridLayout m_Layout;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    public void Init(string data)
    {
        if(!string.IsNullOrEmpty(data))
            DeserializeData(data);
        gridLayout = new Vector3Int((int)m_Data.width, (int)m_Data.height, (int)m_Data.length);
        m_Layout = new GridLayout((uint)gridLayout.x, (uint)gridLayout.y, (uint)gridLayout.z);
        m_Grid = new MyGrid<Block>(m_Layout);
        m_BlockInitIndex = -1;
        m_Grid.ForEach(CreateBlockAtCell);
#if DEBUG
        //m_Grid.DebugLog();
#endif
    }
    public Block GetBlockAt(GridPosition gridPosition)
    {
        var block = m_Grid.GetValueAt(gridPosition);
        return block;
    }
    public Vector3 GetWorldPositionAt(Vector3Int gridPosition)
    {
        Vector3 localPosition = (Vector3)gridPosition * betweenDistance;
        return transform.position + localPosition;
    }
    public Vector3 GetWorldPositionAt(GridPosition gridPosition)
    {
        Vector3 localPosition = (Vector3)gridPosition * betweenDistance;
        return transform.position + localPosition;
    }
    public void SetBlockAt(GridPosition gridPosition, Block block)
    {
        m_Grid.SetValueAt(gridPosition, block);
    }
    public bool HasValueAt(GridPosition gridPosition)
    {
        var block = m_Grid.GetValueAt(gridPosition);
        return block == null ? false : true;
    }
    public void Simulate(GameState refSim)
    {
        refSim.blockDatas = new MyGrid<Block.BlockData>(m_Grid.Layout);
        m_Grid.ForEach(x =>
        {
            if (x.Value != null)
            {
                refSim.blockDatas.SetValueAt(x.GetGridPosition(), x.Value.GetData());
            }
        });
    }
    public string SerializeData()
    {
        List<string> blocks = new List<string>();
        for (int i = 0; i < Layout.GetTotalCount(); i++)
        {
            var data = m_Data.blockDatas[i];
            Type type = data.GetType();
            data.actualType = type.FullName;
            if (type == typeof(Block.BlockData))
            {
                string dataString = JsonUtility.ToJson(data);
                blocks.Add(dataString);
            }
        }
        string blocksString = string.Join(",\n", blocks.ToArray());
        string boardData = @"
{
""betweenDistance"": " + betweenDistance + @",
""width"": " + Layout.xCount + @",
""length"": " + Layout.zCount + @",
""height"": " + Layout.yCount + @", 
""blockDatas"": [" + blocksString + @"]
}";
        return boardData;
    }
    public void DeserializeData(string content)
    {
        m_Data = JsonUtility.FromJson<BoardData>(content);
    }
    private void CreateBlockAtCell(Cell<Block> cell)
    {
        //Debug.Log("Block at " + cell.GridPosition);
        m_BlockInitIndex++;
        var data = m_Data.blockDatas[m_BlockInitIndex];
        if (data == null) { Debug.LogError("Data is null!"); return; }

        var prefab = GetBlockFromID(data.id);
        if (prefab == null) 
        { 
            //Debug.Log($"Board: {cell} Empty"); 
            return; 
        }
        var characterObj = Instantiate(prefab, transform);
        var block = characterObj.GetComponent<Block>();
        block.Init(data);
        cell.SetValue(block);

        //Debug.Log($"Board: {cell} {block.ID}");
    }
    private GameObject GetBlockFromID(int id)
    {
        if(id == -1) { return null; }
        return System.Array.Find(prefabs, x => x.GetComponent<Block>().ID == id);
    }
    
#if DEBUG
    private void DrawGizmosCubeAtCell(Cell<Block> cell)
    {
        Vector3Int gridPosition = cell.GridPosition;
        Vector3 localPosition = (Vector3)gridPosition * betweenDistance;
        Gizmos.color = color;
        Gizmos.DrawWireCube(transform.position + localPosition, Vector3.one);
    }
    private void OnDrawGizmos()
    {
        if (!displayGizmos) { return; }
        if(m_Grid == null) { return; }
        m_Grid.ForEach(DrawGizmosCubeAtCell);
    }
#endif

    [System.Serializable]
    public class BoardData : IPersistentData
    {
        public float betweenDistance;
        public uint width;
        public uint length;
        public uint height;
        public Block.BlockData[] blockDatas;

        public BoardData(float dis, GridLayout layout)
        {
            betweenDistance = dis;
            width = layout.xCount;
            length = layout.yCount;
            height = layout.zCount;
            blockDatas = new Block.BlockData[layout.GetTotalCount()];
        }

        public void SetBlockIDs(Block.BlockData[] srcArray)
        {
            blockDatas = new Block.BlockData[srcArray.Length];
            for (int i = 0; i < srcArray.Length; i++)
            {
                var srcData = srcArray[i];
                Block.BlockData data = new Block.BlockData(srcData.id, srcData.GridPosition, srcData.front_up_right);
                blockDatas[i] = data;
            }
        }

        public void LoadFrom(object data)
        {
            BoardData boardData = data as BoardData;
            betweenDistance = boardData.betweenDistance;
            width = boardData.width;
            length = boardData.length;
            height = boardData.height;
            SetBlockIDs(boardData.blockDatas);
        }

        public void SaveTo(object data)
        {
            BoardData boardData = data as BoardData;
            boardData.betweenDistance = betweenDistance;
            boardData.width = width;
            boardData.length = length;
            boardData.height = height;
            boardData.SetBlockIDs(blockDatas);
        }

    }
}
