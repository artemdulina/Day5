using MathematicalEntities;
using NUnit.Framework;

namespace MathematicalEntitiesTests
{
    [TestFixture]
    public class PolynomialTests
    {
        [Test]
        [TestCase(new double[] { -5, 1, 1, 18 }, new int[] { 100, 100, 5, 0 }, ExpectedResult = "18+x^(5)-4x^(100)")]
        [TestCase(new double[] { 5, 1, 18 }, new int[] { 100, 100, 5 }, ExpectedResult = "18x^(5)+6x^(100)")]
        public string PolynomialConstructorAndToStringTest_ShouldReturnCorrectString(double[] coefficients, int[] degrees)
        {
            // arrange
            Polynomial test = new Polynomial(coefficients, degrees);

            // act
            return test.ToString();

            // assert
        }

        [Test]
        [TestCase(new double[] { 18, 1, -5, 1 }, new int[] { 0, 5, 100, 100 },
                  new double[] { 18, 10, 1, 1 }, new int[] { 0, 1, 2, 1000 }, ExpectedResult = "36+10x+x^(2)+x^(5)-4x^(100)+x^(1000)")]
        public string AdditionTest_ShouldReturnCorrectString(double[] coefficients, int[] degrees, double[] coefficientss, int[] degreess)
        {
            // arrange
            Polynomial first = new Polynomial(coefficients, degrees);
            Polynomial second = new Polynomial(coefficientss, degreess);

            // act
            return (first + second).ToString();

            // assert
        }

        [Test]
        [TestCase(new double[] { 500 }, new int[] { 2 },
                 new double[] { 100, 100, 300 }, new int[] { 2, 2, 2 }, ExpectedResult = true)]
        public bool OperatorEqual_ShouldReturnCorrectString(double[] coefficients, int[] degrees, double[] coefficientss, int[] degreess)
        {
            // arrange
            Polynomial first = new Polynomial(coefficients, degrees);
            Polynomial second = new Polynomial(coefficientss, degreess);

            // act
            return first == second;

            // assert
        }

        [Test]
        [TestCase(new double[] { 500 }, new int[] { 2 }, new double[] { 500 }, new int[] { 2 })]
        [TestCase(new double[] { 500.121 }, new int[] { 2 }, new double[] { 500.121 }, new int[] { 2 })]
        [TestCase(new double[] { 500.0000001 }, new int[] { 2 }, new double[] { 500.0000001 }, new int[] { 2 })]
        [TestCase(new double[] { 0.00000017586 }, new int[] { 2 }, new double[] { 0.00000017586 }, new int[] { 2 })]
        public void HashCodeTest_ShouldReturnCorrectString(double[] coefficients, int[] degrees, double[] coefficientss, int[] degreess)
        {
            // arrange
            Polynomial first = new Polynomial(coefficients, degrees);
            Polynomial second = new Polynomial(coefficientss, degreess);

            // act;

            // assert
            Assert.IsTrue(first.GetHashCode() == second.GetHashCode());
        }
    }
}