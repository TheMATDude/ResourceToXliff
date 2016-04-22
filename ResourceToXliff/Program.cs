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
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Microsoft.Multilingual.Utilities;
    using Microsoft.Multilingual.Xliff;

    /// <summary>
    /// Converts resource files (RESX, RESW or RESJSON) to XLIFF 1.2 files that can be import w/recycling enabled
    /// into a MAT enabled project, allowing for recovery of resources previously translated without MAT.
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
            // Parse the command line arguments
            var appArgs = new CookedArgs(args);
            if (appArgs.IsArgError)
            {
                appArgs.Usage();
                return 1;
            }

            // Ensure output folder exits
            if (!Directory.Exists(appArgs.OutputFolder))
                Directory.CreateDirectory(appArgs.OutputFolder);

            // Throws if invalid :-(
            var sourceCulture = new CultureInfo(appArgs.DefaultLanguage);

            // Created the Source XLIFF file in memory (Source = sourceCulture and Target = sourceCulture)
            var xliffSources = XliffDocument.ReadFromResource(appArgs.DefaultResourceFile, sourceCulture, string.Empty, appArgs.ResourceFileType);

            // IMPORTANT: 
            // The translated RESX files MUST have the target culture as part of the RESX extension
            // For example: AppResources.ru.resx contains (neutral) Russian translations 
            // This is what the RESX compiler expects, so it should already be in this format.
            //
            // The translated RESW and RESJSON files must either follow the RESX naming requirements,
            // or be in a name language folder.  All files must follow the same naming pattern.
            // For example: en\Resources.resw (source) followed by ru\Resources.resw (target) fr\Resources.resw (target), etc

            Console.WriteLine();
            Console.WriteLine("Resulting XLF files:");

            // Now, process all the translated resource file.  
            foreach (var resourceFile in appArgs.TranslatedResourceFiles)
            {
                // Best line of code I should never write again :-)
                var cultureName = Path.GetExtension(Path.GetFileNameWithoutExtension(resourceFile)).Replace(".", "");

                // If culture is not specified in the filename it might be in the directory name
                if (string.IsNullOrEmpty(cultureName))
                {
                    // Keeping the code style on the same level ;-)
                    cultureName = Path.GetFileName(Path.GetDirectoryName(resourceFile));
                }

                var targetCulture = new CultureInfo(cultureName);
                var xliffTargets = XliffDocument.ReadFromResource(resourceFile, sourceCulture, string.Empty, appArgs.ResourceFileType);
                if (xliffTargets.Count > 1)
                    throw new Exception("Unexpected number of XLIFF Documents returned");

                // Switch the XLIFF Target language to that of the translated resource file
                foreach (var xliffTarget in xliffTargets)
                {
                    foreach (var file in xliffTarget.FileInfos)
                    {
                        file.TargetCulture = targetCulture;
                    }
                }

                // Now for each matching ID, apply the source XLIFF source string to the target XLIFF files source string
                // This will make the target XLIFF file import (w/recycling) into the MAT based projects

                // It's brute force time...
                foreach (var xliffSource in xliffSources)
                {
                    foreach (var file in xliffSource.FileInfos)
                    {
                        foreach (var group in file.Body.TranslationGroups)
                        {
                            foreach (var transUnit in group.TranslationUnits)
                            {
                                var matchedUnits = GetMatchingUnits(xliffTargets, transUnit);
                                foreach(var matchedUnit in matchedUnits)
                                {
                                    // Switch the source to the actual source string (As it was originally created as the translated string)
                                    // Note: The Translation may be the same as the source in some cases.  If this is detected, mark it as needs review
                                    // Note: You may want to leave it as 'new' as this will block future recycling attempts
                                    if (matchedUnit.SourceSegment.Equals(transUnit.SourceSegment, StringComparison.InvariantCulture))
                                    {
                                        matchedUnit.State = TransUnitState.NeedsReview;
                                        matchedUnit.Notes.Add(new Note() { From = Note.MultilingualUpdateIdentifier, Content = "Resource is marked as 'Needs review' since the Source and target were the same value." });
                                    }
                                    else
                                    {
                                        matchedUnit.State = TransUnitState.Translated;
                                        matchedUnit.SourceSegment = transUnit.SourceSegment;
                                    }

                                    ////
                                    //// Include to force converting of Windows Phone SL lower case design to newer uppercase model.
                                    ////
                                    //if (matchedUnit.SourceSegment.Length > 1)
                                    //{
                                    //    matchedUnit.SourceSegment = char.ToUpper(matchedUnit.SourceSegment[0]) + matchedUnit.SourceSegment.Substring(1);
                                    //}
                                    //if (matchedUnit.TargetSegment.Length > 1)
                                    //{
                                    //    matchedUnit.TargetSegment = char.ToUpper(matchedUnit.TargetSegment[0]) + matchedUnit.TargetSegment.Substring(1);
                                    //}
                                }
                            }
                        }
                    }
                }

                // Another 'best' line of code...
                var xliffFileName = Path.Combine(appArgs.OutputFolder, Path.GetFileNameWithoutExtension(appArgs.DefaultResourceFile)) + "." + targetCulture.Name + ".xlf";

                // Save the Target XLIFF file
                Debug.Assert(xliffTargets.Count > 0);
                xliffTargets[0].Save(xliffFileName);
                Console.WriteLine(xliffFileName);
            }

            return 0;
        }

        /// <summary>
        /// Matches the source TU to the target XLF file's TU
        /// </summary>
        /// <param name="xliffTargets">Target XLIFF file</param>
        /// <param name="sourceTransUnit">TU from source XLIFF file</param>
        /// <returns>List of matches</returns>
        /// <remarks>
        /// Zero matched indicate the resource was not in the translated resource file
        /// Usually a match results in only entry, but more than one match can happen if the ID is non-unique in the resource file
        /// </remarks>
        private static IEnumerable<TranslationUnit> GetMatchingUnits(List<XliffDocument> xliffTargets, TranslationUnit sourceTransUnit)
        {
            var matched = new List<TranslationUnit>();
            foreach (var xliffTarget in xliffTargets)
            {
                foreach (var group in xliffTarget.FileInfos.SelectMany(file => file.Body.TranslationGroups))
                {
                    matched.AddRange(@group.TranslationUnits.Where(transUnit => transUnit.Id == sourceTransUnit.Id));
                }
            }

            return matched;
        }
    }
}
