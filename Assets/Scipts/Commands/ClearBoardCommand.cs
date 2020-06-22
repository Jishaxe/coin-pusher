public class ClearBoardCommand : BaseCommand
{
    public class Data: ICommandData
    {
    }
    public override void Load(string data)
    {
        LoadCommandData<Data>(data);
    }

    public override void Invoke()
    {
        
    }
}