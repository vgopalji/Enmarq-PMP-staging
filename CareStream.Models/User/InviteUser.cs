using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareStream.Models
{
    public class InviteUser
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "InvitedUserEmailAddress", Required = Required.Default)]
        public string InvitedUserEmailAddress { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "InviteRedirectUrl", Required = Required.Default)]
        public string InviteRedirectUrl { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "InvitedUserDisplayName", Required = Required.Default)]
        public string InvitedUserDisplayName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "InvitedUserType", Required = Required.Default)]
        public string InvitedUserType { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "SendInvitationMessage", Required = Required.Default)]
        public string SendInvitationMessage { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "InviteRedeemUrl", Required = Required.Default)]
        public string InviteRedeemUrl { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Status", Required = Required.Default)]
        public string Status { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "InvitedUser", Required = Required.Default)]
        public Microsoft.Graph.User InvitedUser { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "InvitedUserMessageInfo", Required = Required.Default)]
        public string MessageInfo { get; set; }
   
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "CustomizedMessageBody", Required = Required.Default)]
        public string CustomizedMessageBody { get; set; }
        
    }
}
