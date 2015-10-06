using GoogleProductFeed.Module.Pipelines;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleProductFeed.Module.GPF
{
    public class GoogleProductFeedTaskCommand
    {
        public void CreateGoogleProductFeed(Sitecore.Data.Items.Item[] items, Sitecore.Tasks.CommandItem command,
           Sitecore.Tasks.ScheduleItem scheduleItem)
        {

            if (!GoogleProductFeedConfiguration.IsGoogleProductFeedXMLSchedulerEnabled())
            {
                Log.Info("GoogleProductFeed scheduler is disabled. To enable it check GoogleProductFeed.config-> GoogleProductFeed.SchedulerEnabled setting.", this);
                return;
            }

            if (items != null && items.Any())
            {
                Log.Info("Google Product Feed Started", this);
                foreach (var item in items)
                {
                    var sitesInConfig = GoogleProductFeedConfiguration.GetSitecoreSites();

                    var currentConfigurations = sitesInConfig.Where(x => x.RootItemPath.ToLower().Equals(item.Paths.FullPath.ToLower())).FirstOrDefault();

                    if (currentConfigurations != null)
                    {
                        try
                        {
                            List<GoogleProductFeed> feeds = new List<GoogleProductFeed>();

                            //Get Google Product Feed Configuration Item
                            Item GPFConfigurationItem = Sitecore.Configuration.Factory.GetDatabase("master").GetItem(currentConfigurations.ConfigurationPath); //Sitecore.Context.Database.GetItem(new Sitecore.Data.ID("{3DA415DD-526E-4F59-BDE4-F10D07EFC7F1}"));

                            LinkField rootFolder = GPFConfigurationItem.Fields["Root Item Path"];

                            NameValueListField GPFProperties = GPFConfigurationItem.Fields["Fields"];
                            NameValueCollection fields = GPFProperties.NameValues;

                            if (GPFProperties != null && GPFProperties.NameValues != null && GPFProperties.NameValues.Count > 0)
                            {
                                string ProductTemplateID = GPFConfigurationItem["Template"];

                                using (var context = ContentSearchManager.CreateSearchContext((SitecoreIndexableItem)item))
                                {
                                    // All Products
                                    var searchResultItems = context.GetQueryable<SearchResultItem>()
                                        .Where(x => x.TemplateId == Sitecore.Data.ID.Parse(ProductTemplateID) && x.Path.Contains(rootFolder.TargetItem.Paths.FullPath))
                                        .AsEnumerable()
                                        .Where(x => x != null && x.GetItem() != null && x.Version == x.GetItem().Versions.GetLatestVersion().Version.ToString());

                                    if (searchResultItems != null && searchResultItems.Any())
                                    {
                                        foreach (var results in searchResultItems)
                                        {
                                            GoogleProductFeed feed = new GoogleProductFeed();
                                            var itemtest = results.GetItem();
                                            feed.Title = (results.GetField(fields[FieldNames.Title]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.Title]).Value))) ? results.GetField(fields[FieldNames.Title]).Value : string.Empty;

                                            Sitecore.Links.UrlOptions URLOptions = new Sitecore.Links.UrlOptions();
                                            URLOptions.Site = Sitecore.Configuration.Factory.GetSite(currentConfigurations.SiteName);
                                            URLOptions.LanguageEmbedding = LanguageEmbedding.Never;
                                            var itemUrl = LinkManager.GetItemUrl(results.GetItem(), URLOptions);
                                            feed.Link = string.Format("{0}{1}", currentConfigurations.UrlPrefix, itemUrl);
                                            feed.Description = (results.GetField(fields[FieldNames.Description]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.Description]).Value))) ? results.GetField(fields[FieldNames.Description]).Value : string.Empty;
                                            feed.gID = (results.GetField(fields[FieldNames.gID]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gID]).Value))) ? results.GetField(fields[FieldNames.gID]).Value : string.Empty;
                                            feed.gItem_Group_ID = (results.GetField(fields[FieldNames.gItem_Group_ID]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gItem_Group_ID]).Value))) ? results.GetField(fields[FieldNames.gItem_Group_ID]).Value : string.Empty;
                                            feed.gSize = (results.GetField(fields[FieldNames.gSize]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gSize]).Value))) ? results.GetField(fields[FieldNames.gSize]).Value : string.Empty;
                                            feed.gSize_Type = (results.GetField(fields[FieldNames.gSize_Type]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gSize_Type]).Value))) ? results.GetField(fields[FieldNames.gSize_Type]).Value : string.Empty;
                                            feed.gMPN = (results.GetField(fields[FieldNames.gMPN]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gMPN]).Value))) ? results.GetField(fields[FieldNames.gMPN]).Value : string.Empty;
                                            feed.gBrand = (results.GetField(fields[FieldNames.gBrand]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gBrand]).Value))) ? results.GetField(fields[FieldNames.gBrand]).Value : string.Empty;
                                            feed.gCondition = (results.GetField(fields[FieldNames.gCondition]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gCondition]).Value))) ? results.GetField(fields[FieldNames.gCondition]).Value : string.Empty;
                                            feed.gPrice = (results.GetField(fields[FieldNames.gPrice]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gPrice]).Value))) ? results.GetField(fields[FieldNames.gPrice]).Value : string.Empty;
                                            if (!string.IsNullOrWhiteSpace(feed.gPrice) && GPFConfigurationItem.Fields["Add Currency"].Value.Equals("1"))
                                            {
                                                feed.gPrice = feed.gPrice + " " + GPFConfigurationItem.Fields["Currency"].Value;
                                            }
                                            feed.gAvailability = (results.GetField(fields[FieldNames.gAvailability]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gAvailability]).Value))) ? results.GetField(fields[FieldNames.gAvailability]).Value : string.Empty;
                                            feed.gImage_Link = (results.GetField(fields[FieldNames.gImage_Link]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gImage_Link]).Value))) ? results.GetField(fields[FieldNames.gImage_Link]).Value : string.Empty;
                                            feed.gMobile_Link = (results.GetField(fields[FieldNames.gMobile_Link]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gMobile_Link]).Value))) ? results.GetField(fields[FieldNames.gMobile_Link]).Value : string.Empty;
                                            feed.gAvailability_Date = (results.GetField(fields[FieldNames.gAvailability_Date]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gAvailability_Date]).Value))) ? results.GetField(fields[FieldNames.gAvailability_Date]).Value : string.Empty;
                                            feed.gSale_Price = (results.GetField(fields[FieldNames.gSale_Price]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gSale_Price]).Value))) ? results.GetField(fields[FieldNames.gSale_Price]).Value : string.Empty;
                                            feed.gTin = (results.GetField(fields[FieldNames.gTin]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gTin]).Value))) ? results.GetField(fields[FieldNames.gTin]).Value : string.Empty;
                                            feed.gColor = (results.GetField(fields[FieldNames.gColor]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gColor]).Value))) ? results.GetField(fields[FieldNames.gColor]).Value : string.Empty;
                                            feed.gGender = (results.GetField(fields[FieldNames.gGender]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gGender]).Value))) ? results.GetField(fields[FieldNames.gGender]).Value : string.Empty;
                                            feed.gAge_Group = (results.GetField(fields[FieldNames.gAge_Group]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gAge_Group]).Value))) ? results.GetField(fields[FieldNames.gAge_Group]).Value : string.Empty;
                                            feed.gMaterial = (results.GetField(fields[FieldNames.gMaterial]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gMaterial]).Value))) ? results.GetField(fields[FieldNames.gMaterial]).Value : string.Empty;
                                            feed.gPattern = (results.GetField(fields[FieldNames.gPattern]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gPattern]).Value))) ? results.GetField(fields[FieldNames.gPattern]).Value : string.Empty;
                                            feed.gAdditional_Image_Link = (results.GetField(fields[FieldNames.gAdditional_Image_Link]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gAdditional_Image_Link]).Value))) ? results.GetField(fields[FieldNames.gAdditional_Image_Link]).Value : string.Empty;
                                            feed.gGoogle_Product_Category = (results.GetField(fields[FieldNames.gGoogle_Product_Category]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gGoogle_Product_Category]).Value))) ? results.GetField(fields[FieldNames.gGoogle_Product_Category]).Value : string.Empty;
                                            feed.gCountry = (results.GetField(fields[FieldNames.gCountry]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gCountry]).Value))) ? results.GetField(fields[FieldNames.gCountry]).Value : string.Empty;
                                            feed.gService = (results.GetField(fields[FieldNames.gService]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gService]).Value))) ? results.GetField(fields[FieldNames.gService]).Value : string.Empty;
                                            feed.gShippingPrice = (results.GetField(fields[FieldNames.gShipping_Price]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gShipping_Price]).Value))) ? results.GetField(fields[FieldNames.gShipping_Price]).Value : string.Empty;
                                            if (!string.IsNullOrWhiteSpace(feed.gShippingPrice) && GPFConfigurationItem.Fields["Add Currency"].Value.Equals("1"))
                                            {
                                                feed.gShippingPrice = feed.gShippingPrice + " " + GPFConfigurationItem.Fields["Currency"].Value;
                                            }
                                            feed.gShipping_Weight = (results.GetField(fields[FieldNames.gShipping_Weight]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gShipping_Weight]).Value))) ? results.GetField(fields[FieldNames.gShipping_Weight]).Value : string.Empty;
                                            feed.gShipping_Label = (results.GetField(fields[FieldNames.gShipping_Label]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gShipping_Label]).Value))) ? results.GetField(fields[FieldNames.gShipping_Label]).Value : string.Empty;
                                            feed.gMultipack = (results.GetField(fields[FieldNames.gMultipack]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gMultipack]).Value))) ? results.GetField(fields[FieldNames.gMultipack]).Value : string.Empty;
                                            feed.gIs_Bundle = (results.GetField(fields[FieldNames.gIs_Bundle]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gIs_Bundle]).Value))) ? results.GetField(fields[FieldNames.gIs_Bundle]).Value : string.Empty;
                                            feed.gAdult = (results.GetField(fields[FieldNames.gAdult]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gAdult]).Value))) ? results.GetField(fields[FieldNames.gAdult]).Value : string.Empty;
                                            feed.gAdwords_Redirect = (results.GetField(fields[FieldNames.gAdwords_Redirect]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gAdwords_Redirect]).Value))) ? results.GetField(fields[FieldNames.gAdwords_Redirect]).Value : string.Empty;
                                            feed.gExpiration_Date = (results.GetField(fields[FieldNames.gExpiration_Date]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gExpiration_Date]).Value))) ? results.GetField(fields[FieldNames.gExpiration_Date]).Value : string.Empty;
                                            feed.gExcluded_Destination = (results.GetField(fields[FieldNames.gExcluded_Destination]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gExcluded_Destination]).Value))) ? results.GetField(fields[FieldNames.gExcluded_Destination]).Value : string.Empty;
                                            feed.gGoogle_Product_Type = new List<GoogleProductType>();
                                            GoogleProductType type = new GoogleProductType();
                                            type.gProduct_Type = (results.GetField(fields[FieldNames.gGoogle_Product_Type]) != null && (!string.IsNullOrWhiteSpace(results.GetField(fields[FieldNames.gGoogle_Product_Type]).Value))) ? results.GetField(fields[FieldNames.gGoogle_Product_Type]).Value : string.Empty;
                                            feed.gGoogle_Product_Type.Add(type);

                                            if (!string.IsNullOrWhiteSpace(feed.Description) && feed.Description.Count() > 5000)
                                            {
                                                feed.Description = feed.Description.Take(5000).ToString();
                                            }
                                            feeds.Add(feed);
                                        }
                                    }
                                }
                            }

                            try
                            {
                                if (feeds != null && feeds.Count() > 0)
                                {

                                    feeds = feeds.OrderBy(x => x.Title).ToList();
                                    string xmlContent = GoogleProductFeedCreation.BuildGoogleProductFeedXML(feeds);

                                    if (!string.IsNullOrWhiteSpace(xmlContent))
                                    {
                                        // Get Google Product Feed XML item from Master database, Update it and Publish it
                                        Database masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
                                        if (masterDb != null)
                                        {
                                            Item googleProductFeedXMLItem = masterDb.GetItem(currentConfigurations.XMLPath);
                                            if (googleProductFeedXMLItem != null)
                                            {
                                                bool isItemUpdatedSuccessfully = true;
                                                GoogleProductFeedCreation.UpdateGoogleProductFeedXMLItem(googleProductFeedXMLItem, xmlContent, ref isItemUpdatedSuccessfully);

                                                if (isItemUpdatedSuccessfully)
                                                {
                                                    GoogleProductFeedCreation.PublishGoogleProductFeedXMLItem(googleProductFeedXMLItem, masterDb, true, Sitecore.Configuration.Factory.GetDatabase("web"));
                                                }
                                            }
                                            else
                                            {
                                                Log.Warn("GoogleProductFeed XML Item not found for " + Sitecore.Context.Site.StartPath, this);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Error while generating googleproductfeed.xml for " + Sitecore.Context.Site, ex, this);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Error in Google Product Feed", ex, this);
                        }
                    }
                }

                Log.Info("Google Product Feed Ended", this);
            }
        }
    }
}
