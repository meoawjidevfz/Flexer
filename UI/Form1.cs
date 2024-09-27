using System.Drawing.Drawing2D;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.VisualBasic;
using static System.Windows.Forms.DataFormats;

namespace Flexer_Evo
{
    public partial class Form1 : Form
    {

        public class Client
        {
            public string Name { get; set; }
            public int Id { get; set; }

            public Client(string name, int id)
            {
                Name = name;
                Id = id;
            }
        }

        private System.Windows.Forms.Timer checkRobloxTimer;

        private HttpClient httpClient;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private int borderRadius = 9;
        private int borderSize = 0;
        private Color borderColor = Color.FromArgb(0, 0, 0);

        private int injected = 0;

        private int open_Menu = 0;

        private int open_Setup = 0;

        private int unlock_all = 0;

        private string nowVersion = "flexer-evo01";
        public Form1()
        {
            httpClient = new HttpClient();

            InitializeComponent();

            InitializeWebView();

            checkRobloxTimer = new System.Windows.Forms.Timer();
            checkRobloxTimer.Interval = 1000;
            checkRobloxTimer.Tick += CheckRobloxTimer_Tick;

        }
        public List<ClientInfo> ActiveClients { get; private set; } = new();
        public struct ClientInfo
        {
            public string version;
            public string name;
            public int id;
        }
        private async void InitializeWebView()
        {
            webView21.CoreWebView2InitializationCompleted += async (sender, e) =>
            {
                if (e.IsSuccess)
                {
                    string htmlFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Monaco\\index.html");
                    webView21.Source = new Uri(htmlFilePath);
                }
                else
                {
                    MessageBox.Show("WebView initialization failed.");

                    Application.Exit();
                }
            };

            await webView21.EnsureCoreWebView2Async(null);
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void FormRegionAndBorder(Form form, float radius, Graphics graph, Color borderColor, float borderSize)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                using (GraphicsPath roundPath = GetRoundedPath(form.ClientRectangle, radius))
                using (Pen penBorder = new Pen(borderColor, borderSize))
                using (Matrix transform = new Matrix())
                {
                    graph.SmoothingMode = SmoothingMode.AntiAlias;
                    form.Region = new Region(roundPath);
                    if (borderSize >= 1)
                    {
                        Rectangle rect = form.ClientRectangle;
                        float scaleX = 1.0F - ((borderSize + 1) / rect.Width);
                        float scaleY = 1.0F - ((borderSize + 1) / rect.Height);
                        transform.Scale(scaleX, scaleY);
                        transform.Translate(borderSize / 1.6F, borderSize / 1.6F);
                        graph.Transform = transform;
                        graph.DrawPath(penBorder, roundPath);
                    }
                }
            }
        }

        private async void check_Server()
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://ro-exec.live/pro.php");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                JArray jsonResponse = JArray.Parse(responseBody);

