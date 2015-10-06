using GoogleProductFeed.Module.Pipelines;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Publishing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GoogleProductFeed.Module.GPF
{
    public class GoogleProductFeedCreation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="googleProductFeedXMLItem"></param>
        /// <param name="xmlContent"></param>
        /// <param name="isItemUpdatedSuccessfully"></param>
        public static void UpdateGoogleProductFeedXMLItem(Item googleProductFeedXMLItem, string xmlContent, ref bool isItemUpdatedSuccessfully)
        {
            if (googleProductFeedXMLItem.Fields["Content"] != null)
            {
                try
                {
                    using (new Sitecore.SecurityModel.SecurityDisabler())
                    {
                        googleProductFeedXMLItem.Editing.BeginEdit();
                        googleProductFeedXMLItem["Content"] = xmlContent;
                        googleProductFeedXMLItem.Editing.EndEdit();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error in UpdateGoogleProductFeedXMLItem(). Item ID: " + googleProductFeedXMLItem.ID, ex, typeof(GoogleProductFeedCreation));
                    isItemUpdatedSuccessfully = false;
                }
            }
        }


        /// <summary>
        /// Publish Google Product Feed XML Item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isPublishAsync"></param>
        public static void PublishGoogleProductFeedXMLItem(Item googleProductFeedXMLItem, Database masterDb, bool isPublishAsync, Database TargetDb)
        {
            try
            {
                PublishOptions publishOptions = new PublishOptions(masterDb,
                                                                   TargetDb,
                                                                   Sitecore.Publishing.PublishMode.SingleItem,
                                                                   googleProductFeedXMLItem.Language,
                                                                   System.DateTime.Now);

                Publisher publisher = new Publisher(publishOptions);
                publisher.Options.RootItem = googleProductFeedXMLItem;
                publisher.Options.Deep = false;

                if (isPublishAsync)
                    publisher.PublishAsync();
                else
                    publisher.Publish();
            }
            catch (Exception ex)
            {
                Log.Error("Error in PublishGoogleProductFeedXMLItem(). Item ID: " + googleProductFeedXMLItem.ID, ex, typeof(GoogleProductFeedCreation));
            }
        }

        public static string BuildGoogleProductFeedXML(List<GoogleProductFeed> items)
        {
            XmlDocument doc = new XmlDocument();

            XmlElement rssNode = doc.CreateElement("rss");
            XmlAttribute xmlnsAttr = doc.CreateAttribute("xmlns:g");
            xmlnsAttr.Value = GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue;
            rssNode.SetAttributeNode(xmlnsAttr);

            xmlnsAttr = doc.CreateAttribute("version");
            xmlnsAttr.Value = GoogleProductFeedConfiguration.XmlnsVersion; ;
            rssNode.Attributes.Append(xmlnsAttr);

            XmlElement channelNode = doc.CreateElement("channel");
            rssNode.AppendChild(channelNode);

            doc.AppendChild(rssNode);

            if (items != null && items.Any())
            {
                foreach (var item in items)
                {
                    if (item != null)
                    {
                        doc = BuildGoogleProductFeedItem(doc, item);
                    }
                }
            }

            StringWriter sWriter = new StringWriter();
            XmlWriter xmlTextWriter = null;
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = " ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace
                };
                xmlTextWriter = XmlWriter.Create(sWriter, settings);
                doc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
            }
            finally
            {
                if (xmlTextWriter != null)
                    xmlTextWriter.Close();
            }

            return sWriter.GetStringBuilder().ToString();
        }

        public static XmlDocument BuildGoogleProductFeedItem(XmlDocument doc, GoogleProductFeed item)
        {
            const string prefix = "g";

            /*rss
                * channel
                    *item 
                        *title 
                        *link
                        *description 
                        *g:id 
                        *g:item_group_id
                        *g:size
                        *g:size_type
                        *g:MPN
                        *g:brand
                        *g:condition
                        *g:price
                        *g:availability
                        *g:image_link 500X500
                        *g:mobile_link
                        *g:availability_Date
                        *g:sale_Price
                        *g:tin
                        *g:color
                        *g:gender
                        *g:age_group
                        *g:material
                        *g:pattern
                        *g:additional_image_link
                        *g:google_product_category
                        *<g:shipping>
                           *<g:country>IN</g:country>
                           *<g:service>Standard</g:service>
                           *<g:price>10.00 INR</g:price>
                        *</g:shipping>
                        *g:shipping_weight
                        *g:shipping_label
                        *g:multipack
                        *g:is_bundle
                        *g:adult
                        *g:adwards_redirect
                        *g:expiration_date
                        *g:excluded_destination
                        *g:product_type
                    *item
                *channel
             *rss
             */

            XmlNode urlsetNode = doc.LastChild.LastChild;
            XmlElement itemNode = doc.CreateElement("item");
            urlsetNode.AppendChild(itemNode);

            if (!string.IsNullOrWhiteSpace(item.Title))
            {
                XmlNode titleNode = doc.CreateElement("title");
                titleNode.AppendChild(doc.CreateTextNode(item.Title));
                itemNode.AppendChild(titleNode);
            }

            if (!string.IsNullOrWhiteSpace(item.Link))
            {
                XmlNode linkNode = doc.CreateElement("link");
                linkNode.AppendChild(doc.CreateTextNode(item.Link));
                itemNode.AppendChild(linkNode);
            }

            if (!string.IsNullOrWhiteSpace(item.Description))
            {
                XmlNode descriptionNode = doc.CreateElement("description");
                descriptionNode.AppendChild(doc.CreateCDataSection(item.Description));
                itemNode.AppendChild(descriptionNode);
            }

            if (!string.IsNullOrWhiteSpace(item.gID))
            {
                XmlNode idNode = doc.CreateElement(prefix, "id", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                idNode.AppendChild(doc.CreateTextNode(item.gID));
                itemNode.AppendChild(idNode);
            }

            if (!string.IsNullOrWhiteSpace(item.gItem_Group_ID))
            {
                XmlNode itemGroupIdNode = doc.CreateElement(prefix, "item_group_id", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemGroupIdNode.AppendChild(doc.CreateTextNode(item.gItem_Group_ID));
                itemNode.AppendChild(itemGroupIdNode);
            }

            if (!string.IsNullOrWhiteSpace(item.gSize))
            {
                XmlNode sizeNode = doc.CreateElement(prefix, "size", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                sizeNode.AppendChild(doc.CreateTextNode(item.gSize));
                itemNode.AppendChild(sizeNode);
            }

            if (!string.IsNullOrWhiteSpace(item.gSize_Type))
            {
                XmlNode sizeTypeNode = doc.CreateElement(prefix, "size_type", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                sizeTypeNode.AppendChild(doc.CreateTextNode(item.gSize_Type));
                itemNode.AppendChild(sizeTypeNode);
            }

            if (!string.IsNullOrWhiteSpace(item.gMPN))
            {
                XmlNode MPNNode = doc.CreateElement(prefix, "MPN", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(MPNNode);
                MPNNode.AppendChild(doc.CreateTextNode(item.gMPN));
            }

            if (!string.IsNullOrWhiteSpace(item.gBrand))
            {
                XmlNode brandNode = doc.CreateElement(prefix, "brand", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(brandNode);
                brandNode.AppendChild(doc.CreateTextNode(item.gBrand));
            }

            if (!string.IsNullOrWhiteSpace(item.gCondition))
            {
                XmlNode conditionNode = doc.CreateElement(prefix, "condition", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(conditionNode);
                conditionNode.AppendChild(doc.CreateTextNode(item.gCondition));
            }

            if (!string.IsNullOrWhiteSpace(item.gPrice))
            {
                XmlNode priceNode = doc.CreateElement(prefix, "price", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(priceNode);
                priceNode.AppendChild(doc.CreateTextNode(item.gPrice));
            }

            if (!string.IsNullOrWhiteSpace(item.gCountry) || !string.IsNullOrWhiteSpace(item.gService) || !string.IsNullOrWhiteSpace(item.gPrice))
            {
                XmlNode shippingNode = doc.CreateElement(prefix, "shipping", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(shippingNode);

                if (!string.IsNullOrWhiteSpace(item.gCountry))
                {
                    XmlNode shippingCountryNode = doc.CreateElement(prefix, "country", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                    shippingCountryNode.AppendChild(doc.CreateTextNode(item.gCountry));
                    shippingNode.AppendChild(shippingCountryNode);
                }
                if (!string.IsNullOrWhiteSpace(item.gService))
                {
                    XmlNode shippingServiceNode = doc.CreateElement(prefix, "service", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                    shippingServiceNode.AppendChild(doc.CreateTextNode(item.gService));
                    shippingNode.AppendChild(shippingServiceNode);
                }
                if (!string.IsNullOrWhiteSpace(item.gPrice))
                {
                    XmlNode shippingPriceNode = doc.CreateElement(prefix, "price", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                    shippingPriceNode.AppendChild(doc.CreateTextNode(item.gPrice));
                    shippingNode.AppendChild(shippingPriceNode);
                }
            }

            if (!string.IsNullOrWhiteSpace(item.gAvailability))
            {
                XmlNode availabilityNode = doc.CreateElement(prefix, "availability", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(availabilityNode);
                availabilityNode.AppendChild(doc.CreateTextNode(item.gAvailability));
            }

            if (!string.IsNullOrWhiteSpace(item.gImage_Link))
            {
                XmlNode imageLinkNode = doc.CreateElement(prefix, "image_link", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(imageLinkNode);
                imageLinkNode.AppendChild(doc.CreateTextNode(item.gImage_Link));
            }

            if (!string.IsNullOrWhiteSpace(item.gMobile_Link))
            {
                XmlNode mobileLinkNode = doc.CreateElement(prefix, "mobile_link", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(mobileLinkNode);
                mobileLinkNode.AppendChild(doc.CreateTextNode(item.gMobile_Link));
            }

            if (!string.IsNullOrWhiteSpace(item.gAvailability_Date))
            {
                XmlNode availabilityDateNode = doc.CreateElement(prefix, "availability_date", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(availabilityDateNode);
                availabilityDateNode.AppendChild(doc.CreateTextNode(item.gAvailability_Date));
            }

            if (!string.IsNullOrWhiteSpace(item.gSale_Price))
            {
                XmlNode salePriceNode = doc.CreateElement(prefix, "sale_price", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(salePriceNode);
                salePriceNode.AppendChild(doc.CreateTextNode(item.gSale_Price));
            }

            if (!string.IsNullOrWhiteSpace(item.gTin))
            {
                XmlNode tinNode = doc.CreateElement(prefix, "tin", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(tinNode);
                tinNode.AppendChild(doc.CreateTextNode(item.gTin));
            }

            if (!string.IsNullOrWhiteSpace(item.gColor))
            {
                XmlNode colorNode = doc.CreateElement(prefix, "color", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(colorNode);
                colorNode.AppendChild(doc.CreateTextNode(item.gColor));
            }

            if (!string.IsNullOrWhiteSpace(item.gGender))
            {
                XmlNode genderNode = doc.CreateElement(prefix, "gender", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(genderNode);
                genderNode.AppendChild(doc.CreateTextNode(item.gGender));
            }

            if (!string.IsNullOrWhiteSpace(item.gAge_Group))
            {
                XmlNode ageGroupNode = doc.CreateElement(prefix, "age_group", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(ageGroupNode);
                ageGroupNode.AppendChild(doc.CreateTextNode(item.gAge_Group));
            }

            if (!string.IsNullOrWhiteSpace(item.gGoogle_Product_Category))
            {
                XmlNode googleProductCategoryNode = doc.CreateElement(prefix, "google_product_category", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(googleProductCategoryNode);
                googleProductCategoryNode.AppendChild(doc.CreateTextNode(item.gGoogle_Product_Category));
            }

            if (item.gGoogle_Product_Type != null && item.gGoogle_Product_Type.Count > 0)
            {
                foreach (var category in item.gGoogle_Product_Type)
                {
                    XmlNode productTypeNode = doc.CreateElement(prefix, "product_type", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                    itemNode.AppendChild(productTypeNode);
                    productTypeNode.AppendChild(doc.CreateTextNode(category.gProduct_Type));
                }
            }

            if (!string.IsNullOrWhiteSpace(item.gMaterial))
            {
                XmlNode materialNode = doc.CreateElement(prefix, "material", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(materialNode);
                materialNode.AppendChild(doc.CreateTextNode(item.gMaterial));
            }
            if (!string.IsNullOrWhiteSpace(item.gPattern))
            {
                XmlNode patternNode = doc.CreateElement(prefix, "pattern", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(patternNode);
                patternNode.AppendChild(doc.CreateTextNode(item.gPattern));
            }
            if (!string.IsNullOrWhiteSpace(item.gAdditional_Image_Link))
            {
                XmlNode additionalImageLinkNode = doc.CreateElement(prefix, "additional_image_link", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(additionalImageLinkNode);
                additionalImageLinkNode.AppendChild(doc.CreateTextNode(item.gAdditional_Image_Link));
            }
            if (!string.IsNullOrWhiteSpace(item.gShipping_Label))
            {
                XmlNode shipingLabelNode = doc.CreateElement(prefix, "shipping_label", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(shipingLabelNode);
                shipingLabelNode.AppendChild(doc.CreateTextNode(item.gShipping_Label));
            }
            if (!string.IsNullOrWhiteSpace(item.gShipping_Weight))
            {
                XmlNode shipingWeightNode = doc.CreateElement(prefix, "shipping_weight", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(shipingWeightNode);
                shipingWeightNode.AppendChild(doc.CreateTextNode(item.gShipping_Weight));
            }

            if (!string.IsNullOrWhiteSpace(item.gMultipack))
            {
                XmlNode multipackNode = doc.CreateElement(prefix, "multipack", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(multipackNode);
                multipackNode.AppendChild(doc.CreateTextNode(item.gMultipack));
            }
            if (!string.IsNullOrWhiteSpace(item.gIs_Bundle))
            {
                XmlNode isBundleNode = doc.CreateElement(prefix, "is_bundle", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(isBundleNode);
                isBundleNode.AppendChild(doc.CreateTextNode(item.gIs_Bundle));
            }
            if (!string.IsNullOrWhiteSpace(item.gAdult))
            {
                XmlNode adultNode = doc.CreateElement(prefix, "adult", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(adultNode);
                adultNode.AppendChild(doc.CreateTextNode(item.gAdult));
            }
            if (!string.IsNullOrWhiteSpace(item.gAdwords_Redirect))
            {
                XmlNode adwardsRedirectNode = doc.CreateElement(prefix, "adwards_redirect", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(adwardsRedirectNode);
                adwardsRedirectNode.AppendChild(doc.CreateTextNode(item.gAdwords_Redirect));
            }
            if (!string.IsNullOrWhiteSpace(item.gExpiration_Date))
            {
                XmlNode expiration_dateNode = doc.CreateElement(prefix, "expiration_date", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(expiration_dateNode);
                expiration_dateNode.AppendChild(doc.CreateTextNode(item.gExpiration_Date));
            }
            if (!string.IsNullOrWhiteSpace(item.gExcluded_Destination))
            {
                XmlNode excludedDestinationNode = doc.CreateElement(prefix, "excluded_destination", GoogleProductFeedConfiguration.XmlnsGoogleProductFeedValue);
                itemNode.AppendChild(excludedDestinationNode);
                excludedDestinationNode.AppendChild(doc.CreateTextNode(item.gExcluded_Destination));
            }

            return doc;
        }
    }
}
