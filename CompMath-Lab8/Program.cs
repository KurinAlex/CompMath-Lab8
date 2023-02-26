using CompMath_Lab6;

namespace CompMath_Lab8;

public class Program
{
	const double X0 = 1.0;
	const double Y0 = -5.0 / 6.0;
	const double W = 1.0;

	const double H = 0.01;
	const int N = (int)(W / H) + 1;

	const int NH = 100;
	const double MinH = 0.1;
	const double MaxH = 0.3;
	const double StepH = (MaxH - MinH) / (NH - 1);

	static double F(double x, double y) => x * x * x - 2.0 * y / x;
	static double TrueF(double x) => x * x * x * x / 6.0 - 1.0 / (x * x);

	static readonly IMethod[] methods = { new RungeKuttaMethod(), new AdamsMoultonMethod() };

	static void Main()
	{
		var x = Enumerable.Range(0, N).Select(k => X0 + k * H).ToArray();
		var y = new Dictionary<string, double[]>(methods.Length)
		{
			["True"] = x.Select(x => TrueF(x)).ToArray()
		};

		var h = Enumerable.Range(0, NH).Select(k => MinH + k * StepH).ToArray();
		var e = new Dictionary<string, double[]>(methods.Length);

		foreach (var method in methods)
		{
			var yRes = method.Solve(F, X0, Y0, W, H);
			y.Add(method.Name, yRes);

			var res = h.Select(h =>
				method.Solve(F, X0, Y0, W, h)
					.Select((d, i) => Math.Abs(d - TrueF(X0 + i * h)))
					.Max())
				.ToArray();
			e.Add(method.Name, res);
		}

		Drawer.DrawTable(x, "x", "F6", y, "e(x)", "F8");
		Drawer.DrawTable(h, "h", "F6", e, "e(h)", "E6");
		Console.ReadLine();
	}
}