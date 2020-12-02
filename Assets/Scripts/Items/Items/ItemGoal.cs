


using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum ItemGoalType
{
    BIG_COIN,
    COIN_BLAST
}

public class ItemGoal : Item
{
    public ItemGoalType ItemGoalType;
    
    public override RawItemData Save()
    {
        var data = new RawItemGoalData();
        data.itemGoalType = ItemGoalType;

        SaveBase(data);
        
        return data;
    }

    public override void Load(RawItemData rawData)
    {
        LoadBase(rawData);
        var data = (RawItemGoalData) rawData;
        ItemGoalType = data.itemGoalType;
    }
}

public class RawItemGoalData: RawItemData
{
    public ItemGoalType itemGoalType;
}