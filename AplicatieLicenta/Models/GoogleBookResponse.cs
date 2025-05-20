using System.Collections.Generic;

namespace AplicatieLicenta.Models
{
    public class GoogleBooksResponse
    {
        public List<Item> Items { get; set; }

        public class Item
        {
            public VolumeInfo VolumeInfo { get; set; }
        }

        public class VolumeInfo
        {
            public string Title { get; set; }
            public List<string> Authors { get; set; }
            public string Description { get; set; }
        }
    }
}
