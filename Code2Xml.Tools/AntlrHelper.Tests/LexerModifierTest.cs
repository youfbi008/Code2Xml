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

using NUnit.Framework;

namespace Code2Xml.Tools.AntlrHelper.Tests {
	/// <summary>
	///   A test suite for <see cref="LexerModifier" /> .
	/// </summary>
	public class LexerModifierTest {
		[Test]
		public void ConvertJavaIntoCSharp() {
			const string code =
					"       skip();";
			const string expected =
					"       Skip();";
			Assert.That(
					LexerModifier.ModifyFromJavaToCSharp(code),
					Is.EqualTo(expected));
		}
	}
}