using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace EltApplication
{
    class JsonOperation
    {
        //out Json字符串操作
        public String OutToJson(DataModel dom)
        {
            return Thzfxg(JsonConvert.SerializeObject(dom).ToString());//序列化
        }
        public DataModel OutToModel(String strJson)
        {
            return JsonConvert.DeserializeObject<DataModel>(strJson);//反序列化
        }       
        //fast_ls_in Json字符串操作
        public String FastLsInToJson(DataFastLsToInModel dom)
        {
            return Thzfxg(JsonConvert.SerializeObject(dom).ToString());//序列化
        }
        public DataFastLsToInModel FastLsInToModel(String strJson)
        {
            return JsonConvert.DeserializeObject<DataFastLsToInModel>(strJson);//反序列化
        }
        //fast_ls Json字符串操作
        public String FastLsToJson(DataFastLsModel dom)
        {
            return Thzfxg(JsonConvert.SerializeObject(dom).ToString());//序列化
        }
        public DataFastLsModel FastLsToModel(String strJson)
        {
            return JsonConvert.DeserializeObject<DataFastLsModel>(strJson);//反序列化
        }        
        //silob Json字符串操作
        public String SILOBToJson(DataSILOB ds)
        {
            return Thzfxg(JsonConvert.SerializeObject(ds).ToString());//序列化
        }
        public DataSILOB SILOBToModel(String strJson)
        {
            return JsonConvert.DeserializeObject<DataSILOB>(strJson);//反序列化
        }
        //silobOut Json字符串操作
        public String SILOBOutToJson(DataSILOBOut ds)
        {
            return Thzfxg(JsonConvert.SerializeObject(ds).ToString());//序列化
        }
        public DataSILOBOut SILOBOutToModel(String strJson)
        {
            return JsonConvert.DeserializeObject<DataSILOBOut>(strJson);//反序列化
        }
        //silobmOut Json字符串操作
        public String SILOBMOutToJson(DataSILOMOut ds)
        {
            return Thzfxg(JsonConvert.SerializeObject(ds));//序列化
        }
        public DataSILOMOut SILOBMOutToModel(String strJson)
        {
            return JsonConvert.DeserializeObject<DataSILOMOut>(strJson);//反序列化
        }
        //silob Json字符串操作
        public String SILOBMToJson(DataSILOM ds)
        {
            return Thzfxg(JsonConvert.SerializeObject(ds));//序列化
        }
        public DataSILOM SILOBMToModel(String strJson)
        {
            return JsonConvert.DeserializeObject<DataSILOM>(strJson);//反序列化
        }

        //替换字符\
        private String Thzfxg(String strItem)
        {
            return strItem.Replace("\\","");
        }
    }
}
