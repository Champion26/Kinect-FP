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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace KinectWPF
{
    class ActionMessage
    {

        private Brush _colour;
        private string _error = null;

        public Brush Colour
        {
            get
            {
                return _colour;
            }
            set
            {
                _colour = value;
            }
        }

        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
            }
        }

        public string GenerateInfoMessage()
        {
            StringBuilder sb = new StringBuilder();
            
            return sb.ToString();
        }
    }
}
