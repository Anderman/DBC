// <copyright>Copyright (c) 2014 SpryMedia Ltd - All Rights Reserved</copyright>
//
// <summary>
// Editor class for reading tables as well as creating, editing and deleting rows
// </summary>
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Web;
using DataTables.EditorUtil;

namespace DataTables
{
    /// <summary>
    /// DataTables Editor base class for creating editable tables.
    ///
    /// Editor class instances are capable of servicing all of the requests that
    /// DataTables and Editor will make from the client-side - specifically:
    ///
    /// * Get data
    /// * Create new record
    /// * Edit existing record
    /// * Delete existing records
    ///
    /// The Editor instance is configured with information regarding the
    /// database table fields that you which to make editable, and other information
    /// needed to read and write to the database (table name for example!).
    ///
    /// This documentation is very much focused on describing the API presented
    /// by these DataTables Editor classes. For a more general overview of how
    /// the Editor class is used, and how to install Editor on your server, please
    /// refer to the Editor manual ( https://editor.datatables.net/manual ).
    /// </summary>
    public class Editor
    {
        /// <summary>
        /// Version string
        /// </summary>
        public const string Version = "1.4.2";

        /// <summary>
        /// Create a new Editor instance
        /// </summary>
        /// <param name="db">An instance of the DataTables Database class that we can use for the DB connection. Can also be set with the <code>Db()</code> method.</param>
        /// <param name="table">The table name in the database to read and write information from and to. Can also be set with the <code>Table()</code> method.</param>
        /// <param name="pkey">Primary key column name in the table given. Can also be set with the <code>PKey()</code> method.</param>
        public Editor(Database db = null, string table = null, string pkey = null)
        {
            if (db != null)
            {
                Db(db);
            }

            if (table != null)
            {
                Table(table);
            }

            if (pkey != null)
            {
                Pkey(pkey);
            }
        }


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Private parameters
         */

        private Database _Db;
        private List<Field> _Field = new List<Field>();
        private string _IdPrefix = "row_";
        private DtRequest _ProcessData;
        private Dictionary<string, object> _FormData;
        private Type _UserModelT;
        private string _Pkey = "id";
        private List<string> _Table = new List<string>();
        private bool _Transaction = true;
        private List<WhereCondition> _Where = new List<WhereCondition>();
        private List<LeftJoin> _LeftJoin = new List<LeftJoin>();
        private DtResponse _Out = new DtResponse();
        private List<MJoin> _MJoin = new List<MJoin>();
        private HttpRequest _request;


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Public methods
         */

        /// <summary>
        /// Get the response object that has been created by this instance. This
        /// is only useful after <code>process()</code> has been called.
        /// </summary>
        /// <returns>The response object as populated by this instance</returns>
        public DtResponse Data()
        {
            return _Out;
        }

        /// <summary>
        /// Get the database instance used by this instance
        /// </summary>
        /// <returns>Database connection instance</returns>
        public Database Db()
        {
            return _Db;
        }

        /// <summary>
        /// Set the database connection instance
        /// </summary>
        /// <param name="db">Connection instance to set</param>
        /// <returns>Self for chaining</returns>
        public Editor Db(Database db)
        {
            _Db = db;
            return this;
        }

        /// <summary>
        /// Get the fields that have been configured for this instance
        /// </summary>
        /// <returns>List of fields</returns>
        public List<Field> Field()
        {
            return _Field;
        }

        /// <summary>
        /// Add a new field to this instance
        /// </summary>
        /// <param name="f">New field to add</param>
        /// <returns>Self for chaining</returns>
        public Editor Field(Field f)
        {
            _Field.Add(f);
            return this;
        }

        /// <summary>
        /// Add multiple fields too this instance
        /// </summary>
        /// <param name="fields">Collection of fields to add</param>
        /// <returns>Self for chaining</returns>
        public Editor Field(IEnumerable<Field> fields)
        {
            foreach (Field f in fields)
            {
                _Field.Add(f);
            }
            return this;
        }

