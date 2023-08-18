using Microsoft.Win32;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LCRP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        public const int EM_SETCUEBANNER = 0x1501;

        //string cache_url = "https://libertycityrpg.com/cdn/cache.rar";
        string samp_url = "https://libertycityrpg.com/cdn/sa-mp-0.3.DL-R1-install.exe";
        string gta_url = "https://libertycityrpg.com/cdn/gta-san-andreas.rar";
        string modloader_url = "https://libertycityrpg.com/cdn/modloader.rar";

        bool samp_instalado = false;
        bool cache_encontrado = false;
        bool download_ativo = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            //progressBar1.Hide();
            progressBar2.Hide();
            progressBar3.Hide();
            progressBar4.Hide();
            progressBar5.Hide();
            // label1.Visible = false;
            label7.Visible = false;
            label9.Visible = false;
            label2.Visible = false;
            // button7.Visible = false;
            button3.Visible = false;
            button1.Visible = false;
            button4.Visible = false;
            button5.Visible = false;

            //label3.Visible = false;
            label8.Visible = false;
            label4.Visible = false;
            label10.Visible = false;
            string gta_sa_path = Registry.CurrentUser.OpenSubKey(@"Software\SAMP")?.GetValue("gta_sa_exe")?.ToString();
            if (string.IsNullOrEmpty(gta_sa_path))
            {
                MessageBox.Show("Gta_sa.exe dosyası bulunamadı. Lütfen önce GTA San Andreas'ı kurun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            string modloader_folder_path = Path.GetDirectoryName(gta_sa_path) + "\\modloader";
            if (Directory.Exists(modloader_folder_path))
            {
                label10.Text = "Evet";
                label10.ForeColor = Color.Green;
                //label10.Hide();
                //label9.Hide();
                //button4.Hide();
            }
            else
            {
                label10.Text = "Hayır";
                label10.ForeColor = Color.Red;
                //label9.Show();
                //button4.Show();
            }

            string samp_path = Path.GetDirectoryName(gta_sa_path) + "\\samp.exe";
            if (File.Exists(samp_path))
            {
                label4.Text = "Evet";
                label4.ForeColor = Color.Green;
                // label4.Hide();
                // label2.Hide();
                // button1.Hide();
            }
            else
            {
                label4.Text = "Hayır";
                label4.ForeColor = Color.Red;
                // label2.Show();
                // button1.Show();
            }

            if (File.Exists(gta_sa_path))
            {
                label8.Text = "Evet";
                label8.ForeColor = Color.Green;
                // label8.Hide();
                // label7.Hide();
                // button3.Hide();
            }
            else
            {
                label8.Text = "Hayır";
                label8.ForeColor = Color.Red;
                /*label8.Show();
                label7.Show();
                button3.Show();*/
            }
            if (Directory.Exists(modloader_folder_path))
            {
                label10.Text = "Evet";
                label10.ForeColor = Color.Green;
            }
            else
            {
                label10.Text = "Hayır";
                label10.ForeColor = Color.Red;
            }

            string myRegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\\SAMP").GetValue("gta_sa_exe").ToString();
            myRegistryKey = myRegistryKey.Substring(0, myRegistryKey.LastIndexOf(@"\") + 1);

            if (!File.Exists(myRegistryKey + "gta_sa.exe"))
            {
                label8.Text = "Hayır";
                label8.ForeColor = Color.Red;
            }
            else if (!File.Exists(myRegistryKey + "samp.exe"))
            {
                samp_instalado = false;
                label4.Text = "Hayır";
                label4.ForeColor = Color.Red;

                // GTA
                label8.Text = "Evet";
                label8.ForeColor = Color.Green;
            }
            else
            {
                samp_instalado = true;
                label4.Text = "Evet";
                label4.ForeColor = Color.Green;

                // GTA
                label8.Text = "Evet";
                label8.ForeColor = Color.Green;
            }

            string cache_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GTA San Andreas User Files\\SAMP\\cache";
            //string lsrp_cache_path = cache_path + "\\" + server_ip + "." + server_port;
            /* if (Directory.Exists(cache_path))
             {
                 if (Directory.Exists(lsrp_cache_path))
                 {
                     if (!IsDirectoryEmpty(lsrp_cache_path))
                     {
                         cache_encontrado = true;
                         label3.Text = "Evet";
                         label3.ForeColor = Color.Green;
                     }
                     else
                     {
                         cache_encontrado = false;
                         label3.Text = "Evet";
                         label3.ForeColor = Color.Red;
                     }
                 }
             }
             else
             {
                 cache_encontrado = false;
                 label3.Text = "Evet";
                 label3.ForeColor = Color.Red;
             }*/
        }

        public bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(base.Handle, 0xa1, 2, "0");
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            if (download_ativo)
            {
                DialogResult result = MessageBox.Show("İndirme işlemi devam ediyor. İndirmeyi iptal edip, çıkmak istiyor musunuz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    Close();
                    return;
                }
            }
            else
            {
                Close();
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        /* private void button2_Click(object sender, EventArgs e)
         {
             if (!samp_instalado)
             {
                 MessageBox.Show("SA-MP istemcisi sunucuda oynamak için kurulu değil, lütfen önce indirin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Hand);
             }
             /* else if (!cache_encontrado)
              {
                  MessageBox.Show("Sunucu Cache bulunamadı, önce indirin", "Hata.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
              }
             else if (textBox1.Text.Length > 24 || textBox1.Text.Length < 3)
             {
                 MessageBox.Show("Adınız 3 ila 24 karakter uzunluğunda olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Hand);
             }
             else
             {
                 string sa_path = Registry.CurrentUser.OpenSubKey(@"Software\SAMP")?.GetValue("gta_sa_exe")?.ToString();

                 if (string.IsNullOrEmpty(sa_path))
                 {
                     MessageBox.Show("SA-MP istemcisi bulunamadı, lütfen önce kurun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                     return;
                 }

                 string searchFileName = "samp.exe";


                 string[] foundFiles = Directory.GetFiles(Path.GetDirectoryName(sa_path), searchFileName, SearchOption.AllDirectories);

                 if (foundFiles.Length == 0)
                 {
                     MessageBox.Show("samp.exe dosyası bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                     return;
                 }


                 string sampExePath = foundFiles[0];
                 //DialogResult dialogResult = MessageBox.Show(sampExePath);
                 Registry.CurrentUser.OpenSubKey(@"Software\SAMP", true).SetValue("PlayerName", textBox1.Text);


                 if (IsProcessOpen("gta_sa.exe"))
                 {
                     MessageBox.Show("GTA San Andreas zaten açık.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                     return;
                 }

                 ProcessStartInfo start_info = new ProcessStartInfo();
                 start_info.FileName = sampExePath;
                 start_info.Arguments = server_ip + ":" + server_port + " " + server_password;
                 Process.Start(start_info);

                 label11.Text = "Oyuna giriş yapılıyor... (3 saniye)";
                 label11.Visible = true;
                 Timer timer = new Timer();
                 int remainingTime = 3;
                 timer.Interval = 1000;
                 timer.Tick += (s, ev) =>
                 {
                     remainingTime--;
                     label11.Text = $"Oyuna giriş yapılıyor... ({remainingTime} saniye)";
                     if (remainingTime == 0)
                     {
                         label11.Visible = false;
                         timer.Stop();
                     }
                 };
                 timer.Start();
             }
         }

        private bool IsProcessOpen(string name)
        {
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName.Contains(name))
                    return true;
            }
            return false;
        }*/


        private async void button7_Click(object sender, EventArgs e)
        {
            /* progressBar1.Show();

             if (download_ativo)
             {
                 MessageBox.Show("Zaten bir şey indiriyorsunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                 return;
             }

             try
             {
                 using (WebClient web_client = new WebClient())
                 {
                     string cache_url = "https://libertycityrpg.com/cdn/cache.rar";
                     string targetFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GTA San Andreas User Files", "SAMP", "51.210.222.58.7777");

                     web_client.DownloadProgressChanged += Cache_DownloadProgressChanged;
                     web_client.DownloadFileCompleted += Cache_DownloadFileCompleted;

                     // İndirme işlemini başlat
                     download_ativo = true;
                     await web_client.DownloadFileTaskAsync(new Uri(cache_url), Path.Combine(targetFolderPath, "cache.rar"));
                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.Message, "Cache sunucularına ulaşılamıyor.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 download_ativo = false;
                 progressBar1.Hide();
             }*/
        }


        private void button1_Click(object sender, EventArgs e)
        {
            File.Delete("samp_client.exe");
            if (samp_instalado)
            {
                MessageBox.Show("SA-MP istemcisi zaten yüklü.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            progressBar2.Show();

            if (download_ativo == true)
            {
                MessageBox.Show("Zaten bir şey indiriyorsunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

            else
            {
                try
                {
                    using (WebClient web_client = new WebClient())
                    {
                        web_client.DownloadProgressChanged += SAMP_DownloadProgressChanged;
                        web_client.DownloadFileCompleted += new AsyncCompletedEventHandler(SAMP_DownloadFileCompleted);
                        web_client.DownloadFileAsync(new System.Uri(samp_url), "samp_client.exe");
                        download_ativo = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "SA-MP istemcisi indirilemiyor.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void SAMP_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                double bytes_baixados = double.Parse(e.BytesReceived.ToString());
                double bytes_totais = double.Parse(e.TotalBytesToReceive.ToString());

                double porcentagem = bytes_baixados / bytes_totais * 100;
                progressBar2.Value = int.Parse(Math.Truncate(porcentagem).ToString());
            });
        }

        private void SAMP_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                progressBar2.Hide();

                if (e.Error == null)
                {
                    MessageBox.Show("İstemci indirme işlemi başarıyla tamamlandı. Kurulumdan sonra, başlatıcıyı tekrar açın.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    try
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = "samp_client.exe";
                        process.StartInfo.UseShellExecute = true; // Bu satırı ekleyin
                        process.Start();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("İstemci açılırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("İstemci indirilirken bir hata oluştu: " + e.Error.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                download_ativo = false;
            });
        }

        /* void Cache_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
         {
             this.BeginInvoke((MethodInvoker)delegate
             {
                 double bytes_baixados = double.Parse(e.BytesReceived.ToString());
                 double bytes_totais = double.Parse(e.TotalBytesToReceive.ToString());

                 double porcentagem = bytes_baixados / bytes_totais * 100;
                 progressBar1.Value = int.Parse(Math.Truncate(porcentagem).ToString());
             });
         }

         void Cache_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
         {
             this.BeginInvoke((MethodInvoker)delegate
             {
                 string cache_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GTA San Andreas User Files\\SAMP\\cache";
                 System.IO.DirectoryInfo di = new DirectoryInfo(cache_path);

                 foreach (FileInfo file in di.GetFiles())
                 {
                     file.Delete();
                 }
                 foreach (DirectoryInfo dir in di.GetDirectories())
                 {
                     dir.Delete(true);
                 }

                 string cache_rar_path = Path.Combine(cache_path, "cache.rar");

                 // Rar dosyasını çıkart

                 progressBar1.Hide();
                 cache_encontrado = true;
                 label3.Text = "Evet";
                 label3.ForeColor = Color.Green;
                 MessageBox.Show("Cachen indirme ve çıkarma tamamlandı.", "Başarılı!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                 download_ativo = false;
             });
         }*/



        void GTA_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                double bytes_baixados = double.Parse(e.BytesReceived.ToString());
                double bytes_totais = double.Parse(e.TotalBytesToReceive.ToString());
                double porcentagem = bytes_baixados / bytes_totais * 100;
                progressBar3.Value = int.Parse(Math.Truncate(porcentagem).ToString());
            });
        }

        void GTA_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                string sa_path;

                if (Environment.Is64BitOperatingSystem == true)
                {
                    sa_path = Path.GetPathRoot(Environment.SystemDirectory) + "\\Program Files (x86)\\Rockstar Games\\GTA San Andreas";
                }
                else
                {
                    sa_path = Path.GetPathRoot(Environment.SystemDirectory) + "\\Program Files\\Rockstar Games\\GTA San Andreas";
                }

                bool existe = System.IO.Directory.Exists(sa_path);

                if (!existe)
                    System.IO.Directory.CreateDirectory(sa_path);

                System.IO.DirectoryInfo di = new DirectoryInfo(sa_path);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }

                ZipFile.ExtractToDirectory("GTA.zip", sa_path);

                progressBar3.Hide();
                label8.Text = "Evet";
                label8.ForeColor = Color.Green;
                MessageBox.Show("Gta indirme işlemi tamamlandı.", "Başarılı!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                download_ativo = false;
            });
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (download_ativo == true)
            {
                MessageBox.Show("Zaten bir şey indiriyorsunuz.", "Hata.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

            else
            {
                try
                {
                    progressBar3.Show();
                    using (WebClient web_client = new WebClient())
                    {
                        web_client.DownloadProgressChanged += GTA_DownloadProgressChanged;
                        web_client.DownloadFileCompleted += new AsyncCompletedEventHandler(GTA_DownloadFileCompleted);
                        web_client.DownloadFileAsync(new System.Uri(gta_url), "GTA.zip");
                        download_ativo = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Sunucu hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (download_ativo == true)
            {
                MessageBox.Show("Zaten bir şey indiriyorsunuz.", "Hata.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            string modloaderFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "modloader");
            string modloaderAsiFile = Path.Combine(modloaderFolder, "ModLoader.asi");

            if (File.Exists(modloaderFolder) && File.Exists(modloaderAsiFile))
            {
                MessageBox.Show("ModLoader zaten yüklü.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                string gta_sa_path = Registry.CurrentUser.OpenSubKey(@"Software\SAMP")?.GetValue("gta_sa_exe")?.ToString();
                if (string.IsNullOrEmpty(gta_sa_path))
                {
                    MessageBox.Show("Gta_sa.exe dosyası bulunamadı. Lütfen önce GTA San Andreas'ı kurun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }

                string modloader_folder_path = Path.GetDirectoryName(gta_sa_path);
                string modloader_rar_path = Path.Combine(modloader_folder_path, "modloader.rar");
                using (WebClient web_client = new WebClient())
                {
                    web_client.DownloadFileCompleted += new AsyncCompletedEventHandler(ModLoader_DownloadFileCompleted);
                    web_client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ModLoader_DownloadProgressChanged);

                    web_client.DownloadFileAsync(new Uri(modloader_url), modloader_rar_path);
                    download_ativo = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sunucu hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void ModLoader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                double bytes_baixados = double.Parse(e.BytesReceived.ToString());
                double bytes_totais = double.Parse(e.TotalBytesToReceive.ToString());

                double porcentagem = bytes_baixados / bytes_totais * 100;
                progressBar4.Value = int.Parse(Math.Truncate(porcentagem).ToString());
            });
        }

        void ModLoader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                string gta_sa_path = Registry.CurrentUser.OpenSubKey(@"Software\SAMP")?.GetValue("gta_sa_exe")?.ToString();
                if (string.IsNullOrEmpty(gta_sa_path))
                {
                    MessageBox.Show("Gta_sa.exe dosyası bulunamadı. Lütfen önce GTA San Andreas'ı kurun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }

                string modloader_folder_path = Path.GetDirectoryName(gta_sa_path);
                string modloader_rar_path = Path.Combine(modloader_folder_path, "modloader.rar");

                try
                {
                    using (Stream stream = File.OpenRead(modloader_rar_path))
                    using (var archive = RarArchive.Open(stream))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            if (!entry.IsDirectory)
                            {
                                entry.WriteToDirectory(modloader_folder_path, new ExtractionOptions()
                                {
                                    ExtractFullPath = true,
                                    Overwrite = true
                                });
                            }
                        }
                    }

                    File.Delete(modloader_rar_path);

                    label10.Text = "Evet";
                    label10.ForeColor = Color.Green;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ModLoader.rar dosyası çıkartılırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                progressBar4.Hide();
                MessageBox.Show("ModLoader.rar dosyası başarıyla indirildi ve çıkartıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                download_ativo = false;
            });
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string gta_sa_path = Registry.CurrentUser.OpenSubKey(@"Software\SAMP")?.GetValue("gta_sa_exe")?.ToString();

            string modloader_folder_path = Path.Combine(Path.GetDirectoryName(gta_sa_path), "modloader");
            string liberty_rar_path = Path.Combine(modloader_folder_path, "liberty.rar");
            string liberty_folder_path = Path.Combine(modloader_folder_path, "Liberty");
            if (Directory.Exists(liberty_folder_path))
            {
                MessageBox.Show("Liberty dosyaları zaten yüklü.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (WebClient web_client = new WebClient())
                {
                    web_client.DownloadFileCompleted += new AsyncCompletedEventHandler(Liberty_DownloadFileCompleted);
                    web_client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Liberty_DownloadProgressChanged);

                    web_client.DownloadFileAsync(new Uri("https://libertycityrpg.com/cdn/liberty.rar"), liberty_rar_path);
                    download_ativo = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sunucu hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void Liberty_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                try
                {
                    string gta_sa_path = Registry.CurrentUser.OpenSubKey(@"Software\SAMP")?.GetValue("gta_sa_exe")?.ToString();
                    string modloader_folder_path = Path.Combine(Path.GetDirectoryName(gta_sa_path), "modloader");
                    string liberty_rar_path = Path.Combine(modloader_folder_path, "liberty.rar");
                    string liberty_folder_path = Path.Combine(modloader_folder_path, "Liberty");
                    if (!Directory.Exists(liberty_folder_path))
                        Directory.CreateDirectory(liberty_folder_path);

                    using (var archive = ArchiveFactory.Open(liberty_rar_path))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            if (!entry.IsDirectory)
                            {
                                string entryPath = Path.Combine(liberty_folder_path, entry.Key);
                                entry.WriteToFile(entryPath, new ExtractionOptions()
                                {
                                    ExtractFullPath = true,
                                    Overwrite = true
                                });
                            }
                        }
                    }

                    MessageBox.Show("Liberty dosyaları başarıyla yüklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    label10.Text = "Evet";
                    label10.ForeColor = Color.Green;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                progressBar4.Hide();
                download_ativo = false;
            });
        }
        void Liberty_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                double bytes_baixados = double.Parse(e.BytesReceived.ToString());
                double bytes_totais = double.Parse(e.TotalBytesToReceive.ToString());

                double porcentagem = bytes_baixados / bytes_totais * 100;
                progressBar5.Value = int.Parse(Math.Truncate(porcentagem).ToString());
            });
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private bool IsGTAProcessRunning()
        {
            Process[] processes = Process.GetProcessesByName("gta_sa");
            return processes.Length > 0;
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            label14.Hide();
            label15.Hide();


            label12.Text = "Patcher sekmesine geçildi";


            //label1.Visible = true;
            pictureBox1.Visible = false;
            label7.Visible = true;
            label9.Visible = true;
            label2.Visible = true;
            // button7.Visible = true;
            button3.Visible = true;
            button1.Visible = true;
            button4.Visible = true;
            button5.Visible = true;

            //label3.Visible = true;
            label8.Visible = true;
            label4.Visible = true;
            label10.Visible = true;
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            if (download_ativo)
            {
                MessageBox.Show("İndirme işlemi devam ediyor, lütfen bekleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // label1.Visible = false;
            pictureBox1.Visible = true;
            label7.Visible = false;
            label9.Visible = false;
            label2.Visible = false;
            // button7.Visible = false;
            button3.Visible = false;
            button1.Visible = false;
            button4.Visible = false;
            button5.Visible = false;

            //label3.Visible = false;
            label8.Visible = false;
            label4.Visible = false;
            label10.Visible = false;

            label14.Visible = true;
            label15.Visible = true;
            label12.Text = "Anasayfaya geçildi.";
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            string discordURL = "https://discord.gg/lcrpg";
            string oldText = label12.Text;
            label12.Text = "Discord'a yönlendiriliyor...";

            Timer timer = new Timer();
            timer.Interval = 5000;
            timer.Tick += (s, ev) =>
            {
                label12.Text = oldText;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();


            try
            {
                System.Diagnostics.Process.Start("explorer.exe", discordURL);
            }
            catch (Exception ex)
            {
                MessageBox.Show("URL açma hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            string oldText = label12.Text;
            label12.Text = "YouTube'a yönlendiriliyor...";

            Timer timer = new Timer();
            timer.Interval = 5000;
            timer.Tick += (s, ev) =>
            {
                label12.Text = oldText;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();

            string url = "https://www.youtube.com/@LibertyCityRoleplayGaming";

            try
            {
                System.Diagnostics.Process.Start("explorer.exe", url);
            }
            catch (Exception ex)
            {
                MessageBox.Show("URL açma hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
