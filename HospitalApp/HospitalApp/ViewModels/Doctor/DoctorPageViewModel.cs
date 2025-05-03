using System;
using HospitalApp.Services;

namespace HospitalApp.ViewModels{
    public class DoctorPageViewModel:ViewModelBase
    {
        private readonly ApiService _apiService;
        private readonly SignalRService _signalRService;
        public DoctorPageViewModel(ApiService _apiService, SignalRService _signalRService){
            this._apiService = _apiService;
            this._signalRService = _signalRService;
        }
    }
}
 