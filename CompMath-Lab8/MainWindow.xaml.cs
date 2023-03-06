using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

using CompMath_Lab8.Utilities;

namespace CompMath_Lab8;

public partial class MainWindow : Window
{
	private const string DataSaveDirectoryName = @"D:\Sources\University\2 course\CompMath\CompMath-Lab8\Data";
	private const string EXDataFileName = "EXData.txt";
	private const string EHDataFileName = "EHData.txt";

	private const int Precision = 6;

	private readonly ViewModel _viewModel = new();

	private double _w1;
	private double _h;

	private double _w2;
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

	private static bool IsPositive(double x)
	{
		return x > 0.0;
	}
	private static void OpenTxtFile(string path)
	{
		if (Path.GetExtension(path) != ".txt")
		{
			throw new ArgumentException("File must have extension .txt", nameof(path));
		}
		Process.Start("notepad.exe", path);
	}

	private void UpdateEXModel(object sender, RoutedEventArgs e)
	{
		if (hInput == null || !double.TryParse(hInput.Text, out _h) || !IsPositive(_h))
		{
			return;
		}

		if (w1Input == null || !double.TryParse(w1Input.Text, out _w1) || !IsPositive(_w1))
		{
			return;
		}

		_viewModel.UpdateEXModel(_w1, _h);
		exPlot.InvalidatePlot();
		exPlot.ResetAllAxes();
	}
	private void UpdateEHModel(object sender, RoutedEventArgs e)
	{
		if (w2Input == null || !double.TryParse(w2Input.Text, out _w2) || !IsPositive(_w2))
		{
			return;
		}

		if (startHInput == null || !double.TryParse(startHInput.Text, out _startH) || !IsPositive(_startH))
		{
			return;
		}

		if (endHInput == null || !double.TryParse(endHInput.Text, out _endH) || !IsPositive(_endH) || _endH < _startH)
		{
			return;
		}

		if (normChoice == null)
		{
			return;
		}
		_norm = (Norm)normChoice.SelectedItem;

		_viewModel.UpdateEHModel(_w2, _startH, _endH, _norm);
		ehPlot.InvalidatePlot();
		ehPlot.ResetAllAxes();
	}

	private void DownloadEXData(object sender, RoutedEventArgs e)
	{
		var (xArr, ySeries) = _viewModel.GetEXData(_w1, _h);

		string tableString = Drawer.GetTableString(xArr, "x", ySeries, "e(x)", Precision);
		string data = string.Join(Environment.NewLine, $"w = {_w1}", $"h = {_h}", tableString);

		string path = Path.Combine(DataSaveDirectoryName, EXDataFileName);
		File.WriteAllText(path, data);
		OpenTxtFile(path);
	}
	private void DownloadEHData(object sender, RoutedEventArgs e)
	{
		var (xArr, ySeries) = _viewModel.GetEHData(_w2, _startH, _endH, _norm);

		string tableString = Drawer.GetTableString(xArr, "h", ySeries, "e(h)", Precision);
		string data = string.Join(Environment.NewLine, $"w = {_w2}", $"h: [{_startH}; {_endH}]", $"Norm: {_norm}", tableString);

		string path = Path.Combine(DataSaveDirectoryName, EHDataFileName);
		File.WriteAllText(path, data);
		OpenTxtFile(path);
	}
}
