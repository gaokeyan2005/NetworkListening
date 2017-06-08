using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace NetworkListening
{
    public partial class FrmNetwork : Form
    {
        private static System.Timers.Timer timeServer;//定时器监听

        public FrmNetwork()
        {
            InitializeComponent();

            //RunNetworkListening();
            //cmd();
            WindowState = FormWindowState.Minimized;
            //timeServer = new System.Timers.Timer();
            //timeServer.Interval = 300000;  //设置计时器事件间隔执行时间
            //timeServer.Elapsed += new System.Timers.ElapsedEventHandler(timeServer_Elapsed);
            //timeServer.Enabled = true;
            NetworkListeningLog("tracert", tracert("www.sohu.com"));

            CreateZipFile((Application.StartupPath + @"\NetworkListeningLog\" + DateTime.Now.ToString("yyyyMMdd") + ComputerInfo.ComputerName), (Application.StartupPath + @"\NetworkListeningLog\" + DateTime.Now.ToString("yyyyMMdd") + ComputerInfo.ComputerName + ".zip"));
        }
        private  void timeServer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            RunNetworkListening();
        }
        private void btnBegin_Click(object sender, EventArgs e)
        {
            NetworkListeningLog("baidu", displayReply("www.baidu.com"));
            NetworkListeningLog("yokitalk", displayReply("api.chaojiwaijiao.com"));
            NetworkListeningLog("netease.im", displayReply("103.211.192.67"));
            NetworkListeningLog("netease.video", displayReply("43.230.90.69"));
        }
        private void RunNetworkListening()
        {
            int iTime = 10;
            for(int i=0;i< iTime;i++)
            {
                NetworkListeningLog("baidu", displayReply("www.baidu.com"));
                NetworkListeningLog("yokitalk", displayReply("api.chaojiwaijiao.com"));
                NetworkListeningLog("netease.im", displayReply("103.211.192.67"));
                NetworkListeningLog("netease.video", displayReply("43.230.90.69"));
            }
        }
        private string displayReply(string strDns) //显示结果
        {
            try
            {
                Ping p1 = new Ping(); //只是演示，没有做错误处理
                
                PingReply reply = p1.Send(strDns);

                StringBuilder sbuilder;

                if (reply.Status == IPStatus.Success)
                {
                    sbuilder = new StringBuilder();
                    //sbuilder.Append(string.Format("Address: {0} ", reply.Address.ToString()));
                    //sbuilder.Append(string.Format("RoundTrip time: {0} ", reply.RoundtripTime));
                    //sbuilder.Append(string.Format("Time to live: {0} ", reply.Options.Ttl));
                    //sbuilder.Append(string.Format("Don't fragment: {0} ", reply.Options.DontFragment));
                    //sbuilder.Append(string.Format("Buffer size: {0} ", reply.Buffer.Length));

                    sbuilder.Append(string.Format("\"Domain name\": \"{0}\",\"Address\": \"{1}\",\"Revovery Date\": \"{2}\",\"Revovery Time\": \"{3}\",\"RoundTrip time\":{4},\"Time to live\": \"{5}\",\"Buffer size\":{6},\"Computer name\": \"{7}\"",
                        strDns, reply.Address.ToString(), DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), reply.RoundtripTime, reply.Options.Ttl, reply.Buffer.Length, ComputerInfo.ComputerName));
                    return "{" + sbuilder.ToString() + "},";
                    //Console.WriteLine("{"+sbuilder.ToString()+"},");
                }
                else if (reply.Status == IPStatus.TimedOut)
                {
                    sbuilder = new StringBuilder();
                    sbuilder.Append(string.Format("\"Domain name\": \"{0}\",\"Address\": \"{1}\",\"Revovery Date\": \"{2}\",\"Revovery Time\": \"{3}\",\"RoundTrip time\":{4},\"Time to live\": \"{5}\",\"Buffer size\":{6},\"Computer name\": \"{7}\"",
                        strDns, "", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), 0, 0, 0, ComputerInfo.ComputerName));
                    return "{" + sbuilder.ToString() + "},";
                    //Console.WriteLine("超时");
                }
                else
                {
                    sbuilder = new StringBuilder();
                    sbuilder.Append(string.Format("\"Domain name\": \"{0}\",\"Address\": \"{1}\",\"Revovery Date\": \"{2}\",\"Revovery Time\": \"{3}\",\"RoundTrip time\":{4},\"Time to live\": \"{5}\",\"Buffer size\":{6},\"Computer name\": \"{7}\"",
                        strDns, "", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), -1, 0, 0, ComputerInfo.ComputerName));
                    return "{" + sbuilder.ToString() + "},";
                    //Console.WriteLine("失败");
                }
            }
            catch (Exception ex)
            {
                return "{error:" + ex.Message + "}";
            }
        }

        private static void NetworkListeningLog(string dnsType,string str)
        {
            if (!Directory.Exists(Application.StartupPath + @"\NetworkListeningLog\"+DateTime.Now.ToString("yyyyMMdd")+ComputerInfo.ComputerName))
            {
                Directory.CreateDirectory(Application.StartupPath + @"\NetworkListeningLog\" + DateTime.Now.ToString("yyyyMMdd") + ComputerInfo.ComputerName);
            }

            using (StreamWriter sw = new StreamWriter(Application.StartupPath + @"\NetworkListeningLog\" + DateTime.Now.ToString("yyyyMMdd") + ComputerInfo.ComputerName+@"\" + DateTime.Now.ToString("yyyyMMdd")+ dnsType + ".txt", true))
            {
                sw.WriteLine(str);
                sw.Close();
            }
        }

        #region 界面样式菜单处理
        private void FrmNetwork_Load(object sender, EventArgs e)
        {
            
            notifyIcon1.ShowBalloonTip(5, "Title", "The network monitoring service, close abnormal error, do not touch！", ToolTipIcon.Info);
            this.Hide();
        }

        private void FrmNetwork_FormClosing(object sender, FormClosingEventArgs e)
        {
            //注意判断关闭事件Reason来源于窗体按钮，否则用菜单退出时无法退出!
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;    //取消"关闭窗口"事件
                this.WindowState = FormWindowState.Minimized;    //使关闭时窗口向右下角缩小的效果
                notifyIcon1.Visible = true;
                this.Hide();
                return;
            }
        }

        private void FrmNetwork_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)    //最小化到系统托盘
            {
                notifyIcon1.Visible = true;    //显示托盘图标
                this.Hide();    //隐藏窗口
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon1.Visible = false;
            this.Show();
            WindowState = FormWindowState.Normal;
            this.Focus();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Determination of exit?", "Operation tips", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                this.Close();
                this.Dispose();
                Application.Exit();
                //notifyIcon1.Visible = true;    //显示托盘图标
                //this.Hide();    //隐藏窗口
            }
        }

        private void MainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            this.Show();
            WindowState = FormWindowState.Normal;
            this.Focus();
        }
        #endregion
        private string tracert(string strCmd)
        {
            strCmd = "tracert " + strCmd;
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(strCmd + "&exit");

            p.StandardInput.AutoFlush = true;
            //p.StandardInput.WriteLine("exit");
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令

            //获取cmd窗口的输出信息
            string output = p.StandardOutput.ReadToEnd();

            //StreamReader reader = p.StandardOutput;
            //string line=reader.ReadLine();
            //while (!reader.EndOfStream)
            //{
            //    str += line + "  ";
            //    line = reader.ReadLine();
            //}

            p.WaitForExit();//等待程序执行完退出进程
            p.Close();
            int index = output.IndexOf("tracert");
            output = output.Substring(index, output.Length - index);
            output = "--------[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]begin-------\r\n" + output 
                   + "--------[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]end---------\r\n";
            return output;
        }
        /// <summary>
        /// 文件ZIP压缩
        /// </summary>
        /// <param name="filesPath"></param>
        /// <param name="zipFilePath"></param>
        private static void CreateZipFile(string filesPath, string zipFilePath)
        {
            if (!Directory.Exists(filesPath))
            {
                Console.WriteLine("Cannot find directory '{0}'", filesPath);
                return;
            }
            try
            {
                string[] filenames = Directory.GetFiles(filesPath);
                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
                {
                    s.SetLevel(9); // 压缩级别 0-9
                    //s.Password = "123"; //Zip压缩文件密码
                    byte[] buffer = new byte[4096]; //缓冲区大小
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during processing {0}", ex);
            }
        }
        /// <summary>
        /// ZIP解压缩
        /// </summary>
        /// <param name="zipFilePath"></param>
        private static void UnZipFile(string zipFilePath)
        {
            if (!File.Exists(zipFilePath))
            {
                Console.WriteLine("Cannot find file '{0}'", zipFilePath);
                return;
            }

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    Console.WriteLine(theEntry.Name);

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    // create directory
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(theEntry.Name))
                        {

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
