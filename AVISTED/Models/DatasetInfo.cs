using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using System.Xml.Serialization;


namespace AVISTED.Models
{

    /// <remarks/>
      
        public class parameter
        {
            [XmlAttribute("name")]
            public string name { get; set; }
            [XmlElement("min")]
            public string min { get; set; }
            [XmlElement("max")]
            public string max { get; set; }
            [XmlElement("type")]
            public string type { get; set; }
            [XmlElement("display")]
            public string display { get; set; }
        }

    [XmlRoot("dataset")]
    public sealed class DatasetInfo
        {
            [XmlElement("name")]
            public string name { get; set; }
            [XmlElement("description")]
            public string description { get; set; }
            [XmlElement("format")]
            public string format { get; set; }
            [XmlElement("size")]
            public string size { get; set; }
            [XmlElement("author")]
            public string author { get; set; }
            [XmlElement("date")]
            public string date { get; set; }
            [XmlElement("path")]
            public string path { get; set; }
            [XmlElement("parameter")]
            public List<parameter> parameters { get; set; }
            public string parameterField { get; set; }
            public DateTime startDateField { get; set; }
            public DateTime endDateField { get; set; }
            public int latminField { get; set; }
            public int latmaxField { get; set; }
            public int lonminField { get; set; }
            public int lonmaxField { get; set; }
            public string statField { get; set; }
            public string outFormatField { get; set; }
            public Boolean saveDownload { get; set; }
            public string fileName { get; set; }
            public bool isPostback { get; set;}
    }


        
}


