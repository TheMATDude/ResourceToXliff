# Multilingual App Toolkit (**MAT**) RESX 2 XLIFF converter

Converts Resource files (RESX, RESW, RESJSON) to XLIFF 1.2 files that can be import (w/recycling) into a MAT enabled project, allowing for recovery of resources previously translated without MAT. 

**Usage:**<br>
ResourceToXliff OutputFolder DefaultLanguageCode DefaultResourceFile TranslatedResourceFile1 [TranslatedResourceFile2 ...]

**Where:**

| Parameter | Value |
| --------- | ----- |
| OutputFolder | The folder to place the resulting XLF files |
| DefaultLanguageCode | Language of DefaultResourceFile.  (e.g.: 'en-US') |
| DefaultResourceFile | Resource file based on project's default resource file.  (e.g.: 'AppResources.resx') |
| TranslatedResourceFile1 | project's Translated Resource file.  (e.g.: 'AppResources.de-DE.resx') |
| TranslatedResourceFile2 | Ditto |

**RESX usage example**<br />
ResourceToXliff .\ en-US AppResources.resx AppResources.de-DE.resx AppResources.fr-FR.resx

**Will create two XLIFF 1.2 based files in the current directory:**<br />
  AppResources.de-DE.xlf<br />
  AppResources.fr-FR.xlf<br />

**RESW usage example**<br />
ResourceToXliff .\ en-US en-US\Resources.resw de-DE\Resources.resw fr-FR\Resources.resw

**Will create two XLIFF 1.2 based files in the current directory:**<br />
  Resources.de-DE.xlf
  Resources.fr-FR.xlf

The resulting XLIFF files can now be imported into your MAT enabled projects.
NOTE: You will need to check the 'Enable resource recycling' checkbox when importing these files.

**Build note:** *If MAT is not installed on the machine, the DLL references will fail when building. Click the following link to install the [Multilingual App Toolkit] (http://aka.ms/matinstall).*
