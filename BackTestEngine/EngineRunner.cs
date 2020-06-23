using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Quant_BackTest_Backend.BackTestEngine {
    public class EngineRunner {
        //调用python核心代码
        public void RunPythonScript(string sArgName, string args = "", params string[] teps) {
            Process p = new Process();
            // string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + sArgName;// 获得python文件的绝对路径（将文件放在c#的debug文件夹中可以这样操作）
            string path = @"python C:\Users\leon\NET_FINAL\strategy_backtest\examples\" + sArgName;//(因为我没放debug下，所以直接写的绝对路径,替换掉上面的路径了)
                                                                                                   //p.StartInfo.FileName = @"D:\Python\envs\python3\python.exe";//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可

            //设置要启动的应用程序
            p.StartInfo.FileName = @"cmd.exe";

            /*
            string sArguments = path;
            foreach (string sigstr in teps) {
                sArguments += " " + sigstr;//传递参数
            }

            sArguments += " " + args;
            */

            //p.StartInfo.Arguments = sArguments;
            //p.StartInfo.Arguments = path;

            //是否使用操作系统shell启动
            p.StartInfo.UseShellExecute = false;

            // 接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardInput = true;

            //输出信息
            p.StartInfo.RedirectStandardOutput = true;

            //输出错误
            p.StartInfo.RedirectStandardError = true;

            //不显示程序窗口
            p.StartInfo.CreateNoWindow = true;

            p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);

            //启动程序
            p.Start();
            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(path);
            p.StandardInput.AutoFlush = true;

            p.BeginOutputReadLine();
            //Console.ReadLine();
            p.WaitForExit(10000);
        }

        //输出打印的信息
        static void p_OutputDataReceived(object sender, DataReceivedEventArgs e) {
            if (!string.IsNullOrEmpty(e.Data)) {
                AppendText(e.Data + Environment.NewLine);
            }
        }
        public delegate void AppendTextCallback(string text);
        public static void AppendText(string text) {
            Console.WriteLine(text);     //此处在控制台输出.py文件print的结果

        }
    }
}