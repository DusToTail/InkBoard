using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private Board board;
    //private TurnController<RequestHandler<BaseCharacter>> turnController;

    private void Start()
    {
        Init();
    }
    private void Init()
    {
        board.Init();

        string cubeData = @"{""actualType"": ""CubeCharacter.CubeData"", ""id"": 0, ""gridPosition"": [0, 0, 0], ""orientation"": [1, 1, 1]}";
        string noCharacterData = @"{""actualType"": ""BaseCharacter.BaseCharacterData"", ""id"": -1, ""gridPosition"": [-1, -1, -1], ""orientation"": [-1, -1, -1]}";
        string[] example = new string[board.Layout.GetTotalCount()];
        System.Array.Fill(example, noCharacterData);
        example.SetValue(cubeData, 0);
        string concat = string.Join('\n',example);
        characterManager.Init(concat);
    }
}