        /// <summary>
        /// Get the DOM prefix.
        /// 
        /// Typically primary keys are numeric and this is not a valid ID value in an
        /// HTML document - is also increases the likelihood of an ID clash if multiple
        /// tables are used on a single page. As such, a prefix is assigned to the 
        /// primary key value for each row, and this is used as the DOM ID, so Editor
        /// can track individual rows.
        /// </summary>
        /// <returns>DOM prefix</returns>
        public string IdPrefix()
        {
            return _IdPrefix;
        }

        /// <summary>
        /// Set the DOM prefix.
        /// 
        /// Typically primary keys are numeric and this is not a valid ID value in an
        /// HTML document - is also increases the likelihood of an ID clash if multiple
        /// tables are used on a single page. As such, a prefix is assigned to the 
        /// primary key value for each row, and this is used as the DOM ID, so Editor
        /// can track individual rows.
        /// </summary>
        /// <param name="prefix">Prefix to set</param>
        /// <returns>Self for chaining</returns>
        public Editor IdPrefix(string prefix)
        {
            _IdPrefix = prefix;
            return this;
        }

        /// <summary>
        /// Get the data that is being processed by the Editor instance. This is only
        /// useful once the <code>Process()</code> method has been called, and
        /// is available for use in validation and formatter methods.
        /// </summary>
        /// <returns>Data given to <code>Process()</code></returns>
        public DtRequest InData()
        {
            return _ProcessData;
        }

        /// <summary>
        /// Add a left join condition to the Editor instance, allowing it to operate
        /// over multiple tables. Multiple <code>leftJoin()</code> calls can be made for a
        /// single Editor instance to join multiple tables.
        ///
        /// A left join is the most common type of join that is used with Editor
        /// so this method is provided to make its use very easy to configure. Its
        /// parameters are basically the same as writing an SQL left join statement,
        /// but in this case Editor will handle the create, update and remove
        /// requirements of the join for you:
        ///
        /// * Create - On create Editor will insert the data into the primary table
        ///   and then into the joined tables - selecting the required data for each
        ///   table.
        /// * Edit - On edit Editor will update the main table, and then either
        ///   update the existing rows in the joined table that match the join and
        ///   edit conditions, or insert a new row into the joined table if required.
        /// * Remove - On delete Editor will remove the main row and then loop over
        ///   each of the joined tables and remove the joined data matching the join
        ///   link from the main table.
        ///
        /// Please note that when using join tables, Editor requires that you fully
        /// qualify each field with the field's table name. SQL can result table
        /// names for ambiguous field names, but for Editor to provide its full CRUD
        /// options, the table name must also be given. For example the field
        /// <code>first_name</code> in the table <code>users</code> would be given
        /// as <code>users.first_name</code>.
        /// </summary>
        /// <param name="table">Table name to do a join onto</param>
        /// <param name="field1">Field from the parent table to use as the join link</param>
        /// <param name="op">Join condition (`=`, '<`, etc)</param>
        /// <param name="field2">Field from the child table to use as the join link</param>
        /// <returns>Self for chaining</returns>
        public Editor LeftJoin(string table, string field1, string op, string field2)
        {
            _LeftJoin.Add(new LeftJoin(table, field1, op, field2));

            return this;
        }

        /// <summary>
        /// Add a 1-to-many ("mjoin") join to the Editor instance. The way the
        /// join operates is defined by the MJoin class
        /// </summary>
        /// <param name="join">MJoin link to use</param>
        /// <returns></returns>
        public Editor MJoin(MJoin join)
        {
            _MJoin.Add(join);

            return this;
        }

        /// <summary>
        /// Set a model to use.
        ///
        /// In keeping with the MVC style of coding, you can define the fields
        /// and their types that you wish to get from the database in a simple
        /// class. Editor will automatically add fields from the model.
        ///
        /// Note that fields that are defined in the model can also be defined
        /// as <code>Field</code> instances should you wish to add additional
        /// options to a specific field such as formatters or validation.
        /// </summary>
        /// <typeparam name="T">Model to use</typeparam>
        /// <returns>Self for chaining</returns>
        public Editor Model<T>()
        {
            _UserModelT = typeof(T);

            return this;
        }


        /// <summary>
        /// Get the database table name this Editor instance will use
        /// </summary>
        /// <returns>Table name</returns>
        public List<string> Table()
        {
            return _Table;
        }

