using System;

namespace DocComAPI.Models
{
    public class comment
    {

        public Guid id { get; set; }

        public Guid poster { get; set; }

        public Guid subject { get; set; }

        public string content { get; set; }

        public int commentStatus { get; set; }

        public DateTime date { get; set; }


    }
}
