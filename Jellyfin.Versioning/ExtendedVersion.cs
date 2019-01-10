// Jellyfin.Versioning/ExtendedVersion.cs
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
using System.Runtime.Serialization;
using System.Text;

namespace Jellyfin.Versioning
{
    public class ExtendedVersion
    {
        [IgnoreDataMember]
        public Version ApiVersion { get; }

        public string CommitHash { get; }

        public long Revision { get; }

        public string Branch { get; }

        public string TagDescription { get; }

        public Uri Remote { get; }

        public ExtendedVersion()
        {
            ApiVersion = new Version(1, 0, 0, 0);
            //Defaults
            CommitHash = string.Empty;
            Revision = 1;
            Branch = "master";
            TagDescription = "master";
            Remote = new Uri("https://jellyfin.media");

        }

        public ExtendedVersion(Version ApiVersion) : this()
        {
            this.ApiVersion = ApiVersion;
        }

        public ExtendedVersion(Version ApiVersion, string[] ExtendedVersionFileArray) : this(ApiVersion)
        {           
            int line = 1;           

            foreach (string item in ExtendedVersionFileArray)
            {
                if (string.IsNullOrWhiteSpace(item.Trim()))
                {
                    //empty line, skip
                    continue;
                }
                var kvpair = item.Split('=');
                if (kvpair.Length == 2)
                {
                    var key = kvpair[0].Trim().ToLower();
                    var value = kvpair[1].Trim();
                    switch (key)
                    {
                        case "commit":
                            if(value.Length < 7 || value.Length > 40)
                            {
                                throw new ArgumentException(nameof(ExtendedVersionFileArray),
                                    $"ExtendedVersionFile has a bad commit hash '{value}' on line {line}, it should be a string between 7 and 40 characters.");
                            }
                            CommitHash = value;
                            break;
                        case "branch":
                            if (string.IsNullOrWhiteSpace(value))
                            {
                                throw new ArgumentException(nameof(ExtendedVersionFileArray),
                                    $"ExtendedVersionFile has a bad branch '{value}' on line {line}, it can not be empty.");
                            }
                            Branch = value;
                            break;
                        case "revision":
                            if (!long.TryParse(value, out long rev))
                            {
                                throw new ArgumentException(nameof(ExtendedVersionFileArray),
                                    $"ExtendedVersionFile has a bad revision '{value}' on line {line}, it should be an integer.");
                            }
                            Revision = rev;
                            break;
                        case "tagdesc":
                            if (string.IsNullOrWhiteSpace(value))
                            {
                                throw new ArgumentException(nameof(ExtendedVersionFileArray),
                                    $"ExtendedVersionFile has a bad tag description '{value}' on line {line}, it can not be empty.");
                            }
                            TagDescription = value;
                            break;
                        case "remote":
                            var remoteRepo = value.Replace(".git", string.Empty).Replace("git@github.com:","https://github.com/");
                            if (Uri.IsWellFormedUriString(remoteRepo, UriKind.Absolute))
                            {
                                Remote = new Uri(remoteRepo);
                            }
                            else if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
                            {
                                //fallback if the replace about broke the Uri
                                Remote = new Uri(value);
                            }
                            else
                            {
                                throw new ArgumentException(nameof(ExtendedVersionFileArray),
                                    $"ExtendedVersionFile has a bad remote URI '{value}' on line {line}, it should be a valid remote URI (ssh or https).");
                            }
                            break;
                        default:
                            throw new ArgumentException(nameof(ExtendedVersionFileArray),
                            $"ExtendedVersionFile contains an unrecognized key-value pair '{item}' at line {line}.");
                    }
                }
                else
                {
                    throw new ArgumentException(nameof(ExtendedVersionFileArray),
                        $"ExtendedVersionFile contains bad key-value pair '{item}' at line {line}.");
                }
                line++;
            }
        }

        public override string ToString()
        {
            return $"{ApiVersion};{CommitHash};{Revision};{Branch};{TagDescription};{Remote}";
        }        
    }
}
