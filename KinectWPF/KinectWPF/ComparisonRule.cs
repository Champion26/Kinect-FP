﻿using System;
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

        protected ComparisonRule(ComparisonRuleAboveBelow other)
        {
            this._jointA = other._jointA;
            this._jointB = other._jointB;
            this._comparisonType = other._comparisonType;
            this._tolerances = other._tolerances;
        }

        protected ComparisonRule(ComparisonRuleTolerance other)
        {
            this._jointA = other._jointA;
            this._jointB = other._jointB;
            this._comparisonType = other._comparisonType;
            this._tolerances = other._tolerances;
        }


        public ComparisonRule(string jA,
                              string jB,
                              ComparisonType ct)
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
            if (ct != null)
            {
                this._comparisonType = ct;
            }

        }

        public ComparisonRule()
        {

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
          ComparisonRule rComp = new ComparisonRule(this.JointA, this.JointB, this.CompType);
          if (this.Tolerances.Count > 0)
          {
              foreach (Tolerance tol in this.Tolerances)
              {
                  Tolerance rtol = new Tolerance(tol.upperTolerance, tol.lowerTolerance, tol.colour);
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

        public Brush CompareValueAgainstTolerances(double angle)
        {
            if (this.Tolerances.Count > 0)
            {
                foreach (Tolerance tol in this.Tolerances)
                {
                    if (angle > tol.lowerTolerance && angle < tol.upperTolerance)
                    {
                        return tol.colour;
                    }
                }
            }
            return Brushes.Green;
        }


        public double GetCoordinateDifference(double a,
                                               double b)
        {
            double c = a * 100 - b * 100;
            if (c < 0)
            {
                c = System.Math.Abs(c);
            }
            return c;
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

        public virtual Brush Compare(Joint JointA,
                             Joint JointB,
                             Streaming stream)
        {
            return Brushes.Green;
        }

        public Brush CheckComparison(Joint JointA,
                                    Joint JointB,
                                    Streaming stream)
        {
            return this.Compare(JointA, JointB, stream);
        }

        public enum ComparisonType
        {
            Over,
            Under,
            Equal,
            Tolerance
        }

    }


}
