using System;
using System.Collections.Generic;
using System.Text;

using System;
using System.Data;
using OPCAutomation;
using System.Data.OracleClient;
using System.Diagnostics;
using System.Collections;


namespace ABOPCClient
{
    class SetOPCItem
    {
        
        #region 快速装车站点配置
        public ArrayList ItemFAST_LS()
        {
            ArrayList arlItem = new ArrayList();
            //
            arlItem.Add("MHA");
            arlItem.Add("MHB");
            arlItem.Add("MHC");
            arlItem.Add("MHD");
            //
            arlItem.Add("PLC3!%MF1224");
            arlItem.Add("PLC3!%MF1226");
            arlItem.Add("PLC3!%MF1228");
            arlItem.Add("PLC3!%MF1246");
            return arlItem;
        }
        #endregion 快速装车站点配置

        #region 大块筒仓点配置
        public ArrayList ItemSILO_B()
        {
            ArrayList arlItem = new ArrayList();
            //
            arlItem.Add("Channel_4.Device_6.Bool_0");
            arlItem.Add("Channel_4.Device_6.Bool_15");
            arlItem.Add("Channel_4.Device_6.Bool_0");
            arlItem.Add("Channel_4.Device_6.Bool_15");
            //
            arlItem.Add("PLC3!%MF1224");
            arlItem.Add("PLC3!%MF1226");
            arlItem.Add("PLC3!%MF1228");
            arlItem.Add("PLC3!%MF1246");
            return arlItem;
        }
        #endregion 大块筒仓点配置

        #region 中块筒仓点配置
        public ArrayList ItemSILO_M()
        {
            ArrayList arlItem = new ArrayList();
            //
            arlItem.Add("MHA");
            arlItem.Add("MHB");
            arlItem.Add("MHC");
            arlItem.Add("MHD");
            //
            arlItem.Add("PLC3!%MF1224");
            arlItem.Add("PLC3!%MF1226");
            arlItem.Add("PLC3!%MF1228");
            arlItem.Add("PLC3!%MF1246");
            return arlItem;
        }
        #endregion 中块筒仓点配置

    }
}
