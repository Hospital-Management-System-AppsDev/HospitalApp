using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using System.Collections.Generic;
using HospitalApp.Services;
using System;
using System.Threading.Tasks;


public class AdminDashChartPatientsViewModel
{
    private readonly ApiService _apiService = new ApiService();

    // Store the series instance so we can update it
    private LineSeries<int> _lineSeries;

    public AdminDashChartPatientsViewModel()
    {
        _lineSeries = new LineSeries<int>
        {
            Values = new List<int>(), // initially empty
            Fill = null,
            GeometrySize = 10
        };

        Series = new ISeries[] { _lineSeries };
        _ = Initialize(); // call async void method
    }

    private async Task Initialize()
    {
        var data = await _apiService.GetNumPatientsAsyc(DateTime.Now.Year, DateTime.Now.Month);

        _lineSeries.Values = data; // update the chart with the actual data
    }

    public ISeries[] Series { get; set; }

    public LabelVisual Title { get; set; } = new LabelVisual
    {
        Text = "New Patients/Month",
        TextSize = 15,
        Paint = new SolidColorPaint
        {
            Color = SKColor.Parse("#3E8DC0"),
            SKTypeface = SKTypeface.FromFamilyName("Rubik", SKFontStyle.Bold)
        }
    };

    public Axis[] XAxes { get; set; } =
    {
        new Axis {
            Name = "Month",
            TextSize = 15,
            NameTextSize = 15,    // Axis title text size
            MinStep = 1,
            Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }
        }
    };

    public Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            Name = "Number of Patients",
            MinStep = 1, // ensures spacing of at least 1 between ticks
            Labeler = value => ((int)value).ToString(), // converts to integer
            TextSize = 15,
            NameTextSize = 15,    // Axis title text size
        }
    };
}
