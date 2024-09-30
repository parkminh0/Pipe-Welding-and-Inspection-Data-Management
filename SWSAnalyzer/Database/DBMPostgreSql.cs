using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
//using Devart.Data;
using Devart.Data.PostgreSql;

namespace SWSAnalyzer
{
    public class DBMPostgreSql
    {
        private PgSqlConnection moleConnect;
        private PgSqlDataAdapter moleAdapter;
        public PgSqlCommand moleCommand;
        private PgSqlTransaction tx;
        private bool isready;

        public int mUpdate(string query)
        {
            int result = -1;
            moleCommand.CommandType = CommandType.Text;
            if (!isready) return result;

            moleCommand.CommandText = query;
            moleCommand.Connection.Open();
            moleCommand.Parameters.Clear();

            result = moleCommand.ExecuteNonQuery();
            moleCommand.Connection.Close();

            return result;
        }

        public void mOpen()
        {
            try
            {
                moleCommand.Connection.Open();
            }
            catch (Exception pException)
            {
                throw new ExceptionManager(this, "Connection.Open()", pException);
            }
        }

        public void mClose()
        {
            try
            {
                moleCommand.Connection.Close();
            }
            catch (Exception pException)
            {
                throw new ExceptionManager(this, "mClose()", pException);
            }
        }

        public void mNonQuery(string query)
        {
            moleCommand.CommandType = CommandType.Text;
            moleCommand.CommandText = query;
            try
            {
                moleCommand.ExecuteNonQuery();
            }
            catch (Exception pException)
            {
                throw new ExceptionManager(this, "mNonQuery(string query)", pException);
            }
        }

        /// <summary>
        /// Tranction처리
        /// Commit / Rollback
        /// </summary>
        /// <param name="querys"></param>
        /// <returns></returns>
        //public bool ExcuteTransaction(string[] querys)
        //{
        //    bool bl = true;
        //    moleCommand.Connection.Open();
        //    tx = moleConnect.BeginTransaction();
        //    moleCommand.Transaction = tx;
        //    try
        //    {
        //        for (int i = 0; i < querys.Length; i++)
        //        {
        //            moleCommand.CommandType = CommandType.Text;
        //            moleCommand.CommandText = querys[i];
        //            moleCommand.ExecuteNonQuery();
        //        }

        //        tx.Commit();   //Commit
        //    }
        //    catch (Exception)
        //    {
        //        bl = false;
        //        tx.Rollback(); //Rollback
        //    }
        //    finally
        //    {
        //        moleCommand.Connection.Close();
        //    }

        //    return bl;
        //}
        /// <summary>
        /// Transaction처리 List
        /// </summary>
        /// <param name="querys"></param>
        /// <returns></returns>
        //public bool ExcuteTransaction(List<string> querys)
        //{
        //    string[] sqls = querys.ToArray();
        //    return ExcuteTransaction(sqls);
        //}

        /// <summary>
        /// Tranction처리
        /// Commit / Rollback
        /// </summary>
        /// <param name="querys"></param>
        /// <returns>작업 성공 시 빈문자열 Return, 실패 시 Exception Message Return</returns>
        public string ExcuteTransaction(string[] querys)
        {
            string str = "";
            moleCommand.Connection.Open();
            tx = moleConnect.BeginTransaction();
            moleCommand.Transaction = tx;
            try
            {
                for (int i = 0; i < querys.Length; i++)
                {
                    moleCommand.CommandType = CommandType.Text;
                    moleCommand.CommandText = querys[i];
                    moleCommand.ExecuteNonQuery();
                }

                tx.Commit();   //Commit
            }
            catch (Exception ex)
            {
                str = ex.Message;
                tx.Rollback(); //Rollback
            }
            finally
            {
                moleCommand.Connection.Close();
            }

            return str;
        }
        /// <summary>
        /// Transaction처리 List
        /// </summary>
        /// <param name="querys"></param>
        /// <returns>작업 성공 시 빈문자열 Return, 실패 시 Exception Message Return</returns>
        public string ExcuteTransaction(List<string> querys)
        {
            string[] sqls = querys.ToArray();
            return ExcuteTransaction(sqls);
        }

        /// <summary>
        /// count, sum 등 Integer형 sql 실행
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int GetIntScalar(string query)
        {
            int v = 0;
            if (!isready) return v;

            moleCommand.CommandType = CommandType.Text;
            moleCommand.CommandText = query;
            moleCommand.Connection.Open();
            try
            {
                v = (int)moleCommand.ExecuteScalar();
                moleCommand.Connection.Close();
            }
            catch (Exception ex)
            {
                moleCommand.Connection.Close();
                throw ex;
            }

            return v;
        }

        /// <summary>
        /// count, sum 등 Integer형 sql 실행
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public float getFloatScalar(string query)
        {
            float v = 0.0f;
            if (!isready) return v;

            moleCommand.CommandType = CommandType.Text;
            moleCommand.CommandText = query;
            moleCommand.Connection.Open();
            try
            {
                v = float.Parse(moleCommand.ExecuteScalar().ToString());
                moleCommand.Connection.Close();
            }
            catch (Exception ex)
            {
                moleCommand.Connection.Close();
                throw ex;
            }

            return v;
        }

