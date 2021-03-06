#region License

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
using System.IO;
using System.Threading;
using Code2Xml.Core.Generators;
using NUnit.Framework;
using ParserTests;

namespace Code2Xml.Core.Tests.Generators {
    public abstract class SyntaxTreeGeneratorTest<TGenerator, TNode, TToken>
            where TGenerator : SyntaxTreeGenerator<TNode, TToken>
            where TNode : SyntaxTreeNode<TNode, TToken>
            where TToken : SyntaxTreeToken<TToken> {
        private long _codeLength;
        private long _timeToGenerateTree;
        private long _timeToGenerateCode;
        private TGenerator _generator;

        protected TGenerator Generator {
            get { return _generator ?? (_generator = CreateGenerator()); }
        }

        protected abstract TGenerator CreateGenerator();

        protected void StartMeasuringTimes() {
            _codeLength = 0;
            _timeToGenerateTree = 0;
            _timeToGenerateCode = 0;
        }

        protected void ShowTimes(string path, string url = null) {
            var name = Path.GetFileName(path);
            var total = _timeToGenerateTree + _timeToGenerateCode;
            Console.WriteLine(
                    "Tree: " + _timeToGenerateTree + ", Code: " + _timeToGenerateCode + ", Total: "
                    + total);
            var rootPath = Fixture.GetRootPath("Code2Xml.sln");
            var info = new FileInfo(Path.Combine(rootPath, "Performance.md.latest"));
            var exists = info.Exists;
            using (var fs = info.Open(FileMode.Append, FileAccess.Write)) {
                using (var stream = new StreamWriter(fs)) {
                    if (!exists) {
                        stream.WriteLine("| Project | Size | Tree | Code | Total |");
                        stream.WriteLine("| --- | ---: | ---: | ---: | ---: |");
                    }
                    stream.Write("| ");
                    if (string.IsNullOrEmpty(url)) {
                        stream.Write(name);
                    } else {
                        stream.Write("[" + name + "](" + url + ")");
                    }
                    stream.Write(" | ");
                    stream.Write(_codeLength.ToString("N0"));
                    stream.Write(" | ");
                    stream.Write(_timeToGenerateTree.ToString("N0"));
                    stream.Write(" | ");
                    stream.Write(_timeToGenerateCode.ToString("N0"));
                    stream.Write(" | ");
                    stream.Write(total.ToString("N0"));
                    stream.WriteLine(" |");
                }
            }
        }

        protected void VerifyParsing(string code) {
            var time = Environment.TickCount;
            var root = Generator.GenerateTreeFromCodeText(code);
            _timeToGenerateTree += (Environment.TickCount - time);

            Console.WriteLine(root.ToXml());
        }

        protected void VerifyInterConverting(string code) {
            var time = Environment.TickCount;
            var r1 = Generator.GenerateTreeFromCodeText(code, true);
            var time2 = Environment.TickCount;
            var c1 = Generator.GenerateCodeFromTree(r1);
            _timeToGenerateCode += (Environment.TickCount - time2);
            _timeToGenerateTree += (time2 - time);
            _codeLength += code.Length;

            var r2 = Generator.GenerateTreeFromCodeText(c1, true);
            var c2 = Generator.GenerateCodeFromTree(r2);
            var r3 = Generator.GenerateTreeFromCodeText(c2, true);
            var c3 = Generator.GenerateCodeFromTree(r3);

            Assert.That(r2, Is.EqualTo(r3));
            Assert.That(c2, Is.EqualTo(c3));
        }

        protected void VerifyRestoringCodeAndInspect(string code, bool write = true) {
            try {
                VerifyRestoringCode(code, write);
            } catch (Exception e) {
                Inspect(code);
                throw e;
            }
        }

        protected void VerifyRestoringCode(string code, bool write = true) {
            code = code.Replace("\r\n", "\n");

            var time = Environment.TickCount;
            var tree = Generator.GenerateTreeFromCodeText(code, true);
            var time2 = Environment.TickCount;
            var code2 = Generator.GenerateCodeFromTree(tree);
            _timeToGenerateCode += (Environment.TickCount - time2);
            _timeToGenerateTree += (time2 - time);

            Assert.That(code2, Is.EqualTo(code));
            if (write) {
                Console.WriteLine(tree.ToXml());
            }
            VerifyLocation(code, tree);
        }

        protected abstract void VerifyLocation(string code, TNode root);

        protected void VerifyRestoringFile(string langName, string fileName) {
            var path = Fixture.GetInputCodePath(langName, fileName);
            VerifyRestoringCode(File.ReadAllText(path));
        }

        protected void VerifyRestoringProjectDirectory(
                string langName, string directoryName,
                params string[] patterns) {
            PrivateVerifyRestoringProjectDirectory(
                    langName, directoryName, File.ReadAllText, patterns);
        }

        protected void VerifyRestoringProjectDirectory(
                string langName, string directoryName,
                Func<string, string> readFileFunc, params string[] patterns) {
            var path = Fixture.GetInputProjectPath(langName, directoryName);
            PrivateVerifyRestoringProjectDirectory(path, null, readFileFunc, patterns);
        }

        private void PrivateVerifyRestoringProjectDirectory(
                string path, string url, Func<string, string> readFileFunc,
                params string[] patterns) {
            StartMeasuringTimes();
            foreach (var pattern in patterns) {
                var filePaths = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
                foreach (var filePath in filePaths) {
                    Console.Write(".");
                    try {
                        VerifyRestoringCode(readFileFunc(filePath), false);
                    } catch (Exception e) {
                        Console.WriteLine();
                        Console.WriteLine(filePath);
                        throw new ParseException(e);
                    }
                }
            }
            Console.WriteLine();
            ShowTimes(path, url);
        }

        protected void VerifyRestoringGitRepository(
                string url, string commitPointer,
                params string[] patterns) {
            VerifyRestoringGitRepository(url, commitPointer, File.ReadAllText, patterns);
        }

        protected void VerifyRestoringGitRepository(
                string url, string commitPointer,
                Func<string, string> readFileFunc, params string[] patterns) {
            var path = Fixture.GetGitRepositoryPath(url);
            Directory.CreateDirectory(path);
            Git.CloneAndCheckout(path, url, commitPointer);
            PrivateVerifyRestoringProjectDirectory(path, url, readFileFunc, patterns);
        }

        protected void VerifyRestoringGitRepositorySavingRepo(
                string url, string commitPointer, string filePath,
                params string[] patterns) {
            VerifyRestoringGitRepositorySavingRepo(
                    url, commitPointer, filePath, File.ReadAllText, patterns);
        }

        protected void VerifyRestoringGitRepositorySavingRepo(
                string url, string commitPointer, string filePath,
                Func<string, string> readFileFunc, params string[] patterns) {
            var thread = new Thread(
                    () => {
                        var path = Fixture.GetGitRepositoryPath(url);
                        Directory.CreateDirectory(path);
                        Git.CloneAndCheckout(path, url, commitPointer);
                        PrivateVerifyRestoringProjectDirectory(path, url, readFileFunc, patterns);
                        File.AppendAllText(
                                filePath,
                                "[TestCase(@\"" + url + "\",\r\n" +
                                "@\"" + commitPointer + "\")]\r\n");
                    });
            thread.Start();
            if (thread.Join(1000 * 60 * 5)) {
                thread.Abort();
            }
        }

        protected void Inspect(string code) {
            code = code.Replace("\r\n", "\n");
            var lines = code.Split('\n');
            var length = code.Length;
            for (int i = lines.Length - 1; i >= 0; i--) {
                length -= lines[i].Length;
                try {
                    var code2 = code.Substring(0, length);
                    var tree = Generator.GenerateTreeFromCodeText(code2);
                    var code3 = Generator.GenerateCodeFromTree(tree);
                    if (code2 == code3) {
                        Console.WriteLine();
                        Console.WriteLine("Max parsable line: " + i);
                        return;
                    }
                } catch {}
            }
        }
    }
}