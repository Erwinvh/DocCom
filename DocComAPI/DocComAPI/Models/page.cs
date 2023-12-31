﻿namespace DocComAPI.Models
{
    public class page
    {

        public Guid id { get; set; }

        public Guid author { get; set; }

        public string authorUsername { get; set; }

        public string title { get; set; }

        public DateTime date { get; set; }

        public string documentType { get; set; }

        public int documentStatus { get; set; }

        public string documentLocation { get; set; }

        public int documentSecurityLevel { get; set; }

        public List<commentView> comments { get; set; }

    }
}