        /// <summary>
        /// Set the database table name this Editor instance will use
        /// </summary>
        /// <param name="t">Table name</param>
        /// <returns>Self for chaining</returns>
        public Editor Table(string t)
        {
            _Table.Add(t);
            return this;
        }

        /// <summary>
        /// Add multiple tables to the Editor instance
        /// </summary>
        /// <param name="tables">Collection of tables to add</param>
        /// <returns>Self for chaining</returns>
        public Editor Table(IEnumerable<string> tables)
        {
            foreach (string t in tables)
            {
                _Table.Add(t);
            }
            return this;
        }

        /// <summary>
        /// Get the transaction state for this instance.
        /// 
        /// When enabled (which it is by default) Editor will use an SQL transaction
        /// to ensure data integrity while it is performing operations on the table.
        /// This can be optionally disabled using this method, if required by your
        /// database configuration.
        /// </summary>
        /// <returns></returns>
        public bool Transaction()
        {
            return _Transaction;
        }

        /// <summary>
        /// Set the transaction state for this instance.
        /// </summary>
        /// <param name="set">Value to set - true to enable transactions, false to disabled.</param>
        /// <returns></returns>
        public Editor Transaction(bool set)
        {
            _Transaction = set;
            return this;
        }

        /// <summary>
        /// Get the primary key field that has been configured.
        /// 
        /// The primary key must be known to Editor so it will know which rows are being
        /// edited / deleted upon those actions. The default value is 'id'.
        /// </summary>
        /// <returns>Primary key</returns>
        public string Pkey()
        {
            return _Pkey;
        }

        /// <summary>
        /// Set the primary key field to use. Please note that at this time
        /// Editor does not support composite primary keys in a table, only a
        /// single field primary key is supported.
        /// 
        /// The primary key must be known to Editor so it will know which rows are being
        /// edited / deleted upon those actions. The default value is 'id'.
        /// </summary>
        /// <param name="id">Primary key column name</param>
        /// <returns>Self for chaining</returns>
        public Editor Pkey(string id)
        {
            _Pkey = id;
            return this;
        }

        /// <summary>
        /// Process a request from the Editor client-side to get / set data.
        /// </summary>
        /// <param name="data">Data sent from the client-side</param>
        /// <returns>Self for chaining</returns>
        public Editor Process(DtRequest data)
        {
            try
            {
                _ProcessData = data;
                _FormData = data.Data ?? null;

                if (_Transaction)
                {
                    _Db.Transaction();
                }

                _PrepJoin();
                _PrepModel();

                if (data.RequestType == DtRequest.RequestTypes.DataTablesGet ||
                    data.RequestType == DtRequest.RequestTypes.DataTablesSsp)
                {
                    // DataTable get request
                    _Out.Merge(_Get(null, data));
                }
                else if (data.RequestType == DtRequest.RequestTypes.EditorUpload)
                {
                    // File upload
                    _Upload(data);
                }
                else if (data.RequestType == DtRequest.RequestTypes.EditorRemove)
                {
                    // Remove rows
                    _Remove(data);
                }
                else
                {
                    // Create or edit
                    bool valid = Validate(_Out, data);

                    // Global validation - if you want global validation - do it here
                    // _Out.Error = "";

                    if (valid)
                    {
                        _Out.row = data.RequestType == DtRequest.RequestTypes.EditorCreate ?
                            _Insert() :
                            _Update(data.Ids);
                    }
                }

                if (_Transaction)
                {
                    _Db.Commit();
                }
            }
            catch (Exception e)
            {
                _Out.error = e.Message;

                if (_Transaction)
                {
                    _Db.Rollback();
                }
            }

            return this;
        }

        /// <summary>
        /// Process a request from the Editor client-side to get / set data.
        /// For use with WebAPI's 'FormDataCollection' collection
        /// </summary>
        /// <param name="data">Data sent from the client-side</param>
        /// <returns>Self for chaining</returns>
        public Editor Process(IEnumerable<KeyValuePair<string, string>> data = null)
        {
            return Process(new DtRequest(data));
        }

