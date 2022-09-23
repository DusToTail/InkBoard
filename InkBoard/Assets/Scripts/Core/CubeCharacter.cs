using UnityEngine;

public class CubeCharacter : BaseCharacter
{
    private CubeMovement m_Movement;
    private CubeData m_CubeData;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            Move(Direction.Left);
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            Move(Direction.Right);
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            Move(Direction.Front);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Direction.Back);
        }
    }

    public override void Init(object data)
    {
        if (m_CubeData == null) { return; }
        // Basic info
        base.Init(data);
        // Cube specific
        m_Movement = new CubeMovement(this);
    }
    public void Move(Direction dir)
    {
        var movement = GetAndSetMovement(gridPosition, gridPosition + dir);
        manager.RequestMovement(
            (Movement x) =>
            {
                return movement.Move();
            },
            (Movement x) =>
            {
                return movement.IsValid();
            }
            );
    }
    
    private Movement GetAndSetMovement(Vector3Int from, Vector3Int to)
    {
        m_Movement.SetMovement(from, to);
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
        public override bool Move()
        {
            m_Cube.SetGridPosition(m_To);
            return true;
        }
        public override bool IsValid()
        {
            // Need to refer to other characters, the board, etc
            return true;
        }
        public void SetMovement(Vector3Int from, Vector3Int to)
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


