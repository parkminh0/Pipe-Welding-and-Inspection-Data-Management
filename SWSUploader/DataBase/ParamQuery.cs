using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SWSUploader
{
    public class ParamQuery
    {
        private string sql;
        List<SqlParameter> paramList;

        /// <summary>
        /// 생성자
        /// </summary>
        public ParamQuery()
        {
            paramList = new List<SqlParameter>();
        }

        public string Sql
        {
            get
            {
                return sql;
            }

            set
            {
                this.sql = value;
            }
        }

        public List<SqlParameter> ParamCollection
        {
            get
            {
                return paramList;
            }

            set
            {
                this.paramList = value;
            }
        }

        public void AddParameter(SqlParameter sqlParam)
        {
            paramList.Add(sqlParam);
        }

        public void AddParameter(string parameterName, object value)
        {
            paramList.Add(new SqlParameter(parameterName, value));
        }

    }
}
