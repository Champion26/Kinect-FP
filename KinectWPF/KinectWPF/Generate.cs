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
    class Generate
    {

        private List<ComparisonRule> _comparisons;

        public List<ComparisonRule> Comparisons
        {
            get
            {
                return _comparisons;
            }
        }

        public Generate()
        {
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
                        ComparisonRule comparisonRule = new ComparisonRule(cmp.Attributes.Item(0).Value.ToString(),
                                                                       cmp.Attributes.Item(1).Value.ToString(),
                                                                       (ComparisonRule.ComparisonType)Enum.Parse(typeof(ComparisonRule.ComparisonType), cmp.Attributes.Item(2).Value.ToString()));

                         //possibly check rule validity
                        if (comparisonRule.CompType == ComparisonRule.ComparisonType.Tolerance)
                        {
                            XmlNodeList toleranceNodes = cmp.SelectNodes("Tolerance");
                            if (toleranceNodes.Count > 0)
                            {
                                foreach (XmlNode subtol in toleranceNodes)
                                {
                                    BrushConverter conv = new BrushConverter();

                                    Tolerance t = new Tolerance(Convert.ToDouble(subtol.Attributes.Item(0).Value.ToString()),
                                                                Convert.ToDouble(subtol.Attributes.Item(1).Value.ToString()),
                                                                conv.ConvertFromString(subtol.Attributes.Item(2).Value.ToString()) as Brush);
                                    comparisonRule.Tolerances.Add(t);
                                }
                            }
                        }
                        _comparisons.Add(comparisonRule);
                    }
                }

            }
          
        }

        
    }
}