        /// <summary>
        /// Process a request from the Editor client-side to get / set data.
        /// For use with MVC's 'Request.Form' collection
        /// </summary>
        /// <param name="data">Data sent from the client-side</param>
        /// <returns>Self for chaining</returns>
        public Editor Process(NameValueCollection data = null)
        {
            var list = new List<KeyValuePair<string, string>>();

            if (data != null)
            {
                foreach (var key in data.AllKeys)
                {
                    list.Add(new KeyValuePair<string, string>(key, data[key]));
                }
            }

            return Process(new DtRequest(list));
        }

        /// <summary>
        /// Process a request from the Editor client-side to get / set data.
        /// For use with an HttpRequest object
        /// </summary>
        /// <param name="data">Data sent from the client-side</param>
        /// <returns>Self for chaining</returns>
        public Editor Process(HttpRequest request)
        {
            _request = request;

            return Process(request.Form);
        }

        /// <summary>
        /// Perform validation on a data set.
        ///
        /// Note that validation is performed on data only when the action is
        /// 'create' or 'edit'. Additionally, validation is performed on the _wire
        /// data_ - i.e. that which is submitted from the client, without formatting.
        /// Any formatting required by <code>setFormatter</code> is performed after
        /// the data from the client has been validated.
        /// </summary>
        /// <param name="response">DataTables response object</param>
        /// <param name="request">DataTables request object</param>
        /// <returns>`true` if the data is valid, `false` if not.</returns>
        public bool Validate(DtResponse response, DtRequest request)
        {
            // Validation is only performed on create and edit
            if (request.RequestType != DtRequest.RequestTypes.EditorCreate &&
                request.RequestType != DtRequest.RequestTypes.EditorEdit)
            {
                return true;
            }

            foreach (var field in _Field)
            {
                var validation = field.Validate(request.Data, this);

                if (validation != null)
                {
                    response.fieldErrors.Add(new DtResponse.FieldError()
                    {
                        name = field.Name(),
                        status = validation
                    });
                }
            }

            // MJoin validation
            foreach (var mjoin in _MJoin)
            {
                mjoin.Validate(response, this, request.Data);
            }

            return !response.fieldErrors.Any();
        }

        /// <summary>
        /// Where condition to add to the query used to get data from the database.
        /// Multiple conditions can be added if required.
        /// 
        /// Can be used in two different ways:
        /// 
        /// * Simple case: `where( field, value, operator )`
        /// * Complex: `where( fn )`
        ///
        /// The simple case is fairly self explanatory, a condition is applied to the
        /// data that looks like `field operator value` (e.g. `name = 'Allan'`). The
        /// complex case allows full control over the query conditions by providing a
        /// closure function that has access to the database Query that Editor is
        /// using, so you can use the `where()`, `or_where()`, `and_where()` and
        /// `where_group()` methods as you require.
        ///
        /// Please be very careful when using this method! If an edit made by a user
        /// using Editor removes the row from the where condition, the result is
        /// undefined (since Editor expects the row to still be available, but the
        /// condition removes it from the result set).
        /// </summary>
        /// <param name="fn">Delegate to execute adding where conditions to the table</param>
        /// <returns>Self for chaining</returns>
        public Editor Where(Action<Query> fn)
        {
            _Where.Add(new WhereCondition
            {
                Custom = fn
            });

            return this;
        }

        /// <summary>
        /// Where condition to add to the query used to get data from the database.
        /// Multiple conditions can be added if required.
        /// 
        /// Can be used in two different ways:
        /// 
        /// * Simple case: `where( field, value, operator )`
        /// * Complex: `where( fn )`
        ///
        /// The simple case is fairly self explanatory, a condition is applied to the
        /// data that looks like `field operator value` (e.g. `name = 'Allan'`). The
        /// complex case allows full control over the query conditions by providing a
        /// closure function that has access to the database Query that Editor is
        /// using, so you can use the `where()`, `or_where()`, `and_where()` and
        /// `where_group()` methods as you require.
        ///
        /// Please be very careful when using this method! If an edit made by a user
        /// using Editor removes the row from the where condition, the result is
        /// undefined (since Editor expects the row to still be available, but the
        /// condition removes it from the result set).
        /// </summary>
        /// <param name="key">Database column name to perform the condition on</param>
        /// <param name="value">Value to use for the condition</param>
        /// <param name="op">Conditional operator</param>
        /// <returns>Self for chaining</returns>
        public Editor Where(string key, object value, string op = "=")
        {
            _Where.Add(new WhereCondition
            {
                Key = key,
                Value = value,
                Operator = op
            });

            return this;
        }


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Internal methods
         */

