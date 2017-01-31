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
using System.Xml;

namespace KinectWPF
{
    class Bone
    {

        private JointType _JointA;
        private JointType _JointB;

        public JointType JointA
        {
            get
            {
                return _JointA;
            }
            set
            {
                _JointA = value;
            }
        }

        public JointType JointB
        {
            get
            {
                return _JointB;
            }
            set
            {
                _JointB = value;
            }
        }

        public Bone(JointType jA, JointType jB)
        {
            this.JointA = jA;
            this.JointB = jB;
        }
    }
}
