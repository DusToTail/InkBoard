
public class GameState : Simulation
{
    public GameState(string id) : base(id)
    {
    }
    public MyGrid<Block.BlockData> blockDatas;
    public MyGrid<BaseCharacter.BaseCharacterData> characterDatas;
}
