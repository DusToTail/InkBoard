using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Configurations")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private float betweenDistance;
    [SerializeField] private Vector3Int gridLayout;
    [Header("Gizmos")]
    [SerializeField] private Color color;
    [SerializeField] private bool displayGizmos;
    private MyGrid<Block> m_Grid;
    private BoardData m_Data;
    private uint m_BlockInitIndex;

    private RequestHandler<Board> m_RequestHandler;

    private void Start()
    {
        Init();

        m_RequestHandler = new RequestHandler<Board>();
        m_RequestHandler.AddNewRequest(DoA, ActionIsFalse);
        m_RequestHandler.AddNewRequestAt(this, DoB, ActionIsTrue);
        m_RequestHandler.ProcessRequests(true);
    }

    private void DoA(Board board)
    {
        Debug.Log($"Board {board.name} did A!");
    }
    private void DoB(Board board)
    {
        Debug.Log($"Board {board.name} did B!");
    }
    private bool ActionIsTrue(Board board)
    {
        return true;
    }
    private bool ActionIsFalse(Board board)
    {
        return false;
    }

    public void Init()
    {
        if(m_Data != null)
        {
            betweenDistance = m_Data.betweenDistance;
            gridLayout = new Vector3Int((int)m_Data.width, (int)m_Data.length, (int)m_Data.height);
        }
        m_Grid = new MyGrid<Block>(new GridLayout((uint)gridLayout.x, (uint)gridLayout.y, (uint)gridLayout.z));
        m_BlockInitIndex = 0;
        m_Grid.ForEach(CreateBlockAtCell);
#if DEBUG
        m_Grid.DebugLog();
#endif
    }

    private void CreateBlockAtCell(Cell<Block> cell)
    {
        Vector3Int gridPosition = cell.GetGridPositionVec3();
        Vector3 localPosition = (Vector3)gridPosition * betweenDistance;
        var prefab = GetBlockFromID(m_Data == null ? 0 : m_Data.blockIDs[m_BlockInitIndex]);

        if (prefab == null) { return; }

        var blockObj = Instantiate(prefab, transform.position + localPosition, Quaternion.identity, transform);
        var block = blockObj.GetComponent<Block>();
        block.Init();
        cell.SetValue(block);

        m_BlockInitIndex++;
    }

    private GameObject GetBlockFromID(int id)
    {
        return System.Array.Find(prefabs, x => x.GetComponent<Block>().ID == id);
    }

    

#if DEBUG
    private void DrawGizmosCubeAtCell(Cell<Block> cell)
    {
        Vector3Int gridPosition = cell.GetGridPositionVec3();
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

    public class BoardData : IPersistentData
    {
        public float betweenDistance;
        public uint width;
        public uint length;
        public uint height;
        public int[] blockIDs;

        public BoardData(float dis, GridLayout layout)
        {
            betweenDistance = dis;
            width = layout.xCount;
            length = layout.yCount;
            height = layout.zCount;
            blockIDs = new int[layout.GetTotalCount()];
        }

        public void SetBlockIDs(int[] srcArray)
        {
            System.Array.Copy(srcArray, blockIDs, srcArray.Length);
        }

        public void LoadFrom(object data)
        {
            BoardData boardData = data as BoardData;
            betweenDistance = boardData.betweenDistance;
            width = boardData.width;
            length = boardData.length;
            height = boardData.height;
            SetBlockIDs(boardData.blockIDs);
        }

        public void SaveTo(object data)
        {
            BoardData boardData = data as BoardData;
            boardData.betweenDistance = betweenDistance;
            boardData.width = width;
            boardData.length = length;
            boardData.height = height;
            boardData.SetBlockIDs(blockIDs);
        }
    }
}
