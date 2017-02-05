using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4
{
    public interface ILeastSquareError
    {
        double[,] GetOptimalA(double[,] X, double[,] y);
    }
}
