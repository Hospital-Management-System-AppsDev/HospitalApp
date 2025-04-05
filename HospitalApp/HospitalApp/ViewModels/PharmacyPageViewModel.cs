using System.Collections.ObjectModel;
using System.Linq;
using HospitalApp.Models;

namespace HospitalApp.ViewModels
{
    public class PharmacyPageViewModel : ViewModelBase
    {
        private string _searchText = string.Empty;
        private ObservableCollection<Medicine> _filteredItems;

        public ObservableCollection<Medicine> Medicines { get; } = new()
        {
            new Medicine { Name = "Paracetamol" },
            new Medicine { Name = "Loperamide" },
            new Medicine { Name = "Ibuprofen" },
            new Medicine { Name = "Amoxicillin" },
            new Medicine { Name = "Cetirizine" },
            new Medicine { Name = "Aspirin" },
            new Medicine { Name = "Metformin" },
            new Medicine { Name = "Simvastatin" },
            new Medicine { Name = "Omeprazole" },
            new Medicine { Name = "Losartan" },
            new Medicine { Name = "Atorvastatin" },
            new Medicine { Name = "Levothyroxine" },
            new Medicine { Name = "Amlodipine" },
            new Medicine { Name = "Gabapentin" },
            new Medicine { Name = "Prednisone" },
            new Medicine { Name = "Albuterol" },
            new Medicine { Name = "Tramadol" },
            new Medicine { Name = "Hydrochlorothiazide" },
            new Medicine { Name = "Furosemide" },
            new Medicine { Name = "Sertraline" },
            new Medicine { Name = "Fluoxetine" },
            new Medicine { Name = "Tamsulosin" },
            new Medicine { Name = "Ciprofloxacin" },
            new Medicine { Name = "Azithromycin" },
            new Medicine { Name = "Doxycycline" },
            new Medicine { Name = "Ranitidine" },
            new Medicine { Name = "Pantoprazole" },
            new Medicine { Name = "Diazepam" },
            new Medicine { Name = "Clonazepam" },
            new Medicine { Name = "Lorazepam" },
            new Medicine { Name = "Warfarin" },
            new Medicine { Name = "Rosuvastatin" },
            new Medicine { Name = "Duloxetine" },
            new Medicine { Name = "Venlafaxine" },
            new Medicine { Name = "Escitalopram" },
            new Medicine { Name = "Citalopram" },
            new Medicine { Name = "Bupropion" },
            new Medicine { Name = "Mirtazapine" },
            new Medicine { Name = "Hydroxyzine" },
            new Medicine { Name = "Montelukast" },
            new Medicine { Name = "Budesonide" },
            new Medicine { Name = "Mometasone" },
            new Medicine { Name = "Fluticasone" },
            new Medicine { Name = "Metoprolol" },
            new Medicine { Name = "Propranolol" },
            new Medicine { Name = "Bisoprolol" },
            new Medicine { Name = "Carvedilol" },
            new Medicine { Name = "Lisinopril" },
            new Medicine { Name = "Enalapril" },
            new Medicine { Name = "Ramipril" },
            new Medicine { Name = "Diltiazem" },
            new Medicine { Name = "Verapamil" },
            new Medicine { Name = "Nifedipine" },
            new Medicine { Name = "Digoxin" },
            new Medicine { Name = "Insulin Glargine" },
            new Medicine { Name = "Insulin Aspart" },
            new Medicine { Name = "Sitagliptin" },
            new Medicine { Name = "Glimepiride" },
            new Medicine { Name = "Pioglitazone" },
            new Medicine { Name = "Dapagliflozin" },
            new Medicine { Name = "Empagliflozin" },
            new Medicine { Name = "Ezetimibe" },
            new Medicine { Name = "Fenofibrate" },
            new Medicine { Name = "Omega-3 Acid Ethyl Esters" },
            new Medicine { Name = "Clopidogrel" },
            new Medicine { Name = "Aspirin-Dipyridamole" },
            new Medicine { Name = "Rivaroxaban" },
            new Medicine { Name = "Apixaban" },
            new Medicine { Name = "Dabigatran" },
            new Medicine { Name = "Methotrexate" },
            new Medicine { Name = "Hydroxychloroquine" },
            new Medicine { Name = "Sulfasalazine" },
            new Medicine { Name = "Colchicine" },
            new Medicine { Name = "Allopurinol" },
            new Medicine { Name = "Febuxostat" },
            new Medicine { Name = "Tizanidine" },
            new Medicine { Name = "Baclofen" },
            new Medicine { Name = "Cyclobenzaprine" },
            new Medicine { Name = "Methocarbamol" },
            new Medicine { Name = "Orphenadrine" },
            new Medicine { Name = "Ondansetron" },
            new Medicine { Name = "Meclizine" },
            new Medicine { Name = "Promethazine" },
            new Medicine { Name = "Prochlorperazine" },
            new Medicine { Name = "Metoclopramide" },
            new Medicine { Name = "Sumatriptan" },
            new Medicine { Name = "Rizatriptan" },
            new Medicine { Name = "Zolmitriptan" },
            new Medicine { Name = "Topiramate" },
            new Medicine { Name = "Lamotrigine" },
            new Medicine { Name = "Phenytoin" },
            new Medicine { Name = "Carbamazepine" },
            new Medicine { Name = "Valproic Acid" },
            new Medicine { Name = "Levetiracetam" },
            new Medicine { Name = "Ethosuximide" },
            new Medicine { Name = "Ketoconazole" },
            new Medicine { Name = "Fluconazole" },
            new Medicine { Name = "Itraconazole" },
            new Medicine { Name = "Miconazole" }
        };

        public PharmacyPageViewModel()
        {
            _filteredItems = new ObservableCollection<Medicine>(Medicines);
        }

        public ObservableCollection<Medicine> FilteredItems
        {
            get => _filteredItems;
            private set
            {
                _filteredItems = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    FilterMedicines();
                }
            }
        }

        private void FilterMedicines()
        {
            if (string.IsNullOrWhiteSpace(_searchText))
            {
                FilteredItems = new ObservableCollection<Medicine>(Medicines);
            }
            else
            {
                var filtered = Medicines
                    .Where(m => m.Name.ToLower().Contains(_searchText.ToLower()))
                    .ToList();

                FilteredItems = new ObservableCollection<Medicine>(filtered);
            }
        }
    }
}