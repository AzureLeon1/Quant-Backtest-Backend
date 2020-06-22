using System.Web;
using System.Web.Mvc;

namespace Quant_BackTest_Backend {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
