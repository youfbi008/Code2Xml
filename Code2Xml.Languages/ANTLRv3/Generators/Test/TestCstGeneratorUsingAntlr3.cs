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
using Antlr.Runtime;
using Code2Xml.Core.Generators;
using Code2Xml.Languages.ANTLRv3.Core;

namespace Code2Xml.Languages.ANTLRv3.Generators.Test {
    /// <summary>
    /// Represents a Test parser and a Test code generator.
    /// </summary>
    [Export(typeof(CstGenerator))]
    public class TestCstGeneratorUsingAntlr3 : CstGeneratorUsingAntlr3<TestParser> {
        /// <summary>
        /// Gets the language name except for the version.
        /// </summary>
        public override string LanguageName {
            get { return "Test"; }
        }

        /// <summary>
        /// Gets the language version.
        /// </summary>
        public override string LanguageVersion {
            get { return "5.3"; }
        }

        public TestCstGeneratorUsingAntlr3() : base(".test") {}

        protected override ITokenSource CreateLexer(ICharStream stream) {
            return new TestLexer(stream);
        }

        protected override TestParser CreateParser(ITokenStream stream) {
            return new TestParser(stream);
        }

        protected override Antlr3CstNode Parse(TestParser parser) {
            return parser.compilation_unit();
        }
    }
}