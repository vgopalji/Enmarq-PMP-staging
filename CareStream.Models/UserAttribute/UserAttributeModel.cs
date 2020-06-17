using System;
using System.Collections.Generic;
using System.Text;

namespace CareStream.Models
{
    public class UserAttributeModel
    {
        public string Id { get; set; }
        public List<ExtensionSchema> ExtensionSchemas { get; set; }

    }

    public class ExtensionSchema
    {
        public string Name { get; set; }

        public string DataType { get; set; }
    }
}
