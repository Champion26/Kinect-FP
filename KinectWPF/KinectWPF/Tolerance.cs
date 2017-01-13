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

        public Tolerance(double ut,
                         double lt,
                         Brush c)
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
        }


    }
}
