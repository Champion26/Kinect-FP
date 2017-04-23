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
using System.Xml.Linq;

namespace KinectWPF
{
    class ToleranceList
    {

        private List<Tolerance> _tolerances;

        public List<Tolerance> Tolerances
        {
            get
            {
                return _tolerances;
            }
        }

        public ToleranceList(ToleranceListType type = ToleranceListType.None)
        {
            _tolerances = new List<Tolerance>();
            switch (type)
            {
                case ToleranceListType.DominantShoulder:
                    GenerateDominantShoulderTolerances();
                    break;
                default:
                    DoNothing();
                    break;
            }

        }

        public void DoNothing()
        {

        }

        private List<Tolerance> CreateTolerancesFromXML(string toleranceName)
        {
            List<Tolerance> tls = new List<Tolerance>();

            XmlDocument xml = new XmlDocument(); 
            xml.Load(String.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName,"\\AppDetails.xml"));

            if (xml.DocumentElement != null )
            {
                XmlElement root = xml.DocumentElement;
                XmlNodeList nodes = root.SelectNodes(String.Concat("/ToleranceValues/", toleranceName));

                if (nodes.Count > 0 && nodes.Item(0).ChildNodes.Count > 0)
                {
                    foreach (XmlNode node in nodes.Item(0).ChildNodes)
                    {
                        if (node.Attributes.Count == 3)
                        {
                            BrushConverter conv = new BrushConverter();
                            Brush br = conv.ConvertFromString(node.Attributes.Item(2).Value.ToString()) as Brush;
                            Tolerance tl = new Tolerance(
                                                         Convert.ToDouble(
                                                            node.Attributes.Item(0).Value.ToString()),
                                                         Convert.ToDouble(
                                                            node.Attributes.Item(1).Value.ToString()),
                                                         br);
                            tls.Add(tl);
                        }
                    }
                }
            }
    

            return tls;

        }

        public void GenerateDominantShoulderTolerances()
        {
            //retrieve from XML

            List<Tolerance> tls = CreateTolerancesFromXML("DominantShoulder");

            if (tls.Count > 0)
            {
                foreach (Tolerance t in tls)
                {
                    this.Tolerances.Add(t);
                }
            }
            

        }

        public Brush CompareValueAgainstTolerances(double value)
        {
            foreach (Tolerance t in this.Tolerances)
            {
                if (value > t.lowerTolerance && value < t.upperTolerance)
                {
                    return t.colour;
                }
            }
            return Brushes.Green;
        }

        public enum ToleranceListType
        {
            DominantShoulder,
            None
        }

      
    }

    
}
