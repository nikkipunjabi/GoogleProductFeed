using Sitecore.Data.Items;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace GoogleProductFeed.Module.Pipelines
{
    class GoogleProductFeedHandler : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            bool isValidSite = ValidateSite();

            if (isValidSite)
                return;

            HttpContext currentContext = HttpContext.Current;

            if (currentContext == null)
                return;

            string requestedURL = currentContext.Request.Url.ToString().ToLower();

            if (requestedURL.EndsWith("googleproductfeed.xml"))
            {
                SiteContext siteContext = Sitecore.Context.Site;
                if (siteContext == null)
                    return;

                var sites = GoogleProductFeedConfiguration.GetSitecoreSites();

                var googleProductFeedConfiguration = sites.Where(x => x.RootItemPath.ToLower().Equals(Sitecore.Context.Site.StartPath.ToLower())).FirstOrDefault();

                if (googleProductFeedConfiguration != null)
                {

                    Item googleProductFeedXMLItem = Sitecore.Context.Database.GetItem(googleProductFeedConfiguration.XMLPath);

                    if (googleProductFeedXMLItem == null)
                        return;

                    string responseText = googleProductFeedXMLItem["Content"];
                    if (responseText.ToString().StartsWith("<br") || string.IsNullOrWhiteSpace(responseText))
                    {
                        XmlDocument doc = new XmlDocument();

                        XmlNode declarationNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                        doc.AppendChild(declarationNode);
                        XmlNode urlsetNode = doc.CreateElement("urlset");
                        XmlAttribute xmlnsAttr = doc.CreateAttribute("xmlns");
                        xmlnsAttr.Value = GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue;
                        urlsetNode.Attributes.Append(xmlnsAttr);

                        doc.AppendChild(urlsetNode);
                        responseText = doc.OuterXml;
                    }

                    currentContext.Response.ContentType = "text/xml";
                    currentContext.Response.Write(responseText);
                    try
                    {
                        currentContext.Response.End();
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                        // Ignore it
                    }
                }
            }
        }

        public static bool ValidateSite()
        {
            bool isValidSite = false;

            // When Site is null, we are not doing 
            // Any processing
            if (Sitecore.Context.Site == null)
                return true;

            switch (Sitecore.Context.Site.Name.ToLower())
            {
                case "shell":
                case "login":
                case "admin":
                case "service":
                case "modules_shell":
                case "modules_website":
                case "scheduler":
                case "system":
                case "publisher":
                    isValidSite = true;
                    break;
                default:
                    break;
            }

            return isValidSite;
        }
    }
}
