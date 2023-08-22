using DikeErosion.Data;

namespace DikeErosion.Visualization.ViewModels
{
    public static class TimeDependentOutputVariableExtensions
    {
        public static string ToTitle(this TimeDependentOutputVariable variable)
        {
            switch (variable.Name)
            {
                case "schadegetalPerTijdstap":
                    return "Schadegetal [-]";
                case "toenameSchade":
                    return "Toename schade [1 / Tijdstap]";
                case "logBreukspanning":
                    return "Breukspanning";
                case "maximalePiekdruk":
                    return "Maximale piekdruk";
                case "stijfheidsverhouding":
                    return "Stijfheidsverhouding [-]";
                case "rekendikte":
                    return "Rekendikte asfaltbekleding [m]";
                case "equivalenteStijfheidsmodulus":
                    return "Equivalente stijfheidsmodulus";
                case "belastingBekleding":
                    return "Belasting bekleding";
                case "bovengrensBelasting":
                    return "Bovengrens belastingzone [m + NAP]";
                case "ondergrensBelasting":
                    return "Ondergrens belastingzone [m + NAP]";
                case "maximumGolfhoogte":
                    return "Maximale waarde golfhoogte golfklap [m]";
                case "minimumGolfhoogte":
                    return "Minimale waarde golfhoogte golfklap [m]";
                case "impactGolfhoek":
                    return "Reductiefactor scheef invallende golven [-]";
                case "impactGolfhoogte":
                    return "Rekenwaarde golfhoogte golfklap [m]";
                case "hellingBuitentalud":
                    return "Helling buitentalud [1/m]";
                case "bovenzijdeHellingvlak":
                    return "Bovenzijde hellingvlak [m NAP]";
                case "rechterzijdeHellingvlak":
                    return "Rechterzijde hellingvlak";
                case "onderzijdeHellingvlak":
                    return "Onderzijde hellingvlak [m + NAP]";
                case "linkerzijdeHellingvlak":
                    return "Linkerzijde hellingvlak";
                case "diepteMaximaleGolfbelasting":
                    return "Diepte maximale golfbelasting";
                case "golfbrekingparameter":
                    return "Golfbrekingparameter";
                case "golfsteilheidDiepWater":
                    return "Golfsteilheid diep water";
                case "afstandMaximaleStijghoogte":
                    return "Afstand maximale stijghoogte [m]";
                case "hydraulischeBelasting":
                    return "Hydraulische belasting";
                case "referentietijdDegradatie":
                    return "Referentietijd degradatie [sec]";
                case "sterkteBekleding":
                    return "Sterkte bekleding";
                case "referentieDegradatie":
                    return "Referentie degradatie";
                case "maatgevendeBreedteGolfklap":
                    return "Maatgevende breedte golfklap [m]";
                default:
                    return variable.Name;
            }
        }
    }
}
