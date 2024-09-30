using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SWSUploader
{
    public class Constance
    {
        //int propertyName;
        public string compName = "wooribnc";
        public string SystemTitle = "SWS Data Uploader v2.0";
        //public string ServerIP = "192.168.35.188";
        //-----------------------------------------------------------------------------------------------------------------//
        // Program Update History
        // 2020.07.08 (v1.0) : 최초 신규 
        // 
        //-----------------------------------------------------------------------------------------------------------------//
        public string DBTestQuery = "select GETDATE() ";
        public bool DBConnectInSplash = true;

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
        private string dbFilePathOrg = Environment.CurrentDirectory;
        public string DbFilePathOrg
        {
            get { return dbFilePathOrg; }
        }
        

    }
}
