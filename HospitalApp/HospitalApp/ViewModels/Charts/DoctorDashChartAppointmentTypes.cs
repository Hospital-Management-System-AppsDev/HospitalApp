using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HospitalApp.Services;

namespace HospitalApp.ViewModels;

public class DoctorDashChartAppointmentTypesViewModel
{
    private readonly ApiService _apiService = new ApiService();
    private readonly UserSessionService _session = UserSessionService.Instance;

    public string Title { get; set; } = "Types of Appointments";

    public ISeries[] Series { get; set; } = new ISeries[] { };

    public SolidColorPaint LegendTextPaint { get; set; } = new SolidColorPaint
    {
        Color = SKColors.Black,
        SKTypeface = SKTypeface.FromFamilyName("Arial")
    };

    public DoctorDashChartAppointmentTypesViewModel()
    {
        _ = Initialize();
    }

    public async Task Initialize()
    {
        int doctorId = _session.CurrentUser.Id;

        var data = await _apiService.GetAppointmentsByType(doctorId);

        Series = data.Select(kv => new PieSeries<int>
        {
            Name = kv.Key,
            Values = new[] { kv.Value },
            DataLabelsSize = 14,
            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
            DataLabelsPaint = new SolidColorPaint(SKColors.White),
        }).ToArray();
    }
}
