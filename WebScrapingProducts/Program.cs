using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net;

class Program
{
    static void Main()
    {
        var html = @"<div class=""item"" rating = ""3"" data-pdid=""5426""><figure><a
href=""https://www.100percent.co.nz/Product/WCM7000WD/Electrolux-700LChest-Freezer""><img alt=""Electrolux 700L Chest Freezer &amp; Filter""
src=""/productimages/thumb/1/5426_5731_4009.jpg"" data-alternateimage=""/productimages/thumb/2/5426_5731_4010.jpg"" class=""mouseoverset""><span class=""overlay top-horizontal""><span class=""sold-out""><img
alt=""Sold Out""
Src=""/Images/Overlay/overlay_1_2_1.png""></span></span></a></figure><div
class=""item-detail""><h4><a
href=""https://www.100percent.co.nz/Product/WCM7000WD/Electrolux-700LChest-Freezer"">Electrolux 700L Chest Freezer</a></h4><div class=""pricing""
itemprop=""offers"" itemscope=""itemscope""
itemtype=""http://schema.org/Offer""><meta itemprop=""priceCurrency""
content=""NZD""><p class=""price""><span class=""price-display formatted""
itemprop=""price""><span style=""display: none"">$2,099.00</span>$<span
class=""dollars over500"">2,099</span><span class=""cents
zero"">.00</span></span></p></div><p class=""style-number"">WCM7000WD</p><p
class=""offer""><a
href=""https://www.100percent.co.nz/Product/WCM7000WD/Electrolux-700LChest-Freezer""><span style=""color:#CC0000;"">WCM7000WD</span></a></p><div
class=""item-asset""><!--.--></div></div></div>
<div class=""item"" rating = ""3.6"" data-pdid=""5862""><figure><a
href=""https://www.100percent.co.nz/Product/E203S/Electrolux-Anti-OdourVacuum-Bags""><img alt=""Electrolux Anti-Odour Vacuum Bags""
src=""/productimages/thumb/1/5862_6182_4541.jpg""></a></figure><div
class=""item-detail""><h4><a
href=""https://www.100percent.co.nz/Product/E203S/Electrolux-Anti-OdourVacuum-Bags"">Electrolux Anti-Odour Vacuum Bags</a></h4><div
class=""pricing"" itemprop=""offers"" itemscope=""itemscope""
itemtype=""http://schema.org/Offer""><meta itemprop=""priceCurrency""
content=""NZD""><p class=""price""><span class=""price-display formatted""
itemprop=""price""><span style=""display: none"">$22.99</span>$<span
class=""dollars"">22</span><span class=""cents"">.99</span></span></p></div><p
class=""style-number"">E203S</p><p class=""offer""><a
href=""https://www.100percent.co.nz/Product/E203S/Electrolux-Anti-Odour-
Vacuum-Bags""><span style=""color:#CC0000;"">E203S</span></a></p><div
class=""item-asset""><!--.--></div></div></div>
<div class=""item"" rating = ""8.4"" data-pdid=""4599""><figure><a
href=""https://www.100percent.co.nz/Product/USK11ANZ/Electrolux-UltraFlexStarter-Kit""><img alt=""Electrolux UltraFlex Starter &#91; Kit &#93; ""
src=""/productimages/thumb/1/4599_4843_2928.jpg""></a></figure><div
class=""item-detail""><h4><a
href=""https://www.100percent.co.nz/Product/USK11ANZ/Electrolux-UltraFlexStarter-Kit"">Electrolux UltraFlex &#64; Starter Kit</a></h4><div
class=""pricing"" itemprop=""offers"" itemscope=""itemscope""
itemtype=""http://schema.org/Offer""><meta itemprop=""priceCurrency""
content=""NZD""><p class=""price""><span class=""price-display formatted""
itemprop=""price""><span style=""display: none"">$44.99</span>$<span
class=""dollars"">44</span><span class=""cents"">.99</span></span></p></div><p
class=""style-number"">USK11ANZ</p><p class=""offer""><a
href=""https://www.100percent.co.nz/Product/USK11ANZ/Electrolux-UltraFlexStarter-Kit""><span style=""color:#CC0000;"">USK11ANZ</span></a></p><div
class=""item-asset""><!--.--></div></div></div>";

        var document = new HtmlDocument();
        document.LoadHtml(html);

        var products = new List<ProductInfo>();

        var productNodes = document.DocumentNode.SelectNodes("//div[@class='item']");
        if (productNodes != null)
        {
            foreach (var productNode in productNodes)
            {
                var productInformation = new ProductInfo();

                var productName = productNode.SelectSingleNode(".//img").GetAttributeValue("alt", null);
                productInformation.productName = WebUtility.HtmlDecode(productName);

                var priceNode = productNode.SelectSingleNode(".//p[@class='price']");
                if (priceNode != null)
                {
                    if (priceNode.SelectSingleNode(".//span[@class='dollars']") != null)
                    {
                        var price = priceNode.SelectSingleNode(".//span[@class='dollars']").InnerText.Trim() +
                                    priceNode.SelectSingleNode(".//span[@class='cents']").InnerText.Trim();

                        productInformation.price = price;
                    }
                    else
                    {
                        var productPrice = priceNode.SelectSingleNode(".//span[@class='dollars over500']").InnerHtml;
                        
                        if (decimal.TryParse(productPrice, NumberStyles.Number, CultureInfo.InvariantCulture, out var price)) 
                        {
                            productInformation.price = price.ToString("0.00").Replace(",", ".");
                        }               
                    }
                }

                var productRating = productNode.GetAttributeValue("rating", null);
                if (decimal.TryParse(productRating, NumberStyles.Number, CultureInfo.InvariantCulture, out var rating))
                {
                    if (rating > 5)
                    {
                        rating /= 2;
                    }
                    productInformation.rating = rating.ToString().Replace(",", ".");
                }
                products.Add(productInformation);
            }
        }

        var jsonResult = JsonConvert.SerializeObject(products, Formatting.Indented);
        Console.WriteLine(jsonResult);
    }
}

class ProductInfo
{
    public string productName { get; set; }
    public string price { get; set; }
    public string rating { get; set; }
}