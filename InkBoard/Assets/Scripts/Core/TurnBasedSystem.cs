using System.Collections.Generic;
using System;
using UnityEngine;

public class TurnController<T> where T : class
{
    public enum REGISTER_STATUS : int
    {
        SUCCESS,
        FAIL
    }
    public enum DEBUG_INFO : int
    {
        TURNS,
        ACTIONS,
        PLAYERS
    }
    public TurnController(Func<T, bool> defaultAction)
    {
        m_Players = new List<Player>();
        m_AttachedActions = new List<AttachedAction>();
        m_Turns = new List<Turn>();
        m_DefaultAction = defaultAction;
        m_PlayerIDCount = 0;
        m_TurnIndex = 0;
    }
    ~TurnController()
    {
    }

    public void ProcessTurn()
    {
        Turn turn = new Turn(m_TurnIndex);
        for(int i = 0; i < m_AttachedActions.Count; i++)
        {
            AttachedAction action = m_AttachedActions[i];
            action.Execute();

            turn.RegisterAction(action);
        }
        m_Turns.Add(turn);
        m_TurnIndex++;
    }
    public REGISTER_STATUS RegisterPlayer(T playerObject, Func<T, bool> playerDefaultAction)
    {
        if (PlayerIsRegistered(playerObject)) { return REGISTER_STATUS.FAIL; }
        Player newPlayer = new Player(m_PlayerIDCount++, playerObject);
        Func<T, bool> useDefaultAction;
        if (playerDefaultAction != null)
            useDefaultAction = playerDefaultAction;
        else
            useDefaultAction = m_DefaultAction;
        var attachedTurn = new AttachedAction(newPlayer, useDefaultAction);

        m_Players.Add(newPlayer);
        m_AttachedActions.Add(attachedTurn);
        return REGISTER_STATUS.SUCCESS;
    }
    public REGISTER_STATUS RegisterAction(T playerObject, Func<T, bool> action)
    {
        if(!PlayerIsRegistered(playerObject)) { return REGISTER_STATUS.FAIL; }
        var perPlayerAction = GetAttachedAction(playerObject);
        if (perPlayerAction == null) { return REGISTER_STATUS.FAIL; }

        perPlayerAction.SetAction(action);
        return REGISTER_STATUS.SUCCESS;
    }
    public REGISTER_STATUS RegisterAction(int playerID, Func<T, bool> action)
    {
        if (!PlayerIsRegistered(playerID)) { return REGISTER_STATUS.FAIL; }
        var perPlayerAction = GetAttachedAction(playerID);
        if (perPlayerAction == null) { return REGISTER_STATUS.FAIL; }

        perPlayerAction.SetAction(action);
        return REGISTER_STATUS.SUCCESS;
    }
#if DEBUG
    public void DebugLog(DEBUG_INFO info)
    {
        switch(info)
        {
            case DEBUG_INFO.TURNS:
                {
                    Debug.Log("**********TURNS REGISTERED**********");
                    for (int i = 0; i < m_Turns.Count; i++)
                    {
                        Debug.Log(m_Turns[i].ToString());
                    }
                    break;
                }
            case DEBUG_INFO.ACTIONS:
                {
                    Debug.Log("**********ACTIONS REGISTERED**********");
                    for (int i = 0; i < m_AttachedActions.Count; i++)
                    {
                        Debug.Log(m_AttachedActions[i].ToString());
                    }
                    break;
                }
            case DEBUG_INFO.PLAYERS:
                {
                    Debug.Log("**********PLAYERS REGISTERED**********");
                    for (int i = 0; i < m_Players.Count; i++)
                    {
                        Debug.Log(m_Players[i].ToString());
                    }
                    break;
                }
            default:
                break;
        }
    }
#endif
    private AttachedAction GetAttachedAction(T playerObject)
    {
        return m_AttachedActions.Find(x => x.Player.Object == playerObject);
    }
    private AttachedAction GetAttachedAction(int id)
    {
        return m_AttachedActions.Find(x => x.Player.ID == id);
    }
    private bool PlayerIsRegistered(T player)
    {
        var findResult = m_Players.Find(x => x.Object == player);
        if(findResult != null) { return true; }
        return false;
    }
    private bool PlayerIsRegistered(int id)
    {
        var findResult = m_Players.Find(x => x.ID == id);
        if (findResult != null) { return true; }
        return false;
    }

    public int ActionCount { get { return m_AttachedActions == null ? -1 : m_AttachedActions.Count; } }
    public int TurnIndex { get { return m_TurnIndex; } }

    private List<Player> m_Players;
    private List<AttachedAction> m_AttachedActions;
    private List<Turn> m_Turns;
    private Func<T, bool> m_DefaultAction;
    private int m_PlayerIDCount;
    private int m_TurnIndex;

    private class Turn
    {
        public Turn(int index)
        {
            m_ExecutedActions = new List<AttachedAction>();
            m_Index = index;
        }
        ~Turn()
        {
        }
        public void RegisterAction(AttachedAction action)
        {
            m_ExecutedActions.Add(action);
        }
        public override string ToString()
        {
            string result = $"*** TURN {m_Index} ***\n";
            for(int i = 0; i < m_ExecutedActions.Count; i++)
            {
                var action = m_ExecutedActions[i];
                result += $"{action.ToString()}\n";
            }
            return result;
        }

        public int Index { get { return m_Index; } }

        private List<AttachedAction> m_ExecutedActions;
        private int m_Index;
    }

    private class AttachedAction
    {
        public enum STATUS
        {
            SUCCESS,
            FAILED,
            PENDING
        }
        public AttachedAction(Player player, Func<T, bool> action)
        {
            m_Player = player;
            m_Action = action;
            m_Status = STATUS.PENDING;

        }
        ~AttachedAction()
        {
        }

        public void Execute()
        {
            bool success = m_Action.Invoke(Player.Object);
            m_Status = success ? STATUS.SUCCESS : STATUS.FAILED;
        }
        public void SetAction(Func<T, bool> action)
        {
            m_Action = action;
            m_Status = STATUS.PENDING;
        }
        public Player Player { get { return m_Player; } }
        public Func<T, bool> Action { get { return m_Action; } }
        public STATUS Status { get { return m_Status; } }
        public override string ToString()
        {
            return String.Format("Player: {0} - Action: {1} - Status: {2}", m_Player, m_Action.Method.Name, m_Status);
        }
        private Player m_Player;
        private Func<T, bool> m_Action;
        private STATUS m_Status;
    }

    private class Player
    {
        public Player(int id, T player)
        {
            m_ID = id;
            m_Object = player;
        }
        ~Player()
        {
        }
        public override string ToString()
        {
            return String.Format("{0}({1})", m_Object, m_ID);
        }
        public int ID { get { return m_ID; } }
        public T Object { get { return m_Object; } }

        private int m_ID;
        private T m_Object;
    }
}


