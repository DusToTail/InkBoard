using UnityEngine;

public class CubeCharacter : BaseCharacter
{
    private CubeMovement m_Movement;
    private CubeData m_CubeData;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            GetAndSetMovement(GridPosition, GridPosition + Direction.Left);
            GameManager.Instance.RegisterAction(this, (x) =>
            {
                return x.SendMoveRequest();
            });
            GameManager.Instance.ProcessTurn();
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            GetAndSetMovement(GridPosition, GridPosition + Direction.Right);
            GameManager.Instance.RegisterAction(this, (x) =>
            {
                return x.SendMoveRequest();
            });
            GameManager.Instance.ProcessTurn();
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            GetAndSetMovement(GridPosition, GridPosition + Direction.Front);
            GameManager.Instance.RegisterAction(this, (x) =>
            {
                return x.SendMoveRequest();
            });
            GameManager.Instance.ProcessTurn();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            GetAndSetMovement(GridPosition, GridPosition + Direction.Back);
            GameManager.Instance.RegisterAction(this, (x) =>
            {
                return x.SendMoveRequest();
            });
            GameManager.Instance.ProcessTurn();
        }
    }

    public override void Init(object data)
    {
        if (data == null) 
        {
            Debug.LogError("CubeCharacter: Fail to initialize! Data is null");
            return;
        }
        // Basic info
        var baseData = data as BaseCharacterData;
        base.Init(baseData);
        // Cube specific
        Debug.Log("CubeCharacter: Init");
        m_CubeData = data as CubeData;
        m_Movement = new CubeMovement(this);
    }
    public override bool DefaultAction()
    {
        Debug.Log("CubeCharacter: DefaultAction");
        return true;
    }
    public override bool SendMoveRequest()
    {
        Debug.Log("CubeCharacter: SendMoveRequest");
        GameManager.Instance.Request(m_Movement);
        return true;
    }
    
    private Movement GetAndSetMovement(Vector3Int from, Vector3Int to)
    {
        if(m_Movement == null) { Debug.LogError("Movement is not initialized!", this); }
        m_Movement.SetProperties(from, to);
        return m_Movement;
    }

    public class CubeMovement : Movement
    {
        public CubeMovement(CubeCharacter cube)
        {
            m_Cube = cube;
            m_From = Vector3Int.zero;
            m_To = Vector3Int.zero;
        }
        public CubeMovement(CubeCharacter cube, Vector3Int from, Vector3Int to)
        {
            m_Cube = cube;
            m_From = from;
            m_To = to;

        }
        ~CubeMovement()
        {
        }
        public override bool Execute()
        {
            m_Cube.SetGridPosition(m_To);
            m_Cube.transform.position = Board.Instance.GetWorldPositionAt(m_To);
            return true;
        }
        public override bool IsValid()
        {
            // Need to refer to other characters, the board, etc
            return true;
        }
        public void SetProperties(Vector3Int from, Vector3Int to)
        {
            m_From = from;
            m_To = to;
        }
        public Vector3Int From { get { return m_From; } }
        public Vector3Int To { get { return m_To; } }

        private Vector3Int m_From;
        private Vector3Int m_To;
        private CubeCharacter m_Cube;
    }

    [System.Serializable]
    public class CubeData : BaseCharacterData
    {
        public CubeData(int id, Vector3Int gridPosition, Vector3Int orientation)
            : base(id, gridPosition, orientation)
        {
        }

        public override void SetData(int id, Vector3Int gridPosition, Vector3Int orientation)
        {
            base.SetData(id, gridPosition, orientation);
        }

        public override void LoadFrom(object data)
        {
            CubeData cubeData = data as CubeData;
            base.LoadFrom(cubeData);
        }

        public override void SaveTo(object data)
        {
            CubeData cubeData = data as CubeData;
            base.SaveTo(cubeData);
        }
    }
}


