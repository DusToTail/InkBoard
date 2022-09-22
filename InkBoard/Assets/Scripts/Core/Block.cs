using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] protected int id;

    public int ID { get { return id; } }

    public virtual void Init()
    {
    }
}
