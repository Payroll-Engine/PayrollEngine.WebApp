using System;
using PayrollEngine.IO;

namespace PayrollEngine.WebApp;

/// <summary>
/// Download tool
/// </summary>
public static class DownloadTool
{
    /// <summary>
    /// Convert file name to download file name
    /// </summary>
    /// <param name="fileName">Original file name</param>
    /// <param name="removeSpaces">Remove spaces</param>
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