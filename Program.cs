using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace LCRP
{
    namespace MyApp
    {
        static class Program
        {
            private static Version currentVersion = new Version("1.2");

            [STAThread]
            static void Main()
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if (CheckForUpdates())
                {
                    DialogResult result = MessageBox.Show("Yeni bir g�ncelleme mevcut. Y�nlendirilmek istiyor musunuz?", "G�ncelleme", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        string url = "https://libertycityrpg.com/cdn/Liberty_City_Roleplay.exe";

                        try
                        {
                            System.Diagnostics.Process.Start("explorer.exe", url);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("URL a�ma hatas�: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        Application.Exit(); // Uygulamay� kapat
                        return;
                    }
                }
                Application.Run(new Form1());
            }

            static bool CheckForUpdates()
            {
                try
                {
                    string url = "https://libertycityrpg.com/cdn/version";
                    string serverVersion;

                    using (var client = new WebClient())
                    using (var stream = client.OpenRead(url))
                    using (var reader = new StreamReader(stream))
                    {
                        serverVersion = reader.ReadToEnd();
                    }

                    Version serverVersionObj = new Version(serverVersion);
                    int compareResult = currentVersion.CompareTo(serverVersionObj);

                    return (compareResult < 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("G�ncelleme kontrol� s�ras�nda bir hata olu�tu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }
    }
}
