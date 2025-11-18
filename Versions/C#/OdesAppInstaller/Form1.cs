using System.Diagnostics;

namespace OdesAppInstaller
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource cts = new CancellationTokenSource(); // idfk how this works but its required for being able to cancel the methods that are running when the user clicks "cancel"
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DownloadList(MainVariables.AppListLink); // call the funny method with the link to the applist.txt
        }

        private async Task DownloadList(string url) // download list but doing it in a way where it wont make the main program freeze up while downloading
        {
            using (HttpClient client = new HttpClient()) // http client to make requests
            {
                try // try to download file
                {
                    InstallLogText.Text += "-- Fetching App List... --"; // enviroment.newline is not needed here as this is the first message in the log window
                    string fileContent = await client.GetStringAsync(url); // "await" to make the code wait until file is downloaded
                    ParseContent(fileContent); // parse the contents of the file by calling the parsecontent method and giving it the contents of the file
                }
                catch (HttpRequestException ex) // catch error if error happened
                {
                    InstallLogText.Text += Environment.NewLine + $"Download Error: {ex.Message}";
                }
            }
        }

        public class ProgramInstallerItem // some variables
        {
            public bool Install { get; set; } = true;
            public string Name { get; set; }
            public string Link { get; set; }
            public string SilentSwitch { get; set; }
        }

        private void ParseContent(string fileContent) // grab the downloaded file contents from the downloadlist method and parse it
        {
            InstallLogText.Text += Environment.NewLine + "App List Downloaded.";
            string[] lines = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries); // splits the file into 2 parts. name and download link.
            InstallLogText.Text += Environment.NewLine + $"--- Parsing {lines.Length} lines ---"; // adds a little message at the top that says that its going through all the found programs
            ProgramListDataGridView.Rows.Clear(); // clears everything in the datagridview to make space for the new stuff
            foreach (string line in lines) // for each line do the stuff below. just works as a loop to go through all the lines and get the download link and program name
            {
                string[] parts = line.Split('|'); // turns the line into two parts. the name part and the download link part
                if (parts.Length == 2) // if the line has 2 parts then do the stuff below and else throw an error
                {
                    string programName = parts[0].Trim(); // makes a variable named programname and sets it as the first part of the line
                    string downloadLink = parts[1].Trim(); // makes a variable named downloadinfo and sets it as the second part of the line
                    ProgramListDataGridView.Rows.Add( // add the strings to the datagridview and set the checkbox as true by default
                        true,
                        programName,
                        downloadLink
                    );
                    InstallLogText.Text += Environment.NewLine + $"Found: {programName}"; // add the program to the log
                }
                else // error logging
                {
                    InstallLogText.Text += Environment.NewLine + $"[WARNING] Skipping invalid line: {line}";
                }
            }
        }

        private async void InstallButton_Click(object sender, EventArgs e)
        {
            cts = new CancellationTokenSource();
            InstallButton.Enabled = false;

            List<string> selectedLinks = new List<string>();

            foreach (DataGridViewRow row in ProgramListDataGridView.Rows)
            {
                if (!row.IsNewRow)
                {
                    object checkboxValue = row.Cells["Install"].Value;

                    if (checkboxValue != null && checkboxValue != DBNull.Value && (bool)checkboxValue)
                    {
                        string linkToInstall = row.Cells["Link"].Value.ToString();
                        selectedLinks.Add(linkToInstall);
                    }
                }
            }

            InstallLogText.Text += Environment.NewLine + $"Found {selectedLinks.Count} programs selected for install.";

            await ProcessInstallationsAsync(selectedLinks, cts.Token);

            InstallButton.Enabled = true;
            InstallLogText.Text += Environment.NewLine + "--- Installation Queue Finished ---";
        }

        private async Task ProcessInstallationsAsync(List<string> links, CancellationToken token)
        {
            using (var client = new HttpClient())
            {
                foreach (string fullInstallString in links)
                {
                    if (token.IsCancellationRequested)
                    {
                        InstallLogText.Text += Environment.NewLine + "Installation cancelled by user.";
                        return;
                    }

                    string[] parts = fullInstallString.Split(new[] { ' ' }, 2);
                    string downloadUrl = parts[0];
                    string silentArgs = parts.Length > 1 ? parts[1] : "";

                    string fileName = Path.GetFileName(new Uri(downloadUrl).LocalPath);
                    string tempFilePath = Path.Combine(Path.GetTempPath(), fileName);

                    try
                    {
                        InstallLogText.Text += Environment.NewLine + $"Downloading {fileName}...";

                        byte[] fileBytes = await client.GetByteArrayAsync(downloadUrl, token);
                        await File.WriteAllBytesAsync(tempFilePath, fileBytes, token);

                        InstallLogText.Text += Environment.NewLine + $"Downloaded successfully. Installing...";

                        await ExecuteInstallerAsync(tempFilePath, silentArgs, token);

                        InstallLogText.Text += Environment.NewLine + $"Installation of {fileName} completed.";
                    }
                    catch (OperationCanceledException)
                    {
                        InstallLogText.Text += Environment.NewLine + $"Installation of {fileName} was interrupted.";
                    }
                    catch (Exception ex)
                    {
                        InstallLogText.Text += Environment.NewLine + $"[ERROR] Failed to install {fileName}: {ex.Message}";
                    }
                    finally
                    {
                        if (File.Exists(tempFilePath))
                        {
                            File.Delete(tempFilePath);
                        }
                    }
                }
            }
        }

        private Task ExecuteInstallerAsync(string filePath, string arguments, CancellationToken token)
        {
            return Task.Run(() => // runs the downloaded installer for the program that is currently getting installed
            {
                using (Process installerProcess = new Process()) // "using" so that it can safely get rid of the current installer
                {
                    installerProcess.StartInfo.FileName = filePath;
                    installerProcess.StartInfo.Arguments = arguments;
                    installerProcess.StartInfo.UseShellExecute = false;
                    installerProcess.StartInfo.CreateNoWindow = true; // hides the installer window as this program is meant to quickly and silently install all the programs that the user wishes to install

                    installerProcess.Start(); // run the installer

                    while (!installerProcess.HasExited) // enter a loop that is finished when the installer is no longer running
                    {
                        token.ThrowIfCancellationRequested(); // trigger the loop if the user clicks cancel
                        Thread.Sleep(500); // wait half a second for the installer to finish 
                    }

                    if (installerProcess.ExitCode != 0) // if something went wrong during installation then throw error
                    {
                        throw new Exception($"Installer exited with code {installerProcess.ExitCode}.");
                    }
                }
            }, token); // passes the cts stuff to task.run so that the background installer is aware and should stop if called
        }

        private void CancelButton_Click(object sender, EventArgs e) // cancel button
        {
            cts.Cancel(); // cancels the installation
            InstallLogText.Text += Environment.NewLine + "Cancellation requested. Waiting for current task to finish...";
        }
    }
}
