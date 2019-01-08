@ECHO OFF
goto licenseblock
update-version.bat
Part of the Jellyfin project (https://jellyfin.media)

   This Source Code Form is subject to the terms of the Mozilla Public
   License, v. 2.0. If a copy of the MPL was not distributed with this file,
   You can obtain one at http://mozilla.org/MPL/2.0/.

   All copyright belongs to the Jellyfin contributors; a full list can
   be found in the file CONTRIBUTORS.md

   Alternatively, the contents of this file may be used under the terms
   of the GNU General Public License Version 2 or later, as described below:

   This program is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 2 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with this program. If not, see <https://www.gnu.org/licenses/>.
:licenseblock

SET commit=
for /f "delims=" %%a in ('git rev-parse HEAD') do @set commit=%%a
SET count=
for /f "delims=" %%a in ('git rev-list HEAD --count') do @set count=%%a
SET branch=
for /f "delims=" %%a in ('git rev-parse --abbrev-ref HEAD') do @set branch=%%a
SET desc=
for /f "delims=" %%a in ('git describe --tags') do @set desc=%%a
SET remote=
for /f "delims=" %%a in ('git config --get remote.origin.url') do @set remote=%%a
echo %commit%^|%count%^|%branch%^|%desc%^|%remote% > jellyfin.version
echo Updated build version to %commit%^|%count%^|%branch%^|%desc%^|%remote%