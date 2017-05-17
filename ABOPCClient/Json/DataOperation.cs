using System;

namespace EltApplication
{
    class DataOperation
    {
        /*
         * 处理SILOB转化
        */
        //SILOB字符串转对象
        public DataSILOB SilobInToSz(String strData)
        {
            JsonOperation jo = new JsonOperation();
            DataSILOB ds = jo.SILOBToModel(strData);

            return ds;
        }
        //SILOB数组转字符
        public String SilobInToZf(DataSILOB strDatas)
        {
            JsonOperation jo = new JsonOperation();
            return jo.SILOBToJson(strDatas);
        }
        /*
        * 处理SILOM转化
       */
        //SILOM字符串转对象
        public DataSILOM SilomInToSz(String strData)
        {
            JsonOperation jo = new JsonOperation();
            DataSILOM dsm = jo.SILOBMToModel(strData);
            return dsm;
        }
        //SILOB对象转字符
        public String SilomInToZf(DataSILOM strDatas)
        {
            JsonOperation jo = new JsonOperation();
            return jo.SILOBMToJson(strDatas);
        }
        /*
         * 处理FASTLS转化
         */
        //FASTLS字符串转对象
        public DataFastLsModel FaseLsToSz(String strData)
        {
            JsonOperation jo = new JsonOperation();
            DataFastLsModel dflm = jo.FastLsToModel(strData);
            return dflm;
        }
        //FASTLS对象转字符
        public String FaseLsToZf(DataFastLsModel strDatas)
        {
            DataFastLsModel dflm = new DataFastLsModel();
            JsonOperation jo = new JsonOperation();
            return jo.FastLsToJson(dflm);
        }
        /*
         * 处理FASTLSIN转化
         */
        //FASTLSIN字符串转对象
        public DataFastLsToInModel FaseLsInToSz(String strData)
        {
            JsonOperation jo = new JsonOperation();
            DataFastLsToInModel dflm = jo.FastLsInToModel(strData);

            return dflm;
        }
        //FASTLSIN对象转字符
        public String FaseLsInToZf(DataFastLsToInModel strDatas)
        {
            JsonOperation jo = new JsonOperation();
            return jo.FastLsInToJson(strDatas);
        }
    }
}
