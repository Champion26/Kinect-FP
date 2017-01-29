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
    class ComparisonRuleBehind : ComparisonRule
    {

        public ComparisonRuleBehind ConvertFromBaseComparisonRule(ComparisonRule cr)
        {
            ComparisonRuleBehind crt = new ComparisonRuleBehind();
            crt.JointA = cr.JointA;
            crt.JointB = cr.JointB;
            crt.CompType = cr.CompType;
            crt.Tolerances = cr.Tolerances;
            return crt;
        }

        public override void Compare(Joint JointA,
                                      Joint JointB,
                                      Streaming stream,
                                      ref ActionMessage am)
        {
            //get height difference of joints (will form part of triangle)

            if (JointA.Position.Z > JointB.Position.Z)
            {
                if (this.Tolerances.Count > 0)
                {

                }
                else
                {
                    am.Colour = Brushes.Red;
                    am.Error = String.Concat("Please move ", JointNameToReadableString(JointA), " in front of ", JointNameToReadableString(JointB));
                }
            }

        }
    }


}
