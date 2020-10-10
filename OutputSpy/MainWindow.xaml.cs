using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OutputSpy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Process process;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            ProcessName = @"C:\WINDOWS\system32\cmd.exe";
            InputFileName = @"input.txt";
        }

        public string ProcessName
        {
            get { return (string)GetValue(ProcessNameProperty); }
            set { SetValue(ProcessNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProcessName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProcessNameProperty =
            DependencyProperty.Register("ProcessName", typeof(string), typeof(MainWindow), new PropertyMetadata(""));



        public string InputFileName
        {
            get { return (string)GetValue(InputFileNameProperty); }
            set { SetValue(InputFileNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputFileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputFileNameProperty =
            DependencyProperty.Register("InputFileName", typeof(string), typeof(MainWindow), new PropertyMetadata(""));



        void RegisterProcesses(string processName)
        {
            ProcessStartInfo info = new ProcessStartInfo(processName);
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardInput = true;
            info.UseShellExecute = false;

            process = new Process();
            process.StartInfo = info;
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.Exited += Process_Exited;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();  
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            var process = sender as Process;
            process.OutputDataReceived -= Process_OutputDataReceived;
            process.ErrorDataReceived -= Process_ErrorDataReceived;
            process.Exited -= Process_Exited;
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            AddOutput(e.Data, true);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            AddOutput(e.Data, false);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RegisterProcesses(ProcessName);
        }

        private void AddOutput(string text, bool isError)
        {
            uxFlowDocument.Dispatcher.Invoke(() =>
            {
                Run run = new Run(text, uxFlowDocument.ContentEnd);
                run.Foreground = isError ? Brushes.Red : Brushes.Black;
                uxFlowDocument.ContentEnd.InsertLineBreak();

                uxScroll.ScrollToEnd();
            });
        }

        private void Input_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(InputFileName))
            {
                var lines = File.ReadLines(InputFileName);

                foreach (var line in lines)
                {
                    process.StandardInput.WriteLine(line);
                }

                process.StandardInput.Close();

                process.Close();
            }
        }
    }


}
