// MediaBrowser.Model/BuildVersion.cs
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
using System.Text;

namespace MediaBrowser.Model.System
{
    public class BuildVersion
    {
        public string CommitHash { get; }

        public long Revision { get; }

        public string Branch { get; }

        public string TagDescription { get; }

        public Uri Remote { get; }

        public BuildVersion(string buildVersionString)
        {
            var items = buildVersionString.Split('|');
            if (items.Length == 5)
            {
                CommitHash = items[0];
                if (!long.TryParse(items[1], out long rev))
                {
                    throw new ArgumentException(nameof(buildVersionString),
                        $"_buildVersionString's second element '{items[2]}' should be an integer.");
                }
                Revision = rev;
                Branch = items[2];
                TagDescription = items[3];
                var remoteRepo = items[4].Replace(".git", string.Empty);
                if (Uri.IsWellFormedUriString(remoteRepo, UriKind.Absolute))
                {
                    Remote = new Uri(remoteRepo);
                }
                else if(Uri.IsWellFormedUriString(items[4], UriKind.Absolute))
                {
                    //fallback if the replace about broke the Uri
                    Remote = new Uri(items[4]);
                }
                else
                {
                    throw new ArgumentException(nameof(buildVersionString),
                        $"_buildVersionString's fifth element '{items[4]}' should be a valid remote URI.");
                }
            }
            else
            {
                throw new ArgumentException(nameof(buildVersionString),
                    $"_buildVersionString '{buildVersionString}' does not contain 5 pipe delimited items.");
            }
        }

        /// <summary>
        /// Creates a BuildVersion object from a simple file.
        /// </summary>
        /// <param name="filename">The pipe delimited build version file.</param>
        /// <returns>The BuildVersion object representing the contents on the build verison file.</returns>
        public static BuildVersion FromSimpleFile(string filename)
        {
            return new BuildVersion(File.ReadAllText(filename).Trim());
        }

        public override string ToString()
        {
            return $"{CommitHash};{Revision};{Branch};{TagDescription};{Remote}";
        }
    }
}
