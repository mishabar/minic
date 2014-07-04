using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Minie.Carters.Interfaces.Repositories;
using Minie.Carters.Repositories;

namespace Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            MongoDB.Driver.MongoClient client = new MongoDB.Driver.MongoClient("mongodb://appharbor_3c3466c7-6308-4646-82f9-f2a3f03fe811:thrk7ku9kpm3bag0f0ndrhjgdo@ds027799.mongolab.com:27799/appharbor_3c3466c7-6308-4646-82f9-f2a3f03fe811");
            IProductsRepository repo = new ProductsRepository(client.GetServer().GetDatabase("appharbor_3c3466c7-6308-4646-82f9-f2a3f03fe811"));
            ICategoriesRepository crepo = new CategoriesRepository(client.GetServer().GetDatabase("appharbor_3c3466c7-6308-4646-82f9-f2a3f03fe811"));

            WebClient wc = new WebClient();

            string filename = null;
            string url = null;
            long timestamp = DateTime.Now.Ticks;

            foreach (var category in crepo.FindAll())
            {
                foreach (var scutegory in category.SubCategories)
                {
                    url = scutegory.OriginUrl;
                    filename = Path.GetTempFileName();
                    wc.DownloadFile(url, filename);
                    HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                    document.Load(filename, true);
                    File.Delete(filename);

                    HtmlAgilityPack.HtmlNodeCollection tiles = document.DocumentNode.SelectNodes("//div[@data-itemid]");

                    foreach (var product in tiles)
                    {
                        try
                        {
                            string name = product.SelectSingleNode("div[@class='product-name']/h2/a").InnerText.Trim().Replace("&amp;", "&").Replace("&#39;", "'");
                            string id = product.GetAttributeValue("data-itemid", null);
                            HtmlAgilityPack.HtmlNode node = product.SelectSingleNode("div[@class='product-pricing']//span[@class='product-standard-price']");
                            string msrp = node == null ? "$0" : node.InnerText.Trim().Split('\n')[1];
                            string price = product.SelectSingleNode("div[@class='product-pricing']//span[starts-with(@class, 'product-sales-price')]").InnerText.Trim().Split('-').Last().Trim();
                            string image = product.SelectSingleNode("div[@class='product-image']/a/img").GetAttributeValue("src", null).Split('?')[0];

                            Console.WriteLine(string.Format("{0} ({1})\r\n{2} -> {3}\r\n{4}\r\n", name, id, msrp, price, image));
                            var quickViewLink = product.SelectSingleNode(".//a[@class='thumb-link']");

                            Minie.Carters.Data.Product dbProduct = new Minie.Carters.Data.Product
                            {
                                Name = name,
                                SKU = id,
                                Image = image,
                                Active = true,
                                MSRP = (msrp == "$0" ? new float?() : float.Parse(msrp.Substring(1))),
                                Price = float.Parse(price.Substring(1)) * (float)(scutegory.CategoryID.ToLower().Contains("clearance") ? 1.2 : 1),
                                Category = scutegory.CategoryID,
                                Sizes = new List<string>(),
                                Timestamp = timestamp
                            };

                            try
                            {
                                HtmlAgilityPack.HtmlDocument document2 = new HtmlAgilityPack.HtmlDocument();
                                //wc.DownloadFile(url.Split('?')[0] + "/" + id + ".html?source=quickview&format=ajax", filename);
                                string url2 = quickViewLink.GetAttributeValue("href", string.Empty);
                                if (!url2.StartsWith("http://"))
                                    url2 = "http://www.carters.com" + url2;
                                wc.DownloadFile(url2, filename);
                                document2.Load(filename);

                                HtmlAgilityPack.HtmlNodeCollection sizes = document2.DocumentNode.SelectNodes("//ul[@class='swatches size']/li/a");
                                if (sizes != null)
                                {
                                    foreach (var size in sizes)
                                    {
                                        if (!size.ParentNode.GetAttributeValue("class", string.Empty).Contains("unselectable"))
                                        {
                                            dbProduct.Sizes.Add(size.GetAttributeValue("title", string.Empty));
                                        }
                                    }
                                }
                                dbProduct.Invalid = false;
                            }
                            catch (Exception ex)
                            {
                                dbProduct.Invalid = true;
                            }
                            repo.Save(dbProduct);
                        }
                        catch
                        { 
                        }
                    }
                }
            }

            repo.DeleteOutdated(timestamp);

            //crepo.Save(new Minie.Carters.Data.Category
            //{
            //    CategoryID = "baby-boy",
            //    Name = "Baby Boy",
            //    SubCategories = new List<Minie.Carters.Data.Category>(
            //        new Minie.Carters.Data.Category[] {
            //            new Minie.Carters.Data.Category { CategoryID = "baby-boy-sets", Name = "Sets", OriginUrl = "http://www.carters.com/carters-baby-baby-boy-sets?cgid=carters-baby-baby-boy-sets&startRow=0&sz=all&format=ajax" },
            //            new Minie.Carters.Data.Category { CategoryID = "baby-boy-one-pieces", Name = "One Piece", OriginUrl = "http://www.carters.com/carters-baby-boy-one-pieces?cgid=carters-baby-boy-one-pieces&startRow=0&sz=all&format=ajax" },
            //            new Minie.Carters.Data.Category { CategoryID = "baby-boy-bodysuits", Name = "Bodysuits", OriginUrl = "http://www.carters.com/carters-baby-boy-bodysuits?cgid=carters-baby-boy-bodysuits&startRow=0&sz=all&format=ajax" },
            //            new Minie.Carters.Data.Category { CategoryID = "baby-boy-tops", Name = "Tops", OriginUrl = "http://www.carters.com/carters-baby-boy-tops?cgid=carters-baby-boy-tops&startRow=0&sz=all&format=ajax" },
            //            new Minie.Carters.Data.Category { CategoryID = "baby-boy-bottoms", Name = "Bottoms", OriginUrl = "http://www.carters.com/carters-baby-boy-bottoms?cgid=carters-baby-boy-bottoms&startRow=0&sz=all&format=ajax" },
            //            new Minie.Carters.Data.Category { CategoryID = "baby-boy-jackets-and-outerwear", Name = "Jackets & Outerwear", OriginUrl = "http://www.carters.com/carters-baby-boy-jackets-and-outerwear?cgid=carters-baby-boy-jackets-and-outerwear&startRow=0&sz=all&format=ajax" },
            //            new Minie.Carters.Data.Category { CategoryID = "baby-boy-pajamas", Name = "Pajamas", OriginUrl = "http://www.carters.com/carters-baby-boy-pajamas?cgid=carters-baby-boy-pajamas&startRow=0&sz=all&format=ajax" },
            //            new Minie.Carters.Data.Category { CategoryID = "baby-boy-swimwear", Name = "Swimwear", OriginUrl = "http://www.carters.com/carters-baby-boy-baby-swimwear?cgid=carters-baby-boy-baby-swimwear&startRow=0&sz=all&format=ajax" },
            //            new Minie.Carters.Data.Category { CategoryID = "baby-boy-shoes", Name = "Shoes", OriginUrl = "http://www.carters.com/carters-baby-baby-boy-accessories-shoes-and-slippers?cgid=carters-baby-bay-boy-accessories-shoes-and-slippers&startRow=0&sz=all&format=ajax" },
            //            new Minie.Carters.Data.Category { CategoryID = "baby-boy-accessories", Name = "Accessories", OriginUrl = "http://www.carters.com/carters-baby-boy-accessories?cgid=carters-baby-boy-accessories&startRow=0&sz=all&format=ajax" },
            //            new Minie.Carters.Data.Category { CategoryID = "baby-boy-burpclothes-and-bibs", Name = "Bibs & Burp Cloths", OriginUrl = "http://www.carters.com/carters-baby-baby-boy-burpclothes-and-bibs?cgid=carters-baby-baby-boy-burpclothes-and-bibs&startRow=0&sz=all&format=ajax" },
            //            new Minie.Carters.Data.Category { CategoryID = "baby-boy-blankets", Name = "Blankets", OriginUrl = "http://www.carters.com/carters-baby-baby-boy-blankets?cgid=carters-baby-baby-boy-blankets&startRow=0&sz=all&format=ajax" },
            //            new Minie.Carters.Data.Category { CategoryID = "baby-boy-toy-and-gift", Name = "Toys & Gifts", OriginUrl = "http://www.carters.com/carters-baby-boy-toy-and-gift?cgid=carters-baby-boy-toy-and-gift&startRow=0&sz=all&format=ajax" },
            //            new Minie.Carters.Data.Category { CategoryID = "baby-boy-bathtime", Name = "Bath Time", OriginUrl = "http://www.carters.com/carters-baby-baby-boy-bathtime?cgid=carters-baby-baby-boy-bathtime&startRow=0&sz=all&format=ajax" },
            //            new Minie.Carters.Data.Category { CategoryID = "baby-boy-diaper-bags", Name = "Diaper Bags", OriginUrl = "http://www.carters.com/carters-baby-boy-toys-and-gifts-diaper-bags?cgid=carters-baby-boy-toys-and-gifts-diaper-bags&startRow=0&sz=all&format=ajax" },
            //            new Minie.Carters.Data.Category { CategoryID = "baby-boy-clearance", Name = "Clearance", OriginUrl = "http://www.carters.com/carters-baby-boy-clearance?cgid=carters-baby-boy-clearance&startRow=0&sz=all&format=ajax" }
            //        })
            //});

            wc.Dispose();
        }
    }
}
