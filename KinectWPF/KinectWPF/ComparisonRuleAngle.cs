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

        private Direction _direction;

        public Direction direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }

        public ComparisonRuleAngle ConvertFromBaseComparisonRule(ComparisonRule cr)
        {
            ComparisonRuleAngle crt = new ComparisonRuleAngle();
            crt.JointA = cr.JointA;
            crt.JointB = cr.JointB;
            crt.CompType = cr.CompType;
            crt.Tolerances = cr.Tolerances;
            crt.OriginNode = cr.OriginNode;
            crt.FillAdditionalFields();
            //Add addition fields 
            return crt;
        }

      public override void Compare(Joint JointA,
                                    Joint JointB,
                                    Streaming stream,
                                    ref ActionMessage am)
      {
        //get height difference of joints (will form part of triangle)
        double opposite = GetCoordinateDifference(JointA.Position.Y, JointB.Position.Y);
        //set hypotenuse
        double adjacent = GetCoordinateDifference(JointA.Position.X, JointB.Position.X);
        double hypo = GetHypotenuse(opposite, adjacent);

        double targetAngle = GetAngleFromOpposite(opposite, hypo);

          Tolerance tol = CompareValueAgainstTolerances(targetAngle, ref am);

       //JointDirectionCheck(JointA, JointB)
        // if above rule isn't working, autoInvalidate the tolerance and say raise/lower accordinly 
        if (tol != null)
        {
            HandleAngleTolerance(ref am, tol, targetAngle, JointA, JointB);
        }
        
      }

      public void HandleAngleTolerance(ref ActionMessage am, 
                                 Tolerance tol,
                                 double value,
                                 Joint JointA,
                                 Joint JointB)
      {


          if (tol.Optimal == true && !JointDirectionCheck(JointA, JointB))
              {
                  am.Error = null;
              }
              else
              {
                  Tolerance OptimalTolerance = GetOptimalTolerance();
                  if (OptimalTolerance != null)
                  {
                      if (this.direction == Direction.None)
                      {
                          string JointBName = this.StripPreferenceFromJointName(JointB.JointType.ToString()).ToLower();
                          if (JointDirectionCheck(JointA, JointB))
                          {
                              string dir = "";
                              switch (this.direction)
                              {
                                  case (Direction.A_Above_B):
                                      dir = " lower ";
                                      break;
                                  case (Direction.B_Above_A):
                                      dir = " raise ";
                                      break;
                              }
                              am.Error = String.Concat("Please", dir, JointBName, ".");
                              am.Colour = FindToleranceByType(this, Tolerance.ToleranceType.Invalid).colour;
                              return;
                          }

                          string direction = (this.direction == Direction.A_Above_B) ? "lower" : "raise";

                          double distance = 0.0;
                          if (value > OptimalTolerance.upperTolerance)
                          {

                              direction = (this.direction == Direction.A_Above_B) ? "raise" : "lower";
                              distance = value - OptimalTolerance.upperTolerance;
                          }
                          else if (value < OptimalTolerance.lowerTolerance)
                          {
                              distance = OptimalTolerance.lowerTolerance - value;
                          }

                          StringBuilder str = new StringBuilder(string.Concat("Please ", direction, " your ", JointBName));

                          str.Append(String.Concat(" by ", distance, "°"));

                          str.Append(".");
                          am.Error = str.ToString();
                      }
                      else
                      {
                          am.Colour = Brushes.Red;
                          am.Error = "There is an error with you comparison rule. Please check the XML file.";
                      }
                  }
                  else
                  {
                      am.Error = "Error";
                  }
              }
      }

      private bool JointDirectionCheck(Joint jA, Joint jB)
      {
          switch (this.direction)
          {
              case Direction.A_Above_B:
                  if (jB.Position.Y > jA.Position.Y)
                  {
                      return true;
                  }
                  break;
              case Direction.B_Above_A:
                  if (jA.Position.Y > jB.Position.Y)
                  {
                      return true;
                  }
                  break;
          }
          return false;
      }

      private void FillAdditionalFields()
      {
          GenerateDirection();
      }

      private void GenerateDirection()
      {
          if (this.OriginNode != null)
          {
              Generate gn = new Generate();
              try
              {
                  this.direction = (Direction)Enum.Parse(typeof(Direction), gn.FindAttribute(OriginNode.Attributes, "AngleDirection").Value.ToString());
              }
              catch
              {
                  this.direction = Direction.None;
              }
          }
      }

      public enum Direction
      {
          A_Above_B,
          B_Above_A,
          None
      }
    }


}
