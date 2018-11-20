using Mouse_Click_Simulator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PyrRet
{
    class Program
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string sClass, string sWindow);
        private static Int32 mainHandle;
        private static Int32 settingsHandle;
        private const Int32 settingsButtonCords = 0x00100017;
        private const Int32 applyButtonCords = 0x02400210;

        static void FirstClick()
        {
            Win32.SendMessage(mainHandle, Win32.WM_LBUTTONDOWN, 0x00000001, settingsButtonCords);
            Win32.SendMessage(mainHandle, Win32.WM_LBUTTONUP, 0x00000000, settingsButtonCords);
        }
        static void LastClick()
        {
            Win32.SendMessage(settingsHandle, Win32.WM_LBUTTONDOWN, 0x00000001, applyButtonCords);
            Win32.SendMessage(settingsHandle, Win32.WM_LBUTTONUP, 0x00000000, applyButtonCords);
        }
        static string GetLogFileName()
        {
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();
            string day = DateTime.Now.Day.ToString();
            if (month.Length == 1) month = "0" + month;
            if (day.Length == 1) month = "0" + day;
            string filename = year + month + day + "_PyrRec.log";
            return filename;
        }
        static void HandleRefresh()
        {
            while (true)
            {
                mainHandle = Win32.FindWindow("QWidget", "PyrRec");
                settingsHandle = Win32.FindWindow("QWidget", "Настройки");
                Thread.Sleep(5000);
            }
        }
        static void PyrRecUpdate()
        {
            while (true)
            {           
                var lastLineOfLog = File.ReadLines(GetLogFileName()).Last();
                if (lastLineOfLog.IndexOf("error") != -1 && lastLineOfLog.IndexOf("192.168.1.4") != -1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[PYRREC ERROR] " + "Connection refused");
                    Console.ForegroundColor = ConsoleColor.White;
                    FirstClick();
                    Thread.Sleep(2000);
                    LastClick();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("[CONNECT] " + DateTime.Now);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Thread.Sleep(2000);
            }
        }
        static void Main(string[] args)
        {
            if (File.Exists("pyrrec.exe"))
            {
                if (File.Exists(GetLogFileName()))
                {
                    Thread handleRefresherThread = new Thread(HandleRefresh);
                    handleRefresherThread.Start();
                    Thread pyrRecUpdateThread = new Thread(PyrRecUpdate);
                    pyrRecUpdateThread.Start();
                } else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[IO ERROR] " + "PyrRec log file not found");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadKey();
                }
            } else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[IO ERROR] " + "Please, move program to PyrRec folder");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();
            }
            
        }
    }
}
