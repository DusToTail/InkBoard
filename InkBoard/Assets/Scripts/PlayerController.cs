using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private BaseCharacter m_Character;

    private void Update()
    {
        if(m_Character == null) { return; }

        if (Input.GetKeyDown(KeyCode.A))
        {
            m_Character.ExecuteCommand(KeyCode.A);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            m_Character.ExecuteCommand(KeyCode.D);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            m_Character.ExecuteCommand(KeyCode.W);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            m_Character.ExecuteCommand(KeyCode.S);
        }
    }

    public void Init(BaseCharacter character)
    {
        m_Character = character;
    }
}

public class InputCommand
{
    public InputCommand(KeyCode keyCode, Func<object, bool> action)
    {
        m_KeyCode = keyCode;
        m_Action = action;
    }
    public KeyCode KeyCode { get { return m_KeyCode; } }
    public Func<object, bool> Action { get { return m_Action; } }
    private KeyCode m_KeyCode;
    private Func<object, bool> m_Action;

}
