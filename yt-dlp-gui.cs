// C# Standard library references
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

// Assembly information
[assembly: AssemblyTitle("YT-DLP GUI")]
[assembly: AssemblyDescription("YT-DLP GUI")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyCompany("Lafika")]
[assembly: AssemblyProduct("YT-DLP GUI")]
[assembly: AssemblyCopyright("Copyright (C) 2022 Lafika <lafikateh@gmail.com>")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("1.0.0.0")] 
[assembly: AssemblyFileVersion("1.0.0.0")]

// Developer namespace
namespace Lafika
{
	// Program class
	public class YT_DLP_GUI
	{
		// Internal data
		private static Configuration Config;
		private static bool Continue;
		private static Form Window;
		private static Font Regular;
		private static Font Small;
		private static Label LinkLabel;
		private static TextBox Link;
		private static Label DirectoryLabel;
		private static TextBox Directory;
		private static Label FormatLabel;
		private static ComboBox Format;
		private static Label DownloadVideoLabel;
		private static CheckBox DownloadVideo;
		private static Button Download;
		private static ProgressBar Progress;
		
		// Window closed event function
		private static void Window_Closed(Object Sender, FormClosedEventArgs Arguments)
		{
			// Set the continue flag to false
			Continue = false;
			
			// Get a list of processes which have the "yt-dlp" title
			Process[] Process_List = Process.GetProcessesByName("yt-dlp");
			
			// Kill all processes in the list
			foreach(Process Handle in Process_List)
			{
				Handle.Kill();
			}
			
			// Save configuration data
			Config.AppSettings.Settings["Last_Directory"].Value = Directory.Text;
			Config.AppSettings.Settings["Last_Video"].Value = DownloadVideo.Checked.ToString();
			Config.AppSettings.Settings["Last_Format"].Value = Format.SelectedIndex.ToString();
			ConfigurationManager.RefreshSection(Config.AppSettings.SectionInformation.Name);
			Config.Save(ConfigurationSaveMode.Modified);
		}
		
		// Process finished event function
		private static void Process_Finished(Object Sender, EventArgs Arguments)
		{
			// Enable the controls
			Link.Enabled = true;
			Directory.Enabled = true;
			DownloadVideo.Enabled = true;
			Download.Enabled = true;
			
			// Stop the progress bar
			Progress.MarqueeAnimationSpeed = 0;
		}
		
		// Directory textbox pressed event function
		private static void Directory_Pressed(Object Sender, EventArgs Arguments)
		{
			// Prepare the save dialog
			SaveFileDialog Dialog = new SaveFileDialog();
			Dialog.FileName = "Output directory";
			
			// Show the dialog and get the directory path
			if(Dialog.ShowDialog() == DialogResult.OK)
			{
				Directory.Text = Path.GetDirectoryName(Dialog.FileName);
			}
		}
		
		// Download video checkbox pressed event function
		private static void Checkbox_Pressed(Object Sender, EventArgs Arguments)
		{
			if(DownloadVideo.Checked == true)
			{
				Format.Items.Clear();
				Format.ResetText();
				Format.Items.Add("webm");
				Format.Items.Add("mp4");
				Format.Items.Add("mkv");
				Format.Items.Add("mov");
				Format.Items.Add("avi");
				Format.SelectedIndex = 0;
			}
			else
			{
				Format.Items.Clear();
				Format.ResetText();
				Format.Items.Add("opus");
				Format.Items.Add("vorbis");
				Format.Items.Add("mp3");
				Format.Items.Add("wav");
				Format.Items.Add("flac");
				Format.SelectedIndex = 0;
			}
		}
		
		// Download button pressed event function
		private static void Download_Pressed(Object Sender, EventArgs Arguments)
		{
			// Disable the controls
			Link.Enabled = false;
			Directory.Enabled = false;
			DownloadVideo.Enabled = false;
			Download.Enabled = false;
			
			// Start the progress bar
			Progress.MarqueeAnimationSpeed = 1;
			
			// Prepare the parameters string
			string Parameters = "--no-playlist ";
			if(DownloadVideo.Checked == true)
			{
				Parameters = Parameters + "--recode-video " + Format.SelectedText + " " + Link.Text;
			}
			else
			{
				Parameters = Parameters + "--extract-audio --audio-format " + Format.SelectedText + " " + Link.Text;
			}
			
			// Add the output directory to the parameters string
			if(Directory.Text != "")
			{
				Parameters = Parameters + " --output " + "\"" + Directory.Text + "\\" + "%(title)s.%(ext)s\"";
			}
			else
			{
				Parameters = Parameters + " --output " + "\"" + System.IO.Directory.GetCurrentDirectory() + "\\" + "%(title)s.%(ext)s\"";
			}
			
			// Prepare the process start information
			Process DLP = new Process();
			DLP.StartInfo.UseShellExecute = false;
			DLP.StartInfo.FileName = "yt-dlp.exe";
			DLP.StartInfo.Arguments = Parameters;
			DLP.StartInfo.CreateNoWindow = true;
			DLP.EnableRaisingEvents = true;
			DLP.Exited += Process_Finished;
			
			// Start the process
			DLP.Start();
		}
		
