using System.ComponentModel.Composition;
using System.IO;
using System.Xml.Linq;
using Code2Xml.Core;
using Code2Xml.Core.Processors;
using Code2Xml.Languages.ExternalProcessors.Properties;
using Paraiba.IO;
using ParserTests;

namespace Code2Xml.Languages.ExternalProcessors.Processors.Cobol
{
    /// <summary>
    /// Represents a Cobol-85 processor for inter-converting between source code and XML-based ASTs.
    /// </summary>
    [Export(typeof(Processor))]
    public class Cobol85Processor : ProcessorUsingExternalProcessor
    {
        private static readonly string DirectoryPath = Path.Combine("ParserScripts", "Cobol");

        private static readonly string[] PrivateXmlGeneratorArguments = {
             "-cp",
             Path.Combine(DirectoryPath, "cobol852xml.jar"),
             "koopa.app.cli.ToXml"
        };//java -cp koopa.jar koopa.app.cli.ToXml [--free-format] <cobol-file> <xml-file>

        private static readonly string[] PrivateCodeGeneratorArguments = {
         //   Path.Combine(DirectoryPath, ""),
        };

        /// <summary>
        /// Gets the language name except for the version.
        /// </summary>
        public override string LanguageName
        {
            get { return "Cobol"; }
        }

        /// <summary>
        /// Gets the language version.
        /// </summary>
        public override string LanguageVersion
        {
            get { return "85"; }
        }

        protected override string XmlGeneratorPath
        {
            get { return _processorPath; }
        }

        protected override string[] XmlGeneratorArguments
        {
            get { return PrivateXmlGeneratorArguments; }
        }

        protected override string CodeGeneratorPath
        {
            get { return _processorPath; }
        }

        protected override string[] CodeGeneratorArguments
        {
            get { return PrivateCodeGeneratorArguments; }
        }

        private readonly string _processorPath;

        public Cobol85Processor()
            : this("java") { }

        public Cobol85Processor(string processorPath)
            : base(".CBL")
        {
            _processorPath = processorPath;

            ParaibaFile.WriteIfDifferentSize(PrivateXmlGeneratorArguments[1], Resources.cobol852xml);
        }
    }
}