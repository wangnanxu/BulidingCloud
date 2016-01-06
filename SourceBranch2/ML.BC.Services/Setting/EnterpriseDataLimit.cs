using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
namespace ML.BC.Services.Setting
{
    class EnterpriseDataLimit:IEnterpriseDataLimit
    {
        public List<string> GetXMLData()
        {
            XmlDocument xml = new XmlDocument();
//            xml.Load();
            return new List<string>();
        }

        public void UpdateXMLData()
        {
            throw new NotImplementedException();
        }
    }
}
