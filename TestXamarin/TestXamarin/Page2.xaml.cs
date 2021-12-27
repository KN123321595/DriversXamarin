using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace TestXamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page2 : ContentPage
    {
        private double startLatitude;
        private double endLatitude;
        private double startLongitude;
        private double endLongitude;

        private TimeSpan timeStartDay;
        private TimeSpan timeEndDay;
        private TimeSpan timeStartDinner;
        private TimeSpan timeEndDinner;

     
        enum typeLocation { startLocation, endLocation}

        public Page2()
        {
            InitializeComponent();

            //логика выгрузки автомобилей
            //и пробега из бд
            numberPicker.Items.Add("123");
        }

        private async void ButtonDay_Clicked(object sender, EventArgs e)
        {
            

            if (dayButton.Text == "Начать день")
            {

                //логика начала дня

                await GetCurrentLocation(typeLocation.startLocation);

                if (startLatitude == 0 || startLongitude == 0)
                {
                    await DisplayAlert("Уведомление", "Пожалуйта, включите геолокацию", "ОК");
                    return;
                }

                timeStartDay = DateTime.Now.TimeOfDay;

                await DisplayAlert("Уведомление", $"Широта: { startLatitude } Долгота: { startLongitude } ", "ОК");

                dayButton.Text = "Окончить день";

                //CheckFillElements();
            }

            else
            {
                //логика окончания дня

                await GetCurrentLocation(typeLocation.endLocation);

                if (endLatitude == 0 || endLongitude == 0)
                {
                    await DisplayAlert("Уведомление", "Пожалуйта, включите геолокацию", "ОК");
                    return;
                }

                timeEndDay = DateTime.Now.TimeOfDay;

                await DisplayAlert("Уведомление", $"Широта: { endLatitude } Долгота: { endLongitude }", "ОК");

                dayButton.IsEnabled = false;

                //CheckFillElements();
            }

        }

        private void ButtonDinner_Clicked(object sender, EventArgs e)
        {
            if (dinnerButton.Text == "Начать обед")
            {

                //логика начала обеда

                timeStartDinner = DateTime.Now.TimeOfDay;

                dinnerButton.Text = "Окончить обед";
            }

            else
            {
                //логика окончания обеда

                timeEndDinner = DateTime.Now.TimeOfDay;

                dinnerButton.IsEnabled = false;
            }
        }

        private void ButtonSend_Clicked(object sender, EventArgs e)
        {
            //логика отправки сообщения
            DisplayAlert("", $"Широта1: { startLatitude } Долгота1: { startLongitude } Широта2: { endLatitude } Долгота2: { endLongitude } Время1: { timeStartDay }  Время2: { timeEndDay }  Время3: { timeStartDinner }  Время4: { timeEndDinner }", "ОК");
        }

    //получение геопозиции
        private async Task GetCurrentLocation(typeLocation type)
        {
            CancellationTokenSource cts;
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);
            
                if (location != null)
                {
                    //Debug.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    if (type == typeLocation.startLocation)
                    {
                        startLatitude = location.Latitude;
                        startLongitude = location.Longitude;
                    }

                    else if (type == typeLocation.endLocation)
                    {
                        endLatitude = location.Latitude;
                        endLongitude = location.Longitude;
                    }

                }

                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }

        }

        private void runStartEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            //CheckFillElements();
        }

        private void numberPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            //CheckFillElements();
        }

        //проверить заполненость элементов, для появления кнопки отправить данные
        private void CheckFillElements()
        {
            
            TimeSpan time = new TimeSpan(0, 0, 0);
            if (startLatitude == 0 || endLatitude == 0 || startLongitude == 0 || endLongitude == 0 || numberPicker.SelectedIndex == -1 || TimeSpan.Compare(timeStartDay, time) == 0 || TimeSpan.Compare(timeEndDay, time) == 0 || runStartEntry.Text == null || runEndEntry.Text == null)
            {
                sendbutton.IsVisible = false;
            }

            else
            { 
            sendbutton.IsVisible = true;
            }
      
        }

        
    }
}