        /// <summary>
        /// Get the request object used for this instance
        /// </summary>
        /// <returns>HTTP request object</returns>
        internal HttpRequest Request()
        {
            return _request;
        }


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         * Private methods
         */

        private void _PrepModel()
        {
            // Add fields which are defined in the model, but not in the _Field list
            if (_UserModelT != null)
            {
                _FieldFromModel(_UserModelT);
            }
        }

        private void _FieldFromModel(Type model, string parent = "")
        {
            // Add the properties
            foreach (var pi in model.GetProperties())
            {
                var field = _FindField(parent + pi.Name, "name");

                // If the field doesn't exist yet, create it
                if (field == null)
                {
                    field = new Field(parent + pi.Name);
                    Field(field);
                }

                // Then assign the information from the model
                field.Type(pi.PropertyType);

                if (pi.IsDefined(typeof(EditorTypeErrorAttribute)) == true)
                {
                    var err = pi.GetCustomAttribute<EditorTypeErrorAttribute>();
                    field.TypeError(err.Msg);
                }

                if (pi.IsDefined(typeof(EditorHttpNameAttribute)) == true)
                {
                    var name = pi.GetCustomAttribute<EditorHttpNameAttribute>();
                    field.Name(name.Name);
                }
            }

            // Add any nested classes and their properties
            var nested = model.GetNestedTypes(BindingFlags.Public | BindingFlags.Instance);

            foreach (var t in nested)
            {
                _FieldFromModel(t, parent + t.Name + ".");
            }
        }



        private DtResponse _Get(object id = null, DtRequest http = null)
        {
            Query query = _Db
                .Query("select")
                .Table(_Table)
                .Get(_Pkey);

            // Add all fields that we need to get from the database
            foreach (Field field in _Field)
            {
                if (field.Apply("get") && field.GetValue() == null)
                {
                    query.Get(field.DbField());
                }
            }

            _GetWhere(query);
            _PerformLeftJoin(query);
            DtResponse ssp = _SspQuery(query, http);

            if (id != null)
            {
                query.Where(_Pkey, id);
            }

            Result res = query.Exec();
            Dictionary<string, object> row;
            DtResponse dtData = new DtResponse();

            while ((row = res.Fetch()) != null)
            {
                var inner = new Dictionary<string, object>();
                inner.Add("DT_RowId", _IdPrefix + row[_Pkey]);

                foreach (Field field in _Field)
                {
                    if (field.Apply("get"))
                    {
                        field.Write(inner, row);
                    }
                }

                dtData.data.Add(inner);
            }

            // Field options
            foreach (Field field in _Field)
            {
                List<Dictionary<string, object>> opts = field.OptionsExec(_Db);

                if (opts != null)
                {
                    dtData.options.Add(field.Name(), opts);
                }
            }

            // Row based joins
            foreach (var mjoin in _MJoin)
            {
                mjoin.Data(this, dtData);
            }

            return dtData.Merge(ssp);
        }


        private Dictionary<string, object> _Insert()
        {
            // Insert the new row
            object id = _InsertOrUpdate(null);

            // Was the primary key sent? Unusual, but it is possible
            if (_FormData.ContainsKey(_Pkey))
            {
                id = _FormData[_Pkey];
            }

            // Row based joins
            foreach (var mjoin in _MJoin)
            {
                mjoin.Insert(this, id, _FormData);
            }

            // Full data set for the created row
            DtResponse row = _Get(id);
            return row.data.Count() > 0 ?
                row.data[0] :
                null;
        }


        private Dictionary<string, object> _Update(List<string> ids)
        {
            // Only single row editing (at the moment)
            string id = ids[0].Replace(_IdPrefix, "");

            // Update or insert the rows for the parent table and the left joined
            // tables
            _InsertOrUpdate(id);

            // Row based joins
            foreach (var mjoin in _MJoin)
            {
                mjoin.Update(this, id, _FormData);
            }

            // Was the primary key altered as part of the edit? Unusual, but it is
            // possible
            object getId = _FormData.ContainsKey(_Pkey) ?
                _FormData[_Pkey] :
                id;

            // Full data set for the modified row
            DtResponse row = _Get(id);
            return row.data.Count() > 0 ?
                row.data[0] :
                null;
        }


