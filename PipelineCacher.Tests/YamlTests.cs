using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PipelineCacher.Shared.Extensions;
using System;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace PipelineCacher.Tests
{
    public class YamlTests
    {
        private readonly ITestOutputHelper output;

        public YamlTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public void TestNoStages()
        {            
            var str = File.ReadAllText(@"Yaml\nostages.yaml");
            ProcessStages(str);
        }

        [Fact]
        public void TestNoStagesWithYaml()
        {
            var str = File.ReadAllText(@"Yaml\nostages.yaml");
            ProcessStagesWithYaml(str);
        }

        void ProcessStages(string s, Action<JToken> callback = null)
        {
            var deserializer = new DeserializerBuilder()
               //.JsonCompatible()
               //.WithNamingConvention(CamelCaseNamingConvention.Instance)
               .Build();
            var stringReader = new StringReader(s);
            var yamlpipeline = deserializer.Deserialize(stringReader);
            var jobject = JObject.FromObject(yamlpipeline);
            var stages = jobject["stages"]?.Value<JArray>();
            if (stages != null)
            {
                output.WriteLine($"Number of stages found: {stages.Count}");
                foreach (var stage in stages)
                {
                    if (callback != null)
                    {
                        callback(stage);
                    }
                    output.WriteLine($"{stage["stage"]}, {stage["displayName"]}");
                }
            }
            else
            {
                output.WriteLine("No stages found");
            }
        }

        [Fact]
        public void TestStages()
        {
            var str = File.ReadAllText(@"Yaml\stages.yaml");
            ProcessStages(str, (jtoken) =>
            {
                var str = jtoken.ToString();
                output.WriteLine(str);
            });
        }

        [Fact]
        public void TestStagesWithYaml()
        {
            var str = File.ReadAllText(@"Yaml\stages.yaml");
            ProcessStagesWithYaml(str);
        }
       
        private void ProcessStagesWithYaml(string str)
        {
            var sr = new StringReader(str);
            var stream = new YamlStream();
            stream.Load(sr);
            var rootNode = stream.Documents[0].RootNode;
            if (rootNode.NodeType == YamlNodeType.Mapping)
            {
                var stages = rootNode.SequenceValue("stages");
                if (stages != null)
                {
                    foreach (var stage in stages)
                    {
                        output.WriteLine($"{stage.ScalarValue("stage")?.Value}, {stage.ScalarValue("displayName")?.Value}");
                        output.WriteLine($"test = {stage.ScalarValue("test")?.Value}");
                        output.WriteLine($"test2 = {stage.ScalarValue("test2")?.Value}");
                        output.WriteLine($"test3 = {stage.ScalarValue("test3")?.Value}");
                        output.WriteLine(PipelineCacher.Shared.Utilities.Yaml.YamlNodeToString(stage));
                    }
                }
                else
                {
                    output.WriteLine("No stages found");
                }
            }
        }

        private static string Update(string json, object update)
        {
            var updateObj = JObject.Parse(JsonConvert.SerializeObject(update));

            var result = new StringWriter();
            var writer = new JsonTextWriter(result);
            writer.Formatting = Formatting.Indented;

            var reader = new JsonTextReader(new StringReader(json));
            while (reader.Read())
            {

                if (reader.Value == null)
                {
                    writer.WriteToken(reader.TokenType);
                    continue;
                }

                var token =
                   reader.TokenType == JsonToken.Comment ||
                   reader.TokenType == JsonToken.PropertyName ||
                   string.IsNullOrEmpty(reader.Path)
                   ? null
                   : updateObj.SelectToken(reader.Path);

                if (token == null)
                    writer.WriteToken(reader.TokenType, reader.Value);
                else
                    writer.WriteToken(reader.TokenType, token.ToObject(reader.ValueType));
            }

            return result.ToString();
        }

        [Fact]
        void TestJsonUpdate()
        {
            string json = @"{
   //broken
   'CPU': 'Intel',
   'PSU': '500W',
   'Drives': [
     'DVD read/writer'
     /*broken*/,
     '500 gigabyte hard drive',
     '200 gigabype hard drive'
   ]
}";

            var update = Update(json, new { CPU = "AMD", Drives = new[] { "120 gigabytes ssd" } });
            output.WriteLine(update);
        }

        [Fact]
        public void TestYamlDocument()
        {
            var str = File.ReadAllText(@"Yaml\stages.yaml");
            var stream = new YamlStream();
            stream.Load(new StringReader(str));
        }

        [Fact]
        public void TestStreamSaver()
        {
            var mappingNode = new YamlMappingNode();
            var yaml = new YamlDocument(mappingNode);            
            mappingNode.Add("one", "other");
            var yamlStream = new YamlStream(yaml);
            var buffer = new StringBuilder();
            using (var writer = new StringWriter(buffer))
            {
                yamlStream.Save(writer, assignAnchors: false);
                output.WriteLine(writer.ToString());
            }
        }
    }   

    
}
