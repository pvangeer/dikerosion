namespace DikeErosion.Data.CrossShoreProfile
{
    public static class ProfileExtensions
    {
        /// <summary>
        /// Gives an interpolated z value for given <param name="x"/>
        /// </summary>
        /// <param name="x">X value at the location to interpolate a z-value</param>
        /// <returns></returns>
        public static double InterpolateZ(this Profile profile, double x)
        {
            var xCoordinates = profile.Coordinates.Select(c => c.X).ToArray();
            var zCoordinates = profile.Coordinates.Select(c => c.Z).ToArray();
            if (!xCoordinates.Any() || x < xCoordinates.First() || x > xCoordinates.Last())
            {
                return double.NaN;
            }

            var coordinateCount = xCoordinates.Length;

            if (x - xCoordinates.First() <= xCoordinates.Last() - x)
            {
                for (var i = 0; i < coordinateCount; i++)
                {
                    if (Math.Abs(xCoordinates[i] - x) < 1E-8)
                        return zCoordinates[i];

                    if (xCoordinates[i] < x && x < xCoordinates[i + 1])
                    {
                        return LinearInterpolator.Interpolate(xCoordinates[i], zCoordinates[i],
                                                                         xCoordinates[i + 1], zCoordinates[i + 1],
                                                                         x);
                    }
                }
            }
            else
            {
                for (var i = coordinateCount - 1; i > 0; i--)
                {
                    if (Math.Abs(xCoordinates[i] - x) < 1E-8)
                        return zCoordinates[i];

                    if (xCoordinates[i - 1] < x && x < xCoordinates[i])
                    {
                        return LinearInterpolator.Interpolate(xCoordinates[i - 1], zCoordinates[i - 1],
                                                                         xCoordinates[i], zCoordinates[i],
                                                                         x);
                    }
                }
            }
            throw new Exception("Something went wrong"); // Should never occur
        }
    }
}
