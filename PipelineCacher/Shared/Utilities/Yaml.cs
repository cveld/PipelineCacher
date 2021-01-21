using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace PipelineCacher.Shared.Utilities
{
    public class Yaml
    {
        public static string YamlNodeToString(YamlNode node)
        {
            var stream = new YamlStream(new YamlDocument(node));
            var buffer = new StringBuilder();
            using (var writer = new StringWriter(buffer))
            {
                stream.Save(writer, assignAnchors: false);
                var s = writer.ToString();
                // workaround for a limitation in the yaml.net library. it unnecessarily emits an expicit document marker "...\r\n"
                // which we try to detect and remove
                if (s.Substring(s.Length - 5) == "...\r\n")
                {
                    s = s.Substring(0, s.Length - 5);
                }
                return s;
            }
        }
    }
}
