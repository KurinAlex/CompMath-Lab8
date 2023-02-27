using System;
using System.Linq;
using System.Collections.Generic;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Legends;

using CompMath_Lab8.Methods;

namespace CompMath_Lab8;

public class ViewModel
{
	const double X0 = 1.0;
	const double Y0 = -5.0 / 6.0;

	const int N = 100;
	const double W = 1.0;

	static double F(double x, double y) => x * x * x - 2.0 * y / x;
	static double TrueF(double x) => x * x * x * x / 6.0 - 1.0 / (x * x);

	static readonly IMethod[] methods = { new RungeKuttaMethod(), new AdamsMoultonMethod() };

	public ViewModel()
	{
		EXModel = InitPlotModel("e(x)");
		EHModel = InitPlotModel("e(h)");
	}

	public PlotModel EXModel { get; init; }
	public PlotModel EHModel { get; init; }

	private static double Norm(IEnumerable<double> values)
	{
		return values.Max(v => Math.Abs(v));
	}

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
	private void InitSeries(
		PlotModel model,
		IEnumerable<double> xArr,
		Dictionary<string, IEnumerable<double>> ySeries)
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
	public (IEnumerable<double>, Dictionary<string, IEnumerable<double>>) GetEHData(double minH, double maxH)
	{
		double stepH = (maxH - minH) / (N - 1);

		var hArr = Enumerable.Range(0, N).Select(k => minH + k * stepH);
		var eSeries = new Dictionary<string, IEnumerable<double>>(methods.Length);

		foreach (var method in methods)
		{
			var e = hArr.Select(h => method.Solve(F, X0, Y0, W, h).Select((y, i) => y - TrueF(X0 + i * h)));
			var eSer = e.Select(e => Norm(e));
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
	public void UpdateEHModel(double minH, double maxH)
	{
		EHModel.Series.Clear();
		var (hArr, eSeries) = GetEHData(minH, maxH);
		InitSeries(EHModel, hArr, eSeries);
	}
}
