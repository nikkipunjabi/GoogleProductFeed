using Sitecore.Configuration;
using Sitecore.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GoogleProductFeed.Module.Pipelines
{
    public class GoogleProductFeedConfiguration
    {
        #region Properties
        public static string XmlnsGoogleProductFeedValue
        {
            get
            {
                return GetValueByName("xmlnsGoogleProductFeedValue");
            }
        }

        public static string XmlnsVersion
        {
            get
            {
                return GetValueByName("xmlnsVersion");
            }
        }

        #endregion Properties

        #region Methods

        private static string GetValueByName(string name)
        {
            string result = string.Empty;

            foreach (XmlNode node in Factory.GetConfigNodes("GoogleProductFeed/GoogleFeedVariables/GoogleFeedVariable"))
            {
                if (XmlUtil.GetAttribute("name", node) == name)
                {
                    result = XmlUtil.GetAttribute("value", node);
                    break;
                }
            }

            return result;
        }

        public static bool IsGoogleProductFeedXMLSchedulerEnabled()
        {
            bool isEnabled = false;
            bool.TryParse(Sitecore.Configuration.Settings.GetSetting("GoogleProductFeed.SchedulerEnabled"), out isEnabled);

            return isEnabled;
        }

        public static List<GoogleProductFeedConfigurations> GetSitecoreSites()
        {
            List<GoogleProductFeedConfigurations> result = new List<GoogleProductFeedConfigurations>();
            var test = Factory.GetConfigNodes("GoogleProductFeed/SitecorePaths/Site");
            foreach (XmlNode node in Factory.GetConfigNodes("GoogleProductFeed/SitecorePaths/Site"))
            {
                if (XmlUtil.GetAttribute("name", node) != null)
                {
                    var googleProductFeedConfigurations = new GoogleProductFeedConfigurations();
                    googleProductFeedConfigurations.SiteName = XmlUtil.GetAttribute("name", node);
                    googleProductFeedConfigurations.UrlPrefix = XmlUtil.GetAttribute("urlPrefix", node);
                    googleProductFeedConfigurations.RootItemPath = XmlUtil.GetAttribute("rootItemPath", node);
                    googleProductFeedConfigurations.ConfigurationPath = node.ChildNodes[0].Attributes["value"].Value;
                    googleProductFeedConfigurations.XMLPath = node.ChildNodes[1].Attributes["value"].Value; //XmlUtil.GetAttribute("XMLPath", node.ChildNodes[0]);
                    

                    result.Add(googleProductFeedConfigurations);
                }
            }

            return result;
        }

        public class GoogleProductFeedConfigurations
        {
            public string SiteName { get; set; }
            public string UrlPrefix { get; set; }
            public string RootItemPath { get; set; }
            public string XMLPath { get; set; }
            public string ConfigurationPath { get; set; }
        }

        #endregion
    }
}
