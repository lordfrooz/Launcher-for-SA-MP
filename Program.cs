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
                    DialogResult result = MessageBox.Show("Yeni bir güncelleme mevcut. Yönlendirilmek istiyor musunuz?", "Güncelleme", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        string url = "https://libertycityrpg.com/cdn/Liberty_City_Roleplay.exe";

                        try
                        {
                            System.Diagnostics.Process.Start("explorer.exe", url);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("URL açma hatasý: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        Application.Exit(); // Uygulamayý kapat
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
                    MessageBox.Show("Güncelleme kontrolü sýrasýnda bir hata oluþtu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }
    }
}
