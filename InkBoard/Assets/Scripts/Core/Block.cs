using UnityEngine;

public class Block : MonoBehaviour
{
    public Cell Cell { get { return m_Cell; } }
    private Cell m_Cell;

    public virtual void Init(Cell cell)
    {
        m_Cell = cell;
    }
}
