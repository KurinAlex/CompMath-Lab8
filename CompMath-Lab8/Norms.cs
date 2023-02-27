using System;
using System.Collections.Generic;
using System.Linq;

namespace CompMath_Lab8;

public static class Norms
{
	public static double ComputeUniformNorm(IEnumerable<double> values) => values.Max(v => Math.Abs(v));
	public static double ComputeL2Norm(IEnumerable<double> values, double h) => Math.Sqrt(values.Sum(v => v * v) * h);
}
