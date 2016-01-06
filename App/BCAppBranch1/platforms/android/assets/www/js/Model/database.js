//本地数据库处理
/**
 *数据库操作辅助类,定义对象、数据操作方法都在这里定义
 */
var DataBase = function() {
	this.options = {
		dbname: 'bcdb',
		version: '1.0',
		dbdesc: 'bcdb',
		dbsize: 30000,
		db: null,
		table_CurrentUser: 'tb_CurrentUser',
		CurrentUser: ['ID', 'UserKey', 'UserID'],
		table_UserMessage: 'tb_UserMessage',
		UserMessage: ['MessageID', 'UserID', 'Message', 'SendTime'],
		table_FrontUsers: 'tb_FrontUsers',
		FrontUsers: ['UserID', 'Name', 'EnterpriseID', 'RegistDate', 'LastDate', 'lastIP', 'UpdateTime', 'LoginByDesktop', 'LoginByMobile', 'HeadPicture', 'DepartmentID'],
		table_Departments: 'tb_Departments',
		Departments: ['DepartmentID', 'Name', 'EnterpriseID', 'Description', 'Available'],
		table_Projects: 'tb_Projects',
		Projects: ['ProjectID', 'Name', 'EnterPriseID', 'RegistDate', 'BeginDate', 'EndDate', 'Status'],
		table_Scenes: 'tb_Scenes',
		Scenes: ['SceneID', 'Name', 'EnterpriseID', 'ProjectID', 'Worker', 'RegistDate', 'BeginDate', 'EndDate', 'Status'],
		table_SceneMessage: 'tb_SceneMessage',
		SceneMessage: ['MessageID', 'SceneID', 'Message', 'UpdateDate', 'UserID', 'Status', 'BeginDate', 'EndDate'],
		table_Materials: 'tb_Materials',
		Materials: ['MatterialID', 'Name', 'UpdateDate', 'UserID', 'EnterpriseID', 'Download', 'Address'],
		table_UserRoles: 'tb_UserRoles',
		UserRoles: ['UserID', 'RoleID', 'UpdateTime']
	}
}
DataBase.prototype = {
	OpenDB: function(callback) {
		try {
			if (!window.openDatabase) {
				alert('该浏览器不支持数据库');
				return false;
			}
			this.options.db = window.openDatabase(this.options.dbname, this.options.version, this.options.dbdesc, this.options.dbsize);
			return true;
		} catch (e) {
			if (e == 2) {
				alert("数据库版本无效");
			} else {
				alert("未知错误 " + e + ".");
			}
			return false;
		}
	},
	ExecSql: function(sql, param, callback) {
		if (this.options.db == null) {
			this.OpenDB();
		}
		this.options.db.transaction(function(tx) {
			tx.executeSql(sql, param, function(tx, result) {
				if (typeof(callback) == 'function') {
					callback(true)
				}
				return true;
			}, function(tx, error) {
				if (typeof(callback) == 'function') {
					callback(false)
				}
				alert(error.message);
				return false;
			});
		});
	},
	InitDB: function() {
		if (this.options.db == null) {
			this.OpenDB();
		}
		this.CreateTable(this.options.table_CurrentUser, this.options.CurrentUser, {
			"ID": "INTEGER primary key ",
			"UserKey": "",
			"app_flow_no": "not null"
		});
		this.CreateTable(this.options.table_UserMessage, this.options.UserMessage, {
			"MessageID": "INTEGER primary key",
			"app_flow_no": "not null"
		});
		/*this.CreateTable(this.options.table_project, this.options.Project, {
			"id": "INTEGER  primary key",
			"app_flow_no": "not null"
		});
		this.CreateTable(this.options.table_field, this.options.Field, {
			"id": "INTEGER primary key",
			"app_flow_no": "not null"
		});
		this.CreateTable(this.options.table_space, this.options.Space, {
			"id": "INTEGER primary key",
			"app_flow_no": "not null"
		});
		this.CreateTable(this.options.table_space, this.options.Space, {
			"id": "INTEGER primary key",
			"app_flow_no": "not null"
		});*/
	},
	CreateTable: function(tableName, fields, constraint) {

		if (this.options.db == null) {
			this.OpenDB();
		}
		var sql = 'CREATE TABLE IF NOT EXISTS ' + tableName + ' (';
		for (i in fields) {
			var key = "";
			if (typeof(constraint) != "undefined" && typeof(constraint[fields[i]]) != "undefined") {
				key = " " + constraint[fields[i]];
			}
			sql += fields[i] + key + ",";
		}
		sql = sql.substr(0, sql.length - 1);
		sql += ")";
		//log(sql);
		this.ExecSql(sql);
	},
	UpdateTable: function(tableName, setFields, setParams, whereStr, wherParams, callback) {
		var sql = "update " + tableName + " set ";
		for (i in setFields) {
			sql += setFields[i] + "=?,";
		}
		sql = sql.substr(0, sql.length - 1);
		if (typeof(whereStr) != "undefined" && typeof(wherParams) != "undefined" && whereStr != "") {
			sql += " where " + whereStr;
			setParams = setParams.concat(wherParams);
		}
		this.ExecSql(sql, setParams, callback);
	},
	InsertTable: function(tableName, insertFields, insertParams, callback) {
		var sql = "insert into " + tableName + " (";
		var sql2 = " values(";
		for (i in insertFields) {
			sql += insertFields[i] + ",";
			sql2 += "?,"
		}
		sql = sql.substr(0, sql.length - 1);
		sql2 = sql2.substr(0, sql2.length - 1);
		sql += ")";
		sql2 += ")";
		if (typeof(callback) == 'function')
			this.ExecSql(sql + sql2, insertParams, callback);
		else
			this.ExecSql(sql + sql2, insertParams);
	},
	DeleteTable: function(tableName, whereStr, wherParams) {
		var sql = "delete from " + tableName;
		if (typeof(whereStr) != "undefined" && typeof(wherParams) != "undefined" && whereStr != "") {
			sql += " where " + whereStr;
		}
		this.ExecSql(sql, wherParams);
	},
	DropTable: function(tableName, callback) {
		var sql = 'DROP TABLE IF EXISTS ' + tableName;
		if (typeof(whereStr) != "undefined" && typeof(wherParams) != "undefined" && whereStr != "") {
			sql += " where " + whereStr;
		}
		this.ExecSql(sql, [], callback);
	},
	SelectTable: function(tableName, selectFields, whereStr, wherParams, callback) {
		if (this.options.db == null) {
			this.OpenDB();
		}
		var sql = "SELECT " + selectFields + " FROM " + tableName;
		if (typeof(whereStr) != "undefined" && typeof(wherParams) != "undefined" && whereStr != "") {
			sql += " where " + whereStr;
		}
		this.options.db.transaction(function(tx) {
			tx.executeSql(sql, wherParams, function(tx, results) {
				if (results.rows.length < 1) {
					if (typeof(callback) == 'function') {
						callback(false)
					}
				} else {
					if (typeof(callback) == 'function') {
						callback(results.rows)
					}
				}
			}, function(tx, error) {
				return false;
			});
		});
	},
	SaveOrUpdateTable: function(tableName, insertFields, insertParams, key, keyVal, callback) {
		var me = this;
		if (typeof(key) != "undefined" && typeof(keyVal) != "undefined" && key != "") {
			me.SelectTable(tableName, insertFields[0], key + "=?", [keyVal], function(rows) {
				if (rows) {
					me.UpdateTable(tableName, insertFields, insertParams, key + "=?", [keyVal], callback);
				} else {
					insertFields.push(key);
					insertParams.push(keyVal);
					me.InsertTable(tableName, insertFields, insertParams, callback);
				}
			})
		} else {
			me.InsertTable(tableName, insertFields, insertParams, callback);
		}
	}
}