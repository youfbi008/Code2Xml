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

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using Code2Xml.Core.Generators;
using Code2Xml.Core.Tests.Generators;
using Code2Xml.Languages.ANTLRv4.Core;
using Code2Xml.Languages.ANTLRv4.Generators.Java;
using NUnit.Framework;

namespace Code2Xml.Languages.ANTLRv4.Tests.Generators {
    [TestFixture]
    public class JavaCstGeneratorTest : CstGeneratorTest {
        protected override CstGenerator CreateGenerator() {
            return new JavaCstGenerator();
        }

        [Test]
        [TestCase(@"//test
import javax.swing.*;
 
public class Hello extends JFrame {
    Hello() /*test*/ {
        setDefaultCloseOperation(WindowConstants.DISPOSE_ON_CLOSE);
        pack(); // pack();
    }
 
    public static void main(String[] args) {
        new Hello().setVisible(true);
    }
}")]
        [TestCase(@"class Main {
  void test() { obj.method().<Object>method2(); }
}")]
        [TestCase(@"public class AlignedTuplePrinter {
    List<String> columnLines = new ArrayList<>();
}")]
        [TestCase(@"class Klass { void main() {
	try { } finally { }
}}")]
        [TestCase(@"class Klass { void main() {
	try (Object obj = new Object()) { } finally { }
}}")]
        [TestCase(@"class Klass { void main() {
	try (Object obj = new Object() ; Object obj = new Object()) { } finally { }
}}")]
        [TestCase(@"class Klass { void main() {
		checkArgument(args == null);
		Preconditions.checkArgument(args != null);
		com.google.common.base.Preconditions.checkArgument(args.length == 0, str);
}}")]
        [TestCase(@"class K { void m() { if (true) {} for(;false;) {} }}")]
        [TestCase(@"
@Retention(RetentionPolicy.CLASS)
@Target({
    ElementType.ANNOTATION_TYPE,
    ElementType.CONSTRUCTOR,
    ElementType.FIELD,
    ElementType.METHOD,
    ElementType.TYPE})
@Documented
@GwtCompatible
public @interface Beta {}")]
        [TestCase(@"
public class AlignedTuplePrinter {
    List<String> columnLines = new ArrayList<>();
}")]
        public void Parse(string code) {
            VerifyRestoringCode(code);
        }

        [Test]
        public void CheckIds() {
            var nodes = Generator.GenerateTreeFromCodeText("class K { void m() { if (true) stmt(); else stmt(); } }")
                    .Descendants("statement")
                    .ToList();
            Assert.That(nodes[0].RuleId, Is.Not.EqualTo(nodes[1].RuleId));
            Assert.That(nodes[0].RuleId, Is.Not.EqualTo(nodes[2].RuleId));
            Assert.That(nodes[1].RuleId, Is.Not.EqualTo(nodes[2].RuleId));
        }

        [Test]
        public void ParseComments() {
            var code = @"//test
import javax.swing.*;
 
public class Hello extends JFrame {
    Hello() /*test*/ {
        setDefaultCloseOperation(WindowConstants.DISPOSE_ON_CLOSE);
        pack(); // pack();
    }
 
    public static void main(String[] args) {
        new Hello().setVisible(true);
    }
}
";
            var cst = TestParsing(code);
            var nComments = cst.AllTokensWithHiddens()
                    .Count(e => e.Name == "COMMENT" || e.Name == "LINE_COMMENT");
            Assert.That(nComments, Is.EqualTo(3));
        }

        [Test]
        public void ParseHexicalNumber() {
            var code = @"
class Hello {
    void main(String[] args) {
		System.out.println((String)args[0x00]);
    }
}
";
            TestParsing(code);
        }

        [Test]
        public void ParseGenericMethod() {
            var code = @"
class Main {
  void test() { obj.method().<Object>method2(); }
}";

            TestParsing(code);
        }

        [Test]
        public void ParseBrokenCodeIgnoringException() {
            var code = @"class A {{ }";
            var processor = new JavaCstGenerator();
            processor.GenerateTreeFromCodeText(code, false);
        }

        [Test, ExpectedException(typeof(ParseException))]
        public void ParseBrokenCode() {
            var code = @"class A {{ }";
            var processor = new JavaCstGenerator();
            processor.GenerateTreeFromCodeText(code, true);
        }

        [Test]
        public void ParseJavaSource() {
            var dirInfo = new DirectoryInfo(@"C:\Users\exKAZUu\Desktop\src");
            if (!dirInfo.Exists) {
                return;
            }

            var javaFiles = dirInfo.GetFiles("*.java", SearchOption.AllDirectories);
            var processor = new JavaCstGenerator();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (var javaFile in javaFiles) {
                Console.WriteLine(javaFile);
                var code = javaFile.OpenText().ReadToEnd();
                var cst = processor.GenerateTreeFromCodeText(code);
                var code2 = cst.Code;
                Assert.That(code2, Is.EqualTo(code));
            }
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
        }

        [Test]
        public void Test() {
            var inputStream = new AntlrInputStream("public /*aa*/ class Klass { }");
            var javaLexer = new JavaLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(javaLexer);
            var javaParser = new JavaParser(commonTokenStream);
            var context = javaParser.compilationUnit();
            var visitor = new CstBuilderForAntlr4(javaParser);
            visitor.Visit(context);
            Console.WriteLine(visitor.FinishParsing().ToXml());
        }

        private static CstNode TestParsing(string code) {
            var processor = new JavaCstGenerator();
            var cst = processor.GenerateTreeFromCodeText(code, true);
            var code2 = cst.Code;
            Assert.That(code2, Is.EqualTo(code));
            Console.WriteLine(cst);
            return cst;
        }
    }
}