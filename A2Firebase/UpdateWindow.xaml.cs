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

namespace A2Firebase.Images
{
    /// <summary>
    /// Lógica de interacción para UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        private Character character;
        private FirebaseClient firebaseClient;
        private FirebaseStorage firebaseStorage;

        public UpdateWindow(Character character, FirebaseClient firebaseClient, FirebaseStorage firebaseStorage)
        {
            InitializeAsync(character, firebaseClient, firebaseStorage);   
        }

        private void InitializeAsync(Character character, FirebaseClient firebaseClient, FirebaseStorage firebaseStorage)
        {
            InitializeComponent();
            this.character = character;
            this.firebaseClient = firebaseClient;
            this.firebaseStorage = firebaseStorage;
            txtName.Text = character.Name;
            txtDescription.Text = character.Description;
            txtShow.Text = character.TvShow;
            txtFile.Text = character.Image;
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if(txtName.Text == "" || txtDescription.Text == "" || txtShow.Text == "" || txtFile.Text == "")
            {
                MessageBox.Show("Please fill all the fields");
            }
            else
            {
                try
                {
                    try
                    {
                        //Delete the old image from the storage
                        await firebaseStorage
                                .Child("Characters")
                                .Child($"{character.Name}")
                                .DeleteAsync();
                    } catch(Exception ex)
                    {
                    }
                    

                    //Add the new image to the storage
                    var stream = File.Open(txtFile.Text, FileMode.Open);

                    var task = await firebaseStorage
                        .Child("Characters")
                        .Child(character.Name)
                        .PutAsync(stream);

                    var downloadUrl = await firebaseStorage
                        .Child("Characters")
                        .Child(character.Name)
                        .GetDownloadUrlAsync();

                    character.TvShow = txtShow.Text;
                    character.Description = txtDescription.Text;
                    character.Image = downloadUrl;


                    await firebaseClient
                        .Child("Characters")
                        .Child($"{character.Name}")
                        .PutAsync(character);

                    this.Close();
                } catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
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
    }
}