		// Entry point of the program
		[STAThread] public static int Main()
		{
			// Check if the yt-dlp executable is present
			if(File.Exists("yt-dlp.exe") == false)
			{
				MessageBox.Show("Could not find yt-dlp.exe!");
				return 1;
			}
			
			// Check if the ffprobe executable is present
			if(File.Exists("ffprobe.exe") == false)
			{
				MessageBox.Show("Could not find ffprobe.exe!");
				return 1;
			}
			
			// Check if the ffmpeg executable is present
			if(File.Exists("ffmpeg.exe") == false)
			{
				MessageBox.Show("Could not find ffmpeg.exe!");
				return 1;
			}
			
			// Load program configuration
			Config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			
			// Read last directory value
			if(Config.AppSettings.Settings["Last_Directory"] == null)
			{
				Config.AppSettings.Settings.Add("Last_Directory", "");
			}
			
			// Read last checkbox value
			if(Config.AppSettings.Settings["Last_Video"] == null)
			{
				Config.AppSettings.Settings.Add("Last_Video", "True");
			}
			
			// Read last format value
			if(Config.AppSettings.Settings["Last_Format"] == null)
			{
				Config.AppSettings.Settings.Add("Last_Format", "0");
			}
			
			// Enable modern visual styles
			Application.EnableVisualStyles();
			
			// Create the window
			Window = new Form();
			Window.Width = 560;
			Window.Height = 288;
			Window.MaximizeBox = false;
			Window.FormBorderStyle = FormBorderStyle.FixedSingle;
			Window.FormClosed += Window_Closed;
			Window.Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);
			Window.Text = "GUI for yt-dlp.exe";
			
			// Create fonts
			Regular = new Font("Arial", 14.0f);
			Small = new Font("Arial", 12.0f);
			
			// Create the link label
			LinkLabel = new Label();
			LinkLabel.Top = 16;
			LinkLabel.Left = 16;
			LinkLabel.Width = 128;
			LinkLabel.Height = 24;
			LinkLabel.Font = Regular;
			LinkLabel.Text = "Target URL:";
			
			// Create the link textbox
			Link = new TextBox();
			Link.Left = 16;
			Link.Top = 40;
			Link.Width = 512;
			Link.Height = 24;
			Link.Font = Small;
			
			// Create the directory label
			DirectoryLabel = new Label();
			DirectoryLabel.Left = 16;
			DirectoryLabel.Top = 72;
			DirectoryLabel.Width = 256;
			DirectoryLabel.Height = 24;
			DirectoryLabel.Font = Regular;
			DirectoryLabel.Text = "Output directory:";
			
			// Create the directory textbox
			Directory = new TextBox();
			Directory.Left = 16;
			Directory.Top = 96;
			Directory.Width = 512;
			Directory.Height = 24;
			Directory.ReadOnly = true;
			Directory.Click += Directory_Pressed;
			Directory.Font = Small;
			Directory.Text = Config.AppSettings.Settings["Last_Directory"].Value;
			
			// Create the format label
			FormatLabel = new Label();
			FormatLabel.Left = 224;
			FormatLabel.Top = 128;
			FormatLabel.Width = 80;
			FormatLabel.Height = 24;
			FormatLabel.Font = Regular;
			FormatLabel.Text = "Format:";
			
			// Create the format box
			Format = new ComboBox();
			Format.Left = 308;
			Format.Top = 128;
			Format.Width = 64;
			Format.Height = 24;
			Format.Font = Small;
			
			// Display correct formats
			if(Config.AppSettings.Settings["Last_Video"].Value == "True")
			{
				Format.Items.Add("webm");
				Format.Items.Add("mp4");
				Format.Items.Add("mkv");
				Format.Items.Add("mov");
				Format.Items.Add("avi");
			}
			else
			{
				Format.Items.Add("opus");
				Format.Items.Add("vorbis");
				Format.Items.Add("mp3");
				Format.Items.Add("wav");
				Format.Items.Add("flac");
			}
			
			// Set the format index
			Format.SelectedIndex = Convert.ToInt32(Config.AppSettings.Settings["Last_Format"].Value);
			
			// Create the download video label
			DownloadVideoLabel = new Label();
			DownloadVideoLabel.Left = 16;
			DownloadVideoLabel.Top = 128;
			DownloadVideoLabel.Width = 160;
			DownloadVideoLabel.Height = 24;
			DownloadVideoLabel.Font = Regular;
			DownloadVideoLabel.Text = "Download video:";
			
			// Create the download video checkbox
			DownloadVideo = new CheckBox();
			DownloadVideo.Left = 180;
			DownloadVideo.Top = 124;
			DownloadVideo.Width = 32;
			DownloadVideo.Height = 32;
			DownloadVideo.Checked = Convert.ToBoolean(Config.AppSettings.Settings["Last_Video"].Value);
			DownloadVideo.Click += Checkbox_Pressed;
			
			// Create the download button
			Download = new Button();
			Download.Left = 16;
			Download.Top = 160;
			Download.Width = 512;
			Download.Height = 32;
			Download.Font = Regular;
			Download.Click += Download_Pressed;
			Download.Text = "Download";
			
			// Create the progress bar
			Progress = new ProgressBar();
			Progress.Left = 16;
			Progress.Top = 208;
			Progress.Width = 512;
			Progress.Height = 24;
			Progress.Style = ProgressBarStyle.Marquee;
			Progress.MarqueeAnimationSpeed = 0;
			
			// Add all of the elements to the window and show it
			Window.Controls.Add(LinkLabel);
			Window.Controls.Add(Link);
			Window.Controls.Add(DirectoryLabel);
			Window.Controls.Add(Directory);
			Window.Controls.Add(FormatLabel);
			Window.Controls.Add(Format);
			Window.Controls.Add(DownloadVideoLabel);
			Window.Controls.Add(DownloadVideo);
			Window.Controls.Add(Download);
			Window.Controls.Add(Progress);
			Window.Show();
			
			// Loop forever until window is closed
			Continue = true;
			while(Continue == true)
			{
				// Process events
				Application.DoEvents();
				
				// Sleep for 16 miliseconds
				Thread.Sleep(16);
			}
			
			// Report success
			return 0;
		}
	}
}
