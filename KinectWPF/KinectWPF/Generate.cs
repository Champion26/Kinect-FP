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
        private List<Bone> _bones;

        public List<ComparisonRule> Comparisons
        {
            get
            {
                return _comparisons;
            }
        }

        public List<Bone> Bones
        {
            get
            {
                return _bones;
            }
        }

        public Generate()
        {
            _comparisons = new List<ComparisonRule>();
            _bones = new List<Bone>();

        }
  
        public XMLIssueMessage GetFromXml()
        {
            XMLIssueMessage msg = new XMLIssueMessage();

            try
            {
                XmlDocument xml = new XmlDocument();
                string filePath = String.Concat(System.AppDomain.CurrentDomain.BaseDirectory, "\\AppDetails.xml");
           
                try
                {
                    xml.Load(filePath);
                }
                catch
                {
                    msg = new XMLIssueMessage(true, String.Format("The XML rule sheet cannot be loaded. {0}", filePath), Brushes.Red, true);
                }
              
                if (xml.DocumentElement != null)
                {
                    XmlElement root = null;
                    try
                    {
                        root = xml.DocumentElement;
                    }
                    catch
                    {
                        msg = new XMLIssueMessage(true, "The XML root cannot be instantiated.", Brushes.Red, true);
                    }

                    if (root != null)
                    {
                        try
                        {
                            GenerateBonesAndJoints(root);
                            if (Bones.Count == 0)
                            {
                                throw new Exception();
                            }
                        }
                        catch
                        {
                            msg = new XMLIssueMessage(true, "There is an issue generating the joints and bones. Please check the XML rule sheet.", Brushes.Red, true);
                        }

                        try
                        {
                            GenerateComparisonRules(root);
                            if (Comparisons.Count == 0)
                            {
                                throw new Exception();
                            }
                        }
                        catch
                        {
                            msg = new XMLIssueMessage(true, "There is an issue generating the comparison rules. Please check the XML rule sheet.", Brushes.Red, true);
                        }
                    }
                }
            }
            catch
            {
                msg = new XMLIssueMessage(true, "There is an issue with the XML rule sheet", Brushes.Red, true);
            }

            return msg;
          
        }

        private void GenerateBonesAndJoints(XmlElement root)
        {
            XmlNodeList nodes = root.SelectNodes("Bones");
            if (nodes.Count > 0 && nodes.Item(0).ChildNodes.Count > 0)
            {
                foreach (XmlNode cmp in nodes.Item(0).ChildNodes)
                {
                    try
                    {
                        Bone bone = new Bone(MatchJointType(cmp, "JointA"),
                                      MatchJointType(cmp, "JointB"));
                        Bones.Add(bone);
                    }
                    catch
                    {

                    }                
                }
            }
        }

        private JointType MatchJointType(XmlNode cmp, string joint)
        {
            return (JointType)Enum.Parse(typeof(JointType), FindAttribute(cmp.Attributes, joint).Value.ToString());
        }

        private void GenerateComparisonRules(XmlElement root)
        {
            XmlNodeList nodes = root.SelectNodes("Comparisons");

            if (nodes.Count > 0 && nodes.Item(0).ChildNodes.Count > 0)
            {
                foreach (XmlNode cmp in nodes.Item(0).ChildNodes)
                {
                    try
                    {
                        ComparisonRule comparisonRule = new ComparisonRule(FindAttribute(cmp.Attributes, "JointA").Value.ToString(),
                                                                  FindAttribute(cmp.Attributes, "JointB").Value.ToString(),
                                                                  (ComparisonRule.ComparisonType)Enum.Parse(typeof(ComparisonRule.ComparisonType), FindAttribute(cmp.Attributes, "Comparison").Value.ToString()),
                                                                  cmp);

                        //possibly check rule validity

                        XmlNodeList toleranceNodes = cmp.SelectNodes("Tolerance");
                        if (toleranceNodes.Count > 0)
                        {
                            foreach (XmlNode subtol in toleranceNodes)
                            {
                                BrushConverter conv = new BrushConverter();

                                bool op = false;
                         
                                if (FindAttribute(subtol.Attributes, "Optimal") != null)
                                {
                                    op = Convert.ToBoolean(FindAttribute(subtol.Attributes, "Optimal").Value.ToString());
                                }

                                try
                                {
                                    Tolerance t = new Tolerance(Convert.ToDouble(FindAttribute(subtol.Attributes, "UpperLimit").Value.ToString()),
                                                           Convert.ToDouble(FindAttribute(subtol.Attributes, "LowerLimit").Value.ToString()),
                                                           conv.ConvertFromString(FindAttribute(subtol.Attributes, "Colour").Value.ToString()) as Brush,
                                                           op,
                                                           (Tolerance.ToleranceType)Enum.Parse(typeof(Tolerance.ToleranceType), FindAttribute(subtol.Attributes, "ToleranceType").Value.ToString()));

                                    comparisonRule.Tolerances.Add(t);
                                }
                                catch {}
                               
                            }
                        }
                        Comparisons.Add(comparisonRule);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public XmlNode FindAttribute(XmlAttributeCollection attributes, string name)
        {
            if (attributes.Count > 0)
            {
                foreach (XmlNode item in attributes)
                {
                    if (item.Name == name)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        
    }
}

public class XMLIssueMessage
{

    private bool _preventAnalysis;

    public bool preventAnalysis
    {
        get
        {
            return _preventAnalysis;
        }
        set
        {
            _preventAnalysis = value;
        }
    }

    private bool _issuePresent = false;

    public bool issuePresent
    {
        get
        {
            return _issuePresent;
        }
        set
        {
            _issuePresent = value;
        }
    }

    private Brush _colour;

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

    private string _msg;
    public string msg
    {
        get
        {
            return _msg;
        }
        set
        {
            _msg = value;
        }
    }

    public XMLIssueMessage(bool issue = false,
                            string txt = null,
                           Brush brush = null,
                           bool prevent = false)
    {

        issuePresent = issue;
        preventAnalysis = prevent;

        if (txt != null)
        {
            msg = txt;
        }

        if (brush != null)
        {
            colour = brush;
        }

    }

}
