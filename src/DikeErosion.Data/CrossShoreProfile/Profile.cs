using System.Collections.ObjectModel;

namespace DikeErosion.Data.CrossShoreProfile;

public class Profile
{
    public Profile()
    {
        Coordinates = new ObservableCollection<CrossShoreCoordinate>();
        CharacteristicPoints = new ObservableCollection<CharacteristicPoint>();
    }

    public ObservableCollection<CrossShoreCoordinate> Coordinates { get; }

    public ObservableCollection<CharacteristicPoint> CharacteristicPoints { get; }
}