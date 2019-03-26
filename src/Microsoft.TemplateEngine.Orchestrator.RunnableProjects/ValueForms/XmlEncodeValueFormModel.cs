using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.TemplateEngine.Abstractions.Json;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.ValueForms
{
    public class XmlEncodeValueFormModel : IValueForm
    {
        private static readonly XmlWriterSettings Settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };

        public string Identifier => "xmlEncode";

        public string Name { get; }

        public XmlEncodeValueFormModel()
        {
        }

        public XmlEncodeValueFormModel(string name)
        {
            Name = name;
        }

        public IValueForm FromJson(string name, IJsonObject configuration)
        {
            return new XmlEncodeValueFormModel(name);
        }

        public string Process(IReadOnlyDictionary<string, IValueForm> forms, string value)
        {
            StringBuilder output = new StringBuilder();
            using (XmlWriter w = XmlWriter.Create(output, Settings))
            {
                w.WriteString(value);
            }
            return output.ToString();
        }
    }
}
