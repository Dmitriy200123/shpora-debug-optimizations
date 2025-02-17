using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace JPEG
{
    internal static class PInvoke
    {
        private const string Psapi = "Psapi.dll";
        private const string Kernel32 = "kernel32.dll";
        private const string User32 = "user32.dll";


        [DllImport(Psapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetProcessMemoryInfo(IntPtr hProcess, out PROCESS_MEMORY_COUNTERS_EX counters,
            Int32 cb);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_MEMORY_COUNTERS_EX
    {
        private readonly Int32 cb;
        private readonly Int32 PageFaultCount;
        public IntPtr PeakWorkingSetSize;
        private readonly IntPtr WorkingSetSize;
        private readonly IntPtr QuotaPeakPagedPoolUsage;
        private readonly IntPtr QuotaPagedPoolUsage;
        private readonly IntPtr QuotaPeakNonPagedPoolUsage;
        private readonly IntPtr QuotaNonPagedPoolUsage;
        private readonly IntPtr PagefileUsage;
        public IntPtr PeakPagefileUsage;
        public IntPtr PrivateUsage;
    }

    public static class MemoryMeter
    {
        private static Process _process = Process.GetCurrentProcess();

        public static long PrivateBytes()
        {
            var sizeOfCountersEx = Marshal.SizeOf<PROCESS_MEMORY_COUNTERS_EX>();
            return PInvoke.GetProcessMemoryInfo(_process.Handle, out var counters, sizeOfCountersEx)
                ? counters.PrivateUsage.ToInt64()
                : 0;
        }

        public static long PeakPrivateBytes()
        {
            var sizeOfCountersEx = Marshal.SizeOf<PROCESS_MEMORY_COUNTERS_EX>();
            return PInvoke.GetProcessMemoryInfo(_process.Handle, out var counters, sizeOfCountersEx)
                ? counters.PeakPagefileUsage.ToInt64()
                : 0;
        }

        public static long PeakWorkingSet()
        {
            var sizeOfCountersEx = Marshal.SizeOf<PROCESS_MEMORY_COUNTERS_EX>();
            return PInvoke.GetProcessMemoryInfo(_process.Handle, out var counters, sizeOfCountersEx)
                ? counters.PeakWorkingSetSize.ToInt64()
                : 0;
        }
    }
}