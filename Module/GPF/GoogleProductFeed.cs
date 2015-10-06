using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Publishing;
using GoogleProductFeed.Module.Pipelines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GoogleProductFeed.Module.GPF
{
    public class GoogleProductFeed
    {
        public string Title { get; set; }
        
        public string Link { get; set; }
        
        public string Description { get; set; }
        
        public string gID { get; set; }

        public string gItem_Group_ID { get; set; }

        public string gSize { get; set; }

        public string gSize_Type { get; set; }
        
        public string gMPN { get; set; }
        
        public string gBrand { get; set; }
        
        public string gCondition { get; set; }
        
        public string gPrice { get; set; }
        
        public string gAvailability { get; set; }
        
        public string gImage_Link { get; set; }

        public string gMobile_Link { get; set; }

        public string gAvailability_Date { get; set; }

        public string gSale_Price { get; set; }

        public string gTin { get; set; }

        public string gColor { get; set; }

        public string gGender { get; set; }

        public string gAge_Group { get; set; }

        public string gMaterial { get; set; }

        public string gPattern { get; set; }

        public string gAdditional_Image_Link { get; set; }
        
        public string gGoogle_Product_Category { get; set; }

        public string gCountry { get; set; }

        public string gService { get; set; }

        public string gShippingPrice { get; set; }

        public string gShipping_Weight { get; set; }

        public string gShipping_Label { get; set; }

        public string gMultipack { get; set; }

        public string gIs_Bundle { get; set; }

        public string gAdult { get; set; }

        public string gAdwords_Redirect { get; set; }

        public string gExpiration_Date { get; set; }

        public string gExcluded_Destination { get; set; }
        
        public List<GoogleProductType> gGoogle_Product_Type { get; set; }

    }

    public class GoogleProductType
    {
        public string gProduct_Type { get; set; }
    }

    public static class FieldNames
    {
        public const string Title = "Title";
        public const string Description = "Description";
        public const string gID = "gID";
        public const string gItem_Group_ID = "gItem_Group_ID";
        public const string gSize = "gSize";
        public const string gSize_Type = "gSize_Type";
        public const string gMPN = "gMPN";
        public const string gBrand = "gBrand";
        public const string gCondition = "gCondition";
        public const string gPrice = "gPrice";
        public const string gAvailability = "gAvailability";
        public const string gImage_Link = "gImage_Link";
        public const string gMobile_Link = "gMobile_Link";
        public const string gAvailability_Date = "gAvailability_Date";
        public const string gSale_Price = "gSale_Price";
        public const string gTin = "gTin";
        public const string gColor = "gColor";
        public const string gGender = "gGender";
        public const string gAge_Group = "gAge_Group";
        public const string gMaterial = "gMaterial";
        public const string gPattern = "gPattern";
        public const string gAdditional_Image_Link = "gAdditional_Image_Link";
        public const string gGoogle_Product_Category = "gGoogle_Product_Category";
        public const string gCountry = "gCountry";
        public const string gService = "gService";
        public const string gShipping_Price = "gShipping_Price";
        public const string gShipping_Weight = "gShipping_Weight";
        public const string gShipping_Label = "gShipping_Label";
        public const string gMultipack = "gMultipack";
        public const string gIs_Bundle = "gIs_Bundle";
        public const string gAdult = "gAdult";
        public const string gAdwords_Redirect = "gAdwords_Redirect";
        public const string gExpiration_Date = "gExpiration_Date";
        public const string gExcluded_Destination = "gExcluded_Destination";
        public const string gGoogle_Product_Type = "gGoogle_Product_Type";
    }
}
