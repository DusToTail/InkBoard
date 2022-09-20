using UnityEngine;

public class Block : MonoBehaviour
{
    public Cell<Block> Cell { get { return m_Cell; } }
    private Cell<Block> m_Cell;

    public virtual void Init(Cell<Block> cell)
    {
        m_Cell = cell;
    }
}
