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

using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using Code2Xml.Core;
using Code2Xml.Core.Generators;
using Code2Xml.Languages.ExternalGenerators.Properties;
using Paraiba.IO;

namespace Code2Xml.Languages.ExternalGenerators.Generators.Python {
    /// <summary>
    /// Represents a Python 3.x CstGenerator for inter-converting between source code and XML-based ASTs.
    /// </summary>
    [Export(typeof(CstGenerator))]
    public class Python3CstGenerator : CstGeneratorUsingExternalProgram {
        private static readonly string DirectoryPath =
                Path.Combine(Code2XmlConstants.DependenciesDirectoryName, "Python");

        private static readonly string[] XmlGeneratorArguments = {
            Path.Combine(DirectoryPath, "st2xml.py"),
        };

        /// <summary>
        /// Gets the language name except for the version.
        /// </summary>
        public override string LanguageName {
            get { return "Python"; }
        }

        /// <summary>
        /// Gets the language version.
        /// </summary>
        public override string LanguageVersion {
            get { return "3"; }
        }

        private readonly string _processorPath;

        public Python3CstGenerator()
                : this("\n") {}

        public Python3CstGenerator(string newLine)
                : this(newLine, ExternalProgramUtils.GetPythonPath("3") ?? "python3") {}

        public Python3CstGenerator(string newLine, string processorPath)
                : base(newLine, ".py") {
            _processorPath = processorPath;

            ParaibaFile.WriteIfDifferentSize(XmlGeneratorArguments[0], Resources.st2xml);
        }

        protected override Process StartProcess(string code) {
            code = code.Replace("\r\n", "\n");
            return ExternalProgramUtils.StartProcess(
                    code, _processorPath, XmlGeneratorArguments);
        }
    }
}