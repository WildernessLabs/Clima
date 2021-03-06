﻿using Clima.Contracts.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WildernessLabs.Clima.App.Models;
using Xamarin.Forms;

namespace WildernessLabs.Clima.App
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ClimaModel> ClimateList { get; set; }

        string ipAddress;
        public string IpAddress 
        {
            get => ipAddress;
            set { ipAddress = value; OnPropertyChanged(nameof(IpAddress)); } 
        }

        bool isRefreshing;
        public bool IsRefreshing
        {
            get => isRefreshing;
            set { isRefreshing = value; OnPropertyChanged(nameof(IsRefreshing)); }
        }

        public Command GetHumidityCommand { private set; get; }

        public MainViewModel() 
        {
            ClimateList = new ObservableCollection<ClimaModel>();            

            GetHumidityCommand = new Command(async (s) => await GetReadingsAsync());
        }

        async Task GetReadingsAsync()
        {
            if (string.IsNullOrEmpty(IpAddress))
                throw new InvalidEnumArgumentException("You must enter a valid IP address.");

            ClimateList.Clear();

            var response = await NetworkManager.GetAsync(IpAddress);

            if (response != null)
            {
                string json = await response.Content.ReadAsStringAsync();
                var values = (List<ClimateReading>)System.Text.Json.JsonSerializer.Deserialize(json, typeof(List<ClimateReading>));

                foreach (ClimateReading value in values)
                {
                    ClimateList.Add(new ClimaModel() { Date = value.TimeOfReading.ToString(), Temperature = value.TempC });
                }                
            }

            IsRefreshing = false;
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
