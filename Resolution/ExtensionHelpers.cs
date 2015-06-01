// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionHelpers.cs" company="Solidsoft Reply Ltd.">
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
//   limitations under the License.     ワ⯹଀鐀卌㩃���ߎ���毑䞩毒༘毒毑ᢢ㖥 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SolidsoftReply.Esb.Libraries.Resolution
{
    using System;
    using System.Runtime.Caching;
    using System.Text;

    /// <summary>
    /// A library of helper extension methods.
    /// </summary>
    internal static class ExtensionHelpers
    {
        /// <summary>
        /// Returns a schema strong name that has been cached.
        /// </summary>
        /// <param name="schemaStrongName">
        /// The schema strong name to be cached.
        /// </param>
        /// <param name="schemaName">
        /// The schema name, used as a key in the cache.
        /// </param>
        /// <returns>
        /// The cached strong name.
        /// </returns>
        public static string CachedAsSchemaStrongName(this string schemaStrongName, string schemaName)
        {
            Transformer.SchemaStrongNameCache.Add(
                schemaName,
                schemaStrongName,
                new CacheItemPolicy { SlidingExpiration = TimeSpan.FromDays(1) });
            return schemaStrongName;
         }

        /// <summary>
        /// Returns a Base64 encoded string.
        /// </summary>
        /// <param name="unencodedData">The unencoded string.</param>
        /// <returns>A Base64 encoded representation of the string.</returns>
        public static string EncodeToBase64(this string unencodedData)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(unencodedData ?? string.Empty));
        }

        /// <summary>
        /// Returns an unencoded string from Base64 representation.
        /// </summary>
        /// <param name="encodedData">The Base64 encoded string.</param>
        /// <returns>An unencoded string.</returns>
        public static string DecodeFromBase64(this string encodedData)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedData ?? string.Empty));
        }
    }
}
