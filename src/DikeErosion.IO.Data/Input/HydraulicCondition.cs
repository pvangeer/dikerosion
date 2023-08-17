namespace DikeErosion.IO.Data.Input;

public class HydraulicConditionItem
{
    public HydraulicConditionItem(string name, double[] values)
    {
        Name = name;
        Values = values;
    }

    public string Name { get; }

    public double[] Values { get; }
}