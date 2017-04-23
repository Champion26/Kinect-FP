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



namespace KinectWPF
{
    class ComparisonRuleAboveBelow : ComparisonRule
    {

        private Func<double, double, bool> AGreaterThanB = (a, b) => a > b;
        private Func<double, double, bool> ALessThanB = (a, b) => a < b;
        private Func<double, double, bool> AEqualB = (a, b) => a == b;
      private string _operator;

      public string Operator
      {
        get
        {
            return _operator;
        }
          set
          {
              _operator = value;
          }
      }


      public ComparisonRuleAboveBelow(string jA,
                            string jB,
                            ComparisonType ct,
                            string op,
                            XmlNode xn) : base(jA, jB, ct, xn)
      {
          if (op != null)
          {
            this._operator = op;
          }

      }

      public ComparisonRuleAboveBelow() 
      {

      }

      public ComparisonRuleAboveBelow ConvertFromBaseComparisonRule(ComparisonRule cr)
      {
          ComparisonRuleAboveBelow crab = new ComparisonRuleAboveBelow();
          crab.JointA = cr.JointA;
          crab.JointB = cr.JointB;
          crab.CompType = cr.CompType;
          crab.Tolerances = cr.Tolerances;
          return crab;
      }

      public string DetermineStringOperator(ComparisonType ct)
      {
        switch (ct)
        {
            case (ComparisonType.Over):
              return ">";
            case (ComparisonType.Under):
              return "<";
            default:
              return  "==";
        }
      }

      public Func<double, double, bool> DetermineComparisonFunction()
      {
          Func<double, double, bool> rFunc;
        switch(this.Operator){
          case (">"):
            rFunc = AGreaterThanB;
            break;
          case ("<"):
            rFunc = ALessThanB;
            break;
          default:
            rFunc = AEqualB;
            break;
        }
        return rFunc;
      }

      public override void Compare(Joint JointA,
                                    Joint JointB,
                                    Streaming stream,
                                    ref ActionMessage am)
      {
        Func<double, double, bool> op = DetermineComparisonFunction();
     
            if (op(stream.GetXandYColourPoint(JointA).Y, stream.GetXandYColourPoint(JointB).Y))
            {
                am.Colour = Brushes.Red;
                am.Error = String.Concat(JointNameToReadableString(JointA), " needs to be above ", JointNameToReadableString(JointB), ".");
            }
        

      }
    }


}
