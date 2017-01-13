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
    class Comparison
    {
        private Joint _jointA;
        private Joint _jointB;
        private ComparisonType _comparisonType;
        private Streaming stream;

        private List<Tolerance> shoulderTolerances;

        public Joint JointA
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

        public Joint JointB
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

        public ComparisonType comparisonType
        {
            get
            {
                return _comparisonType;
            }
            set{
                _comparisonType = value;
            }
        }

        private Brush validColour
        {
            get
            {
                return Brushes.Green;
            }
        }

        private Brush toleranceColour
        {
            get
            {
                return Brushes.Yellow;
            }
        }

        private Brush invalidColour
        {
            get
            {
                return Brushes.Red;
            }
        }

        public List<Joint> getJointList()
        {
            List<Joint> joints = new List<Joint>();

            joints.Add(JointA);
            joints.Add(JointB);

            return joints;

        }

        public Comparison(Streaming s,
                          Joint jA,
                          Joint jB)
        {
            if (s != null)
            {
                this.stream = s;
            }
            if (jA != null)
            {
                this.JointA = jA;
            }
            if (jB != null)
            {
                this.JointB = jB;
            }
            shoulderTolerances = new List<Tolerance>();
            GenerateTolerances();

        }

        private void GenerateTolerances()
        {

            Tolerance shoulderValid = new Tolerance(45, 25, Brushes.Green);
            Tolerance shoulderAcceptable =  new Tolerance(55, 5, Brushes.Yellow);
            Tolerance shoulderInvalid =  new Tolerance(70, 0, Brushes.Red);

            shoulderTolerances.Add(shoulderValid);
            shoulderTolerances.Add(shoulderAcceptable);
            shoulderTolerances.Add(shoulderInvalid);


        }

        private int CalcJointValue(string jointName)
        {
            if (jointName.Contains(JointFlags.Elbow.ToString()))
            {
                return (int)JointFlags.Elbow;
            }
            else if (jointName.Contains(JointFlags.Shoulder.ToString()))
            {
                return (int)JointFlags.Shoulder;
            }
            else
            {
                return (int)JointFlags.Wrist;
            }
        }

        public void CalculateComparisonType()
        {
            //use flag system to work out joints

            int jointTotal = 0;

            string jointAName = JointA.JointType.ToString();
            string jointBName = JointB.JointType.ToString();

            //if both are of the same hand/side (left or right)
            if (jointAName.Contains(stream.hand.ToString()) && 
                jointBName.Contains(stream.hand.ToString())){

                    List<string> joints = new List<string>();
                    joints.Add(jointAName);
                    joints.Add(jointBName);

                    foreach (string joint in joints)
                    {
                        jointTotal += CalcJointValue(joint);
                    }

                    if (jointTotal > 0)
                    {

                        var values = EnumUtil.GetValues<ComparisonType>();
                        foreach (ComparisonType type in values)
                        {
                            if ((int)type == jointTotal)
                            {
                                comparisonType = type;
                                return;
                            }
                        }

                    }                
             }
           
            comparisonType = ComparisonType.None;
           

        }

        public static class EnumUtil
        {
            public static IEnumerable<T> GetValues<T>()
            {
                return Enum.GetValues(typeof(T)).Cast<T>();
            }
        }

        private Joint findJointByTypeInList(List<Joint> joints,
                                            string type)
        {
            Joint rJoint = new Joint();

            if (joints.Count > 0)
            {
                foreach (Joint joint in joints)
                {
                    if (joint.JointType.ToString().Contains(type))
                    {
                        rJoint = joint;
                        break;
                    }
                }
            }

            return rJoint;

        }

        private bool AOverB(Joint JointA,
                            Joint JointB)
        {
            if (JointA.Position.Y > JointB.Position.Y)
            {
                return true;
            }
            return false;
        }

        private Brush ShoulderElbowComparison()
        {
            List<Joint> joints = getJointList();


            //determine shoulder
            //determine elbow
            Joint shoulder = findJointByTypeInList(joints, "Shoulder");
            Joint elbow = findJointByTypeInList(joints, "Elbow");

            //coordinate comparison
            //first see if elbow is above shoulder

            if (AOverB(shoulder, elbow))
            {
                return invalidColour;
            }


            return MainElbowAngleCheck(shoulder, elbow);
        }               

        private double GetCoordinateDifference(double a,
                                               double b)
        {
            double c = a * 100 - b * 100;
            if (c < 0)
            {
                c = System.Math.Abs(c);
            }
            return c;
        }

        private double GetAngleFromOpposite(double opposite,
                                            double hypotenuse)
        {
            return Math.Asin(opposite / hypotenuse) * 180/Math.PI;
        }

        private Brush MainElbowAngleCheck(Joint shoulder,
                                          Joint elbow)
        {

            //get height difference of joints (will form part of triangle)
            double opposite = GetCoordinateDifference(shoulder.Position.Y, elbow.Position.Y);
            //set hypotenuse
            double adjacent = GetCoordinateDifference(shoulder.Position.X, elbow.Position.X);
            double hypo = GetHypotenuse(opposite, adjacent);




            double shoulderAngle = GetAngleFromOpposite(opposite, hypo);
            double elbowAngle = GetAngleFromOpposite(adjacent, hypo);
            double c = GetAngleFromOpposite(8, 16);
            

            Brush br = Brushes.Green;


            if (Math.Round(shoulderAngle) + Math.Round(elbowAngle) == 90)
            {
                //make sure angle values add up

                //check shoulder angle 
                
                foreach (Tolerance tl in shoulderTolerances)
                {
                    if (shoulderAngle > tl.lowerTolerance && shoulderAngle < tl.upperTolerance)
                    {
                        br = tl.colour;
                        break;
                    }
                }


            }

            return br;

        }

        public double GetHypotenuse(double a, double b)
        {
            return Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
        }

        private Brush ElbowWristComparison()
        {

            List<Joint> joints = getJointList();

            //determine elbow
            //determine wrist
            Joint elbow = findJointByTypeInList(joints, "Elbow");
            Joint wrist = findJointByTypeInList(joints, "Wrist");

            if (AOverB(wrist, elbow))           
            {
                return invalidColour;
            }
            
            return validColour;
        }
              
        public Brush Compare()
        {

            if (this.comparisonType != null)
            {
                if (this.comparisonType == ComparisonType.ShoulderToElbow)
                {
                    return ShoulderElbowComparison();
                }
                else if (this.comparisonType == ComparisonType.ElbowToWrist)
                {
                    //return ElbowWristComparison();
                }
                else
                {
                    return Brushes.Green;
                }
            }
               
            return Brushes.Green;
            


        }

    }
 
    [Flags]
    public enum JointFlags
    {
        Elbow = 1,
        Wrist = 2,
        Shoulder = 4
    }

    [Flags]
    public enum ComparisonType
    {
        ShoulderToElbow = 5,
        ElbowToWrist = 3,
        None = 0
    }

    public enum Limits
    {
       Valid,
       Acceptable,
       Invalid
    }


}
