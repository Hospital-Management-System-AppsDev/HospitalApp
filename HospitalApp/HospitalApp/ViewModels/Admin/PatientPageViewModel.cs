using System;
using Avalonia;

namespace HospitalApp.ViewModels;

public partial class PatientPageViewModel:ViewModelBase
{
    private readonly ApiService _apiService;
    private readonly SignalRService _signalRService;
    public PatientPageViewModel(ApiService _apiService, SignalRService _signalRService){
        this._apiService = _apiService;
        this._signalRService = _signalRService;
    }
}
