using System;
using PayrollEngine.IO;

namespace PayrollEngine.WebApp;

public static class DownloadTool
{
    public static string ToDownloadFileName(string fileName, bool removeSpaces = true)
    {
        if (fileName == null)
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        // ensure upper word
        fileName = fileName.ToSentence(startCase: CharacterCase.ToUpper,
            wordCase: CharacterCase.ToUpper);
        // remove invalid characters
        fileName = FileTool.ToValidFileName(fileName);
        // remove spaces
        if (removeSpaces)
        {
            fileName = fileName.Replace(" ", string.Empty);
        }
        return fileName;
    }
}