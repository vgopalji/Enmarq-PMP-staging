using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareStream.Models
{
    [Table("BulkUser")]
    public class BulkUser
    {
       [Key]
        public Int64 Id { get; set; }

        [ForeignKey("FK_BulkUserFile_BulkUser")]
        public Int64 FileId { get; set; }

        [StringLength(100)]
        public string Action { get; set; }

        [StringLength(100)]
        public string Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        [StringLength(250)]
        public string ObjectID { get; set; }

        [StringLength(250)]
        public string UserPrincipalName { get; set; }

        [StringLength(250)]
        public string DisplayName { get; set; }

        public bool BlockSignIn { get; set; }

        [StringLength(500)]
        public string InitialPassword { get; set; }

        [StringLength(250)]
        public string FirstName { get; set; }

        [StringLength(250)]
        public string LastName { get; set; }

        [StringLength(500)]
        public string JobTitle { get; set; }

        [StringLength(250)]
        public string Department { get; set; }

        [StringLength(500)]
        public string Usagelocation { get; set; }

        [StringLength(500)]
        public string StreetAddress { get; set; }

        [StringLength(100)]
        public string State { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        [StringLength(500)]
        public string Office { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string ZIP { get; set; }

        [StringLength(100)]
        public string OfficePhone { get; set; }

        [StringLength(100)]
        public string MobilePhone { get; set; }

        [StringLength(250)]
        public string InviteeEmail { get; set; }

        [StringLength(500)]
        public string InviteRedirectURL { get; set; }

        public bool SendEmail { get; set; }

        public string CustomizedMessageBody { get; set; }

        public string Error { get; set; }
    }
}