        private void _Remove(DtRequest data)
        {
            List<string> ids = new List<string>();

            if (data.Ids.Count() == 0)
            {
                throw new Exception("No ids submitted for the delete");
            }

            // Strip the ID prefix that the client-side sends back
            for (int i = 0, ien = data.Ids.Count(); i < ien; i++)
            {
                ids.Add(data.Ids[i].Replace(_IdPrefix, ""));
            }

            // Row based joins - remove first as the host row will be removed which is
            // a dependency
            foreach (var mjoin in _MJoin)
            {
                mjoin.Remove(this, ids);
            }

            // Remove from the left join tables
            for (int i = 0, ien = _LeftJoin.Count(); i < ien; i++)
            {
                LeftJoin join = _LeftJoin[i];
                string table = _Alias(join.Table, "orig");
                string parentLink;
                string childLink;


                // which side of the join refers to the parent table?
                if (join.Field1.IndexOf(join.Table) == 0)
                {
                    parentLink = join.Field2;
                    childLink = join.Field1;
                }
                else
                {
                    parentLink = join.Field1;
                    childLink = join.Field2;
                }

                // Only delete on the primary key, since that is what the ids refer
                // to - otherwise we'd be deleting on random data!
                if (parentLink == _Pkey)
                {
                    _RemoveTable(join.Table, ids, childLink);
                }
            }

            // Remove from the primary tables
            for (int i = 0, ien = _Table.Count(); i < ien; i++)
            {
                _RemoveTable(_Table[i], ids);
            }
        }


        private void _Upload(DtRequest data)
        {
            if (_request == null)
            {
                throw new Exception("File upload requires that 'Process' be called with an HttpRequest object");
            }

            var field = _FindField(data.UploadField, "name");
            if (field == null)
            {
                throw new Exception("Unknown upload field name submitted");
            }

            var upload = field.Upload();
            if (upload == null)
            {
                throw new Exception("File uploaded to a field that does not have upload options configured");
            }

            object res = upload.Exec(this);

            if (res is Boolean && (Boolean)res == false)
            {
                _Out.fieldErrors.Add(new DtResponse.FieldError()
                {
                    name = field.Name(),
                    status = upload.Error()
                });
            }
            else
            {
                var row = new Dictionary<string, object>();

                _Out.upload.id = res;
                _Out.upload.row = row;

                var table = upload.Table();
                if (table == null)
                {
                    return;
                }

                var q = Db()
                    .Query("select", table)
                    .Where(upload.Pkey(), res);

                foreach (var getField in _Field)
                {
                    if (getField.Apply("get") && _Part(getField.DbField()) == table)
                    {
                        q.Get(getField.DbField());
                    }
                }

                var dbRow = q.Exec().Fetch();

                foreach (var getField in _Field)
                {
                    if (getField.Apply("get") && _Part(getField.DbField()) == table)
                    {
                        getField.Write(row, dbRow);
                    }
                }
            }
        }


        private void _RemoveTable(string table, IEnumerable<string> ids, string pkey = null)
        {
            if (pkey == null)
            {
                pkey = _Pkey;
            }

            // Check that there is a field which has a set option for this table
            var count = _Field.Count(
                field => !field.DbField().Contains(".") || (
                    _Part(field.DbField(), "table") == table &&
                    field.Set() != DataTables.Field.SetType.None
                )
            );

            if (count > 0)
            {
                _Db.Query("delete")
                    .Table(table)
                    .OrWhere(pkey, ids)
                    .Exec();
            }
        }


        private void _PrepJoin()
        {
            if (!_LeftJoin.Any())
            {
                return;
            }

            // Check if the primary key has a table identifier - if not, add one
            if (!_Pkey.Contains('.'))
            {
                _Pkey = _Alias(_Table[0], "alias") + '.' + _Pkey;
            }

            // Check that all the fields have a table selector, orhterwise, we'd need to
            // know the structure of the tables, to know which fields belong in
            // which. This extra requirement on the fields removed that
            foreach (var field in _Field)
            {
                var name = field.DbField();

                if (!name.Contains('.'))
                {
                    throw new Exception("Table part of the field '" + name + "' was not found. " +
                        "In Editor instance that use a join, all the fields must have the " +
                        "database table set explicity."
                    );
                }
            }
        }

