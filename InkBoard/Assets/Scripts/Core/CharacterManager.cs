using System;
using System.Collections.Generic;
using UnityEngine;

using BaseData = BaseCharacter.BaseCharacterData;
public class CharacterManager : MonoBehaviour
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
        m_CharacterInitIndex = 0;
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
    private void CreateCharacterAtCell(Cell<BaseCharacter> cell)
    {
        Debug.Log("Character at " + cell.GridPosition);
        var data = m_Datas[m_CharacterInitIndex];
        if(data == null) { Debug.LogError("Data is null!"); return; }
        if (data.id == -1) { Debug.Log("ID is -1!"); return; }
        var prefab = GetCharacterFromID(data.id);
        if(prefab == null) { Debug.LogError("No prefab found with ID: " + data.id + "!"); return; }

        Vector3Int gridPosition = data.GridPosition;
        Vector3 worldPosition = board.GetWorldPositionAt(gridPosition);
        var characterObj = Instantiate(prefab, worldPosition, Quaternion.identity, transform);
        var character = characterObj.GetComponent<BaseCharacter>();
        character.Init(data);
        character.manager = this;
        cell.SetValue(character);
        GameManager.Instance.RegisterPlayer(character, (x) =>
        {
            return x.DefaultAction();
        });
        m_CharacterInitIndex++;
        Debug.Log("Created!");
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
