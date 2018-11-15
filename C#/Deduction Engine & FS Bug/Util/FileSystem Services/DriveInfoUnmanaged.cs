using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Util.FileSystem_Services
{
    /// <summary>
    /// Obtains Drive Information using pinvoke.
    /// </summary>
    public class DriveInfoUnmanaged
    {
        #region DllImport

        [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true, CharSet = CharSet.Auto)]
        private static extern SafeFileHandle CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile
        );

        [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true, CharSet = CharSet.Auto)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            int nInBufferSize,
            IntPtr lpOutBuffer,
            int nOutBufferSize,
            ref uint lpBytesReturned,
            IntPtr lpOverlapped
        );

        #endregion DllImport

        #region StructLayout

        [StructLayout(LayoutKind.Sequential)]
        private struct StorageDescriptorHeader
        {
            private readonly int Version;
            public readonly int Size;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct StorageAccessAlignmentDescriptor
        {
            private readonly int Version;
            private readonly int Size;
            private readonly int BytesPerCacheLine;
            private readonly int BytesOffsetForCacheAlignment;
            public readonly int BytesPerLogicalSector;
            public readonly int BytesPerPhysicalSector;
            private readonly int BytesOffsetForSectorAlignment;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct StoragePropertyQuery
        {
            public int PropertyId;
            public int QueryType;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            private readonly byte[] AdditionalParameters;
        }

        #endregion StructLayout

        #region CTCL_CODE

        private const uint FileDeviceMassStorage = 0x2d; // DeviceType
        private const uint FunctionCode = 0x500; // FunctionCode
        private const uint MethodBuffered = 0; // TransferType
        private const uint FileAnyAccess = 0; // RequiredAccess

        // From the equivalent C macro. 
        private static uint CTL_CODE(uint deviceType, uint functionCode, uint transferType, uint requiredAccess)
            => deviceType * 65536 + requiredAccess * 16384 + functionCode * 4 + transferType;

        #endregion CTCL_CODE

        #region FLAGS

        private const int StorageAccessAlignmentProperty = 0x6; // _STORAGE_PROPERTY_QUERY STORAGE_PROPERTY_ID
        private const uint FileShareWrite = 0x2; // CreateFile dwShareMode
        private const uint FileShareRead = 0x1; // CreateFile dwShareMode
        private const uint OpenExisting = 0x3; // CreateFile dwCreationDisposition

        public const uint
            FileFlagNoBuffering = 0x20000000; // CreateFile dwFlagsAndAttributes (also for .NET FileStream)

        #endregion FLAGS

        /// <inheritdoc cref="OverloadedMethod(PhysicalDriveIdentifier, out int, out int)"/>
        public static void GetSectorInfo(DriveLetter driveLetter, out int bytesPerLogicalSector,
            out int bytesPerPhysicalSector)
            => GetSectorInfo(PhysicalDriveConvertor.GetFromDriveInfo(driveLetter), out bytesPerLogicalSector,
                out bytesPerPhysicalSector);

        /// <summary>
        /// Obtain both the Logical and Physical Bytes per Sector.
        /// </summary>
        /// <param name="drive">The drive to get the Bytes per Sector for.</param>
        /// <param name="bytesPerLogicalSector">The data-store for the Logical Bytes per Sector.</param>
        /// <param name="bytesPerPhysicalSector">The data-store for the Physical Bytes per Sector.</param>
        /// <returns>Nothing or an exception.</returns>
        public static void GetSectorInfo(PhysicalDriveIdentifier drive, out int bytesPerLogicalSector,
            out int bytesPerPhysicalSector)
        {
            bytesPerLogicalSector = 0;
            bytesPerPhysicalSector = 0;

            using (var deviceHandle = CreateFile(drive.Drive, 0, FileShareWrite | FileShareRead, default, OpenExisting,
                0, default))
            {
                if (deviceHandle == null || deviceHandle.IsInvalid)
                    throw new InvalidOperationException();

                var structure =
                    GetStorageStructure<StorageAccessAlignmentDescriptor>(deviceHandle, StorageAccessAlignmentProperty);
                bytesPerPhysicalSector = structure.BytesPerPhysicalSector;
                bytesPerLogicalSector = structure.BytesPerLogicalSector;
            }
        }

        private static T GetStorageStructure<T>(SafeFileHandle deviceHandle, int propertyId) where T : struct
        {
            // Setup "Storage Query" Data-Store.
            var sqSize = Marshal.SizeOf(typeof(StoragePropertyQuery));
            var sqPtr = Marshal.AllocHGlobal(sqSize);

            // Setup "Storage Query Result Header" Data-Store.
            var sqrhSize = Marshal.SizeOf(typeof(StorageDescriptorHeader));
            var sqrhPtr = Marshal.AllocHGlobal(sqrhSize);

            // Initialize "Storage Query"
            var sq = new StoragePropertyQuery {PropertyId = propertyId, QueryType = 0};
            Marshal.StructureToPtr(sq, sqPtr, true);

            // Execute "Storage Query" for getting Result Header.
            var sqrh = GetDeviceIoControl<StorageDescriptorHeader>(deviceHandle, sqPtr, sqSize, sqrhPtr, sqrhSize);

            // Setup "Storage Query Result Body" Data-Store.
            var sqrbPtr = Marshal.AllocHGlobal(sqrh.Size);

            // Execute "Storage Query" for getting Result Body.
            return GetDeviceIoControl<T>(deviceHandle, sqPtr, sqSize, sqrbPtr, sqrh.Size);
        }

        private static T GetDeviceIoControl<T>(SafeFileHandle deviceHandle, IntPtr sqPtr, int sqSize, IntPtr bufferPtr,
            int bufferSize) where T : struct
        {
            var bytesReturned = default(uint);
            var ctlCode = CTL_CODE(FileDeviceMassStorage, FunctionCode, MethodBuffered, FileAnyAccess);

            if (!DeviceIoControl(deviceHandle, ctlCode, sqPtr, sqSize, bufferPtr, bufferSize, ref bytesReturned,
                IntPtr.Zero))
            {
                FreeResources(sqPtr, bufferPtr);
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (bufferSize != bytesReturned)
            {
                FreeResources(sqPtr, bufferPtr);
                throw new InvalidOperationException();
            }

            return (T) Marshal.PtrToStructure(bufferPtr, typeof(T));
        }

        private static void FreeResources(params IntPtr[] resources)
        {
            foreach (var resource in resources)
                Marshal.FreeHGlobal(resource);
        }
    }
}