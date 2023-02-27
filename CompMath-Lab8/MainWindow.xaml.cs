using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CompMath_Lab8;

public partial class MainWindow : Window
{
	private const double MinStep = 0.0;
	private const double MaxStep = 1.0 / 3.0;

	private readonly ViewModel _viewModel = new();

	private double _h;
	private double _startH;
	private double _endH;

	public MainWindow()
	{
		InitializeComponent();

		DataContext = _viewModel;

		exPlot.Model = _viewModel.EXModel;
		ehPlot.Model = _viewModel.EHModel;
	}

	private static bool CheckHRange(double h)
	{
		return h > MinStep && h <= MaxStep;
	}

	private void UpdateEXModel(object sender, TextChangedEventArgs e)
	{
		if (hInput == null || !double.TryParse(hInput.Text, out _h) || !CheckHRange(_h))
		{
			return;
		}

		_viewModel.UpdateEXModel(_h);
		exPlot.InvalidatePlot();
	}
	private void UpdateEHModel(object sender, TextChangedEventArgs e)
	{
		if (startHInput == null || !double.TryParse(startHInput.Text, out _startH) || !CheckHRange(_startH))
		{
			return;
		}

		if (endHInput == null || !double.TryParse(endHInput.Text, out _endH)
			|| !CheckHRange(_endH) || _endH < _startH)
		{
			return;
		}

		_viewModel.UpdateEHModel(_startH, _endH);
		ehPlot.InvalidatePlot();
	}

	private void ButtonEXClick(object sender, RoutedEventArgs e)
	{
		var (xArr, ySeries) = _viewModel.GetEXData(_h);

		string tableString = Drawer.GetTableString(xArr, "x", ySeries, "e(x)", 6);
		tableString = $"h = {_h}{Environment.NewLine}" + tableString;

		File.WriteAllText("EXData.txt", tableString);
	}
	private void ButtonEHClick(object sender, RoutedEventArgs e)
	{
		var (xArr, ySeries) = _viewModel.GetEHData(_startH, _endH);

		string tableString = Drawer.GetTableString(xArr, "h", ySeries, "e(h)", 6);
		tableString = $"h: [{_startH}; {_endH}]{Environment.NewLine}" + tableString;

		File.WriteAllText("EHData.txt", tableString);
	}
}
