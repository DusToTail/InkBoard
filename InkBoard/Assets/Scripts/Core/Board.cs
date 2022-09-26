using UnityEngine;

public class Board : MonoBehaviour
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
    private RequestHandler<Board> m_BoardHandler;
    private TurnController<Board> m_TurnController;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    private void Start()
    {
        //m_BoardHandler = new RequestHandler<Board>();
        //m_BoardHandler.AddNewRequest(DoA, ActionConditionIsFalse);
        //m_BoardHandler.StackNewRequestAt(this, DoB, ActionConditionIsTrue);
        //m_BoardHandler.ProcessRequests(true);

        //m_TurnController = new TurnController<Board>(DoDefault);
        //m_TurnController.RegisterPlayer(this, DoDefault);
        //m_TurnController.RegisterAction(this, DoA);
        //m_TurnController.RegisterAction(this, DoB);
        //for (int i = 0; i < 3; i++)
        //{
        //    m_TurnController.ProcessTurn();
        //}
        //m_TurnController.DebugLog(TurnController<Board>.DEBUG_INFO.TURNS);
    }
    
    private bool DoDefault(Board board)
    {
        Debug.Log($"Board {board.name} did default!");
        return true;
    }
    private bool DoA(Board board)
    {
        Debug.Log($"Board {board.name} did A!");
        return true;
    }
    private bool DoB(Board board)
    {
        Debug.Log($"Board {board.name} did B!");
        return true;
    }
    private bool ActionConditionIsTrue(Board board)
    {
        return true;
    }
    private bool ActionConditionIsFalse(Board board)
    {
        return false;
    }
    
    public void Init(string data)
    {
        if(!string.IsNullOrEmpty(data))
            DeserializeData(data); 
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
    public string SerializeData()
    {
        return JsonUtility.ToJson(m_Data);
    }
    public void DeserializeData(string content)
    {
        m_Data = JsonUtility.FromJson<BoardData>(content);
        betweenDistance = m_Data.betweenDistance;
        gridLayout = new Vector3Int((int)m_Data.width, (int)m_Data.height, (int)m_Data.length);
    }
    private void CreateBlockAtCell(Cell<Block> cell)
    {
        Vector3Int gridPosition = cell.GridPosition;
        Vector3 localPosition = (Vector3)gridPosition * betweenDistance;
        m_BlockInitIndex++;
        var prefab = GetBlockFromID(m_Data == null ? 0 : m_Data.blockIDs[m_BlockInitIndex]);
        if (prefab == null) { Debug.Log($"Board: {cell} Empty"); return; }

        Vector3 worldPosition = transform.position + localPosition;
        var blockObj = Instantiate(prefab, worldPosition, Quaternion.identity, transform);
        var block = blockObj.GetComponent<Block>();
        block.Init();
        cell.SetValue(block);

        Debug.Log($"Board: {cell} {block.ID}");
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
