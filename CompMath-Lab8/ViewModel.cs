using System.Linq;
using System.Collections.Generic;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Legends;

using CompMath_Lab8.Methods;

namespace CompMath_Lab8;

public class ViewModel
{
	private const double X0 = 1.0;
	private const double Y0 = -5.0 / 6.0;

	private const int N = 100;
	private const double W = 1.0;

	private static double F(double x, double y) => x * x * x - 2.0 * y / x;
	private static double TrueF(double x) => x * x * x * x / 6.0 - 1.0 / (x * x);

	private static readonly IMethod[] methods = { new RungeKuttaMethod(), new AdamsMoultonMethod() };

	public ViewModel()
	{
		EXModel = InitPlotModel("e(x)");
		EHModel = InitPlotModel("e(h)");
	}

	public PlotModel EXModel { get; init; }
	public PlotModel EHModel { get; init; }

	private PlotModel InitPlotModel(string title)
	{
		var model = new PlotModel
		{
			Title = title
		};

		var legend = new Legend
		{
			LegendBorder = OxyColors.Black,
			LegendPosition = LegendPosition.LeftTop
		};
		model.Legends.Add(legend);

		return model;
	}
	private void InitSeries(PlotModel model, IEnumerable<double> xArr, Dictionary<string, IEnumerable<double>> ySeries)
	{
		foreach (var ySer in ySeries)
		{
			var series = new LineSeries
			{
				Title = ySer.Key
			};
			foreach (var (x, y) in xArr.Zip(ySer.Value))
			{
				series.Points.Add(new(x, y));
			}
			model.Series.Add(series);
		}
	}

	public (IEnumerable<double>, Dictionary<string, IEnumerable<double>>) GetEXData(double h)
	{
		int n = (int)(W / h) + 1;

		var xArr = Enumerable.Range(0, n).Select(k => X0 + k * h);
		var ySeries = new Dictionary<string, IEnumerable<double>>(methods.Length)
		{
			["True"] = xArr.Select(x => TrueF(x))
		};

		foreach (var method in methods)
		{
			var ySer = method.Solve(F, X0, Y0, W, h);
			ySeries.Add(method.Name, ySer);
		}
		return (xArr, ySeries);
	}
	public (IEnumerable<double>, Dictionary<string, IEnumerable<double>>) GetEHData(double minH, double maxH, Norm norm)
	{
		double stepH = (maxH - minH) / (N - 1);

		var hArr = Enumerable.Range(0, N).Select(k => minH + k * stepH);
		var eSeries = new Dictionary<string, IEnumerable<double>>(methods.Length);

		foreach (var method in methods)
		{
			var e = hArr.Select(h => method.Solve(F, X0, Y0, W, h).Select((y, i) => y - TrueF(X0 + i * h)));
			var eSer = norm == Norm.Uniform
				? e.Select(e => Norms.ComputeUniformNorm(e))
				: e.Zip(hArr).Select(t => Norms.ComputeL2Norm(t.First, t.Second));
			eSeries.Add(method.Name, eSer);
		}
		return (hArr, eSeries);
	}

	public void UpdateEXModel(double h)
	{
		EXModel.Series.Clear();
		var (xArr, ySeries) = GetEXData(h);
		InitSeries(EXModel, xArr, ySeries);
	}
	public void UpdateEHModel(double minH, double maxH, Norm norm)
	{
		EHModel.Series.Clear();
		var (hArr, eSeries) = GetEHData(minH, maxH, norm);
		InitSeries(EHModel, hArr, eSeries);
	}
}
