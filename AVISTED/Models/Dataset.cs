using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AVISTED.Models
{
    public class Dataset
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Dataset Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Format")]
        public string Format { get; set; }
        public string Author { get; set; }
        public string Size { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime UploadDate { get; set; }
        public string Parameters { get; set; }
        public string Status { get; set; }
        public string EmailId { get; set; }
    }
}
