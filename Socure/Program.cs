using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using static System.Console;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WriteLine("Hello World!");
            var setting = Reg.ReadCurrentUser("Path");

            Reg.AppendNewPath("c:\\test");
            //Reg.PrintAllPath(setting);
        }
    }

    public static class Reg
    {
        public static string ReadCurrentUser(string key)
        {
            try
            {
                using (var registryKey = Registry.CurrentUser.OpenSubKey("Environment"))
                {
                    var o = registryKey?.GetValue(key);
                    if (o != null) { return o as string; }
                }
            }
            catch (Exception ex)
            {
                Out.WriteLine(ex.ToString());
                Out.WriteLine(ex.StackTrace);
            }

            return string.Empty;
        }

        public static IEnumerable<string> PrintAllPath(string value)
        {
            var temp = value.Split(';');
            foreach (var s in temp)
            {
                Out.WriteLine(s);
                yield return s;
            }
        }

        public static bool CheckPathExist(string path)
        {
            if (File.Exists(path) || Directory.Exists(path)) { return true; }

            Out.WriteLine($"Path : {path} not exist in system");
            return false;
        }

        public static bool AppendNewPath(string path)
        {
            if (!CheckPathExist(path)) { return false; }

            try
            {
                using (var registryKey = Registry.CurrentUser.OpenSubKey("Environment", true))
                {
                    var raw = registryKey?.GetValue("Path") as string;

                    var temp = PrintAllPath(raw);
                    if (!temp.Contains(path))
                    {
                        raw += $";{path}";

                        try
                        {
                            registryKey.SetValue("Path", raw);
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            Out.WriteLine("Unauthorized Access");
                            throw;
                        }

                        return true;
                    }

                    Out.WriteLine("Path Exist");
                }
            }
            catch (Exception ex)
            {
                Out.WriteLine(ex.ToString());
                Out.WriteLine(ex.StackTrace);
            }

            return false;
        }
    }
}
