using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float InputDuration { get; private set; }
    public float CalculationDuration { get; private set; }
    public float ExecutionDuration { get; private set; }
    public float CleanDuration { get; private set; }

    public RythmController RythmController { get { return rythmController; } }
    public TrackManager TrackManager { get { return trackManager; } }
    public TurnController<BaseCharacter> TurnController { get { return m_TurnController; } }
    public RequestHandler<Movement> MovementHandler { get { return m_MovementHandler; } }

    [SerializeField] private TrackManager trackManager;
    [SerializeField] private RythmController rythmController;
    [SerializeField] private PlayerController playerController;
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

        string block1 = @"{""actualType"": ""Block.BlockData"", ""id"": 0, ""gridPosition"": [0, 0, 0], ""front_up_right"": [""Front"", ""Up"", ""Right""]}";
        string block2 = @"{""actualType"": ""Block.BlockData"", ""id"": 0, ""gridPosition"": [1, 0, 0], ""front_up_right"": [""Front"", ""Up"", ""Right""]}";
        string block3 = @"{""actualType"": ""Block.BlockData"", ""id"": 0, ""gridPosition"": [2, 0, 0], ""front_up_right"": [""Front"", ""Up"", ""Right""]}";
        string block4 = @"{""actualType"": ""Block.BlockData"", ""id"": 0, ""gridPosition"": [1, 1, 0], ""front_up_right"": [""Front"", ""Up"", ""Right""]}";
        string block5 = @"{""actualType"": ""Block.BlockData"", ""id"": 0, ""gridPosition"": [0, 0, 1], ""front_up_right"": [""Front"", ""Up"", ""Right""]}";
        string block6 = @"{""actualType"": ""Block.BlockData"", ""id"": 0, ""gridPosition"": [1, 0, 1], ""front_up_right"": [""Front"", ""Up"", ""Right""]}";
        string block7 = @"{""actualType"": ""Block.BlockData"", ""id"": 0, ""gridPosition"": [2, 0, 1], ""front_up_right"": [""Front"", ""Up"", ""Right""]}";
        string block8 = @"{""actualType"": ""Block.BlockData"", ""id"": 0, ""gridPosition"": [0, 0, 2], ""front_up_right"": [""Front"", ""Up"", ""Right""]}";
        string block9 = @"{""actualType"": ""Block.BlockData"", ""id"": 0, ""gridPosition"": [1, 0, 2], ""front_up_right"": [""Front"", ""Up"", ""Right""]}";
        string block10 = @"{""actualType"": ""Block.BlockData"", ""id"": 0, ""gridPosition"": [2, 0, 2], ""front_up_right"": [""Front"", ""Up"", ""Right""]}";
        string block11 = @"{""actualType"": ""Block.BlockData"", ""id"": 0, ""gridPosition"": [2, 1, 2], ""front_up_right"": [""Front"", ""Up"", ""Right""]}";



        string noBlockData = @"{""actualType"": ""Block.BlockData"", ""id"": -1, ""gridPosition"": [-1, -1, -1], ""front_up_right"": [""None"", ""None"", ""None""]}";
        string boardData = @"
{
""betweenDistance"": 1,
""width"": 3,
""length"": 3,
""height"": 2, 
""blockDatas"": [
" + block1 + @",
" + block2 + @",
" + block3 + @",
" + noBlockData + @",
" + block4 + @",
" + noBlockData + @",
" + block5 + @",
" + block6 + @",
" + block7 + @",
" + noBlockData + @",
" + noBlockData + @",
" + noBlockData + @",
" + block8 + @",
" + block9 + @",
" + block10 + @",
" + noBlockData + @",
" + noBlockData + @",
" + block11 + @"
]}";
        Board.Instance.Init(boardData);

        string cubeData = @"{""actualType"": ""CubeCharacter.CubeData"", ""id"": 0, ""gridPosition"": [0, 1, 0], ""front_up_right"": [""Front"", ""Up"", ""Right""]}";
        string noCharacterData = @"{""actualType"": ""BaseCharacter.BaseCharacterData"", ""id"": -1, ""gridPosition"": [-1, -1, -1], ""front_up_right"": [""None"", ""None"", ""None""]}";
        string[] example = new string[Board.Instance.Layout.GetTotalCount()];
        System.Array.Fill(example, noCharacterData);
        example.SetValue(cubeData, 0);
        string concat = string.Join('\n',example);
        CharacterManager.Instance.Init(concat);
        playerController.Init(CharacterManager.Instance.GetCharacter(0));
        trackManager.Init();
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
        var status = m_TurnController.RegisterAction(playerObject, (x)=>
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
    public void StackNewRequestAt<T,U>(T targetChange, U stackChange) where T : Change where U : Change
    {
        if (typeof(T).IsSubclassOf(typeof(Movement)) && typeof(U).IsSubclassOf(typeof(Movement)))
        {
            var target = targetChange as Movement;
            var stack = stackChange as Movement;
            m_MovementHandler.StackNewRequestAt(
                target,
                (x) =>
                {
                    return stack.Execute();
                },
                (x) =>
                {
                    return stack.IsValid();
                });
        }
    }
    public void StartPlay()
    {
        rythmController.IsPlaying = true;
        var track = trackManager.CurrentTrack;
        track.Source.pitch = rythmController.playbackSpeed;
        track.Source.loop = rythmController.isLooped;
        StartCoroutine(PlaybackCoroutine());
        StartCoroutine(PlayAudioCoroutine(trackManager.CurrentTrack.Source, rythmController.trackOffsetInSeconds));
    }
    public void StopPlay()
    {
        rythmController.IsPlaying = false;
        StopAllCoroutines();
        trackManager.CurrentTrack.Source.Stop();
        trackManager.CurrentTrack.ResetIndex();
    }
    private IEnumerator PlaybackCoroutine()
    {
        var track = trackManager.CurrentTrack;
        track.ResetIndex();
        var startTime = Time.time;
        var anchorTimeStamp = Time.time;
        while (true)
        {
            float turnTime = Time.time;
            if (track.CurrentBeat == track.PreviousBeat) 
            {
                if(rythmController.isLooped)
                    track.ResetIndex();
                else
                {
                    //Debug.Log("Supposed End Time: " + (startTime + track.DurationInMilliSeconds));
                    //Debug.Log("Actual End Time: " + (Time.time - startTime));
                    yield break;
                }
            }
            float intervalInSeconds = (float)track.CurrentBeat.IntervalInMilliseconds / (rythmController.playbackSpeed * 1000);
            anchorTimeStamp += intervalInSeconds;
            yield return StartCoroutine(ProcessTurnCoroutine(anchorTimeStamp - Time.time));
            track.IncrementBeatIndex();

            //Debug.Log("Supposed Time Per Turn: " + intervalInSeconds);
            //Debug.Log("Actual Time Per Turn: " + (Time.time - turnTime));
        }

        
    }
    private IEnumerator ProcessTurnCoroutine(float timeInSeconds)
    {
        var startTurnTime = Time.time;

        var beatLayout = rythmController.BeatLayout;
        InputDuration = beatLayout.GetDuration("Input", true) * timeInSeconds;
        // Allow input until the end of the duration
        playerController.ResetInput();
        playerController.CanControl = true;
        yield return new WaitForSeconds(InputDuration);
        playerController.CanControl = false;

        CalculationDuration = beatLayout.GetDuration("Calculation", true) * timeInSeconds;
        // Each turn will contain a list of <Player, Action> values. There are no duplicates
        // Each action here will be a request to the corresponding RequestHandler to make changes
        m_TurnController.ProcessTurn();
        yield return new WaitForSeconds(CalculationDuration);

        ExecutionDuration = beatLayout.GetDuration("Execution", true) * timeInSeconds;
        // Process requests after the request list has been populated in TurnController
        ProcessRequests<Movement>();
        yield return new WaitForSeconds(ExecutionDuration);

        CleanDuration = beatLayout.GetDuration("Clean", true) * timeInSeconds;
#if DEBUG
        m_TurnController.DebugLog(TurnController<BaseCharacter>.DEBUG_INFO.CURRENT_TURN);
        Debug.Log(playerController.InputEvaluation);
#endif
        m_TurnController.ResetToDefaultActions();
        yield return new WaitForSeconds(CleanDuration);
    }
    private IEnumerator PlayAudioCoroutine(AudioSource source, float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        source.Play();
    }
    private void ProcessRequests<T>() where T : Change
    {
        if (typeof(T) == typeof(Movement))
        {
            m_MovementHandler.ProcessRequests(true);
        }
    }

    private void OnGUI()
    {
        if (!rythmController.IsPlaying) { return; }
        var track = trackManager.CurrentTrack;
        if (track == null) { return; }
        float timeStamp = (float)track.CurrentBeat.TimeStampInMilliseconds;
        float normalizedTimeStamp = timeStamp / (float)track.DurationInMilliSeconds;
        float x = normalizedTimeStamp * Screen.width;
        float y = 0.9f * Screen.height;
        float width = Screen.width / track.GetBeats().Length;
        float height = 0.2f * Screen.height;
        Rect rect = new Rect(x, y, width, height);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
    }
}
