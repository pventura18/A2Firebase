using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth.UI;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace A2Firebase
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {
        public App()
        {
            // Force override culture & language
            //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("cs");
            //CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("cs");

            // Firebase UI initialization
            var config = new FirebaseAuthConfig
            {
                ApiKey = "AIzaSyCiZYAMbXtArVjxB4_b44HVETVbPb4IBEM",
                AuthDomain = "provadam-f54b4.firebaseapp.com",

                UserRepository = new FileUserRepository("FirebaseSample")
            };

            var client = new FirebaseAuthClient(config);
        }
    }
}
