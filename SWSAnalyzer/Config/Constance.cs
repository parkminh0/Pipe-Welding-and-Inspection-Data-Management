using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SWSAnalyzer
{
    public class Constance
    {
        //public string ServerIP = "localhost";
        //public string ServerIP = "192.168.35.188";
        public string compName = "wooribnc";
        public string SystemTitle = "SWS Data Analyzer v2.0";
        //-----------------------------------------------------------------------------------------------------------------//
        // Program Update History
        // 2020.08.05 (v1.0) : 최초 신규 
        // 
        //-----------------------------------------------------------------------------------------------------------------//
        public string DBTestQuery = "select GETDATE() ";
        public bool DBConnectInSplash = true;
        public int AngleUnit = 10; //각도 단위

        /// <summary>
        /// OS공통 작업폴더
        /// </summary>
        public string CommonFilePath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }
        }

        /// <summary>
        /// DB FILE위치
        /// </summary>
        //private string dbFilePathOrg = Environment.CurrentDirectory;
        //public string DbFilePathOrg
        //{
        //    get { return dbFilePathOrg; }
        //}

        //private string dbFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wooribnc");
        //private string dbFilePath;
        ///// <summary>
        ///// 실제 DB파일이 위치할 폴더
        ///// </summary>
        //public string DbFilePath
        //{
        //    get { return dbFilePath; }
        //    set
        //    {
        //        dbFilePath = value;
        //    }
        //}

        //private string dbFileName = "ALCStaff.db"; // SQLite
        //public string DbFileName
        //{
        //    get { return dbFileName; }
        //}

        ///// <summary>
        ///// 실제 DB파일의 FULL PATH + Name
        ///// </summary>
        //public string DbTargetFullName
        //{
        //    get
        //    {
        //        return Path.Combine(dbFilePath, dbFileName);
        //    }
        //}

        //private string dbBaseFileName = "ALCStaff_Initial.db";
        //private string dbInitFileName = "ALCStaff_Initial.db";
        ///// <summary>
        ///// Initial DB Name
        ///// </summary>
        //public string DbInitFileName
        //{
        //    get { return dbInitFileName; }
        //}
        ///// <summary>
        ///// 원본 Init파일 Full Path + Name
        ///// </summary>
        //public string DbInitOrgFullName
        //{
        //    get
        //    {
        //        return Path.Combine(dbFilePathOrg, dbInitFileName);
        //    }
        //}

        ///// <summary>
        ///// MainDB가 되기 직전의 Full Name
        ///// </summary>
        //public string DbInitFullName
        //{
        //    get
        //    {
        //        return Path.Combine(dbFilePath, dbInitFileName);
        //    }
        //}

        ///// <summary>
        ///// 원본 base파일 Full Path + Name : 초기데이터 들어있는 Init DB
        ///// </summary>
        //public string DbBaseOrgFullName
        //{
        //    get
        //    {
        //        return Path.Combine(dbFilePathOrg, dbBaseFileName);
        //    }
        //}

    }
}
