namespace CompMath_Lab8;

public class AdamsMoultonMethod : IMethod
{
	public string Name => "Adams-Moulton";

	public double[] Solve(Func<double, double, double> f, double x0, double y0, double w, double h)
	{
		if (h <= 0.0)
		{
			throw new ArgumentOutOfRangeException(nameof(h));
		}

		int n = (int)(w / h);
		if (n < 3)
		{
			throw new ArgumentOutOfRangeException(nameof(w));
		}

		var y = new double[n + 1];
		var startY = new RungeKuttaMethod().Solve(f, x0, y0, 3.0 * h, h);
		startY.CopyTo(y, 0);

		for (int i = 3; i <= n; i++)
		{
			var (x1, y1) = (x0 + (i - 3) * h, y[i - 3]);
			var (x2, y2) = (x1 + h, y[i - 2]);
			var (x3, y3) = (x2 + h, y[i - 1]);
			var (x4, y4) = (x3 + h, y[i]);

			y[i] = y3 + (9.0 * f(x4, y4) + 19.0 * f(x3, y3) - 5.0 * f(x2, y2) + f(x1, y1)) * h / 24.0;
		}
		return y;
	}
}
