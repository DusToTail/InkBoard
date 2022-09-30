using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool CanControl { get; set; }
    public string InputEvaluation { get; set; }
    public TimeLayout InputLayout { get; private set; }

    [SerializeField] private BaseCharacter m_Character;
    private float m_Timer = 0;
    private float m_NormalizedTimer = 0;

    private void Start()
    {
        InputLayout = new TimeLayout("PlayerInputLayout");

        // outside would not be recored
        InputLayout.PushBack("Early", 0.15f);
        InputLayout.PushBack("Perfect", 0.7f);
        InputLayout.PushBack("Late", 0.15f);
    }
    private void Update()
    {
        if(m_Character == null || !CanControl) { return; }

        if (Input.GetKeyDown(KeyCode.A))
        {
            m_Character.ExecuteCommand(KeyCode.A);
            InputEvaluation = TimeEvaluation.Evaluate(InputLayout, m_NormalizedTimer, true);
            CanControl = false;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            m_Character.ExecuteCommand(KeyCode.D);
            InputEvaluation = TimeEvaluation.Evaluate(InputLayout, m_NormalizedTimer, true);
            CanControl = false;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            m_Character.ExecuteCommand(KeyCode.W);
            InputEvaluation = TimeEvaluation.Evaluate(InputLayout, m_NormalizedTimer, true);
            CanControl = false;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            m_Character.ExecuteCommand(KeyCode.S);
            InputEvaluation = TimeEvaluation.Evaluate(InputLayout, m_NormalizedTimer, true);
            CanControl = false;
        }
        m_Timer += Time.deltaTime;
        m_NormalizedTimer = m_Timer / GameManager.Instance.InputDuration;
    }
    public void Init(BaseCharacter character)
    {
        m_Character = character;
    }
    public void ResetInput()
    {
        InputEvaluation = "Missed";
        m_Timer = 0;
        m_NormalizedTimer = 0;
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
