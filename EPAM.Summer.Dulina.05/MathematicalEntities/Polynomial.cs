using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace MathematicalEntities
{
    public class Polynomial
    {
        private struct PolynomialElement
        {
            public double coefficient;
            public int degree;

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
                    sb.Append($"{coeff}");
                }
                else
                {
                    sb.Append($"{coeff}x^({degree})");
                }
                return sb.ToString();
            }
        }
        public int Degree { get; }

        private List<PolynomialElement> elements;

        public Polynomial()
        {
            elements = new List<PolynomialElement>();
        }

        public Polynomial(double[] coefficients, int[] degrees)
        {
            if (coefficients.Length != degrees.Length)
            {
                throw new ArgumentException("Number of coefficients should be equal to the number of degrees");
            }
            elements = new List<PolynomialElement>();
            AddToCollection(coefficients, degrees);
            elements.Sort(new SortByDegree());
            Degree = elements[elements.Count - 1].degree;
        }

        public Polynomial Addition(Polynomial x)
        {
            Polynomial result = new Polynomial();
            if (x.GetPolynomialElementsCount() == 0 && 0 == GetPolynomialElementsCount())
            {
                return result;
            }
            int i = 0, j = 0;
            while (i < x.GetPolynomialElementsCount() && j < GetPolynomialElementsCount())
            {
                if (elements[i].degree == x[j].degree)
                {
                    PolynomialElement add;
                    add.degree = elements[i].degree;
                    add.coefficient = elements[i].coefficient + x[j].coefficient;
                    result.AddPolynomialElement(add);
                    i++;
                    j++;
                }
                else if (elements[i].degree < x[j].degree)
                {
                    result.AddPolynomialElement(elements[i]);
                    i++;
                }
                else
                {
                    result.AddPolynomialElement(x[j]);
                    j++;
                }
            }

            while (i < x.GetPolynomialElementsCount())
            {
                result.AddPolynomialElement(elements[i]);
                i++;
            }

            while (j < GetPolynomialElementsCount())
            {
                result.AddPolynomialElement(x[j]);
                j++;
            }

            return result;
        }

        public double CalculateValue(double x)
        {
            double result = 0;
            for (int i = 0; i < elements.Count; i++)
            {
                result += elements[i].coefficient * Math.Pow(x, elements[i].degree);
            }
            return result;
        }

        public static Polynomial operator +(Polynomial x, Polynomial y)
        {
            return x.Addition(y);
        }

        public static bool operator ==(Polynomial x, Polynomial y)
        {
            if (x.GetPolynomialElementsCount() != y.GetPolynomialElementsCount())
                return false;
            for (int i = 0; i < x.GetPolynomialElementsCount(); i++)
            {
                if (x[i].degree != y[i].degree || x[i].coefficient != y[i].coefficient)
                    return false;
            }
            return true;
            //return x.ToString() == y.ToString();
        }

        public static bool operator !=(Polynomial x, Polynomial y) => !(x == y);

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (elements.Count == 0)
                return "Empty polynomial";
            foreach (PolynomialElement item in elements)
            {
                sb.Append(item);
            }
            if (sb[0] == '+')
                sb.Remove(0, 1);
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            Polynomial compare = obj as Polynomial;
            return this == compare;
        }

        private int GetPolynomialElementsCount()
        {
            return elements.Count;
        }

        private PolynomialElement this[int index]
        {
            get { return elements[index]; }
        }

        private void AddPolynomialElement(PolynomialElement element)
        {
            elements.Add(element);
        }

        private void AddToCollection(double[] coefficients, int[] degrees)
        {
            PolynomialElement add;
            for (int i = 0; i < coefficients.Length; i++)
            {
                add.degree = degrees[i];
                add.coefficient = coefficients[i];
                elements.Add(add);
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
