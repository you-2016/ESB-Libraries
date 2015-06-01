﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interchange.cs" company="Solidsoft Reply Ltd.">
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
//   limitations under the License.     ⱈ⯹Ѐ耀//   Copyright 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SolidsoftReply.Esb.Libraries.Resolution.ResolutionService
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Serialization;

    using SolidsoftReply.Esb.Libraries.Resolution.Dictionaries;

    /// <summary>
    /// Represents an interchange of directives.  Provides a convenient approach to accessing
    /// parameters as a dictionary.
    /// </summary>
    public partial class Interchange
    {
        /// <summary>
        /// Gets or sets the collection of general purpose parameters.
        /// </summary>
        [XmlIgnore]
        public ParametersDictionary ParametersDictionary
        {
            get
            {
                if (this.parametersField == null)
                {
                    return null;
                }

                var parametersDictionary = new ParametersDictionary();

                foreach (var parametersDictionaryItem in this.parametersField)
                {
                    var valueSerializer = new NetDataContractSerializer();
                    using (var ms = new MemoryStream())
                    {
                        var sw = new StreamWriter(ms);
                        sw.Write(parametersDictionaryItem.Value.OuterXml);
                        sw.Flush();
                        ms.Seek(0, SeekOrigin.Begin);

                        parametersDictionary.Add(parametersDictionaryItem.Key.@string, valueSerializer.Deserialize(ms));
                    }
                }

                return parametersDictionary;
            }

            set
            {
                foreach (var parametersDictionaryItem in value)
                {
                    var item = new ParametersDictionaryItem();
                    var valueSerializer = new NetDataContractSerializer();

                    item.Key.@string = parametersDictionaryItem.Key;

                    using (var ms = new MemoryStream())
                    {
                        valueSerializer.Serialize(ms, parametersDictionaryItem.Value);
                        ms.Position = 0;
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load(ms);

                        var newItemsArray = new ParametersDictionaryItem[parametersField.Length + 1];
                        parametersField.CopyTo(newItemsArray, 0);
                        newItemsArray[parametersField.Length] = new ParametersDictionaryItem
                                                               {
                                                                   Key =
                                                                       new ParametersDictionaryItemKey
                                                                           {
                                                                               @string
                                                                                   =
                                                                                   parametersDictionaryItem
                                                                                   .Key
                                                                           },
                                                                   Value =
                                                                       xmlDoc.DocumentElement
                                                               };

                        parametersField = newItemsArray;
                    }
                }
            }
        }
    }
}