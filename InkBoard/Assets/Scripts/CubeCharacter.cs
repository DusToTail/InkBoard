using UnityEngine;
using System.Collections;

public class CubeCharacter : BaseCharacter
{
    public float SecondsPerRoll { get { return secondsPerRoll; } }
    [SerializeField] private float secondsPerRoll;

    private Vector3Int[] m_Orientation;
    private CubeMovement m_Movement;
    private CubeData m_CubeData;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GetAndSetMovement(GridPosition, GridPosition + Direction.Left);
            GameManager.Instance.RegisterAction(this, Move, "Move Left");
            GameManager.Instance.ProcessTurn();
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            GetAndSetMovement(GridPosition, GridPosition + Direction.Right);
            GameManager.Instance.RegisterAction(this, Move, "Move Right");
            GameManager.Instance.ProcessTurn();
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            GetAndSetMovement(GridPosition, GridPosition + Direction.Front);
            GameManager.Instance.RegisterAction(this, Move, "Move Forward");
            GameManager.Instance.ProcessTurn();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            GetAndSetMovement(GridPosition, GridPosition + Direction.Back);
            GameManager.Instance.RegisterAction(this, Move, "Move Back");
            GameManager.Instance.ProcessTurn();
        }

        bool Move(CubeCharacter cube)
        {
            return cube.SendMoveRequest();
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
        m_Orientation = new Vector3Int[3];
        SetOrientation(m_CubeData.Right, m_CubeData.Up, m_CubeData.Front);
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
    public void SetOrientation(Vector3Int right, Vector3Int up, Vector3Int forward)
    {
        m_Orientation[0] = right;
        m_Orientation[1] = up;
        m_Orientation[2] = forward;
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
            if(m_Coroutine != null) { m_Cube.StopCoroutine(m_Coroutine); }
            m_Coroutine = m_Cube.StartCoroutine(RollCoroutine());
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
        private IEnumerator RollCoroutine()
        {
            var dir = Direction.GetDirection(m_To - m_From);
            var axis = Vector3.Cross(Direction.Up, dir);
            var anchor = ((Board.Instance.GetWorldPositionAt(m_From) + Board.Instance.GetWorldPositionAt(m_To)) / 2) + (Vector3)Direction.Down * 0.5f;
            float timer = 0;
            while(timer < m_Cube.SecondsPerRoll)
            {
                yield return null;
                m_Cube.transform.RotateAround(anchor, axis, 90f * Time.deltaTime / m_Cube.SecondsPerRoll);
                timer += Time.deltaTime;
            }

            m_Cube.SetGridPosition(m_To);
            m_Cube.SetPosition(Board.Instance.GetWorldPositionAt(m_To));

            Quaternion rotation = Quaternion.AngleAxis(90f, axis);
            Vector3Int right = Vector3Int.RoundToInt(rotation * m_Cube.m_Orientation[0]);
            Vector3Int up = Vector3Int.RoundToInt(rotation * m_Cube.m_Orientation[1]);
            Vector3Int forward = Vector3Int.RoundToInt(rotation * m_Cube.m_Orientation[2]);
            m_Cube.SetRotation(forward, up);
            m_Cube.SetOrientation(right, up, forward);
        }

        public Vector3Int From { get { return m_From; } }
        public Vector3Int To { get { return m_To; } }

        private Vector3Int m_From;
        private Vector3Int m_To;
        private Coroutine m_Coroutine;
        private CubeCharacter m_Cube;
    }

    [System.Serializable]
    public class CubeData : BaseCharacterData
    {
        public CubeData(int id, Vector3Int gridPosition, string[] front_up_right)
            : base(id, gridPosition, front_up_right)
        {
        }

        public override void SetData(int id, Vector3Int gridPosition, string[] front_up_right)
        {
            base.SetData(id, gridPosition, front_up_right);
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

