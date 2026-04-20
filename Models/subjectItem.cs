using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class subjectItem
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public required string[] Text { get; set; }
        public required string Image { get; set; }
    }
}
