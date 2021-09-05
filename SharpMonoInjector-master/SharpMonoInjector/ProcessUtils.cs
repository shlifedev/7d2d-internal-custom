﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace SharpMonoInjector
{
    public static class ProcessUtils
    {        
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process2([In] IntPtr hProcess, [Out] out ushort processMachine, [Out] out ushort nativeMachine);

        private static bool isTargetx64;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

        public static IEnumerable<ExportedFunction> GetExportedFunctions(IntPtr handle, IntPtr mod)
        {
            using (Memory memory = new Memory(handle))
            {
                int e_lfanew = memory.ReadInt(mod + 0x3C);
                IntPtr ntHeaders = mod + e_lfanew;
                IntPtr optionalHeader = ntHeaders + 0x18;
                IntPtr dataDirectory = optionalHeader + (Is64BitProcess(handle) ? 0x70 : 0x60);
                IntPtr exportDirectory = mod + memory.ReadInt(dataDirectory);
                IntPtr names = mod + memory.ReadInt(exportDirectory + 0x20);
                IntPtr ordinals = mod + memory.ReadInt(exportDirectory + 0x24);
                IntPtr functions = mod + memory.ReadInt(exportDirectory + 0x1C);
                int count = memory.ReadInt(exportDirectory + 0x18);

                for (int i = 0; i < count; i++)
                {
                    try // Added 8-7-2021 J.E
                    {
                        int offset = memory.ReadInt(names + i * 4);
                        string name = memory.ReadString(mod + offset, 32, Encoding.ASCII);
                        short ordinal = memory.ReadShort(ordinals + i * 2);
                        IntPtr address = mod + memory.ReadInt(functions + ordinal * 4);

                        if (address != IntPtr.Zero)
                        {
                            yield return new ExportedFunction(name, address);
                        }
                    }
                    finally { }
                }
            }
        }

        public static bool GetMonoModule(IntPtr handle, out IntPtr monoModule)
        {
            int size = Is64BitProcess(handle) ? 8 : 4;

            IntPtr[] ptrs = new IntPtr[0];

            if (!Native.EnumProcessModulesEx(handle, ptrs, 0, out int bytesNeeded, ModuleFilter.LIST_MODULES_ALL))
            {
                throw new InjectorException("Failed to enumerate process modules", new Win32Exception(Marshal.GetLastWin32Error()));
            }

            int count = bytesNeeded / size;
            ptrs = new IntPtr[count];

            if (!Native.EnumProcessModulesEx(handle, ptrs, bytesNeeded, out bytesNeeded, ModuleFilter.LIST_MODULES_ALL))
            {
                throw new InjectorException("Failed to enumerate process modules", new Win32Exception(Marshal.GetLastWin32Error()));
            }

            for (int i = 0; i < count; i++)
            {
                try
                {
                    StringBuilder path = new StringBuilder(260);
                    Native.GetModuleFileNameEx(handle, ptrs[i], path, 260);

                    if (path.ToString().IndexOf("mono", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        if (!Native.GetModuleInformation(handle, ptrs[i], out MODULEINFO info, (uint)(size * ptrs.Length)))
                        {
                            throw new InjectorException("Failed to get module information", new Win32Exception(Marshal.GetLastWin32Error()));
                        }

                        var funcs = GetExportedFunctions(handle, info.lpBaseOfDll);

                        if (funcs.Any(f => f.Name == "mono_get_root_domain"))
                        {
                            monoModule = info.lpBaseOfDll;
                            return true;
                        }
                    }
                }
                catch (Exception ex) { File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\DebugLog.txt", "[ProcessUtils] GetMono - ERROR: " + ex.Message + "\r\n"); }
            }

            monoModule = IntPtr.Zero;
            return false;
        }

        public static bool Is64BitProcess(IntPtr handle)
        {
            try
            {
                if (!Environment.Is64BitOperatingSystem) { return false; }

                string OSVer = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows NT\CurrentVersion", "ProductName", null);
                Console.WriteLine(OSVer);

                if(OSVer.Contains("Windows 10"))
                {
                    #region[Win10]
            
                    isTargetx64 = false;

                    if (handle != IntPtr.Zero)
                    {
                        ushort pMachine = 0;
                        ushort nMachine = 0;

                        try
                        {
                            if (!IsWow64Process2(handle, out pMachine, out nMachine))
                            {
                                //handle error
                            }

                            if (pMachine == 332)
                            {
                                isTargetx64 = false;
                            }
                            else
                            {
                                isTargetx64 = true;

                            }

                            return isTargetx64;
                        }
                        catch { /* Will try the Win7 method */ }
                    }
            
                    #endregion
                }

                #region[Win7]

                IsWow64Process(handle, out bool isTargetWOWx64);

                if (isTargetWOWx64)
                {
                    return false; // It is WOW64 so it's a 32-bit process
                }
                else 
                {
                    return true; // It's not a WOW64 process so 64-bit process, and we already check if OS is 32 or 64 bit.
                }

                #endregion


                //ORIG
                //if (!IsWow64Process(handle, out bool is64bit))
                //{
                //    return IntPtr.Size == 8; // assume it's the same as the current process */ 
                //}
            }
            catch (Exception ex) { File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\DebugLog.txt", "[ProcessUtils] is64Bit - ERROR: " + ex.Message + "\r\n"); }
            return true;
        }
    }
}
