using System.Text;

namespace SAK;

/// <summary>
/// An enumeration of well-known Windows symbolic paths.
/// </summary>
/// <remarks>
/// See good info about paths here:
/// https://stackoverflow.com/a/17108534
/// %ALLUSERSPROFILE%           C:\ProgramData
/// %APPDATA%                   C:\Users\[Username]\AppData\Roaming
/// %COMMONPROGRAMFILES%        C:\Program Files\Common Files
/// %COMMONPROGRAMFILES(x86)%   C:\Program Files(x86)\Common Files
/// %COMSPEC%                   C:\Windows\System32\cmd.exe
/// %HOMEDRIVE%                 C:
/// %HOMEPATH%                  C:\Users\[Username]
/// %LOCALAPPDATA%              C:\Users\[Username]\AppData\Local
/// %PROGRAMDATA%               C:\ProgramData
/// %PROGRAMFILES%              C:\Program Files
/// %PROGRAMFILES(X86)%         C:\Program Files(x86) (only in 64-bit version)
/// %PUBLIC%                    C:\Users\Public
/// %SystemDrive%               C:
/// %SystemRoot%                C:\Windows
/// %TEMP% and %TMP%            C:\Users\[Username]\AppData\Local\Temp
/// %USERPROFILE%               C:\Users\[Username]
/// %WINDIR%                    C:\Windows
/// </remarks>
public enum WellKnownWindowsSymbolicPaths
{
    AllUsersProfile = 1,
    AppData,
}

public static class WindowsUtilities
{
    public static IReadOnlyDictionary<WellKnownWindowsSymbolicPaths, string> WellKnownWindowsSymbolicPathSymbols = new Dictionary<WellKnownWindowsSymbolicPaths, string>
    {
        [WellKnownWindowsSymbolicPaths.AllUsersProfile] = "%ALLUSERSPROFILE%",
        [WellKnownWindowsSymbolicPaths.AppData] = "%APPDATA%",
    };

    /// <summary>
    /// Construct a <see cref="StreamWriter"/> that does not insert a UTF-8 byte order mark (BOM).
    /// This is useful for interop with programs that do not allow or expect it.
    /// </summary>
    /// <param name="path">The path to the file to write to.</param>
    /// <param name="append">Whether to append to the file or not.</param>
    /// <returns>The ready to use <see cref="StreamWriter"/>.</returns>
    public static StreamWriter Utf8WithoutBomWriter(string path, bool append = false)
    {
        var writer = new StreamWriter(path, append: append, new UTF8Encoding(false));
        return writer;
    }


}