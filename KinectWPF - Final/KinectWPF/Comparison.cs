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
using System.Xml;
using System.Xml.Linq;

namespace KinectWPF
{
    class Comparison
    {

        private Generate gn;
        private Joint _jointA;
        private Joint _jointB;
        private Streaming _stream;

        public Joint JointA
        {
          get
          {
              return _jointA;
          }
        }

        public Joint JointB
        {
          get
          {
              return _jointB;
          }
        }

        public Streaming Stream
        {
            get
            {
                return _stream;
            }
        }

        public Comparison(Generate g,
                          Joint ja,
                          Joint jb,
                          Streaming s)
        {
            this._stream = s;
            this.gn = g;
            this._jointA = ja;
            this._jointB = jb;
        }

        private Streaming.HandPreference DetermineOppositeHand(Streaming.HandPreference hp)
        {
          if (hp == Streaming.HandPreference.Right)
          {
              return Streaming.HandPreference.Left;
          }

          return Streaming.HandPreference.Right;
        }

        private string ReplaceDominantAndPassiveStrings(string searchString,
                                                      string dominantStringReplace,
                                                      string passiveStringReplace)
        {
            if (searchString.Contains("Dominant")){
                return searchString.Replace("Dominant", dominantStringReplace);
            }
            else if (searchString.Contains("Passive")){
                return searchString.Replace("Passive", passiveStringReplace);
            }
            return searchString;
        }

        public void RunComparison(ref Streaming.HandPreference hp,
                                  ref ActionMessage am)
        {

          string domHand = hp.ToString();
          string pasHand = (hp == Streaming.HandPreference.Right) ? Streaming.HandPreference.Left.ToString() : Streaming.HandPreference.Right.ToString();
          ComparisonRule emptyCr = new ComparisonRule(null, null, ComparisonRule.ComparisonType.Over);

          List<ComparisonRule> cList = emptyCr.CloneComparisonList(gn.Comparisons);
          //copy comparison list
          //change Dominant and Passive in types to real hands
          if (cList.Count > 0)
          {
            foreach(ComparisonRule cr in cList)
            {
              cr.JointA = ReplaceDominantAndPassiveStrings(cr.JointA, domHand, pasHand);
              cr.JointB = ReplaceDominantAndPassiveStrings(cr.JointB, domHand, pasHand);
            }

            foreach(ComparisonRule cr in cList)
            {
                if (cr.JointNameCheck(this.JointA.JointType.ToString()))
              {
                if (cr.JointNameCheck(this.JointB.JointType.ToString()))
                {
                    Joint jA = cr.DetermineComparisonRuleJointOrder(JointA, JointB, true);
                    Joint jB = cr.DetermineComparisonRuleJointOrder(JointA, JointB, false);

                    switch (cr.CompType)
                    {
                        case (ComparisonRule.ComparisonType.Over):
                        case (ComparisonRule.ComparisonType.Under):
                        case (ComparisonRule.ComparisonType.Equal):
                            ComparisonRuleAboveBelow crb = new ComparisonRuleAboveBelow();
                            crb = crb.ConvertFromBaseComparisonRule(cr);
                            if (crb.Operator == null)
                            {
                                crb.Operator = crb.DetermineStringOperator(cr.CompType);
                            }
                            crb.Compare(jA, jB, this.Stream, ref am);
                            break;
                        case (ComparisonRule.ComparisonType.Angle):
                            ComparisonRuleAngle crt = new ComparisonRuleAngle();
                            crt = crt.ConvertFromBaseComparisonRule(cr);
                            crt.Compare(jA, jB, this.Stream, ref am);
                            break;
                        case (ComparisonRule.ComparisonType.Behind):
                            ComparisonRuleBehind cb = new ComparisonRuleBehind();
                            cb = cb.ConvertFromBaseComparisonRule(cr);
                            cb.Compare(jA, jB, this.Stream, ref am);
                            break;
                    }
                }
              }
            }

          }
          

        }

     

        public enum HandType
        {
            Dominant,
            Passive
        }



    }
}
