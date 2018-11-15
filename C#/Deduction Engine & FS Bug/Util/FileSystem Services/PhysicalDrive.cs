namespace Util.FileSystem_Services
{
    /// <summary>
    /// Represents a Physical Drive Identifier compatible with the C++ CreateFile function.
    /// </summary>
    public struct PhysicalDriveIdentifier
    {
        public string Drive { get; }

        public PhysicalDriveIdentifier(string name)
        {
            Drive = name;
        }
    }
}