using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;
using System.Data.SQLite;
using OwinDemo.AuthorizationServer.Model;
using SQLinq;
using SQLinq.Dapper;
using System.Linq.Expressions;

namespace DapperDal
{
    public abstract class BaseDal<T> where T:new()
    {
        //set sqlCUD for different tables
        public BaseDal()
        {
            SetSqlCUD();
        }
        protected SqlCUD sqlCUD = new SqlCUD();
        public abstract void SetSqlCUD();

        //get a conncection
        private static readonly string sqlconnection = @"Data Source=" + AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\AuthData.db;Version=3;";
        public IDbConnection GetOpenConnection()
        {
            IDbConnection connection = new SQLiteConnection(sqlconnection);
            connection.Open();
            return connection;
        }


        #region query

        //getEntities
        public IEnumerable<T> GetEntities(Expression<Func<T, bool>> predicate)
        {
            using (IDbConnection conn = GetOpenConnection())
            {
                try
                {
                    SQLinq<T> query = new SQLinq<T>().Where(predicate);
                    return conn.Query<T>(query);
                }
                catch
                {
                    return null;
                }
            }
        }

        //getPagedEntities
        public IEnumerable<T> GetPagedEntities(int pageSize, int pageIndex, out int total, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> keySelector, bool isAsc)
        {
            int skipCount = pageSize * (pageIndex - 1);
            total = 0;
            //create  sqlQuery
            SQLinq<T> query = new SQLinq<T>().Where(predicate);
            if (isAsc)
            {
                query = query.OrderBy(keySelector);
            }
            else
            {
                query = query.OrderByDescending(keySelector);
            }
            query.Skip(skipCount).Take(pageSize);
            //do  query
            return Query(query);
        }

        #endregion

        #region CUD
        //insert
        public int Insert(T entity)
        {
            using (IDbConnection conn = GetOpenConnection())
            {
                string query = sqlCUD.Add;//"insert into Users(sName,sGender,sAge) values(@sNmae,@sGender,@sAge)";
                int row = conn.Execute(query, entity);//new {sName="GoldenKey",sGender=true,sAge=22 });
                return row;
            }
        }

        //update
        public int Update(T entity)
        {
            using (IDbConnection conn = GetOpenConnection())
            {
                string query = sqlCUD.Update;//"update Users set sName=@sName,sGender=@sGender,sAge=@sAge where sId=@sId";
                int row = conn.Execute(query, entity);
                return row;
            }
        }

        //delete
        public int Delete(T entity)
        {
            using (IDbConnection conn = GetOpenConnection())
            {
                string query = sqlCUD.Delete;//"delete from Users where sId=@sId";
                int row = conn.Execute(query, entity);
                return row;
            }
        }
        #endregion

        #region Helper Methods
        private IEnumerable<T> Query(SQLinq<T> query)
        {
            try
            {
                using (IDbConnection dbConnection = GetOpenConnection())
                {
                    return dbConnection.Query<T>(query);
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion



    }
}
