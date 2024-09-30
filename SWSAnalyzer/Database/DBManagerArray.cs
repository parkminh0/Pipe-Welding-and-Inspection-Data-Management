using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Data.SQLite;

namespace SWSAnalyzer
{
    public class DBManagerArray
    {
        /// <summary>
        /// 인덱서
        /// </summary>
        /// <param name="index">Instance 번호</param>
        /// <returns></returns>
        public DBManagerArray this[int index]
        {
            get
            {
                if (index < 0 || index >= CONN_CNT)
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    return instance[index];
                }
            }
            set
            {
                if (!(index < 0 || index >= CONN_CNT))
                {
                    instance[index] = value;
                }
            }
        }

        public static string DBMErrString;
        static DBManagerArray[] instance = new DBManagerArray[INSTANCE_CNT];
        public static DBManagerArray[] Instance
        {
            get
            {
                for (int i = 0; i < INSTANCE_CNT; i++)
                {
                    if (instance[i] == null)
                    {
                        instance[i] = new DBManagerArray();
                    }
                }                
                return instance;
            }
        }
        OptionInfo OCF
        {
            get
            {
                return Program.Option;
            }
        }

        const int CONN_CNT = 2; // 한개 쓰레드의 커넥션 수
        const int INSTANCE_CNT = 5; // 쓰레드의 수
        DBMSSQL[] dbArr = new DBMSSQL[CONN_CNT];
        string pswd = AES.AESDecrypt256("6snUHxhR5ee36Xa5C6NcQg==", Program.constance.compName); // passw0rd132$
        /// <summary>
        /// 생성자
        /// </summary>
        public DBManagerArray()
        {
            lock (thisLock)
            {
                //string connStr = string.Format("Persist Security Info = False; Data Source='{0}';Password='{1}';Max DataBase Size=4091", Program.constance.DbTargetFullName, pswd);
                string connStr = "Data Source=" + Program.constance.ServerIP + ";Initial Catalog=JWDS;Persist Security Info=True;User ID=sa;Password=" + pswd;
                //string connStr = "Database=TEST;UserID=db2admin; Password=password;Server=IS500"; // DB2 ConnectionString Sample
                for (int i = 0; i < dbArr.Length; i++)
                {
                    //dbArr[i] = new DBMSSQLCE();
                    dbArr[i] = new DBMSSQL();
                    DBMErrString = dbArr[i].SetReady(connStr);
                }
            }
        }

        object thisLock = new object();
        int currIdx = -1;
        //public DBMSSQLCE MDB
        public DBMSSQL MDB
        {
            get
            {
                //DBMSSQLCE db = null;
                DBMSSQL db = null;
                lock (thisLock)
                {
                    if (currIdx >= CONN_CNT - 1)
                    {
                        currIdx = -1;
                    }
                    currIdx++;
                    db = dbArr[currIdx];
                }
                return db;
            }
        }

        /// <summary>
        /// Database에서 데이터를 불러와 DataTable에 담아 넘겨줌
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string query)
        {
            DataTable resultDT = null;
            try
            {
                resultDT = MDB.GetDataTable(query);
            }
            catch (ExceptionManager pException)
            {
                MDB.moleCommand.Connection.Close();
                Program.WMSG.MSG(string.Format("Exception Method = {0}\r\n InnerException = {1} \r\n Message = {2} ", pException.Method, pException.InnerException.Message, pException.Message));
            }
            catch (Exception e)
            {
                MDB.moleCommand.Connection.Close();
                Program.WMSG.MSG(e.Message);
            }

            return resultDT;
        }

        /// <summary>
        /// 실행
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int ExcuteDataUpdate(string query)
        {
            int result = -1;
            try
            {
                result = MDB.mUpdate(query);
            }
            catch (ExceptionManager pException)
            {
                MDB.moleCommand.Connection.Close();
                Program.WMSG.MSG(string.Format("Exception Method = {0}\r\n InnerException = {1} \r\n Message = {2} ", pException.Method, pException.InnerException.Message, pException.Message));
            }
            catch (Exception e)
            {
                MDB.moleCommand.Connection.Close();
                Program.WMSG.MSG(e.Message);
            }

            return result;
        }

        /// <summary>
        /// Int값 하나 가져오기
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int GetIntScalar(string query)
        {
            int result = 0;
            try
            {
                result = MDB.GetIntScalar(query);
            }
            catch (Exception e)
            {
                MDB.moleCommand.Connection.Close();
                Program.WMSG.MSG(e.Message);
            }

            return result;
        }

        /// <summary>
        /// Int값 하나 가져오기
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public float GetFloatScalar(string query)
        {
            float result = 0;
            try
            {
                result = MDB.getFloatScalar(query);
            }
            catch (Exception e)
            {
                MDB.moleCommand.Connection.Close();
                Program.WMSG.MSG(e.Message);
            }

            return result;
        }

        /// <summary>
        /// Double값 하나 가져오기
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public double GetDoubleScalar(string query)
        {
            double result = 0;
            try
            {
                result = MDB.getDoubleScalar(query);
            }
            catch (Exception e)
            {
                MDB.moleCommand.Connection.Close();
                Program.WMSG.MSG(e.Message);
            }

            return result;
        }

        /// <summary>
        /// Double값 하나 가져오기
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public decimal GetDecimalScalar(string query)
        {
            decimal result = 0;
            try
            {
                result = MDB.getDecimalScalar(query);
            }
            catch (Exception e)
            {
                MDB.moleCommand.Connection.Close();
                Program.WMSG.MSG(e.Message);
            }

            return result;
        }

        /// <summary>
        /// string값 하나 가져오기
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string GetStringScalar(string query)
        {
            string resultString = string.Empty;
            try
            {
                resultString = MDB.getStringScalar(query);
            }
            catch (Exception e)
            {
                MDB.moleCommand.Connection.Close();
                Program.WMSG.MSG(e.Message);
            }

            return resultString;
        }

        /// <summary>
        /// 벌크 Insert (Only Insert)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="targetTableName"></param>
        /// <returns></returns>
        //public bool DoBulkCopy(DataTable dt, string targetTableName)
        //{
        //    try
        //    {
        //        MDB.DoBulkCopy(dt, targetTableName);
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        MDB.moleCommand.Connection.Close();
        //        Program.WMSG.MSG(e.Message);
        //        return false;
        //        //throw;
        //    }
        //}

        /// <summary>
        /// 벌크 Insert (Truncate and Insert)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="targetTableName"></param>
        /// <returns></returns>
        //public bool DoBulkCopy2(DataTable dt, string targetTableName)
        //{
        //    try
        //    {
        //        MDB.BulkCopy2(dt, targetTableName);
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        MDB.moleCommand.Connection.Close();
        //        Program.WMSG.MSG(e.Message);
        //        return false;
        //        //throw;
        //    }
        //}

    }
}
