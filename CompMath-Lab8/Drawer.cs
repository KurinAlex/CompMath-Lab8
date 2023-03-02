using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompMath_Lab8;

public static class Drawer
{
	private static string Center(this string s, int length)
		=> s.PadLeft((length - s.Length) / 2 + s.Length).PadRight(length);

	public static string GetTableString(
		IEnumerable<double> xData,
		string xLabel,
		Dictionary<string, IEnumerable<double>> yData,
		string yAreaLabel,
		int precision)
	{
		if (precision < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(precision), "Precision must be non-negative");
		}

		int n = xData.Count();
		if (yData.Values.Any(d => d.Count() != n))
		{
			throw new ArgumentException("Wrong entries count in y data array", nameof(yData));
		}

		// id column data
		string idLabel = "id";
		var idStrings = Enumerable.Range(1, n).Select(d => d.ToString());
		int idColumnWidth = idStrings.Append(idLabel).Max(x => x.Length);
		var id = idStrings.Select(x => x.Center(idColumnWidth)).ToArray();

		// x column data
		var xStrings = xData.Select(x => x.ToString($"F{precision}"));
		int xColumnWidth = xStrings.Append(xLabel).Max(x => x.Length);
		var x = xStrings.Select(x => x.Center(xColumnWidth)).ToArray();

		// y columns data
		var yStrings = yData.ToDictionary(p => p.Key, p => p.Value.Select(y => y.ToString($"E{precision}")));
		var yColumnsWidth = yStrings.Select(col => col.Value.Append(col.Key).Max(y => y.Length));
		int yAreaWidth = yColumnsWidth.Sum() + yData.Count - 1;
		var y = yStrings
			.Zip(yColumnsWidth)
			.ToDictionary(
				t => t.First.Key.Center(t.Second),
				t => t.First.Value.Select(y => y.Center(t.Second)).ToArray());

		// dividers and spaces
		string idRowDivider = new('─', idColumnWidth);
		string idRowSpace = new(' ', idColumnWidth);
		string xRowDivider = new('─', xColumnWidth);
		string xRowSpace = new(' ', xColumnWidth);
		string yAreaDivider = new('─', yAreaWidth);
		var yRowsDividers = yColumnsWidth.Select(w => new string('─', w));

		// center the labels
		idLabel = idLabel.Center(idColumnWidth);
		xLabel = xLabel.Center(xColumnWidth);
		yAreaLabel = yAreaLabel.Center(yAreaWidth);

		var sb = new StringBuilder();
		sb.AppendFormat("┌{0}┬{1}┬{2}┐", idRowDivider, xRowDivider, yAreaDivider).AppendLine();
		sb.AppendFormat("│{0}│{1}│{2}│", idRowSpace, xRowSpace, yAreaLabel).AppendLine();
		sb.AppendFormat("│{0}│{1}├{2}┤", idLabel, xLabel, string.Join('┬', yRowsDividers)).AppendLine();
		sb.AppendFormat("│{0}│{1}│{2}│", idRowSpace, xRowSpace, string.Join('│', y.Keys)).AppendLine();
		for (int i = 0; i < n; i++)
		{
			sb.AppendFormat("├{0}┼{1}┼{2}┤", idRowDivider, xRowDivider, string.Join('┼', yRowsDividers)).AppendLine();
			sb.AppendFormat("│{0}│{1}│{2}│", id[i], x[i], string.Join('│', y.Values.Select(y => y[i]))).AppendLine();
		}
		sb.AppendFormat("└{0}┴{1}┴{2}┘", idRowDivider, xRowDivider, string.Join('┴', yRowsDividers));

		return sb.ToString();
	}
}
