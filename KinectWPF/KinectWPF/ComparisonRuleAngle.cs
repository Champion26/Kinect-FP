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
    class ComparisonRuleAngle : ComparisonRule
    {

        public ComparisonRuleAngle ConvertFromBaseComparisonRule(ComparisonRule cr)
        {
            ComparisonRuleAngle crt = new ComparisonRuleAngle();
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
        bool isInvalid = false;

        double opposite = GetCoordinateDifference(JointA.Position.Y, JointB.Position.Y);
        //set hypotenuse
        double adjacent = GetCoordinateDifference(JointA.Position.X, JointB.Position.X);
        double hypo = GetHypotenuse(opposite, adjacent);

        double targetAngle = GetAngleFromOpposite(opposite, hypo);

          Tolerance tol = CompareValueAgainstTolerances(targetAngle, ref am);

        if (JointA.Position.Y > JointB.Position.Y)
        {
            isInvalid = true;
            am.Colour = Brushes.Red;
            tol = FindToleranceByType(this, Tolerance.ToleranceType.Invalid);
        }
        
        if (tol != null)
        {
            HandleToleranceError(tol, ref am, targetAngle, "°", isInvalid);
        }
        
      }
    }


}
