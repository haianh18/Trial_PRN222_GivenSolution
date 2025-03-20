using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace Project1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Star> Stars { get; set; } = new List<Star>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtDoB.Text) || string.IsNullOrEmpty(txtDescription.Text) || string.IsNullOrEmpty(txtNationality.Text))
            {
                MessageBox.Show("Please fill all the fields");
                return;
            }
            var star = new Star
            {
                Name = txtName.Text,
                Dob = DateTime.Parse(txtDoB.Text),
                Description = txtDescription.Text,
                Nationality = txtNationality.Text,
                Male = (bool)txtGender.IsChecked! ? true : false
            };
            Stars.Add(star);
            txtList.Text = string.Empty;
            foreach (var item in Stars)
            {
                txtList.Text += $"{item.Name}  {item.Dob?.ToString("d")}  {item.Description}  {item.Male} {item.Nationality} \n";
            }
        }

        private async void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            var json = JsonSerializer.Serialize(Stars);
            var data = Encoding.UTF8.GetBytes(json);

            try
            {
                using (var client = new TcpClient("127.0.0.1", 5000))
                using (var stream = client.GetStream())
                {
                    await stream.WriteAsync(data, 0, data.Length);

                    var buffer = new byte[1024];
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (response == "accepted")
                    {
                        MessageBox.Show("Data successfully sent to the server.");
                    }
                    else
                    {
                        MessageBox.Show("Error: Server did not accept the data.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
