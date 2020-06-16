using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareStream.Models
{
    [Table("BulkUserFile")]
    public class BulkUserFile 
    {
       [Key]
        public Int64 Id { get; set; }

        [StringLength(250)]
        public string FileName { get; set; }

        public string FileSize { get; set; }

        [StringLength(150)]
        public string UploadBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(100)]
        public string Action { get; set; }

        [StringLength(100)]
        public string Status { get; set; }

        public string Error { get; set; }
    }
}
