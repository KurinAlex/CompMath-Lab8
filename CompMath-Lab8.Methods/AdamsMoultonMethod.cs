namespace CompMath_Lab8.Methods;

public class AdamsMoultonMethod : IMethod
{
	public string Name => "Adams-Moulton";

	public double[] Solve(Func<double, double, double> f, double x0, double y0, double w, double h)
	{
		if (h <= 0.0)
		{
			throw new ArgumentOutOfRangeException(nameof(h));
		}

		if (w <= 0.0)
		{
			throw new ArgumentOutOfRangeException(nameof(w));
		}

		int n = (int)(w / h) + 1;

		var y = new double[n];
		var startY = new RungeKuttaMethod().Solve(f, x0, y0, Math.Min(3.0 * h, w), h);
		startY.CopyTo(y, 0);

		for (int i = 3; i < n; i++)
		{
			var (x1, y1) = (x0 + (i - 3) * h, y[i - 3]);
			var (x2, y2) = (x1 + h, y[i - 2]);
			var (x3, y3) = (x2 + h, y[i - 1]);
			var (x4, y4) = (x3 + h, y[i]);

			double mapping(double y)
				=> y3 + (9.0 * f(x4, y) + 19.0 * f(x3, y3) - 5.0 * f(x2, y2) + f(x1, y1)) * h / 24.0;
			y[i] = SolveNonLinear(mapping, y4);
		}
		return y;
	}

	private double SolveNonLinear(Func<double, double> f, double x0)
	{
		double x = x0;
		double oldX;
		do
		{
			(oldX, x) = (x, f(x));
		} while (Math.Abs(x - oldX) > 1e-10);
		return x;
	}
}
