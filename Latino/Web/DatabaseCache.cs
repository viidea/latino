/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    DatabaseCache.cs
 *  Desc:    Database cache for Web search engine results
 *  Created: Mar-2007
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Security.Cryptography;
using System.Data;
using System.Text;
using System.Xml;
using System.IO;
using Latino.Persistance;
using Latino.TextMining;

namespace Latino.Web
{
    /* .-----------------------------------------------------------------------
       |
       |  Class DatabaseCache
       |
       '-----------------------------------------------------------------------
    */
    public class DatabaseCache : ISearchEngineCache
    {
        private DatabaseConnection mConnection;
        private int mTtl
            = 0;

        public WebUtils.NormalizeQueryDelegate NormalizeQuery
            = WebUtils.NormalizeQuery;

        private static Logger mLogger
            = Logger.GetLogger(typeof(DatabaseCache));

        public DatabaseCache(string connectionString, string username, string password, string database, string server)
		{
            mConnection = new DatabaseConnection();
            mConnection.ConnectionString = connectionString; // throws ArgumentNullException
            mConnection.Username = username; // throws ArgumentNullException
            mConnection.Password = password; // throws ArgumentNullException
            mConnection.Database = database; // throws ArgumentNullException
            mConnection.Server = server; // throws ArgumentNullException
            mConnection.Connect(); // throws OleDbException
		}

        public DatabaseCache(ConnectionType connectionType, string username, string password, string database, string server)
        {
            mConnection = new DatabaseConnection();
            mConnection.SetConnectionString(connectionType);
            mConnection.Username = username; // throws ArgumentNullException
            mConnection.Password = password; // throws ArgumentNullException
            mConnection.Database = database; // throws ArgumentNullException
            mConnection.Server = server; // throws ArgumentNullException
            mConnection.Connect(); // throws OleDbException
        }

        public DatabaseCache(string connectionString)
        {
            mConnection = new DatabaseConnection();
            mConnection.ConnectionString = connectionString; // throws ArgumentNullException
            mConnection.Username = "";
            mConnection.Password = "";
            mConnection.Database = "";
            mConnection.Server = "";
            mConnection.Connect(); // throws OleDbException
        }

        public int Ttl
        {
            get { return mTtl; }
            set
            {
                Utils.ThrowException(value < 0 ? new ArgumentOutOfRangeException("Ttl") : null);
                mTtl = value;
            }
        }

        public void Disconnect()
        {
            Utils.ThrowException(mConnection == null ? new InvalidOperationException() : null);
            mConnection.Disconnect(); 
            mConnection = null;
        }

        // *** ISearchEngineCache interface implementation ***

        public bool GetFromCache(string source, Language language, string query, int maxSize, ref long totalHits, ref SearchEngineResultSet resultSet)
        {
            Utils.ThrowException(source == null ? new ArgumentNullException("source") : null);
            Utils.ThrowException(query == null ? new ArgumentNullException("query") : null);
            Utils.ThrowException(maxSize < 0 ? new ArgumentOutOfRangeException("maxSize") : null);
            Utils.ThrowException(mConnection == null ? new InvalidOperationException() : null);
            string normalizedQuery = string.Format("{0} {1} {2}", source, language, NormalizeQuery == null ? query : NormalizeQuery(query));
            bool cacheMiss = true;
            DataTable dataTable = mConnection.ExecuteQuery("select * from Queries where Query = ?", normalizedQuery);
            if (dataTable.Rows.Count != 0) 
            {
				DateTime timeStamp = (DateTime)dataTable.Rows[0]["TimeStamp"];
                if (mTtl == 0 || DateTime.Now.Subtract(timeStamp).TotalDays <= mTtl) // record is not outdated 
                {
                    int actualSizeCached = (int)dataTable.Rows[0]["ActualSize"];
                    long totalHitsCached = (long)dataTable.Rows[0]["TotalHits"];
                    if (totalHitsCached == actualSizeCached || maxSize <= actualSizeCached)
                    {
                        totalHits = totalHitsCached;
                        XmlTextReader xmlReader = new XmlTextReader(new StringReader((string)dataTable.Rows[0]["ResultSetXml"]));
                        resultSet = new SearchEngineResultSet(xmlReader, maxSize);
                        mLogger.Trace("GetFromCache", "Cache hit.");
                        cacheMiss = false;
                    }
                }
            }
            if (cacheMiss) { mLogger.Trace("GetFromCache", "Cache miss."); }
            return !cacheMiss;
        }

        public void PutIntoCache(string source, Language language, string query, long totalHits, SearchEngineResultSet resultSet)
        {
            Utils.ThrowException(source == null ? new ArgumentNullException("source") : null);
            Utils.ThrowException(query == null ? new ArgumentNullException("query") : null);
            Utils.ThrowException(resultSet == null ? new ArgumentNullException("resultSet") : null);
            Utils.ThrowException(totalHits < resultSet.Count ? new ArgumentValueException("totalHits") : null);
            string normalizedQuery = string.Format("{0} {1} {2}", source, language, NormalizeQuery == null ? query : NormalizeQuery(query));
            mConnection.StartTransaction(); // start transaction
            // check if such query already exists
            DataTable dataTable = mConnection.ExecuteQuery("select * from Queries where Query = ?", normalizedQuery);
            if (dataTable.Rows.Count > 0)
            {
                // remove old database record 
                mConnection.ExecuteNonQuery("delete from Queries where Query = ?", normalizedQuery);
            }
            // add new database record
            StringWriter strWriter = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(strWriter);
            xmlWriter.Formatting = Formatting.Indented;
            resultSet.SaveXml(xmlWriter);
            mConnection.ExecuteNonQuery("insert into Queries (Query, TotalHits, ActualSize, ResultSetXml, TimeStamp) values (?, ?, ?, ?, ?)",
                normalizedQuery, totalHits, resultSet.Count, strWriter.ToString(), DateTime.Now);
            mConnection.Commit(); // commit
        }
    }
}