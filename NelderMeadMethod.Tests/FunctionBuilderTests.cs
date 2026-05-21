using Xunit;

namespace NelderMeadMethod.Tests
{
    public class FunctionBuilderTests
    {
        [Fact]
        public void Build_SimpleExpression_ReturnsCorrectValue()
        {
            var f = FunctionBuilder.Build("x0 + x1", 2);
            double[] point = { 2.5, 3.5 };
            double result = f(point);
            Assert.Equal(6.0, result, 5);
        }
    }
}