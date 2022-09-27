using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private TrackManager trackManager;
    [SerializeField] private RythmController rythmController;
    private TurnController<BaseCharacter> m_TurnController;
    private RequestHandler<Movement> m_MovementHandler;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    public void Init()
    {
        m_TurnController = new TurnController<BaseCharacter>((x) => { return x.DefaultAction(); });
        m_MovementHandler = new RequestHandler<Movement>();

        string boardData = @"
{
""betweenDistance"": 1,
""width"": 3,
""length"": 3,
""height"": 2, 
""blockIDs"": [  0,  0,  0, -1, -1, -1,  0,  0,  0, 
                -1, -1, -1,  0,  0,  0, -1, -1, -1  ]
}";
        Board.Instance.Init(boardData);

        string cubeData = @"{""actualType"": ""CubeCharacter.CubeData"", ""id"": 0, ""gridPosition"": [0, 1, 0], ""front_up_right"": [""Front"", ""Up"", ""Right""]}";
        string noCharacterData = @"{""actualType"": ""BaseCharacter.BaseCharacterData"", ""id"": -1, ""gridPosition"": [-1, -1, -1], ""front_up_right"": [""None"", ""None"", ""None""]}";
        string[] example = new string[Board.Instance.Layout.GetTotalCount()];
        System.Array.Fill(example, noCharacterData);
        example.SetValue(cubeData, 0);
        string concat = string.Join('\n',example);
        CharacterManager.Instance.Init(concat);

        trackManager.Init();
    }
    public void ProcessTurn()
    {
        // Each turn will contain a list of <Player, Action> values. There are no duplicates
        // Each action here will be a request to the corresponding RequestHandler to make changes
        m_TurnController.ProcessTurn();
        m_TurnController.DebugLog(TurnController<BaseCharacter>.DEBUG_INFO.TURNS);
        // Process requests after the request list has been populated in TurnController
        ProcessRequests<Movement>();
    }
    public void RegisterPlayer<T>(T playerObject, Func<T, bool> playerDefaultAction, string actionName) where T : BaseCharacter
    {
        var status = m_TurnController.RegisterPlayer(playerObject, (x) =>
        {
            return playerDefaultAction.Invoke(x as T);
        },
        actionName);
        if(status == TurnController<BaseCharacter>.REGISTER_STATUS.FAIL) 
        {
            Debug.Log("Player Registration: FAIL", this);
            Debug.Log($"{playerObject.gameObject} is already registered!", playerObject);
        }
        else if(status == TurnController<BaseCharacter>.REGISTER_STATUS.SUCCESS)
        {
            Debug.Log("Player Registration: SUCCESS", this);
            Debug.Log($"{playerObject.gameObject} registered successfully.", playerObject);
        }
    }
    public void RegisterAction<T>(T playerObject, Func<T, bool> action, string actionName) where T : BaseCharacter
    {
        m_TurnController.RegisterAction(playerObject, (x)=>
        {
            return action.Invoke(x as T);
        },
        actionName);
    }
    public void Request<T>(T change) where T : Change
    {
        if(typeof(T).IsSubclassOf(typeof(Movement)))
        {
            var moveChange = change as Movement;
            m_MovementHandler.AddNewRequest(
                moveChange,
                (x)=>
                {
                    return x.Execute();
                },
                (x)=>
                {
                    return x.IsValid();
                });
        }
    }
    public void StackNewRequestAt<T>(T targetMovement, Func<T, bool> action, Func<T, bool> validCheck) where T : Change
    {
        if (typeof(T).IsSubclassOf(typeof(Movement)))
        {
            var target = targetMovement as Movement;
            var moveAction = action as Func<Movement, bool>;
            var moveValidCheck = validCheck as Func<Movement, bool>;
            m_MovementHandler.StackNewRequestAt(target, moveAction, moveValidCheck);
        }
    }
    private void ProcessRequests<T>() where T : Change
    {
        if (typeof(T) == typeof(Movement))
        {
            m_MovementHandler.ProcessRequests(true);
        }
    }
}
