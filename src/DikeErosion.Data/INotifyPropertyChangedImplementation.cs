using System.ComponentModel;

namespace DikeErosion.Data;

public interface INotifyPropertyChangedImplementation : INotifyPropertyChanged
{
    void OnPropertyChanged(string? propertyName);
}