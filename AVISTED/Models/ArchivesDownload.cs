using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AVISTED.Models
{
    public class ArchivesDownload
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Enter the File Name")]
        public string FileName { get; set; }
        public DateTime Date { get; set; }
        public string Path { get; set; }
        public Boolean ImgDown { get; set; }
        public string FileType { get; set; }
    }
}
