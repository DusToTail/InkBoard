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
    private Coroutine m_InputEvaluationCoroutine;

    private void Start()
    {
        InputLayout = new TimeLayout("PlayerInputLayout");

        // outside would not be recored
        InputLayout.PushBack("Early", 0.15f);
        InputLayout.PushBack("Perfect", 0.7f);
        InputLayout.PushBack("Late", 0.15f);
    }
    public void Init(BaseCharacter character)
    {
        m_Character = character;
    }
    public void StartInputEvaluation()
    {
        if (m_InputEvaluationCoroutine != null)
            StopCoroutine(m_InputEvaluationCoroutine);
        m_InputEvaluationCoroutine = StartCoroutine(InputEvaluationCoroutine());
    }
    public void StopInputEvaluation()
    {
        if (m_InputEvaluationCoroutine != null)
            StopCoroutine(m_InputEvaluationCoroutine);
    }
    private IEnumerator InputEvaluationCoroutine()
    {
        float timer = 0;
        float normalizedTimer = 0;
        while (timer <= GameManager.Instance.InputDuration)
        {
            yield return null;
            if (CanControl)
            {
                InputEvaluation = "Missed";
                if (Input.GetKeyDown(KeyCode.A))
                {
                    m_Character.ExecuteCommand(KeyCode.A);
                    TimeEvaluation.Evaluate(InputLayout, normalizedTimer, true);
                    CanControl = false;
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    m_Character.ExecuteCommand(KeyCode.D);
                    TimeEvaluation.Evaluate(InputLayout, normalizedTimer, true);
                    CanControl = false;
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    m_Character.ExecuteCommand(KeyCode.W);
                    TimeEvaluation.Evaluate(InputLayout, normalizedTimer, true);
                    CanControl = false;
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    m_Character.ExecuteCommand(KeyCode.S);
                    TimeEvaluation.Evaluate(InputLayout, normalizedTimer, true);
                    CanControl = false;
                }
            }
            timer += Time.deltaTime;
            normalizedTimer = timer / GameManager.Instance.InputDuration;
        }
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
