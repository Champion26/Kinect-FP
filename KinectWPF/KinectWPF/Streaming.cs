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
    public class Streaming
    {

        public KinectSensor sensor;
        public MultiSourceFrameReader reader;
 
        byte[] colourData;
        ColorImageFormat format;
        ColorFrameReader colourFrameReader;
        Canvas canvas;
        Image camera;

        public Brush JointColour;

        public HandPreference hand = HandPreference.Right;

        public bool bodyOn = false;

        System.Windows.Controls.Label window;

        private Generate generate;

        #region Image Streaming

        public void FillImageFromSensor()
        {
            //get current colour frame
            var frameInUse = sensor.ColorFrameSource;

            //get frame metadata
            var frameData = frameInUse.CreateFrameDescription(ColorImageFormat.Bgra);

            uint frameSize = frameData.BytesPerPixel * frameData.LengthInPixels;
            colourData = new byte[frameSize];
            format = ColorImageFormat.Bgra;

            colourFrameReader = frameInUse.OpenReader();
            //set frame arrived event handler
            colourFrameReader.FrameArrived += colourFrameReader_FrameArrived;
                        
        }

        public bool checkSensor()
        {
            KinectSensor sensor = KinectSensor.GetDefault();
            sensor.Open();
            if (sensor.IsAvailable)
            {
                return true;
            }
            return false;
        }

        public void checkAndRunSensor(Canvas c = null,
                                      Image i = null)
        {
            if (c != null)
            {
                this.canvas = c;
            }

            if (i != null)
            {
                this.camera = i;
            }

            generate = new Generate();

            generate.GetFromXml();

            if (canvas != null)
            {

                if (KinectSensor.GetDefault() is KinectSensor)
                {
                    sensor = KinectSensor.GetDefault();
                    //access sensor
                    sensor.Open();
                    //check if avaliable 
                    if (sensor.IsAvailable)
                    {
                        //start image fill
                        FillImageFromSensor();
                    }
                    else
                    {
                        //if sensor is unavaliable, check back in .25 seconds 
                        System.Threading.Thread.Sleep(250);
                        this.checkAndRunSensor();
                    }
                }
            }

        }

        public void colourFrameReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            if (e.FrameReference == null)
            {
                //check if frame reference is valid, if not stop 
                return;
            }
            using (ColorFrame colourFrame = e.FrameReference.AcquireFrame())
            {
                if (colourFrame == null)
                {
                    //check if colour frame is valid, if not stop 
                    return;
                }
                BitmapSource bmps = ConvertToBitmap(colourFrame);
                camera.Source = bmps;
      
            }
        }

        public BitmapSource ConvertToBitmap(ColorFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            byte[] pixels = new byte[width * height * ((format.BitsPerPixel + 7) / 8)];

            if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
            {
                frame.CopyRawFrameDataToArray(pixels);
            }
            else
            {
                frame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        public void StopSensorUsage()
        {
            if (reader != null)
            {
                reader.Dispose();
            }

            if (sensor != null)
            {
                sensor.Close();
            }
        }


        #endregion

        #region Body Tracking

        // Reader for body frames
        private BodyFrameReader bodyFrameReader = null;
        IList<Body> bodies;

        public void StartBodyTracking()
        {
            this.bodyOn = true;
            bodyFrameReader = sensor.BodyFrameSource.OpenReader();
            bodyFrameReader.FrameArrived += Body_FrameArrived;
        }

        public void StopBodyTracking()
        {
            this.bodyOn = false;
        }             

        private void Body_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            canvas.Children.Clear();

            if (bodyOn)
            {
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    bodies = new Body[bodyFrame.BodyFrameSource.BodyCount];

                    bodyFrame.GetAndRefreshBodyData(bodies);
                 
                    foreach (var body in bodies)
                    {

                        if (body.IsTracked)
                        {
                            List<ActionMessage> amList = new List<ActionMessage>();

                            List<Joint> drawJoints = new List<Joint>();

                            foreach (Bone bone in generate.Bones)
                            {
                                CheckAndDrawBone(body, bone.JointA, bone.JointB, ref amList, ref drawJoints);
                            }

                            DrawJoints(drawJoints);
                       
                        }
                    }
                }
            }
        }
           
        }

        private void CheckAndDrawBone(Body body,
                                      JointType JointA,
                                      JointType JointB,
                                      ref List<ActionMessage> amList,
                                      ref List<Joint> jointList)
        {
            if (CheckJoint(body, JointA) && CheckJoint(body, JointB))
            {
                CheckAndAddJointsToList(ref jointList, body, body.Joints[JointA]);
                CheckAndAddJointsToList(ref jointList, body, body.Joints[JointB]);
                amList.Add(DrawBone(body.Joints[JointA], body.Joints[JointB]));
            }
        }

        private void CheckAndAddJointsToList(ref List<Joint> jointList, Body body, Joint j)
        {
            if (!jointList.Contains(j))
            {
                jointList.Add(j);
            }
        }

        private void CheckAndAddJoints(ref List<Joint> ljt,
                                       JointType jt,
                                       Body body)
        {
            if (CheckJoint(body, jt))
            {
               ljt.Add(body.Joints[jt]);
            }
        }

        private bool CheckJoint(Body body,
                                JointType Joint)
        {
            if (body.Joints.ContainsKey(Joint))
            {
                if (body.Joints[Joint].TrackingState ==TrackingState.Tracked || body.Joints[Joint].TrackingState == TrackingState.Inferred)
                {
                    return true;
                }
            }
            return false;
        }

        public ColorSpacePoint GetXandYColourPoint(Joint joint)
        {

            CameraSpacePoint jointPosition = joint.Position;
            ColorSpacePoint colorPoint = sensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);
            
            return colorPoint;

        }

        private bool ValidCoordinateCheck(ColorSpacePoint csp)
        {
            bool r = false;

            if (csp.X > 0 || csp.Y > 0)
            {
                r = true;
            }

            return r;
        }

        private ActionMessage DrawBone(Joint first,
                              Joint second)
        {
            ActionMessage am = new ActionMessage();
            am.Colour = Brushes.Green;
            Line line = new Line();

            ColorSpacePoint fcp = GetXandYColourPoint(first);
            ColorSpacePoint scp = GetXandYColourPoint(second);

            if (ValidCoordinateCheck(fcp) && ValidCoordinateCheck(scp))
            {

                line.X1 = fcp.X;
                line.X2 = scp.X;
                line.Y1 = fcp.Y;
                line.Y2 = scp.Y;

                line.StrokeThickness = 4;

                ComparisonCheck(first, second, ref am);
                line.Stroke = am.Colour;
                Canvas.SetZIndex(line, -99);

                canvas.Children.Add(line);

                if (am.Error != null)
                {
                    canvas.Children.Add(GenerateInfoMessage(line, am));
                }
            }
            return am;
        }

        private TextBox GenerateInfoMessage(Line line,
                                         ActionMessage am)
        {
            TextBox tb = new TextBox();
            tb.Text = am.Error;
            Canvas.SetLeft(tb, FindMidpoint(line, CoordinateType.X));
            Canvas.SetTop(tb, FindMidpoint(line, CoordinateType.Y));
            return tb;
        }

        private double FindMidpoint(Line line,
                                    CoordinateType ct)
        {
            switch (ct)
            {
                case (CoordinateType.X):
                    return CalcMidPoint(line.X1, line.X2);
                case (CoordinateType.Y):
                    return CalcMidPoint(line.Y1, line.Y2);
            }

            return 0.0;
        }

        private double CalcMidPoint(double a, double b)
        {
            double dbl = (a - b) / 2;
            if (dbl < 0)
            {
                dbl = dbl * -1;
            }
            return FindMinValue(a, b) + dbl;
        }

        private double FindMinValue(double a,
                                    double b)
        {
            if (a > b)
            {
                return b;
            }
            return a;
        }

        private void DrawJoints(List<Joint> joints)
        {
            if (joints.Count > 0)
            {
                foreach (Joint joint in joints)
                {
                    if (joint.TrackingState == TrackingState.Tracked)
                    {
                        // Draw joint dots
                        DrawEllipse(joint);
                    }
                }
            }
        }



        private void DrawEllipse(Joint joint)
        {
            Ellipse ellipse = new Ellipse
            {
                Fill = this.JointColour,
                Width = 15,
                Height = 15
            };

            Canvas.SetLeft(ellipse, GetXandYColourPoint(joint).X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, GetXandYColourPoint(joint).Y - ellipse.Height / 2);
            Canvas.SetZIndex(ellipse, 99);
            canvas.Children.Add(ellipse);
        }
                    
       
        #endregion

        #region Coordinate Comparison

        private void ComparisonCheck(Joint JointA,
                                      Joint JointB,
                                      ref ActionMessage am)
        {

            Comparison c = new Comparison(this.generate, JointA, JointB, this);
            c.RunComparison(ref hand, ref am);
        }
        #endregion
        public enum HandPreference
        {
            Right,
            Left
        }

        public enum CoordinateType
        {
            X,
            Y
        }
    }
}
