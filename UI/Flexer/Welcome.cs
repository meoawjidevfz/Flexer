using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Net.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Flexer_Evo.Welcome;
using System.Diagnostics;

namespace Flexer_Evo
{
    public partial class Welcome : Form
    {

        private string now_Version = "flexer-evo01";
        public Welcome()
        {
            InitializeComponent();
        }

        public class ResponseData
        {
            public string Id { get; set; }
            public string x_online { get; set; }
            public string dll_version { get; set; }
        }

        private async void check_Server()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://ro-exec.live/pro.php");

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        List<ResponseData> dataList = JsonSerializer.Deserialize<List<ResponseData>>(content);

                        ResponseData data = dataList[0];

                        if (data.x_online == "1")
                        {

                            if (data.dll_version == now_Version)
                            {

                                check_Hwid();

                            }
                            else
                            {

                                MessageBox.Show("Wrong version detected !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                Environment.Exit(0);

                            }

                        }

                        else
                        {

                            MessageBox.Show("Server Down !", "Flexer executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            Environment.Exit(0);

                        }
                    }
                    else
                    {

                        MessageBox.Show($"Request failed with status code: {response.StatusCode}");

                    }

                }

                catch (HttpRequestException e)

                {

                    MessageBox.Show($"Request exception: {e.Message}");

                }

                catch (JsonException e)

                {

                    MessageBox.Show($"JSON parsing exception: {e.Message}");

                }

            }

        }

        public class CheckData
        {
            public string message { get; set; }

        }

        private async void check_Hwid()
        {
            string hostName = Dns.GetHostName();
            string my_hwid = $"Flexer-auth-{hostName}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"https://ro-exec.live/check_key.php?license={my_hwid}");

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        try
                        {
                            CheckData data = JsonSerializer.Deserialize<CheckData>(content);

                            if (data != null)
                            {

                                if (data.message != null)
                                {
                                    if (data.message == "License not found in database.")
                                    {
                                        register_Acc();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Work !", "Flexer executor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Message data is missing or invalid.");
                                }
                            }
                            else
                            {
                                MessageBox.Show("No data found in the response.");
                            }
                        }
                        catch (JsonException e)
                        {
                            MessageBox.Show($"JSON parsing exception: {e.Message}\nContent: {content}");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Request failed with status code: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException e)
                {
                    MessageBox.Show($"Request exception: {e.Message}");
                }
            }
        }
        public class GetData
        {
            public string Key { get; set; }

        }

        public class LoadingData
        {
            public int Id { get; set; }
            public string License { get; set; }
            public string KeyPoint { get; set; }
            public string Hwid { get; set; }
            public string Ban { get; set; }
            public string Role { get; set; }
        }

        private async void register_Acc()
        {

            string hostName = Dns.GetHostName();

            string name = Dns.GetHostName();

            string my_hwid = $"Flexer-auth-{hostName}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"https://ro-exec.live/auth_01.php?username={name}&user_hwid={my_hwid}");

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        try
                        {
                            CheckData data = JsonSerializer.Deserialize<CheckData>(content);

                            if (data != null)
                            {
                                if (data.message != null)
                                {
                                    if (data.message == "Register key success !")
                                    {
                                        MessageBox.Show("Work !", "Flexer executor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    else if (data.message == "Key is Already registered")
                                    {
                                        using (HttpClient k_client = new HttpClient())
                                        {
                                            try
                                            {
                                                HttpResponseMessage k_response = await client.GetAsync($"https://ro-exec.live/get_Key.php?hwid={my_hwid}");

                                                if (k_response.IsSuccessStatusCode)
                                                {
                                                    string k_content = await k_response.Content.ReadAsStringAsync();

                                                    try
                                                    {
                                                        GetData k_data = JsonSerializer.Deserialize<GetData>(k_content);

                                                        if (k_data != null && k_data.Key != null)
                                                        {
                                                            string my_Key = k_data.Key;

                                                            using (HttpClient gg_client = new HttpClient())
                                                            {
                                                                try
                                                                {
                                                                    HttpResponseMessage gg_response = await gg_client.GetAsync($"https://ro-exec.live/main.php?license={my_Key}");

                                                                    if (gg_response.IsSuccessStatusCode)
                                                                    {
                                                                        string gg_content = await gg_response.Content.ReadAsStringAsync();

                                                                        try
                                                                        {
                                                                            LoadingData gg_data = JsonSerializer.Deserialize<LoadingData>(gg_content);

                                                                            if (gg_data != null && gg_data.KeyPoint != null)
                                                                            {

                                                                                string got_Ban = gg_data.Ban;

                                                                                string my_Point = gg_data.KeyPoint;

                                                                                string my_Role = gg_data.Role;

                                                                                if (got_Ban == "No")
                                                                                {

                                                                                    if (my_Role == "Free")
                                                                                    {

                                                                                        MessageBox.Show("Login with : Free License !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                                                                        this.Hide();

                                                                                        Form1 meoawji = new Form1();

                                                                                        meoawji.ShowDialog();

                                                                                    }
                                                                                    else
                                                                                    {

                                                                                        if (my_Role == "Dev")
                                                                                        {

                                                                                            MessageBox.Show("Login with : Developer License !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                                                                            this.Hide();

                                                                                            Form1 meoawji = new Form1();

                                                                                            meoawji.ShowDialog();

                                                                                        }
                                                                                        else
                                                                                        {

                                                                                            if (my_Role == "Private")
                                                                                            {

                                                                                                MessageBox.Show("Login with : Private License !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                                                                                this.Hide();

                                                                                                Form1 meoawji = new Form1();

                                                                                                meoawji.ShowDialog();

                                                                                            }

                                                                                        }

                                                                                    }

                                                                                }

                                                                                else
                                                                                {

                                                                                    if (got_Ban == "Yes")
                                                                                    {

                                                                                        MessageBox.Show("You got ban to use flexer !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                                                                        await Task.Delay(2000);

                                                                                        Environment.Exit(0);

                                                                                    }

                                                                                }

                                                                            }
                                                                            else
                                                                            {
                                                                                MessageBox.Show("Failed to retrieve KeyPoint data.");
                                                                            }
                                                                        }
                                                                        catch (JsonException)
                                                                        {
                                                                            MessageBox.Show("Failed to deserialize LoadingData.");
                                                                        }
                                                                    }
                                                                }
                                                                catch (HttpRequestException e)
                                                                {
                                                                    MessageBox.Show($"Request exception: {e.Message}");
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Failed to retrieve Key data.");
                                                        }
                                                    }
                                                    catch (JsonException)
                                                    {
                                                        MessageBox.Show("Failed to deserialize GetData.");
                                                    }
                                                }
                                            }
                                            catch (HttpRequestException e)
                                            {
                                                MessageBox.Show($"Request exception: {e.Message}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (JsonException)
                        {
                            MessageBox.Show("Failed to deserialize CheckData.");
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    MessageBox.Show($"Request exception: {e.Message}");
                }
            }

        }

        private void Welcome_Load(object sender, EventArgs e)
        {
            this.Hide();

            this.Opacity = 0;

            check_Server();

        }

    }
}
