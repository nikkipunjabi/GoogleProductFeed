
## Google Product Feed

1)	What is Google Product Feed?    
2) 	About Google Product Feed Module  
3)	How to Install  
4)	Configuration

* Sitecore Configuration
* Config Configuration
  
5)	Output  
6)	Challenges faced

## What is Google Product Feed?
*	It is a feed in XML Format which we can submit to google for listing the products in Google Shopping Tab.
*	A google product feed xml file has list of product which use groupings of attributes that define each product in a unique way. Attributes, such as 'condition' and 'availability', can have standardized values, or accepted "answers," or attributes can be open to fill with your own choice of value, such as 'id' or 'title'. Accurately describing items using these attributes allows users to search and find your items more easily.
*	Google Shopping Tab is available in USA. It is not available in India.

## About Google Product Feed Module:
*	This will help you to create feed in very simple and easy way by just doing few configurations.
*	With help of this module you can create feed based on any one Sitecore Template.
*	You can specify whatever properties you want from a given template to read and create a file. Any property that you don’t want to specify for google product feed than just enter any other name for which you haven’t defined any field in a given template. For example – You don’t want to list _<g:mobile_link>_ then just enter the value **“NA”** with respect to a Name-Value field.
*	All the google product feed xml nodes are available.
*	Don’t remove any value from the Name Value list called **“field”**. Just specify **“null”** or **“NA”**, if you do not want any node to appear.
*	This module support Multi-Site Solution. If you have two sites in one solution and you want to create file for both than this could be easily done by this module.
*	Google Product Feed XML Sitecore Item will be created and published to web database. If the requested URL ends with googleproductfeed.xml – system will serve the request and display value from Google Product Feed XML Item.
*	This module supports RSS 2.0 Specification

## Useful Links:
* [Google Product Feed Example](http://nikkipunjabi.com/Sitecore/googleproductfeed.xml)
* [Products Feed Specification](https://support.google.com/merchants/answer/188494?vid=0-635787921466627000-317046340)
* [RSS 2.0 Specification](https://support.google.com/merchants/answer/160589?hl=en&ref_topic=2473799&vid=1-635795621251302496-2505481245)


## How To Install

1. Download the Zip file **NP_Google Product Feed Module** from Packages\V1\ or [NP_Google Product Feed Module]( http://nikkipunjabi.com/Sitecore/NP_Google Product Feed Module.zip)
2. Extract the zip file.
   It Contains:

	i. **NP_Google Product Feed Feed-1.0.0.0.zip** - A Package which you need to install in your Sitecore solution.
	 
    * This package contains:
  		* Template:
	       1. **Google Product Feed Configuration**
	       1. **Google Product Feed XML**
	    * Command and Scheduler: 
		    1. **Google Product Feed Command**
		    2. **Google Product Feed Scheduler**
	    * Sitecore Item
		    1. **Google Product Feed Configuration** – This contains core configuration which you need to do before starting with feed creation.
		    2. **Google Product Feed XML** – This will contain the feed information created for google to parse. A feed content will be stored. It is used to display xml feed.

    ii. **GoogleProductFeed.config** - For fetching configuration from Sitecore.

  * You have to modify the configuration if you are changing the path of Configuration and XML Item path in Sitecore.
  * Additionally you have to specify the Site name and rootItemPath in order to create a feed file. It is used to fetch the Site configuration for creating feed.
  
    iii. **GoogleProductFeed.dll** -  Add this file in your project bin folder. **_This will restart your Sitecore Solution._**


## Configurations

 1) **Sitecore Configurations**

   We'll start with **Google Product Feed Configuration** item. 

![Google Product Feed Configuration](http://nikkipunjabi.com/Sitecore/GPF/1.%20Google%20Product%20Feed%20Configuration.png) 

You need to specify the **Root Item Path** - A Path from where Module should start search for the Products.

**Template** - Template ID of the Products for which module should search for.

**Fields** - You need to specify the **field name** which feed should use for the respected google product feed xml node based on the selected Template.

![Google Product Feed Configurations](http://nikkipunjabi.com/Sitecore/GPF/2.%20Google%20Product%20Feed%20Configuration.png)

Here **Title** is the field which we have provided for Title, Description, gID, etc. You have to specify according to the requirement.

![Product - Title](http://nikkipunjabi.com/Sitecore/GPF/3.%20Product%20-%20Title.png)

***Note:*** *Do not remove any value from Name-Value field. Just specify any other value which is not a field in selected template, if you do not want a field in xml node.*

![Google Product Feed Configuration](http://nikkipunjabi.com/Sitecore/GPF/4.%20Google%20Product%20Feed%20Configuration.png)

**Add Currency** - This checkbox you have to enable if you want to append Currency after the Price. For example: 15 USD – Google Product Feed require the currency value. So if your specified Product Template ID Field doesn’t contain currency than enable this checkbox and enter the Currency Value.

**Description** – Google Product Feed requires description of not more than 5000 characters. So if your description exceeds the limit, than it will be truncated to 5000 characters.

**Scheduler** – Specify the Site-Root Path – Site for which you want to generate feed.

![Scheduler](http://nikkipunjabi.com/Sitecore/GPF/5.%20Scheduler.png)


2) **Config Configurations**

![Config Configurations](http://nikkipunjabi.com/Sitecore/GPF/7.%20Config%20Configuration.png)


***Note: If you want to generate feed for Multi-Site than you have to specify Site Root Item path in Scheduler and with respect to that same in Config Configurations.***

![Multi-Site Scheduler Configuration](http://nikkipunjabi.com/Sitecore/GPF/6.%20Multi-Site%20Config%20Configuration.png)

*So Config Configurations for above displayed Scheduler will be-*
![Multi-Site Config Configuration](http://nikkipunjabi.com/Sitecore/GPF/8.%20Multi-Site%20Config%20Configuration.png)

That’s it – You can now start the Scheduler. I use [Jobs.aspx](http://nikkipunjabi.com/Sitecore/Jobs.zip) for executing the Scheduler. Just copy these files in /Sitecore/admin/ and execute the scheduler.


##**Output:**
![Output](http://nikkipunjabi.com/Sitecore/GPF/9.%20Output.png)
