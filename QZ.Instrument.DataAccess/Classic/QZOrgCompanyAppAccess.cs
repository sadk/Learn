﻿/* Copyright (c) 2016 Qianzhan Information Lim. Co. All rights reserved.
 * Contributor: Sha Jianjian
 * 2016
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using QZ.Instrument.Model;
using QZ.Foundation.Utility;
using QZ.Instrument.Utility;

namespace QZ.Instrument.DataAccess
{
    public class QZOrgCompanyAppAccess : AccessBase, IDisposable
    {
        /// <summary>
        /// AppOrgCompanyLog分表数量最大值，超过此值，则每次生成一个新的分表时最同时删除一个最老的分表
        /// </summary>
        private const int Log_TableCountCeiling = 365;
        #region Initlize DataAccess
        private string _key_0;
        private string _key_1;
        
        /// <summary>
        /// 数据库连接
        /// </summary>
        Database _db_0;
        public Database Db_0
        {
            get
            {
                if (_db_0 == null)
                    _db_0 = DatabaseFactory.CreateDatabase(_key_0);
                return _db_0;
            }
        }
        Database _db_1;
        public Database Db_1
        {
            get
            {
                if (_key_1 == null)
                    throw new ArgumentNullException("_key_1");
                if(_db_1 == null)
                    _db_1 = DatabaseFactory.CreateDatabase(_key_1);
                return _db_1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connName"></param>
        public QZOrgCompanyAppAccess(string key)
        {
            this._key_0 = key;
        }

        public QZOrgCompanyAppAccess Set_Key_1(string key)
        {
            this._key_1 = key;
            return this;
        }
        #endregion

        #region IDisposable 接口实现
        /// <summary>
        /// 终结器, 调用虚拟的Dispose方法
        /// </summary>
        ~QZOrgCompanyAppAccess()
        {
            Dispose(false);
        }

        /// <summary>
        /// 调用虚拟的Dispose方法, 禁止Finalization（终结操作）
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 虚拟的Dispose方法
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

        }
        #endregion

        private void createSysLog(string table)
        {
            
            DbCommand dbCommandWrapper = Db_0.GetSqlStringCommand($"SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('{table}') AND type = 'U'");

            object rtn = Db_0.ExecuteScalar(dbCommandWrapper);
            if (rtn == null)//创建表
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(string.Format(@"create table {0} (
                    [err_id] [int] IDENTITY(1,1) NOT NULL,
                    [err_guid] [varchar](32) NOT NULL,
	                [err_type] [nvarchar](20) NOT NULL,
	                [err_message] [nvarchar](500) NOT NULL,
	                [err_source] [varchar](max) NOT NULL,
	                [err_stack] [varchar](max) NOT NULL,
	                [err_url] [varchar](250) NOT NULL,
	                [err_referrer] [varchar](250) NOT NULL,
                    [err_ip] [varchar](20) NOT NULL,
	                [err_user] [varchar](30) NOT NULL,
	                [err_time] [datetime] NOT NULL,
                    [err_totalMilliseconds] [int] NOT NULL,", table));

                sb.Append(string.Format("constraint PK_{0} primary key (err_id))", table));
                //sb.Append(string.Format("create unique index IX_ueo_guid on {0} (ueo_guid ASC)", tableName));
                //sb.Append(string.Format("create index IX_uol_user on {0} (uol_user ASC)", tableName));

                dbCommandWrapper = Db_0.GetSqlStringCommand(sb.ToString());
                Db_0.ExecuteNonQuery(dbCommandWrapper);
            }
        }

        public int ErrorLog_Insert(LogError obj)
        {
            var table = $"tbSysLog_{DateTime.Now.ToString("yyyyMMdd")}";
            this.createSysLog(table);

            string SQL = string.Format(@"INSERT INTO {0}(
                    err_type
                   ,err_guid
                   ,err_message
                   ,err_source
                   ,err_stack
                   ,err_url
                   ,err_referrer
                   ,err_ip
                   ,err_user
                   ,err_time
                   ,err_totalMilliseconds
                ) VALUES(
	                @err_type
                   ,@err_guid
                   ,@err_message
                   ,@err_source
                   ,@err_stack
                   ,@err_url
                   ,@err_referrer
                   ,@err_ip
                   ,@err_user
                   ,@err_time
                   ,@err_totalMilliseconds
                ); set @err_id=@@identity", table);

            DbCommand dbCommandWrapper = Db_0.GetSqlStringCommand(SQL);

            Db_0.AddOutParameter(dbCommandWrapper, "@err_id", DbType.Int32, 4);
            Db_0.AddInParameter(dbCommandWrapper, "@err_type", DbType.String, obj.err_type);
            Db_0.AddInParameter(dbCommandWrapper, "@err_guid", DbType.String, obj.err_guid);
            Db_0.AddInParameter(dbCommandWrapper, "@err_message", DbType.String, obj.err_message);
            Db_0.AddInParameter(dbCommandWrapper, "@err_source", DbType.String, obj.err_source);
            Db_0.AddInParameter(dbCommandWrapper, "@err_stack", DbType.String, obj.err_stack);
            Db_0.AddInParameter(dbCommandWrapper, "@err_url", DbType.String, obj.err_url);
            Db_0.AddInParameter(dbCommandWrapper, "@err_referrer", DbType.String, obj.err_referrer);
            Db_0.AddInParameter(dbCommandWrapper, "@err_ip", DbType.String, obj.err_ip);
            Db_0.AddInParameter(dbCommandWrapper, "@err_user", DbType.String, obj.err_user);
            Db_0.AddInParameter(dbCommandWrapper, "@err_time", DbType.DateTime, obj.err_time);
            Db_0.AddInParameter(dbCommandWrapper, "@err_totalMilliseconds", DbType.Int32, obj.err_totalMilliseconds);

            Db_0.ExecuteNonQuery(dbCommandWrapper);
            int err_id = (int)dbCommandWrapper.Parameters["@err_id"].Value;

            return err_id;
            /*using(DataAccess access = new DataAccess())
            {
                return access.tbSearchLog_Insert( obj );
            }*/
        }
        public string District_Code_Get(string oc_area, string sp_Name)
        {
            var cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@dc_a_code", DbType.String, oc_area);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    return reader["dc_d_code"].ToString();
                }
            }
            return "";

        }
        private void SearchHistory_Sp_Map(Company c, string sp_Name, out DbCommand cmd)
        {
            cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(cmd, "@sh_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@sh_oc_name", DbType.String, c.oc_name.To_Sql_Safe());
            Db_0.AddInParameter(cmd, "@sh_oc_area", DbType.String, c.oc_area.To_Sql_Safe());
            Db_0.AddInParameter(cmd, "@sh_od_regMLower", DbType.Currency, string.IsNullOrEmpty(c.oc_reg_capital_floor)? 0 : decimal.Parse(c.oc_reg_capital_floor));
            Db_0.AddInParameter(cmd, "@sh_od_regMUpper", DbType.Currency, string.IsNullOrEmpty(c.oc_reg_capital_ceiling) ? 0: decimal.Parse(c.oc_reg_capital_ceiling));
            Db_0.AddInParameter(cmd, "@sh_od_regType", DbType.String, c.oc_reg_type.To_Sql_Safe());
            Db_0.AddInParameter(cmd, "@sh_oc_code", DbType.String, c.oc_code.To_Sql_Safe());
            Db_0.AddInParameter(cmd, "@sh_oc_number", DbType.String, c.oc_number.To_Sql_Safe());
            Db_0.AddInParameter(cmd, "@sh_od_faRen", DbType.String, c.oc_art_person.To_Sql_Safe());
            Db_0.AddInParameter(cmd, "@sh_od_gd", DbType.String, c.oc_stock_holder.To_Sql_Safe());
            Db_0.AddInParameter(cmd, "@sh_od_bussinessDes", DbType.String, c.oc_business.To_Sql_Safe());
            Db_0.AddInParameter(cmd, "@sh_u_name", DbType.String, c.u_name.To_Sql_Safe());
            Db_0.AddInParameter(cmd, "@sh_u_uid", DbType.Int32, c.u_id.ToInt());
            Db_0.AddInParameter(cmd, "@sh_date", DbType.DateTime, DateTime.Now);
            Db_0.AddInParameter(cmd, "@sh_searchType", DbType.Int32, (int)c.q_type + 1);
            Db_0.AddInParameter(cmd, "@sh_od_ext", DbType.String, c.oc_ext.To_Sql_Safe());
            Db_0.AddInParameter(cmd, "@sh_od_orderBy", DbType.Int32, (int)c.oc_sort);
            Db_0.AddInParameter(cmd, "@sh_oc_address", DbType.String, c.oc_addr.To_Sql_Safe());
            Db_0.AddInParameter(cmd, "@sh_oc_areaName", DbType.String, string.Empty);  //todo: user has no way to specify the area name 
        }

        public int SearchHistory_Slave_Insert(Company c, string sp_Name)
        {
            DbCommand cmd;
            SearchHistory_Sp_Map(c, sp_Name, out cmd);
            Db_0.AddInParameter(cmd, "@tableName", DbType.String, $"SearchHistory_{c.u_id.ToInt() % 256:D3}");

            int _returnValue = Db_0.ExecuteNonQuery(cmd);
            int sh_id = (int)cmd.Parameters["@sh_id"].Value;

            return sh_id;
        }
        public int SearchHistoryExt_Slave_Insert(Req_Info_Query query, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(cmd, "@sh_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@sh_str", DbType.String, query.query_str.To_Sql_Safe());
            Db_0.AddInParameter(cmd, "@sh_type", DbType.Byte, query.q_type);
            Db_0.AddInParameter(cmd, "@sh_uid", DbType.Int32, query.u_id.ToInt());
            Db_0.AddInParameter(cmd, "@sh_uname", DbType.String, query.u_name);
            //Db_0.AddInParameter(cmd, "@sh_oc_name", DbType.String, query.u_name);
            Db_0.AddInParameter(cmd, "@tableName", DbType.String, string.Format("SearchHistoryExt_{0:D3}", query.u_id.ToInt() % 256));

            int _returnValue = Db_0.ExecuteNonQuery(cmd);
            int sh_id = (int)cmd.Parameters["@sh_id"].Value;

            return _returnValue;
        }
        public int SearchHistoryExt_Master_Insert(Req_Info_Query query, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(cmd, "@sh_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@sh_str", DbType.String, query.query_str.To_Sql_Safe());
            Db_0.AddInParameter(cmd, "@sh_type", DbType.Byte, query.q_type);
            Db_0.AddInParameter(cmd, "@sh_uid", DbType.String, query.u_id);
            Db_0.AddInParameter(cmd, "@sh_uname", DbType.String, query.u_name);

            int _returnValue = Db_0.ExecuteNonQuery(cmd);
            int sh_id = (int)cmd.Parameters["@sh_id"].Value;

            return _returnValue;
        }

        public int SearchHistory_Master_Insert(Company c, string sp_Name)
        {
            DbCommand cmd;
            SearchHistory_Sp_Map(c, sp_Name, out cmd);

            try
            {

                int _returnValue = Db_0.ExecuteNonQuery(cmd);
                int sh_id = (int)cmd.Parameters["@sh_id"].Value;

                return _returnValue;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 插入记录到分表
        /// </summary>
        /// <param name="log">记录实体类</param>
        /// <returns></returns>
        public int AppOrgCompanyLog_Insert(AppOrgCompanyLogInfo log, string sp_Name)
        {
            string tablename = "AppOrgCompanyLog_" + DateTime.Now.Date.ToString("yyyyMMdd");
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@cl_action", DbType.String, log.cl_action);
            Db_0.AddInParameter(dbCommandWrapper, "@cl_u_name", DbType.String, log.cl_u_name);
            Db_0.AddInParameter(dbCommandWrapper, "@cl_u_uid", DbType.Int32, log.cl_u_uid);
            Db_0.AddInParameter(dbCommandWrapper, "@cl_date", DbType.DateTime, log.cl_date);
            Db_0.AddInParameter(dbCommandWrapper, "@cl_osName", DbType.String, log.cl_osName);
            Db_0.AddInParameter(dbCommandWrapper, "@cl_ip", DbType.String, log.cl_ip);
            Db_0.AddInParameter(dbCommandWrapper, "@cl_browser", DbType.String, log.cl_browser);
            Db_0.AddInParameter(dbCommandWrapper, "@cl_screenSize", DbType.String, log.cl_screenSize);
            Db_0.AddInParameter(dbCommandWrapper, "@cl_cookieId", DbType.String, log.cl_cookieId);
            Db_0.AddInParameter(dbCommandWrapper, "@cl_appVer", DbType.String, log.cl_appVer);
            Db_0.AddInParameter(dbCommandWrapper, "@tableName", DbType.String, tablename);
            Db_0.AddInParameter(dbCommandWrapper, "@tableCountCeiling", DbType.Int32, Log_TableCountCeiling);
            Db_0.AddOutParameter(dbCommandWrapper, "@cl_id", DbType.Int32, 4);

            Db_0.ExecuteNonQuery(dbCommandWrapper);
            var cl_id = (int)dbCommandWrapper.Parameters["@cl_id"].Value;

            return cl_id;
        }

        /// <summary>
        /// query in CompanyBlackList for the existence of a company determined by it's oc_code 
        /// </summary>
        /// <param name="oc_code"></param>
        /// <param name="sp_Name"></param>
        /// <returns>exists, true</returns>
        public bool CompanyBlackList_Exist_ByCode(string oc_code, string sp_Name)
        {
            var cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@cbl_oc_code", DbType.String, oc_code);

            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    return (bool)reader["cbl_status"];
                }
            }
            return false;
        }
        public bool CompanyBlackList_Exist_ByName(string oc_name, string sp_Name)
        {
            var cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@cbl_oc_name", DbType.String, oc_name);

            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    return (bool)reader["cbl_status"];
                }
            }
            return false;
        }

        private void BrowseLog_Sp_Map(BrowseLogInfo log, string sp_Name, out DbCommand cmd)
        {
            cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(cmd, "@bl_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@bl_oc_name", DbType.String, log.bl_oc_name);
            Db_0.AddInParameter(cmd, "@bl_oc_code", DbType.String, log.bl_oc_code);
            Db_0.AddInParameter(cmd, "@bl_oc_area", DbType.String, log.bl_oc_area);
            Db_0.AddInParameter(cmd, "@bl_u_name", DbType.String, log.bl_u_name);
            Db_0.AddInParameter(cmd, "@bl_u_uid", DbType.Int32, log.bl_u_uid);
            Db_0.AddInParameter(cmd, "@bl_date", DbType.DateTime, log.bl_date);
            Db_0.AddInParameter(cmd, "@bl_ip", DbType.String, log.bl_ip);
            Db_0.AddInParameter(cmd, "@bl_osName", DbType.String, log.bl_osName);
            Db_0.AddInParameter(cmd, "@bl_appVer", DbType.String, log.bl_appVer);
        }

        /// <summary>
        /// Insert into master table when user not login
        /// </summary>
        /// <param name="log"></param>
        /// <param name="sp_Name"></param>
        /// <returns></returns>
        public int BrowseLog_Master_Insert(BrowseLogInfo log, string sp_Name)
        {
            DbCommand cmd;
            BrowseLog_Sp_Map(log, sp_Name, out cmd);
            int _returnValue = Db_0.ExecuteNonQuery(cmd);
            int bl_id = (int)cmd.Parameters["@bl_id"].Value;

            return bl_id;
        }

        /// <summary>
        /// Insert to slave table when user login
        /// </summary>
        /// <param name="log"></param>
        /// <param name="sp_Name"></param>
        /// <returns></returns>
        public int BrowseLog_Slave_Insert(BrowseLogInfo log, string sp_Name)
        {
            DbCommand cmd;
            BrowseLog_Sp_Map(log, sp_Name, out cmd);
            Db_0.AddInParameter(cmd, "@tableName", DbType.String, "BrowseLog_" + log.bl_u_uid % 256);  //todo: slave table name: BrowseLog_{ : D3} or { }
            int _returnValue = Db_0.ExecuteNonQuery(cmd);
            int bl_id = (int)cmd.Parameters["@bl_id"].Value;

            return bl_id;
        }

        public List<string> Browses_Fresh_Get(int count, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@count", DbType.Int32, count);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                var list = new List<string>();
                while(reader.Read())
                {
                    list.Add(reader["bl_oc_code"].ToString());
                }
                return list;
            }
        }

        public CompanyEvaluateInfo CompanyEvaluate_Select(string oc_code, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@ce_oc_code", DbType.String, oc_code);
            CompanyEvaluateInfo obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    obj = new CompanyEvaluateInfo();
                    obj.ce_id = (int)reader["ce_id"];
                    obj.ce_oc_name = reader["ce_oc_name"].ToString();
                    obj.ce_oc_code = reader["ce_oc_code"].ToString();
                    obj.ce_oc_area = reader["ce_oc_area"].ToString();
                    obj.ce_tucaoNum = (int)reader["ce_tucaoNum"];
                    obj.ce_likeNum = (int)reader["ce_likeNum"];
                    obj.ce_notLikeNum = (int)reader["ce_notLikeNum"];
                    obj.ce_visitNum = (int)reader["ce_visitNum"];
                    obj.ce_FavorNum = (int)reader["ce_FavorNum"];
                }
            }


            return obj;
        }
        public int CompanyEvaluate_Update(CompanyEvaluateInfo info, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@ce_id", DbType.Int32, info.ce_id);
            Db_0.AddInParameter(cmd, "@ce_oc_name", DbType.String, info.ce_oc_name);
            Db_0.AddInParameter(cmd, "@ce_oc_code", DbType.String, info.ce_oc_code);
            Db_0.AddInParameter(cmd, "@ce_oc_area", DbType.String, info.ce_oc_area);
            Db_0.AddInParameter(cmd, "@ce_tucaoNum", DbType.Int32, info.ce_tucaoNum);
            Db_0.AddInParameter(cmd, "@ce_likeNum", DbType.Int32, info.ce_likeNum);
            Db_0.AddInParameter(cmd, "@ce_notLikeNum", DbType.Int32, info.ce_notLikeNum);
            Db_0.AddInParameter(cmd, "@ce_visitNum", DbType.Int32, info.ce_visitNum);
            Db_0.AddInParameter(cmd, "@ce_FavorNum", DbType.Int32, info.ce_FavorNum);
            return Db_0.ExecuteNonQuery(cmd);
        }

        public List<TradeEntity> TradeCategory_AllSelect(string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            var list = new List<TradeEntity>(1000);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while(reader.Read())
                {
                    var id = reader["ts_id"].ToString();
                    if (!id.Contains("Q"))
                    {
                        var e = new TradeEntity();
                        e.ts_id = id;
                        e.ts_name = reader["ts_name"].ToString();
                        e.ts_tc_id = reader["ts_tc_id"].ToString();
                        list.Add(e);
                    }
                }
                return list;
            }
        }
        /// <summary>
        /// select all records where len(pc_path) < 5
        /// </summary>
        /// <param name="sp_Name"></param>
        /// <returns></returns>
        public List<ProductEntity> ProductCategory_AllSelect(string sp_Name)
        {
            var cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@columns", DbType.String, "*");
            Db_0.AddInParameter(cmd, "@where", DbType.String, " where len(pc_path) < 5 ");
            Db_0.AddInParameter(cmd, "@order", DbType.String, " order by pc_path ");
            Db_0.AddInParameter(cmd, "@page", DbType.Int32, 1);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, 2000);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);
            var list = new List<ProductEntity>(1000);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while(reader.Read())
                {
                    var p = new ProductEntity();
                    p.pc_name = reader["pc_name"].ToString();
                    p.pc_path = reader["pc_path"].ToString();
                    list.Add(p);
                }
            }
            return list;
        }

        public List<OrgCompanyListInfo> OrgCompanyList_Page_Select(DatabaseSearchModel s, string sp_Name)
        {
            var cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@columns", DbType.String, s.Column);
            Db_0.AddInParameter(cmd, "@where", DbType.String, s.Where);
            Db_0.AddInParameter(cmd, "@order", DbType.String, s.Order);
            Db_0.AddInParameter(cmd, "@page", DbType.Int32, s.PageIndex);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, s.PageSize);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);

            List<OrgCompanyListInfo> lst = new List<OrgCompanyListInfo>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    OrgCompanyListInfo obj = new OrgCompanyListInfo();
                    obj.oc_id = (int)reader["oc_id"];
                    obj.oc_code = reader["oc_code"].ToString();
                    obj.oc_number = reader["oc_number"].ToString();
                    obj.oc_issuetime = (DateTime)reader["oc_issuetime"];
                    obj.oc_area = reader["oc_area"].ToString();
                    obj.oc_areaName = reader["oc_areaName"].ToString();
                    obj.oc_regOrgName = reader["oc_regOrgName"].ToString();
                    obj.oc_name = reader["oc_name"].ToString();
                    obj.oc_address = reader["oc_address"].ToString();
                    obj.oc_createUser = reader["oc_createUser"].ToString();
                    obj.oc_createTime = (DateTime)reader["oc_createTime"];
                    lst.Add(obj);
                }
                reader.NextResult();
            }


            return lst;
        }

        public List<OrgCompanyDtlInfo> OrgCompanyDtl_Page_Select(DatabaseSearchModel s, string sp_Name)
        {
            var cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@columns", DbType.String, s.Column);
            Db_0.AddInParameter(cmd, "@where", DbType.String, s.Where);
            Db_0.AddInParameter(cmd, "@order", DbType.String, s.Order);
            Db_0.AddInParameter(cmd, "@page", DbType.Int32, s.PageIndex);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, s.PageSize);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);

            List<OrgCompanyDtlInfo> lst = new List<OrgCompanyDtlInfo>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    OrgCompanyDtlInfo obj = new OrgCompanyDtlInfo();
                    obj.od_oc_code = reader["od_oc_code"].ToString();
                    //obj.oc_number = reader["oc_number"].ToString();
                    obj.od_regMoney = reader["od_regMoney"].ToString();
                    obj.od_faRen = reader["od_faRen"].ToString();
                    lst.Add(obj);
                }
                reader.NextResult();
            }


            return lst;
        }

        public OrgCompanyDtlInfo OrgCompanyDtl_Select(string oc_code, string sp_Name)
        {
            var cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@oc_code", DbType.String, oc_code);
            Db_0.AddOutParameter(cmd, "@status", DbType.Byte, 1);
            OrgCompanyDtlInfo obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    obj = OrgCompanyDtl_Map(reader);
                }

                reader.NextResult();
                int status = Convert.ToInt32(cmd.Parameters["@status"].Value);
                if (obj != null)
                    obj.oc_status = status;
            }
            return obj;
        }
        private OrgCompanyDtlInfo OrgCompanyDtl_Map(IDataReader reader)
        {
            OrgCompanyDtlInfo d = new OrgCompanyDtlInfo();
            d.od_id = (int)reader["od_id"];
            d.od_oc_code = reader["od_oc_code"].ToString();
            d.od_faRen = reader["od_faRen"].ToString();
            d.od_regMoney = reader["od_regMoney"].ToString();
            d.od_factMoney = reader["od_factMoney"].ToString();
            d.od_regDate = reader["od_regDate"].ToString();
            d.od_bussinessS = reader["od_bussinessS"].ToString();
            d.od_bussinessE = reader["od_bussinessE"].ToString();
            d.od_chkDate = reader["od_chkDate"].ToString();
            d.od_yearChk = reader["od_yearChk"].ToString();
            d.od_regType = reader["od_regType"].ToString();
            d.od_bussinessDes = reader["od_bussinessDes"].ToString();
            d.od_ext = reader["od_ext"].ToString();
            d.od_CreateTime = (DateTime)reader["od_CreateTime"];
            d.oc_name = reader["oc_name"].ToString();
            d.oc_address = reader["oc_address"].ToString();
            d.oc_number = reader["oc_number"].ToString();
            return d;
        }

        public OrgCompanyListInfo OrgCompanyList_Select(string oc_code, string sp_Name)
        {
            var cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@oc_code", DbType.String, oc_code);
            OrgCompanyListInfo obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    obj = OrgCompanyList_Map(reader);
                }
            }
            return obj;
        }

        private OrgCompanyListInfo OrgCompanyList_Map(IDataReader reader)
        {
            OrgCompanyListInfo obj = new OrgCompanyListInfo();
            obj = new OrgCompanyListInfo();
            obj.oc_id = (int)reader["oc_id"];
            obj.oc_code = reader["oc_code"].ToString();
            obj.oc_number = reader["oc_number"].ToString();
            obj.oc_area = reader["oc_area"].ToString();
            obj.oc_areaName = reader["oc_areaName"].ToString();
            obj.oc_regOrgName = reader["oc_regOrgName"].ToString();
            obj.oc_name = reader["oc_name"].ToString();
            obj.oc_address = reader["oc_address"].ToString();
            obj.oc_createUser = reader["oc_createUser"].ToString();
            obj.oc_createTime = (DateTime)reader["oc_createTime"];
            obj.oc_status = (bool)reader["oc_status"];

            object ut = reader["oc_updatetime"];
            if (ut != DBNull.Value)
                obj.oc_updatetime = (DateTime)ut;
            else
                obj.oc_updatetime = new DateTime(1900, 1, 1);

            //obj.oc_updatetime = (DateTime)reader["oc_updatetime"];

            object it = reader["oc_issuetime"];
            if (it != DBNull.Value)
                obj.oc_issuetime = (DateTime)it;
            else
                obj.oc_issuetime = new DateTime(1900, 1, 1);


            //obj.oc_issuetime = (DateTime)reader["oc_issuetime"];

            object iv = reader["oc_invalidtime"];
            if (iv != DBNull.Value)
                obj.oc_invalidtime = (DateTime)iv;
            else
                obj.oc_invalidtime = new DateTime(1900, 1, 1);

            //obj.oc_invalidtime = (DateTime)reader["oc_invalidtime"];

            object ct = reader["oc_companytype"];
            if (ct != DBNull.Value)
            {
                obj.oc_companytype = (string)ct;
            }
            else
            {
                obj.oc_companytype = string.Empty;
            }
            return obj;
        }

        public List<Company_Member> CompanyMember_Select(string oc_code, string sp_Name)
        {
            var cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@om_oc_code", DbType.String, oc_code);
            List<Company_Member> lst = new List<Company_Member>();

            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    Company_Member obj = CompanyMember_Map_1(reader);
                    lst.Add(obj);
                }
                reader.NextResult();
            }
            return lst;
        }
        public List<Company_Member> CompanyMember_Select(string oc_area, string sp_Name, DatabaseSearchModel<OrgCompanyGsxtDtlMgrInfo> s)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@area", DbType.String, oc_area);
            Db_0.AddInParameter(cmd, "@columns", DbType.String, s.Column);
            Db_0.AddInParameter(cmd, "@where", DbType.String, s.Where);
            Db_0.AddInParameter(cmd, "@order", DbType.String, s.Order);
            Db_0.AddInParameter(cmd, "@page", DbType.Int32, s.PageIndex);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, s.PageSize);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);

            List<Company_Member> lst = new List<Company_Member>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    Company_Member obj = CompanyMember_Map(reader);
                    if (obj.oc_member_status != 4)
                        lst.Add(obj);
                }
                reader.NextResult();
            }
            return lst;
        }

        private Company_Member CompanyMember_Map_1(IDataReader reader)
        {
            var m = new Company_Member();
            m.oc_member_name = reader["om_name"].ToString();
            m.oc_member_position = reader["om_position"].ToString();
            return m;
        }
        private Company_Member CompanyMember_Map(IDataReader reader)
        {
            var m = new Company_Member();
            m.oc_member_name = reader["om_name"].ToString();
            m.oc_member_position = reader["om_position"].ToString();
            m.oc_member_status = (byte)reader["om_status"];
            return m;
        }

        //[Obsolete("reference to \"CompanyMember_Select(string oc_code, string sp_Name)\"")]
        public List<OrgCompanyDtlMgrInfo> OrgCompanyDtlMgr_Select(string oc_code, string sp_Name)
        {
            var cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@om_oc_code", DbType.String, oc_code);
            try
            {
                List<OrgCompanyDtlMgrInfo> lst = new List<OrgCompanyDtlMgrInfo>();
                using (IDataReader reader = Db_0.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        OrgCompanyDtlMgrInfo obj = OrgCompanyDtlMgr_Map(reader);
                        lst.Add(obj);
                    }
                    reader.NextResult();
                }
                return lst;
            }
            catch(Exception)
            {
                throw;
            }
        }
        public List<OrgCompanyGsxtDtlMgrInfo> OrgCompanyGsxtDtlMgr_Page_Select(DatabaseSearchModel s, string oc_area, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@area", DbType.String, oc_area);
            Db_0.AddInParameter(dbCommandWrapper, "@columns", DbType.String, s.Column);
            Db_0.AddInParameter(dbCommandWrapper, "@where", DbType.String, s.Where);
            Db_0.AddInParameter(dbCommandWrapper, "@order", DbType.String, s.Order);
            Db_0.AddInParameter(dbCommandWrapper, "@page", DbType.Int32, s.PageIndex);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, s.PageSize);
            Db_0.AddOutParameter(dbCommandWrapper, "@rowCount", DbType.Int32, 4);

            List<OrgCompanyGsxtDtlMgrInfo> lst = new List<OrgCompanyGsxtDtlMgrInfo>();
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while (reader.Read())
                {
                    OrgCompanyGsxtDtlMgrInfo obj = new OrgCompanyGsxtDtlMgrInfo();
                    obj.om_id = (int)reader["om_id"];
                    obj.om_oc_code = reader["om_oc_code"].ToString();
                    obj.om_name = reader["om_name"].ToString();
                    obj.om_position = reader["om_position"].ToString();
                    obj.om_type = reader["om_type"].ToString();
                    obj.om_createTime = (DateTime)reader["om_createTime"];
                    obj.om_invalidTime = (DateTime)reader["om_invalidTime"];
                    obj.om_status = (byte)reader["om_status"];
                    obj.om_updateTime = (DateTime)reader["om_updateTime"];
                    lst.Add(obj);
                }
                reader.NextResult();
                // rowcount = (int)dbCommandWrapper.Parameters["@rowCount"].Value;
            }

            return lst;
        }

        public List<Resp_Oc_Abs> CompanyList_Buld_Get(List<string> codes, string sp_Name)
        {
            var cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@oc_code", DbType.String, string.Empty);
            var list = new List<Resp_Oc_Abs>();
            foreach(var c in codes)
            {
                Db_0.SetParameterValue(cmd, "@oc_code", c);
                using (IDataReader reader = Db_0.ExecuteReader(cmd))
                {
                    if(reader.Read())
                    {
                        var o = new Resp_Oc_Abs();
                        o.oc_addr = reader["oc_address"].ToString();
                        o.oc_area = reader["oc_area"].ToString();
                        o.oc_code = reader["oc_code"].ToString();
                        o.oc_issue_time = reader["oc_issue_time"].ToString();
                        o.oc_name_hl = reader["oc_name"].ToString();
                        o.oe_status = (bool)reader["oc_status"];
                        o.oc_type = reader["oc_companytype"].ToString();
                        list.Add(o);
                    }
                }
            }
            return list;
        }

        public List<OrgCompanyBrand_Dtl> OrgCompanyBrand_Select(string oc_code, string sp_Name)
        {
            var cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@ob_oc_code", DbType.String, oc_code);

            List<OrgCompanyBrand_Dtl> lst = new List<OrgCompanyBrand_Dtl>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var obj = OrgCompanyBrand_Dtl_Map(reader);
                    lst.Add(obj);
                }
                reader.NextResult();
            }
            return lst;
        }
        public List<OrgCompanyBrand> OrgCompanyBrand_Page_Select(DatabaseSearchModel s, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@Columns", DbType.String, s.Column);
            Db_0.AddInParameter(cmd, "@Where", DbType.String, s.Where);
            Db_0.AddInParameter(cmd, "@Order", DbType.String, s.Order);
            Db_0.AddInParameter(cmd, "@Page", DbType.Int32, s.PageIndex);
            Db_0.AddInParameter(cmd, "@PageSize", DbType.Int32, s.PageSize);
            Db_0.AddInParameter(cmd, "@tableName", DbType.String, s.Table);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);
            List<OrgCompanyBrand> lst = new List<OrgCompanyBrand>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var obj = OrgCompanyBrand_Map(reader);
                    lst.Add(obj);
                }
                reader.NextResult();
            }
            return lst;
        }
        public OrgCompanyBrand OrgCompanyBrand_Map(IDataReader reader)
        {
            var o = new OrgCompanyBrand();
            o.ob_applicationDate = (DateTime)reader["ob_applicationDate"];
            //o.ob_classNo = reader["ob_classNo"].ToString();
            o.ob_proposer = reader["ob_proposer"].ToString();
            o.ob_img = reader["ob_img"].ToString();
            o.ob_regNo = reader["ob_regNo"].ToString();
            o.ob_classNo = reader["ob_classNo"].ToString();
            //o.oe_brandProcess = reader["oe_brandProcess"].ToString();
            //o.oe_service = reader["oe_service"].ToString();
            //o.oe_serviceCode = reader["oe_serviceCode"].ToString();
            
            o.ob_oc_code = reader["ob_oc_code"].ToString();
            o.ob_name = reader["ob_name"].ToString();
            o.ob_dlrmc = reader["ob_dlrmc"].ToString();
            o.ob_zyksqx = reader["ob_zyksqx"].ToString();
            o.ob_zyjsqx = reader["ob_zyjsqx"].ToString();
            o.ob_id = (int)reader["ob_id"];

            var oe_brandProcess = BrandProcess_Get(o.ob_regNo, o.ob_classNo);
            if (!string.IsNullOrEmpty(oe_brandProcess))
            {
                var bps = oe_brandProcess.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var bp = bps.Last();
                var ds = bp.Split('-');
                o.ob_status = ds[0];
            }
            return o;
        }

        public string BrandProcess_Get(string reg_no, string class_no)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_OrgCompanyBrandExt_SelectByRegClassNo");
            Db_0.AddInParameter(cmd, "@oe_ob_regNo", DbType.String, reg_no);
            Db_0.AddInParameter(cmd, "@oe_ob_classNo", DbType.String, class_no);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                    return reader["oe_brandProcess"].ToString();
            }
            return string.Empty;
        }

        public OrgCompanyBrand_Dtl OrgCompanyBrand_Dtl_Map(IDataReader reader)
        {
            var o = new OrgCompanyBrand_Dtl();
            o.ob_applicationDate = (DateTime)reader["ob_applicationDate"];
            o.ob_classNo = reader["ob_classNo"].ToString();
            o.ob_proposer = reader["ob_proposer"].ToString();
            o.ob_img = reader["ob_img"].ToString();
            o.ob_regNo = reader["ob_regNo"].ToString();
            //o.oe_brandProcess = reader["oe_brandProcess"].ToString();
            //o.oe_service = reader["oe_service"].ToString();
            //o.oe_serviceCode = reader["oe_serviceCode"].ToString();
            o.ob_oc_code = reader["ob_oc_code"].ToString();
            o.ob_name = reader["ob_name"].ToString();
            o.ob_dlrmc = reader["ob_dlrmc"].ToString();
            o.ob_zyksqx = reader["ob_zyksqx"].ToString();
            o.ob_zyjsqx = reader["ob_zyjsqx"].ToString();
            return o;
        }

        public List<CompanyPatent> OrgCompanyPatent_Select(string oc_code, string sp_Name)
        {
            var cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@oc_code", DbType.String, oc_code);

            List<CompanyPatent> lst = new List<CompanyPatent>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var obj = OrgCompanyPatent_Map(reader);
                    lst.Add(obj);
                }
                reader.NextResult();
            }
            return lst;
        }
        public List<CompanyPatent> OrgCompanyPatent_Page_Select(DatabaseSearchModel s, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@Columns", DbType.String, s.Column);
            Db_0.AddInParameter(cmd, "@Where", DbType.String, s.Where);
            Db_0.AddInParameter(cmd, "@Order", DbType.String, s.Order);
            Db_0.AddInParameter(cmd, "@Page", DbType.Int32, s.PageIndex);
            Db_0.AddInParameter(cmd, "@PageSize", DbType.Int32, s.PageSize);
            Db_0.AddInParameter(cmd, "@tableName", DbType.String, s.Table);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);
            List<CompanyPatent> lst = new List<CompanyPatent>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var obj = OrgCompanyPatent_Map(reader);
                    lst.Add(obj);
                }
                reader.NextResult();
            }
            return lst;
        }

        public Tuple<List<Software_Abs>, int> Software_Page_Select(DatabaseSearchModel s, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@Columns", DbType.String, s.Column);
            Db_0.AddInParameter(cmd, "@Where", DbType.String, s.Where);
            Db_0.AddInParameter(cmd, "@Order", DbType.String, s.Order);
            Db_0.AddInParameter(cmd, "@Page", DbType.Int32, s.PageIndex);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, s.PageSize);
            Db_0.AddInParameter(cmd, "@tableName", DbType.String, s.Table);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);

            var list = new List<Software_Abs>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while(reader.Read())
                {
                    var o = new Software_Abs();
                    //o.id = (int)reader["sc_id"];
                    o.name = reader["sc_softName"].ToString();
                    o.s_name = reader["sc_shortName"].ToString();
                    o.reg_no = reader["sc_softId"].ToString();
                    o.reg_date = ((DateTime)reader["sc_successDate"]).ToString("yyyy-MM-dd");
                    o.author = reader["sc_softAuthor"].ToString();
                    o.cat_no = reader["sc_typeNum"].ToString();
                    list.Add(o);
                }
                reader.NextResult();
                int count = (int)cmd.Parameters["@rowCount"].Value;
                return new Tuple<List<Software_Abs>, int>(list, count);
            }
        }
        public Tuple<List<Product_Abs>, int> Product_Page_Select(DatabaseSearchModel s, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@Columns", DbType.String, s.Column);
            Db_0.AddInParameter(cmd, "@Where", DbType.String, s.Where);
            Db_0.AddInParameter(cmd, "@Order", DbType.String, s.Order);
            Db_0.AddInParameter(cmd, "@Page", DbType.Int32, s.PageIndex);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, s.PageSize);
            Db_0.AddInParameter(cmd, "@tableName", DbType.String, s.Table);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);

            var list = new List<Product_Abs>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var o = new Product_Abs();
                    //o.id = (int)reader["pc_id"];
                    o.name = reader["pc_productName"].ToString();
                    //o.s_name = reader["sc_shortName"].ToString();
                    o.reg_no = reader["pc_regId"].ToString();
                    o.reg_date = ((DateTime)reader["pc_regDate"]).ToString("yyyy-MM-dd");
                    o.type = reader["pc_type"].ToString();
                    o.finish_date = ((DateTime)reader["pc_completionDate"]).ToString("yyyy-MM-dd");
                    o.publish_date = ((DateTime)reader["pc_firstPublicationDate"]).ToString("yyyy-MM-dd");
                    o.author = reader["pc_productAuthor"].ToString();
                    list.Add(o);
                }
                reader.NextResult();
                int count = (int)cmd.Parameters["@rowCount"].Value;
                return new Tuple<List<Product_Abs>, int>(list, count);
            }
        }

        public CompanyPatent OrgCompanyPatent_Map(IDataReader reader)
        {
            var p = new CompanyPatent();
            p.ID = (int)reader["ID"];
            p.oc_code = reader["oc_code"].ToString();
            p.oc_name = reader["oc_name"].ToString();
            p.Patent_No = reader["Patent_No"].ToString();
            p.patent_date = reader["Patent_Day"].ToString();
            p.Patent_gkh = reader["Patent_gkh"].ToString();
            //p.Patent_gkr = reader["Patent_gkr"].ToString();
            p.Patent_Name = reader["Patent_Name"].ToString();
            //p.Patent_sjr = reader["Patent_sjr"].ToString();
            p.Patent_Type = reader["Patent_Type"].ToString();
            p.Patent_Status = reader["Patent_Status"].ToString();
            p.Patent_sqr = reader["Patent_sqr"].ToString();
            //p.Patent_zy = reader["Patent_zy"].ToString();
            //p.Patent_yxq = reader["Patent_yxq"].ToString();
            return p;
        }

        public List<WenshuIndex> OrgCompanyJudge_Page_Select(DatabaseSearchModel s, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@Columns", DbType.String, s.Column);
            Db_0.AddInParameter(cmd, "@Where", DbType.String, s.Where);
            Db_0.AddInParameter(cmd, "@Order", DbType.String, s.Order);
            Db_0.AddInParameter(cmd, "@Page", DbType.Int32, s.PageIndex);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, s.PageSize);
            Db_0.AddInParameter(cmd, "@tableName", DbType.String, s.Table);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);
            List<WenshuIndex> lst = new List<WenshuIndex>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var obj = OrgCompanyJudge_Map(reader);
                    lst.Add(obj);
                }
                reader.NextResult();
            }
            return lst;
        }
        public List<ShixinIndex> OrgCompanyDishonest_Page_Select(DatabaseSearchModel s, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@Columns", DbType.String, s.Column);
            Db_0.AddInParameter(cmd, "@Where", DbType.String, s.Where);
            Db_0.AddInParameter(cmd, "@Order", DbType.String, s.Order);
            Db_0.AddInParameter(cmd, "@Page", DbType.Int32, s.PageIndex);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, s.PageSize);
            Db_0.AddInParameter(cmd, "@tableName", DbType.String, s.Table);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);
            List<ShixinIndex> lst = new List<ShixinIndex>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var obj = OrgCompanyDishonest_Map(reader);
                    lst.Add(obj);
                }
                reader.NextResult();
            }
            return lst;
        }
        public ShixinIndex OrgCompanyDishonest_Map(IDataReader reader)
        {
            var x = new ShixinIndex();
            x.sx_id = (int)reader["sx_id"];
            x.sx_iname = reader["sx_iname"].ToString();
            x.sx_performance = reader["sx_performance"].ToString();
            x.sx_publishDate = (DateTime)reader["sx_publishDate"];
            x.sx_caseCode = reader["sx_caseCode"].ToString();
            x.sx_courtName = reader["sx_courtName"].ToString();
            x.sx_areaName = reader["sx_areaName"].ToString();
            return x;
        }

        public int Company_ScoreMark(Req_Oc_Score score, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(dbCommandWrapper, "@cs_id", DbType.Int32, 4);
            Db_0.AddInParameter(dbCommandWrapper, "@cs_oc_code", DbType.String, score.oc_code);
            Db_0.AddInParameter(dbCommandWrapper, "@cs_oc_name", DbType.String, score.oc_name);
            Db_0.AddInParameter(dbCommandWrapper, "@cs_u_name", DbType.String, score.u_name);
            Db_0.AddInParameter(dbCommandWrapper, "@cs_u_uid", DbType.String, score.u_id);
            Db_0.AddInParameter(dbCommandWrapper, "@cs_oc_score", DbType.Int32, score.oc_score);

            int _returnValue = Db_0.ExecuteNonQuery(dbCommandWrapper);
            int cs_id = (int)dbCommandWrapper.Parameters["@cs_id"].Value;

            return cs_id;
        }


        public WenshuIndex OrgCompanyJudge_Map(IDataReader reader)
        {
            var w = new WenshuIndex();
            w.jd_guid = (Guid)reader["jd_id"];
            w.jd_date = (DateTime)reader["jd_docDate"];
            w.jd_ch = reader["jd_courtHierarchy"].ToString();
            w.jd_num = reader["jd_caseNumber"].ToString();
            w.jd_title = reader["jd_docTitle"].ToString();
            return w;
        }

        [Obsolete]
        public OrgCompanyDtlMgrInfo OrgCompanyDtlMgr_Map(IDataReader reader)
        {
            OrgCompanyDtlMgrInfo obj = new OrgCompanyDtlMgrInfo();
            obj.om_id = (int)reader["om_id"];
            obj.om_oc_code = reader["om_oc_code"].ToString();
            obj.om_name = reader["om_name"].ToString();
            obj.om_position = reader["om_position"].ToString();
            obj.om_type = reader["om_type"].ToString();
            return obj;
        }

        [Obsolete("reference to \"CompanyMember_Select(string oc_area, string sp_Name, DatabaseSearchModel<OrgCompanyGsxtDtlMgrInfo> s)\"")]
        public List<OrgCompanyGsxtDtlMgrInfo> OrgCompanyGsxtDtlMgr_Select(string oc_area, string sp_Name, DatabaseSearchModel<OrgCompanyGsxtDtlMgrInfo> s)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@area", DbType.String, oc_area);
            Db_0.AddInParameter(cmd, "@columns", DbType.String, s.Column);
            Db_0.AddInParameter(cmd, "@where", DbType.String, s.Where);
            Db_0.AddInParameter(cmd, "@order", DbType.String, s.Order);
            Db_0.AddInParameter(cmd, "@page", DbType.Int32, s.PageIndex);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, s.PageSize);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);

            try
            {

                List<OrgCompanyGsxtDtlMgrInfo> lst = new List<OrgCompanyGsxtDtlMgrInfo>();
                using (IDataReader reader = Db_0.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        OrgCompanyGsxtDtlMgrInfo obj = new OrgCompanyGsxtDtlMgrInfo();
                        obj.om_id = (int)reader["om_id"];
                        obj.om_oc_code = reader["om_oc_code"].ToString();
                        obj.om_name = reader["om_name"].ToString();
                        obj.om_position = reader["om_position"].ToString();
                        obj.om_type = reader["om_type"].ToString();
                        obj.om_createTime = (DateTime)reader["om_createTime"];
                        obj.om_invalidTime = (DateTime)reader["om_invalidTime"];
                        obj.om_status = (byte)reader["om_status"];
                        obj.om_updateTime = (DateTime)reader["om_updateTime"];
                        lst.Add(obj);
                    }
                    reader.NextResult();
                    //rowcount = (int)cmd.Parameters["@rowCount"].Value;
                }
                return lst;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oc_code"></param>
        /// <param name="sp_Name"></param>
        /// <returns></returns>
        public List<Company_Sh> Company_Sh_Select(string oc_code, string sp_Name)
        {
            var cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@og_oc_code", DbType.String, oc_code);

            var list = new List<Company_Sh>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while(reader.Read())
                {
                    list.Add(Company_Sh_Map_0(reader));
                }
            }
            return list;
        }
        public List<OrgCompanyDtlGDInfo> OrgCompanyDtlGD_FromOccode_Select(string oc_code, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@og_oc_code", DbType.String, oc_code);

            try
            {

                List<OrgCompanyDtlGDInfo> lst = new List<OrgCompanyDtlGDInfo>();
                using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
                {
                    while (reader.Read())
                    {
                        OrgCompanyDtlGDInfo obj = new OrgCompanyDtlGDInfo();
                        obj.og_int = (int)reader["og_int"];
                        obj.og_oc_code = reader["og_oc_code"].ToString();
                        obj.og_name = reader["og_name"].ToString();
                        obj.og_money = (decimal)reader["og_money"];
                        obj.og_BL = (decimal)reader["og_BL"];
                        obj.og_unit = reader["og_unit"].ToString();
                        obj.og_pro = reader["og_pro"].ToString();
                        obj.og_type = reader["og_type"].ToString();
                        lst.Add(obj);
                    }
                    reader.NextResult();
                }


                return lst;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public List<OrgCompanyGsxtDtlGDInfo> OrgCompanyGsxtDtlGD_Page_Select(DatabaseSearchModel s, string oc_area, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@area", DbType.String, oc_area);
            Db_0.AddInParameter(dbCommandWrapper, "@columns", DbType.String, s.Column);
            Db_0.AddInParameter(dbCommandWrapper, "@where", DbType.String, s.Where);
            Db_0.AddInParameter(dbCommandWrapper, "@order", DbType.String, s.Order);
            Db_0.AddInParameter(dbCommandWrapper, "@page", DbType.Int32, s.PageIndex);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, s.PageSize);
            Db_0.AddOutParameter(dbCommandWrapper, "@rowCount", DbType.Int32, 4);

            List<OrgCompanyGsxtDtlGDInfo> lst = new List<OrgCompanyGsxtDtlGDInfo>();
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while (reader.Read())
                {
                    OrgCompanyGsxtDtlGDInfo obj = new OrgCompanyGsxtDtlGDInfo();
                    obj.og_int = (int)reader["og_int"];
                    obj.og_oc_code = reader["og_oc_code"].ToString();
                    obj.og_name = reader["og_name"].ToString();
                    obj.og_type = reader["og_type"].ToString();
                    obj.og_licenseType = reader["og_licenseType"].ToString();
                    obj.og_licenseNum = reader["og_licenseNum"].ToString();
                    obj.og_subscribeAccount = (decimal)reader["og_subscribeAccount"];
                    obj.og_subscribeForm = reader["og_subscribeForm"].ToString();
                    obj.og_subscribeDate = reader["og_subscribeDate"].ToString();
                    obj.og_realAccount = (decimal)reader["og_realAccount"];
                    obj.og_realForm = reader["og_realForm"].ToString();
                    obj.og_realDate = reader["og_realDate"].ToString();
                    obj.og_unit = reader["og_unit"].ToString();
                    obj.og_createTime = (DateTime)reader["og_createTime"];
                    obj.og_invalidTime = (DateTime)reader["og_invalidTime"];
                    obj.og_status = (byte)reader["og_status"];
                    obj.og_updateTime = (DateTime)reader["og_updateTime"];
                    lst.Add(obj);
                }
                reader.NextResult();
                //rowcount = (int)dbCommandWrapper.Parameters["@rowCount"].Value;
            }


            return lst;
        }

        public List<Company_Sh> Company_Sh_Select(string oc_area, string column, string where, string order, int pg_index, int pg_size, string sp_Name)
        {
            var cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@area", DbType.String, oc_area);
            Db_0.AddInParameter(cmd, "@columns", DbType.String, column);
            Db_0.AddInParameter(cmd, "@where", DbType.String, where);
            Db_0.AddInParameter(cmd, "@order", DbType.String, order);
            Db_0.AddInParameter(cmd, "@page", DbType.Int32, pg_index);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, pg_size);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);

            var list = new List<Company_Sh>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                    list.Add(Company_Sh_Map_1(reader));

                return list;
            }
        }

        public Dictionary<string, string> OrgCompany_Tel_Get(DatabaseSearchModel s, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@Columns", DbType.String, s.Column);
            Db_0.AddInParameter(dbCommandWrapper, "@Where", DbType.String, s.Where);
            Db_0.AddInParameter(dbCommandWrapper, "@Order", DbType.String, s.Order);
            Db_0.AddInParameter(dbCommandWrapper, "@Page", DbType.Int32, s.PageIndex);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, s.PageSize);
            Db_0.AddOutParameter(dbCommandWrapper, "@rowCount", DbType.Int32, 4);
            Db_0.AddInParameter(dbCommandWrapper, "@tableName", DbType.String, s.Table);

            var dict = new Dictionary<string, string>();
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while (reader.Read())
                {
                    string year = reader["year"].ToString();
                    if (!dict.ContainsKey(year))
                    {
                        string tel = reader["phone"].ToString();
                        dict.Add(year, tel);
                    }
                }

                return dict;
            }
        }

        private Company_Sh Company_Sh_Map_1(IDataReader reader)
        {
            var sh = new Company_Sh();
            sh.oc_code = reader["og_oc_code"].ToString();
            sh.sh_name = reader["og_name"].ToString();
            sh.sh_money = (decimal)reader["og_subscribeAccount"];
            sh.sh_money_ratio = 0;
            sh.sh_money_unit = reader["og_unit"].ToString();
            sh.sh_cat = reader["og_type"].ToString();
            //sh.sh_type = reader["og_type"].ToString();
            sh.sh_status = (byte)reader["og_status"];
            return sh;
        }
        private Company_Sh Company_Sh_Map_0(IDataReader reader)
        {
            var sh = new Company_Sh();
            sh.oc_code = reader["og_oc_code"].ToString();
            sh.sh_name = reader["og_name"].ToString();
            sh.sh_money = (decimal)reader["og_money"];
            sh.sh_money_ratio = Math.Round((decimal)reader["og_BL"]/100, 2);
            sh.sh_money_unit = reader["og_unit"].ToString();
            sh.sh_cat = reader["og_pro"].ToString();
            sh.sh_type = reader["og_type"].ToString();
            //sh.sh_status = (byte)reader["og_status"];
            return sh;
        }

        public List<OrgCompanyDtl_EvtInfo> OrgCompanyDtl_Evt_Select(string column, string where, string order, int pg_index, int pg_size, string sp_Name)
        {
            var cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@columns", DbType.String, column);
            Db_0.AddInParameter(cmd, "@where", DbType.String, where);
            Db_0.AddInParameter(cmd, "@order", DbType.String, order);
            Db_0.AddInParameter(cmd, "@page", DbType.Int32, pg_index);
            Db_0.AddInParameter(cmd, "@pagesize", DbType.Int32, pg_size);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);

            var list = new List<OrgCompanyDtl_EvtInfo>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while(reader.Read())
                {
                    list.Add(OrgCompanyDtl_Evt_Get(reader));
                }
            }
            return list;
        }
        private OrgCompanyDtl_EvtInfo OrgCompanyDtl_Evt_Get(IDataReader reader)
        {
            var info = new OrgCompanyDtl_EvtInfo();
            info.oe_id = (int)reader["oe_id"];
            info.oe_oc_code = reader["oe_oc_code"].ToString();
            info.oe_date = reader["oe_date"].ToString();
            info.oe_type = reader["oe_type"].ToString();
            info.oe_dtl = reader["oe_dtl"].ToString();
            return info;
        }

        public List<OrgCompanyGsxtBgsxInfo> OrgCompanyGsxtBgsx_Select(string area, string columns, string where, string order, int page, int pagesize, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@area", DbType.String, area);
            Db_0.AddInParameter(cmd, "@columns", DbType.String, columns);
            Db_0.AddInParameter(cmd, "@where", DbType.String, where);
            Db_0.AddInParameter(cmd, "@order", DbType.String, order);
            Db_0.AddInParameter(cmd, "@page", DbType.Int32, page);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, pagesize);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);

            List<OrgCompanyGsxtBgsxInfo> lst = new List<OrgCompanyGsxtBgsxInfo>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    OrgCompanyGsxtBgsxInfo obj = new OrgCompanyGsxtBgsxInfo();
                    obj.id = (int)reader["id"];
                    obj.oc_code = reader["oc_code"].ToString();
                    obj.bgsx = reader["bgsx"].ToString();
                    obj.bgq = reader["bgq"].ToString();
                    obj.bgh = reader["bgh"].ToString();
                    obj.bgrq = (DateTime)reader["bgrq"];

                    lst.Add(obj);
                }

            }


            return lst;
        }

        public List<Company_Icpl> Company_Icpl_Select(string column, string where, string order, int pg_index, int pg_size, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@columns", DbType.String, column);
            Db_0.AddInParameter(cmd, "@where", DbType.String, where);
            Db_0.AddInParameter(cmd, "@order", DbType.String, order);
            Db_0.AddInParameter(cmd, "@page", DbType.Int32, pg_index);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, pg_size);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);

            List<Company_Icpl> lst = new List<Company_Icpl>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    
                    lst.Add(Company_Icpl_Map(reader));
                }
                reader.NextResult();
            }

            return lst;
        }
        public List<OrgCompanySiteInfo> OrgCompanySite_SelectPaged(DatabaseSearchModel s, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@columns", DbType.String, s.Column);
            Db_0.AddInParameter(dbCommandWrapper, "@where", DbType.String, s.Where);
            Db_0.AddInParameter(dbCommandWrapper, "@order", DbType.String, s.Order);
            Db_0.AddInParameter(dbCommandWrapper, "@page", DbType.Int32, s.PageIndex);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, s.PageSize);
            Db_0.AddOutParameter(dbCommandWrapper, "@rowCount", DbType.Int32, 4);

            List<OrgCompanySiteInfo> lst = new List<OrgCompanySiteInfo>();
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while (reader.Read())
                {
                    OrgCompanySiteInfo obj = new OrgCompanySiteInfo();
                    obj.ocs_id = (int)reader["ocs_id"];
                    obj.ocs_oc_code = reader["ocs_oc_code"].ToString();
                    obj.ocs_domain = reader["ocs_domain"].ToString();
                    obj.ocs_host = reader["ocs_host"].ToString();
                    obj.ocs_hostType = reader["ocs_hostType"].ToString();
                    obj.ocs_number = reader["ocs_number"].ToString();
                    obj.ocs_siteName = reader["ocs_siteName"].ToString();
                    obj.ocs_siteHomePage = reader["ocs_siteHomePage"].ToString();
                    obj.ocs_status = (byte)reader["ocs_status"];
                    obj.ocs_checkTime = (DateTime)reader["ocs_checkTime"];
                    obj.ocs_createTime = (DateTime)reader["ocs_createTime"];
                    obj.ocs_ext = reader["ocs_ext"].ToString();
                    lst.Add(obj);
                }
                reader.NextResult();
            }


            return lst;
        }

        private Company_Icpl Company_Icpl_Map(IDataReader reader)
        {
            Company_Icpl obj = new Company_Icpl();
            obj.icpl_id = (int)reader["ocs_id"];
            obj.oc_code = reader["ocs_oc_code"].ToString();
            obj.icpl_domain = reader["ocs_domain"].ToString();
            obj.icpl_host = reader["ocs_host"].ToString();
            obj.icpl_type = reader["ocs_hostType"].ToString();
            obj.icpl_number = reader["ocs_number"].ToString();
            obj.icpl_name = reader["ocs_siteName"].ToString();
            obj.icpl_uri = reader["ocs_siteHomePage"].ToString();
            obj.icpl_operate_status = (byte)reader["ocs_status"] == 0 ? "正常":"注销";
            obj.icpl_check_time = (DateTime)reader["ocs_checkTime"];
            obj.icpl_create_time = (DateTime)reader["ocs_createTime"];
            obj.icpl_ext = reader["ocs_ext"].ToString();
            return obj;
        }

        #region annual
        public List<Company_Annual_Abs> Company_Annual_Abs_Select(string oc_code, string oc_area, string year, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_SelectOrgCompanyGsxtNbByCodeSimple");
            Db_0.AddInParameter(cmd, "@oc_code", DbType.String, oc_code);
            Db_0.AddInParameter(cmd, "@areaCode", DbType.String, oc_area);
            Db_0.AddInParameter(cmd, "@year", DbType.String, year);

            List<Company_Annual_Abs> lst = new List<Company_Annual_Abs>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var obj = new Company_Annual_Abs();
                    obj.create_time = ((DateTime)reader["createTime"]).ToString("yyyy-MM-dd");
                    obj.oc_status = reader["runStatus"].ToString();
                    obj.year = reader["year"].ToString();
                    if (!string.IsNullOrEmpty(obj.year))
                        lst.Add(Company_Annual_Abs_Map(reader));
                }
                reader.NextResult();
            }


            return lst;
        }

        private Company_Annual_Abs Company_Annual_Abs_Map(IDataReader reader)
        {
            var obj = new Company_Annual_Abs();
            obj.create_time = ((DateTime)reader["createTime"]).ToString("yyyy-MM-dd");
            obj.oc_status = reader["runStatus"].ToString();
            obj.year = reader["year"].ToString();
            return obj;
        }

        public Company_Annual_Dtl Company_Annual_Dtl_Select(string oc_code, string oc_area, string year, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@oc_code", DbType.String, oc_code);
            Db_0.AddInParameter(cmd, "@areaCode", DbType.String, oc_area);
            Db_0.AddInParameter(cmd, "@year", DbType.String, year);

            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                
                if(reader.Read())
                {
                    var d = Company_Annual_Dtl.Candidate;
                    d.annual = Annual_Company_Map(reader);

                    reader.NextResult();
                    d.warranty_list = Annual_Oc_Warranty_Get(reader);

                    reader.NextResult();
                    d.invest_list = Annual_Oc_Invest_Get(reader);

                    reader.NextResult();
                    d.sh_contribute_list = Annual_Sh_Contribute_Get(reader);

                    reader.NextResult();
                    d.stock_change_list = Annual_Stock_Change_Get(reader);

                    reader.NextResult();
                    d.site_list = Annual_Oc_Site_Get(reader);

                    reader.NextResult();
                    d.change_list = Annual_Oc_Change_Get(reader);

                    return d;
                }
                else
                {
                    return Company_Annual_Dtl.Default;
                }
            }
        }
        
        private Annual_Company Annual_Company_Map(IDataReader reader)
        {
            var a = new Annual_Company();
            a.oc_code = reader["oc_code"].ToString();
            a.year = reader["year"].ToString();
            a.oc_name = reader["name"].ToString();
            a.oc_tel = reader["phone"].ToString();
            a.oc_post = reader["postCode"].ToString();
            a.oc_addr = reader["address"].ToString();
            a.oc_email = reader["mail"].ToString();
            a.flag_stock_transfer = (byte)reader["shareTransfer"]==1;
            a.flag_other_stock = (byte)reader["otherShare"]==1;
            a.flag_website = (byte)reader["webSite"] == 1;
            a.oc_status = reader["runStatus"].ToString();
            a.oc_assets = reader["totalAssets"].ToString();
            a.oc_equity_total = reader["totalEquity"].ToString();
            a.oc_income_total = reader["totalIncome"].ToString();
            a.oc_profit_total = reader["toalProfit"].ToString();
            a.oc_income_main = reader["mainIncome"].ToString();
            a.oc_profit_net = reader["netProfit"].ToString();
            a.oc_tax_total = reader["totalTax"].ToString();
            a.oc_debt_total = reader["totalDebt"].ToString();
            a.create_time = (DateTime)reader["createTime"];
            a.oc_number = reader["oc_number"].ToString();
            return a;
        }
        private List<Annual_Oc_Warranty> Annual_Oc_Warranty_Get(IDataReader reader)
        {
            var list = new List<Annual_Oc_Warranty>();
            
            while (reader.Read())
            {
                var w = new Annual_Oc_Warranty();
                w.oc_code = reader["oc_code"].ToString();
                w.year = reader["year"].ToString();
                w.creditor = reader["zcr"].ToString();
                w.debtor = reader["zwr"].ToString();
                w.credit_cat = reader["zzczl"].ToString();
                w.credit_amount = reader["zzcse"].ToString();
                w.credit_period = reader["lxzwqx"].ToString();
                w.warranty_period = reader["bzqj"].ToString();
                w.warranty_style = reader["bzfs"].ToString();
                w.warranty_range = reader["bzdbfw"].ToString();
                list.Add(w);
            }
            return list;
        }
        private List<Annual_Oc_Invest> Annual_Oc_Invest_Get(IDataReader reader)
        {
            var list = new List<Annual_Oc_Invest>();
            
            while(reader.Read())
            {
                var i = new Annual_Oc_Invest();
                i.oc_code = reader["oc_code"].ToString();
                i.year = reader["year"].ToString();
                i.invest_com = reader["compName"].ToString();
                i.reg_num = reader["regNumber"].ToString();
                list.Add(i);
            }
            return list;
        }
        private List<Annual_Sh_Contribute> Annual_Sh_Contribute_Get(IDataReader reader)
        {
            var list = new List<Annual_Sh_Contribute>();
            while(reader.Read())
            {
                var c = new Annual_Sh_Contribute();
                c.oc_code = reader["oc_code"].ToString();
                c.year = reader["year"].ToString();
                c.stock_holder = reader["gd"].ToString();
                c.subscribe_capital = reader["rjcze"].ToString();
                c.subscribe_time = (DateTime)reader["rjczsj"];
                c.subscribe_style = reader["rjczfs"].ToString();
                c.paid_contribute = reader["sjcze"].ToString();
                c.contribute_time = (DateTime)reader["czsj"];
                c.contribute_style = reader["czfs"].ToString();
                list.Add(c);
            }
            return list;
        }

        private List<Annual_Stock_Change> Annual_Stock_Change_Get(IDataReader reader)
        {
            var list = new List<Annual_Stock_Change>();
            
            while(reader.Read())
            {
                var c = new Annual_Stock_Change();
                c.oc_code = reader["oc_code"].ToString();
                c.year = reader["year"].ToString();
                c.stock_holder = reader["gd"].ToString();
                c.stock_ratio_pre = reader["qgqbl"].ToString();
                c.stock_ratio_post = reader["hgqbl"].ToString();
                c.stock_change_time = (DateTime)reader["bgsj"];
                list.Add(c);
            }
            return list;
        }

        private List<Annual_Oc_Site> Annual_Oc_Site_Get(IDataReader reader)
        {
            var list = new List<Annual_Oc_Site>();
            
            while(reader.Read())
            {
                var s = new Annual_Oc_Site();
                s.oc_code = reader["oc_code"].ToString();
                s.year = reader["year"].ToString();
                s.type = reader["type"].ToString();
                s.name = reader["name"].ToString();
                s.website = reader["webSite"].ToString();
                list.Add(s);
            }
            return list;
        }
        private List<Annual_Oc_Change> Annual_Oc_Change_Get(IDataReader reader)
        {
            var list = new List<Annual_Oc_Change>();
            
            while(reader.Read())
            {
                var c = new Annual_Oc_Change();
                c.oc_code = reader["oc_code"].ToString();
                c.year = reader["year"].ToString();
                c.change_item = reader["xgsx"].ToString();
                c.change_pre = reader["xgq"].ToString();
                c.change_post = reader["xgh"].ToString();
                c.change_date = (DateTime)reader["xgsj"];
                list.Add(c);
            }
            return list;
        }

        #endregion
        public int Company_Correct(CompanyInfoCorrectInfo info, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(dbCommandWrapper, "@cic_id", DbType.Int32, 4);
            Db_0.AddInParameter(dbCommandWrapper, "@cic_u_name", DbType.String, info.cic_u_name);
            Db_0.AddInParameter(dbCommandWrapper, "@cic_u_uid", DbType.Int32, info.cic_u_uid);
            Db_0.AddInParameter(dbCommandWrapper, "@cic_oc_code", DbType.String, info.cic_oc_code);
            Db_0.AddInParameter(dbCommandWrapper, "@cic_oc_name", DbType.String, info.cic_oc_name);
            Db_0.AddInParameter(dbCommandWrapper, "@cic_type", DbType.Int32, info.cic_type);
            Db_0.AddInParameter(dbCommandWrapper, "@cic_phone", DbType.String, info.cic_phone);
            Db_0.AddInParameter(dbCommandWrapper, "@cic_content", DbType.String, info.cic_content);
            Db_0.AddInParameter(dbCommandWrapper, "@cic_date", DbType.DateTime, info.cic_date);

            int _returnValue = Db_0.ExecuteNonQuery(dbCommandWrapper);
            int cic_id = (int)dbCommandWrapper.Parameters["@cic_id"].Value;

            return _returnValue;
        }
        public bool Company_Favorite_Exist(int u_id, string oc_code, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@fl_u_uid", DbType.Int32, u_id);
            Db_0.AddInParameter(cmd, "@fl_oc_code", DbType.String, oc_code);

            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if(reader.Read())
                {
                    return (int)reader["fl_valid"] == 1;
                }
                return false;
            }
        }
        public FavoriteViewTraceInfo FavoriteViewTrace_Select(int u_id, string oc_code, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@fvt_oc_code", DbType.String, oc_code);
            Db_0.AddInParameter(cmd, "@fvt_u_uid", DbType.Int32, u_id);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var i = new FavoriteViewTraceInfo();
                    i.fvt_id = (int)reader["fvt_id"];
                    i.fvt_oc_code = reader["fvt_oc_code"].ToString();
                    i.fvt_u_uid = (int)reader["fvt_u_uid"];
                    i.fvt_viewtime = (DateTime)reader["fvt_viewtime"];
                    i.fvt_status = (bool)reader["fvt_status"];
                    return i;
                }
                return null;
            }
        }
        public int FavoriteLog_Insert(FavoriteLogInfo obj, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(cmd, "@fl_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@fl_oc_name", DbType.String, obj.fl_oc_name);
            Db_0.AddInParameter(cmd, "@fl_oc_code", DbType.String, obj.fl_oc_code);
            Db_0.AddInParameter(cmd, "@fl_u_name", DbType.String, obj.fl_u_name);
            Db_0.AddInParameter(cmd, "@fl_u_uid", DbType.Int32, obj.fl_u_uid);
            Db_0.AddInParameter(cmd, "@fl_date", DbType.DateTime, obj.fl_date);
            Db_0.AddInParameter(cmd, "@fl_valid", DbType.Int32, obj.fl_valid);
            Db_0.AddInParameter(cmd, "@fl_oc_area", DbType.String, obj.fl_oc_area);

            Db_0.ExecuteNonQuery(cmd);
            int fl_id = (int)cmd.Parameters["@fl_id"].Value;

            return fl_id;
        }
        public List<string> Brand_Query_Hot(DatabaseSearchModel search, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@Columns", DbType.String, search.Column);
            Db_0.AddInParameter(cmd, "@Where", DbType.String, search.Where);
            Db_0.AddInParameter(cmd, "@Order", DbType.String, search.Order);
            Db_0.AddInParameter(cmd, "@Page", DbType.Int32, search.PageIndex);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, search.PageSize);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@tableName", DbType.String, search.Table);
            List<string> lst = new List<string>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    lst.Add(reader["ob_name"].ToString());
                }
                return lst;
            }
        }
        public List<string> Patent_Query_Hot(DatabaseSearchModel search, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@Columns", DbType.String, search.Column);
            Db_0.AddInParameter(cmd, "@Where", DbType.String, search.Where);
            Db_0.AddInParameter(cmd, "@Order", DbType.String, search.Order);
            Db_0.AddInParameter(cmd, "@Page", DbType.Int32, search.PageIndex);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, search.PageSize);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@tableName", DbType.String, search.Table);
            List<string> lst = new List<string>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    lst.Add(reader["Patent_Name"].ToString());
                }
                return lst;
            }
        }
        public List<string> Dishonest_Query_Hot(DatabaseSearchModel search, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@Columns", DbType.String, search.Column);
            Db_0.AddInParameter(cmd, "@Where", DbType.String, search.Where);
            Db_0.AddInParameter(cmd, "@Order", DbType.String, search.Order);
            Db_0.AddInParameter(cmd, "@Page", DbType.Int32, search.PageIndex);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, search.PageSize);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@tableName", DbType.String, search.Table);
            List<string> lst = new List<string>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    lst.Add(reader["sx_iname"].ToString());
                }
                return lst;
            }
        }

        public List<string> Exhibit_Query_Hot(DatabaseSearchModel search, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@Columns", DbType.String, search.Column);
            Db_0.AddInParameter(cmd, "@Where", DbType.String, search.Where);
            Db_0.AddInParameter(cmd, "@Order", DbType.String, search.Order);
            Db_0.AddInParameter(cmd, "@Page", DbType.Int32, search.PageIndex);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, search.PageSize);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@tableName", DbType.String, search.Table);
            List<string> lst = new List<string>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    lst.Add(reader["e_name"].ToString());
                }
                return lst;
            }
        }

        public List<string> Judge_Query_Hot(DatabaseSearchModel search, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@Columns", DbType.String, search.Column);
            Db_0.AddInParameter(cmd, "@Where", DbType.String, search.Where);
            Db_0.AddInParameter(cmd, "@Order", DbType.String, search.Order);
            Db_0.AddInParameter(cmd, "@Page", DbType.Int32, search.PageIndex);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, search.PageSize);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@tableName", DbType.String, search.Table);
            List<string> lst = new List<string>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    lst.Add(reader["jd_docTitle"].ToString());
                }
                return lst;
            }
        }

        public int FavoriteLog_Disable(string oc_code, int u_id, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@fl_oc_code", DbType.String, oc_code);
            Db_0.AddInParameter(cmd, "@fl_u_uid", DbType.Int32, u_id);

            return Db_0.ExecuteNonQuery(cmd);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns>true, if update successfully</returns>
        public int FavoriteViewTrace_Update(FavoriteViewTraceInfo i)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_FavoriteViewTrace_Update");
            Db_0.AddInParameter(cmd, "@fvt_id", DbType.Int32, i.fvt_id);
            Db_0.AddInParameter(cmd, "@fvt_oc_code", DbType.String, i.fvt_oc_code);
            Db_0.AddInParameter(cmd, "@fvt_u_uid", DbType.Int32, i.fvt_u_uid);
            Db_0.AddInParameter(cmd, "@fvt_viewtime", DbType.DateTime, i.fvt_viewtime);
            Db_0.AddInParameter(cmd, "@fvt_status", DbType.Boolean, i.fvt_status);

            return Db_0.ExecuteNonQuery(cmd);
        }

        public int FavoriteViewTrace_Insert(FavoriteViewTraceInfo i, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(cmd, "@fvt_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@fvt_oc_code", DbType.String, i.fvt_oc_code);
            Db_0.AddInParameter(cmd, "@fvt_u_uid", DbType.Int32, i.fvt_u_uid);
            Db_0.AddInParameter(cmd, "@fvt_viewtime", DbType.DateTime, i.fvt_viewtime);
            Db_0.AddInParameter(cmd, "@fvt_status", DbType.Boolean, i.fvt_status);

            return Db_0.ExecuteNonQuery(cmd);
        }
        public int TopicUserTrace_Status_Turnoff(string t_id, int u_id, string t_type, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@tut_t_id", DbType.String, t_id);
            Db_0.AddInParameter(dbCommandWrapper, "@tut_t_type", DbType.String, t_type);
            Db_0.AddInParameter(dbCommandWrapper, "@tut_uid", DbType.Int32, u_id);

            return Db_0.ExecuteNonQuery(dbCommandWrapper);
        }
        public int TopicUsersTrace_All_TurnOff(int u_id, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@tut_uid", DbType.Int32, u_id);

            return Db_0.ExecuteNonQuery(dbCommandWrapper);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="u_id"></param>
        /// <param name="oc_code"></param>
        /// <param name="sp_Name"></param>
        /// <returns>true, if user does up for this company</returns>
        public Topic_Up_Down Company_User_Up_Down(int u_id, string oc_code, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@lon_u_uid", DbType.Int32, u_id);
            Db_0.AddInParameter(cmd, "@lon_oc_code", DbType.String, oc_code);
            Db_0.AddInParameter(cmd, "@lon_type", DbType.Int32, 1);

            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    if ((int)reader["lon_valid"] == 1)
                        return Topic_Up_Down.Up;
                }
            }

            // check whether user does down for this company
            Db_0.SetParameterValue(cmd, "@lon_type", 2);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    if ((int)reader["lon_valid"] == 1)
                        return Topic_Up_Down.Down;
                }
            }
            return Topic_Up_Down.None;
        }
        public Topic_Dtl CompanyTopic_FromId_Get(int t_id, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@ctt_id", DbType.Int32, t_id);

            Topic_Dtl obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                if (reader.Read())
                {
                    obj = new Topic_Dtl();
                    obj.topic_id = (int)reader["ctt_id"];
                    obj.oc_code = reader["ctt_oc_code"].ToString();
                    obj.oc_name = reader["ctt_oc_name"].ToString();
                    obj.u_name = reader["ctt_u_name"].ToString();
                    obj.u_id = reader["ctt_u_uid"].ToString();
                    obj.topic_content = reader["ctt_content"].ToString();
                    obj.topic_date = (DateTime)reader["ctt_date"];
                    obj.oc_area = reader["ctt_oc_area"].ToString();
                    obj.u_face = reader["ctt_u_face"].ToString();
                    obj.topic_tag = reader["ctt_tag"]?.ToString() ?? string.Empty;
                    obj.status = Convert.ToInt32(reader["ctt_status"]) == 1;
                }
            }
            return obj;
        }
        public int Company_Topic_Insert(CompanyTieziTopicInfo info, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(cmd, "@ctt_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@ctt_oc_code", DbType.String, info.ctt_oc_code);
            Db_0.AddInParameter(cmd, "@ctt_oc_name", DbType.String, info.ctt_oc_name);
            Db_0.AddInParameter(cmd, "@ctt_u_name", DbType.String, info.ctt_u_name);
            Db_0.AddInParameter(cmd, "@ctt_u_uid", DbType.Int32, info.ctt_u_uid);
            Db_0.AddInParameter(cmd, "@ctt_content", DbType.String, info.ctt_content);
            Db_0.AddInParameter(cmd, "@ctt_date", DbType.DateTime, info.ctt_date);
            Db_0.AddInParameter(cmd, "@ctt_oc_area", DbType.String, info.ctt_oc_area);
            Db_0.AddInParameter(cmd, "@ctt_u_face", DbType.String, info.ctt_u_face);
            Db_0.AddInParameter(cmd, "@ctt_tag", DbType.String, info.ctt_tag);

            int _returnValue = Db_0.ExecuteNonQuery(cmd);
            return  (int)cmd.Parameters["@ctt_id"].Value;
        }
        public int TopicUsersTrace_Insert(TopicUsersTraceInfo info, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(cmd, "@tut_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@tut_t_id", DbType.String, info.tut_t_id);
            Db_0.AddInParameter(cmd, "@tut_t_type", DbType.String, info.tut_t_type);
            Db_0.AddInParameter(cmd, "@tut_uid", DbType.Int32, info.tut_uid);
            Db_0.AddInParameter(cmd, "@tut_t_count", DbType.Int32, info.tut_t_count);
            Db_0.AddInParameter(cmd, "@tut_status", DbType.Boolean, info.tut_status);

            return Db_0.ExecuteNonQuery(cmd);
        }

        public int TopicUsersTrace_Refresh(string t_id, string t_type, int u_id)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_TopicUsersTrace_Refresh");
            Db_0.AddInParameter(dbCommandWrapper, "@tut_id", DbType.Int32, 0);
            Db_0.AddInParameter(dbCommandWrapper, "@tut_t_id", DbType.String, t_id);
            Db_0.AddInParameter(dbCommandWrapper, "@tut_t_type", DbType.String, t_type);
            Db_0.AddInParameter(dbCommandWrapper, "@tut_uid", DbType.Int32, u_id);

            return Db_0.ExecuteNonQuery(dbCommandWrapper);
        }
        public int TopicUserTrace_Reset(string t_id, string t_type, int u_id, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@tut_t_id", DbType.String, t_id);
            Db_0.AddInParameter(dbCommandWrapper, "@tut_t_type", DbType.String, t_type);
            Db_0.AddInParameter(dbCommandWrapper, "@tut_uid", DbType.Int32, u_id);
            return Db_0.ExecuteNonQuery(dbCommandWrapper);
        }

        public int Company_Reply_Insert(CompanyTieziReplyInfo info, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(cmd, "@ctr_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@ctr_teizi", DbType.Int32, info.ctr_teizi);
            Db_0.AddInParameter(cmd, "@ctr_u_name", DbType.String, info.ctr_u_name);
            Db_0.AddInParameter(cmd, "@ctr_u_uid", DbType.Int32, info.ctr_u_uid);
            Db_0.AddInParameter(cmd, "@ctr_content", DbType.String, info.ctr_content);
            Db_0.AddInParameter(cmd, "@ctr_date", DbType.DateTime, info.ctr_date);

            return Db_0.ExecuteNonQuery(cmd);
            //int ctr_id = (int)cmd.Parameters["@ctr_id"].Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="uris"></param>
        /// <param name="sp_Name"></param>
        /// <returns>count of records which were inserted into database successfully</returns>
        public int CompanyTieziImage_Bulk_Insert(CompanyTieziImageInfo info, List<string> uris, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_CompanyTieziImage_Insert");
            Db_0.AddOutParameter(cmd, "@cti_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@cti_tiezi_id", DbType.Int32, info.cti_tiezi_id);
            Db_0.AddInParameter(cmd, "@cti_tiezi_type", DbType.Int32, info.cti_tiezi_type);
            Db_0.AddInParameter(cmd, "@cti_uid", DbType.Int32, info.cti_uid);
            Db_0.AddInParameter(cmd, "@cti_url", DbType.String, info.cti_url);

            int count = 0;
            foreach(var uri in uris)
            {
                Db_0.SetParameterValue(cmd, "@cti_url", uri);
                try
                {
                    Db_0.ExecuteNonQuery(cmd);
                    count++;
                }
                catch (Exception e)
                {
                }
            }
            return count;
        }
        public Judge_Dtl Judge_Dtl_Query(Guid guid, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@jd_id", DbType.Guid, guid);
            var dtl = new Judge_Dtl();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if(reader.Read())
                {
                    dtl.content = reader["jd_docContent"].ToString();
                    dtl.jdg_oc_code = reader["oc_code"].ToString();
                }
                return dtl;
            }
        }
        public Dishonest_Dtl Dishonest_Dtl_Query(int id, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@sx_id", DbType.Int32, id);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var dtl = new Dishonest_Dtl();
                    dtl.code = reader["sx_cardNum"].ToString();
                    dtl.approvel_unit = reader["sx_gistUnit"].ToString();
                    dtl.publish_date = ((DateTime)reader["sx_publishDate"]).ToString("yyyy-MM-dd");
                    dtl.disrupt = reader["sx_disruptTypeName"].ToString();
                    dtl.duty = reader["sx_duty"].ToString();
                    dtl.province = reader["sx_areaName"].ToString();
                    dtl.exe_no = reader["sx_gistId"].ToString();
                    return dtl;
                }
                return null;
            }
        }
        public Patent_Dtl Patent_Dtl_FromNoCat_Get(string p_no, string m_cat, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@patent_no", DbType.String, p_no);
            Db_0.AddInParameter(cmd, "@patent_gkh", DbType.String, m_cat);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var d = new Patent_Dtl();
                    //d.applicant = p.Patent_sqr;
                    //d.m_cat = p.Patent_lknflh;
                    //d.name = p.Patent_Name;
                    //d.application_date = p.Patent_Day;
                    //d.cat_no = p.Patent_flh;
                    d.oc_code = reader["oc_code"].ToString() ?? string.Empty;
                    d.grant_date = reader["Patent_gkr"].ToString() ?? string.Empty;
                    d.inventer = reader["Patent_sjr"].ToString() ?? string.Empty;
                    d.img = reader["Patent_img"].ToString() ?? string.Empty;
                    d.detail = reader["Patent_zy"].ToString() ?? string.Empty;
                    d.application_no = reader["Patent_No"].ToString() ?? string.Empty;
                    d.status = reader["Patent_Status"].ToString() ?? string.Empty;
                    d.type = reader["Patent_Type"].ToString() ?? string.Empty;
                    d.addr = reader["Patent_Addr"].ToString() ?? string.Empty;
                    d.priority = reader["Patent_yxq"].ToString() ?? string.Empty;
                    return d;
                }
                return null;
            }
        }

        public Patent_Dtl Patent_Dtl_Get(int id, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@id", DbType.Int32, id);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    var d = new Patent_Dtl();
                    //d.applicant = p.Patent_sqr;
                    //d.m_cat = p.Patent_lknflh;
                    //d.name = p.Patent_Name;
                    //d.application_date = p.Patent_Day;
                    //d.cat_no = p.Patent_flh;
                    d.oc_code = reader["oc_code"].ToString() ?? string.Empty;
                    d.grant_date = reader["Patent_gkr"].ToString() ?? string.Empty;
                    d.inventer = reader["Patent_sjr"].ToString() ?? string.Empty;
                    d.img = reader["Patent_img"].ToString() ?? string.Empty;
                    d.detail = reader["Patent_zy"].ToString() ?? string.Empty;
                    d.application_no = reader["Patent_No"].ToString() ?? string.Empty;
                    d.status = reader["Patent_Status"].ToString() ?? string.Empty;
                    d.type = reader["Patent_Type"].ToString() ?? string.Empty;
                    d.addr = reader["Patent_Addr"].ToString() ?? string.Empty;
                    d.priority = reader["Patent_yxq"].ToString() ?? string.Empty;
                    return d;
                }
                return null;
            }
        }
        public Brand_Dtl Brand_Dtl_Get(int id, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@ob_id", DbType.Int32, id);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if(reader.Read())
                {
                    var d = new Brand_Dtl();
                    d.agent = reader["ob_dlrmc"].ToString();
                    d.time_limit = reader["ob_zyksqx"].ToString() + " 至 " + reader["ob_zyjsqx"].ToString();
                    //d.oc_code = reader["ob_oc_code"].ToString();
                    var service = reader["oe_service"].ToString();
                    var code = reader["oe_serviceCode"].ToString();
                    var process = reader["oe_brandProcess"].ToString();
                    if (!string.IsNullOrEmpty(service))
                    {
                        var services = service.Split(';');
                        if (!string.IsNullOrEmpty(code))
                        {
                            var codes = code.Split(';');
                            int len = services.Length > codes.Length ? code.Length : services.Length;
                            for (int i = 0; i < len; i++)
                            {
                                services[i] = $"{services[i]} ({codes[i]})";
                            }
                        }
                        d.services = services.ToList();
                    }
                    else
                        d.services = new List<string>();

                    if (!string.IsNullOrEmpty(process))
                    {
                        var processes = process.Split(';');
                        var pro_lst = new List<Brand_Process>();
                        foreach (var p in processes)
                        {
                            var pair = p.Split('-');
                            if (pair.Length == 2)
                            {
                                var pro = new Brand_Process() { status = pair[0], date = pair[1] };
                                pro_lst.Add(pro);
                            }
                        }
                        d.process_list = pro_lst;
                    }
                    else
                        d.process_list = new List<Brand_Process>();

                    return d;
                }
                return new Brand_Dtl() { process_list = new List<Brand_Process>(), services = new List<string>() };
            }
        }

        public Brand_Dtl Brand_Dtl_FromRegClass_Get(string regno, string classno, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@oe_ob_regNo", DbType.String, regno);
            Db_0.AddInParameter(cmd, "@oe_ob_classNo", DbType.String, classno);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                var d = new Brand_Dtl();
                #region `brand_extension` table
                if (reader.Read())
                {
                    

                    //d.oc_code = reader["ob_oc_code"].ToString();
                    var service = reader["oe_service"].ToString();
                    var code = reader["oe_serviceCode"].ToString();
                    var process = reader["oe_brandProcess"].ToString();
                    if (!string.IsNullOrEmpty(service))
                    {
                        var services = service.Split(';');
                        if (!string.IsNullOrEmpty(code))
                        {
                            var codes = code.Split(';');
                            int len = services.Length > codes.Length ? code.Length : services.Length;
                            for (int i = 0; i < len; i++)
                            {
                                services[i] = $"{services[i]} ({codes[i]})";
                            }
                        }
                        d.services = services.ToList();
                    }
                    else
                        d.services = new List<string>();

                    if (!string.IsNullOrEmpty(process))
                    {
                        var processes = process.Split(';');
                        var pro_lst = new List<Brand_Process>();
                        foreach (var p in processes)
                        {
                            var pair = p.Split('-');
                            if (pair.Length == 2)
                            {
                                var pro = new Brand_Process() { status = pair[0], date = pair[1] };
                                pro_lst.Add(pro);
                            }
                        }
                        d.process_list = pro_lst;
                    }
                    else
                        d.process_list = new List<Brand_Process>();

                    
                }

                #endregion
                if(reader.NextResult() && reader.Read())
                {
                    d.agent = reader["ob_dlrmc"].ToString();
                    d.oc_code = reader["ob_oc_code"].ToString();
                    d.time_limit = reader["ob_zyksqx"].ToString() + " 至 " + reader["ob_zyjsqx"].ToString();
                }
                return d;
                //return new Brand_Dtl() { process_list = new List<Brand_Process>(), services = new List<string>() };
            }
        }

        public List<Province> Area_Get(string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@where", DbType.String, "where len([a_code])=2");


            List<Province> lst = new List<Province>();

            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var a = new Province();

                    a.a_code = reader["a_code"].ToString();
                    a.a_name = reader["a_name"].ToString();

                    lst.Add(a);
                }
            }
            foreach(var l in lst)
            {
                Db_0.SetParameterValue(cmd, "@where",  $"where len([a_code])=4 and a_code like '{l.a_code}%'");
                using (IDataReader reader = Db_0.ExecuteReader(cmd))
                {
                    l.children = new List<City>();
                    while (reader.Read())
                    {
                        var a = new City();
                        a.a_code = reader["a_code"].ToString();
                        a.a_name = reader["a_name"].ToString();
                        l.children.Add(a);
                    }
                }
            }
            return lst;
        }

        public List<Topic_Abs> Company_Topics_Get(string column, string where, string order, int pg_index, int pg_size, string sp_Name, out int count)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@columns", DbType.String, column);
            Db_0.AddInParameter(cmd, "@where", DbType.String, where);
            Db_0.AddInParameter(cmd, "@order", DbType.String, order);
            Db_0.AddInParameter(cmd, "@page", DbType.Int32, pg_index);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, pg_size);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);
            List<Topic_Abs> lst = new List<Topic_Abs>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var obj = new Topic_Abs();
                    obj.oc_code = reader["ctt_oc_code"].ToString();
                    obj.oc_name = reader["ctt_oc_name"].ToString();
                    obj.u_name = reader["ctt_u_name"].ToString();
                    obj.u_id = reader["ctt_u_uid"].ToString();
                    obj.topic_content = reader["ctt_content"].ToString();
                    obj.topic_date = (DateTime)reader["ctt_date"];
                    obj.oc_area = reader["ctt_oc_area"].ToString();
                    obj.u_face = reader["ctt_u_face"].ToString();
                    obj.topic_tag = reader["ctt_tag"]?.ToString() ?? string.Empty;
                    lst.Add(obj);
                }
                reader.NextResult();
                count = (int)cmd.Parameters["@rowCount"].Value;
            }

            return lst;
        }
        public List<Topic_Dtl> Company_Topics_Dtl_Get(string column, string where, string order, int pg_index, int pg_size, string sp_Name, out int count)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@columns", DbType.String, column);
            Db_0.AddInParameter(cmd, "@where", DbType.String, where);
            Db_0.AddInParameter(cmd, "@order", DbType.String, order);
            Db_0.AddInParameter(cmd, "@page", DbType.Int32, pg_index);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, pg_size);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);
            List<Topic_Dtl> lst = new List<Topic_Dtl>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var obj = new Topic_Dtl();
                    obj.oc_code = reader["ctt_oc_code"].ToString();
                    obj.oc_name = reader["ctt_oc_name"].ToString();
                    obj.u_name = reader["ctt_u_name"].ToString();
                    obj.u_id = reader["ctt_u_uid"].ToString();
                    obj.topic_content = reader["ctt_content"].ToString();
                    obj.topic_date = (DateTime)reader["ctt_date"];
                    obj.oc_area = reader["ctt_oc_area"].ToString();
                    obj.u_face = reader["ctt_u_face"].ToString();
                    obj.topic_tag = reader["ctt_tag"]?.ToString()??string.Empty;
                    obj.topic_id = (int)reader["ctt_id"];
                    lst.Add(obj);
                }
                reader.NextResult();
                count = (int)cmd.Parameters["@rowCount"].Value;
            }

            return lst;
        }

        public int Company_Topic_ReplyCount_Get(int t_id, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@tid", DbType.Int32, t_id);
            return (int)Db_0.ExecuteScalar(cmd);
        }
        public List<string> Comment_Imgs_Select(int t_id, int type, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@ati_tiezi_id", DbType.Int32, t_id);
            Db_0.AddInParameter(cmd, "@ati_tiezi_type", DbType.Int32, type);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                var list = new List<string>();
                while (reader.Read())
                {
                    list.Add(reader["ati_url"].ToString());
                }
                return list;
            }
        }
        public List<string> Oc_Imgs_Select(int t_id, int type, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@cti_tiezi_id", DbType.Int32, t_id);
            Db_0.AddInParameter(cmd, "@cti_tiezi_type", DbType.Int32, type);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                var list = new List<string>();
                while (reader.Read())
                {
                    list.Add(reader["cti_url"].ToString());
                }
                return list;
            }
        }
        public int Company_Topic_Up2Down(CompanyLikeOrNotLogInfo info)
        {
            // cancel up firstly
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_CompanyLikeOrNotLog_Cancel");
            Db_0.AddInParameter(dbCommandWrapper, "@cll_teizi", DbType.Int32, info.cll_teizi);
            Db_0.AddInParameter(dbCommandWrapper, "@cll_u_uid", DbType.Int32, info.cll_u_uid);
            Db_0.AddInParameter(dbCommandWrapper, "@cll_type", DbType.Int32, 1);

            if (Db_0.ExecuteNonQuery(dbCommandWrapper) > 0)  // succeed to cancel up, then vote down
            {
                info.cll_type = 2;
                info.cll_valid = 1;
                return Company_Topic_UpDown_Vote(info, "Proc_CompanyLikeOrNotLog_Insert");
            }

            return -1;  // operate failed
        }
        public int Company_Topic_Down2Up(CompanyLikeOrNotLogInfo info)
        {
            // cancel down firstly
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_CompanyLikeOrNotLog_Cancel");
            Db_0.AddInParameter(dbCommandWrapper, "@cll_teizi", DbType.Int32, info.cll_teizi);
            Db_0.AddInParameter(dbCommandWrapper, "@cll_u_uid", DbType.Int32, info.cll_u_uid);
            Db_0.AddInParameter(dbCommandWrapper, "@cll_type", DbType.Int32, 2);

            if (Db_0.ExecuteNonQuery(dbCommandWrapper) > 0)  // succeed to cancel up, then vote down
            {
                info.cll_type = 1;
                info.cll_valid = 1;
                return Company_Topic_UpDown_Vote(info, "Proc_CompanyLikeOrNotLog_Insert");
            }

            return -1;  // operate failed
        }
        public int Company_Topic_UpDown_Vote(CompanyLikeOrNotLogInfo info, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(dbCommandWrapper, "@cll_id", DbType.Int32, 4);
            Db_0.AddInParameter(dbCommandWrapper, "@cll_teizi", DbType.Int32, info.cll_teizi);
            Db_0.AddInParameter(dbCommandWrapper, "@cll_u_name", DbType.String, info.cll_u_name);
            Db_0.AddInParameter(dbCommandWrapper, "@cll_u_uid", DbType.Int32, info.cll_u_uid);
            Db_0.AddInParameter(dbCommandWrapper, "@cll_date", DbType.DateTime, info.cll_date);
            Db_0.AddInParameter(dbCommandWrapper, "@cll_type", DbType.Int32, info.cll_type);
            Db_0.AddInParameter(dbCommandWrapper, "@cll_valid", DbType.Int32, info.cll_valid);

            int _returnValue = Db_0.ExecuteNonQuery(dbCommandWrapper);
            int cll_id = (int)dbCommandWrapper.Parameters["@cll_id"].Value;

            return _returnValue;
        }
        public int Company_UpDown_Vote(LikeOrNotLogInfo obj, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(dbCommandWrapper, "@lon_id", DbType.Int32, 4);
            Db_0.AddInParameter(dbCommandWrapper, "@lon_u_name", DbType.String, obj.lon_u_name);
            Db_0.AddInParameter(dbCommandWrapper, "@lon_u_uid", DbType.Int32, obj.lon_u_uid);
            Db_0.AddInParameter(dbCommandWrapper, "@lon_oc_name", DbType.String, obj.lon_oc_name);
            Db_0.AddInParameter(dbCommandWrapper, "@lon_oc_code", DbType.String, obj.lon_oc_code);
            Db_0.AddInParameter(dbCommandWrapper, "@lon_date", DbType.DateTime, obj.lon_date);
            Db_0.AddInParameter(dbCommandWrapper, "@lon_valid", DbType.Int32, obj.lon_valid);
            Db_0.AddInParameter(dbCommandWrapper, "@lon_oc_area", DbType.String, obj.lon_oc_area);
            Db_0.AddInParameter(dbCommandWrapper, "@lon_type", DbType.Int32, obj.lon_type);

            int _returnValue = Db_0.ExecuteNonQuery(dbCommandWrapper);
            int lon_id = (int)dbCommandWrapper.Parameters["@lon_id"].Value;

            return _returnValue;
        }
        public int Company_Up2Down(LikeOrNotLogInfo info)
        {
            // cancel up firstly
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_LikeOrNotLog_Cancel");
            Db_0.AddInParameter(dbCommandWrapper, "@lon_u_uid", DbType.Int32, info.lon_u_uid);
            Db_0.AddInParameter(dbCommandWrapper, "@lon_oc_code", DbType.String, info.lon_oc_code);
            Db_0.AddInParameter(dbCommandWrapper, "@lon_type", DbType.Int32, 1);

            if (Db_0.ExecuteNonQuery(dbCommandWrapper) > 0)  // succeed to cancel up, then vote down
            {
                info.lon_type = 2;
                return Company_UpDown_Vote(info, "Proc_LikeOrNotLog_Insert");
            }

            return -1;  // operate failed
        }
        public int Company_Down2Up(LikeOrNotLogInfo info)
        {
            // cancel down firstly
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_LikeOrNotLog_Cancel");
            Db_0.AddInParameter(dbCommandWrapper, "@lon_u_uid", DbType.Int32, info.lon_u_uid);
            Db_0.AddInParameter(dbCommandWrapper, "@lon_oc_code", DbType.String, info.lon_oc_code);
            Db_0.AddInParameter(dbCommandWrapper, "@lon_type", DbType.Int32, 2);

            if (Db_0.ExecuteNonQuery(dbCommandWrapper) > 0)  // succeed to cancel up, then vote down
            {
                info.lon_type = 1;
                return Company_UpDown_Vote(info, "Proc_LikeOrNotLog_Insert");
            }

            return -1;  // operate failed
        }
        /// <summary>
        /// 
        /// </summary>
        /// up -> 1, down -> 2
        /// <param name="t_id"></param>
        /// <param name="sp_Name"></param>
        /// <returns></returns>
        public Tuple<int, int> Company_Topic_UpDown_Count_Get(int t_id, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@cll_type", DbType.Int32, 1);
            Db_0.AddInParameter(cmd, "@cll_teizi", DbType.Int32, t_id);

            int up_count = 0;
            int down_count = 0;
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                    up_count = (int)reader[0];
            }

            Db_0.SetParameterValue(cmd, "@cll_type", 2);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                    down_count = (int)reader[0];
            }
            return new Tuple<int, int>(up_count, down_count);
        }

        /// <summary>
        /// up_type = 1; down_type = 2
        /// </summary>
        /// <param name="t_id"></param>
        /// <param name="u_id"></param>
        /// <param name="sp_Name"></param>
        /// <returns></returns>
        public Tuple<int, int> Company_Topic_UpDown_Flag_Get(int t_id, int u_id, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@cll_teizi", DbType.Int32, t_id);
            Db_0.AddInParameter(cmd, "@cll_u_uid", DbType.Int32, u_id);
            Db_0.AddInParameter(cmd, "@cll_type", DbType.Int32, 1);

            int up = 0;
            int down = 0;
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                    up = (int)reader["cll_valid"];
            }

            Db_0.SetParameterValue(cmd, "@cll_type", 2);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                    down = (int)reader["cll_valid"];
            }
            return new Tuple<int, int>(up, down);
        }


        public List<Reply_Dtl> Replies_Dtl_Select(string column, string where, string order, int pg_index, int pg_size, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@columns", DbType.String, column);
            Db_0.AddInParameter(cmd, "@where", DbType.String, where);
            Db_0.AddInParameter(cmd, "@order", DbType.String, order);
            Db_0.AddInParameter(cmd, "@page", DbType.Int32, pg_index);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, pg_size);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);

            List<Reply_Dtl> lst = new List<Reply_Dtl>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    Reply_Dtl obj = new Reply_Dtl();
                    obj.reply_id = (int)reader["ctr_id"];
                    obj.topic_id = (int)reader["ctr_teizi"];
                    obj.u_name = reader["ctr_u_name"].ToString();
                    obj.u_id = reader["ctr_u_uid"].ToString();
                    obj.reply_content = reader["ctr_content"].ToString();
                    obj.reply_date = (DateTime)reader["ctr_date"];
                    lst.Add(obj);
                }
            }
            return lst;
        }
        public List<Reply_Dtl> Cm_Replies_Dtl_Select(string column, string where, string order, int pg_index, int pg_size, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@columns", DbType.String, column);
            Db_0.AddInParameter(cmd, "@where", DbType.String, where);
            Db_0.AddInParameter(cmd, "@order", DbType.String, order);
            Db_0.AddInParameter(cmd, "@page", DbType.Int32, pg_index);
            Db_0.AddInParameter(cmd, "@pageSize", DbType.Int32, pg_size);
            Db_0.AddOutParameter(cmd, "@rowCount", DbType.Int32, 4);

            List<Reply_Dtl> lst = new List<Reply_Dtl>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    Reply_Dtl obj = new Reply_Dtl();
                    obj.reply_id = (int)reader["atr_id"];
                    obj.topic_id = (int)reader["atr_teizi"];
                    obj.u_name = reader["atr_u_name"].ToString();
                    obj.u_id = reader["atr_u_uid"].ToString();
                    obj.reply_content = reader["atr_content"].ToString();
                    obj.reply_date = (DateTime)reader["atr_date"];
                    lst.Add(obj);
                }
            }
            return lst;
        }

        public List<History_Query> SearchHistory_PageSelect(DatabaseSearchModel search, int u_id, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@Columns", DbType.String, search.Column);
            Db_0.AddInParameter(dbCommandWrapper, "@tableName", DbType.String, string.Format("SearchHistory_{0:D3}", u_id % 256));
            Db_0.AddInParameter(dbCommandWrapper, "@Where", DbType.String, search.Where);
            Db_0.AddInParameter(dbCommandWrapper, "@Order", DbType.String, search.Order);
            Db_0.AddInParameter(dbCommandWrapper, "@Page", DbType.Int32, search.PageIndex);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, search.PageSize);


            List<History_Query> lst = new List<History_Query>();
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while (reader.Read())
                {
                    //sh_oc_name, max(sh_id) as max_id, max(sh_date) as max_date, max(sh_oc_code) as max_oc_code, max(sh_oc_area) as max_oc_area
                    History_Query obj = new History_Query();
                    obj.oc_name = reader["sh_oc_name"].ToString();
                    //obj.oc_area = reader["sh_oc_area"].ToString();
                    obj.oc_area = reader["max_oc_area"].ToString();
                    //obj.oc_area_name = reader["sh_oc_areaName"].ToString();
                    //obj.oc_code = reader["sh_oc_code"].ToString();
                    obj.oc_code = reader["max_oc_code"].ToString();
                    //obj.query_id = (int)reader["sh_id"];
                    obj.query_id = (int)reader["max_id"];
                    //obj.query_date = ((DateTime)reader["sh_date"]).ToString("yyyy-MM-dd:hh-mm-ss");
                    obj.query_date = ((DateTime)reader["max_date"]).ToString("yyyy-MM-dd:hh-mm-ss");
                    obj.q_type = 0;// (int)reader["sh_searchType"] - 1;
                    //obj.oc_addr = reader["sh_oc_address"].ToString();
                    //obj.oc_reg_type = reader["sh_od_regType"].ToString();
                    //obj.oc_reg_capital_floor = reader["sh_od_regMLower"].ToString();
                    //obj.oc_reg_capital_ceiling = reader["sh_od_regMUpper"].ToString();
                    //obj.oc_sort = (int)reader["sh_od_orderBy"];
                    //obj.oc_stock_holder = reader["sh_od_gd"].ToString();
                    //obj.oc_number = reader["sh_oc_number"].ToString();
                    //History_Query obj = new History_Query();
                    //obj.oc_name = reader["sh_oc_name"].ToString();
                    //obj.oc_area = reader["sh_oc_area"].ToString();
                    //obj.oc_area_name = reader["sh_oc_areaName"].ToString();
                    //obj.oc_code = reader["sh_oc_code"].ToString();
                    //obj.query_id = (int)reader["sh_id"];
                    //obj.query_date = ((DateTime)reader["sh_date"]).ToString("yyyy-MM-dd:hh-mm-ss");
                    //obj.q_type = (int)reader["sh_searchType"] - 1;
                    //obj.oc_addr = reader["sh_oc_address"].ToString();
                    //obj.oc_reg_type = reader["sh_od_regType"].ToString();
                    //obj.oc_reg_capital_floor = reader["sh_od_regMLower"].ToString();
                    //obj.oc_reg_capital_ceiling = reader["sh_od_regMUpper"].ToString();
                    //obj.oc_sort = (int)reader["sh_od_orderBy"];
                    //obj.oc_stock_holder = reader["sh_od_gd"].ToString();
                    //obj.oc_number = reader["sh_oc_number"].ToString();
                    //obj.oc_ext = reader["sh_od_ext"].ToString();
                    lst.Add(obj);
                }
                reader.NextResult();
                //count = (int)dbCommandWrapper.Parameters["@rowCount"].Value;
            }

            return lst;
        }
        public List<string> Ext_SearchHistory_Page_Select(DatabaseSearchModel search, int u_id, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@Columns", DbType.String, search.Column);
            Db_0.AddInParameter(dbCommandWrapper, "@tableName", DbType.String, string.Format("SearchHistoryExt_{0:D3}", u_id % 256));
            Db_0.AddInParameter(dbCommandWrapper, "@Where", DbType.String, search.Where);
            Db_0.AddInParameter(dbCommandWrapper, "@Order", DbType.String, search.Order);
            Db_0.AddInParameter(dbCommandWrapper, "@Page", DbType.Int32, search.PageIndex);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, search.PageSize);
            Db_0.AddOutParameter(dbCommandWrapper, "@rowCount", DbType.Int32, 4);
            var list = new List<string>();
            using(IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while(reader.Read())
                {
                    list.Add(reader["sh_str"].ToString());
                }
            }
            return list;
        }
        public List<History_Query> History_Queries_Get(DatabaseSearchModel search, int u_id, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@Columns", DbType.String, search.Column);
            Db_0.AddInParameter(dbCommandWrapper, "@tableName", DbType.String, string.Format("SearchHistory_{0:D3}", u_id%256));
            Db_0.AddInParameter(dbCommandWrapper, "@Where", DbType.String, search.Where);
            Db_0.AddInParameter(dbCommandWrapper, "@Order", DbType.String, search.Order);
            Db_0.AddInParameter(dbCommandWrapper, "@Page", DbType.Int32, search.PageIndex);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, search.PageSize);
            Db_0.AddOutParameter(dbCommandWrapper, "@rowCount", DbType.Int32, 4);


            List<History_Query> lst = new List<History_Query>();
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while (reader.Read())
                {
                    History_Query obj = new History_Query();
                    obj.query_id = (int)reader["max_id"];
                    obj.oc_name = reader["sh_oc_name"].ToString();
                    obj.oc_area = reader["max_oc_area"].ToString();
                    //obj.oc_area_name = reader["sh_oc_areaName"].ToString();
                    //obj.sh_od_regMLower = (decimal)reader["sh_od_regMLower"];
                    //obj.sh_od_regMUpper = (decimal)reader["sh_od_regMUpper"];
                    //obj.oc_reg_type = reader["sh_od_regType"].ToString();
                    obj.oc_code = reader["max_oc_code"].ToString();
                    //obj.oc_number = reader["sh_oc_number"].ToString();
                    //obj.sh_od_faRen = reader["sh_od_faRen"].ToString();
                    //obj.sh_od_gd = reader["sh_od_gd"].ToString();
                    //obj.sh_od_bussinessDes = reader["sh_od_bussinessDes"].ToString();
                    //obj.u_name = reader["sh_u_name"].ToString();
                    //obj.sh_u_uid = (int)reader["sh_u_uid"];
                    obj.query_date = ((DateTime)reader["max_date"]).ToString("yyyy-MM-dd:hh-mm-ss");
                    //obj.sh_od_ext = reader["sh_od_ext"].ToString();
                    //obj.sh_od_orderBy = (int)reader["sh_od_orderBy"];
                    //obj.oc_addr = reader["sh_oc_address"].ToString();
                    lst.Add(obj);
                }
                reader.NextResult();
                //count = (int)dbCommandWrapper.Parameters["@rowCount"].Value;
            }

            return lst;
        }
        public int Query_Delete(int u_id, string oc_name, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@sh_u_uid", DbType.Int32, u_id);
            Db_0.AddInParameter(dbCommandWrapper, "@sh_oc_name", DbType.String, oc_name);
            Db_0.AddInParameter(dbCommandWrapper, "@tableName", DbType.String, string.Format("SearchHistory_{0:D3}", u_id % 256));
            int _returnValue = Db_0.ExecuteNonQuery(dbCommandWrapper);

            return _returnValue;
        }
        public int Ext_SearchHistory_Drop(int u_id, byte q_type, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@sh_uid", DbType.Int32, u_id);
            Db_0.AddInParameter(cmd, "@sh_type", DbType.Byte, q_type);
            Db_0.AddInParameter(cmd, "@tableName", DbType.String, string.Format("SearchHistoryExt_{0:D3}", u_id % 256));
            return Db_0.ExecuteNonQuery(cmd);
        }

        public int Query_Drop(int u_id, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@sh_u_uid", DbType.Int32, u_id);
            Db_0.AddInParameter(dbCommandWrapper, "@tableName", DbType.String, string.Format("SearchHistory_{0:D3}", u_id % 256));
            int _returnValue = Db_0.ExecuteNonQuery(dbCommandWrapper);

            return _returnValue;
        }
        public List<Favorite_Log> Favorites_Get(DatabaseSearchModel search, string sp_Name, out int count)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_FavoriteLog_SelectPaged");
            Db_0.AddInParameter(dbCommandWrapper, "@columns", DbType.String, search.Column);
            Db_0.AddInParameter(dbCommandWrapper, "@where", DbType.String, search.Where);
            Db_0.AddInParameter(dbCommandWrapper, "@order", DbType.String, search.Order);
            Db_0.AddInParameter(dbCommandWrapper, "@page", DbType.Int32, search.PageIndex);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, search.PageSize);
            Db_0.AddOutParameter(dbCommandWrapper, "@rowCount", DbType.Int32, 4);

            List<Favorite_Log> lst = new List<Favorite_Log>();
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while (reader.Read())
                {
                    Favorite_Log obj = new Favorite_Log();
                    obj.favorite_id = (int)reader["fl_id"];
                    obj.oc_name = reader["fl_oc_name"].ToString();
                    obj.oc_code = reader["fl_oc_code"].ToString();
                    //obj.fl_u_name = reader["fl_u_name"].ToString();
                    //obj.fl_u_uid = (int)reader["fl_u_uid"];
                    obj.favorite_date = ((DateTime)reader["fl_date"]).ToString("yyyy-MM-dd:hh-mm-ss");
                    //obj.fl_valid = (int)reader["fl_valid"];
                    obj.oc_area = reader["fl_oc_area"].ToString();
                    lst.Add(obj);
                }
                reader.NextResult();

                count = (int)dbCommandWrapper.Parameters["@rowCount"].Value;
            }


            return lst;
        }
        public List<Oc_Notice> FavoriteTraces_Get(DatabaseSearchModel search, string sp_Name, out int count)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_Favorite_View_Trace_SelectPaged");
            Db_0.AddInParameter(dbCommandWrapper, "@columns", DbType.String, search.Column);
            Db_0.AddInParameter(dbCommandWrapper, "@where", DbType.String, search.Where);
            Db_0.AddInParameter(dbCommandWrapper, "@order", DbType.String, search.Order);
            Db_0.AddInParameter(dbCommandWrapper, "@page", DbType.Int32, search.PageIndex);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, search.PageSize);
            Db_0.AddOutParameter(dbCommandWrapper, "@rowCount", DbType.Int32, 4);

            List<Oc_Notice> lst = new List<Oc_Notice>();
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while (reader.Read())
                {
                    Oc_Notice obj = new Oc_Notice();
                    obj.oc_code = reader["ct_oc_code"].ToString();
                    obj.oc_name = reader["ct_oc_name"].ToString();
                    obj.notice_date = ((DateTime)reader["ct_createtime"]).ToString("yyyy.MM.dd");
                    //obj.ct_lvl = (int)reader["ct_lvl"];
                    //obj.fl_u_uid = (int)reader["fl_u_uid"];
                    obj.oc_area = reader["fl_oc_area"].ToString();
                    obj.read_flag = (int)reader["IsRead"] != 1;
                    lst.Add(obj);
                }
                reader.NextResult();
                count = (int)dbCommandWrapper.Parameters["@rowCount"].Value;
            }
            return lst;
        }
        public List<Topic_Notice> Notice_Topics_Get(DatabaseSearchModel search, string sp_Name, out int count)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@columns", DbType.String, search.Column);
            Db_0.AddInParameter(dbCommandWrapper, "@where", DbType.String, search.Where);
            Db_0.AddInParameter(dbCommandWrapper, "@order", DbType.String, search.Order);
            Db_0.AddInParameter(dbCommandWrapper, "@page", DbType.Int32, search.PageIndex);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, search.PageSize);
            Db_0.AddOutParameter(dbCommandWrapper, "@rowCount", DbType.Int32, 4);

            List<Topic_Notice> lst = new List<Topic_Notice>();
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while (reader.Read())
                {
                    Topic_Notice obj = new Topic_Notice();
                    obj.notice_id = (int)reader["tut_id"];
                    obj.topic_id = int.Parse(reader["tut_t_id"].ToString());
                    obj.topic_type = int.Parse(reader["tut_t_type"].ToString());
                    obj.count = (int)reader["tut_t_count"];
                    lst.Add(obj);
                }
                reader.NextResult();
                count = (int)dbCommandWrapper.Parameters["@rowCount"].Value;
            }
            return lst;
        }
        public FavoriteViewTraceInfo FavoriteViewTrace_FromUidOccode_Get(string oc_code, int u_id, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@fvt_oc_code", DbType.String, oc_code);
            Db_0.AddInParameter(dbCommandWrapper, "@fvt_u_uid", DbType.Int32, u_id);

            FavoriteViewTraceInfo obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                if (reader.Read())
                {
                    obj = new FavoriteViewTraceInfo();
                    obj.fvt_id = (int)reader["fvt_id"];
                    obj.fvt_oc_code = reader["fvt_oc_code"].ToString();
                    obj.fvt_u_uid = (int)reader["fvt_u_uid"];
                    obj.fvt_viewtime = (DateTime)reader["fvt_viewtime"];
                    obj.fvt_status = (bool)reader["fvt_status"];
                }
            }
            return obj;
        }

        public List<Browse_Log> Browses_Get_1(DatabaseSearchModel search, int u_id, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@Columns", DbType.String, search.Column);
            Db_0.AddInParameter(dbCommandWrapper, "@Where", DbType.String, search.Where);
            Db_0.AddInParameter(dbCommandWrapper, "@Order", DbType.String, search.Order);
            Db_0.AddInParameter(dbCommandWrapper, "@Page", DbType.Int32, search.PageIndex);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, search.PageSize);
            Db_0.AddInParameter(dbCommandWrapper, "@tableName", DbType.String, string.Format("BrowseLog_{0}", u_id % 256));

            List<Browse_Log> lst = new List<Browse_Log>();
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while (reader.Read())
                {
                    //Browse_Log obj = new Browse_Log();
                    //obj.browse_id = (int)reader["bl_id"];
                    //obj.oc_name = reader["bl_oc_name"].ToString();
                    //obj.oc_code = reader["bl_oc_code"].ToString();
                    //obj.oc_area = reader["bl_oc_area"].ToString();
                    ////obj.bl_u_name = reader["bl_u_name"].ToString();
                    ////obj.bl_u_uid = (int)reader["bl_u_uid"];
                    //obj.browse_date = (DateTime)reader["bl_date"];
                    ////obj.bl_ip = reader["bl_ip"].ToString();
                    ////obj.bl_osName = reader["bl_osName"].ToString();
                    ////obj.bl_appVer = reader["bl_appVer"].ToString();
                    //lst.Add(obj);
                    Browse_Log obj = new Browse_Log();
                    obj.browse_id = (int)reader["max_id"];
                    //obj.browse_id = (int)reader["bl_id"];
                    obj.oc_name = reader["bl_oc_name"].ToString();
                    //obj.oc_code = reader["bl_oc_code"].ToString();
                    obj.oc_code = reader["max_oc_code"].ToString();
                    //obj.oc_area = reader["bl_oc_area"].ToString();
                    obj.oc_area = reader["max_oc_area"].ToString();
                    //obj.bl_u_name = reader["bl_u_name"].ToString();
                    //obj.bl_u_uid = (int)reader["bl_u_uid"];
                    //obj.browse_date = (DateTime)reader["bl_date"];
                    obj.browse_date = ((DateTime)reader["max_date"]).ToString("yyyy-MM-dd:hh-mm-ss");
                    //obj.bl_ip = reader["bl_ip"].ToString();
                    //obj.bl_osName = reader["bl_osName"].ToString();
                    //obj.bl_appVer = reader["bl_appVer"].ToString();
                    lst.Add(obj);
                }
                reader.NextResult();
                //count = (int)dbCommandWrapper.Parameters["@rowCount"].Value;
            }


            return lst;
        }

        public List<Browse_Log> Browses_Get(DatabaseSearchModel search, int u_id, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_BaseBetween_SelectByPageIndex");
            Db_0.AddInParameter(dbCommandWrapper, "@Columns", DbType.String, search.Column);
            Db_0.AddInParameter(dbCommandWrapper, "@Where", DbType.String, search.Where);
            Db_0.AddInParameter(dbCommandWrapper, "@Order", DbType.String, search.Order);
            Db_0.AddInParameter(dbCommandWrapper, "@Page", DbType.Int32, search.PageIndex);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, search.PageSize);
            Db_0.AddOutParameter(dbCommandWrapper, "@rowCount", DbType.Int32, 4);
            Db_0.AddInParameter(dbCommandWrapper, "@tableName", DbType.String, string.Format("BrowseLog_{0}", u_id % 256));

            List<Browse_Log> lst = new List<Browse_Log>();
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while (reader.Read())
                {
                    //Browse_Log obj = new Browse_Log();
                    //obj.browse_id = (int)reader["max_id"];
                    //obj.oc_name = reader["bl_oc_name"].ToString();
                    //obj.oc_code = reader["max_oc_code"].ToString();
                    //obj.oc_area = reader["max_oc_area"].ToString();
                    ////obj.bl_u_name = reader["bl_u_name"].ToString();
                    ////obj.bl_u_uid = (int)reader["bl_u_uid"];
                    //obj.browse_date = (DateTime)reader["max_date"];
                    ////obj.bl_ip = reader["bl_ip"].ToString();
                    ////obj.bl_osName = reader["bl_osName"].ToString();
                    ////obj.bl_appVer = reader["bl_appVer"].ToString();
                    //lst.Add(obj);
                    Browse_Log obj = new Browse_Log();
                    obj.browse_id = (int)reader["max_id"];
                    obj.oc_name = reader["bl_oc_name"].ToString();
                    obj.oc_code = reader["max_oc_code"].ToString();
                    obj.oc_area = reader["max_oc_area"].ToString();
                    //obj.bl_u_name = reader["bl_u_name"].ToString();
                    //obj.bl_u_uid = (int)reader["bl_u_uid"];
                    obj.browse_date = ((DateTime)reader["max_date"]).ToString("yyyy-MM-dd:hh-mm-ss");
                    //obj.bl_ip = reader["bl_ip"].ToString();
                    //obj.bl_osName = reader["bl_osName"].ToString();
                    //obj.bl_appVer = reader["bl_appVer"].ToString();
                    lst.Add(obj);
                }
                reader.NextResult();
                //count = (int)dbCommandWrapper.Parameters["@rowCount"].Value;
            }


            return lst;
        }
        public int Browses_Delete(int u_id, string oc_name, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@bl_u_uid", DbType.Int32, u_id);
            Db_0.AddInParameter(dbCommandWrapper, "@bl_oc_name", DbType.String, oc_name);
            Db_0.AddInParameter(dbCommandWrapper, "@tableName", DbType.String, string.Format("BrowseLog_{0}", u_id % 256));

            int _returnValue = Db_0.ExecuteNonQuery(dbCommandWrapper);

            return _returnValue;
        }
        public int Browses_Drop(int u_id, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@bl_u_uid", DbType.Int32, u_id);
            Db_0.AddInParameter(dbCommandWrapper, "@tableName", DbType.String, string.Format("BrowseLog_{0}", u_id % 256));

            int _returnValue = Db_0.ExecuteNonQuery(dbCommandWrapper);

            return _returnValue;
        }
        public bool TopicTrace_Status_Get(int u_id, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@tut_uid", DbType.Int32, u_id);

            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                return reader.Read();
            }
        }
        public User_Mini_Info User_FromId_Select(int u_id)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_Users_Selectbyu_uid");
            Db_0.AddInParameter(cmd, "@u_uid", DbType.Int32, u_id);

            User_Mini_Info obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    obj = new User_Mini_Info();
                    obj.u_id = (int)reader["u_uid"];
                    obj.u_type = (byte)reader["u_type"];
                    obj.u_name = reader["u_name"].ToString();
                    obj.u_face = reader["u_face"].ToString();
                    obj.u_email = reader["u_email"].ToString();
                    obj.u_email_status = (byte)reader["u_status_email"] == 1;
                }
                return obj;
            }
        }

        public User_Dtl User_FromName_Select_1(string u_name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_Users_Selectbyusername");
            Db_0.AddInParameter(cmd, "@u_name", DbType.Int32, u_name);

            User_Dtl obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    obj = new User_Dtl();
                    obj.u_id = (int)reader["u_uid"];
                    obj.u_type = (byte)reader["u_type"];
                    obj.u_name = reader["u_name"].ToString();
                    obj.u_face = reader["u_face"].ToString();
                    obj.u_email = reader["u_email"].ToString();
                    obj.u_status = (int)reader["u_status"];
                    obj.last_login_time = reader["u_prevLoginTime"].ToString();
                    obj.cur_login_time = reader["u_curLoginTime"].ToString();
                    obj.u_exp = (int)reader["u_total_exp"];
                    obj.u_pwd = reader["u_pwd"].ToString();
                }
                return obj;
            }
        }

        public User_Dtl User_FromEmail_Select_1(string u_email)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_Users_Selectbyus_email");
            Db_0.AddInParameter(cmd, "@u_email", DbType.Int32, u_email);

            User_Dtl obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    obj = new User_Dtl();
                    obj.u_id = (int)reader["u_uid"];
                    obj.u_type = (byte)reader["u_type"];
                    obj.u_name = reader["u_name"].ToString();
                    obj.u_face = reader["u_face"].ToString();
                    obj.u_email = reader["u_email"].ToString();
                    obj.u_status = (int)reader["u_status"];
                    obj.last_login_time = reader["u_prevLoginTime"].ToString();
                    obj.cur_login_time = reader["u_curLoginTime"].ToString();
                    obj.u_exp = (int)reader["u_total_exp"];
                    obj.u_pwd = reader["u_pwd"].ToString();
                }
                return obj;
            }
        }

        #region classic user getting
        public UserInfo User_FromId_Select_2(int u_id)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_Users_Selectbyu_uid");
            Db_0.AddInParameter(dbCommandWrapper, "@u_uid", DbType.Int32, u_id);

            UserInfo obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                if (reader.Read())
                {
                    obj = GetUser(reader);
                }
                return obj;
            }
        }
        public UserInfo User_FromName_Select_2(string u_name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_Users_Selectbyusername");
            Db_0.AddInParameter(dbCommandWrapper, "@u_name", DbType.String, u_name);

            UserInfo obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                if (reader.Read())
                {
                    obj = GetUser(reader);
                }
            }


            return obj;
        }
        public UserInfo User_FromEmail_Select_2(string email)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_Users_Selectbyus_email");
            Db_0.AddInParameter(dbCommandWrapper, "@u_email", DbType.String, email);
            UserInfo obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                if (reader.Read())
                {
                    obj = GetUser(reader);
                }
            }

            return obj;
        }
        public UserInfo User_FromPhoneNum_Select_2(string u_mobile)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_Users_Selectbyu_mobile");
            Db_0.AddInParameter(dbCommandWrapper, "@u_mobile", DbType.String, u_mobile);

            UserInfo obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                if (reader.Read())
                {
                    obj = this.GetUser(reader);
                }
            }
            return obj;
        }
        public int UserId_Select(int u_id, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@u_id", DbType.Int32, u_id);
            int u_uid = 0;
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                if (reader.Read())
                {
                    u_uid = (int)reader["u_uid"];
                }
            }
            return u_uid;
        }
        public int User_Insert_2(UserInfo obj)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_Users_Insert");
            Db_0.AddOutParameter(cmd, "@u_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@u_uid", DbType.Int32, obj.u_uid);
            Db_0.AddInParameter(cmd, "@u_type", DbType.Byte, obj.u_type);
            Db_0.AddInParameter(cmd, "@u_name", DbType.String, obj.u_name);
            Db_0.AddInParameter(cmd, "@u_email", DbType.String, obj.u_email);
            Db_0.AddInParameter(cmd, "@u_mobile", DbType.String, obj.u_mobile);
            Db_0.AddInParameter(cmd, "@u_pwd", DbType.String, obj.u_pwd);
            Db_0.AddInParameter(cmd, "@u_status", DbType.Int32, obj.u_status);
            Db_0.AddInParameter(cmd, "@u_status_email", DbType.Byte, obj.u_status_email);
            Db_0.AddInParameter(cmd, "@u_status_mobile", DbType.Byte, obj.u_status_mobile);
            Db_0.AddInParameter(cmd, "@u_status_verify", DbType.Byte, obj.u_status_verify);
            Db_0.AddInParameter(cmd, "@u_regsex", DbType.Byte, obj.u_regsex);
            Db_0.AddInParameter(cmd, "@u_face", DbType.String, obj.u_face);
            Db_0.AddInParameter(cmd, "@u_face2", DbType.String, obj.u_face2);
            Db_0.AddInParameter(cmd, "@u_face3", DbType.String, obj.u_face3);
            Db_0.AddInParameter(cmd, "@u_signature", DbType.String, obj.u_signature);
            Db_0.AddInParameter(cmd, "@u_signatureImg", DbType.String, obj.u_signatureImg);
            Db_0.AddInParameter(cmd, "@u_regTime", DbType.DateTime, obj.u_regTime);
            Db_0.AddInParameter(cmd, "@u_prevLoginTime", DbType.String, obj.u_prevLoginTime);
            Db_0.AddInParameter(cmd, "@u_curLoginTime", DbType.String, obj.u_curLoginTime);
            Db_0.AddInParameter(cmd, "@u_login_num", DbType.Int32, obj.u_login_num);
            Db_0.AddInParameter(cmd, "@u_login_duration", DbType.Int32, obj.u_login_duration);
            Db_0.AddInParameter(cmd, "@u_total_money", DbType.Int32, obj.u_total_money);
            Db_0.AddInParameter(cmd, "@u_total_exp", DbType.Int32, obj.u_total_exp);
            Db_0.AddInParameter(cmd, "@u_grade", DbType.Byte, obj.u_grade);
            Db_0.AddInParameter(cmd, "@u_birthday", DbType.String, obj.u_birthday);
            Db_0.AddInParameter(cmd, "@u_astro", DbType.String, obj.u_astro);
            Db_0.AddInParameter(cmd, "@u_profession", DbType.String, obj.u_profession);
            Db_0.AddInParameter(cmd, "@u_height", DbType.Int32, obj.u_height);
            Db_0.AddInParameter(cmd, "@u_weight", DbType.Int32, obj.u_weight);
            Db_0.AddInParameter(cmd, "@u_live_country", DbType.String, obj.u_live_country);
            Db_0.AddInParameter(cmd, "@u_live_city", DbType.String, obj.u_live_city);
            Db_0.AddInParameter(cmd, "@u_home_country", DbType.String, obj.u_home_country);
            Db_0.AddInParameter(cmd, "@u_home_city", DbType.String, obj.u_home_city);
            Db_0.AddInParameter(cmd, "@u_interest", DbType.String, obj.u_interest);
            Db_0.AddInParameter(cmd, "@u_weibo", DbType.String, obj.u_weibo);
            Db_0.AddInParameter(cmd, "@u_total_tiezi", DbType.Int32, obj.u_total_tiezi);
            Db_0.AddInParameter(cmd, "@u_total_huifu", DbType.Int32, obj.u_total_huifu);
            Db_0.AddInParameter(cmd, "@u_total_shang", DbType.Int32, obj.u_total_shang);
            Db_0.AddInParameter(cmd, "@u_total_shangQZ", DbType.Int32, obj.u_total_shangQZ);
            Db_0.AddInParameter(cmd, "@u_total_shangQF", DbType.Int32, obj.u_total_shangQF);
            Db_0.AddInParameter(cmd, "@u_total_shangJY", DbType.Int32, obj.u_total_shangJY);
            Db_0.AddInParameter(cmd, "@u_total_pinglun", DbType.Int32, obj.u_total_pinglun);
            Db_0.AddInParameter(cmd, "@u_tableId", DbType.Int32, obj.u_tableId);
            Db_0.AddInParameter(cmd, "@u_today_shangF", DbType.Int32, obj.u_today_shangF);
            Db_0.AddInParameter(cmd, "@u_today_shangJY", DbType.Int32, obj.u_today_shangJY);
            int _returnValue = Db_0.ExecuteNonQuery(cmd);
            int u_id = (int)cmd.Parameters["@u_id"].Value;
            return u_id;
        }
        private UserInfo GetUser(IDataReader reader)
        {
            UserInfo obj = new UserInfo();
            obj.u_id = (int)reader["u_id"];
            obj.u_uid = (int)reader["u_uid"];
            obj.u_type = (byte)reader["u_type"];
            obj.u_name = reader["u_name"].ToString();
            obj.u_email = reader["u_email"].ToString();
            obj.u_mobile = reader["u_mobile"].ToString();
            obj.u_pwd = reader["u_pwd"].ToString();
            obj.u_status = (int)reader["u_status"];
            obj.u_status_email = (byte)reader["u_status_email"];
            obj.u_status_mobile = (byte)reader["u_status_mobile"];
            obj.u_status_verify = (byte)reader["u_status_verify"];
            obj.u_regsex = (byte)reader["u_regsex"];
            obj.u_face = reader["u_face"].ToString();
            obj.u_face2 = reader["u_face2"].ToString();
            obj.u_face3 = reader["u_face3"].ToString();
            obj.u_signature = reader["u_signature"].ToString();
            obj.u_signatureImg = reader["u_signatureImg"].ToString();
            obj.u_regTime = (DateTime)reader["u_regTime"];
            obj.u_prevLoginTime = reader["u_prevLoginTime"].ToString();
            obj.u_curLoginTime = reader["u_curLoginTime"].ToString();
            obj.u_login_num = (int)reader["u_login_num"];
            obj.u_login_duration = (int)reader["u_login_duration"];
            obj.u_total_money = (int)reader["u_total_money"];
            obj.u_total_exp = (int)reader["u_total_exp"];
            obj.u_grade = (byte)reader["u_grade"];
            obj.u_birthday = reader["u_birthday"].ToString();
            obj.u_astro = reader["u_astro"].ToString();
            obj.u_profession = reader["u_profession"].ToString();
            obj.u_height = (int)reader["u_height"];
            obj.u_weight = (int)reader["u_weight"];
            obj.u_live_country = reader["u_live_country"].ToString();
            obj.u_live_city = reader["u_live_city"].ToString();
            obj.u_home_country = reader["u_home_country"].ToString();
            obj.u_home_city = reader["u_home_city"].ToString();
            obj.u_interest = reader["u_interest"].ToString();
            obj.u_weibo = reader["u_weibo"].ToString();
            obj.u_total_tiezi = (int)reader["u_total_tiezi"];
            obj.u_total_huifu = (int)reader["u_total_huifu"];
            obj.u_total_shang = (int)reader["u_total_shang"];
            obj.u_total_shangQZ = (int)reader["u_total_shangQZ"];
            obj.u_total_shangQF = (int)reader["u_total_shangQF"];
            obj.u_total_shangJY = (int)reader["u_total_shangJY"];
            obj.u_total_pinglun = (int)reader["u_total_pinglun"];
            obj.u_tableId = (int)reader["u_tableId"];
            obj.u_today_shangF = (int)reader["u_today_shangF"];
            obj.u_today_shangJY = (int)reader["u_today_shangJY"];

            return obj;
        }
        #endregion

        public User_Dtl User_FromPhoneNum_Select_1(string u_phone)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_Users_Selectbyu_name");
            Db_0.AddInParameter(cmd, "@u_name", DbType.Int32, u_phone);

            User_Dtl obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    obj = new User_Dtl();
                    obj.u_id = (int)reader["u_uid"];
                    obj.u_type = (byte)reader["u_type"];
                    obj.u_name = reader["u_name"].ToString();
                    obj.u_face = reader["u_face"].ToString();
                    obj.u_email = reader["u_email"].ToString();
                    obj.u_status = (int)reader["u_status"];
                    obj.last_login_time = reader["u_prevLoginTime"].ToString();
                    obj.cur_login_time = reader["u_curLoginTime"].ToString();
                    obj.u_exp = (int)reader["u_total_exp"];
                    obj.u_pwd = reader["u_pwd"].ToString();
                }
                return obj;
            }
        }

        public int LoginLog_Insert(Users_LoginLogs_Info info, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(cmd, "@ul_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@ul_u_uid", DbType.Int32, info.ul_u_uid);
            Db_0.AddInParameter(cmd, "@ul_u_name", DbType.String, info.ul_u_name);
            Db_0.AddInParameter(cmd, "@ul_type", DbType.Int32, info.ul_type);
            Db_0.AddInParameter(cmd, "@ul_status", DbType.Int32, info.ul_status);
            Db_0.AddInParameter(cmd, "@ul_error", DbType.String, info.ul_error);
            Db_0.AddInParameter(cmd, "@ul_createTime", DbType.DateTime, info.ul_createTime);
            Db_0.AddInParameter(cmd, "@ul_ip", DbType.String, info.ul_ip);
            Db_0.AddInParameter(cmd, "@ul_os", DbType.String, info.ul_os);
            Db_0.AddInParameter(cmd, "@ul_browser", DbType.String, info.ul_browser);
            Db_0.AddInParameter(cmd, "@ul_clientId", DbType.String, info.ul_clientId);
            Db_0.AddInParameter(cmd, "@ul_userAgent", DbType.String, info.ul_userAgent);
            return Db_0.ExecuteNonQuery(cmd);
        }

        public int User_Update(UserInfo obj)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_Users_Update");
            Db_0.AddInParameter(dbCommandWrapper, "@u_uid", DbType.Int32, obj.u_uid);
            Db_0.AddInParameter(dbCommandWrapper, "@u_type", DbType.Byte, obj.u_type);
            Db_0.AddInParameter(dbCommandWrapper, "@u_name", DbType.String, obj.u_name);
            Db_0.AddInParameter(dbCommandWrapper, "@u_email", DbType.String, obj.u_email);
            Db_0.AddInParameter(dbCommandWrapper, "@u_mobile", DbType.String, obj.u_mobile);
            Db_0.AddInParameter(dbCommandWrapper, "@u_pwd", DbType.String, obj.u_pwd);
            Db_0.AddInParameter(dbCommandWrapper, "@u_status", DbType.Int32, obj.u_status);
            Db_0.AddInParameter(dbCommandWrapper, "@u_status_email", DbType.Byte, obj.u_status_email);
            Db_0.AddInParameter(dbCommandWrapper, "@u_status_mobile", DbType.Byte, obj.u_status_mobile);
            Db_0.AddInParameter(dbCommandWrapper, "@u_status_verify", DbType.Byte, obj.u_status_verify);
            Db_0.AddInParameter(dbCommandWrapper, "@u_regsex", DbType.Byte, obj.u_regsex);
            Db_0.AddInParameter(dbCommandWrapper, "@u_face", DbType.String, obj.u_face);
            Db_0.AddInParameter(dbCommandWrapper, "@u_face2", DbType.String, obj.u_face2);
            Db_0.AddInParameter(dbCommandWrapper, "@u_face3", DbType.String, obj.u_face3);
            Db_0.AddInParameter(dbCommandWrapper, "@u_signature", DbType.String, obj.u_signature);
            Db_0.AddInParameter(dbCommandWrapper, "@u_signatureImg", DbType.String, obj.u_signatureImg);
            Db_0.AddInParameter(dbCommandWrapper, "@u_regTime", DbType.DateTime, obj.u_regTime);
            Db_0.AddInParameter(dbCommandWrapper, "@u_prevLoginTime", DbType.String, obj.u_prevLoginTime);
            Db_0.AddInParameter(dbCommandWrapper, "@u_curLoginTime", DbType.String, obj.u_curLoginTime);
            Db_0.AddInParameter(dbCommandWrapper, "@u_login_num", DbType.Int32, obj.u_login_num);
            Db_0.AddInParameter(dbCommandWrapper, "@u_login_duration", DbType.Int32, obj.u_login_duration);
            Db_0.AddInParameter(dbCommandWrapper, "@u_total_money", DbType.Int32, obj.u_total_money);
            Db_0.AddInParameter(dbCommandWrapper, "@u_total_exp", DbType.Int32, obj.u_total_exp);
            Db_0.AddInParameter(dbCommandWrapper, "@u_grade", DbType.Byte, obj.u_grade);
            Db_0.AddInParameter(dbCommandWrapper, "@u_birthday", DbType.String, obj.u_birthday);
            Db_0.AddInParameter(dbCommandWrapper, "@u_astro", DbType.String, obj.u_astro);
            Db_0.AddInParameter(dbCommandWrapper, "@u_profession", DbType.String, obj.u_profession);
            Db_0.AddInParameter(dbCommandWrapper, "@u_height", DbType.Int32, obj.u_height);
            Db_0.AddInParameter(dbCommandWrapper, "@u_weight", DbType.Int32, obj.u_weight);
            Db_0.AddInParameter(dbCommandWrapper, "@u_live_country", DbType.String, obj.u_live_country);
            Db_0.AddInParameter(dbCommandWrapper, "@u_live_city", DbType.String, obj.u_live_city);
            Db_0.AddInParameter(dbCommandWrapper, "@u_home_country", DbType.String, obj.u_home_country);
            Db_0.AddInParameter(dbCommandWrapper, "@u_home_city", DbType.String, obj.u_home_city);
            Db_0.AddInParameter(dbCommandWrapper, "@u_interest", DbType.String, obj.u_interest);
            Db_0.AddInParameter(dbCommandWrapper, "@u_weibo", DbType.String, obj.u_weibo);
            Db_0.AddInParameter(dbCommandWrapper, "@u_total_tiezi", DbType.Int32, obj.u_total_tiezi);
            Db_0.AddInParameter(dbCommandWrapper, "@u_total_huifu", DbType.Int32, obj.u_total_huifu);
            Db_0.AddInParameter(dbCommandWrapper, "@u_total_shang", DbType.Int32, obj.u_total_shang);
            Db_0.AddInParameter(dbCommandWrapper, "@u_total_shangQZ", DbType.Int32, obj.u_total_shangQZ);
            Db_0.AddInParameter(dbCommandWrapper, "@u_total_shangQF", DbType.Int32, obj.u_total_shangQF);
            Db_0.AddInParameter(dbCommandWrapper, "@u_total_shangJY", DbType.Int32, obj.u_total_shangJY);
            Db_0.AddInParameter(dbCommandWrapper, "@u_total_pinglun", DbType.Int32, obj.u_total_pinglun);
            Db_0.AddInParameter(dbCommandWrapper, "@u_tableId", DbType.Int32, obj.u_tableId);
            Db_0.AddInParameter(dbCommandWrapper, "@u_today_shangF", DbType.Int32, obj.u_today_shangF);
            Db_0.AddInParameter(dbCommandWrapper, "@u_today_shangJY", DbType.Int32, obj.u_today_shangJY);

            int _returnValue = Db_0.ExecuteNonQuery(dbCommandWrapper);

            return _returnValue;
        }
        public Users_SocialInfo Open_User_Select(byte us_type, string us_uid, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@us_type", DbType.Byte, us_type);
            Db_0.AddInParameter(cmd, "@us_uid", DbType.String, us_uid);

            Users_SocialInfo obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    obj = GetUsers_Socials(reader);
                }
            }
            return obj;
        }
        public int Users_Socials_Insert(Users_SocialInfo u, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(cmd, "@us_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@us_u_uid", DbType.Int32, u.us_u_uid);
            Db_0.AddInParameter(cmd, "@us_type", DbType.Byte, u.us_type);
            Db_0.AddInParameter(cmd, "@us_uid", DbType.String, u.us_uid);
            Db_0.AddInParameter(cmd, "@us_nick", DbType.String, u.us_nick);
            Db_0.AddInParameter(cmd, "@us_name", DbType.String, u.us_name);
            Db_0.AddInParameter(cmd, "@us_location", DbType.String, u.us_location);
            Db_0.AddInParameter(cmd, "@us_siteurl", DbType.String, u.us_siteurl);
            Db_0.AddInParameter(cmd, "@us_headImg", DbType.String, u.us_headImg);
            Db_0.AddInParameter(cmd, "@us_gender", DbType.String, u.us_gender);
            Db_0.AddInParameter(cmd, "@us_fansNum", DbType.Int32, u.us_fansNum);
            Db_0.AddInParameter(cmd, "@us_attentionsNum", DbType.Int32, u.us_attentionsNum);
            Db_0.AddInParameter(cmd, "@us_favorsNum", DbType.Int32, u.us_favorsNum);
            Db_0.AddInParameter(cmd, "@us_contentsNum", DbType.Int32, u.us_contentsNum);
            Db_0.AddInParameter(cmd, "@us_verified", DbType.Boolean, u.us_verified);
            Db_0.AddInParameter(cmd, "@us_code", DbType.String, u.us_code);
            Db_0.AddInParameter(cmd, "@us_openkey", DbType.String, u.us_openkey);
            Db_0.AddInParameter(cmd, "@us_sync2", DbType.Boolean, u.us_sync2);
            Db_0.AddInParameter(cmd, "@us_createTime", DbType.DateTime, u.us_createTime);
            Db_0.AddInParameter(cmd, "@us_lastLogin", DbType.String, u.us_lastLogin);
            Db_0.AddInParameter(cmd, "@us_loginNum", DbType.Int32, u.us_loginNum);

            return Db_0.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 获取社会接入
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Users_SocialInfo GetUsers_Socials(IDataReader reader)
        {
            Users_SocialInfo obj = new Users_SocialInfo();
            obj.us_id = (int)reader["us_id"];
            obj.us_u_uid = (int)reader["us_u_uid"];
            obj.us_type = (byte)reader["us_type"];
            obj.us_uid = reader["us_uid"].ToString();
            obj.us_nick = reader["us_nick"].ToString();
            obj.us_name = reader["us_name"].ToString();
            obj.us_location = reader["us_location"].ToString();
            obj.us_siteurl = reader["us_siteurl"].ToString();
            obj.us_headImg = reader["us_headImg"].ToString();
            obj.us_gender = reader["us_gender"].ToString();
            obj.us_fansNum = (int)reader["us_fansNum"];
            obj.us_attentionsNum = (int)reader["us_attentionsNum"];
            obj.us_favorsNum = (int)reader["us_favorsNum"];
            obj.us_contentsNum = (int)reader["us_contentsNum"];
            obj.us_verified = (bool)reader["us_verified"];
            obj.us_code = reader["us_code"].ToString();
            obj.us_openkey = reader["us_openkey"].ToString();
            obj.us_sync2 = (bool)reader["us_sync2"];
            obj.us_createTime = (DateTime)reader["us_createTime"];
            obj.us_lastLogin = reader["us_lastLogin"].ToString();
            obj.us_loginNum = (int)reader["us_loginNum"];
            return obj;
        }
        public bool User_FirstLogin_Exist(int u_id)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_Users_AppFirstLoginLogs_Selectbyul_u_uid");
            Db_0.AddInParameter(cmd, "@ul_u_uid", DbType.Int32, u_id);

            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                return reader.Read();
            }
        }

        public int User_FirstLogin_Insert(Users_AppFirstLoginLogsInfo obj)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_Users_AppFirstLoginLogs_Insert");
            Db_0.AddOutParameter(cmd, "@ul_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@ul_u_uid", DbType.Int32, obj.ul_u_uid);
            Db_0.AddInParameter(cmd, "@ul_u_name", DbType.String, obj.ul_u_name);
            Db_0.AddInParameter(cmd, "@ul_createTime", DbType.DateTime, obj.ul_createTime);

            int _returnValue = Db_0.ExecuteNonQuery(cmd);
            //int ul_id = (int)cmd.Parameters["@ul_id"].Value;

            return _returnValue;
        }

        public int ShortMsg_Insert(User_SMSLogInfo info)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_SMSLog_Insert");
            Db_0.AddOutParameter(cmd, "@p_sms_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@p_sms_type", DbType.Byte, info.Sms_type);
            Db_0.AddInParameter(cmd, "@p_sms_code", DbType.String, info.Sms_code);
            Db_0.AddInParameter(cmd, "@p_sms_success", DbType.Byte, info.Sms_success);
            Db_0.AddInParameter(cmd, "@p_sms_time", DbType.DateTime, info.Sms_time);
            Db_0.AddInParameter(cmd, "@p_sms_phone", DbType.String, info.Sms_phone);
            Db_0.AddInParameter(cmd, "@p_sms_purpose", DbType.Byte, info.Sms_purpose);
            return Db_0.ExecuteNonQuery(cmd);
        }
        public int User_PwdReset_Log_Insert(Users_PwdFoundLogInfo obj)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_Users_PwdFoundLogs_Insert");
            Db_0.AddOutParameter(cmd, "@pl_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@pl_uid", DbType.String, obj.pl_uid);
            Db_0.AddInParameter(cmd, "@pl_to", DbType.String, obj.pl_to);
            Db_0.AddInParameter(cmd, "@pl_url", DbType.String, obj.pl_url);
            Db_0.AddInParameter(cmd, "@pl_expireTime", DbType.DateTime, obj.pl_expireTime);
            Db_0.AddInParameter(cmd, "@pl_status", DbType.Int32, obj.pl_status);
            Db_0.AddInParameter(cmd, "@pl_remark", DbType.String, obj.pl_remark);
            Db_0.AddInParameter(cmd, "@pl_createTime", DbType.DateTime, obj.pl_createTime);
            Db_0.AddInParameter(cmd, "@pl_execTime", DbType.String, obj.pl_execTime);

            return Db_0.ExecuteNonQuery(cmd);
        }
        public int Users_Socials_Update(Users_SocialInfo ou, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@us_id", DbType.Int32, ou.us_id);
            Db_0.AddInParameter(cmd, "@us_u_uid", DbType.Int32, ou.us_u_uid);
            Db_0.AddInParameter(cmd, "@us_type", DbType.Byte, ou.us_type);
            Db_0.AddInParameter(cmd, "@us_uid", DbType.String, ou.us_uid);
            Db_0.AddInParameter(cmd, "@us_nick", DbType.String, ou.us_nick);
            Db_0.AddInParameter(cmd, "@us_name", DbType.String, ou.us_name);
            Db_0.AddInParameter(cmd, "@us_location", DbType.String, ou.us_location);
            Db_0.AddInParameter(cmd, "@us_siteurl", DbType.String, ou.us_siteurl);
            Db_0.AddInParameter(cmd, "@us_headImg", DbType.String, ou.us_headImg);
            Db_0.AddInParameter(cmd, "@us_gender", DbType.String, ou.us_gender);
            Db_0.AddInParameter(cmd, "@us_fansNum", DbType.Int32, ou.us_fansNum);
            Db_0.AddInParameter(cmd, "@us_attentionsNum", DbType.Int32, ou.us_attentionsNum);
            Db_0.AddInParameter(cmd, "@us_favorsNum", DbType.Int32, ou.us_favorsNum);
            Db_0.AddInParameter(cmd, "@us_contentsNum", DbType.Int32, ou.us_contentsNum);
            Db_0.AddInParameter(cmd, "@us_verified", DbType.Boolean, ou.us_verified);
            Db_0.AddInParameter(cmd, "@us_code", DbType.String, ou.us_code);
            Db_0.AddInParameter(cmd, "@us_openkey", DbType.String, ou.us_openkey);
            Db_0.AddInParameter(cmd, "@us_sync2", DbType.Boolean, ou.us_sync2);
            Db_0.AddInParameter(cmd, "@us_createTime", DbType.DateTime, ou.us_createTime);
            Db_0.AddInParameter(cmd, "@us_lastLogin", DbType.String, ou.us_lastLogin);
            Db_0.AddInParameter(cmd, "@us_loginNum", DbType.Int32, ou.us_loginNum);

            return Db_0.ExecuteNonQuery(cmd);
        }

        public int UserAppendInf_Set(int u_id, string field, string value)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_UserAppendInf_Update");
            Db_0.AddInParameter(cmd, "@uid", DbType.Int32, u_id);
            Db_0.AddInParameter(cmd, "@field", DbType.String, field);
            Db_0.AddInParameter(cmd, "@value", DbType.String, value);

            Db_0.ExecuteNonQuery(cmd);
            return 1;
        }

        public MailServersInfo MailServers_SelectRand()
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_MailServers_SelectRand");

            MailServersInfo obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                if (reader.Read())
                {
                    obj = new MailServersInfo();
                    obj.ms_id = (int)reader["ms_id"];
                    obj.ms_smtp = reader["ms_smtp"].ToString();
                    obj.ms_loginName = reader["ms_loginName"].ToString();
                    obj.ms_loginPwd = reader["ms_loginPwd"].ToString();
                    obj.ms_account = reader["ms_account"].ToString();
                    obj.ms_ssl = (bool)reader["ms_ssl"];
                    obj.ms_port = (int)reader["ms_port"];
                    obj.ms_status = (bool)reader["ms_status"];
                    obj.ms_remark = reader["ms_remark"].ToString();
                }
            }

            return obj;
        }

        public int Users_MailLogs_Insert(Users_MailLogInfo obj)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_Users_MailLogs_Insert");
            Db_0.AddOutParameter(cmd, "@ml_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@ml_uid", DbType.String, obj.ml_uid);
            Db_0.AddInParameter(cmd, "@ml_type", DbType.String, obj.ml_type);
            Db_0.AddInParameter(cmd, "@ml_to", DbType.String, obj.ml_to);
            Db_0.AddInParameter(cmd, "@ml_toName", DbType.String, obj.ml_toName);
            Db_0.AddInParameter(cmd, "@ml_cc", DbType.String, obj.ml_cc);
            Db_0.AddInParameter(cmd, "@ml_title", DbType.String, obj.ml_title);
            Db_0.AddInParameter(cmd, "@ml_content", DbType.String, obj.ml_content);
            Db_0.AddInParameter(cmd, "@ml_state", DbType.Int32, obj.ml_state);
            Db_0.AddInParameter(cmd, "@ml_createTime", DbType.DateTime, obj.ml_createTime);
            Db_0.AddInParameter(cmd, "@ml_createUser", DbType.String, obj.ml_createUser);
            Db_0.AddInParameter(cmd, "@ml_resend", DbType.Int32, obj.ml_resend);
            Db_0.AddInParameter(cmd, "@ml_resendRemark", DbType.String, obj.ml_resendRemark);
            Db_0.AddInParameter(cmd, "@ml_from", DbType.String, obj.ml_from);
            Db_0.AddInParameter(cmd, "@ml_fromName", DbType.String, obj.ml_fromName);

            int _returnValue = Db_0.ExecuteNonQuery(cmd);
            int ml_id = (int)cmd.Parameters["@ml_id"].Value;

            return _returnValue;
        }

        public User_Append_Info UserAppendInf_Select(int u_id)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_UserAppendInf_Selectbyuid");
            Db_0.AddInParameter(cmd, "@uid", DbType.Int32, u_id);

            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if(reader.Read())
                {
                    var i = new User_Append_Info();
                    i.u_business = reader["uai_business"].ToString();
                    i.u_company = reader["uai_company"].ToString();
                    i.u_position = reader["uai_position"].ToString();
                    i.u_signature = reader["uai_sign"].ToString();
                    i.pos_favor = reader["uai_p_favorite"].ToString();
                    i.bus_favor = reader["uai_b_favorite"].ToString();
                    return i;
                }
            }
            return null;
        }

        public int UserNameUpdateLog_Count_Get(int u_id)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_User_UpdateNamelog_CountById");
            Db_0.AddInParameter(dbCommandWrapper, "@userId", DbType.Int32, u_id);

            object _returnValue = Db_0.ExecuteScalar(dbCommandWrapper);

            if (_returnValue != null)
                return (int)_returnValue;
            return 0;
        }

        public bool UserNameUpdate_Flag_Get(int u_id, int limit)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_User_CheckUpdateNamelogByUserId");
            Db_0.AddInParameter(dbCommandWrapper, "@userId", DbType.Int32, u_id);
            Db_0.AddInParameter(dbCommandWrapper, "@number", DbType.Int32, limit);
            int result = 0;
            object rtn = Db_0.ExecuteScalar(dbCommandWrapper);
            if (rtn != null)
            {
                int.TryParse(rtn.ToString(), out result);
            }
            return result > 0;
        }

        public int User_UpdateNameLog(User_UpdateNameLog obj)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand("Proc_User_UpdateNameLog_Insert");
            Db_0.AddInParameter(cmd, "@userid", DbType.Int32, obj.userid);
            Db_0.AddInParameter(cmd, "@namefront", DbType.String, obj.namefront);
            Db_0.AddInParameter(cmd, "@nameback", DbType.String, obj.nameback);
            Db_0.AddInParameter(cmd, "@clientIp", DbType.String, obj.clientIp);
            Db_0.AddInParameter(cmd, "@version", DbType.String, obj.version);
            Db_0.AddInParameter(cmd, "@platform", DbType.String, obj.platform);
            return Db_0.ExecuteNonQuery(cmd);
        }

        public int Community_Topic_Insert(AppTieziTopicInfo obj, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(dbCommandWrapper, "@att_id", DbType.Int32, 4);
            Db_0.AddInParameter(dbCommandWrapper, "@att_u_name", DbType.String, obj.att_u_name);
            Db_0.AddInParameter(dbCommandWrapper, "@att_u_uid", DbType.Int32, obj.att_u_uid);
            Db_0.AddInParameter(dbCommandWrapper, "@att_content", DbType.String, obj.att_content);
            Db_0.AddInParameter(dbCommandWrapper, "@att_date", DbType.DateTime, obj.att_date);
            Db_0.AddInParameter(dbCommandWrapper, "@att_u_face", DbType.String, obj.att_u_face);
            Db_0.AddInParameter(dbCommandWrapper, "@att_tag", DbType.String, obj.att_tag);

            Db_0.ExecuteNonQuery(dbCommandWrapper);
            return (int)dbCommandWrapper.Parameters["@att_id"].Value;
        }

        public int CommunityTieziImage_Bulk_Insert(AppTieziImageInfo info, List<string> uris, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(cmd, "@ati_id", DbType.Int32, 4);
            Db_0.AddInParameter(cmd, "@ati_tiezi_id", DbType.Int32, info.ati_tiezi_id);
            Db_0.AddInParameter(cmd, "@ati_tiezi_type", DbType.Int32, info.ati_tiezi_type);
            Db_0.AddInParameter(cmd, "@ati_uid", DbType.Int32, info.ati_uid);
            Db_0.AddInParameter(cmd, "@ati_url", DbType.String, info.ati_url);

            int count = 0;
            foreach (var uri in uris)
            {
                Db_0.SetParameterValue(cmd, "@ati_url", uri);
                try
                {
                    Db_0.ExecuteNonQuery(cmd);
                    count++;
                }
                catch (Exception e)
                {
                }
            }
            return count;
        }

        public int Community_Reply_Insert(AppTeiziReplyInfo obj, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddOutParameter(dbCommandWrapper, "@atr_id", DbType.Int32, 4);
            Db_0.AddInParameter(dbCommandWrapper, "@atr_teizi", DbType.Int32, obj.atr_teizi);
            Db_0.AddInParameter(dbCommandWrapper, "@atr_u_name", DbType.String, obj.atr_u_name);
            Db_0.AddInParameter(dbCommandWrapper, "@atr_u_uid", DbType.Int32, obj.atr_u_uid);
            Db_0.AddInParameter(dbCommandWrapper, "@atr_content", DbType.String, obj.atr_content);
            Db_0.AddInParameter(dbCommandWrapper, "@atr_date", DbType.DateTime, obj.atr_date);

            Db_0.ExecuteNonQuery(dbCommandWrapper);
            return (int)dbCommandWrapper.Parameters["@atr_id"].Value;
        }


        public List<Cm_Topic_Dtl> Community_Topics_Dtl_Get(string column, string where, string order, int pg_index, int pg_size, string sp_Name, out int count)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@columns", DbType.String, column);
            Db_0.AddInParameter(dbCommandWrapper, "@where", DbType.String, where);
            Db_0.AddInParameter(dbCommandWrapper, "@order", DbType.String, order);
            Db_0.AddInParameter(dbCommandWrapper, "@page", DbType.Int32, pg_index);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, pg_size);
            Db_0.AddOutParameter(dbCommandWrapper, "@rowCount", DbType.Int32, 4);

            List<Cm_Topic_Dtl> lst = new List<Cm_Topic_Dtl>();
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while (reader.Read())
                {
                    Cm_Topic_Dtl obj = new Cm_Topic_Dtl();
                    obj.topic_id = (int)reader["att_id"];
                    obj.u_name = reader["att_u_name"].ToString();
                    obj.u_id = reader["att_u_uid"].ToString();
                    obj.topic_content = reader["att_content"].ToString();
                    obj.topic_date = (DateTime)reader["att_date"];
                    //obj.u_face = reader["att_u_face"].ToString();
                    obj.topic_tag = reader["att_tag"].ToString();

                    lst.Add(obj);
                }
                reader.NextResult();
                count = (int)dbCommandWrapper.Parameters["@rowCount"].Value;
            }


            return lst;
        }
        public Cm_Topic_Dtl Community_Topic_Dtl_Get(int t_id, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@att_id", DbType.Int32, t_id);

            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                if (reader.Read())
                {
                    Cm_Topic_Dtl obj = new Cm_Topic_Dtl();
                    obj.topic_id = (int)reader["att_id"];
                    obj.u_name = reader["att_u_name"].ToString();
                    obj.u_id = reader["att_u_uid"].ToString();
                    obj.topic_content = reader["att_content"].ToString();
                    obj.topic_date = (DateTime)reader["att_date"];
                    obj.topic_tag = reader["att_tag"].ToString();
                    obj.u_face = reader["att_u_face"].ToString();
                    obj.status = (byte)reader["att_status"] == 1;
                    return obj;
                }
            }


            return null;
        }
        public int Community_Topic_ReplyCount_Get(int t_id, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@tid", DbType.Int32, t_id);
            return (int)Db_0.ExecuteScalar(cmd);
        }
        public List<int> CommunityReply_GroupByTid_Get(int count, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@count", DbType.Int32, count);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                var list = new List<int>();
                while(reader.Read())
                {
                    list.Add((int)reader["atr_teizi"]);
                }
                return list;
            }
        }

        public Tuple<int, int> Community_Topic_UpDown_Count_Get(int t_id, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@all_type", DbType.Int32, 1);
            Db_0.AddInParameter(cmd, "@all_teizi", DbType.Int32, t_id);

            int up_count = 0;
            int down_count = 0;
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                    up_count = (int)reader[0];
            }

            Db_0.SetParameterValue(cmd, "@all_type", 2);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                    down_count = (int)reader[0];
            }
            return new Tuple<int, int>(up_count, down_count);
        }

        public Tuple<int, int> Community_Topic_UpDown_Flag_Get(int t_id, int u_id, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@all_teizi", DbType.Int32, t_id);
            Db_0.AddInParameter(cmd, "@all_u_uid", DbType.Int32, u_id);
            Db_0.AddInParameter(cmd, "@all_type", DbType.Int32, 1);

            int up = 0;
            int down = 0;
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                    up = (int)reader["all_valid"];
            }

            Db_0.SetParameterValue(cmd, "@all_type", 2);
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                if (reader.Read())
                    down = (int)reader["all_valid"];
            }
            return new Tuple<int, int>(up, down);
        }

        public List<int> CommunityReply_GroupBy_TidUid_Get(int u_id, int pg_index, int pg_size, string sp_Name)
        {
            DbCommand cmd = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(cmd, "@pg_index", DbType.Int32, pg_index);
            Db_0.AddInParameter(cmd, "@uid", DbType.Int32, u_id);
            Db_0.AddInParameter(cmd, "@pg_size", DbType.Int32, pg_size);

            var list = new List<int>();
            using (IDataReader reader = Db_0.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    list.Add((int)reader["atr_teizi"]);
                }
            }
            return list;
        }

        public int Community_Topic_UpDown_Vote(AppLikeOrNotLogInfo obj, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_AppLikeOrNotLog_Insert");
            Db_0.AddOutParameter(dbCommandWrapper, "@all_id", DbType.Int32, 4);
            Db_0.AddInParameter(dbCommandWrapper, "@all_teizi", DbType.Int32, obj.all_teizi);
            Db_0.AddInParameter(dbCommandWrapper, "@all_u_name", DbType.String, obj.all_u_name);
            Db_0.AddInParameter(dbCommandWrapper, "@all_u_uid", DbType.Int32, obj.all_u_uid);
            Db_0.AddInParameter(dbCommandWrapper, "@all_date", DbType.DateTime, obj.all_date);
            Db_0.AddInParameter(dbCommandWrapper, "@all_type", DbType.Int32, obj.all_type);
            Db_0.AddInParameter(dbCommandWrapper, "@all_valid", DbType.Int32, obj.all_valid);

            return Db_0.ExecuteNonQuery(dbCommandWrapper);
        }


        public int Community_Topic_Up2Down(AppLikeOrNotLogInfo info)
        {
            // cancel up firstly
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_AppLikeOrNotLog_Cancel");
            Db_0.AddInParameter(dbCommandWrapper, "@all_teizi", DbType.Int32, info.all_teizi);
            Db_0.AddInParameter(dbCommandWrapper, "@all_u_uid", DbType.Int32, info.all_u_uid);
            Db_0.AddInParameter(dbCommandWrapper, "@all_type", DbType.Int32, 1);

            if(Db_0.ExecuteNonQuery(dbCommandWrapper) > 0)  // succeed to cancel up, then vote down
            {
                info.all_type = 2;
                info.all_valid = 1;
                return Community_Topic_UpDown_Vote(info, "Proc_AppLikeOrNotLog_Insert");
            }

            return -1;  // operate failed
        }

        public int Community_Topic_Down2Up(AppLikeOrNotLogInfo info)
        {
            // cancel down firstly
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_AppLikeOrNotLog_Cancel");
            Db_0.AddInParameter(dbCommandWrapper, "@all_teizi", DbType.Int32, info.all_teizi);
            Db_0.AddInParameter(dbCommandWrapper, "@all_u_uid", DbType.Int32, info.all_u_uid);
            Db_0.AddInParameter(dbCommandWrapper, "@all_type", DbType.Int32, 2);

            if (Db_0.ExecuteNonQuery(dbCommandWrapper) > 0)  // succeed to cancel up, then vote down
            {
                info.all_type = 1;
                info.all_valid = 1;
                return Community_Topic_UpDown_Vote(info, "Proc_AppLikeOrNotLog_Insert");
            }

            return -1;  // operate failed
        }

        public CMSPagesInfo CMSPagesInfo_FromUid_Get(string pg_uid, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@pg_uid", DbType.String, pg_uid);

            CMSPagesInfo obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                if (reader.Read())
                {
                    obj = new CMSPagesInfo();
                    obj.pg_id = (int)reader["pg_id"];
                    obj.pg_uid = reader["pg_uid"].ToString();
                    obj.pg_name = reader["pg_name"].ToString();
                }
            }
            return obj;
        }

        public List<CMSBlocksInfo> CMSBlocks_Selectbyblk_pg_id(int blk_pg_id)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_CMSBlocks_Selectbyblk_pg_id");
            Db_0.AddInParameter(dbCommandWrapper, "@blk_pg_id", DbType.Int32, blk_pg_id);

            List<CMSBlocksInfo> lst = new List<CMSBlocksInfo>();
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while (reader.Read())
                {
                    var obj = new CMSBlocksInfo();
                    obj.blk_id = (int)reader["blk_id"];
                    obj.blk_code = reader["blk_code"].ToString();
                    obj.blk_name = reader["blk_name"].ToString();
                    lst.Add(obj);
                }
                reader.NextResult();
            }

            return lst;
        }

        public List<CMSItemsInfo> CMSItems_FromPgid_Select(DatabaseSearchModel search, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@columns", DbType.String, search.Column);
            Db_0.AddInParameter(dbCommandWrapper, "@where", DbType.String, search.Where);
            Db_0.AddInParameter(dbCommandWrapper, "@order", DbType.String, search.Order);
            Db_0.AddInParameter(dbCommandWrapper, "@page", DbType.Int32, search.PageIndex);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, search.PageSize);
            Db_0.AddOutParameter(dbCommandWrapper, "@rowCount", DbType.Int32, 4);

            var lst = new List<CMSItemsInfo>();
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while (reader.Read())
                {
                    var info = new CMSItemsInfo();
                    info.n_title = reader["n_title"].ToString();
                    info.n_linkUrl = reader["n_linkUrl"].ToString();
                    lst.Add(info);
                }
            }

            return lst;
        }

        #region [CommentTipOff_Insert]
        public int CommentTipOff_Insert(CommentTipOffInfo obj)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_CommentTipOff_Insert");
            Db_0.AddOutParameter(dbCommandWrapper, "@cto_id", DbType.Int32, 4);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_tiezi_id", DbType.Int32, obj.cto_tiezi_id);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_tiezi_type", DbType.Byte, obj.cto_tiezi_type);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_YuanGao_Uid", DbType.Int32, obj.cto_YuanGao_Uid);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_YuanGao_Uname", DbType.String, obj.cto_YuanGao_Uname);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_BeiGao_Uid", DbType.Int32, obj.cto_BeiGao_Uid);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_BeiGao_Uname", DbType.String, obj.cto_BeiGao_Uname);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_status", DbType.Byte, obj.cto_status);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_des", DbType.String, obj.cto_des);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_time", DbType.DateTime, obj.cto_time);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_shield", DbType.Byte, obj.cto_shield);

            try
            {

                int _returnValue = Db_0.ExecuteNonQuery(dbCommandWrapper);
                int cto_id = (int)dbCommandWrapper.Parameters["@cto_id"].Value;

                return _returnValue;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

        #region [CommentTipOff_Update]
        public int CommentTipOff_Update(CommentTipOffInfo obj)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_CommentTipOff_Update");
            Db_0.AddInParameter(dbCommandWrapper, "@cto_id", DbType.Int32, obj.cto_id);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_tiezi_id", DbType.Int32, obj.cto_tiezi_id);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_tiezi_type", DbType.Byte, obj.cto_tiezi_type);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_YuanGao_Uid", DbType.Int32, obj.cto_YuanGao_Uid);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_YuanGao_Uname", DbType.String, obj.cto_YuanGao_Uname);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_BeiGao_Uid", DbType.Int32, obj.cto_BeiGao_Uid);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_BeiGao_Uname", DbType.String, obj.cto_BeiGao_Uname);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_status", DbType.Byte, obj.cto_status);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_des", DbType.String, obj.cto_des);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_time", DbType.DateTime, obj.cto_time);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_shield", DbType.Byte, obj.cto_shield);

            try
            {

                int _returnValue = Db_0.ExecuteNonQuery(dbCommandWrapper);

                return _returnValue;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

        #region [CommentTipOff_SelectPaged]
        public List<CommentTipOffInfo> CommentTipOff_SelectPaged(string columns, string where, string order, int page, int pagesize, out int rowcount)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_CommentTipOff_SelectPaged");
            Db_0.AddInParameter(dbCommandWrapper, "@columns", DbType.String, columns);
            Db_0.AddInParameter(dbCommandWrapper, "@where", DbType.String, where);
            Db_0.AddInParameter(dbCommandWrapper, "@order", DbType.String, order);
            Db_0.AddInParameter(dbCommandWrapper, "@page", DbType.Int32, page);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, pagesize);
            Db_0.AddOutParameter(dbCommandWrapper, "@rowCount", DbType.Int32, 4);

            try
            {

                List<CommentTipOffInfo> lst = new List<CommentTipOffInfo>();
                using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
                {
                    while (reader.Read())
                    {
                        CommentTipOffInfo obj = new CommentTipOffInfo();
                        obj.cto_id = (int)reader["cto_id"];
                        obj.cto_tiezi_id = (int)reader["cto_tiezi_id"];
                        obj.cto_tiezi_type = (byte)reader["cto_tiezi_type"];
                        obj.cto_YuanGao_Uid = (int)reader["cto_YuanGao_Uid"];
                        obj.cto_YuanGao_Uname = reader["cto_YuanGao_Uname"].ToString();
                        obj.cto_BeiGao_Uid = (int)reader["cto_BeiGao_Uid"];
                        obj.cto_BeiGao_Uname = reader["cto_BeiGao_Uname"].ToString();
                        obj.cto_status = (byte)reader["cto_status"];
                        obj.cto_des = reader["cto_des"].ToString();
                        obj.cto_time = (DateTime)reader["cto_time"];
                        obj.cto_shield = (byte)reader["cto_shield"];
                        lst.Add(obj);
                    }
                    reader.NextResult();
                    rowcount = (int)dbCommandWrapper.Parameters["@rowCount"].Value;
                }


                return lst;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

        #region [CommentTipOff_SelectbyYuanGao_Type]
        public CommentTipOffInfo CommentTipOff_SelectbyYuanGao_Type(int cto_yuangao_uid, byte cto_tiezi_type, int tiezi_id)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_CommentTipOff_SelectbyYuanGao_Type");
            Db_0.AddInParameter(dbCommandWrapper, "@cto_YuanGao_Uid", DbType.Int32, cto_yuangao_uid);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_tiezi_type", DbType.Byte, cto_tiezi_type);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_tiezi_id", DbType.Int32, tiezi_id);

            CommentTipOffInfo obj = null;
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                if (reader.Read())
                {
                    obj = new CommentTipOffInfo();
                    obj.cto_id = (int)reader["cto_id"];
                    obj.cto_tiezi_id = (int)reader["cto_tiezi_id"];
                    obj.cto_tiezi_type = (byte)reader["cto_tiezi_type"];
                    obj.cto_YuanGao_Uid = (int)reader["cto_YuanGao_Uid"];
                    obj.cto_YuanGao_Uname = reader["cto_YuanGao_Uname"].ToString();
                    obj.cto_BeiGao_Uid = (int)reader["cto_BeiGao_Uid"];
                    obj.cto_BeiGao_Uname = reader["cto_BeiGao_Uname"].ToString();
                    obj.cto_status = (byte)reader["cto_status"];
                    obj.cto_des = reader["cto_des"].ToString();
                    obj.cto_time = (DateTime)reader["cto_time"];
                    obj.cto_shield = (byte)reader["cto_shield"];
                }
            }


            return obj;
        }
        #endregion

        #region [CommentTipOff_SelectbyBeiGao_Type]
        public CommentTipOffInfo CommentTipOff_SelectbyBeiGao_Type(int cto_beigao_uid, byte cto_tiezi_type)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_CommentTipOff_SelectbyBeiGao_Type");
            Db_0.AddInParameter(dbCommandWrapper, "@cto_BeiGao_Uid", DbType.Int32, cto_beigao_uid);
            Db_0.AddInParameter(dbCommandWrapper, "@cto_tiezi_type", DbType.Byte, cto_tiezi_type);

            try
            {

                CommentTipOffInfo obj = null;
                using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
                {
                    if (reader.Read())
                    {
                        obj = new CommentTipOffInfo();
                        obj.cto_id = (int)reader["cto_id"];
                        obj.cto_tiezi_id = (int)reader["cto_tiezi_id"];
                        obj.cto_tiezi_type = (byte)reader["cto_tiezi_type"];
                        obj.cto_YuanGao_Uid = (int)reader["cto_YuanGao_Uid"];
                        obj.cto_YuanGao_Uname = reader["cto_YuanGao_Uname"].ToString();
                        obj.cto_BeiGao_Uid = (int)reader["cto_BeiGao_Uid"];
                        obj.cto_BeiGao_Uname = reader["cto_BeiGao_Uname"].ToString();
                        obj.cto_status = (byte)reader["cto_status"];
                        obj.cto_des = reader["cto_des"].ToString();
                        obj.cto_time = (DateTime)reader["cto_time"];
                        obj.cto_shield = (byte)reader["cto_shield"];
                    }
                }


                return obj;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

        #region [AreaList_New_2016_SelectAllName]
        public List<string> AreaList_New_2016_SelectAllName()
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_AreaList_New_SelectAllName");

            try
            {

                List<string> lst = new List<string>();
                using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
                {
                    while (reader.Read())
                    {

                        lst.Add(reader["city"].ToString());
                    }
                    reader.NextResult();
                }


                return lst;
            }
            catch (Exception e)
            {
                return new List<string>();
            }
        }
        #endregion

        #region [AreaList_SelectWhere]
        public List<AreaList> AreaList_SelectWhere(string where)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_AreaList_SelectWhere");
            Db_0.AddInParameter(dbCommandWrapper, "@where", DbType.String, where);

            try
            {

                List<AreaList> lst = new List<AreaList>();
                using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
                {
                    while (reader.Read())
                    {
                        AreaList obj = new AreaList();
                        obj.a_id = (int)reader["a_id"];
                        obj.a_code = reader["a_code"].ToString();
                        obj.a_name = reader["a_name"].ToString();
                        obj.a_shortName = reader["a_shortName"].ToString();
                        obj.a_mapX = reader["a_mapX"].ToString();
                        obj.a_mapY = reader["a_mapY"].ToString();
                        lst.Add(obj);
                    }
                    reader.NextResult();
                }


                return lst;
            }
            catch (Exception e)
            {
                return new List<AreaList>();
            }
        }
        #endregion

        #region [ForwardTrade_Insert]
        public int ForwardTrade_Insert(ForwardTrade obj)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand("Proc_ForwardTrade_Insert");
            Db_0.AddOutParameter(dbCommandWrapper, "@ft_id", DbType.Int32, 4);
            Db_0.AddInParameter(dbCommandWrapper, "@ft_name", DbType.String, obj.ft_name);
            Db_0.AddInParameter(dbCommandWrapper, "@ft_date", DbType.DateTime, obj.ft_date);
            Db_0.AddInParameter(dbCommandWrapper, "@ft_hashcode", DbType.Int32, obj.ft_hashcode);

            try
            {

                int _returnValue = Db_0.ExecuteNonQuery(dbCommandWrapper);
                int ft_id = (int)dbCommandWrapper.Parameters["@ft_id"].Value;

                return _returnValue;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            /*using(DataAccess access = new DataAccess())
            {
                return access.ForwardTrade_Insert( obj );
            }*/
        }
        #endregion


        #region [ExhibitionEnterprise_SelectPaged]
        public List<ExhibitAbs> Company_ExhibitAbs_PageSelect(DatabaseSearchModel model, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@columns", DbType.String, model.Column);
            Db_0.AddInParameter(dbCommandWrapper, "@where", DbType.String, model.Where);
            Db_0.AddInParameter(dbCommandWrapper, "@order", DbType.String, model.Order);
            Db_0.AddInParameter(dbCommandWrapper, "@page", DbType.Int32, model.PageIndex);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, model.PageSize);
            Db_0.AddOutParameter(dbCommandWrapper, "@rowCount", DbType.Int32, 4);

            var lst = new List<ExhibitAbs>();
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while (reader.Read())
                {
                    var obj = new ExhibitAbs();
                    //obj.ee_id = (int)reader["ee_id"];
                    //obj.ee_md = reader["ee_md"].ToString();
                    //obj.ee_company = reader["ee_company"].ToString();
                    //obj.ee_oc_code = reader["ee_oc_code"].ToString();
                    //obj.ee_address = reader["ee_address"].ToString();
                    //obj.ee_contact = reader["ee_contact"].ToString();
                    //obj.ee_phone = reader["ee_phone"].ToString();
                    //obj.ee_fax = reader["ee_fax"].ToString();
                    //obj.ee_mail = reader["ee_mail"].ToString();
                    //obj.ee_site = reader["ee_site"].ToString();
                    obj.e_booth = reader["ee_exhBooth"].ToString();
                    //obj.ee_year = (int)reader["ee_year"];
                    var date = reader["ee_exhStartTime"].ToString(); // 展会开始时间
                    if (date != null && date.Length == 8)
                        obj.e_date = date.Substring(0, 4) + "/" + date.Substring(4, 2) + "/" + date.Substring(6, 2);
                    obj.e_name = reader["ee_exhName"].ToString();   // 展会名称
                    obj.e_md = reader["ee_namemd"].ToString();
                    obj.e_trade = reader["ee_exhTrade"].ToString();     // 展会行业
                    //obj.ee_exhArea = reader["ee_exhArea"].ToString();   
                    obj.e_hall = reader["ee_exhHall"].ToString();
                    obj.e_count = (int)reader["ee_exhEntCount"];
                    //obj.ee_createUser = reader["ee_createUser"].ToString();
                    //obj.ee_exhCreateTime = (DateTime)reader["ee_exhCreateTime"];
                    //obj.ee_createTime = (DateTime)reader["ee_createTime"];
                    //obj.ee_imported = (bool)reader["ee_imported"];
                    lst.Add(obj);
                }
                reader.NextResult();
            }
            return lst;
        }



        #endregion

        public List<ExhibitCompany> Exhibit_Companies_Get(DatabaseSearchModel model, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@columns", DbType.String, model.Column);
            Db_0.AddInParameter(dbCommandWrapper, "@where", DbType.String, model.Where);
            Db_0.AddInParameter(dbCommandWrapper, "@order", DbType.String, model.Order);
            Db_0.AddInParameter(dbCommandWrapper, "@page", DbType.Int32, model.PageIndex);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, model.PageSize);
            Db_0.AddOutParameter(dbCommandWrapper, "@rowCount", DbType.Int32, 4);

            var lst = new List<ExhibitCompany>();
            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                while (reader.Read())
                {
                    var obj = new ExhibitCompany();
                    //obj.ee_id = (int)reader["ee_id"];
                    //obj.ee_md = reader["ee_md"].ToString();
                    obj.oc_name = reader["ee_company"].ToString();
                    obj.oc_code = reader["ee_oc_code"].ToString();
                    obj.oc_addr = reader["ee_address"].ToString();
                    //obj.ee_contact = reader["ee_contact"].ToString();
                    obj.oc_tel = reader["ee_phone"].ToString();
                    obj.oc_fax = reader["ee_fax"].ToString();
                    obj.oc_mail = reader["ee_mail"].ToString();
                    obj.oc_site = reader["ee_site"].ToString();
                    //obj.ee_exhBooth = reader["ee_exhBooth"].ToString();
                    //obj.ee_year = (int)reader["ee_year"];
                    //obj.ee_exhStartTime = reader["ee_exhStartTime"].ToString(); // 展会开始时间
                    //obj.e_name = reader["ee_exhName"].ToString();   // 展会名称
                    //obj.e_md = reader["ee_namemd"].ToString();
                    //obj.ee_exhTrade = reader["ee_exhTrade"].ToString();     // 展会行业
                    //obj.ee_exhArea = reader["ee_exhArea"].ToString();   
                    //obj.ee_exhHall = reader["ee_exhHall"].ToString();
                    //obj.e_count = (int)reader["ee_exhEntCount"];
                    //obj.ee_createUser = reader["ee_createUser"].ToString();
                    //obj.ee_exhCreateTime = (DateTime)reader["ee_exhCreateTime"];
                    //obj.ee_createTime = (DateTime)reader["ee_createTime"];
                    //obj.ee_imported = (bool)reader["ee_imported"];
                    lst.Add(obj);
                }
                //reader.NextResult();
            }
            return lst;
        }

        public ExhibitDtl Exhibit_Detail(DatabaseSearchModel model, string sp_Name)
        {
            DbCommand dbCommandWrapper = Db_0.GetStoredProcCommand(sp_Name);
            Db_0.AddInParameter(dbCommandWrapper, "@columns", DbType.String, model.Column);
            Db_0.AddInParameter(dbCommandWrapper, "@where", DbType.String, model.Where);
            Db_0.AddInParameter(dbCommandWrapper, "@order", DbType.String, model.Order);
            Db_0.AddInParameter(dbCommandWrapper, "@page", DbType.Int32, model.PageIndex);
            Db_0.AddInParameter(dbCommandWrapper, "@pageSize", DbType.Int32, model.PageSize);
            Db_0.AddOutParameter(dbCommandWrapper, "@rowCount", DbType.Int32, 4);

            using (IDataReader reader = Db_0.ExecuteReader(dbCommandWrapper))
            {
                if (reader.Read())
                {
                    var obj = new ExhibitDtl();
                    //obj.e_id = (int)reader["e_id"];
                    //obj.e_eshowId = (int)reader["e_eshowId"];
                    //obj.e_year = (int)reader["e_year"];
                    obj.name = reader["e_name"].ToString();
                    //obj.e_namemd = reader["e_namemd"].ToString();
                    obj.trade = reader["e_trade"].ToString();
                    obj.area = reader["e_area"].ToString();
                    obj.start_time = reader["e_startTime"].ToString();
                    obj.hall = reader["e_hall"].ToString();
                    //obj.count = (int)reader["e_count"];
                    //obj.e_file = reader["e_file"].ToString();
                    //obj.e_ext = reader["e_ext"].ToString();
                    //obj.e_createTime = (DateTime)reader["e_createTime"];
                    return obj;
                }
                reader.NextResult();
            }
            return new ExhibitDtl();
        }


    }
}
