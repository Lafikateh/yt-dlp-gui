// C# Standard library references
using System;
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
		private static bool Continue;
		private static Form Window;
		private static Label LinkLabel;
		private static TextBox Link;
		private static Label DirectoryLabel;
		private static TextBox Directory;
		private static Label AudioOnlyLabel;
		private static CheckBox AudioOnly;
		private static Button Download;
		
		// Window closed event function
		private static void Window_Closed(Object Sender, FormClosedEventArgs Arguments)
		{
			Continue = false;
		}
		
		// Process finished event function
		private static void Process_Finished(Object Sender, EventArgs Arguments)
		{
			// Enable the controls
			Link.Enabled = true;
			Directory.Enabled = true;
			AudioOnly.Enabled = true;
			Download.Enabled = true;
		}
		
		// Download button proessed event function
		private static void Download_Pressed(Object Sender, EventArgs Arguments)
		{
			// Disable the controls
			Link.Enabled = false;
			Directory.Enabled = false;
			AudioOnly.Enabled = false;
			Download.Enabled = false;
			
			// Prepare the parameters string
			string Parameters = "--no-playlist ";
			if(AudioOnly.Checked == true)
			{
				Parameters = Parameters + "--extract-audio " + Link.Text;
			}
			else
			{
				Parameters = Parameters + Link.Text;
			}
			
			// Add the output directory to the parameters string
			if(Directory.Text != "")
			{
				Parameters = Parameters + " --output " + Directory.Text + "\\%(title)s.%(ext)s";
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
		public static int Main()
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
			
			// Enable modern visual styles
			Application.EnableVisualStyles();
			
			// Create the window
			Window = new Form();
			Window.Width = 854;
			Window.Height = 480;
			Window.FormClosed += Window_Closed;
			Window.Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location);
			Window.Text = "GUI for yt-dlp.exe";
			
			// Create the link label
			LinkLabel = new Label();
			LinkLabel.Text = "Target URL:";
			
			// Create the link textbox
			Link = new TextBox();
			Link.Left = 32;
			Link.Top = 32;
			
			// Create the directory label
			DirectoryLabel = new Label();
			DirectoryLabel.Text = "Output directory:";
			
			// Create the directory textbox
			Directory = new TextBox();
			Directory.Left = 32;
			Directory.Top = 256;
			
			// Create the audio only label
			AudioOnlyLabel = new Label();
			AudioOnlyLabel.Text = "Just audio";
			
			// Create the audio only checkbox
			AudioOnly = new CheckBox();
			AudioOnly.Left = 256;
			AudioOnly.Top = 256;
			AudioOnly.Width = 32;
			AudioOnly.Height = 32;
			
			// Create the download button
			Download = new Button();
			Download.Left = 32;
			Download.Top = 96;
			Download.Click += Download_Pressed;
			Download.Text = "Download";
			
			// Add all of the elements to the window and show it
			Window.Controls.Add(LinkLabel);
			Window.Controls.Add(Link);
			Window.Controls.Add(DirectoryLabel);
			Window.Controls.Add(Directory);
			Window.Controls.Add(AudioOnlyLabel);
			Window.Controls.Add(AudioOnly);
			Window.Controls.Add(Download);
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