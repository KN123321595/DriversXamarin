using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

        private int step = -1;
        private const int STEP_START_DAY = 0;
        private const int STEP_DINNER = 1;
        private const int STEP_CONTINUE_WORK = 2;
        private const int STEP_END_WORK = 3;

        private readonly HttpClient client = new HttpClient();

        private List<Car> cars;
        private List<WorkingDay> days;

        private int workingDayId;
        enum typeLocation { startLocation, endLocation}


        public static int userId;
        public static string userName;
        

        public Page2()
        {
            InitializeComponent();

            
        }

        //запуск при инициализации страницы
        protected override async void OnAppearing()
        {
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var authData = string.Format("{0}:{1}", Page1.login, Page1.password);
            var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);


            //получаем автомобили и записываем номера машин
            HttpResponseMessage response = await client.GetAsync(ServerUrl.carsUrl);
            string result = await response.Content.ReadAsStringAsync();

            cars = JsonConvert.DeserializeObject<List<Car>>(result);

            foreach (Car car in cars)
            {
                numberPicker.Items.Add(car.number.ToString());                             
            }

            //получем актвных водителей, для получения процесса работы(шагов)
            response = await client.GetAsync(ServerUrl.active_drivers);
            result = await response.Content.ReadAsStringAsync();

    
            days = JsonConvert.DeserializeObject<List<WorkingDay>>(result);


            base.OnAppearing();
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

                step = STEP_START_DAY;

                //timeStartDay = DateTime.Now.TimeOfDay;

                //await DisplayAlert("Уведомление", $"Широта: { startLatitude } Долгота: { startLongitude } ", "ОК");

                var content = new StringContent(JsonConvert.SerializeObject(new { 
                    driver = userId, 
                    car = cars[numberPicker.SelectedIndex].id,
                    geolocation_start = startLatitude + "|" + startLongitude,
                    step = step,
                    mileage_start = cars[numberPicker.SelectedIndex].mileage
                }));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync(ServerUrl.workdays, content);

                //string result = await response.Content.ReadAsStringAsync();

                //await DisplayAlert(null, result, "ok");

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

                step = STEP_END_WORK;

                //timeEndDay = DateTime.Now.TimeOfDay;

                //await DisplayAlert("Уведомление", $"Широта: { endLatitude } Долгота: { endLongitude }", "ОК");


                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    working_day_close_status = true,
                    geolocation_end = endLatitude + "|" + endLongitude,
                    step = step
                })); ;
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PutAsync(ServerUrl.workdays + workingDayId, content);

                //string result = await response.Content.ReadAsStringAsync();

                //await DisplayAlert(null, result, "ok");

                dayButton.IsEnabled = false;
                sendbutton.IsEnabled = true;

                //CheckFillElements();
            }

        }

        private async void ButtonDinner_Clicked(object sender, EventArgs e)
        {
            if (dinnerButton.Text == "Начать обед")
            {

                //логика начала обеда
                step = STEP_DINNER;

                //timeStartDinner = DateTime.Now.TimeOfDay;              

                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    step = step
                }));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PutAsync(ServerUrl.workdays + workingDayId, content);

                //string result = await response.Content.ReadAsStringAsync();

                //await DisplayAlert(null, result, "ok");

                dinnerButton.Text = "Окончить обед";


            }

            else
            {
                //логика окончания обеда
                step = STEP_CONTINUE_WORK;

                //timeEndDinner = DateTime.Now.TimeOfDay;

                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    step = step
                }));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PutAsync(ServerUrl.workdays + workingDayId, content);

                //string result = await response.Content.ReadAsStringAsync();

                //await DisplayAlert(null, result, "ok");

                dinnerButton.IsEnabled = false;
            }
        }

        private async void ButtonSend_Clicked(object sender, EventArgs e)
        {
            
            if (runEndEntry.Text == null || runStartEntry.Text == null)
            {
                await DisplayAlert("Ввведены не все данные", "Пожалуйста, введите пробег на конец дня", "ОК");
                return;
            }

            //логика отправки сообщения
            //////

            await DisplayAlert("Спасибо за работу", "Данные успешно отправлены", "ОК");

            Process.GetCurrentProcess().CloseMainWindow();
        }


        private void numberPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            //устанавливаем пробег (если есть) на начало дня в зависимости от выбранного номера авто
            if (cars[numberPicker.SelectedIndex].mileage != 0)
            {
                runStartEntry.Text = cars[numberPicker.SelectedIndex].mileage.ToString();
                runStartEntry.IsEnabled = false;
            }

            SetWorkingDayStatus();
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

        //устанавливаем водителя и его статус для включения/отключения кнопок
        private void SetWorkingDayStatus()
        {
            UpdateButtons();

            foreach (WorkingDay day in days)
            {
                if (day.driver == userId)
                {
                    if (day.car == cars[numberPicker.SelectedIndex].id)
                    {
                        workingDayId = day.id;
                        step = day.step;
                    }
                }
            }

            if (step == STEP_START_DAY)
            {
                dayButton.Text = "Окончить день";
            }

            else if (step == STEP_DINNER)
            {
                dayButton.Text = "Окончить день";
                dinnerButton.Text = "Окончить обед";
            }

            else if (step == STEP_CONTINUE_WORK)
            {
                dayButton.Text = "Окончить день";
                dinnerButton.Text = "Окончить обед";
                dinnerButton.IsEnabled = false;
            }

            else if (step == STEP_END_WORK)
            {
                dayButton.Text = "Окончить день";
                dinnerButton.Text = "Окончить обед";
                dayButton.IsEnabled = false;
                dinnerButton.IsEnabled = false;
                sendbutton.IsEnabled = true;
            }
        }

        private void UpdateButtons()
        {
            dayButton.IsEnabled = true;
            dinnerButton.IsEnabled = true;
            dayButton.Text = "Начать день";
            dinnerButton.Text = "Начать обед";
        }

        //проверить заполненость элементов, для появления кнопки отправить данные
        //private void CheckFillElements()
        //{

        //    TimeSpan time = new TimeSpan(0, 0, 0);
        //    if (startLatitude == 0 || endLatitude == 0 || startLongitude == 0 || endLongitude == 0 || numberPicker.SelectedIndex == -1 || TimeSpan.Compare(timeStartDay, time) == 0 || TimeSpan.Compare(timeEndDay, time) == 0 || runStartEntry.Text == null || runEndEntry.Text == null)
        //    {
        //        sendbutton.IsVisible = false;
        //    }

        //    else
        //    { 
        //    sendbutton.IsVisible = true;
        //    }

        //}


    }
}