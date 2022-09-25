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
        gridPosition = m_BaseData.GridPosition;
        id = m_BaseData.id;
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
    public void SetGridPosition(Vector3Int gridPosition)
    {
        this.gridPosition = gridPosition;
    }
    [System.Serializable]
    public class BaseCharacterData : IPersistentData
    {
        public Vector3Int GridPosition { get { return new Vector3Int(gridPosition[0], gridPosition[1], gridPosition[2]); } }
        public Vector3Int Orientation { get { return new Vector3Int(orientation[0], orientation[1], orientation[2]); } }
        public string actualType;
        public int id;
        public int[] gridPosition;
        public int[] orientation;

        public BaseCharacterData(int id, Vector3Int gridPosition, Vector3Int orientation)
        {
            this.id = id;
            this.gridPosition = new int[] { gridPosition.x, gridPosition.y, gridPosition.z };
            this.orientation = new int[] { orientation.x, orientation.y, orientation.z };
        }

        public virtual void SetData(int id, Vector3Int gridPosition, Vector3Int orientation)
        {
            this.id = id;
            this.gridPosition = new int[] { gridPosition.x, gridPosition.y, gridPosition.z };
            this.orientation = new int[] { orientation.x, orientation.y, orientation.z };
        }

        public virtual void LoadFrom(object data)
        {
            BaseCharacterData baseCharacterData = data as BaseCharacterData;
            id = baseCharacterData.id;
            gridPosition = baseCharacterData.gridPosition;
            orientation = baseCharacterData.orientation;
            SetIntArray(gridPosition, baseCharacterData.gridPosition);
            SetIntArray(orientation, baseCharacterData.orientation);
        }

        public virtual void SaveTo(object data)
        {
            BaseCharacterData baseCharacterData = data as BaseCharacterData;
            baseCharacterData.id = id;
            baseCharacterData.gridPosition = gridPosition;
            baseCharacterData.orientation = orientation;
            SetIntArray(baseCharacterData.gridPosition, gridPosition);
            SetIntArray(baseCharacterData.orientation, orientation);
        }

        public void SetIntArray(int[] srcArray, int[] destArray)
        {
            System.Array.Copy(srcArray, destArray, srcArray.Length);
        }
    }
}
