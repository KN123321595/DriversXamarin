using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestXamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        private NetworkAccess current;

        private readonly HttpClient client = new HttpClient();


        public static string login;
        public static string password;


        public Page1()
        {
            InitializeComponent();        
        }

        protected override void OnAppearing()
        {
            
            base.OnAppearing();
            Internet_Connect();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            
            if (loginEntry.Text != null && passwordEntry.Text != null)
            {
                login = loginEntry.Text.Trim();
                password = passwordEntry.Text.Trim();


                //логика прохождения авторизации

                //Dictionary<string, string> dict = new Dictionary<string, string>
                //{
                //    { "", "" }
                //};

                //FormUrlEncodedContent form = new FormUrlEncodedContent(dict);

                //HttpResponseMessage response = await client.PostAsync(ServerUrl.driversUrl, form);

                //string result = await response.Content.ReadAsStringAsync();



                //var content = new StringContent(JsonConvert.SerializeObject(new { login = login, password = password }));
                //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                //HttpResponseMessage response = await client.PostAsync(ServerUrl.driversUrl, content);

                //string result = await response.Content.ReadAsStringAsync();

                //await DisplayAlert(null, result, "ok");



                //4shFXwD4YYAM5uE

                //устанавливаем логин и пароль на сервер
                var authData = string.Format("{0}:{1}", login, password);
                var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);


                //получаем список водителей (нужен только положительный статус, чтобы узнать существует ли пользователь)
                HttpResponseMessage response = await client.GetAsync(ServerUrl.driversUrl);
                //string result = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //////
                    string loginUser = "1";
                    ///////


                    response = await client.GetAsync(ServerUrl.usersUrl + loginUser + "/");
                    string result = await response.Content.ReadAsStringAsync();

                    //делаем преобразования и устанавливаем логин и пароль на вторую страницу
                    char[] simv = { '[', ']', '{', '}', '\"' };

                    foreach (char c in simv)
                    {
                        result = result.Replace(c.ToString(), "");
                    }

                    string[] words = result.Split(',');

                    string[] idWords = words[0].Split(':');
                    Page2.userId = Convert.ToInt32(idWords[1]);

                    string[] usernameWords = words[1].Split(':');
                    Page2.userName = usernameWords[1];

                    
                    //await Navigation.PushModalAsync(new Page2());
                    App.Current.MainPage = new NavigationPage(new Page2());
                }

                else
                {
                    await DisplayAlert("Ошибка авторизации", "Логин или пароль введены неверно\nОбратите внимание на язык и регистр", "ОК");
                }

                

            }

        }

        //проверка подключения к интернету
        private async void Internet_Connect()
        {
            bool noInternet = true;

            while (noInternet)
            {
                current = Connectivity.NetworkAccess;

                if (current == NetworkAccess.None)
                {
                    await DisplayAlert("Нет подключения к интернету", "Пожалуйста, включите сеть и закройте это окно", "ОК");
                }

                else
                {
                    noInternet = false;
                }
            }
        }

    }
}