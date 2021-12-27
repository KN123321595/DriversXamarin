using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestXamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        public Page1()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (loginEntry.Text != null && passwordEntry.Text != null)
            {
                string login = loginEntry.Text.Trim();
                string password = passwordEntry.Text.Trim();


                //логика прохождения авторизации


                //await Navigation.PushModalAsync(new Page2());
                App.Current.MainPage = new NavigationPage(new Page2());
            }


            
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {

        }
    }
}