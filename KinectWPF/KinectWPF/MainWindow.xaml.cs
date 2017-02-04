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
using System.Reflection;

namespace KinectWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Streaming stream;

        public List<Brush> Colours;

        public List<GroupBox> GroupBoxes;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            stream = new Streaming();
            stream.checkAndRunSensor(canvas,
                                     camera);
            handSet(Streaming.HandPreference.Right);
            populateLists();
            ChangeJointColour(Brushes.DeepSkyBlue);
        }

        private void populateLists()
        {
            populateGroupBoxList();
        }

        private void populateGroupBoxList()
        {
            this.GroupBoxes = new List<GroupBox>();
            this.GroupBoxes.Add(grpBoxHandPref);
            this.GroupBoxes.Add(grpBoxJointButtons);
        }

        private void btnAnalysis_Click(object sender, RoutedEventArgs e)
        {
            if (stream != null)
            {
                if (!stream.bodyOn)
                {
                    Dispatcher.Invoke(() =>
                    {
                        btnAnalysis.Content = "Stop";
                    });
                    stream.StartBodyTracking();
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        btnAnalysis.Content = "Start Analysis";
                    });
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

        public void ChangeJointColour(Brush colour)
        {
            this.stream.JointColour = colour;
        }

        public void ChangeGroupBoxColours(Brush colour)
        {
            foreach (GroupBox gb in this.GroupBoxes)
            {
                gb.Foreground = colour;
            }
        }

        private void cbJointColour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string s = e.AddedItems[0] as string;

            if (s != null)
            {
                SolidColorBrush scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString(s));
                ChangeJointColour(scb);
                ChangeGroupBoxColours(scb);
            }
        }

        private void PopulateColours()
        {
            Colours = new List<Brush>();

            Type brushesType = typeof(System.Windows.Media.Brushes);

            var properties = brushesType.GetProperties(BindingFlags.Static | BindingFlags.Public);

            foreach (var prop in properties)
            {
                string name = prop.Name;
                SolidColorBrush brush = (SolidColorBrush)prop.GetValue(null, null);
                var colorname = GetColorName(brush.Color);
                cbJointColour.Items.Add(colorname);
            }
        }

        static string GetColorName(Color col)
        {
            PropertyInfo colorProperty = typeof(Colors).GetProperties()
                .FirstOrDefault(p => Color.AreClose((Color)p.GetValue(null), col));
            return colorProperty != null ? colorProperty.Name : "unnamed color";
        }

        private void cbJointColour_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateColours();
        }


              

    }
}
