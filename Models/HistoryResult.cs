using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quant_BackTest_Backend.Models {
    public class HistoryResult {

        public int backtest_id { get; set; }
        //public int strategy_id { get; set; }
        //public int data_id { get; set; }
        public string sy { get; set; }
        public string nsy { get; set; }
        public string hc { get; set; }
        public string xp { get; set; }
        public string report_path { get; set; }
        public string time { get; set; }

        public HistoryResult(int id, string sy, string nsy, string hc, string xp, string report_path, string time) {
            this.backtest_id = id;
            this.sy = sy;
            this.nsy = nsy;
            this.hc = hc;
            this.xp = xp;
            this.report_path = report_path;
            this.time = time;
        }
    }
}