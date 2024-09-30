using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace SWSUploader
{
    public class DBMSSQL
    {
        private SqlConnection moleConnect;
        private SqlDataAdapter moleAdapter;
        public SqlCommand moleCommand;
        private SqlTransaction tx;
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
        /// sql Bulk copy
        /// </summary>
        /// <param name="keepNulls"></param>
        /// <param name="reader"></param>
        public void DoBulkCopy(DataTable dt, string targetTableName)
        {
            moleCommand.Connection.Open();
            SqlBulkCopy bulkCopy = new SqlBulkCopy(moleConnect);
            bulkCopy.DestinationTableName = targetTableName;
            bulkCopy.WriteToServer(dt);
            bulkCopy.Close();
            moleCommand.Connection.Close();
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
            moleConnect = new SqlConnection(connStr);
            moleCommand = new SqlCommand();
            moleCommand.Connection = moleConnect;
            moleAdapter = new SqlDataAdapter(moleCommand);

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
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectString);
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
        /// Bulk Insert 다른 방식 (Delete후 Insert함)
        /// </summary>
        /// <param name="dtTable"></param>
        /// <param name="tableName"></param>
        public void BulkCopyDI(DataTable dtTable, string tableName)
        {
            SqlCommand cmd;
            using (SqlConnection sConn = new SqlConnection(connStr))
            {
                sConn.Open();
                cmd = sConn.CreateCommand();
                cmd.CommandText = string.Format("DELETE FROM {0} WHERE CreateID = '{1}' ",tableName, Program.Option.LoginID); //데이터 삭제 쿼리 
                SqlTransaction sTran = sConn.BeginTransaction(); //트랜잭션 객체 생성
                try
                {
                    cmd.Transaction = sTran;
                    cmd.ExecuteNonQuery(); //데이터 삭제 실행
                }
                catch (System.Exception ex)
                {
                    sTran.Rollback(); //트랜잭션 롤백
                    throw new Exception(ex.ToString());
                }
                finally
                {
                    cmd.Dispose();
                }

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sConn, SqlBulkCopyOptions.Default, sTran)) //SqlBulkCopy 객체 생성, SqlBulkCopyOptions은 열거형으로 옵션을 지정해줌. 여기선 기본설정,  세번째 인자로 트랜잭션 객체를 지정함.
                {
                    bulkCopy.DestinationTableName = tableName; //대상 테이블 설정
                    //따로 쿼리가 필요없음, SqlBulkCopy 클래스는 sql server 테이블로의 입력만 수행함.
                    try
                    {
                        bulkCopy.WriteToServer(dtTable); //대량 카피 실행!
                        sTran.Commit(); //트랜잭션 커밋
                    }
                    catch (System.Exception ex)
                    {
                        sTran.Rollback(); //트랜잭션 롤백
                        throw new Exception(ex.ToString());
                    }
                    finally
                    {
                        dtTable = null;
                    }
                }
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
            moleCommand.Parameters.Add("@iBaseYearMonth", SqlDbType.VarChar, 6);
            moleCommand.Parameters.Add("@iAdjustAmount", SqlDbType.Float);
            moleCommand.Parameters.Add("@iDeleteYn", SqlDbType.VarChar, 1);
            moleCommand.Parameters.Add("@oResult", SqlDbType.Int);
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

        /// <summary>
        /// 각종 Key 채번
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public int uspGetNextKey(string category, int interval = 1)
        {
            int result = 0;
            moleCommand.CommandType = CommandType.StoredProcedure;
            moleCommand.CommandText = "usp_GetNextKey";
            moleCommand.Parameters.Clear();
            moleCommand.Parameters.Add("@iCategory", SqlDbType.VarChar, 30);
            moleCommand.Parameters.Add("@iInterval", SqlDbType.Int);
            moleCommand.Parameters.Add("@oNextKey", SqlDbType.Int);
            moleCommand.Parameters["@iCategory"].Value = category;
            moleCommand.Parameters["@iInterval"].Value = interval;
            moleCommand.Parameters["@oNextKey"].Direction = ParameterDirection.Output;

            try
            {
                moleCommand.Connection.Open();
                moleCommand.ExecuteNonQuery();
                result = int.Parse(moleCommand.Parameters["@oNextKey"].Value.ToString());
                moleCommand.Connection.Close();
            }
            catch (Exception)
            {
                moleCommand.Connection.Close();
                throw;
            }

            return result;
        }

        /// <summary>
        /// Parameter가 있는 트랜잭션 실행
        /// </summary>
        /// <param name="querys"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public string ExcuteTransactionParameter(ParamQuery[] querys)
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
                    moleCommand.CommandText = querys[i].Sql;
                    moleCommand.Parameters.AddRange(querys[i].ParamCollection.ToArray());
                    moleCommand.ExecuteNonQuery();
                    moleCommand.Parameters.Clear();
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
    }
}
