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
                return writer.ToString();
            }

        }
    }
}
