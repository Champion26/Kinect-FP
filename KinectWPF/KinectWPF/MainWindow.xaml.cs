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
using System.Timers;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Animation;   

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
            btnAnalysis.Background = Brushes.Green;
            btnAnalysis.Foreground = Brushes.White;
            //dynamically create InformationTextBox and place accordingly, allowing for modes to be set
            //#FFCDD606
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            stream = new Streaming();
            stream.checkAndRunSensor(canvas,
                                     camera,
                                     this.txtInfo,
                                     this.btnAnalysis);
            handSet(Streaming.HandPreference.Right);

            populateLists();
            ChangeJointColour(Brushes.DeepSkyBlue);
            //trigger selection Change
            this.cbJointColour.SelectedItem = "DeepSkyBlue";
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
                        btnAnalysis.Background = Brushes.Red;
                       
                    });
                    stream.StartBodyTracking();
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        btnAnalysis.Content = "Start Analysis";
                        btnAnalysis.Background = Brushes.Green;
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
            var converter = new System.Windows.Media.BrushConverter();
            ShowHandPreferenceInfoMessage(String.Format("{0} hand selected as dominant.", hp.ToString()));
            Button btn = FindCorrespondingButton(hp);
            
             if (this.stream != null)
            {
                this.stream.hand = hp;
            }

             btn.Background = (Brush)converter.ConvertFromString("#FFCDD606");
            
             foreach (Button b in GetAllHandButtons())
             {
                 if (b != btn)
                 {
                     b.Background = Brushes.White;
                 }
             }
             RemoveControlInfoMessage();
        }

        private async void RemoveControlInfoMessage()
        {
                await Task.Delay(3000);
                stream.ControlInfoHandler.HideInfoMessage();
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

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ShowHandPreferenceInfoMessage(string txt)
        {
            stream.ControlInfoHandler.InfoBox = txtControlInfo;
            stream.ControlInfoHandler.SetInfoMessage(txt, this.stream.JointColour, border: false, fontSize: 20);
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowHandPreferenceInfoMessage("These buttons allow you to select your dominant hand. It is set to 'Right' by default.");
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            stream.ControlInfoHandler.HideInfoMessage();
        }

        private void imgJointColour_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowHandPreferenceInfoMessage("This drop down allows you to set the colour of the joints seen during analysis. It also sets the font colour for the control titles.");
        }

        private void imgAnalysis_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowHandPreferenceInfoMessage("This button will start and stop the form analysis.");
        }


    }
}
