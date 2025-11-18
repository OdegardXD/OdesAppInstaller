namespace OdesAppInstaller
{
    public partial class Form1 : Form
    {
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
                    InstallLogText.Text += "Fetching App List..."; // enviroment.newline is not needed here as this is the first message in the log window
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
    }
}
