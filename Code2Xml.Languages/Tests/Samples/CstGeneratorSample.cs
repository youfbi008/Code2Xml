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

using System.IO;
using Code2Xml.Core.Generators;
using NUnit.Framework;
using ParserTests;

namespace Code2Xml.Languages.Tests.Samples {
    /// <summary>
    /// Sample code for generating CST, XML and code.
    /// </summary>
    [TestFixture]
    public class CstGeneratorSample {
        [Test]
        public void ParseJavaText() {
            const string originalCode = @"class Klass {}";
            var gen = CstGenerators.JavaUsingAntlr3;

            /*** Code <=> CST ***/
            var cst = gen.GenerateTreeFromCodeText(originalCode);
            Assert.That(cst.Code, Is.EqualTo(originalCode));
            // GenerateCodeFromTree() invokes Code
            Assert.That(gen.GenerateCodeFromTree(cst), Is.EqualTo(originalCode));

            /*** CST <=> XML ***/
            var xml = cst.ToXml();
            Assert.That(CstNode.FromXml(xml).Code, Is.EqualTo(originalCode));
            // GenerateXmlFromTree() invokes ToXml()
            Assert.That(gen.GenerateXmlFromTree(cst).ToString(),
                Is.EqualTo(xml.ToString()));
            // GenerateTreeFromXml() invokes CstNode.FromXml()
            Assert.That(gen.GenerateTreeFromXml(xml).Code,
                Is.EqualTo(originalCode));

            /*** Code <=> XML (via CST) ***/
            // GenerateXmlFromCodeText() invokes GenerateTreeFromCodeText() and ToXml()
            Assert.That(gen.GenerateXmlFromCodeText(originalCode).ToString(),
                Is.EqualTo(xml.ToString()));
            // GenerateCodeFromXml() invokes CstNode.FromXml() and Code
            Assert.That(gen.GenerateCodeFromXml(xml),
                Is.EqualTo(originalCode));
        }

        [Test]
        public void ParseCSharpFile() {
            var path = Fixture.GetInputCodePath("CSharp", "Student.cs");
            // To read file, please pass a FileInfo instance
            var cst = CstGenerators.CSharpUsingAntlr3.GenerateTreeFromCode(new FileInfo(path));
            Assert.That(cst.Code, Is.EqualTo(File.ReadAllText(path)));
        }

        [Test]
        public void ParseLuaFileUsingFilePath() {
            var path = Fixture.GetInputCodePath("Lua", "Block1.lua");
            // To read file, please pass a FileInfo instance
            var processor = CstGenerators.GetProcessorByPath(path);
            var cst = processor.GenerateTreeFromCode(new FileInfo(path));
            Assert.That(cst.Code, Is.EqualTo(File.ReadAllText(path)));
        }
    }
}