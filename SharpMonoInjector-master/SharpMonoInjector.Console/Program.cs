using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Security.Principal;

namespace SharpMonoInjector.Console
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            System.Console.Clear();
         
            bool IsElevated = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

            if (!IsElevated)
            {
                System.Console.WriteLine("관리자 권한으로 실행하지 않으면 문제가 생길 수 있음.");
            }

            if (AntivirusInstalled())
            { 
                System.Console.WriteLine("AV Initstallzed");
            }
            else
            {
                System.Console.WriteLine("AV Safe");
            }
            InjectWhile();
             
        }

        private static void PrintHelp()
        {
            const string help =
                "SharpMonoInjector 2.4 wh0am1 Mod\r\n\r\n" +
                "Usage:\r\n" +
                "smi.exe <inject/eject> <options>\r\n\r\n" +
                "Options:\r\n" +
                "-p - The id or name of the target process\r\n" +
                "-a - When injecting, the path of the assembly to inject. When ejecting, the address of the assembly to eject\r\n" +
                "-n - The namespace in which the loader class resides\r\n" +
                "-c - The name of the loader class\r\n" +
                "-m - The name of the method to invoke in the loader class\r\n\r\n" +
                "Examples:\r\n" +
                "smi.exe inject -p testgame -a ExampleAssembly.dll -n ExampleAssembly -c Loader -m Load\r\n" +
                "smi.exe eject -p testgame -a 0x13D23A98 -n ExampleAssembly -c Loader -m Unload\r\n";
            System.Console.WriteLine(help);
        }

        public static void InjectWhile()
        {
            IntPtr address = IntPtr.Zero;

            while (true)
            {
                try
                {
                    System.Console.WriteLine("-인젝트 테스트-");
                    System.Console.WriteLine("숫자1 입력 : 인젝트");
                    System.Console.WriteLine("숫자2 입력 : 이젝트");
                    System.Console.WriteLine("숫자3 입력 : img 테스트");
                    var id = int.Parse(System.Console.ReadLine());
                    var injector = new Injector("7DaysToDie"); 
                    if (id == 1)
                    {
                        var _fakeargs = @"inject -p 7DaysToDie -a 7d2dMonoInternal-main\obj\Debug\ExampleAssembly.dll -n ExampleAssembly -c Loader -m Load";
                        CommandLineArguments args = new CommandLineArguments(_fakeargs.Split(' '));
                        address = Inject(injector, args); 
                    }
                    else if(id == 2)
                    {
                        var addressStr = $"0x{address.ToInt64():X16}";
                        var _fakeargs = $"eject -p 7DaysToDie -a {addressStr} dll -n ExampleAssembly -c Loader -m Unload";
                        System.Console.WriteLine(_fakeargs);
                        CommandLineArguments args = new CommandLineArguments(_fakeargs.Split(' '));
                        Eject(injector, args);
                    }
                    else if(id == 3)
                    {
                        while (true)
                        {
                            injector = new Injector("7DaysToDie");
                            var _fakeargs = @"inject -p 7DaysToDie -a 7d2dMonoInternal-main\obj\Debug\ExampleAssembly.dll -n ExampleAssembly -c Loader -m Load";
                            CommandLineArguments args = new CommandLineArguments(_fakeargs.Split(' '));
                            var idx = Inject(injector, args);
                            if (idx != IntPtr.Zero)
                            {
                                address = idx;
                                break;
                            }
                            System.Threading.Thread.Sleep(125);
                        }

                    }

                    System.Console.WriteLine("process complete!!");  
                }
                catch
                {

                }
            }
        }

        private static IntPtr Inject(Injector injector, CommandLineArguments args)
        {
            string assemblyPath, @namespace, className, methodName;
            byte[] assembly;

            if (args.GetStringArg("-a", out assemblyPath))
            {
                try
                {
                    assembly = File.ReadAllBytes(assemblyPath);
                }
                catch
                {
                    System.Console.WriteLine("Could not read the file " + assemblyPath);
                    return IntPtr.Zero;
                }
            }
            else
            {
                System.Console.WriteLine("No assembly specified");
                return IntPtr.Zero;
            }

            args.GetStringArg("-n", out @namespace);

            if (!args.GetStringArg("-c", out className))
            {
                System.Console.WriteLine("No class name specified");
                return IntPtr.Zero;
            }

            if (!args.GetStringArg("-m", out methodName))
            {
                System.Console.WriteLine("No method name specified");
                return IntPtr.Zero;
            }

            using (injector)
            {
                IntPtr remoteAssembly = IntPtr.Zero;

                try
                {
                    remoteAssembly = injector.Inject(assembly, @namespace, className, methodName);
                }
                catch (InjectorException ie)
                {
                    System.Console.WriteLine("Failed to inject assembly: " + ie);
                }
                catch (Exception exc)
                {
                    System.Console.WriteLine("Failed to inject assembly (unknown error): " + exc);
                }

                if (remoteAssembly == IntPtr.Zero)
                    return IntPtr.Zero;

                return remoteAssembly;

                System.Console.WriteLine($"{Path.GetFileName(assemblyPath)}: " + (injector.Is64Bit ? $"0x{remoteAssembly.ToInt64():X16}" : $"0x{remoteAssembly.ToInt32():X8}"));
            }
        }

        private static void Eject(Injector injector, CommandLineArguments args)
        {
            IntPtr assembly;
            string @namespace, className, methodName;

            if (args.GetIntArg("-a", out int intPtr))
            {
                assembly = (IntPtr)intPtr;
            }
            else if (args.GetLongArg("-a", out long longPtr))
            {
                assembly = (IntPtr)longPtr;
            }
            else
            {
                System.Console.WriteLine("No assembly pointer specified");
                return;
            }

            args.GetStringArg("-n", out @namespace);

            if (!args.GetStringArg("-c", out className))
            {
                System.Console.WriteLine("No class name specified");
                return;
            }

            if (!args.GetStringArg("-m", out methodName))
            {
                System.Console.WriteLine("No method name specified");
                return;
            }

            using (injector)
            {
                try
                {
                    injector.Eject(assembly, @namespace, className, methodName);
                    System.Console.WriteLine("Ejection successful");
                }
                catch (InjectorException ie)
                {
                    System.Console.WriteLine("Ejection failed: " + ie);
                }
                catch (Exception exc)
                {
                    System.Console.WriteLine("Ejection failed (unknown error): " + exc);
                }
            }
        }

        #region[AntiVirus PreTest]

        public static bool AntivirusInstalled()
        {
            // ref: https://stackoverflow.com/questions/1331887/detect-antivirus-on-windows-using-c-sharp

            #region[Pre-Windows 7]
            /* 
            try
            {
                bool defenderFlag = false;
                string wmipathstr = @"\\" + Environment.MachineName + @"\root\SecurityCenter";

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmipathstr, "SELECT * FROM AntivirusProduct");
                ManagementObjectCollection instances = searcher.Get();

                if (instances.Count > 0)
                {
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\DebugLog.txt", "AntiVirus Installed: True\r\n");

                    string installedAVs = "Installed AntiVirus':\r\n";
                    foreach (ManagementBaseObject av in instances)
                    {
                        //installedAVs += av.GetText(TextFormat.WmiDtd20) + "\r\n";
                        var AVInstalled = ((string)av.GetPropertyValue("pathToSignedProductExe")).Replace("//", "") + " " + (string)av.GetPropertyValue("pathToSignedReportingExe");
                        installedAVs += "   " + AVInstalled + "\r\n";

                        if (((string)av.GetPropertyValue("pathToSignedProductExe")).StartsWith("windowsdefender") && ((string)av.GetPropertyValue("pathToSignedReportingExe")).EndsWith("Windows Defender\\MsMpeng.exe")) { defenderFlag = true; }
                    }
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\DebugLog.txt", installedAVs + "\r\n");
                }
                else { File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\DebugLog.txt", "AntiVirus Installed: False\r\n"); }

                if (defenderFlag) { return false; } else { return instances.Count > 0; }
            }

            catch (Exception e)
            {
                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\DebugLog.txt", "Error Checking for AV: " + e.Message + "\r\n");
            }
            */
            #endregion

            try
            {
                List<string> avs = new List<string>();
                bool defenderFlag = false;
                string wmipathstr = @"\\" + Environment.MachineName + @"\root\SecurityCenter2";

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmipathstr, "SELECT * FROM AntivirusProduct");
                ManagementObjectCollection instances = searcher.Get();

                if (instances.Count > 0)
                {
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\DebugLog.txt", "AntiVirus Installed: True\r\n");

                    string installedAVs = "Installed AntiVirus':\r\n";
                    foreach (ManagementBaseObject av in instances)
                    {
                        //installedAVs += av.GetText(TextFormat.WmiDtd20) + "\r\n";
                        var AVInstalled = ((string)av.GetPropertyValue("pathToSignedProductExe")).Replace("//", "") + " " + (string)av.GetPropertyValue("pathToSignedReportingExe");
                        installedAVs += "   " + AVInstalled + "\r\n";
                        avs.Add(AVInstalled.ToLower());

                        // Comment here to test
                        //if (((string)av.GetPropertyValue("pathToSignedProductExe")).StartsWith("windowsdefender") && ((string)av.GetPropertyValue("pathToSignedReportingExe")).EndsWith("Windows Defender\\MsMpeng.exe")) { defenderFlag = true; }
                    }
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\DebugLog.txt", installedAVs + "\r\n");
                }
                else { File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\DebugLog.txt", "AntiVirus Installed: False\r\n"); }

                foreach (Process p in Process.GetProcesses())
                {
                    foreach (var detectedAV in avs)
                    {
                        if (detectedAV.EndsWith(p.ProcessName.ToLower() + ".exe"))
                        {
                            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\DebugLog.txt", "AntiVirus Running: " + detectedAV + "\r\n");
                        }
                    }
                }

                if (defenderFlag) { return false; } else { return instances.Count > 0; }
            }

            catch (Exception e)
            {
                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\DebugLog.txt", "Error Checking for AV: " + e.Message + "\r\n");
            }

            return false;
        }


        #endregion
    }
}
