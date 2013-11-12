#region License

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

using System;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Code2Xml.Core.Processors;

namespace Code2Xml.Core.XmlToCodes {
	/// <summary>
	/// Please use <see cref="Processor"/> class.
	/// </summary>
	[Obsolete]
	public class DefaultXmlToCode : XmlToCodeBase {
		private static DefaultXmlToCode _instance;

		public static DefaultXmlToCode Instance {
			get { return _instance ?? (_instance = new DefaultXmlToCode()); }
		}

		public override string ParserName {
			get { throw new NotImplementedException(); }
		}

		public override ReadOnlyCollection<string> SupportedExtensions {
			get { throw new NotImplementedException(); }
		}

		protected override bool TreatTerminalSymbol(XElement element) {
			return false;
		}
	}
}