using Contracts;
using Microsoft.Win32;
using NbrbAPI.Models;
using Newtonsoft.Json;
using Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TelerikFinanceApp.Models;

namespace FinanceApp.MVVM
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Collection of currency rates displayed in the grid.
        private ObservableCollection<CurrencyToDisplay> _currenciesToDisplay;
        public ObservableCollection<CurrencyToDisplay> CurrenciesToDisplay
        {
            get { return _currenciesToDisplay; }
            set
            {
                if (_currenciesToDisplay != value)
                {
                    _currenciesToDisplay = value;
                    OnPropertyChanged(nameof(CurrenciesToDisplay));
                }
            }
        }

        // Currently selected currency rate in the grid.
        private CurrencyToDisplay _selectedRate;
        public CurrencyToDisplay SelectedRate
        {
            get { return _selectedRate ?? _currenciesToDisplay.FirstOrDefault(); }
            set
            {
                if (_selectedRate != value)
                {
                    _selectedRate = value;
                    OnPropertyChanged(nameof(SelectedRate));
                    Task.Run(() => OnRateSelected()); 
                }
            }
        }

        // List of currency rates used for chart data.
        private List<RateShort> _rates;
        public List<RateShort> Rates
        {
            get { return _rates; }
            set
            {
                if (_rates != value)
                {
                    _rates = value;
                    OnPropertyChanged(nameof(Rates));
                }
            }
        }

        // Event handler when a currency rate is selected.
        private async Task OnRateSelected()
        {
            try
            {
                var rateId = SelectedRate.Cur_ID;
                // Load and update rates for the selected currency for the past year.
                Rates = await _currencyLoaderService.GetRatesDynamicsAsync(rateId, StartDate, EndDate);
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox("Error", $"An error occurred in OnRateSelected: {ex.Message}");
            }
        }

        // Command to execute when the "Load currencies" button is clicked.
        public ICommand ClickCommand { get; }

        // Service for loading currency data.
        private readonly ICurrencyLoaderService _currencyLoaderService;

        // Command to execute when the "Save to JSON" button is clicked.
        public ICommand SaveToJsonCommand { get; }

        // Command to execute when the "Load from JSON" button is clicked.
        public ICommand LoadFromJsonCommand { get; }

        private DateTime _startDate = DateTime.Now.AddMonths(-12);
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (value > _endDate || (value.AddMonths(12) < _endDate))
                {
                    ShowErrorMessageBox("Error", "Invalid Start Date. Difference between Start and End dates must not exceed one year.");
                }
                else
                {
                    _startDate = value;
                    UpdateChartSettings();
                    OnPropertyChanged(nameof(StartDate));
                }
            }
        }

        private DateTime _endDate = DateTime.Now;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (value < _startDate)
                {
                    ShowErrorMessageBox("Error", "Invalid End Date. End date must be greater than or equal to Start date.");
                }
                else
                {
                    _endDate = value;
                    UpdateChartSettings();
                    OnPropertyChanged(nameof(EndDate));
                }
            }
        }

        private string _chartMajorStepUnit = "Month";
        public string ChartMajorStepUnit
        {
            get { return _chartMajorStepUnit; }
            set
            {
                if (_chartMajorStepUnit != value)
                {
                    _chartMajorStepUnit = value;
                    OnPropertyChanged(nameof(ChartMajorStepUnit));
                }
            }
        }

        private int _chartMajorStep = 3; // Default to 3 months
        public int ChartMajorStep
        {
            get { return _chartMajorStep; }
            set
            {
                if (_chartMajorStep != value)
                {
                    _chartMajorStep = value;
                    OnPropertyChanged(nameof(ChartMajorStep));
                }
            }
        }

        private void UpdateChartSettings()
        {
            (ChartMajorStep, ChartMajorStepUnit) = DateRangeToChartSettingsConverter.GetChartSettings(StartDate, EndDate);
        }

        public MainViewModel(ICurrencyLoaderService currencyLoaderService)
        {
            ClickCommand = new RelayCommand(ExecuteLoadCurrenciesClick);
            SaveToJsonCommand = new RelayCommand(SaveToJson);
            LoadFromJsonCommand = new RelayCommand(LoadFromJson);
            _currencyLoaderService = currencyLoaderService;
        }

        // Save currencies data to a JSON file.
        private void SaveToJson(object parameter)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json",
                    Title = "Save Currencies Data",
                    DefaultExt = "json"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    // Serialize and save the currencies data to the selected file.
                    string json = JsonConvert.SerializeObject(CurrenciesToDisplay);
                    File.WriteAllText(saveFileDialog.FileName, json);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox("Error", $"An error occurred in SaveToJson: {ex.Message}");
            }
        }

        // Load currencies data from a JSON file.
        private void LoadFromJson(object parameter)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "JSON files (*.json)|*.json",
                    Title = "Load Currencies Data"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    // Deserialize and update the currencies collection from the selected file.
                    string json = File.ReadAllText(openFileDialog.FileName);
                    CurrenciesToDisplay = JsonConvert.DeserializeObject<ObservableCollection<CurrencyToDisplay>>(json);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox("Error", $"An error occurred in LoadFromJson: {ex.Message}");
            }
        }

        // Event handler for "Load currencies" button click.
        private async void ExecuteLoadCurrenciesClick(object parameter)
        {
            try
            {
                // Load and update the currencies collection from the official rates API.
                var allCurrencies = await _currencyLoaderService.GetCurrenciesAsync();
                var allRates = await _currencyLoaderService.GetOfficialRatesAsync();

                var currenciesToDisplay = new ObservableCollection<CurrencyToDisplay>();

                foreach (var currency in allCurrencies)
                {
                    var currRate = allRates.Find(rate => rate.Cur_ID == currency.Cur_ID);
                    currenciesToDisplay.Add(new CurrencyToDisplay() {
                        Cur_ID = currency.Cur_ID,
                        Cur_Abbreviation = currency.Cur_Abbreviation,
                        Cur_Name = currency.Cur_Name,
                        Date = currRate?.Date,
                        Cur_OfficialRate = currRate?.Cur_OfficialRate
                    });
                }

                CurrenciesToDisplay = currenciesToDisplay;
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox("Error", $"An error occurred in ExecuteClick: {ex.Message}");
            }
        }

        private void ShowErrorMessageBox(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // INotifyPropertyChanged event.
        public event PropertyChangedEventHandler PropertyChanged;

        // Invokes the PropertyChanged event.
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}


