using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CareStream.Models
{
    public class GroupModel
    {
        public GroupModel()
        {
            GroupOwnerAssign = new GroupAssignModel();
            GroupMemberAssign = new GroupAssignModel();
            OwnerSelected = new List<string>();
            MemberSelected = new List<string>();

            GroupTypes = new List<string>
            {
                CareStreamConst.Security,
                CareStreamConst.O365,
            };
            NoOfMembers = 0;
            NoOfOwners = 0;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "createdDateTime", Required = Required.Default)]
        public DateTimeOffset? CreatedDateTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "groupTypes", Required = Required.Default)]
        public IEnumerable<string> GroupTypes { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "groupType", Required = Required.Default)]
        public string GroupType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "description", Required = Required.Default)]
        [MaxLength(500, ErrorMessage = "Only 500 chars allowed")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Please enter valid Description")]
        public string Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "displayName", Required = Required.Default)]
        [Required(ErrorMessage = "Please enter Group Name")]
        [MaxLength(100, ErrorMessage = "Only 100 chars allowed")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please enter valid Group Name")]
        public string DisplayName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mailEnabled", Required = Required.Default)]
        public bool? MailEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mailNickname", Required = Required.Default)]
        public string MailNickname { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "securityEnabled", Required = Required.Default)]
        public bool? SecurityEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "visibility", Required = Required.Default)]
        public string Visibility { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "objectid", Required = Required.Default)]
        public string ObjectId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "allowExternalSenders", Required = Required.Default)]
        public bool? AllowExternalSenders { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "autoSubscribeNewMembers", Required = Required.Default)]
        public bool? AutoSubscribeNewMembers { get; set; }
        //owner and members
        [JsonProperty(PropertyName = "additionaldata", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> AdditionalData { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "noofowners", Required = Required.Default)]
        public int NoOfOwners { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "noofmembers", Required = Required.Default)]
        public int NoOfMembers { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "source", Required = Required.Default)]
        public string Source { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "groupownerassign", Required = Required.Default)]
        public GroupAssignModel GroupOwnerAssign { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "groupmemberassign", Required = Required.Default)]
        public GroupAssignModel GroupMemberAssign { get; set; }

        [JsonProperty(PropertyName = "ownerselected", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> OwnerSelected { get; set; }

        [JsonProperty(PropertyName = "memberselected", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> MemberSelected { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class GroupAssignModel
    {
        public GroupAssignModel()
        {
            AssignList = new Dictionary<string, string>();
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "assignfor", Required = Required.Default)]
        public string AssignFor { get; set; }

        [JsonProperty(PropertyName = "assignlist", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> AssignList { get; set; }


    }

   
}
