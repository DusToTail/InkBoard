using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerInputLayout InputLayout { get { return m_InputLayout; } }
    public bool CanControl { get; set; }
    public string InputEvaluation { get; set; }

    [SerializeField] private BaseCharacter m_Character;
    private PlayerInputLayout m_InputLayout;
    private Coroutine m_InputEvaluationCoroutine;

    private void Start()
    {
        m_InputLayout = new PlayerInputLayout();

        // outside would not be recored
        m_InputLayout.PushBack("Early", 0.15f);
        m_InputLayout.PushBack("Perfect", 0.7f);
        m_InputLayout.PushBack("Late", 0.15f);
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
        float earlyTime = InputLayout.GetNormalizedDuration("Early") * GameManager.Instance.InputDuration;
        float perfectTime = earlyTime + InputLayout.GetNormalizedDuration("Perfect") * GameManager.Instance.InputDuration;
        //float lateTime = perfectTime + InputLayout.GetNormalizedDuration("Late") * GameManager.Instance.InputDuration;

        while (timer <= GameManager.Instance.InputDuration)
        {
            yield return null;
            if (CanControl)
            {
                InputEvaluation = "Missed";
                if (Input.GetKeyDown(KeyCode.A))
                {
                    m_Character.ExecuteCommand(KeyCode.A);
                    CanControl = false;
                    Evaluate();
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    m_Character.ExecuteCommand(KeyCode.D);
                    CanControl = false;
                    Evaluate();
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    m_Character.ExecuteCommand(KeyCode.W);
                    CanControl = false;
                    Evaluate();
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    m_Character.ExecuteCommand(KeyCode.S);
                    CanControl = false;
                    Evaluate();
                }

                void Evaluate()
                {
                    if (timer < earlyTime)
                        InputEvaluation = "Early";
                    else if (timer < perfectTime)
                        InputEvaluation = "Perfect";
                    else
                        InputEvaluation = "Late";
                }
            }
            timer += Time.deltaTime;
        }
    }
}

public class PlayerInputLayout
{
    public PlayerInputLayout()
    {
        m_Sections = new List<Section>();
    }
    ~PlayerInputLayout()
    {
    }
    public float GetNormalizedDuration(string name)
    {
        var section = m_Sections.Find(x => x.Name == name);
        if (section == null) { return 0; }
        return section.NormalizedDuration;
    }
    public void PushBack(string name, float normalizedDuration)
    {
        m_Sections.Add(new Section(name, normalizedDuration));
        m_TotalDuration += normalizedDuration;
        Recalculate();
    }
    public void Remove(string name)
    {
        var section = m_Sections.Find(x => x.Name == name);
        if (section == null) { return; }
        m_Sections.Remove(section);
        m_TotalDuration -= section.NormalizedDuration;
        Recalculate();
    }
    public void Recalculate()
    {
        foreach (var section in m_Sections)
        {
            section.NormalizedDuration /= m_TotalDuration;
        }
        m_TotalDuration = 1;
    }

    public Section[] Sections { get { return m_Sections.ToArray(); } }
    private List<Section> m_Sections;
    private float m_TotalDuration;

    public class Section
    {
        public Section(string name, float normalizedDuration)
        {
            Name = name;
            NormalizedDuration = normalizedDuration;
        }
        ~Section()
        {
        }
        public string Name { get; set; }
        public float NormalizedDuration { get; set; }
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
