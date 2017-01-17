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
using System.Xml;
using System.Xml.Linq;

namespace KinectWPF
{
    class ToleranceList
    {

        private List<Tolerance> _tolerances;
        private string _jointA;
        private string _jointB;

        public List<Tolerance> Tolerances
        {
            get
            {
                return _tolerances;
            }
        }

        public string JointA
        {
            get
            {
                return _jointA;
            }
        }

        public string JointB
        {
            get
            {
                return _jointB;
            }
        }

        public ToleranceList(string ja,
                             string jb)
        {
            _tolerances = new List<Tolerance>();
            _jointA = ja;
            _jointB = jb;
        }
               

    }

    
}
