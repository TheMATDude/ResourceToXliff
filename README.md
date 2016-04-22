# Multilingual App Toolkit (**MAT**) RESX 2 XLIFF converter

Converts RESX files (or RESW ) to XLIFF 1.2 files that can be import (w/recycling) into a MAT enabled project, allowing for recovery of resources previously translated without MAT. 

**Usage:**<br>
ResxToXliff OutputFolder DefaultLanguage DefaultResxFile TranslatedResxFile1 [TranslatedResxFile2 ...]

**Where:**

| Parameter | Value |
| --------- | ----- |
| OutputFolder | The folder to place the resulting XLF files |
| DefaultResxLanguageCode | Language of DefaultResxFile.  (e.g.: 'en-US') |
| DefaultResxFile | RESX based project's default RES file.  (e.g.: 'AppResources.resx') |
| TranslatedResxFile1 | project's Translated RESX file.  (e.g.: 'AppResources.de-DE.resx') |
| TranslatedResxFile2 | Ditto |

**For example, running this command:**<br />
ResxToXliff .\ en-US AppResources.resx AppResources.de-DE.resx AppResources.fr-FR.resx

**Will create two XLIFF 1.2 based files in the current directory:**<br />
  AppResources.de-DE.xlf<br />
  AppResources.fr-FR.xlf<br />

These XLIFF file can now be imported into your MAT enabled project.
NOTE: You will need to check the 'Enable resource recycling' checkbox when importing these files.

**Build note:** *If MAT is not installed on the machine, the DLL references will fail. Click the following link to install the [Microsoft App Toolkit] (http://aka.ms/matinstall).*
