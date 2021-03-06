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
using Code2Xml.Languages.ANTLRv3.Processors.CSharp;

namespace Code2Xml.Languages.ANTLRv3.Generators.CSharp {
    /// <summary>
    /// Represents a CSharp parser and a CSharp code generator.
    /// </summary>
    [Export(typeof(CstGenerator))]
    public class CSharpCstGeneratorUsingAntlr3 : CstGeneratorUsingAntlr3<csParser> {
        /// <summary>
        /// Gets the language name except for the version.
        /// </summary>
        public override string LanguageName {
            get { return "CSharp"; }
        }

        /// <summary>
        /// Gets the language version.
        /// </summary>
        public override string LanguageVersion {
            get { return "4"; }
        }

        public CSharpCstGeneratorUsingAntlr3() : base(".cs") {}

        protected override ITokenSource CreateLexer(ICharStream stream) {
            return new csLexerWithPreProcessor(stream);
        }

        protected override csParser CreateParser(ITokenStream stream) {
            return new csParser(stream);
        }

        protected override Antlr3CstNode Parse(csParser parser) {
            return parser.compilation_unit();
        }
    }
}