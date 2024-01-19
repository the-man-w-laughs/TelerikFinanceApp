using Contracts;
using Microsoft.Win32;
using NbrbAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace FinanceApp.MVVM
{
    public class MainViewModel : INotifyPropertyChanged
    {
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

        private Rate _selectedRate;
        public Rate SelectedRate
        {
            get { return _selectedRate; }
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

        private void OnRateSelected()
        {
            throw new NotImplementedException();
        }

        public ICommand ClickCommand { get; }

        private readonly ICurrencyLoaderService _currencyLoaderService;
        public ICommand SaveToJsonCommand { get; }
        public ICommand LoadFromJsonCommand { get; }

        public MainViewModel(ICurrencyLoaderService currencyLoaderService)
        {
            ClickCommand = new RelayCommand(ExecuteClick);
            SaveToJsonCommand = new RelayCommand(SaveToJson);
            LoadFromJsonCommand = new RelayCommand(LoadFromJson);
            _currencyLoaderService = currencyLoaderService;
        }

        private void SaveToJson(object parameter)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Save Currencies Data",
                DefaultExt = "json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {

                string json = JsonConvert.SerializeObject(Currencies);
                File.WriteAllText(saveFileDialog.FileName, json);
            }
        }

        private void LoadFromJson(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Load Currencies Data"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                Currencies = JsonConvert.DeserializeObject<ObservableCollection<Rate>>(json);
            }
        }
        private async void ExecuteClick(object parameter)
        {
            var allData = await _currencyLoaderService.GetOfficialRatesAsync();
            Currencies = new ObservableCollection<Rate>(allData);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

