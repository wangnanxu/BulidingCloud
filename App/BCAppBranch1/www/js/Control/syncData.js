var PSO = false; //记录组织数据是否同步完成
var PSSAP = false; //记录现场项目数据是否同步完成
var PSMA = false; //记录信息数据是否同步完成
var PSMT = true; //记录资料数据是否同步完成
//页面显示，开始同步数据
$(document).on("pagebeforeshow", "#Page_SyncData", function() {
	if(GetUrlParam("isSame")=="0"){
		PostSyncOrganization(PSOCallback); //请求同步组织
		PostSyncSceneAndProject(PSSAPCallback); //请求同步现场项目
	}else{
		PostInitSyncState(InitSyncState);//同企业账号登陆，服务器更改同步账号状态
		PSO=true;
	}
	var _date=new Date().secondFormat("yyyy-MM-dd hh:mm:ss");
	var _data={
		Token:userInfo.Token,
		QueryTime:_date,
		PageSize:20
	}
	PostHistoryMessage(_data,PSMACallback,"0"); //请求同步信息
	//	PostSyncMaterial(PSMTCallback); //请求同步资料
});
//组织(包括部门、人员)同步数据回调函数
function PSOCallback(jsondata) {
	if (jsondata) {
		var _departments = jsondata.Departments; //部门数据
		var _users = jsondata.FrontUsers; //人员数据
		var _roles = jsondata.UserRoles; //角色数据
		var _lend = _departments.length; //获取部门数据长度
		var _lenu = _users.length; //获取人员数据长度
		var _lenr = _roles.length; //获取人员数据长度
		var _successlend = 0; //部门存储成功数据条数
		var _successlenu = 0; //人员存储成功数据条数
		var _successlenr = 0; //角色存储成功数据条数
		if(_lend==0 && _lenu==0 && _lenr==0){
			PSO = true;
			SyncAllSuccess();
		}
		dbBase.OpenTransaction(function(tx) {
				for (var i = 0; i < _lend; i++) {
					//存储部门数据
					dbBase.SaveOrUpdateTable(tx, "tb_Departments", ['DepartmentID', 'DepartmentName', 'ParentID', 'EnterpriseID'], [_departments[i].DepartmentID, _departments[i].Name, _departments[i].ParentID, _departments[i].EnterpriseID], "DepartmentID", _departments[i].DepartmentID, function(bool) {
						if (bool) {
							_successlend += 1; //每储存一条数据记录一次
							if (_lend == _successlend && (_lenu==0 || _lenu == _successlenu) && (_lenr==0 || _lenr == _successlenr)) {
								PSO = true;
							}
							SyncAllSuccess();
						}
					});
				}
				for (var j = 0; j < _lenu; j++) {
					//存储人员数据
					dbBase.SaveOrUpdateTable(tx, "tb_FrontUsers", ['UserID','UserName', 'DepartmentID', 'EnterpriseID'], [_users[j].UserID,_users[j].Name,_users[j].DepartmentID, _users[j].EnterpriseID], "UserID",_users[j].UserID, function(bool) {
						if (bool) {
							_successlenu += 1; //每储存一条数据记录一次
							if ((_lend==0 || _lend == _successlend) && _lenu == _successlenu && (_lenr==0 || _lenr == _successlenr)) {
								PSO = true;
							}
							SyncAllSuccess();
						}
					});
				}
				for (var k = 0; k < _lenr; k++) {
					//存储人员数据
						dbBase.SaveOrUpdateUerRoleTable(tx, "tb_UserRoles", ['UserID', 'RoleID', 'RoleName', 'EnterpriseID'], [_roles[k].UserID, _roles[k].RoleID, _roles[k].RoleName, _roles[k].EnterpriseID], "UserID=? And RoleID=?", [_roles[k].UserID, _roles[k].RoleID], function(bool) {
						if (bool) {
							_successlenr += 1; //每储存一条数据记录一次
							if ((_lend==0 || _lend == _successlend) && (_lenu==0 || _lenu == _successlenu) && _lenr == _successlenr) {
								PSO = true;
							}
							SyncAllSuccess();
						}
					});
				}
			})
			//数据条数与存储成功数相同则组织数据同步完成
	}
	else{
		PSO = true;
		SyncAllSuccess();
	}
};
//现场和项目同步数据回调函数
function PSSAPCallback(jsondata) {
	if (jsondata) {
		var _projectdata = jsondata.Projects; //获取项目数据
		var _scenedata = jsondata.Scenes; //获取现场数据
		var _lenproject = _projectdata.length; //获取项目数据长度
		var _lenscene = _scenedata.length; //获取现场数据长度
		var _successlenproject = 0; //项目数据存储成功数据条数
		var _successlenscene = 0; //现场数据存储成功数据条数
		if(_lenproject==0 && _lenscene==0){
			PSSAP = true;
			SyncAllSuccess();
		}
		dbBase.OpenTransaction(function(tx) {
			for (var i = 0; i < _lenproject; i++) {
				//存储项目数据
				dbBase.SaveOrUpdateTable(tx, "tb_Projects", ['ProjectID', 'ProjectName', 'Departments', 'HaveScene', 'ProjectRoles', 'ProjectState', 'Manager', 'EnterpriseID'], [_projectdata[i].ProjectID, _projectdata[i].ProjectName, _projectdata[i].Departments, _projectdata[i].HaveScene, _projectdata[i].ProjectRoles, _projectdata[i].ProjectState, _projectdata[i].Manager,_projectdata[i].EnterpriseID], "ProjectID", [_projectdata[i].ProjectID], function(bool) {
					if (bool) {
						_successlenproject += 1;
					}
					//数据条数与存储成功数相同则数据同步完成
					if (_lenproject == _successlenproject && (_lenscene==0 || _lenscene == _successlenscene)) {
						PSSAP = true;
					}
					SyncAllSuccess();
				});
			}
			for (var j = 0; j < _lenscene; j++) {
				//存储现场数据
				dbBase.SaveOrUpdateTable(tx, "tb_Scenes", ['SceneID', 'ParentID', 'SceneName','ProjectID', 'SceneWorker', 'SceneState','SceneType','Address','BeginDate','EndDate','HasData','SendStatus','AllWorkers'], [_scenedata[j].SceneID, _scenedata[j].ParentID, _scenedata[j].SceneName, _scenedata[j].ProjectID, JSON.stringify(_scenedata[j].SceneWorker), _scenedata[j].SceneState, _scenedata[j].SceneType, _scenedata[j].Address, _scenedata[j].BeginDate, _scenedata[j].EndDate, _scenedata[j].HasData,'1',JSON.stringify(_scenedata[j].AllWorkers)], "SceneID", [_scenedata[j].SceneID], function(bool) {
					if (bool) {
						_successlenscene += 1;
					}
					//数据条数与存储成功数相同则数据同步完成
					if ((_lenproject==0 || _lenproject == _successlenproject) && _lenscene == _successlenscene) {
						PSSAP = true;
					}
					SyncAllSuccess();
				});
			}
		})

	}
	else{
		PSSAP = true;
		SyncAllSuccess();
	}
};
//消息同步数据回调函数
function PSMACallback(jsondata) {
	if (jsondata) {
		var _len = jsondata.length; //获取要同步的消息总条数
		var _successlen = 0; //消息数据存储成功条数
		if(_len==0){
			PSMA = true;
			SyncAllSuccess();
		}
		dbBase.OpenTransaction(function(tx) {
			for (var i = 0; i < _len; i++) {
				//存储消息数据
				dbBase.SaveOrUpdateTable(tx, "tb_UserMessage", ['MessageID','SendUserID', 'Message', 'SendTime', 'Recipients', 'SendUserName', 'SendUserPicture', 'HeadPictureURI', 'Type', 'EnterpriseID','Time', 'Status'], [jsondata[i].MessageID,jsondata[i].SendUserID, jsondata[i].Message, jsondata[i].SendTime, jsondata[i].Recipients, jsondata[i].SendUserName, jsondata[i].SendUserPicture, "","", jsondata[i].EnterpriseID,jsondata[i].Time,'1'], "MessageID", [jsondata[i].MessageID], function(bool) {
					if (bool) {
						_successlen += 1;
					}
					if (_len == _successlen) {
						PSMA = true;
					}
					SyncAllSuccess();
				});
			}
		})

	}
	else{
		PSMA = true;
		SyncAllSuccess();
	}
};
//资料同步数据回调函数
function PSMTCallback(jsondata) {
	if (jsondata) {
		var _len = jsondata.length; //获取要同步的资料总条数
		var _successlen = 0; //资料数据存储成功条数
		if(_len==0){
			PSMT = true;
			SyncAllSuccess();
		}
		dbBase.OpenTransaction(function(tx) {
			for (var i = 0; i < _len; i++) {
				//存储消息数据
				dbBase.SaveOrUpdateTable(tx, "tb_Material", ['MatterialID', 'MaterialName', 'UserID', 'Download', 'Type', 'EnterpriseID'], [jsondata[i].MatterialID, jsondata[i].MaterialName, jsondata[i].UserID, jsondata[i].Download, jsondata[i].Type,jsondata[i].EnterpriseID], "", "", function(bool) {
					if (bool) {
						_successlen += 1;
					}
					if (_len == _successlen) {
						PSMT = true;
					}
					SyncAllSuccess();
				});
			}
		})

	}
	else{
		PSMT = true;
		SyncAllSuccess();
	}
};
function InitSyncState(success){
	if(success){
		PSSAP=true;
	}else{
		NotificationConfirm("初始化数据失败,请重新登陆!", "错误提示", "确定", Confirm);
		function Confirm(button) {
			ExitToLoading("0") //从同步界面退回到登陆页面
		}
	}
	SyncAllSuccess();
}
function SyncAllSuccess() {
	//所有数据同步完成，跳转信息页面
	if (PSO && PSSAP && PSMA && PSMT) {
		ChangePage("showMessage.html");
	}
}