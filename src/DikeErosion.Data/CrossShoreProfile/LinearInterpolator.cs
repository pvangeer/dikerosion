namespace DikeErosion.Data.CrossShoreProfile
{
    public static class LinearInterpolator
    {
        /// <summary>
        /// Calculates an interpolated value with the use of linear interpolation. 
        /// </summary>
        /// <param name="x0">X of the first coordinate</param>
        /// <param name="y0">Y of the first coordinate</param>
        /// <param name="x1">X of the second coordinate</param>
        /// <param name="y1">Y of the second coordinate</param>
        /// <param name="x">X value between <paramref name="x0"/> and <paramref name="y0"/> at which y should be interpolated</param>
        /// <returns>The y value at <paramref name="x"/> according to linear interpolation between <paramref name="x0"/> and <paramref name="y0"/></returns>
        public static double Interpolate(double x0, double y0, double x1, double y1, double x)
        {
            if ((x < x0 && x < x1) || (x > x0 && x > x1)) throw new ArgumentOutOfRangeException("x");

            return y0 + (x - x0) * ((y1 - y0) / (x1 - x0));
        }
    }
}