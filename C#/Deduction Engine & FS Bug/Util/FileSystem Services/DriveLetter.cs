namespace Util.FileSystem_Services
{
    /// <summary>
    /// Represents a Drive Letter such as used by Windows (e.g. C:\).
    /// </summary>
    public struct DriveLetter
    {
        public string Letter { get; }

        public DriveLetter(string name)
        {
            Letter = name;
        }
    }
}