                if (jsonResponse.Count > 0)
                {
                    JObject firstItem = (JObject)jsonResponse[0];

                    string online = firstItem["x_online"].ToString();

                    string version = firstItem["dll_version"].ToString();

                    if (version == nowVersion)
                    {
                        if (online == "1")
                        {

                            unlock_all = 1;

                            this.Opacity = .90d;

                            string htmlFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Monaco\\index.html");

                            webView21.Source = new Uri(htmlFilePath);

                            Acount.Visible = false;

                            Book.Visible = false;

                            Screen.Visible = false;

                            panel7.Visible = false;

                            guna2CheckBox1.Visible = false;

                            guna2CheckBox2.Visible = false;

                            Setup.Visible = true;

                            Clear.Visible = true;

                            Open.Visible = true;

                            Save.Visible = true;

                        }
                        else if (online == "2")
                        {
                            MessageBox.Show("Server down !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            Environment.Exit(0);

                        }
                    }
                    else
                    {
                        this.Hide();
                        MessageBox.Show("New version out now !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Environment.Exit(0);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Request error: " + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            check_Server();

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            FormRegionAndBorder(this, borderRadius, e.Graphics, borderColor, borderSize);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            select_line.Location = new Point(9, 188);
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            select_line.Location = new Point(9, 108);
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            select_line.Location = new Point(9, 265);
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            select_line.Location = new Point(9, 340);

            // Multi Instance

            Process[] processes = Process.GetProcessesByName("RobloxPlayerBeta");

            if (processes.Length > 0)
            {

                select_line.Location = new Point(9, 108);

                label8.Text = "Enable success";

                label8.Location = new Point(680, 198);

                label8.ForeColor = Color.FromArgb(71, 242, 59);

            }
            else

            {
                MessageBox.Show("Roblox not found !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

        }

        private void deltect_RobloxS()
        {

        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            select_line.Location = new Point(9, 415);
        }

        private void webView21_Click(object sender, EventArgs e)
        {

        }

        private async Task SendMessageAsync(string message)
        {
            using (var httpClient = new HttpClient())
            {

                var formData = new MultipartFormDataContent();

                formData.Add(new StringContent(message), "message");

                try
                {
                    var response = await httpClient.PostAsync("http://localhost:8573/ro-exec", formData);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        MessageBox.Show("Error: " + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception: " + ex.Message);
                }
            }
        }

        private static string ReplaceSpecialCharactersWithUnicode(string input)
        {
            string pattern = "[^\\u0000-\\u007F]";
            return Regex.Replace(input, pattern, m => "\\u{" + GetUnicodeEscape((int)m.Value[0]) + "}");
        }

        private static string GetUnicodeEscape(int codePoint)
        {
            return string.Format("{0:x4}", codePoint);
        }


        private async void guna2Button7_Click(object sender, EventArgs e)
        {
            if (unlock_all == 0)
            {
                MessageBox.Show("Please wait while client loaded !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (unlock_all == 1)
                {
                    if (injected == 0)
                    {
                        MessageBox.Show("Flexer Evo", "Please inject !", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }
                    else
                    {

                        try
                        {

                            using (var httpClient = new HttpClient())
                            {

                                var formData = new MultipartFormDataContent();

                                formData.Add(new StringContent("Connected"), "message");

                                try
                                {
                                    var response = await httpClient.PostAsync("http://localhost:8573/ro-exec", formData);

                                    if (response.IsSuccessStatusCode)
                                    {

                                        if (injected == 1)
                                        {
                                            string script = ReplaceSpecialCharactersWithUnicode(JsonConvert.DeserializeObject<string>(await webView21.ExecuteScriptAsync("GetText()")));

                                            await SendMessageAsync(script);
                                        }

                                    }
                                    else
                                    {

                                    }
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("Server not found !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }

                            }

                        }
                        catch
                        {

                            MessageBox.Show("Excution error !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }

                    }
                }
            }

        }

        private void Menu_Click(object sender, EventArgs e)
        {
            if (unlock_all == 0)
            {
                MessageBox.Show("Please wait while client loaded !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (unlock_all == 1)
                {
                    if (open_Menu == 0)
                    {

                        open_Menu = 1;

                        Clear.Visible = false;

                        Open.Visible = false;

                        Save.Visible = false;

                        Acount.Visible = true;

                        Book.Visible = true;

                        Screen.Visible = true;

                        Menu.FillColor = Color.FromArgb(21, 184, 81);

                    }
                    else
                    {
                        if (open_Menu == 1)
                        {

                            open_Menu = 0;

                            Acount.Visible = false;

                            Book.Visible = false;

                            Screen.Visible = false;

                            Clear.Visible = true;

                            Open.Visible = true;

                            Save.Visible = true;

                            Menu.FillColor = Color.Transparent;

                        }
                    }
                }
            }

        }

        private async void Clear_Click(object sender, EventArgs e)
        {
            if (unlock_all == 0)
            {
                MessageBox.Show("Please wait while client loaded !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (unlock_all == 1)
                {
                    await webView21.ExecuteScriptAsync($"editor.setValue('--cleaned!')");
                    await Task.Delay(500);
                    await webView21.ExecuteScriptAsync($"editor.setValue('')");
                }
            }

        }

        private string EscapeJsString(string content)
        {
            return content.Replace("'", "\\'").Replace("\r", "\\r").Replace("\n", "\\n");
        }

        private async Task LoadFileIntoEditorAsync(string fileName)
        {
            try
            {
                string fileContent = await Task.Run(() => File.ReadAllText(fileName));
                await webView21.ExecuteScriptAsync($"editor.setValue('--load success!')");
                await Task.Delay(500);
                await webView21.CoreWebView2.ExecuteScriptAsync($"editor.setValue('{EscapeJsString(fileContent)}')");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void Open_Click(object sender, EventArgs e)
        {
            if (unlock_all == 0)
            {
                MessageBox.Show("Please wait while client loaded !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (unlock_all == 1)
                {
                    OpenFileDialog ofd = new OpenFileDialog
                    {
                        Filter = "Txt Files (*.txt)|*.txt|Lua Files (*.lua)|*.lua|All Files (*.*)|*.*"
                    };

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        await LoadFileIntoEditorAsync(ofd.FileName);
                        MessageBox.Show("Open file success !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }

        }

        private async void Save_Click(object sender, EventArgs e)
        {
            if (unlock_all == 0)
            {
                MessageBox.Show("Please wait while client loaded !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (unlock_all == 1)
                {
                    var editorContent = await webView21.ExecuteScriptAsync("editor.getValue();");

                    if (string.IsNullOrWhiteSpace(editorContent))
                    {
                        MessageBox.Show("Save file failed! No content to save.", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        SaveFileDialog sfd = new SaveFileDialog
                        {
                            Filter = "Txt Files (*.txt)|*.txt|Lua Files (*.lua)|*.lua|All Files (*.*)|*.*"
                        };

                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            await File.WriteAllTextAsync(sfd.FileName, editorContent.Trim('"'));

                            MessageBox.Show("Save file success!", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }

        }


        private async void label3_Click(object sender, EventArgs e)
        {
            if (unlock_all == 0)
            {
                MessageBox.Show("Please wait while client loaded !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (unlock_all == 1)
                {
                    await webView21.ExecuteScriptAsync($"editor.setValue('')");

                    label2.ForeColor = Color.Gray;
                    label3.ForeColor = Color.FromArgb(128, 128, 255);
                    label4.ForeColor = Color.Gray;
                    label5.ForeColor = Color.Gray;
                    label6.ForeColor = Color.Gray;

                    string x_link = "https://raw.githubusercontent.com/unified-naming-convention/NamingStandard/main/UNCCheckEnv.lua";
                    string u_script = $"loadstring(game:HttpGet(\"{x_link}\"))()";

                    string jsScript = $"editor.setValue(\"{u_script.Replace("\"", "\\\"")}\");";

                    await webView21.ExecuteScriptAsync(jsScript);
                }
            }

        }

        private async void label2_Click(object sender, EventArgs e)
        {

            if (unlock_all == 0)
            {
                MessageBox.Show("Please wait while client loaded !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (unlock_all == 1)
                {
                    await webView21.ExecuteScriptAsync($"editor.setValue('')");

                    label2.ForeColor = Color.FromArgb(128, 128, 255);
                    label3.ForeColor = Color.Gray;
                    label4.ForeColor = Color.Gray;
                    label5.ForeColor = Color.Gray;
                    label6.ForeColor = Color.Gray;
                }
            }

        }

        private async void label4_Click(object sender, EventArgs e)
        {
            if (unlock_all == 0)
            {
                MessageBox.Show("Please wait while client loaded !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (unlock_all == 1)
                {
                    await webView21.ExecuteScriptAsync($"editor.setValue('')");

                    label4.ForeColor = Color.FromArgb(128, 128, 255);
                    label2.ForeColor = Color.Gray;
                    label3.ForeColor = Color.Gray;
                    label5.ForeColor = Color.Gray;
                    label6.ForeColor = Color.Gray;

                    string x_link = "https://raw.githubusercontent.com/ic3w0lf22/Unnamed-ESP/master/UnnamedESP.lua";
                    string u_script = $"loadstring(game:HttpGet(\"{x_link}\"))()";

                    string jsScript = $"editor.setValue(\"{u_script.Replace("\"", "\\\"")}\");";

                    await webView21.ExecuteScriptAsync(jsScript);
                }
            }

        }

        private async void label5_Click(object sender, EventArgs e)
        {
            if (unlock_all == 0)
            {
                MessageBox.Show("Please wait while client loaded !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (unlock_all == 1)
                {
                    await webView21.ExecuteScriptAsync($"editor.setValue('')");

                    label5.ForeColor = Color.FromArgb(128, 128, 255);
                    label2.ForeColor = Color.Gray;
                    label3.ForeColor = Color.Gray;
                    label4.ForeColor = Color.Gray;
                    label6.ForeColor = Color.Gray;

                    string x_link = "https://raw.githubusercontent.com/flexerdevz/FlexerZ/refs/heads/main/Aimbot.lua";
                    string u_script = $"loadstring(game:HttpGet(\"{x_link}\"))()";

                    string jsScript = $"editor.setValue(\"{u_script.Replace("\"", "\\\"")}\");";

                    await webView21.ExecuteScriptAsync(jsScript);
                }
            }

        }

        private async void label6_Click(object sender, EventArgs e)
        {
            if (unlock_all == 0)
            {
                MessageBox.Show("Please wait while client loaded !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (unlock_all == 1)
                {
                    await webView21.ExecuteScriptAsync($"editor.setValue('')");

                    label6.ForeColor = Color.FromArgb(128, 128, 255);
                    label2.ForeColor = Color.Gray;
                    label3.ForeColor = Color.Gray;
                    label4.ForeColor = Color.Gray;
                    label5.ForeColor = Color.Gray;

                    string x_link = "https://raw.githubusercontent.com/EdgeIY/infiniteyield/master/source";
                    string u_script = $"loadstring(game:HttpGet(\"{x_link}\"))()";

                    string jsScript = $"editor.setValue(\"{u_script.Replace("\"", "\\\"")}\");";

                    await webView21.ExecuteScriptAsync(jsScript);
                }
            }

        }

        private void Setup_Click(object sender, EventArgs e)
        {
            if (unlock_all == 0)
            {
                MessageBox.Show("Please wait for checking update !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (unlock_all == 1)
                {
                    if (open_Setup == 0)
                    {

                        open_Setup = 1;

                        panel7.Visible = true;

                        guna2CheckBox1.Visible = true;

                        guna2CheckBox2.Visible = true;

                    }
                    else
                    {
                        if (open_Setup == 1)
                        {

                            open_Setup = 0;

                            panel7.Visible = false;

                            guna2CheckBox1.Visible = false;

                            guna2CheckBox2.Visible = false;

                        }
                    }
                }
            }

        }

        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox1.Checked)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
        }

        private void guna2CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox2.Checked)
            {
                checkRobloxTimer.Start();
            }
            else
            {
                checkRobloxTimer.Stop();
            }
        }

        private List<Client> clients = new List<Client>
        {
            new Client("Alice", 101),
            new Client("Bob", 102),
            new Client("Charlie", 103),
            new Client("Alice", 101),
            new Client("Bob", 102),
            new Client("Charlie", 103),
            new Client("Alice", 101),
            new Client("Bob", 102),
            new Client("Charlie", 103),
            new Client("Alice", 101),
            new Client("Bob", 102),
            new Client("Charlie", 103),
            new Client("Alice", 101),
            new Client("Bob", 102),
            new Client("Charlie", 103),
            new Client("Alice", 101),
            new Client("Bob", 102),
            new Client("Charlie", 103),
        };

        private void CheckRobloxTimer_Tick(object sender, EventArgs e)
        {

            Process[] processes = Process.GetProcessesByName("RobloxPlayerBeta");

            if (processes.Length > 0)
            {

                int startX = 10;
                int startY = 10;
                int spacingY = 30;

                int start_Select = 0;

                foreach (var client in clients)
                {

                    if (start_Select == 0)
                    {
                        start_Select = 1;

                        CheckBox checkBox = new CheckBox
                        {
                            Text = $"{client.Name}, PID: {client.Id}",
                            ForeColor = Color.White,
                            Font = new Font("Franklin Gothic Medium", 10),
                            Checked = true,
                            AutoSize = true,
                            Location = new Point(startX, startY)
                        };

                        panel8.Controls.Add(checkBox);

                        startY += spacingY;

                    }
                    else
                    {

                        if (start_Select == 1)
                        {

                            CheckBox checkBox = new CheckBox
                            {
                                Text = $"{client.Name}, PID: {client.Id}",
                                ForeColor = Color.White,
                                Font = new Font("Franklin Gothic Medium", 10),
                                Checked = false,
                                AutoSize = true,
                                Location = new Point(startX, startY)
                            };

                            panel8.Controls.Add(checkBox);

                            startY += spacingY;

                        }
                    }
                   
                }

                checkRobloxTimer.Stop();
                Not_Ready.Visible = false;
                Ready.Visible = true;

            }
            else
            {
                Not_Ready.Visible = true;
                Ready.Visible = false;
            }
        }

        private async void guna2Button6_Click(object sender, EventArgs e)
        {
            if (unlock_all == 0)
            {
                MessageBox.Show("Please wait while client loaded !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (unlock_all == 1)
                {

                    Process[] processes = Process.GetProcessesByName("RobloxPlayerBeta");

                    if (processes.Length > 0)
                    {

                        try
                        {

                            string exeLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MeoawInjector.exe");

                            Process process = new Process();
                            process.StartInfo.FileName = exeLocation;
                            process.StartInfo.Arguments = "";
                            process.StartInfo.UseShellExecute = true;

                            process.Start();

                            injected = 1;
                            Not_Ready.Visible = false;
                            Ready.Visible = true;

                        }
                        catch
                        {

                            // Start auto get injector ...



                            MessageBox.Show("MeoawInjector Not found !", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            await Task.Delay(1000);

                            Environment.Exit(1);

                        }

                    }
                    else
                    {
                        MessageBox.Show("Please open roblox", "Flexer Executor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }
            }

        }
    }
}

// All code by MeoawJi X Chat gpt | Last Update 27 / 09 / 2024 14:00