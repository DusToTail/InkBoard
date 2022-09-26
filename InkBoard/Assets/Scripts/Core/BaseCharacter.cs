using System;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    public delegate void Created(BaseCharacter character);
    public delegate void Deleted(BaseCharacter character);

    public static event Created OnCreated;
    public static event Deleted OnDeleted;

    public CharacterManager manager;

    public int ID { get { return id; } }
    public Vector3Int GridPosition { get { return gridPosition; } }

    [SerializeField] protected int id;
    [SerializeField] protected Vector3Int gridPosition;
    private BaseCharacterData m_BaseData;

    public BaseCharacter()
    {
        if(OnCreated != null)
            OnCreated(this);
    }
    ~BaseCharacter()
    {
        if(OnDeleted != null)
            OnDeleted(this);
    }
    public virtual void Init(object data)
    {
        if (data == null) { return; }
        Debug.Log("BaseCharacter: Init");
        m_BaseData = data as BaseCharacterData;
        id = m_BaseData.id;
        SetGridPosition(m_BaseData.GridPosition);
        SetPosition(Board.Instance.GetWorldPositionAt(m_BaseData.GridPosition));
        SetRotation(Quaternion.LookRotation(m_BaseData.Front, m_BaseData.Up));
    }
    public virtual bool DefaultAction() 
    { 
        Debug.Log("BaseCharacter: DefaultAction");
        return true; 
    }
    public virtual bool SendMoveRequest()
    {
        Debug.Log("BaseCharacter: SendMoveRequest");
        return true;
    }
    protected void SetGridPosition(Vector3Int gridPosition)
    {
        this.gridPosition = gridPosition;
    }
    protected void SetPosition(Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }
    protected void SetRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
    [System.Serializable]
    public class BaseCharacterData : IPersistentData
    {
        public Vector3Int GridPosition { get { return new Vector3Int(gridPosition[0], gridPosition[1], gridPosition[2]); } }
        public Direction Front { get { return Direction.StringDictionary[front_up_right[0]]; } }
        public Direction Up { get { return Direction.StringDictionary[front_up_right[1]]; } }
        public Direction Right { get { return Direction.StringDictionary[front_up_right[2]]; } }
        public string actualType;
        public int id;
        public int[] gridPosition;
        public string[] front_up_right;

        public BaseCharacterData(int id, Vector3Int gridPosition, string[] orientation)
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
            BaseCharacterData baseCharacterData = data as BaseCharacterData;
            id = baseCharacterData.id;
            gridPosition = baseCharacterData.gridPosition;
            front_up_right = baseCharacterData.front_up_right;
            SetArray(gridPosition, baseCharacterData.gridPosition);
            SetArray(front_up_right, baseCharacterData.front_up_right);
        }

        public virtual void SaveTo(object data)
        {
            BaseCharacterData baseCharacterData = data as BaseCharacterData;
            baseCharacterData.id = id;
            baseCharacterData.gridPosition = gridPosition;
            baseCharacterData.front_up_right = front_up_right;
            SetArray(baseCharacterData.gridPosition, gridPosition);
            SetArray(baseCharacterData.front_up_right, front_up_right);
        }

        public void SetArray<T>(T[] srcArray, T[] destArray)
        {
            System.Array.Copy(srcArray, destArray, srcArray.Length);
        }
    }
}
