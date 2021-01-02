using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Helpers;
using YamlDotNet.RepresentationModel;

namespace PipelineCacher.Shared.Extensions
{
    public static class Yaml
    {
        public static YamlNode Value(this YamlNode node, YamlNode key)
        {
            return Value(node.Value<YamlMappingNode>()?.Children, key);
        }
        public static YamlSequenceNode SequenceValue(this YamlNode node, YamlNode key)
        {
            return Value(node.Value<YamlMappingNode>()?.Children, key) as YamlSequenceNode;
        }
        public static YamlScalarNode ScalarValue(this YamlNode node, YamlNode key)
        {
            return Value(node.Value<YamlMappingNode>()?.Children, key) as YamlScalarNode;
        }
        public static T Value<T>(this YamlNode node) where T: class
        {
            return node as T;
        }

        public static YamlNode Value(this IOrderedDictionary<YamlNode, YamlNode> dict, YamlNode key)
        {
            if (dict == null)
            {
                return null;
            }
            dict.TryGetValue(key, out var value);
            return value;
        }
        public static YamlNode Value(this YamlMappingNode mappingNode, YamlNode key)
        {
            return Value(mappingNode.Children, key);
        }
    }
}