        private Field _FindField(string name, string type)
        {
            for (int i = 0, ien = _Field.Count(); i < ien; i++)
            {
                Field field = _Field[i];

                if (type == "name" && field.Name() == name)
                {
                    return field;
                }
                else if (type == "db" && field.DbField() == name)
                {
                    return field;
                }
            }

            return null;
        }

        private void _GetWhere(Query query)
        {
            foreach (var where in _Where)
            {
                if (where.Custom != null)
                {
                    where.Custom(query);
                }
                else
                {
                    query.Where(where.Key, where.Value, where.Operator);
                }
            }
        }


        private DtResponse _SspQuery(Query query, DtRequest http)
        {
            DtResponse ssp = new DtResponse();

            if (http == null || http.RequestType == DtRequest.RequestTypes.DataTablesGet)
            {
                return ssp;
            }

            // Add the server-side processing conditions
            _SspLimit(query, http);
            _SspSort(query, http);
            _SspFilter(query, http);

            // Get the nuber of rows in the result set
            Query setCount = _Db
                .Query("select")
                .Table(_Table)
                .Get("COUNT( " + _Pkey + " ) as cnt");
            _GetWhere(setCount);
            _SspFilter(setCount, http);
            _PerformLeftJoin(setCount);

            int setCounted = Convert.ToInt32(setCount.Exec().Fetch()["cnt"]);

            // Get the number of rows in the full set
            Query fullCount = _Db
                .Query("select")
                .Table(_Table)
                .Get("COUNT( " + _Pkey + " ) as cnt");
            _GetWhere(fullCount);

            // A left join is only needed if there is a where condition, incase the
            // conditional items are the ones being joined in
            if (_Where.Count() > 0)
            {
                _PerformLeftJoin(fullCount);
            }

            int fullCounted = Convert.ToInt32(fullCount.Exec().Fetch()["cnt"]);

            ssp.draw = http.Draw;
            ssp.recordsTotal = fullCounted;
            ssp.recordsFiltered = setCounted;

            return ssp;
        }

        private string _SspField(DtRequest http, int index)
        {
            string name = http.Columns[index].Data;
            Field field = _FindField(name, "name");


            if (field == null)
            {
                // Is it the primary key?
                if (name == "DT_RowId")
                {
                    return _Pkey;
                }

                throw new Exception("Unknown field: " + name + " (index " + index + ")");
            }

            return field.DbField();
        }

        private void _SspSort(Query query, DtRequest http)
        {
            for (int i = 0, ien = http.Order.Count(); i < ien; i++)
            {
                var order = http.Order[i];

                query.Order(
                    _SspField(http, order.Column) + " " +
                    (order.Dir == "asc" ? "asc" : "desc")
                );
            }
        }

        private void _SspFilter(Query query, DtRequest http)
        {
            // Global search, add a ( ... or ... ) set of filters for each column
            // in the table (not the fields, just the columns submitted)
            if (http.Search.Value != "")
            {
                query.Where(delegate(Query q)
                {
                    for (int i = 0, ien = http.Columns.Count(); i < ien; i++)
                    {
                        if (http.Columns[i].Searchable)
                        {
                            string field = _SspField(http, i);

                            if (field != null)
                            {
                                q.OrWhere(field, "%" + http.Search.Value + "%", "like");
                            }
                        }
                    }
                });
            }

            // Column filters
            for (int i = 0, ien = http.Columns.Count(); i < ien; i++)
            {
                var column = http.Columns[i];
                string search = column.Search.Value;

                if (search != "" && column.Searchable)
                {
                    query.Where(_SspField(http, i), "%" + search + "%", "like");
                }
            }
        }

        private void _SspLimit(Query query, DtRequest http)
        {
            // -1 is "show all" in DataTables, so there would be no limit at that point
            if (http.Length != -1)
            {
                query
                    .Offset(http.Start)
                    .Limit(http.Length);
            }
        }



