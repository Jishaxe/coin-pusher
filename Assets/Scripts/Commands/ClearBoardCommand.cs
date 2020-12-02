public class ClearBoardCommand : BaseCommand
{
    private BoardController _boardController;
    
    public class Data: ICommandData
    {
    }

    public ClearBoardCommand(BoardController boardController)
    {
        _boardController = boardController;
    }
    
    public override void Load(string data)
    {
        _type = CommandType.CLEAR_BOARD;
        LoadCommandData<Data>(data);
    }

    public override void Invoke()
    {
        _boardController.ClearBoard();
    }
}