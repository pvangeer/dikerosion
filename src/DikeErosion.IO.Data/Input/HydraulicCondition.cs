namespace DikeErosion.IO.Data.Input;

public class HydraulicCondition
{
    public HydraulicCondition(string name, double[] values)
    {
        Name = name;
        Values = values;
    }

    public string Name { get; }

    public double[] Values { get; }
}