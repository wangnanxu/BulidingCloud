//API参数枚举
var DBParasm = {
type:"",    	/*	类型 ,TableOp对象	*/
tableName:"",   /*	表名 ,示例："tb_user"	*/
fields:"",    	/*	字段名,示例：[id,name]	type:TableOp.create/TableOp.insert/TableOp.update/TableOp.saveOrupdate	
						   示例	： "id,name"	type:TableOp.select	*/
constraint: "", /*	字段约束条件,示例：{"id":"primary key"}	type:TableOp.create	*/
params:"",    	/*	参数,示例：[id,name]	type:TableOp.insert/TableOp.update/TableOp.delete/TableOp.select，TableOp.saveOrupdate	*/
where: "",   	/*	where条件,示例："id=?"	type:TableOp.select/TableOp.delete/TableOp.update/TableOp.saveOrupdate	*/
whereparams:"",    	/*	参数,示例：[id,name]	type:TableOp.insert/TableOp.update/TableOp.delete/TableOp.select，TableOp.saveOrupdate	*/
key:"",    		/*	字段名,示例："id"	type:TableOp.saveOrupdate	*/
keyVal:""    	/*	字段名参数,示例：id	type:TableOp.saveOrupdate	*/
}
//API表操作类型参数枚举
var TableOp = {
    create:"CreateTable",
    update:"UpdateTable",
    insert:"InsertTable",
    delete:"DeleteTable",
    select:"SelectTabele",
    saveOrupdate:"SaveOrUpdataTable"
}
//本地数据库通讯
var dbbase;
//打开数据库
var CreatDB=function(){
	if(dbbase==null){
		dbbase=new DataBase();
	}
	if(dbbase.db==null){
		dbbase.initDB();
	}
}

var 

//分析数据
var HandleData=function(DBParasm,successFun,failFun,successParam,failParam){
	switch(DBParasm.type){
		case "update":
			dbbase.updateTable(DBParasm.tableName,"","",CallBackFun);
		break;
		case "insert":
			dbbase.insertTable(DBParasm.tableName,DBParasm.fields,DBParasm.params,CallBackFun);
		break;
		case "select":
			dbbase.select("","","",CallBackFun);
		break;
		case "delete":
			dbbase.deleteRow("","","",CallBackFun);
		break;
		case "saveOrUpdate":
			dbbase.saveOrUpdate("","","",CallBackFun);
		break;
		default:
		break;
	}
	CallBackFun:function(oCallback){
		if (oCallback) {
			if(typeof(successFun)!="function"){
				return;
			}
			successFun(successParam);
		}else{
			if(typeof(failFun)!="function"){
				return;
			}
			failFun(failParam);
		}
	}
}

