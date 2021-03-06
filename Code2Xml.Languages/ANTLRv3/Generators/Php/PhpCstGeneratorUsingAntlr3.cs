﻿#region License

// Copyright (C) 2011-2013 Kazunori Sakamoto
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
using Antlr.Runtime;
using Code2Xml.Core.Generators;
using Code2Xml.Languages.ANTLRv3.Core;
using Code2Xml.Languages.ANTLRv3.Processors.Php;

namespace Code2Xml.Languages.ANTLRv3.Generators.Php {
    /// <summary>
    /// Represents a Php parser and a Php code generator.
    /// </summary>
    [Export(typeof(CstGenerator))]
    public class PhpCstGeneratorUsingAntlr3 : CstGeneratorUsingAntlr3<PhpParser> {
        /// <summary>
        /// Gets the language name except for the version.
        /// </summary>
        public override string LanguageName {
            get { return "Php"; }
        }

        /// <summary>
        /// Gets the language version.
        /// </summary>
        public override string LanguageVersion {
            get { return "5.3"; }
        }

        public PhpCstGeneratorUsingAntlr3() : base(".php") {}

        protected override ITokenSource CreateLexer(ICharStream stream) {
            return new PhpLexer(stream);
        }

        protected override PhpParser CreateParser(ITokenStream stream) {
            return new PhpParser(stream);
        }

        protected override Antlr3CstNode Parse(PhpParser parser) {
            return parser.prog();
        }
    }
}