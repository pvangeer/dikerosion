namespace DikeErosion.Data
{
    public class TimeDependentOutputVariable
    {
        public TimeDependentOutputVariable(string name, Type valueType, TimeDependentOutputVariableValue[] values)
        {
            Name = name;
            Values = values;
            ValueType = valueType;
        }

        public string Name { get; }

        public Type ValueType { get; }

        public TimeDependentOutputVariableValue[] Values { get; }
    }
}