        private object _InsertOrUpdate(object id)
        {
            // Loop over all tables in _Table, doing the insert of update as needed
            for (int i = 0, ien = _Table.Count(); i < ien; i++)
            {
                Result res = _InsertOrUpdateTable(
                    _Table[i],
                    id == null ?
                    null :
                    new Dictionary<string, object>()
                    {
                        { _Pkey, id }
                    }
                );

                // If we don't have an id yet, then the first insert will return
                // the id we want.
                if (id == null)
                {
                    id = res.InsertId();
                }
            }

            string parentLink;
            string childLink;
            Dictionary<string, object> where;

            // And for the left join tables as well
            foreach (LeftJoin join in _LeftJoin)
            {
                where = new Dictionary<string, object>();

                // Which side of the join refers to the parent table?
                string joinTable = _Alias(join.Table, "alias");
                string tablePart = _Part(join.Field1);

                if (_Part(join.Field1, "db") != null)
                {
                    tablePart = _Part(join.Field1, "db") + "." + tablePart;
                }

                if (tablePart == joinTable)
                {
                    parentLink = join.Field2;
                    childLink = join.Field1;
                }
                else
                {
                    parentLink = join.Field1;
                    childLink = join.Field2;
                }

                if (parentLink == _Pkey)
                {
                    where.Add(childLink, id);
                }
                else
                {
                    Field field = _FindField(parentLink, "db");

                    if (field == null)
                    {
                        // The parent field wasn't included in the field list, so
                        // nothing can be done with it
                        continue;
                    }

                    where.Add(childLink, field.Val("set", _FormData));
                }

                _InsertOrUpdateTable(join.Table, where);
            }

            return id;
        }

        private Result _InsertOrUpdateTable(string table, Dictionary<string, object> where)
        {
            Dictionary<string, object> set = new Dictionary<string, object>();
            string action = where == null ? "create" : "edit";
            string tableName = _Alias(table, "orig");
            string tableAlias = _Alias(table, "alias");

            foreach (Field field in _Field)
            {
                string tablePart = _Part(field.DbField());

                if (_Part(field.DbField(), "db") != null)
                {
                    tablePart = _Part(field.DbField(), "db") + "." + tablePart;
                }

                // Does this field apply to this table (only check when a join is
                // being used)
                if (_LeftJoin.Count() > 0 && tablePart != tableAlias)
                {
                    continue;
                }

                // /Check if this field should be set, based on the options and
                // submitted data
                if (!field.Apply(action, _FormData))
                {
                    continue;
                }

                // Some db's (specifically postgres) don't like having the table
                // name prefixing the column name.
                string fieldPart = _Part(field.DbField(), "field");
                set.Add(fieldPart, field.Val("set", _FormData));
            }

            // If nothing to do, then do nothing!
            if (set.Count() == 0)
            {
                return null;
            }

            // Insert or update
            if (action == "create")
            {
                return _Db.Insert(table, set);
            }
            else
            {
                return _Db.Push(table, set, where);
            }
        }

        private void _PerformLeftJoin(Query q)
        {
            if (_LeftJoin.Count() != 0)
            {
                foreach (LeftJoin join in _LeftJoin)
                {
                    q.Join(join.Table, join.Field1 + " " + join.Operator + " " + join.Field2, "LEFT");
                }
            }
        }



        private string _Alias(string name, string type = "alias")
        {
            if (name.IndexOf(" as ", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                string[] a = Regex.Split(name, " as ", RegexOptions.IgnoreCase);
                return type == "alias" ?
                    a[1] :
                    a[0];
            }

            return name;

        }

        private string _Part(string name, string type = "table")
        {
            string db = null;
            string table = null;
            string column = null;

            if (name.Contains("."))
            {
                string[] a = name.Split('.');

                if (a.Count() == 3)
                {
                    db = a[0];
                    table = a[1];
                    column = a[2];
                }
                else if (a.Count() == 2)
                {
                    table = a[0];
                    column = a[1];
                }
            }
            else
            {
                column = name;
            }

            if (type == "db")
            {
                return db;
            }
            else if (type == "table")
            {
                return table;
            }
            return column;
        }
    }
}
