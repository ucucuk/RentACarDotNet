using Microsoft.VisualBasic.ApplicationServices;
using RentACarDotNetCore.Application.Responses.User;
using System.Net.Http.Json;

namespace WebUIWithWindowsForm
{
    public partial class Form1 : Form
    {
        private string url = "https://localhost:44335/api/Users";
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var users = await httpClient.GetFromJsonAsync<List<GetUserResponse>>(new Uri(url));
                dataGridView1.DataSource = users;
            }
        }
    }
}
