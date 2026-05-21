using System;

namespace NelderMeadMethod
{
    public class NelderMeadOptimizer
    {
        private readonly double _alpha = 1.0;
        private readonly double _gamma = 2.0;
        private readonly double _rho = 0.5;
        private readonly double _sigma = 0.5;

        public (double[] xMin, double fMin) Minimize(
            Func<double[], double> f,
            double[] start,
            double tol = 1e-6,
            int maxIter = 1000)
        {
            Simplex simplex = Simplex.CreateInitial(start);
            simplex.EvaluateAll(f);

            for (int iter = 0; iter < maxIter; iter++)
            {
                var (iLow, iHigh, iSecondHigh) = simplex.GetIndicesByValues();

                double[] centroid = simplex.GetCentroid(iHigh);

                // Отражение
                double[] xr = new double[simplex.Dimension];
                for (int j = 0; j < simplex.Dimension; j++)
                    xr[j] = centroid[j] + _alpha * (centroid[j] - simplex.Points[iHigh][j]);
                double fr = f(xr);

                if (fr < simplex.Values[iLow])
                {
                    // Растяжение
                    double[] xe = new double[simplex.Dimension];
                    for (int j = 0; j < simplex.Dimension; j++)
                        xe[j] = centroid[j] + _gamma * (xr[j] - centroid[j]);
                    double fe = f(xe);
                    if (fe < fr)
                        simplex.ReplacePoint(iHigh, xe, fe);
                    else
                        simplex.ReplacePoint(iHigh, xr, fr);
                }
                else if (fr < simplex.Values[iSecondHigh])
                {
                    simplex.ReplacePoint(iHigh, xr, fr);
                }
                else
                {
                    double[] xc = new double[simplex.Dimension];
                    if (fr < simplex.Values[iHigh])
                    {
                        for (int j = 0; j < simplex.Dimension; j++)
                            xc[j] = centroid[j] + _rho * (xr[j] - centroid[j]);
                    }
                    else
                    {
                        for (int j = 0; j < simplex.Dimension; j++)
                            xc[j] = centroid[j] + _rho * (simplex.Points[iHigh][j] - centroid[j]);
                    }
                    double fc = f(xc);
                    if (fc < simplex.Values[iHigh])
                        simplex.ReplacePoint(iHigh, xc, fc);
                    else
                        simplex.Shrink(iLow, f, _sigma);
                }

                if (simplex.MaxDistance() < tol)
                    break;
            }

            return simplex.GetBest();
        }
    }
}