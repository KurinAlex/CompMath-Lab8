namespace CompMath_Lab8;

public interface IMethod
{
	string Name { get; }
	double[] Solve(Func<double, double, double> f, double x0, double y0, double w, double h);
}
