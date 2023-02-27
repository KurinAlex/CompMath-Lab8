namespace CompMath_Lab8.Methods;

public class RungeKuttaMethod : IMethod
{
	public string Name => "Runge-Kutta";

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
		double x = x0;
		double y = y0;
		var res = new double[n];
		res[0] = y0;

		for (int i = 1; i < n; i++)
		{
			double k1 = f(x, y);
			double k2 = f(x + h / 2.0, y + h * k1 / 2.0);
			double k3 = f(x + h / 2.0, y + h * k2 / 2.0);
			double k4 = f(x + h, y + h * k3);

			x += h;
			y += (k1 + 2.0 * k2 + 2.0 * k3 + k4) * h / 6.0;
			res[i] = y;
		}
		return res;
	}
}
