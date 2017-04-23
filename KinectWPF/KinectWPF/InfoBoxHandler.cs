using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Drawing;
using System.Timers;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Animation;  

namespace KinectWPF
{
    public class InfoBoxHandler
    {
        private bool _inUse;

        public bool inUse
        {
            get
            {
                return _inUse;
            }
            set
            {
                _inUse = value;
            }
        }

        private List<InfoBoxMessage> _previousMessages;

        public List<InfoBoxMessage> previousMessages
        {
            get
            {
                return _previousMessages;
            }
            set
            {
                _previousMessages = value;
            }
        }

        private TextBox _InfoBox;

        public TextBox InfoBox
        {
            get
            {
                return _InfoBox;
            }
            set
            {
                _InfoBox = value;
            }
        }

        private InfoBoxMessage _current;

        public InfoBoxMessage current
        {
            get
            {
                return _current;
            }
            set
            {
                _current = value;
            }
        }

        public InfoBoxHandler(TextBox box = null)
        {
            previousMessages = new List<InfoBoxMessage>();
            if (box != null)
            {
                this.InfoBox = box;
            }
        }

        public async void Revert()
        {
            if (previousMessages.Count > 0)
            {
                await Task.Delay(1000);
                InfoBoxMessage replacement = previousMessages[previousMessages.Count - 1];
                SetInfoMessage(replacement.msg, replacement.colour, revert:replacement.revert);
                previousMessages.Remove(replacement);
            }
        }
        //make sure revert on replacement is set after the msg is readded using revert
        //if revert status is on current
        public void SetInfoMessage(string message,
                                   Brush colour,
                                   string title = null,
                                   bool border = true,
                                   int fontSize = 0,
                                   bool revert = false)
        {
            if (InfoBox != null)
            {

                if (inUse && current.revert && !previousMessages.Contains(current))
                {
                    previousMessages.Add(new InfoBoxMessage(txt: InfoBox.Text, brush: InfoBox.Foreground));
                }

                if (!inUse)
                {
                    inUse = true;
                }

                InfoBox.Text = "";

                Thickness defaultThickness = new Thickness(1, 1, 1, 1);

                InfoBox.FontSize = 48;

                if (title != null)
                {
                    InfoBox.AppendText(title);
                    InfoBox.AppendText(Environment.NewLine);
                }
                InfoBox.AppendText(message);
                InfoBox.BorderBrush = colour;
                InfoBox.Foreground = colour;

                if (!border)
                {
                    InfoBox.BorderThickness = new Thickness(0, 0, 0, 0);
                }

                if (fontSize > 0)
                {
                    InfoBox.FontSize = fontSize;
                }

                DoubleAnimation animation = new DoubleAnimation(1, TimeSpan.FromSeconds(1));
                InfoBox.BeginAnimation(TextBox.OpacityProperty, animation);
                ShowInfoMessage(InfoBox);

                current = new InfoBoxMessage(txt: InfoBox.Text, brush: InfoBox.Foreground, revertAfterReplaced:revert);

                if (revert)
                {
                    previousMessages.Add(new InfoBoxMessage(txt: InfoBox.Text, brush: InfoBox.Foreground, revertAfterReplaced:revert));
                }
            }
        }

        public void HideInfoMessage()
        {
            if (InfoBox != null)
            {
                if (previousMessages.Contains(current))
                {
                    previousMessages.Remove(current);
                }

                if (previousMessages.Count == 0)
                {
                    inUse = false;
                    DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(1));
                    InfoBox.BeginAnimation(TextBox.OpacityProperty, animation);
                }
                else
                {
                    Revert();
                }
                
                //txtBox.Visibility = Visibility.Hidden;
            }
        }

        public void ShowInfoMessage(TextBox txtBox)
        {
            if (txtBox != null)
            {
                txtBox.Visibility = Visibility.Visible;
            }
        }

    }

    public class InfoBoxMessage : XMLIssueMessage
    {

        private bool _revert;

        public bool revert
        {
            get
            {
                return _revert;
            }
            set
            {
                _revert = value;
            }
        }

        public InfoBoxMessage(bool issue = false,
                            string txt = null,
                           Brush brush = null,
                           bool prevent = false,
                           bool revertAfterReplaced = false) : base(issue, txt, brush, prevent)
        {

            this.revert = revertAfterReplaced;
            
        }

    }
}
