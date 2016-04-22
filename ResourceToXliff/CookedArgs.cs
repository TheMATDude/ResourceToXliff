// The MIT License(MIT)
//
// Copyright(c) 2016  Microsoft Corporation. All Rights Reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
// associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR 
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace ResourceToXliff
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Microsoft.Multilingual.Utilities;

    internal class CookedArgs
    {
        internal CookedArgs(string[] args)
        {
            if (args.Count() < 4)
            {
                IsArgError = true;
                ErrorMessage = "Invalid number of arguments";
                return;
            }

            OutputFolder = args[0];
            DefaultLanguage = args[1];
            this.DefaultResourceFile = args[2];

            switch (Path.GetExtension(DefaultResourceFile).ToUpperInvariant())
            {
                case ".RESX":
                    ResourceFileType = ResourceType.Resx;
                    break;
                case ".RESW":
                    ResourceFileType = ResourceType.Resw;
                    break;
                case ".RESJSON":
                    ResourceFileType = ResourceType.ResJson;
                    break;
                default:
                    IsArgError = true;
                    ErrorMessage = "Unsupported file extension";
                    return;
            }

            List<string> resourceFiles = new List<string>();
            for(int idx = 3; idx<args.Count(); idx++)
            {
                resourceFiles.Add(args[idx]);
            }

            this.TranslatedResourceFiles = resourceFiles;
        }

        internal void Usage()
        {
            Console.WriteLine("Usage Error: ");
            Console.WriteLine(ErrorMessage);

            Console.WriteLine("\nUsage: ");
            Console.WriteLine("ResourceToXliff OutputFolder DefaultLanguage DefaultResourceFile TranslatedResourceFile1 [TranslatedResourceFile2 ...]");
            Console.WriteLine("\nWhere:");
            Console.WriteLine("\tOutputFolder            = The folder to place the resulting XLF files");
            Console.WriteLine("\tDefaultLanguageCode     = Source language of the DefaultResourceFile.  (e.g.: 'en-US')");
            Console.WriteLine("\tDefaultResourceFile     = project's default resource file.  (e.g.: 'AppResources.resx')");
            Console.WriteLine("\tTranslatedResourceFile1 = project's Translated RESX file.  (e.g.: 'AppResources.de-DE.resx'");
            Console.WriteLine("\tTranslatedResourceFile2 = Ditto");
            Console.WriteLine("\nRESX usage example:");
            Console.WriteLine("\tResourceToXliff .\\ en-US AppResources.resx AppResources.de-DE.resx AppResources.fr-FR.resx");
            Console.WriteLine("\nWill create two XLIFF 1.2 based files in the current directory:");
            Console.WriteLine("\tAppResources.de-DE.xlf");
            Console.WriteLine("\tAppResources.fr-FR.xlf");
            Console.WriteLine("\nRESW usage example:");
            Console.WriteLine("\tResourceToXliff .\\ en-US en-US\\Resources.resw de-DE\\Resources.resw fr-FR\\Resources.resw");
            Console.WriteLine("\nWill create two XLIFF 1.2 based files in the current directory:");
            Console.WriteLine("\tResources.de-DE.xlf");
            Console.WriteLine("\tResources.fr-FR.xlf");
            Console.WriteLine("\nThe resulting XLIFF files can now be imported into your MAT enabled projects.");
            Console.WriteLine("NOTE: You will need to check the 'Enable resource recycling' checkbox when importing these files.");
        }

        internal bool IsArgError { get; private set; }

        internal string ErrorMessage { get; private set; }

        internal string OutputFolder{ get; private set; }

        internal string DefaultLanguage { get; private set; }

        internal string DefaultResourceFile { get; private set; }

        internal ResourceType ResourceFileType { get; private set; }

        internal IEnumerable<string> TranslatedResourceFiles { get; private set; }
    }
}
