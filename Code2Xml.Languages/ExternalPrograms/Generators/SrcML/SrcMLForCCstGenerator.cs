﻿#region License

// Copyright (C) 2011-2014 Kazunori Sakamoto
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.Diagnostics;
using System.IO;
using Code2Xml.Core;
using Code2Xml.Core.Generators;
using Code2Xml.Languages.ExternalGenerators.Properties;
using Paraiba.IO;

namespace Code2Xml.Languages.ExternalGenerators.Generators.SrcML {
    /// <summary>
    /// Represents a C 99 CstGenerator using SrcML for inter-converting between source code and XML-based ASTs.
    /// </summary>
    // TODO: [Export(typeof(CstGenerator))] // because this class is under experiment
    public class SrcMLForCCstGenerator : AstGeneratorUsingExternalProgram {
        private static readonly string DirectoryPath =
                Path.Combine(Code2XmlConstants.DependenciesDirectoryName, "SrcML");

        private static readonly string PrivateXmlGeneratorPath = Path.Combine(
                DirectoryPath, "src2srcml.exe");

        private static readonly string[] PrivateXmlGeneratorArguments = { "-l", "C" };

        private static readonly string PrivateCodeGeneratorPath = Path.Combine(
                DirectoryPath, "srcml2src.exe");

        private static readonly string[] PrivateCodeGeneratorArguments = { };

        /// <summary>
        /// Gets the language name except for the version.
        /// </summary>
        public override string LanguageName {
            get { return "C"; }
        }

        /// <summary>
        /// Gets the language version.
        /// </summary>
        public override string LanguageVersion {
            get { return "99"; }
        }

        public SrcMLForCCstGenerator()
                : base(".c", ".h") {
            ParaibaFile.WriteIfDifferentSize(PrivateXmlGeneratorPath, Resources.src2srcml);
            ParaibaFile.WriteIfDifferentSize(PrivateCodeGeneratorPath, Resources.srcml2src);
            SrcMLFiles.DeployCommonFiles(DirectoryPath);
        }

        protected override ProcessStartInfo CreateProcessStartInfoForGeneratingXml() {
            return ExternalProgramUtils.CreateProcessStartInfo(
                    PrivateXmlGeneratorPath, PrivateXmlGeneratorArguments);
        }

        protected override ProcessStartInfo CreateProcessStartInfoForGeneratingCode() {
            return ExternalProgramUtils.CreateProcessStartInfo(
                    PrivateCodeGeneratorPath, PrivateCodeGeneratorArguments);
        }

        //protected override string NormalizeXmlText(string xml) {
        //	xml = base.NormalizeXmlText(xml);
        //	xml = Regex.Replace(
        //			xml, @"(xmlns:?[^=]*=[""][^""]*[""])", "",
        //			RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //	return xml;
        //}
    }
}