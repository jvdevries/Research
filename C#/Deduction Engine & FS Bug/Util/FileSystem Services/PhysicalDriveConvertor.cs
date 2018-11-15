using System;

namespace Util.FileSystem_Services
{
    /// <summary>
    /// Converts from and to <see cref="PhysicalDriveIdentifier"/>.
    /// </summary>
    public class PhysicalDriveConvertor
    {
        /// <summary>
        /// Gets a <see cref="PhysicalDriveIdentifier"/> from a <see cref="DriveLetter"/>.
        /// </summary>
        /// <param name="drive">The <see cref="DriveLetter"/>.</param>
        /// <returns>The <see cref="PhysicalDriveIdentifier"/> or an exception.</returns>
        public static PhysicalDriveIdentifier GetFromDriveInfo(DriveLetter drive)
        {
            if (drive.Letter == null) throw new ArgumentNullException();

            if (drive.Letter.Length == 0) throw new InvalidOperationException();

            var driveLetter = drive.Letter.Substring(0, 1);

            if (!char.IsLetter(driveLetter[0])) throw new NotSupportedException();

            return new PhysicalDriveIdentifier($"\\\\.\\{driveLetter}:.");
        }
    }
}