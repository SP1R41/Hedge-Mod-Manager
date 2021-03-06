using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HedgeModManager
{
    public partial class UpdateForm : Form
    {

        public Thread Thread;
        public string Url;
        public string FileName;

        public UpdateForm(string url)
        {
            InitializeComponent();
            Url = url;
        }

        public UpdateForm(string url, string fileName)
        {
            InitializeComponent();
            Url = url;
            FileName = fileName;
        }

        private void UpdateForm_Load(object sender, EventArgs e)
        {
            var webClient = new WebClient();

            Thread = new Thread(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(FileName))
                    {
                        // Path to where all the update file are stored
                        string tempPath = Path.Combine(Program.StartDirectory, "updateTemp");
                        // Deletes the temp diretory
                        if (Directory.Exists(tempPath))
                            Directory.Delete(tempPath, true);
                        // Creates the temp directory
                        Directory.CreateDirectory(tempPath);
                        // Adds an event to the "DownloadProgressChanged" EventHandler
                        webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WebClient_DownloadProgressChanged);
                        // Starts downloading the update zip asynchronously
                        Invoke(new Action(() => webClient.DownloadFileAsync(new Uri(Url), Path.Combine(tempPath, "HedgeModManagerUpdate.zip"))));
                        // Waits for the update to finish
                        while (webClient.IsBusy)
                            Thread.Sleep(25);

                        LogFile.AddMessage("Finished downloading update. Installing.");
                        LogFile.AddMessage("Extracting Update...");

                        // Extracts the update files
                        ZipFile.ExtractToDirectory(Path.Combine(tempPath, "HedgeModManagerUpdate.zip"), tempPath);
                        // Deletes the zip file, since we nolonger need it
                        File.Delete(Path.Combine(tempPath, "HedgeModManagerUpdate.zip"));

                        LogFile.AddMessage("Finished extracting update.");

                        // Writes the batch file so we can continue the update process
                        File.WriteAllBytes(Path.Combine(Program.StartDirectory, "update.bat"), Properties.Resources.update);

                        // Runs the batch file and closes the Modloader
                        LogFile.AddMessage($"Closing {Program.ProgramName} to continue the update...");
                        new Process() { StartInfo = new ProcessStartInfo(Path.Combine(Program.StartDirectory, "update.bat")) }.Start();
                        Application.Exit();
                    }
                    else
                    {
                        webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                        Invoke(new Action(() => { webClient.DownloadFileAsync(new Uri(Url), FileName); }));
                        while (webClient.IsBusy)
                        {
                            Thread.Sleep(25);
                        }
                        Invoke(new Action(() => { Close(); }));
                    }
                }
                catch (Exception ex)
                {
                    MainForm.AddMessage("Exception thrown while downloading an update.", ex, Url, FileName);
                }
            });
            Thread.Start();
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Maximum = (int)e.TotalBytesToReceive;
            progressBar.Value = (int)e.BytesReceived;
        }
    }
}