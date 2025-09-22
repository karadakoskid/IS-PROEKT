using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Domain.Domain_Models
{
    public class GoogleBooksResponse
    {
        public List<GoogleBookItem> Items { get; set; }
    }

    public class GoogleBookItem
    {
        public VolumeInfo VolumeInfo { get; set; }
        public SaleInfo SaleInfo { get; set; }
    }

    public class VolumeInfo
    {
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public string Description { get; set; }
        public ImageLinks ImageLinks { get; set; }
    }

    public class ImageLinks
    {
        public string Thumbnail { get; set; }
    }

    public class SaleInfo
    {
        public ListPrice ListPrice { get; set; }
    }

    public class ListPrice
    {
        public double Amount { get; set; }
    }
}
