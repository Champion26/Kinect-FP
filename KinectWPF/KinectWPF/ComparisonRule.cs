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



namespace KinectWPF
{
    class ComparisonRule
    {
        private string _jointA;
        private string _jointB;
        private ComparisonType _comparisonType;

        public string JointA
        {
            get
            {
                return _jointA;
            }
            set
            {
                _jointA = value;
            }
        }

        public string JointB
        {
            get
            {
                return _jointB;
            }
            set
            {
                _jointB = value;
            }
        }

        public ComparisonType CompType
        {
            get
            {
                return _comparisonType;
            }
        }



        public ComparisonRule(string jA,
                              string jB,
                              ComparisonType ct)
        {
            if (jA != null)
            {
                this._jointA = jA;
            }
            if (jB != null)
            {
                this._jointB = jB;
            }
            if (ct != null)
            {
                this._comparisonType = ct;
            }

        }

        public enum ComparisonType
        {
            Over,
            Under
        }

    }


}
