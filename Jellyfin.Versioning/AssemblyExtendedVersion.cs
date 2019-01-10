// Jellyfin.Versioning/AssemblyExtendedVersion.cs
// Part of the Jellyfin project (https://jellyfin.media)
//
//    This Source Code Form is subject to the terms of the Mozilla Public
//    License, v. 2.0. If a copy of the MPL was not distributed with this file,
//    You can obtain one at http://mozilla.org/MPL/2.0/.
//
//    All copyright belongs to the Jellyfin contributors; a full list can
//    be found in the file CONTRIBUTORS.md
//
//    Alternatively, the contents of this file may be used under the terms
//    of the GNU General Public License Version 2 or later, as described below:
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Jellyfin.Versioning
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyExtendedVersion : Attribute {
        public ExtendedVersion ExtendedVersion { get; }

        public AssemblyExtendedVersion(ExtendedVersion ExtendedVersion)
        {
            this.ExtendedVersion = ExtendedVersion;
        }
        public AssemblyExtendedVersion(string ApiVersion, bool ReadResource=true)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames();
            var resourceName = "Jellyfin.Versioning.jellyfin_version.ini";
            var result = new List<string>();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        while(!reader.EndOfStream)
                            result.Add(reader.ReadLine());
                    }
                    ExtendedVersion = new ExtendedVersion(new Version(ApiVersion), result.ToArray());
                }
                else
                {
                    ExtendedVersion = new ExtendedVersion(new Version(ApiVersion));
                }
            }
        }
    }
}
