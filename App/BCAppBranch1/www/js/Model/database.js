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
		table_UserMessage: 'tb_UserMessage',
		//增加type区分公告与一般聊天
		UserMessage: ['MessageID','SendUserID', 'Message', 'SendTime', 'Recipients','SendUserName','SendUserPicture','HeadPictureURI','Type','EnterpriseID','Time','Status'],
		//登陆用户表
		table_CurrentUsers: 'tb_CurrentUsers',
		CurrentUsers: ['UserID', 'UserName', 'NickName','RoleIDs','EnterpriseID', 'EnterpriseName','DepartmentID', 'HeadImage', 'Token','Sign','FunctionIDs'],
		//企业人员表
		table_FrontUsers: 'tb_FrontUsers',
		FrontUsers: ['UserID','UserName','DepartmentID','EnterpriseID'],
		
		//人员角色关系表：RoleName为角色名
		table_UserRoles: 'tb_UserRoles',
		UserRoles: ['UserID','RoleID','RoleName','EnterpriseID'],
		
		//部门表：ParentID为父级部门ID;
		table_Departments: 'tb_Departments',
		Departments: ['DepartmentID', 'DepartmentName','ParentID','EnterpriseID'],
		//项目表
		table_Projects: 'tb_Projects',
		//Roles项目角色,Departments设计部门ID
		Projects: ['ProjectID', 'ProjectName','Departments','HaveScene','ProjectRoles','ProjectState','Manager','EnterpriseID'],
		//现场表:Type为现场类型
		table_Scenes: 'tb_Scenes',
		Scenes: ['SceneID', 'ParentID', 'SceneName','ProjectID', 'SceneWorker', 'SceneState','SceneType','Address','BeginDate','EndDate','SendStatus','HasData','AllWorkers'],
		//资料表
		table_Materials: 'tb_Materials',
		//Type资料类型（word、excel、pdf）为显示资料图片
		Materials: ['MaterialID', 'MaterialName', 'Download','Type','EnterpriseID'],
		//现场信息表
		table_SceneMessageComments: 'tb_SceneMessageComments',
		SceneMessageComments: ['MessageID', 'SceneID', 'UserID', 'UserPicture', 'UserPictureURI', 'UserName', 'Address', 'CreateTime', 'Description', 'Images','Comments','Status','Type','State','PictureGuid'],
		
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
				alert("数据库未知错误 " + e + ".");
			}
			alert("数据库错误"+e.message);
			return false;
		}
	},
	ExecSql: function(tx, sql, param, callback) {
		tx.executeSql(sql, param, function(tx, result) {
			if (typeof(callback) == 'function') {
				callback(true);
			}
			return true;
		}, function(tx, error) {
			if (typeof(callback) == 'function') {
				callback(false);
			}
			return false;
		});
	},
	InitDB: function(tx) {
		if (this.options.db == null) {
			this.OpenDB();
		}
		this.CreateTable(tx, this.options.table_UserMessage, this.options.UserMessage,{
			"MessageID": "primary key"
		});
		this.CreateTable(tx, this.options.table_CurrentUsers, this.options.CurrentUsers, {
			"UserID": "primary key",
			"app_flow_no": "not null"
		});
		this.CreateTable(tx, this.options.table_FrontUsers, this.options.FrontUsers,{
			"id": "INTEGER primary key",
			"app_flow_no": "not null"
		});
		this.CreateTable(tx, this.options.table_UserRoles, this.options.UserRoles,{
			"RoleID":"Int"
		});
		this.CreateTable(tx, this.options.table_Departments, this.options.Departments,{
			"DepartmentID": "Int primary key",
			"app_flow_no": "not null"
		});
		this.CreateTable(tx, this.options.table_Projects, this.options.Projects, {
			"ProjectID": "primary key",
			"app_flow_no": "not null"
		});
		this.CreateTable(tx, this.options.table_Scenes, this.options.Scenes, {
			"SceneID": "primary key",
			"app_flow_no": "not null"
		});
		this.CreateTable(tx, this.options.table_Materials, this.options.Materials, {
			"MatterialID": "primary key",
			"app_flow_no": "not null"
		});
		//企业表不确定是否需要
		this.CreateTable(tx, this.options.table_Enterprises, this.options.Enterprises, {
			"EnterpriseID": "primary key",
			"app_flow_no": "not null"
		});
		this.CreateTable(tx, this.options.table_SceneMessageComments, this.options.SceneMessageComments, {
			"MessageID": "primary key"
		});
	},
	CreateTable: function(tx, tableName, fields, constraint) {

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
		this.ExecSql(tx, sql);
	},
	UpdateTable: function(tx, tableName, setFields, setParams, whereStr, wherParams, callback) {
		var sql = "update " + tableName + " set ";
		for (i in setFields) {
			sql += setFields[i] + "=?,";
		}
		sql = sql.substr(0, sql.length - 1);
		if (typeof(whereStr) != "undefined" && typeof(wherParams) != "undefined" && whereStr != "") {
			sql += " where " + whereStr;
			setParams = setParams.concat(wherParams);
		}
		this.ExecSql(tx, sql, setParams, callback);
	},
	InsertTable: function(tx, tableName, insertFields, insertParams, callback) {
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
			this.ExecSql(tx, sql + sql2, insertParams, callback);
		else
			this.ExecSql(tx, sql + sql2, insertParams);
	},
	DeleteTable: function(tx, tableName, whereStr, wherParams, callback) {
		var sql = "delete from " + tableName;
		if (typeof(whereStr) != "undefined" && typeof(wherParams) != "undefined" && whereStr != "") {
			sql += " where " + whereStr;
		}
		this.ExecSql(tx, sql, wherParams, callback);
	},
	DropTable: function(tx, tableName, callback) {
		var sql = 'DROP TABLE IF EXISTS ' + tableName;
		if (typeof(whereStr) != "undefined" && typeof(wherParams) != "undefined" && whereStr != "") {
			sql += " where " + whereStr;
		}
		this.ExecSql(tx, sql, [], callback);
	},
	//@sql:"select * from tb_UserMessage join tb_FrontUsers on tb_UserMessage.UserID=tb_FrontUsers.UserID where UserID=? order by UserID DESC"
	SelectTable: function(tx, sql, wherParams, callback) {
		tx.executeSql(sql, wherParams, function(tx, result) {
			if (result.rows.length < 1) {
				if (typeof(callback) == 'function') {
					callback(false);
				}
			} else {
				if (typeof(callback) == 'function') {
					callback(result.rows);
				}
			}
			return true;
		}, function(tx, error) {
			return false;
		});
	},
	SaveOrUpdateTable: function(tx, tableName, insertFields, insertParams, key, keyVal, callback) {
		var me = this;
		if (typeof(key) != "undefined" && typeof(keyVal) != "undefined" && key != "") {
		var sql = "SELECT " + insertFields[0] + " FROM " + tableName;
			if (typeof(key) != "undefined" && typeof(keyVal) != "undefined" && key != "") {
			sql += " where " +  key + "=?";
		}
			me.SelectTable(tx, sql, [keyVal], function(rows) {
				if (rows) {
					me.UpdateTable(tx, tableName, insertFields, insertParams, key + "=?", [keyVal], callback);
				} else {
					insertFields.push(key);
					insertParams.push(keyVal);
					me.InsertTable(tx, tableName, insertFields, insertParams, callback);
				}
			})
		} else {
			me.InsertTable(tx, tableName, insertFields, insertParams, callback);
		}
	},
	//添加一个事务
	OpenTransaction: function(callback) {
		if (this.options.db == null) {
			this.OpenDB();
		}
		this.options.db.transaction(function(tx) {
			callback(tx); //执行数据操作
		})
	},
	//人员关系角色
	SaveOrUpdateUerRoleTable: function(tx, tableName, insertFields, insertParams, key, keyVal, callback) {
		var me = this;
		if (typeof(key) != "undefined" && typeof(keyVal) != "undefined" && key != "") {
		var sql = "SELECT " + insertFields[0] + " FROM " + tableName;
			if (typeof(key) != "undefined" && typeof(keyVal) != "undefined" && key != "") {
			sql += " where "+key;
		}
			me.SelectTable(tx, sql, keyVal, function(rows) {
				if (rows) {
					me.UpdateTable(tx, tableName, insertFields, insertParams, key, keyVal, callback);
				} else {
					me.InsertTable(tx, tableName, insertFields, insertParams, callback);
				}
			})
		} else {
			me.InsertTable(tx, tableName, insertFields, insertParams, callback);
		}
	}
}