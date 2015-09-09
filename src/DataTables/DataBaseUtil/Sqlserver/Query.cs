using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DataTables.DatabaseUtil;

namespace DataTables.DatabaseUtil.Sqlserver
{
    public class Query : DataTables.Query
    {
        public Query(Database db, string type)
            : base(db, type)
        {
        }


        /// <summary>
        /// Create LIMIT / OFFSET for SQL Server 2012+. Note that this will only work
        /// with SQL Server 2012 or newer due to the use of the OFFSET and FETCH NEXT
        /// keywords
        /// </summary>
        /// <returns>Limit / offset string</returns>
        override protected string _BuildLimit()
        {
            string limit = "";

            if (_offset != -1)
            {
                limit = " OFFSET " + _offset + " ROWS";
            }

            if (_limit != -1)
            {
                if (_offset == -1)
                {
                    limit += " OFFSET 0 ROWS";
                }

                limit += " FETCH NEXT " + _limit + " ROWS ONLY";
            }

            return limit;
        }


        override protected void _Prepare(string sql)
        {
            DbParameter param;
            var provider = DbProviderFactories.GetFactory(_db.Adapter());
            var cmd = provider.CreateCommand();

            if (_type == "insert")
            {
                var pkeyCmd = provider.CreateCommand();

                pkeyCmd.CommandText = "SELECT column_name " +
                    "FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE " +
                    "WHERE OBJECTPROPERTY(OBJECT_ID(constraint_name), 'IsPrimaryKey') = 1 " +
                    "AND table_name = @table";
                pkeyCmd.Connection = _db.Conn();
                pkeyCmd.Transaction = _transaction;

                param = pkeyCmd.CreateParameter();
                param.ParameterName = "@table";
                param.Value = _table[0];
                pkeyCmd.Parameters.Add(param);

                var dr = pkeyCmd.ExecuteReader();

                // If the table doesn't have a primary key field, we can't get
                // the inserted pkey!
                if (dr.HasRows && dr.Read())
                {
                    sql = sql.Replace(" VALUES (", " OUTPUT INSERTED." + dr["column_name"] + " as insert_id VALUES (");
                }

                dr.Close();
            }

            cmd.CommandText = sql;
            cmd.Connection = _db.Conn();
            cmd.Transaction = _transaction;

            // Bind values
            for (int i = 0, ien = _bindings.Count; i < ien; i++)
            {
                var binding = _bindings[i];

                param = cmd.CreateParameter();
                param.ParameterName = binding.Name;
                param.Value = binding.Value ?? DBNull.Value;

                if (binding.Type != null)
                {
                    param.DbType = binding.Type;
                }

                cmd.Parameters.Add(param);
            }

            _stmt = cmd;
        }


        override protected DataTables.Result _Exec()
        {
            var dt = new System.Data.DataTable();
            var dr = _stmt.ExecuteReader();

            dt.Load(dr);
            dr.Close();

            return new Sqlserver.Result(_db, dt, this);
        }
    }
}
