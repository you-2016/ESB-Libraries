﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectivesDictionary.cs" company="Solidsoft Reply Ltd.">
//   Copyright 2015 Solidsoft Reply Limited.
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.   @ ⼩⯹Ԁ耀//   Copyright 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SolidsoftReply.Esb.Libraries.Facts.Dictionaries
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Xml Serialisable dictionary for directives.   Inherits from the serialisable
    /// generic dictionary.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here."),]
    [XmlSchemaProvider("GetDictionarySchema")]
    [XmlRoot("DirectivesDictionary", Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05", IsNullable = true)]
    [Serializable]
    public class DirectivesDictionary : DictionaryBase<Directive>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectivesDictionary"/> class.
        /// </summary>
        public DirectivesDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectivesDictionary"/> class.
        /// </summary>
        /// <param name="capacity">
        /// The capacity.
        /// </param>
        public DirectivesDictionary(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectivesDictionary"/> class.
        /// </summary>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public DirectivesDictionary(IEqualityComparer<string> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectivesDictionary"/> class.
        /// </summary>
        /// <param name="capacity">
        /// The capacity.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public DirectivesDictionary(int capacity, IEqualityComparer<string> comparer)
            : base(capacity, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectivesDictionary"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public DirectivesDictionary(IDictionary<string, Directive> dictionary)
            : base(dictionary)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectivesDictionary"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public DirectivesDictionary(IDictionary<string, Directive> dictionary, IEqualityComparer<string> comparer)
            : base(dictionary, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectivesDictionary"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected DirectivesDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Returns an XSD schema for the serialisable facts dictionary.  This is referenced by the XmlSchemaProvider
        /// attribute on this class in order control the XML format. 
        /// </summary>
        /// <param name="schemaSet">A cache of XSD schemas.</param>
        /// <returns>The qualified XML name of of the FactsDictionary type.</returns>
        public static new XmlQualifiedName GetDictionarySchema(XmlSchemaSet schemaSet)
        {
            return GetDictionarySchema(
                schemaSet,
                "DirectivesDictionaryType",
                Properties.Resources.DictionaryNamespace,
                Properties.Resources.XsdDirectiveSchemaFile);
        }

        /// <summary>
        /// Reads a key value as a string.
        /// </summary>
        /// <param name="reader">An XML reader containing the serialized key.</param>
        /// <returns>A string key value.</returns>
        protected override string ReadKey(XmlReader reader)
        {
            return this.ReadKey(reader, "DirectivesDictionary");
        }

        /// <summary>
        /// Writes a key value as a string.
        /// </summary>
        /// <param name="writer">An XML writer for the serializable dictionary.</param>
        /// <param name="key">The key value to be serialized.</param>
        protected override void WriteKey(XmlWriter writer, string key)
        {
            this.WriteKey(writer, key, "DirectivesDictionary", Properties.Resources.DictionaryNamespace);
        }

        /// <summary>
        /// Returns an XSD schema for the serializable dictionary.  This is referenced by the XmlSchemaProvider
        /// attribute on this class in order control the XML format. 
        /// </summary>
        /// <param name="manifestResourceName">
        /// The Manifest resource name for the schema.
        /// </param>
        /// <returns>
        /// The XML schema of the Dictionary type.
        /// </returns>
        private static XmlSchema DoGetSchema(string manifestResourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(manifestResourceName);

            if (stream == null)
            {
                return null;
            }

            var xsdReader = new XmlTextReader(stream);
            return XmlSchema.Read(xsdReader, null);
        }
    }
}
