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
using System.Windows.Input;

namespace FinanceApp.MVVM
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Collection of currency rates displayed in the grid.
        private ObservableCollection<Rate> _currencies;
        public ObservableCollection<Rate> Currencies
        {
            get { return _currencies; }
            set
            {
                if (_currencies != value)
                {
                    _currencies = value;
                    OnPropertyChanged(nameof(Currencies));
                }
            }
        }

        // Currently selected currency rate in the grid.
        private Rate _selectedRate;
        public Rate SelectedRate
        {
            get { return _selectedRate ?? _rates.FirstOrDefault(); }
            set
            {
                if (_selectedRate != value)
                {
                    _selectedRate = value;
                    OnPropertyChanged(nameof(SelectedRate));
                    OnRateSelected();
                }
            }
        }

        // List of currency rates used for chart data.
        private List<Rate> _rates;
        public List<Rate> Rates
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
                DateTime today = DateTime.Now;
                DateTime lastYear = today.AddYears(-1);

                // Load and update rates for the selected currency for the past year.
                Rates = await _currencyLoaderService.GetRatesDynamicsAsync(rateId, lastYear, today);
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
                    string json = JsonConvert.SerializeObject(Currencies);
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
                    Currencies = JsonConvert.DeserializeObject<ObservableCollection<Rate>>(json);
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
                var allData = await _currencyLoaderService.GetOfficialRatesAsync();
                Currencies = new ObservableCollection<Rate>(allData);                
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


