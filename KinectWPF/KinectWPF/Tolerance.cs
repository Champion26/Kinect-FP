using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace KinectWPF
{
    class Tolerance
    {

        private double _upperTolerance;
        private double _lowerTolerance;
        private Brush _colour;
        private bool _optimal;
        private ToleranceType _toleranceType;

        public double upperTolerance
        {
            get
            {
                return _upperTolerance;
            }
            set
            {
                _upperTolerance = value;
            }
        }

        public double lowerTolerance
        {
            get
            {
                return _lowerTolerance;
            }
            set
            {
                _lowerTolerance = value;
            }
        }

        public Brush colour
        {
            get
            {
                return _colour;
            }
            set
            {
                _colour = value;
            }
        }

        public bool Optimal
        {
            get
            {
                return _optimal;
            }
            set
            {
                _optimal = value;
            }
        }

        public ToleranceType toleranceType
        {
            get
            {
                return _toleranceType;
            }
            set
            {
                _toleranceType = value;
            }
        }

        public Tolerance(double ut,
                         double lt,
                         Brush c,
                         bool op,
                         ToleranceType tt)
        {
            if (ut != null)
            {
                this.upperTolerance = ut;
            }
            if (lt != null)
            {
                this.lowerTolerance = lt;
            }
            if (c != null)
            {
                this.colour = c;
            }
            if (op != null)
            {
                this.Optimal = op;
            }
            this.toleranceType = tt;
        
        }


        public enum ToleranceType
        {
             Valid,
             Acceptable,
             Invalid
         }


    }
}
