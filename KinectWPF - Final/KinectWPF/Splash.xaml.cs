using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Timers;

namespace KinectWPF
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        Streaming stream = new Streaming();
        Timer timer = new Timer(500);

        public Splash()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void CheckForSensor()
        {
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (stream.checkSensor())
            {
                Dispatcher.Invoke(() =>
                {
                    btnStart.IsEnabled = true;
                    lblWarning.Content = "Click start to continue.";
                });
                timer.Enabled = false;
            }
        }
       
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            App.Current.MainWindow = main;
            this.Close();
            main.Show();
        }

        private void btnStart_Loaded(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = false;
            System.Threading.Thread.Sleep(2000);
            CheckForSensor();
        }
    }
}
