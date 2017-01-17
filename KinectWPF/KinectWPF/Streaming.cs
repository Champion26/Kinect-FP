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
                camera.Source = ConvertToBitmap(colourFrame);
      
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
            bodyFrameReader.FrameArrived -= Body_FrameArrived;
            this.bodyOn = false;
        }             

        private void Body_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    canvas.Children.Clear();

                    bodies = new Body[bodyFrame.BodyFrameSource.BodyCount];

                    bodyFrame.GetAndRefreshBodyData(bodies);
                 
                    foreach (var body in bodies)
                    {

                        if (body.IsTracked)
                        {


                            List<Joint> drawJoints = new List<Joint>();
                            drawJoints.Add(body.Joints[JointType.Head]);
                            drawJoints.Add(body.Joints[JointType.Neck]);
                            drawJoints.Add(body.Joints[JointType.SpineShoulder]);
                            drawJoints.Add(body.Joints[JointType.ShoulderLeft]);
                            drawJoints.Add(body.Joints[JointType.ShoulderRight]);
                            drawJoints.Add(body.Joints[JointType.ElbowRight]);
                            drawJoints.Add(body.Joints[JointType.WristRight]);
                            drawJoints.Add(body.Joints[JointType.HandRight]);
                            drawJoints.Add(body.Joints[JointType.HandTipRight]);
                            drawJoints.Add(body.Joints[JointType.ElbowLeft]);
                            drawJoints.Add(body.Joints[JointType.WristLeft]);
                            drawJoints.Add(body.Joints[JointType.HandLeft]);
                            drawJoints.Add(body.Joints[JointType.HandTipLeft]);
                            drawJoints.Add(body.Joints[JointType.SpineMid]);
                            drawJoints.Add(body.Joints[JointType.SpineBase]);

                            //draw Joints
                            DrawJoints(drawJoints);

                            //draw centre of body
                            DrawBone(body.Joints[JointType.Head], body.Joints[JointType.Neck]);
                            DrawBone(body.Joints[JointType.Neck], body.Joints[JointType.SpineShoulder]);
                            DrawBone(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderLeft]);
                            DrawBone(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderRight]);
                            DrawBone(body.Joints[JointType.SpineShoulder], body.Joints[JointType.SpineMid]);
                            DrawBone(body.Joints[JointType.SpineMid], body.Joints[JointType.SpineBase]);

                            //draw right side
                            DrawBone(body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight]);
                            DrawBone(body.Joints[JointType.ElbowRight], body.Joints[JointType.WristRight]);
                            DrawBone(body.Joints[JointType.WristRight], body.Joints[JointType.HandRight]);
                            DrawBone(body.Joints[JointType.HandRight], body.Joints[JointType.HandTipRight]);
                                   
                     
                            //draw left side
                            DrawBone(body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft]);
                            DrawBone(body.Joints[JointType.ElbowLeft], body.Joints[JointType.WristLeft]);
                            DrawBone(body.Joints[JointType.WristLeft], body.Joints[JointType.HandLeft]);
                            DrawBone(body.Joints[JointType.HandLeft], body.Joints[JointType.HandTipLeft]);
                       
                        }

                    }
                  
                }
            }

           
        }

        public ColorSpacePoint GetXandYColourPoint(Joint joint)
        {

            CameraSpacePoint jointPosition = joint.Position;
            ColorSpacePoint colorPoint = sensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);
            
            return colorPoint;

        }

        private bool InfinityCoordinateCheck(ColorSpacePoint csp)
        {
            bool r = false;

            if (double.IsInfinity(csp.X) || double.IsInfinity(csp.Y))
            {
                r = true;
            }

            return r;
        }

        private void DrawBone(Joint first, Joint second)
        {

            Line line = new Line();

            ColorSpacePoint fcp = GetXandYColourPoint(first);
            ColorSpacePoint scp = GetXandYColourPoint(second);

            if (!(InfinityCoordinateCheck(fcp) && InfinityCoordinateCheck(scp)))
            {


                line.X1 = fcp.X;
                line.X2 = scp.X;
                line.Y1 = fcp.Y;
                line.Y2 = scp.Y;

                line.StrokeThickness = 2;

                List<Joint> jointList = new List<Joint>();
                jointList.Add(first);
                jointList.Add(second);

                line.Stroke = ComparisonCheck(jointList);

                canvas.Children.Add(line);

            }

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
                Fill = Brushes.Blue,
                Width = 10,
                Height = 10
            };

            Canvas.SetLeft(ellipse, GetXandYColourPoint(joint).X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, GetXandYColourPoint(joint).Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);
        }
                    
       
        #endregion

        #region Coordinate Comparison

        private Brush ComparisonCheck(List<Joint> joints)
        {

            return Brushes.Green;
            
                       
        }


        #endregion

        public enum HandPreference
        {
            Right,
            Left
        }
    }
}
