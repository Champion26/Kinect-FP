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

        private void FindAndReplaceStr(ref string searchString,
                                      string criteriaString,
                                      string replaceString)
        {
            if (searchString.Contains(criteriaString))
              {
                searchString =  searchString.Replace(criteriaString, replaceString);
              }
        }

        private void ReplaceDominantAndPassiveStrings(ref string searchString,
                                                      string dominantStringReplace,
                                                      string passiveStringReplace)
        {
            FindAndReplaceStr(ref searchString, "Dominant", dominantStringReplace);
            FindAndReplaceStr(ref searchString, "Passive", passiveStringReplace);
        }

        public Brush RunComparison(ref Streaming.HandPreference hp)
        {
          Brush br = Brushes.Green;

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
              string a = cr.JointA;
              string b = cr.JointB;
              ReplaceDominantAndPassiveStrings(ref a, domHand, pasHand);
              ReplaceDominantAndPassiveStrings(ref b, domHand, pasHand);
              cr.JointA = a;
              cr.JointB = b;
            }

            ComparisonRule c = new ComparisonRule(null, null, ComparisonRule.ComparisonType.Over);

            foreach(ComparisonRule cr in cList)
            {
              if (cr.JointNameCheck(this.JointA.JointType.ToString()))
              {
                if (cr.JointNameCheck(this.JointB.JointType.ToString()))
                {
                    br = cr.CheckComparison(JointA, JointB, this.Stream);
                }
              }
            }

          }

          return br;

        }

        public enum HandType
        {
            Dominant,
            Passive
        }



    }
}
