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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KinectWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Streaming stream;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            stream = new Streaming();
            stream.checkAndRunSensor(canvas,
                                     camera);
        }

        private void btnAnalysis_Click(object sender, RoutedEventArgs e)
        {
            if (stream != null)
            {
                if (!stream.bodyOn)
                {
                    stream.StartBodyTracking();
                }
                else
                {
                    stream.StopBodyTracking();
                }
            }
        }

        #region Hand Preference 
        private void btnRightHand_Click(object sender, RoutedEventArgs e)
        {
            handSet(Streaming.HandPreference.Right);        
        }

        private Button FindCorrespondingButton(Streaming.HandPreference hp)
        {
            if (hp == Streaming.HandPreference.Right)
            {
                return this.btnRightHand;
            }

            return this.btnLeftHand;
        }

        private List<Button> GetAllHandButtons()
        {
            List<Button> l = new List<Button>();

            l.Add(btnLeftHand);
            l.Add(btnRightHand);

            return l;
        }

        private void handSet(Streaming.HandPreference hp)
        {
            Button btn = FindCorrespondingButton(hp);
            
             if (this.stream != null)
            {
                this.stream.hand = hp;
            }

             btn.Background = Brushes.Green;
            
             foreach (Button b in GetAllHandButtons())
             {
                 if (b != btn)
                 {
                     b.Background = Brushes.White;
                 }
             }
          
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            handSet(Streaming.HandPreference.Left);
        }
        #endregion

    }
}
