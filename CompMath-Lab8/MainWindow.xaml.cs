using System;
using System.IO;
using System.Windows;

namespace CompMath_Lab8;

public partial class MainWindow : Window
{
	private const double MinStep = 0.0;
	private const double MaxStep = 1.0 / 3.0;

	private const string DataSaveDirectory = @"D:\Sources\University\2 course\CompMath\CompMath-Lab8\Data";

	private readonly ViewModel _viewModel = new();

	private double _h;
	private double _startH;
	private double _endH;
	private Norm _norm;

	public MainWindow()
	{
		InitializeComponent();

		DataContext = _viewModel;

		exPlot.Model = _viewModel.EXModel;
		ehPlot.Model = _viewModel.EHModel;

		normChoice.ItemsSource = Enum.GetValues<Norm>();
		normChoice.SelectedIndex = 0;
	}

	private static bool CheckHRange(double h)
	{
		return h > MinStep && h <= MaxStep;
	}

	private void UpdateEXModel(object sender, RoutedEventArgs e)
	{
		if (hInput == null || !double.TryParse(hInput.Text, out _h) || !CheckHRange(_h))
		{
			return;
		}

		_viewModel.UpdateEXModel(_h);
		exPlot.InvalidatePlot();
	}
	private void UpdateEHModel(object sender, RoutedEventArgs e)
	{
		if (startHInput == null || !double.TryParse(startHInput.Text, out _startH) || !CheckHRange(_startH))
		{
			return;
		}

		if (endHInput == null || !double.TryParse(endHInput.Text, out _endH) || !CheckHRange(_endH) || _endH < _startH)
		{
			return;
		}

		if (normChoice == null)
		{
			return;
		}
		_norm = (Norm)normChoice.SelectedItem;

		_viewModel.UpdateEHModel(_startH, _endH, _norm);
		ehPlot.InvalidatePlot();
	}

	private void ButtonEXClick(object sender, RoutedEventArgs e)
	{
		var (xArr, ySeries) = _viewModel.GetEXData(_h);

		string tableString = Drawer.GetTableString(xArr, "x", ySeries, "e(x)", 6);
		tableString = $"h = {_h}{Environment.NewLine}" + tableString;

		string path = Path.Combine(DataSaveDirectory, "EXData.txt");
		File.WriteAllText(path, tableString);
	}
	private void ButtonEHClick(object sender, RoutedEventArgs e)
	{
		var (xArr, ySeries) = _viewModel.GetEHData(_startH, _endH, _norm);

		string tableString = Drawer.GetTableString(xArr, "h", ySeries, "e(h)", 6);
		tableString = string.Join(Environment.NewLine, $"h: [{_startH}; {_endH}]", $"Norm = {_norm}", tableString);

		string path = Path.Combine(DataSaveDirectory, "EHData.txt");
		File.WriteAllText(path, tableString);
	}
}
