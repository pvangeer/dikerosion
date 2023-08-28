using DikeErosion.Data;

namespace DikeErosion.Visualization.ViewModels
{
    public static class TimeDependentOutputVariableExtensions
    {
        public static string ToTitle(this TimeDependentOutputVariable variable)
        {
            return variable.Name switch
            {
                "schadegetalPerTijdstap" => "Schadegetal [-]",
                "toenameSchade" => "Toename schade [1 / Tijdstap]",
                "logBreukspanning" => "Breukspanning",
                "maximalePiekdruk" => "Maximale piekdruk",
                "stijfheidsverhouding" => "Stijfheidsverhouding [-]",
                "rekendikte" => "Rekendikte asfaltbekleding [m]",
                "equivalenteStijfheidsmodulus" => "Equivalente stijfheidsmodulus",
                "belastingBekleding" => "Belasting bekleding",
                "bovengrensBelasting" => "Bovengrens belastingzone [m + NAP]",
                "ondergrensBelasting" => "Ondergrens belastingzone [m + NAP]",
                "maximumGolfhoogte" => "Maximale waarde golfhoogte golfklap [m]",
                "minimumGolfhoogte" => "Minimale waarde golfhoogte golfklap [m]",
                "impactGolfhoek" => "Reductiefactor scheef invallende golven [-]",
                "impactGolfhoogte" => "Rekenwaarde golfhoogte golfklap [m]",
                "hellingBuitentalud" => "Helling buitentalud [1/m]",
                "bovenzijdeHellingvlak" => "Bovenzijde hellingvlak [m NAP]",
                "rechterzijdeHellingvlak" => "Rechterzijde hellingvlak",
                "onderzijdeHellingvlak" => "Onderzijde hellingvlak [m + NAP]",
                "linkerzijdeHellingvlak" => "Linkerzijde hellingvlak",
                "diepteMaximaleGolfbelasting" => "Diepte maximale golfbelasting",
                "golfbrekingparameter" => "Golfbrekingparameter",
                "golfsteilheidDiepWater" => "Golfsteilheid diep water",
                "afstandMaximaleStijghoogte" => "Afstand maximale stijghoogte [m]",
                "hydraulischeBelasting" => "Hydraulische belasting",
                "referentietijdDegradatie" => "Referentietijd degradatie [sec]",
                "sterkteBekleding" => "Sterkte bekleding",
                "referentieDegradatie" => "Referentie degradatie",
                "maatgevendeBreedteGolfklap" => "Maatgevende breedte golfklap [m]",
                "verticaleAfstandWaterstandHoogteLocatie" => "Verticale afstand tussen waterstand en hoogte [m]",
                "representatieve2p" => "Golfoploophoogte (2%) [m]",
                "cumulatieveOverbelasting" => "Cumulatieve Overbelasting",
                _ => variable.Name
            };
        }
    }
}
