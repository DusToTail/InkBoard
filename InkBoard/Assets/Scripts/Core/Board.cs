using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Vector3Int gridLayout;
    private MyGrid m_Grid;

    private void Start()
    {
        m_Grid = new MyGrid(new GridLayout((uint)gridLayout.x, (uint)gridLayout.y, (uint)gridLayout.z));
        m_Grid.DebugLog();
    }
}
