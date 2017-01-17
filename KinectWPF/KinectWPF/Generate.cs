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
    class Generate
    {

        private List<ToleranceList> _tolerances;
        private List<ComparisonRule> _comparisons;

        public List<ToleranceList> Tolerances
        {
            get
            {
                return _tolerances;
            }
        }

        public List<ComparisonRule> Comparisons
        {
            get
            {
                return _comparisons;
            }
        }

        public Generate()
        {
            _tolerances = new List<ToleranceList>();
            _comparisons = new List<ComparisonRule>();
        }

  
        public void GetFromXml()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(String.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "\\AppDetails.xml"));

            if (xml.DocumentElement != null)
            {
                XmlElement root = xml.DocumentElement;
                XmlNodeList nodes = root.SelectNodes("Comparisons");

                if (nodes.Count > 0 && nodes.Item(0).ChildNodes.Count > 0)
                {
                    foreach (XmlNode cmp in nodes.Item(0).ChildNodes)
                    {
                        ComparisonRule comparison = new ComparisonRule(cmp.Attributes.Item(0).Value.ToString(),
                                                                       cmp.Attributes.Item(1).Value.ToString(),
                                                                       (ComparisonRule.ComparisonType)Enum.Parse(typeof(ComparisonRule.ComparisonType), cmp.Attributes.Item(2).Value.ToString()));

                         //possibly check rule validity
                        _comparisons.Add(comparison);
                    }
                }

                nodes = root.SelectNodes("Tolerances");
                if (nodes.Count > 0 && nodes.Item(0).ChildNodes.Count > 0)
                {
                    foreach (XmlNode tol in nodes.Item(0).ChildNodes)
                    {
                        ToleranceList tl = new ToleranceList(tol.Attributes.Item(0).Value, tol.Attributes.Item(1).Value);
                        foreach (XmlNode subtol in tol.ChildNodes)
                        {
                            BrushConverter conv = new BrushConverter();

                            Tolerance t = new Tolerance(Convert.ToDouble(subtol.Attributes.Item(0).Value.ToString()),
                                                        Convert.ToDouble(subtol.Attributes.Item(1).Value.ToString()),
                                                        conv.ConvertFromString(subtol.Attributes.Item(2).Value.ToString()) as Brush);
                            tl.Tolerances.Add(t);
                        }
                        _tolerances.Add(tl);

                    }

                }


            }
          
        }

        
    }
}
