using UnityEngine;

public class Block : MonoBehaviour
{
    public int ID { get { return id; } }
    public Vector3Int GridPosition { get { return gridPosition; } }
    [SerializeField] protected int id;
    [SerializeField] protected Vector3Int gridPosition;
    private BlockData m_Data;

    public void Init(object data)
    {
        if (data == null) { return; }
        //Debug.Log("Block: Init");
        m_Data = data as BlockData;
        id = m_Data.id;
        SetGridPosition(m_Data.GridPosition);
        SetPosition(Board.Instance.GetWorldPositionAt(m_Data.GridPosition));
        SetRotation(m_Data.Front, m_Data.Up);
    }
    public virtual BlockData GetData() { return m_Data; }
    protected void SetGridPosition(Vector3Int gridPosition)
    {
        this.gridPosition = gridPosition;
    }
    protected void SetPosition(Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }
    protected void SetRotation(Vector3Int forward, Vector3Int up)
    {
        transform.rotation = Quaternion.LookRotation(forward, up);
    }
    [System.Serializable]
    public class BlockData : IPersistentData
    {
        public Vector3Int GridPosition { get { return new Vector3Int(gridPosition[0], gridPosition[1], gridPosition[2]); } }
        public Direction Front { get { return Direction.StringDictionary[front_up_right[0]]; } }
        public Direction Up { get { return Direction.StringDictionary[front_up_right[1]]; } }
        public Direction Right { get { return Direction.StringDictionary[front_up_right[2]]; } }
        public string actualType;
        public int id;
        public int[] gridPosition;
        public string[] front_up_right;

        public BlockData(int id, Vector3Int gridPosition, string[] orientation)
        {
            this.id = id;
            this.gridPosition = new int[] { gridPosition.x, gridPosition.y, gridPosition.z };
            this.front_up_right = new string[] { orientation[0], orientation[1], orientation[2] };
        }

        public virtual void SetData(int id, Vector3Int gridPosition, string[] orientation)
        {
            this.id = id;
            this.gridPosition = new int[] { gridPosition.x, gridPosition.y, gridPosition.z };
            this.front_up_right = new string[] { orientation[0], orientation[1], orientation[2] };
        }

        public virtual void LoadFrom(object data)
        {
            BlockData blockData = data as BlockData;
            id = blockData.id;
            gridPosition = blockData.gridPosition;
            front_up_right = blockData.front_up_right;
            SetArray(gridPosition, blockData.gridPosition);
            SetArray(front_up_right, blockData.front_up_right);
        }

        public virtual void SaveTo(object data)
        {
            BlockData blockData = data as BlockData;
            blockData.id = id;
            blockData.gridPosition = gridPosition;
            blockData.front_up_right = front_up_right;
            SetArray(blockData.gridPosition, gridPosition);
            SetArray(blockData.front_up_right, front_up_right);
        }

        public void SetArray<T>(T[] srcArray, T[] destArray)
        {
            System.Array.Copy(srcArray, destArray, srcArray.Length);
        }
    }
}
