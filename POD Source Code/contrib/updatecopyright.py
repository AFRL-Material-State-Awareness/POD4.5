import glob
import os


for fname in glob.glob('../**/*.cs', recursive=True):
    
    if "AppData" in fname:
        print("RUN FROM updatecopyright.py FOLDER. NOT YOUR ACCOUNT FOLDER")
        break
        
    if fname.endswith(".Designer.cs"):
        continue
        
    if fname.endswith("AssemblyInfo.cs"):
        continue
    
    if "dockpanelsuite-development_3.0" in fname:
        continue
    
    with open(fname, 'r', encoding='utf-8-sig') as fh:
        text = fh.readlines()
        
    if text[0][0:3] == '//-':
        pass
    else:
        i = 0
        while text[i][0:2] == '//':
            i = i+1
        
        with open(fname, 'w', encoding='utf-8-sig') as fh:
            print("%s" % fname)
            header = ['// <copyright file="%s" company="The University of Dayton">\n' % os.path.basename(fname),
                      '// Copyright © 2025 The University of Dayton. All rights reserved.\n',
                      '//\n',
                      '// Licensed under the GNU General Public License, Version 3.0 (the "License");\n',
                      '// you may not use this file except in compliance with the License.\n',
                      '// You may obtain a copy of the License at\n',
                      '//\n',
                      '//     https://www.gnu.org/licenses/gpl-3.0.txt\n',
                      '//\n',
                      '// Unless required by applicable law or agreed to in writing, software\n',
                      '// distributed under the License is distributed on an "AS IS" BASIS,\n',
                      '// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.\n',
                      '// See the License for the specific language governing permissions and\n',
                      '// limitations under the License.\n',
                      '// </copyright>\n']
            if not text[i].startswith('\n'):
                header.append('\n');
            fh.writelines(header)
            fh.writelines(text[i:])