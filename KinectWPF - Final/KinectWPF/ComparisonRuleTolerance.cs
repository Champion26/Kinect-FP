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
    class ComparisonRuleTolerance : ComparisonRule
    {

        public ComparisonRuleTolerance ConvertFromBaseComparisonRule(ComparisonRule cr)
        {
            ComparisonRuleTolerance crt = new ComparisonRuleTolerance();
            crt.JointA = cr.JointA;
            crt.JointB = cr.JointB;
            crt.CompType = cr.CompType;
            crt.Tolerances = cr.Tolerances;
            return crt;
        }

      public override Brush Compare(Joint JointA,
                                    Joint JointB,
                                    Streaming stream)
      {
        //get height difference of joints (will form part of triangle)
        double opposite = GetCoordinateDifference(JointA.Position.Y, JointB.Position.Y);
        //set hypotenuse
        double adjacent = GetCoordinateDifference(JointA.Position.X, JointB.Position.X);
        double hypo = GetHypotenuse(opposite, adjacent);

        Brush br = Brushes.Green;
        double targetAngle = GetAngleFromOpposite(opposite, hypo);

        if (targetAngle + GetAngleFromOpposite(adjacent, hypo) == 90)
        {
            //make sure angle values add up

            //check shoulder angle

            br = CompareValueAgainstTolerances(targetAngle);

        }

        return br;
      }
    }


}
