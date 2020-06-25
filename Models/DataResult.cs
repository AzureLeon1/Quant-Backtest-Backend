using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quant_BackTest_Backend.Models {
    public class DataResult {
        public int data_id { get; set; }
        public int data_type { get; set; }
        public string data_name { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }
        public string data_path { get; set; }

        public DataResult(int id, int type, string data_name, string start_time, string end_time, string data_path) {
            this.data_id = id;
            this.data_type = type;
            this.data_name = data_name;
            this.start_time = start_time;
            this.end_time = end_time;
            this.data_path = data_path;
        }
    }
}