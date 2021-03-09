using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineCacher.Server.Utilities
{
    public class Serializers
    {
        /// <summary>
        /// Explicit serializer from object to json string.
        /// Required because the build-in serializer does not access properties in inherited classes
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        static public string ObjectToJson(object o)
        {
            var json = JsonConvert.SerializeObject(
                o,
                Newtonsoft.Json.Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return json;
        }

    }
}