        /// <summary>
        /// count, sum 등 Integer형 sql 실행
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public decimal getDecimalScalar(string query)
        {
            decimal v = 0.0m;
            if (!isready) return v;

            moleCommand.CommandType = CommandType.Text;
            moleCommand.CommandText = query;
            moleCommand.Connection.Open();
            try
            {
                v = decimal.Parse(moleCommand.ExecuteScalar().ToString());
                moleCommand.Connection.Close();
            }
            catch (Exception ex)
            {
                moleCommand.Connection.Close();
                throw ex;
            }

            return v;
        }

        /// <summary>
        /// count, sum 등 Integer형 sql 실행
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public double getDoubleScalar(string query)
        {
            double v = 0;
            if (!isready) return v;

            moleCommand.CommandType = CommandType.Text;
            moleCommand.CommandText = query;
            moleCommand.Connection.Open();
            try
            {
                v = double.Parse(moleCommand.ExecuteScalar().ToString());
                moleCommand.Connection.Close();
            }
            catch (Exception ex)
            {
                moleCommand.Connection.Close();
                throw ex;
            }

            return v;
        }

        /// <summary>
        /// 단순 한개의 문자 데이터 추출
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string getStringScalar(string query)
        {
            string str = string.Empty;
            if (!isready) return str;

            moleCommand.CommandType = CommandType.Text;
            moleCommand.CommandText = query;
            moleCommand.Connection.Open();
            try
            {
                str = (string)moleCommand.ExecuteScalar();
                moleCommand.Connection.Close();
            }
            catch (Exception ex)
            {
                moleCommand.Connection.Close();
                throw ex;
            }

            return str;
        }

        /// <summary>
        /// datetime값 하나만 가져오기
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DateTime getDateTimeScalar(string query)
        {
            DateTime dteValue = new DateTime();
            if (!isready) return dteValue;

            moleCommand.CommandType = CommandType.Text;
            moleCommand.CommandText = query;
            moleCommand.Connection.Open();
            try
            {
                dteValue = DateTime.Parse(moleCommand.ExecuteScalar().ToString());
                moleCommand.Connection.Close();
            }
            catch (Exception ex)
            {
                moleCommand.Connection.Close();
                throw ex;
            }

            return dteValue;
        }

        /// <summary>
        /// 결과를 DataTable 로 리턴 한다.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string query)
        {
            DataTable dt = new DataTable();
            if (!isready) return dt;

            moleCommand.CommandType = CommandType.Text;
            moleCommand.CommandText = query;

            moleAdapter.Fill(dt);
            return dt;
        }

        string connStr;
        /// <summary>
        /// 디비 준비 시키기.
        /// </summary>
        /// <param name="pdbm"></param>
        public string SetReady(string strs)
        {
            string result = "";
            connStr = DumpBuilderContents(strs);
            moleConnect = new PgSqlConnection(connStr);
            moleCommand = new PgSqlCommand();
            moleCommand.Connection = moleConnect;
            moleAdapter = new PgSqlDataAdapter(moleCommand);

            try
            {
                moleCommand.CommandText = Program.constance.DBTestQuery;
                moleAdapter.Fill(new DataTable());
                isready = true;
            }
            catch (Exception e)
            {
                isready = false;
                result = e.Message;
            }
            return result;
        }

        private string DumpBuilderContents(string connectString)
        {
            PgSqlConnectionStringBuilder builder = new PgSqlConnectionStringBuilder(connectString);
            return builder.ConnectionString;
        }

        /// <summary>
        /// DB 연결 준비 여부
        /// </summary>
        public bool IsReady
        {
            get
            {
                return isready;
            }
        }

        
        /// <summary>
        /// 프로시저 호출 함수 샘플
        /// </summary>
        /// <param name="ym"></param>
        /// <param name="mixProdNo"></param>
        /// <returns></returns>
        public int SPProcedureSample(string ym, double adjAmount, bool isDelete)
        {
            int result = 0;
            string delYn;
            if (isDelete)
                delYn = "Y";
            else
                delYn = "N";
            moleCommand.CommandType = CommandType.StoredProcedure;
            moleCommand.CommandText = "sp_AdjustAccount";
            moleCommand.Parameters.Clear();
            moleCommand.Parameters.Add("@iBaseYearMonth", PgSqlType.VarChar, 6);
            moleCommand.Parameters.Add("@iAdjustAmount", PgSqlType.Real);
            moleCommand.Parameters.Add("@iDeleteYn", PgSqlType.VarChar, 1);
            moleCommand.Parameters.Add("@oResult", PgSqlType.Int);
            moleCommand.Parameters["@iBaseYearMonth"].Value = ym;
            moleCommand.Parameters["@iAdjustAmount"].Value = adjAmount;
            moleCommand.Parameters["@iDeleteYn"].Value = delYn;
            moleCommand.Parameters["@oResult"].Direction = ParameterDirection.Output;

            try
            {
                moleCommand.Connection.Open();
                moleCommand.ExecuteNonQuery();
                result = int.Parse(moleCommand.Parameters["@oResult"].Value.ToString());
                moleCommand.Connection.Close();
            }
            catch (Exception)
            {
                moleCommand.Connection.Close();
                throw;
            }

            return result;
        }
    }
}
