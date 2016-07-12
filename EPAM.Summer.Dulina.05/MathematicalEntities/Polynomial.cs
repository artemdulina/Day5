using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace MathematicalEntities
{
    public sealed class Polynomial : ICloneable, IEquatable<Polynomial>
    {
        public int Degree { get; }
        private readonly PolynomialElement[] elements;
        private static readonly double accuracy;

        public Polynomial()
        {
            elements = new PolynomialElement[1];
            Degree = 0;
        }

        static Polynomial()
        {
            if (ConfigurationManager.AppSettings.Count > 0)
            {
                accuracy = double.Parse(ConfigurationManager.AppSettings["accuracy"]);
            }
            else
            {
                accuracy = 0.000001;
            }
        }

        public Polynomial(double[] coefficients, int[] degrees)
        {
            if (coefficients.Length != degrees.Length)
            {
                throw new ArgumentException("Number of coefficients should be equal to the number of degrees");
            }
            PolynomialElement[] preInit = new PolynomialElement[degrees.Length];
            AddToCollection(coefficients, degrees, preInit);
            Array.Sort(preInit, new SortByDegree());
            CheckForRepeatedDegrees(ref preInit);
            elements = preInit;
            Degree = elements[elements.Length - 1].degree;
        }

        public Polynomial(Polynomial polynomial)
        {
            elements = new PolynomialElement[polynomial.Count];
            Array.Copy(polynomial.GetElements(), elements, polynomial.Count);
            Degree = polynomial.Degree;
        }

        private Polynomial(PolynomialElement[] storage, int count)
        {
            elements = new PolynomialElement[count];
            Array.Copy(storage, elements, count);
            Degree = storage[count - 1].degree;
        }

        public Polynomial Addition(Polynomial x)
        {
            if (x == null)
            {
                throw new ArgumentNullException("x");
            }
            PolynomialElement[] result = new PolynomialElement[Count + x.Count];
            int i = 0, j = 0, resultIndex = 0;
            while (i < Count && j < x.Count)
            {
                if (elements[i].degree == x[j].degree)
                {
                    //result[resultIndex] = new PolynomialElement(elements[i].coefficient + x[j].coefficient, elements[i].degree);
                    result[resultIndex].coefficient = elements[i].coefficient + x[j].coefficient;
                    result[resultIndex].degree = elements[i].degree;
                    i++;
                    j++;
                }
                else if (elements[i].degree < x[j].degree)
                {
                    result[resultIndex] = elements[i];
                    i++;
                }
                else
                {
                    result[resultIndex] = x[j];
                    j++;
                }
                resultIndex++;
            }

            while (i < Count)
            {
                result[resultIndex] = elements[i];
                i++;
                resultIndex++;
            }

            while (j < x.Count)
            {
                result[resultIndex] = x[j];
                j++;
                resultIndex++;
            }
            Polynomial add = new Polynomial(result, resultIndex);
            return add;
        }

        public double CalculateValue(double x)
        {
            double result = 0;
            for (int i = 0; i < elements.Length; i++)
            {
                result += elements[i].coefficient * Math.Pow(x, elements[i].degree);
            }
            return result;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        private int Count => elements.Length;

        public PolynomialElement this[int index]
        {
            get { return elements[index]; }
            private set { elements[index] = value; }
        }

        private void AddToCollection(double[] coefficients, int[] degrees, PolynomialElement[] preInit)
        {
            for (int i = 0; i < degrees.Length; i++)
            {
                if (Math.Abs(coefficients[i]) > accuracy)
                    preInit[i] = new PolynomialElement(coefficients[i], degrees[i]);
            }
        }

        public static Polynomial operator +(Polynomial lhs, Polynomial rhs)
        {
            if (lhs == null)
            {
                throw new ArgumentNullException("lhs");
            }
            if (rhs == null)
            {
                throw new ArgumentNullException("rhs");
            }
            return lhs.Addition(rhs);
        }

        public static bool operator !=(Polynomial lhs, Polynomial rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(Polynomial other)
        {
            if (other == null) return false;
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            Polynomial compare = (Polynomial)obj;
            return this == compare;
        }

        public static bool operator ==(Polynomial lhs, Polynomial rhs)
        {
            if (ReferenceEquals(lhs, rhs)) return true;
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null)) return false;
            if (lhs.Count != rhs.Count)
                return false;
            for (int i = 0; i < lhs.Count; i++)
            {
                if (lhs[i].degree != rhs[i].degree || Math.Abs(lhs[i].coefficient - rhs[i].coefficient) > accuracy)
                    return false;
            }
            return true;
        }

        public object Clone()
        {
            return new Polynomial(this);
        }

        public PolynomialElement[] GetElements()
        {
            PolynomialElement[] elementsR = new PolynomialElement[Count];
            Array.Copy(elements, elementsR, Count);
            return elementsR;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (elements.Length == 0)
                return "Empty polynomial";
            foreach (PolynomialElement item in elements)
            {
                sb.Append(item);
            }
            if (sb.Length > 0)
            {
                if (sb[0] == '+')
                    sb.Remove(0, 1);
            }
            else
            {
                return "0";
            }
            return sb.ToString();
        }

        private void CheckForRepeatedDegrees(ref PolynomialElement[] preInit)
        {
            if (preInit.Length < 2)
                return;
            int resultIndex = 0;
            for (int i = 0; i < preInit.Length; i++)
            {
                int j = i;
                double coefficients = 0;
                while (j < preInit.Length - 1 && preInit[i].degree == preInit[j + 1].degree)
                {
                    coefficients += preInit[j++].coefficient;
                }
                if (j - i >= 1)
                {
                    preInit[resultIndex] = new PolynomialElement(preInit[j].coefficient + coefficients, preInit[j].degree);
                    if (j == preInit.Length - 1)
                        break;
                    i = i + j - 1;
                }
                else
                {
                    resultIndex++;
                    if (i == preInit.Length - 1 && resultIndex <= preInit.Length - 1)
                    {
                        preInit[resultIndex] = new PolynomialElement(preInit[i].coefficient, preInit[i].degree);
                    }
                }
            }
            if (resultIndex < preInit.Length - 1)
            {
                PolynomialElement[] updatedElements = new PolynomialElement[resultIndex + 1];
                Array.Copy(preInit, updatedElements, resultIndex + 1);
                preInit = updatedElements;
            }
        }

        public struct PolynomialElement
        {
            public double coefficient;
            public int degree;

            public PolynomialElement(double coefficient, int degree)
            {
                this.coefficient = coefficient;
                this.degree = degree;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                string coeff = "";
                if (coefficient > 0)
                {
                    sb.Append("+");
                }
                if (coefficient != 1)
                {
                    coeff = Convert.ToString(coefficient);
                }
                if (degree == 1)
                {
                    sb.Append($"{coeff}x");
                }
                else if (degree == 0)
                {
                    if (coeff != "0")
                        sb.Append($"{coeff}");
                }
                else
                {
                    sb.Append($"{coeff}x^({degree})");
                }
                return sb.ToString();
            }
        }

        private class SortByDegree : IComparer<PolynomialElement>
        {
            public int Compare(PolynomialElement x, PolynomialElement y)
            {
                if (x.degree > y.degree)
                    return 1;
                if (x.degree < y.degree)
                    return -1;
                return 0;
            }
        }

    }
}
