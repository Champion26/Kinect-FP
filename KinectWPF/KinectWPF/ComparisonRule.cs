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
    class ComparisonRule
    {
        private string _jointA;
        private string _jointB;
        private ComparisonType _comparisonType;
        private XmlNode _OriginNode;

        private List<Tolerance> _tolerances;


        public List<Tolerance> Tolerances
        {
            get
            {
                return _tolerances;
            }
            set
            {
                _tolerances = value;
            }
        }

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
            set
            {
                _comparisonType = value;
            }
        }

        public XmlNode OriginNode
        {
            get
            {
                return _OriginNode;
            }
            set
            {
                _OriginNode = value;
            }

        }

        protected ComparisonRule(ComparisonRuleAboveBelow other)
        {
            this._jointA = other._jointA;
            this._jointB = other._jointB;
            this._comparisonType = other._comparisonType;
            this._tolerances = other._tolerances;

        }

        protected ComparisonRule(ComparisonRuleAngle other)
        {
            this._jointA = other._jointA;
            this._jointB = other._jointB;
            this._comparisonType = other._comparisonType;
            this._OriginNode = other._OriginNode;
        }

        public ComparisonRule(string jA,
                              string jB,
                              ComparisonType ct,
                              XmlNode xn = null)
        {
            _tolerances = new List<Tolerance>();
            if (jA != null)
            {
                this._jointA = jA;
            }
            if (jB != null)
            {
                this._jointB = jB;
            }
            this._comparisonType = ct;
            this.OriginNode = xn;
        }

        public ComparisonRule()
        {

        }

        public Joint DetermineComparisonRuleJointOrder(Joint jointA, Joint jointB, bool JointA)
        {
            string jointStr = (JointA) ? this.JointA : this.JointB;
            if (jointStr == jointA.JointType.ToString())
            {
                return jointA;
            }
            return jointB;
        }

        public bool JointNameCheck(string comparisonString)
        {
          if (comparisonString == this.JointA ||
              comparisonString == this.JointB)
          {
             return true;
          }
          return false;
        }

        public ComparisonRule Clone()
        {
          ComparisonRule rComp = new ComparisonRule(this.JointA, this.JointB, this.CompType, this.OriginNode);
          if (this.Tolerances.Count > 0)
          {
              foreach (Tolerance tol in this.Tolerances)
              {
                  Tolerance rtol = new Tolerance(tol.upperTolerance, tol.lowerTolerance, tol.colour, tol.Optimal, tol.toleranceType);
                  rComp.Tolerances.Add(rtol);
              }
          }
          return rComp;
        }

        public List<ComparisonRule> CloneComparisonList(List<ComparisonRule> list)
        {
          List<ComparisonRule> rList = new List<ComparisonRule>();
          if (list.Count > 0){
            foreach (ComparisonRule cmp in list)
            {
              rList.Add(cmp.Clone());
            }
          }
          return rList;
        }

        public Tolerance CompareValueAgainstTolerances(double angle,
                                                  ref ActionMessage am)
        {
            if (this.Tolerances.Count > 0)
            {
                foreach (Tolerance tol in this.Tolerances)
                {
                    if (angle > tol.lowerTolerance && angle < tol.upperTolerance)
                    {
                        am.Colour = tol.colour;
                        return tol;
                    }
                }
            }
            Tolerance InvalidTol = FindToleranceByType(this, Tolerance.ToleranceType.Invalid);
            am.Colour = InvalidTol.colour;
            return InvalidTol;
        }

        public static Tolerance FindToleranceByType(ComparisonRule cr, Tolerance.ToleranceType tt)
        {
            if (cr.Tolerances.Count > 0)
            {
                foreach (Tolerance tol in cr.Tolerances)
                {
                    if (tol.toleranceType == tt)
                    {
                        return tol;
                    }
                }
            }
            return null;

        }

        public string JointNameToReadableString(Joint jt)
        {
            string str = jt.JointType.ToString();
            RemoveStringAddToFront(ref str, "Left", " Left");
            RemoveStringAddToFront(ref str, "Right", " Right");
            return str;
        }

        private void RemoveStringAddToFront(ref string str, string oldChar, string newChar)
        {
            if (str.Contains(oldChar))
            {
                str = String.Concat(oldChar, " ", str.Replace(oldChar, ""));
            }
        }

  

        public Tolerance GetOptimalTolerance()
        {
            if (this.Tolerances.Count > 0)
            {
                foreach (Tolerance tol in this.Tolerances)
                {
                    if (tol.Optimal)
                    {
                        return tol;
                    }
                }
            }
            return null;
        }

        public string StripPreferenceFromJointName(string jointName)
        {
            string str = jointName;
            RemoveString(ref str, "Left");
            RemoveString(ref str, "Right");
            return str;
        }

        public void RemoveString(ref string str, string remove)
        {
            if (str.Contains(remove))
            {
                str = str.Replace(remove, "");
            }
        }

        public double GetCoordinateDifference(double a,
                                               double b)
        {
            double c = a  - b ;
            if (c < 0)
            {
                c = c * -1;
            }
            return c * 100;
        }

        public double GetAngleFromOpposite(double opposite,
                                            double hypotenuse)
        {
            return Math.Round(Math.Asin(opposite / hypotenuse) * 180 / Math.PI);
        }

        public double GetHypotenuse(double a, double b)
        {
            return Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
        }

        public virtual void Compare(Joint JointA,
                             Joint JointB,
                             Streaming stream,
                             ref ActionMessage am)
        {
            am.Colour = Brushes.Green;
        }

        public void CheckComparison(Joint JointA,
                                    Joint JointB,
                                    Streaming stream)
        {
            ActionMessage am = new ActionMessage();
            //this.Compare(JointA, JointB, stream, am);
        }

        public enum ComparisonType
        {
            Over,
            Under,
            Equal,
            Angle,
            Behind
        }

    }


}
