using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Configurations")]
    [SerializeField] private GameObject defaultBlock;
    [SerializeField] private float betweenDistance;
    [SerializeField] private Vector3Int gridLayout;
    [Header("Gizmos")]
    [SerializeField] private Color color;
    [SerializeField] private bool displayGizmos;
    private MyGrid<Block> m_Grid;

    private void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        m_Grid = new MyGrid<Block>(new GridLayout((uint)gridLayout.x, (uint)gridLayout.y, (uint)gridLayout.z));
        m_Grid.ForEach(CreateBlockAtCell);
#if DEBUG
        m_Grid.DebugLog();
#endif
    }

    private void CreateBlockAtCell(Cell<Block> cell)
    {
        Vector3Int gridPosition = cell.GetGridPositionVec3();
        Vector3 localPosition = (Vector3)gridPosition * betweenDistance;
        var blockObj = Instantiate(defaultBlock, transform.position + localPosition, Quaternion.identity, transform);
        var block = blockObj.GetComponent<Block>();
        block.Init();
        cell.SetValue(block);
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

}
