﻿#region License

// Copyright (C) 2009-2012 Kazunori Sakamoto
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

using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using Code2Xml.Core.XmlToCodes;
using Code2Xml.Languages.SrcML.Properties;
using Paraiba.IO;

namespace Code2Xml.Languages.SrcML.XmlToCodes {
	[Export(typeof(XmlToCode))]
	public class SrcMLForCXmlToCode : ExternalXmlToCode {
		private static SrcMLForCXmlToCode _instance;

		private static readonly string DirectoryPath =
				Path.Combine("ParserScripts", "SrcML");

		private static readonly string PrivateProcessorPath =
				Path.Combine(DirectoryPath, "srcml2src.exe");

		private static readonly string[] PrivateArguments = new string[] { };

		public static SrcMLForCXmlToCode Instance {
			get { return _instance ?? (_instance = new SrcMLForCXmlToCode()); }
		}

		public override string ParserName {
			get { return "SrcMLForC"; }
		}

		protected override string ProcessorPath {
			get { return PrivateProcessorPath; }
		}

		protected override string[] Arguments {
			get { return PrivateArguments; }
		}

		public override ReadOnlyCollection<string> SupportedExtensions {
			get { return new ReadOnlyCollection<string>(new[] { ".c", ".h" }); }
		}

		public SrcMLForCXmlToCode() {
			ParaibaFile.WriteIfDifferentSize(PrivateProcessorPath, Resources.srcml2src);
			SrcMLFiles.DeployCommonFiles(DirectoryPath);
		}
	}
}