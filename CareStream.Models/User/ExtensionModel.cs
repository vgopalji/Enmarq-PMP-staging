using System.Collections.Generic;
using Newtonsoft.Json;


namespace CareStream.Models
{
    public class ExtensionModel
    {
        [JsonProperty(PropertyName = "targetObjects", NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public List<string> TargetObjects { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name", Required = Required.Default)]
        public string  Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dataType", Required = Required.Default)]
        public string DataType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "description", Required = Required.Default)]
        public string Description { get; set; }
    }
   
}
