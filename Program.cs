﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace NetworkListening
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ComputerInfo.ComputerName = Dns.GetHostName();
            Process instance = RunningInstance();

            if (instance == null)
            {
                Application.Run(new FrmNetwork());
                //Application.Run(new Form1());
            }
            else
            {
                HandleRunningInstance(instance);
            }
            
        }
        /// <summary>
        /// 获取正在运行的实例，没有运行的实例返回null;
        /// </summary>
        public static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            foreach (Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "//") == current.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 显示已运行的程序。
        /// </summary>
        public static void HandleRunningInstance(Process instance)
        {
            MessageBox.Show("The monitor service is already running! Please double-click the display screen in the system tray...", "Operation tips", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //ShowWindowAsync(instance.MainWindowHandle, SW_MAXIMIZE); //显示，可以注释掉
            //SetForegroundWindow(instance.MainWindowHandle);            //放到前端
        }
    }
    
}
