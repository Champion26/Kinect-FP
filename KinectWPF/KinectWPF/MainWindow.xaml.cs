﻿using System;
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

        public List<Label> LabelTexts;

        public bool developerTools = false;

        public WindowDetails wd;
        
        public MainWindow()
        {
            InitializeComponent();
            btnAnalysis.Background = Brushes.Green;
            btnAnalysis.Foreground = Brushes.White;
         
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.SizeToContent = SizeToContent.WidthAndHeight;

            wd = new WindowDetails(0,0);

            if (!developerTools)
            {
                lblHeight.Visibility = System.Windows.Visibility.Hidden;
                lblWidth.Visibility = System.Windows.Visibility.Hidden;

            }

            stream = new Streaming();

            stream.CreateCameraDetailsReference(ref wd);

         
            stream.checkAndRunSensor(canvas,
                                     camera,
                                     this.txtInfo,
                                     this.btnAnalysis,
                                     this,
                                     wd);
            handSet(Streaming.HandPreference.Right);

            

            populateLists();
            ChangeJointColour(Brushes.DeepSkyBlue);
            //trigger selection Change
            this.cbJointColour.SelectedItem = "Deep Sky Blue";
        }

        private void populateLists()
        {
            populateLabelsList();
        }

        private void populateLabelsList()
        {
            this.LabelTexts = new List<Label>();
            this.LabelTexts.Add(lblHandPreference);
            this.LabelTexts.Add(lblJointColour);
        }

        private void btnAnalysis_Click(object sender, RoutedEventArgs e)
        {
            if (stream != null)
            {
                if (!stream.OffsetsGenerated)
                {
                    wd.originalHeight = camera.ActualHeight;
                    wd.originalWidth = camera.ActualWidth;
                    stream.GenerateOffsets();
                }
                

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
            foreach (Label gb in this.LabelTexts)
            {
                gb.Foreground = colour;
            }
        }

        private void cbJointColour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string s = e.AddedItems[0] as string;

            if (s != null)
            {
                SolidColorBrush scb = new SolidColorBrush((Color)ColorConverter.ConvertFromString(s.Replace(" ", "")));
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
                cbJointColour.Items.Add(AddSpacesToSentence(colorname, false));
            }
        }

        private string AddSpacesToSentence(string text, bool preserveAcronyms)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
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

        private void TriggerElementResize(double newHeight, double newWidth)
        {
            if (this.IsLoaded)
            {
                camera.Height = newHeight;
                camera.Width = newWidth;
                canvas.Height = newHeight;
                canvas.Width = newWidth;  
            }
                      
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e != null)
            {               
                TriggerElementResize(e.NewSize.Height, e.NewSize.Width);
             
                
            }

        }

  

        private void camera_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            this.controlCanvas.Width = camera.ActualWidth;
            this.controlCanvas.Height = camera.ActualHeight;
            txtInfo.Width = camera.ActualWidth / 3.11;
            txtInfo.FontSize = camera.ActualWidth / 40;

            if (txtInfo.FontSize / 2 >= 16)
            {
                this.lblHandPreference.FontSize = txtInfo.FontSize / 2;
                this.lblJointColour.FontSize = txtInfo.FontSize / 2;
            }

            if (wd != null)
            {
                lblWidth.Content = String.Format("Width: {0}", Math.Round(camera.ActualWidth, 2));
                lblHeight.Content = String.Format("Height: {0}", Math.Round(camera.ActualHeight, 2));

            }

        }

      

        private void controlCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {         
                    

            Canvas.SetBottom(this.DominantHandCanvas, this.controlCanvas.Height / 18);
            Canvas.SetLeft(this.DominantHandCanvas, 0);

            Canvas.SetBottom(this.AnalysisCanvas, this.controlCanvas.Height / 12.7);
            Canvas.SetRight(this.AnalysisCanvas, this.controlCanvas.Width / 32);

            Canvas.SetTop(this.ColourCanvas, this.controlCanvas.Height / 13.5);
            Canvas.SetRight(this.ColourCanvas, this.controlCanvas.Width / 32);


            Canvas.SetTop(this.txtInfo, this.controlCanvas.Height / 5.4);
            Canvas.SetLeft(this.txtInfo, this.controlCanvas.Width / 2.94);

            Canvas.SetTop(this.txtControlInfo, this.controlCanvas.Height / 1.8);
            Canvas.SetLeft(this.txtControlInfo, this.controlCanvas.Width / 2.94);

        }

        private void camera_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void camera_SourceUpdated(object sender, DataTransferEventArgs e)
        {
          
        }

        private void camera_TargetUpdated(object sender, DataTransferEventArgs e)
        {
           
        }


    }

    public class WindowDetails
    {
        public double originalHeight
        {
            get
            {
                return _originalHeight;
            }
            set
            {
                _originalHeight = value;
            }
        }

        private double _originalHeight;

        private double _originalWidth;

        public double originalWidth
        {
            get
            {
                return _originalWidth;
            }
            set
            {
                _originalWidth = value;
            }
        }

        public WindowDetails(double width, double height)
        {
            originalWidth = width;
            originalHeight = height;
        }
     
    }
}
