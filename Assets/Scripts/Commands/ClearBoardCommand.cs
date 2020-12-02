public class ClearBoardCommand : BaseCommand
{
    private CoinSpawnController _coinSpawnController;
    
    public class Data: ICommandData
    {
    }

    public ClearBoardCommand(CoinSpawnController coinSpawnController)
    {
        _coinSpawnController = coinSpawnController;
    }
    
    public override void Load(string data)
    {
        _type = CommandType.CLEAR_BOARD;
        LoadCommandData<Data>(data);
    }

    public override void Invoke()
    {
        _coinSpawnController.RemoveAllCoins();
    }
}