using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quant_BackTest_Backend.Models {
    public class StrategyResult {

        public int strategy_id {get; set;}
        public string name { get; set; }
        public string create_time { get; set; }
        public string strategy_hash { get; set; }
        public string last_modify_time { get; set; }
        public int times { get; set; }

        public StrategyResult(int id, string name, string ctime, string hash, string mtime, int times) {
            this.strategy_id = id;
            this.name = name;
            this.create_time = ctime;
            this.strategy_hash = hash;
            this.last_modify_time = mtime;
            this.times = times;
        }
    }
}