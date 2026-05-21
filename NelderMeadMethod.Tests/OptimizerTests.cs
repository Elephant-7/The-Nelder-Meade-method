using System;
using Xunit;

namespace NelderMeadMethod.Tests
{
    public class OptimizerTests
    {
        [Fact]
        public void Minimize_Quadratic_ShouldFindMinimum()
        {
            Func<double[], double> f = x => x[0] * x[0] + x[1] * x[1];
            double[] start = { 2.0, 3.0 };
            var optimizer = new NelderMeadOptimizer();
            var (xMin, fMin) = optimizer.Minimize(f, start, tol: 1e-6);
            Assert.InRange(xMin[0], -1e-5, 1e-5);
            Assert.InRange(xMin[1], -1e-5, 1e-5);
            Assert.InRange(fMin, 0, 1e-5);
        }
    }
}