using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace A2Firebase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FirebaseAuthConfig config;

        FirebaseAuthClient client;

        public MainWindow()
        {
            config = new FirebaseAuthConfig
            {
                ApiKey = "AIzaSyCiZYAMbXtArVjxB4_b44HVETVbPb4IBEM",
                AuthDomain = "provadam-f54b4.firebaseapp.com",

                Providers = new FirebaseAuthProvider[]
{
    new GoogleProvider(),
    new EmailProvider()
},
                UserRepository = new FileUserRepository("FirebaseSample")
            };

            client = new FirebaseAuthClient(config);

            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get email and password from the TextBoxes
                string email = tbEmailLogin.Text;
                string password = tbPasswordLogin.Text;

                // Perform Firebase authentication
                var user = await client.SignInWithEmailAndPasswordAsync(email, password);

                // Authentication successful, you can now handle the user or navigate to another page
                MessageBox.Show($"Logged in");

                GestorWindow gestorWindow = new GestorWindow(user);
                gestorWindow.ShowDialog();
            }
            catch (FirebaseAuthException ex)
            {
                // Handle authentication failure
                MessageBox.Show($"Login failed: {ex.Message}");
            }
        }

        private async void btnSignup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get email, password, and name from the TextBoxes
                string email = tbEmailSignup.Text;
                string password = tbPasswordSignup.Text;
                string name = tbNameSignup.Text;

                // Perform Firebase signup
                var user = await client.CreateUserWithEmailAndPasswordAsync(email, password, name);

                // Signup successful, you can now handle the new user or navigate to another page
                MessageBox.Show($"Signed up");
            }
            catch (FirebaseAuthException ex)
            {
                // Handle signup failure
                MessageBox.Show($"Sign up failed: {ex.Message}");
            }
        }

    }
}
