using System;
using System.Collections.Generic;
using UnityEngine;

using BaseData = BaseCharacter.BaseCharacterData;
public class CharacterManager : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Board board;
    private MyGrid<BaseCharacter> m_Grid;
    private RequestHandler<Movement> m_MovementHandler;
    private List<BaseData> m_Datas;
    private int m_CharacterInitIndex;

    private void OnEnable()
    {
        BaseCharacter.OnCreated += AddCharacter;
        BaseCharacter.OnDeleted += RemoveCharacter;
    }
    private void OnDisable()
    {
        BaseCharacter.OnCreated -= AddCharacter;
        BaseCharacter.OnDeleted -= RemoveCharacter;
    }
    public void Init(string data)
    {
        m_Datas = new List<BaseData>((int)board.Layout.GetTotalCount());
        DeserializeData(data);
        m_Grid = new MyGrid<BaseCharacter>(board.Layout);
        m_MovementHandler = new RequestHandler<Movement>();
        m_CharacterInitIndex = 0;
        m_Grid.ForEach(CreateCharacterAtCell);
#if DEBUG
        m_Grid.DebugLog();
#endif
    }
    public void RequestMovement(Func<Movement, bool> action, Func<Movement, bool> validCheck)
    {
        m_MovementHandler.AddNewRequest(action, validCheck);
    }
    public void StackNewRequestAt(Movement target, Func<Movement, bool> action, Func<Movement, bool> validCheck)
    {
        m_MovementHandler.StackNewRequestAt(target, action, validCheck);
    }
    public void ProcessRequests()
    {
        m_MovementHandler.ProcessRequests(true);
    }
    private void CreateCharacterAtCell(Cell<BaseCharacter> cell)
    {
        var data = m_Datas[m_CharacterInitIndex];
        if(data == null || data.id == -1) { return; }

        var prefab = GetCharacterFromID(data.id);
        if(prefab == null) { return; }

        Vector3Int gridPosition = data.GridPosition;
        Vector3 worldPosition = board.GetWorldPositionAt(gridPosition);
        var characterObj = Instantiate(prefab, worldPosition, Quaternion.identity, transform);
        var character = characterObj.GetComponent<BaseCharacter>();
        character.Init(data);
        character.manager = this;
        cell.SetValue(character);

        m_CharacterInitIndex++;
    }
    private string SerializeData()
    {
        string result = "";
        for(int i = 0; i < board.Layout.GetTotalCount(); i++)
        {
            var data = m_Datas[i];
            Type type = data.GetType();
            data.actualType = type.FullName;
            if (type == typeof(BaseData))
            {
                result += JsonUtility.ToJson(data);
            }
            else if(type == typeof(CubeCharacter.CubeData))
            {
                result += JsonUtility.ToJson(data as CubeCharacter.CubeData);
            }
            result += '\n';
        }
        result.Remove(result.Length - 1);
        return result;
    }
    private void DeserializeData(string content)
    {
        string[] lines = content.Split("\n");
        m_Datas.Clear();
        for (int i = 0; i < board.Layout.GetTotalCount(); i++)
        {
            string line = lines[i];
            if(line.Contains("CubeCharacter.CubeData"))
            {
                var data = JsonUtility.FromJson<CubeCharacter.CubeData>(line);
                m_Datas.Add(data);
            }
            else if(line.Contains("BaseCharacter.BaseCharacterData"))
            {
                var data = JsonUtility.FromJson<BaseData>(line);
                m_Datas.Add(data);
            }
        }
    }
    private void AddCharacter(BaseCharacter character)
    {
        GridPosition gridPosition = new GridPosition(character.GridPosition);
        m_Grid.SetValueAt(gridPosition, character);
    }
    private void RemoveCharacter(BaseCharacter character)
    {
        GridPosition gridPosition = new GridPosition(character.GridPosition);
        m_Grid.SetValueAt(gridPosition, null);
    }
    private GameObject GetCharacterFromID(int id)
    {
        if (id == -1) { return null; }
        return System.Array.Find(prefabs, x => x.GetComponent<BaseCharacter>().ID == id);
    }
}
