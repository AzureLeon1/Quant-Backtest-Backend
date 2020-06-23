using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Quant_BackTest_Backend.BackTestEngine {
    public class EngineUtils {

        // file_name参数应包含.py后缀
        public bool saveFile(string code, string path, string file_name) {
            try {
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                string full_path = path + @"\" + file_name;
                FileStream fs = File.Create(full_path);
                fs.Close();
                StreamWriter sw = new StreamWriter(full_path);
                sw.Write(code);
                sw.Flush();
                sw.Close();
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

        public bool copyFile(string origin, string target) {
            try {
                if (!File.Exists(origin)) {
                    return false;
                }
                if (File.Exists(target)) {
                    File.Delete(target);
                }
                File.Copy(origin, target);
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }
    }
}