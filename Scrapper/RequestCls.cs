using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPE
{
    public class AuxData
    {
        public string pid { get; set; }
        public string ptype { get; set; }
        public string input_uniqueness_key { get; set; }
        public string pos { get; set; }
        public int los { get; set; }
        public int assignment_id { get; set; }
        public string shop_currency_id { get; set; }
        public string region { get; set; }
        public string access_level { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public int reshopping_count { get; set; }
    }

    public class Data
    {
        public string currency { get; set; }
        public string drop_off_date { get; set; }
        public string drop_off_time { get; set; }
        public string fn_drop_off_city { get; set; }
        public string fn_drop_off_country { get; set; }
        public string fn_drop_off_location_code { get; set; }
        public string fn_pick_up_city { get; set; }
        public string fn_pick_up_country { get; set; }
        public string fn_pick_up_location_code { get; set; }
        public string pick_up_date { get; set; }
        public string pick_up_time { get; set; }
        public string pos { get; set; }
        public string provider_drop_off_city { get; set; }
        public string provider_drop_off_country { get; set; }
        public Dictionary<string, string> provider_drop_off_location_additional_fields { get; set; }
        public string provider_drop_off_location_code { get; set; }
        public string provider_drop_off_location_name { get; set; }
        public string provider_pick_up_city { get; set; }
        public string provider_pick_up_country { get; set; }
        public Dictionary<string, string> provider_pick_up_location_additional_fields { get; set; }
        public string provider_pick_up_location_code { get; set; }
        public string provider_pick_up_location_name { get; set; }
        public string scan_date { get; set; }
    }

    public class Root
    {
        public string scan_id { get; set; }
        public string scan_obj_id { get; set; }
        public string provider { get; set; }
        public string system_type { get; set; }
        public string scan_method { get; set; }
        public string level { get; set; }
        public string scan_package_name { get; set; }
        public string bot_id { get; set; }
        public string scan_package_variation { get; set; }
        public string scan_package_uri { get; set; }
        public string provider_scan_id { get; set; }
        public int output_version { get; set; }
        public string customer_name { get; set; }
        public string consumer_name { get; set; }
        public string scheduled_at { get; set; }
        public int scan_phase { get; set; }
        public string state { get; set; }
        public string last_timestamp { get; set; }
        public TraceShop trace_shop { get; set; }
        public Data data { get; set; }
        public AuxData aux_data { get; set; }
        public string task_id { get; set; }
    }

    public class TraceShop
    {
        public string trace_shop_uniqueness { get; set; }
        public string trace_customer_cluster { get; set; }
        public string trace_provider_name { get; set; }
        public string trace_scan_level { get; set; }
        public string trace_access_level { get; set; }
        public string trace_language_id { get; set; }
    }


}
