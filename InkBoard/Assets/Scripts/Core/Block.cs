using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private int blockID;

    public int ID { get { return blockID; } }

    public void Init()
    {
    }
}
