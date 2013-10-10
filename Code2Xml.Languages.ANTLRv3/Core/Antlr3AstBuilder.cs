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

using System.Linq;
using System.Xml.Linq;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Code2Xml.Core;
using Code2Xml.Core.Processors;

namespace Code2Xml.Languages.ANTLRv3.Core {
	public class Antlr3AstBuilder : CommonTreeAdaptor {
		private readonly XElement _dummyNode;
		private readonly CommonTokenStream _stream;
		private int _nextTokenIndex;
		private XElement _lastElement;

		public Antlr3AstBuilder() {}

		public Antlr3AstBuilder(CommonTokenStream stream) {
			_stream = stream;
			_dummyNode = new XElement("dummy");
			_lastElement = _dummyNode;
			_nextTokenIndex = 0;
		}

		public XElement FinishParsing(XElement root) {
			var size = _stream.Count - 1; // Avoid writing "<EOF>"
			for (int i = _nextTokenIndex; i < size; i++) {
				var oldToken = _stream.Get(i);
				var name = oldToken.Channel != Lexer.Hidden ? "TOKEN" : "HIDDEN";
				_lastElement.Add(CreateTokenElement(name, oldToken));
			}

			var firstTokensNode = root.Descendants("TOKENS").FirstOrDefault() ??
			                      root.Descendants().LastOrDefault();
			if (firstTokensNode != null) {
				foreach (var element in _dummyNode.Elements().Reverse()) {
					firstTokensNode.AddFirst(element);
				}
			}
			return root;
		}

		public void AddChild(
				object t, object child, Antlr3AstNode target, Antlr3AstNode parent) {
			parent.Element.Add(target.Element);
			base.AddChild(t, child);
		}

		public object Create(IToken token, Antlr3AstNode parent) {
			if (token != null) {
				var count = token.TokenIndex;
				for (int i = _nextTokenIndex; i < count; i++) {
					var oldToken = _stream.Get(i);
					var name = oldToken.Channel != Lexer.Hidden ? "TOKEN" : "HIDDEN";
					_lastElement.Add(CreateTokenElement(name, oldToken));
				}
				_nextTokenIndex = count + 1;
				_lastElement = new XElement("TOKENS", CreateTokenElement("TOKEN", token));
				parent.Element.Add(_lastElement);
			}
			return base.Create(token);
		}

		private static XElement CreateTokenElement(string name, IToken token) {
			var text = token.Text;
			var newLineCount = text.Count(ch => ch == '\n');
			var tokenElement = new XElement(name, text);
			tokenElement.SetAttributeValue(
					Code2XmlConstants.StartLineName, token.Line);
			tokenElement.SetAttributeValue(
					Code2XmlConstants.StartPositionName, token.CharPositionInLine);
			tokenElement.SetAttributeValue(
					Code2XmlConstants.EndLineName, token.Line + newLineCount);
			tokenElement.SetAttributeValue(
					Code2XmlConstants.EndPositionName, newLineCount == 0
							? token.CharPositionInLine + text.Length - 1
							: text.Length - (text.LastIndexOf('\n') + 1) - 1);
			return tokenElement;
		}
	}

	public class Antlr3AstBuilderWithReportingError : Antlr3AstBuilder {
		public Antlr3AstBuilderWithReportingError(CommonTokenStream stream) : base(stream) {}

		public override object ErrorNode(
				ITokenStream input, IToken start, IToken stop, RecognitionException e) {
			throw new ParseException(e);
		}
	}
}