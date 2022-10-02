using System;
using System.Collections.Generic;
using UnityEngine;

using BaseData = BaseCharacter.BaseCharacterData;
public class CharacterManager : MonoBehaviour, ISimulate<GameState>
{
    public static CharacterManager Instance { get; private set; }

    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Board board;
    private MyGrid<BaseCharacter> m_Grid;
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
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    public void Init(string data)
    {
        if(!string.IsNullOrEmpty(data))
            DeserializeData(data);
        m_Grid = new MyGrid<BaseCharacter>(board.Layout);
        m_CharacterInitIndex = -1;
        m_Grid.ForEach(CreateCharacterAtCell);
#if DEBUG
        //m_Grid.DebugLog();
#endif
    }
    public string SerializeData()
    {
        List<string> characters = new List<string>();
        for (int i = 0; i < board.Layout.GetTotalCount(); i++)
        {
            var data = m_Datas[i];
            Type type = data.GetType();
            data.actualType = type.FullName;
            if (type == typeof(BaseData))
            {
                characters.Add(JsonUtility.ToJson(data));
            }
            else if (type == typeof(CubeCharacter.CubeData))
            {
                characters.Add(JsonUtility.ToJson(data as CubeCharacter.CubeData));
            }
        }
        return string.Join('\n', characters.ToArray());
    }
    public void DeserializeData(string content)
    {
        m_Datas = new List<BaseData>((int)board.Layout.GetTotalCount());
        string[] lines = content.Split("\n");
        for (int i = 0; i < board.Layout.GetTotalCount(); i++)
        {
            string line = lines[i];
            if (line.Contains("CubeCharacter.CubeData"))
            {
                var data = JsonUtility.FromJson<CubeCharacter.CubeData>(line);
                m_Datas.Add(data);
            }
            else if (line.Contains("BaseCharacter.BaseCharacterData"))
            {
                var data = JsonUtility.FromJson<BaseData>(line);
                m_Datas.Add(data);
            }
        }
    }
    public BaseCharacter GetCharacter(int id)
    {
        var cell = System.Array.Find(m_Grid.GetArray(), x => x.Value != null && x.Value.ID == id);
        if(cell == null) { return null; }
        return cell.Value;
    }
    public BaseCharacter GetCharacter(Vector3Int gridPosition)
    {
        return m_Grid.GetValueAt(new GridPosition(gridPosition));
    }
    public void Simulate(GameState refSim)
    {
        refSim.characterDatas = new MyGrid<BaseData>(m_Grid.Layout);
        m_Grid.ForEach(x =>
        {
            if(x.Value != null)
            {
                refSim.characterDatas.SetValueAt(x.GetGridPosition(), x.Value.GetData());
            }
        });
    }
    private void CreateCharacterAtCell(Cell<BaseCharacter> cell)
    {
        //Debug.Log("Character at " + cell.GridPosition);
        m_CharacterInitIndex++;
        var data = m_Datas[m_CharacterInitIndex];
        if(data == null) { Debug.LogError("Data is null!"); return; }
        var prefab = GetCharacterPrefabFromID(data.id);
        if(prefab == null) 
        {
            //Debug.Log($"CharacterManager: {cell} Empty"); 
            return; 
        }

        var characterObj = Instantiate(prefab, transform);
        var character = characterObj.GetComponent<BaseCharacter>();
        character.Init(data);
        character.SetInputCommand();
        character.manager = this;
        cell.SetValue(character);
        GameManager.Instance.RegisterPlayer(character, (x) =>
        {
            return x.DefaultAction();
        },
        "Default Action");
        //Debug.Log($"CharacterManager: {cell} {character.ID}");
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
    private GameObject GetCharacterPrefabFromID(int id)
    {
        if (id == -1) { return null; }
        return System.Array.Find(prefabs, x => x.GetComponent<BaseCharacter>().ID == id);
    }

    
}
