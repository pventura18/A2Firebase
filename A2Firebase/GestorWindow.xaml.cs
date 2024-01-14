using A2Firebase.Images;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace A2Firebase
{
    /// <summary>
    /// Lógica de interacción para GestorWindow.xaml
    /// </summary>
    public partial class GestorWindow : Window
    {
        List<Character> CharacterList { get; set; }
        FirebaseClient firebaseClient;
        FirebaseStorage firebaseStorage;
        public GestorWindow(UserCredential user)
        {
            InitializeAsync(user);
        }

        private async Task InitializeAsync(UserCredential user)
        {
            InitializeComponent();
            firebaseClient = new FirebaseClient(
                "https://provadam-f54b4-default-rtdb.europe-west1.firebasedatabase.app/",
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => user.User.GetIdTokenAsync()
                });
            firebaseStorage = new FirebaseStorage(
                "provadam-f54b4.appspot.com",
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => user.User.GetIdTokenAsync()
                });
            CharacterList = await GetCharactersFromFirebase();
            lvCharacters.ItemsSource = CharacterList;
        }

        private async Task<List<Character>> GetCharactersFromFirebase()
        {
            var characters = await firebaseClient
                .Child("Characters")
                .OnceAsync<Character>();

            foreach (var character in characters)
            {
                character.Object.Name = character.Key;
            }

            return characters.Select(c => c.Object).ToList();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(txtName.Text != "" && txtDescription.Text != "" && txtShow.Text != "" && txtFile.Text != "")
            {
                try
                {
                    var stream = File.Open(txtFile.Text, FileMode.Open);

                    var task = await firebaseStorage
                        .Child("Characters")
                        .Child(txtName.Text)
                        .PutAsync(stream);

                    var downloadUrl = await firebaseStorage
                        .Child("Characters")
                        .Child(txtName.Text)
                        .GetDownloadUrlAsync();

                    Character character = new Character
                    {
                        Name = txtName.Text,
                        Description = txtDescription.Text,
                        TvShow = txtShow.Text,
                        Date = DateTime.Now,
                        Image = downloadUrl
                    };

                    await firebaseClient
                        .Child("Characters")
                        .Child(character.Name)
                        .PutAsync(character);

                    
                    CharacterList.Add(character);
                    lvCharacters.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
                
            }
            else
            {
                MessageBox.Show("Falten camps per omplir");
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            if(txtFilter.Text == "")
            {
                lvCharacters.ItemsSource = CharacterList;
            }
            else
            {
                string filterText = txtFilter.Text.Trim().ToLower();


                var filteredCharacters = CharacterList.Where(c => c.Name.ToLower().Contains(filterText)).ToList();
                lvCharacters.ItemsSource = filteredCharacters;
            }
            
        }

        private void btnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;
                txtFile.Text = imagePath;
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                Character character = btn.DataContext as Character;
                if (character != null)
                {
                    try
                    {
                        UpdateWindow updateWindow = new UpdateWindow(character, firebaseClient, firebaseStorage);
                        updateWindow.ShowDialog();

                        lvCharacters.Items.Refresh();

                        MessageBox.Show("Character updated!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                Character character = btn.DataContext as Character;
                if (character != null)
                {
                    try
                    {
                        await firebaseClient
                            .Child("Characters")
                            .Child($"{character.Name}")
                            .DeleteAsync();

                        await firebaseStorage
                            .Child("Characters")
                            .Child($"{character.Name}")
                            .DeleteAsync();

                        CharacterList.Remove(character);
                        lvCharacters.Items.Refresh();

                        MessageBox.Show("Character deleted!");
                    } catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }          
                }
            }
        }
    }
}
