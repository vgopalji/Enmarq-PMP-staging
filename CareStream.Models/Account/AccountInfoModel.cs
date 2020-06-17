using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareStream.Models
{
    public class AccountInfoModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name", Required = Required.Default)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "account", Required = Required.Default)]
        public string Account { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "roles", Required = Required.Default)]
        public string Roles { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "photo", Required = Required.Default)]
        public string Photo { get; set; }
    }
}
