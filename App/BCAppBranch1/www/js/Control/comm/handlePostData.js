var ACTION = {
		SyncOrganization: 1,
		SyncSceneAndProject: 2,
		SyncSceneData: 4,
		SyncMessage: 8
	}
	//分流Worker返回数据

function HandleWorkerBackData(data) {
	if (data) {
		try {
			var _jsondata = JSON.parse(data);
			if (_jsondata.Success) {
				if (_jsondata.Value != null && _jsondata.Value != "") {
					var _data = _jsondata.Value;
					var _len = _data.length;
					for (var i = 0; i < _len; i++) {
						if (_data[i].DeletedEntities) {
							WorkerDeleteData(_data[i].DeletedEntities); //处理删除数据
						} else {
							WorkerActiveData(_data[i]); //处理动作
						}
					}
				}
			} else {
				if (_jsondata.LoginFailed && _jsondata.LoginFailed == true) {
					ExitToLoading("0");
				}
				alert(_jsondata.Message);
			}
		} catch (e) {

		}
	}
};
//处理动作
function WorkerActiveData(data) {
	if (data) {
		var _active = new HandleActionPost(); //active操作类
		var _activedata = new HandleActionData(); //active post请求返回数据处理类
		var _postdata = {
			Token: userInfo.Token
		}
		switch (data.Action) {
			case ACTION.SyncOrganization:
				_active.SyncOrganization(_postdata, data, _activedata.SyncOrganization);
				break;
			case ACTION.SyncSceneAndProject:
				_active.SyncSceneAndProject(_postdata, data, _activedata.SyncSceneAndProject);
				break;
			case ACTION.SyncSceneData:
				//测试
				//_activedata.SyncSceneData(ceshidata.Value);
				//请求数据
				_active.SyncSceneData(_postdata, data, _activedata.SyncSceneData);
				break;
			case ACTION.SyncMessage:
				//写入数据库
				data.Data.Time = data.Time;
				_activedata.SyncMessage(data.Data);
				//显示数据
				ShowAddUserMessage(data.Data);
				break;
			default:
				//alert("worker同步错误");
				break;
		}
	}
};
//处理删除数据
function WorkerDeleteData(data) {
	if (data) {
		var _delete = new HandleDeleteData(); //删除类
		var _len = data.length;
		dbBase.OpenTransaction(function(tx) {
				for (var i = 0; i < _len; i++) {
					switch (data[i].EntityName) {
						case "SceneItem":
							_delete.DeleteSceneItem(tx, data[i].EntityID);
							WorkerDeleteSceneItem(data[i].EntityID);
							break;
						case "UserRole":
							_delete.DeleteUserRole(data[i].EntityID);
							break;
						case "FrontUser":
							_delete.DeleteFrontUser(data[i].EntityID);
							break;
						case "Role":
							break;
						default:
							break;
					}
				}
			})
			//处理删除数据
	}
};

//worker Activeq请求数据处理
var HandleActionData = function() {
	var _delete = new HandleDeleteData(); //删除类
	//请求组织数据处理
	this.SyncOrganization = function(jsondata) {
			if (jsondata) {
				var _departments = jsondata.Departments; //部门数据
				var _users = jsondata.FrontUsers; //人员数据
				var _roles = jsondata.UserRoles; //角色数据
				var _lend = _departments.length; //获取部门数据长度
				var _lenu = _users.length; //获取人员数据长度
				var _lenr = _roles.length; //获取人员数据长度
				dbBase.OpenTransaction(function(tx) {
					for (var i = 0; i < _lend; i++) {
						if (_departments[i].Deleted) {
							_delete.DeleteDepartment(_departments[i]);
						} else {
							//存储部门数据
							dbBase.SaveOrUpdateTable(tx, "tb_Departments", ['DepartmentID', 'DepartmentName', 'ParentID', 'EnterpriseID'], [_departments[i].DepartmentID, _departments[i].Name, _departments[i].ParentID, _departments[i].EnterpriseID], "DepartmentID", _departments[i].DepartmentID);
						}
					}
					for (var j = 0; j < _lenu; j++) {
						if (_users[j].UserID == userInfo.UserID) {
							userInfo.DepartmentID = _users[j].DepartmentID;
							userInfo.UserName = _users[j].Name;
							userInfo.EnterpriseID = _users[j].EnterpriseID;
						}
						//存储人员数据
						dbBase.SaveOrUpdateTable(tx, "tb_FrontUsers", ['UserID', 'UserName', 'DepartmentID', 'EnterpriseID'], [_users[j].UserID, _users[j].Name, _users[j].DepartmentID, _users[j].EnterpriseID], "UserID", _users[j].UserID);
						dbBase.SaveOrUpdateTable(tx, "tb_SceneMessageComments", ['UserID', 'UserName'], [_users[j].UserID, _users[j].Name], "UserID", _users[j].UserID);
						dbBase.SaveOrUpdateTable(tx, "tb_UserMessage", ['SendUserID', 'SendUserName'], [_users[j].UserID, _users[j].Name], "SendUserID", _users[j].UserID);
					}
					var _arr = new Array();
					for (var k = 0; k < _lenr; k++) {
						if (userInfo.UserID == _roles[k].UserID) {
							_arr.push(_roles[k].RoleID);
						}
						dbBase.SaveOrUpdateUerRoleTable(tx, "tb_UserRoles", ['UserID', 'RoleID', 'RoleName', 'EnterpriseID'], [_roles[k].UserID, _roles[k].RoleID, _roles[k].RoleName, _roles[k].EnterpriseID], "UserID=? And RoleID=?", [_roles[k].UserID, _roles[k].RoleID]);
					}
					userInfo.RoleIDs = _arr.join(",");
				})
			}
		},
		//请求项目、现场数据处理
		this.SyncSceneAndProject = function(jsondata) {
			if (jsondata) {
				var _projectdata = jsondata.Projects; //获取项目数据
				var _scenedata = jsondata.Scenes; //获取现场数据
				var _lenproject = _projectdata.length; //获取项目数据长度
				var _lenscene = _scenedata.length; //获取现场数据长度
				dbBase.OpenTransaction(function(tx) {
					for (var i = 0; i < _lenproject; i++) {
						if (_projectdata[i].Deleted) {
							_delete.DeleteProject(_projectdata[i]);
						} else {
							//存储项目数据
							dbBase.SaveOrUpdateTable(tx, "tb_Projects", ['ProjectID', 'ProjectName', 'Departments', 'HaveScene', 'ProjectRoles', 'ProjectState', 'Manager', 'EnterpriseID'], [_projectdata[i].ProjectID, _projectdata[i].ProjectName, _projectdata[i].Departments, _projectdata[i].HaveScene, _projectdata[i].ProjectRoles, _projectdata[i].ProjectState, _projectdata[i].Manager, _projectdata[i].EnterpriseID], "ProjectID", [_projectdata[i].ProjectID]);
						}
					}
					for (var j = 0; j < _lenscene; j++) {
						if (_scenedata[j].Deleted) {
							_delete.DeleteScene(_scenedata[j]);
						} else {
							//存储现场数据
							dbBase.SaveOrUpdateTable(tx, "tb_Scenes", ['SceneID', 'ParentID', 'SceneName', 'ProjectID', 'SceneWorker', 'SceneState', 'SceneType', 'Address', 'BeginDate', 'EndDate', 'SendStatus', 'HasData', 'AllWorkers'], [_scenedata[j].SceneID, _scenedata[j].ParentID, _scenedata[j].SceneName, _scenedata[j].ProjectID, JSON.stringify(_scenedata[j].SceneWorker), _scenedata[j].SceneState, _scenedata[j].SceneType, _scenedata[j].Address, _scenedata[j].BeginDate, _scenedata[j].EndDate, '1', _scenedata[j].HasData, JSON.stringify(_scenedata[j].AllWorkers)], "SceneID", [_scenedata[j].SceneID]);
							CompleteCurrentScene(_scenedata[j].SceneID, _scenedata[j].SceneState);
						}
					}
				})
			}
		},
		//请求现场具体数据处理
		this.SyncSceneData = function(jsondata) {
			if (jsondata) {
				PushHistoryData(jsondata);
			}
		},
		//请求消息数据处理
		this.SyncMessage = function(jsondata) {
			if (jsondata) {
				PushAddMessage(jsondata);
			}
		}
};
//删除数据
var HandleDeleteData = function() {
	//删除聊天消息
	this.DeleteMessage = function(jsondata) {
			if (jsondata) {
				var _len = jsondata.length;
				dbBase.OpenTransaction(function(tx) {
					dbBase.DeleteTable(tx, "tb_UserMessage", "MessageID=?", [jsondata[i].MessageID]); //删除消息
				})
			}
		},
		//项目:删除项目(worker)
		this.DeleteProject = function(jsondata) {
			if (jsondata) {
				dbBase.OpenTransaction(function(tx) {
					dbBase.DeleteTable(tx, "tb_Projects", "ProjectID=?", [jsondata.ProjectID]); //删除项目
					dbBase.SelectTable(tx, "select SceneID from tb_Scenes where ProjectID=?", [jsondata.ProjectID], function(adata) {
						if (adata) {
							var _length = adata.length;
							for (var i = 0; i < _length; i++) {
								dbBase.DeleteTable(tx, "tb_SceneMessageComments", "SceneID=?", [adata.item(0).SceneID]); //删除与项目有关的具体现场数据
							}
						}
					});
					dbBase.DeleteTable(tx, "tb_Scenes", "ProjectID=?", [jsondata.ProjectID]); //删除项目相关现场
				})
			}
		},
		//现场:删除现场(worker)
		this.DeleteScene = function(jsondata) {
			if (jsondata) {
				dbBase.OpenTransaction(function(tx) {
					var _bool = false;
					dbBase.DeleteTable(tx, "tb_Scenes", "SceneID=?", [jsondata.SceneID], function(data) {
						if (data) {
							if (_bool) {
								DeleteCurrentScene(jsondata.SceneID);
							}
							_bool = true;
						}
					}); //删除现场
					dbBase.DeleteTable(tx, "tb_SceneMessageComments", "SceneID=?", [jsondata.SceneID], function(adata) {
						if (adata) {
							if (_bool) {
								DeleteCurrentScene(jsondata.SceneID);
							}
							_bool = true;
						}
					}); //删除与现场有关的具体现场信息
				})
			}
		},
		//具体现场：删除一条消息
		this.DeleteSceneItem = function(tx, messageid) {
			if (messageid) {
				dbBase.DeleteTable(tx, "tb_SceneMessageComments", "MessageID=?", [messageid]); //删除现场消息
			}
		},
		//资料:删除资料
		this.DeleteMaterial = function(jsondata) {
			if (jsondata) {
				var _len = jsondata.length;
				dbBase.OpenTransaction(function(tx) {
					for (var i = 0; i < _len; i++) {
						dbBase.DeleteTable(tx, "tb_Materials", "MaterialID=?", [jsondata[i].MaterialID]);
					}
				})
			}
		},
		//部门信息:删除部门信息
		this.DeleteDepartment = function(jsondata) {
			if (jsondata) {
				dbBase.OpenTransaction(function(tx) {
					//删除整条数据
					dbBase.ExecSql(tx, "delete from tb_Departments where DepartmentID=?", [jsondata.DepartmentID]);
					dbBase.SaveOrUpdateTable(tx, "tb_FrontUsers", "DepartmentID", [], "DepartmentID=?", [jsondata.DepartmentID]); //删除部门人员
					//dbBase.DeleteTable(tx, "tb_Projects ", "Departments=?", [jsondata.DepartmentID]); //删除部门下项目
					dbBase.SelectTable(tx, "select ProjectID,Departments form tb_Projects where Departments like '%" + jsondata.DepartmentID + "%'", [], function(adata) {
						if (adata) {
							var _len = adata.length;
							for (var i = 0; i < _len; i++) {
								var _str = adata.item(i).Deparments;
								_str = _str.replace(jsondata.DepartmentID, "");
								dbBase.UpdateTable(tx, "tb_Projects", ['Departments'], [_str], "ProjectID", adata.item(i).ProjectID);
							}
						}
					}); //修改与部门有关的项目
				})
			}
		},
		//人员信息:删除人员信息
		this.DeleteFrontUser = function(userid) {
			if (userid) {
				dbBase.OpenTransaction(function(tx) {
					dbBase.DeleteTable(tx, "tb_FrontUsers", "UserID=?", [userid]);
					dbBase.DeleteTable(tx, "tb_UserRoles", "UserID=?", [userid]);
					dbBase.DeleteTable(tx, "tb_UserMessage", "SendUserID=?", [userid]);
					dbBase.DeleteTable(tx, "tb_SceneMessageComments", "UserID=?", [userid]);
				})
			}
		},
		//角色信息:删除角色信息
		this.DeleteRoles = function(jsondata) {
			if (jsondata) {
				var _len = jsondata.length;
				dbBase.OpenTransaction(function(tx) {
					for (var i = 0; i < _len; i++) {
						dbBase.DeleteTable(tx, "tb_UserRoles", "RoleID=?", [jsondata[i].RoleID]);
					}
				})
			}
		},
		//关系信息:删除人员角色关系
		this.DeleteUserRole = function(adata) {
			if (adata) {
				dbBase.OpenTransaction(function(tx) {
					var _jsondata = {
							UserID: adata.substring(0, 6),
							RoleID: adata.substring(7, adata.length)
						}
						//删除整条数据
					dbBase.ExecSql(tx, "delete from tb_UserRoles where UserID=? AND RoleID=?", [_jsondata.UserID, _jsondata.RoleID]);
				});
			}
		}
};
//具体现场：增加新的消息(历史，发送，worker)
function PushHistoryData(jsondata) {
	if (jsondata) {
		var _len = jsondata.length;
		dbBase.OpenTransaction(function(tx) {
			for (var i = 0; i < _len; i++) {
				PushNewToSceneItem(tx, jsondata[i]);
			}
		})
	}
};
//具体现场：增加一条新的消息(历史，发送，worker)

function PushNewToSceneItem(tx, jsondata, callback) {
	if (jsondata) {
		var _strcomments = ""
		if (jsondata.Comments != null)
			_strcomments = JSON.stringify(jsondata.Comments);
		var _strimages = "";
		if (jsondata.Images != null)
			_strimages = JSON.stringify(jsondata.Images);
		dbBase.SaveOrUpdateTable(tx, "tb_SceneMessageComments", ["MessageID", "SceneID", "UserID", "UserPicture", "UserPictureURI", "UserName", "Address", "CreateTime", "Description", "Images", "Comments", "Status", "Type", "State", "PictureGuid"], [jsondata.Id, jsondata.SceneID, jsondata.UserID, jsondata.UserPicture, "", jsondata.UserName, jsondata.Address, jsondata.CreateTime, jsondata.Description, _strimages, _strcomments, jsondata.Status, jsondata.Type, 1, jsondata.PictureGuid], "MessageID", jsondata.Id, AddSceneMessageCallBack);

		function AddSceneMessageCallBack(IsTrue) {
			if (IsTrue && typeof(callback) == "function") {}
		}
	}
};
/*删除具体现场数据*/
function PushDeleteSceneItem(messageid) {
	dbBase.OpenTransaction(function(tx) {
		dbBase.DeleteTable(tx, "tb_SceneMessageComments", "MessageID = ?", [messageid], function(istrue) {
			//			alert("Delete" + istrue);
		});
	})
};
/*//具体现场：归档一条消息
function PushArchiveToSceneItem(messageid) {
	if (jsondata) {
		var _len = jsondata.length;
		dbBase.OpenTransaction(function(tx) {
			for (var i = 0; i < _len; i++) {
				dbBase.SaveOrUpdateTable(tx, "tb_SceneMessage", ["State"], [3], "MessageID=?", messageid);
			}
		})
	}
};
//具体现场：添加一条新的评论
function PushCommentToSceneItem(jsondata) {
	if (jsondata) {
		var _len = jsondata.length;
		dbBase.OpenTransaction(function(tx) {
			for (var i = 0; i < _len; i++) {
				dbBase.InsertTable(tx, "tb_Comment", ["CommentGuid", "Id", "CUserId", "CUserName", "Content", "Time"], [jsondata.CommentGuid, jsondata.MessageID, jsondata.CommentUser, jsondata.CommentContent, jsondata.CommentTime]);
			}
		})
	}
};*/
//消息(只有添加,没有修改与删除):添加消息(历史，发送，回复，worker)
function PushAddMessage(jsondata) {
	if (jsondata) {
		var _len = jsondata.length;
		//存储发送消息数据
		if (_len) {
			dbBase.OpenTransaction(function(tx) {
				for (var i = 0; i < _len; i++) {
					dbBase.SaveOrUpdateTable(tx, "tb_UserMessage", ['MessageID', 'SendUserID', 'Message', 'SendTime', 'Recipients', 'SendUserName', 'SendUserPicture', 'HeadPictureURI', 'Type', 'EnterpriseID', 'Time', 'Status'], [jsondata[i].MessageID, jsondata[i].SendUserID, jsondata[i].Message, jsondata[i].SendTime, jsondata[i].Recipients, jsondata[i].SendUserName, jsondata[i].SendUserPicture, "", "", jsondata[i].EnterpriseID, jsondata[i].Time, '1'], "MessageID", jsondata[i].MessageID);
				}
			})
		} else {
			dbBase.OpenTransaction(function(tx) {
				dbBase.SaveOrUpdateTable(tx, "tb_UserMessage", ['MessageID', 'SendUserID', 'Message', 'SendTime', 'Recipients', 'SendUserName', 'SendUserPicture', 'HeadPictureURI', 'Type', 'EnterpriseID', 'Time', 'Status'], [jsondata.MessageID, jsondata.SendUserID, jsondata.Message, jsondata.SendTime, jsondata.Recipients, jsondata.SendUserName, jsondata.SendUserPicture, "", "", jsondata.EnterpriseID, jsondata.Time, '1'], "MessageID", jsondata.MessageID);
			})
		}
	}
};
//写入发送消息
function PushAddSendMessage(jsondata) {
	if (jsondata) {
		dbBase.OpenTransaction(function(tx) {
			dbBase.SaveOrUpdateTable(tx, "tb_UserMessage", ['MessageID', 'SendUserID', 'Message', 'SendTime', 'Recipients', 'SendUserName', 'SendUserPicture', 'HeadPictureURI', 'Type', 'EnterpriseID', 'Time', 'Status'], [jsondata.MessageID, jsondata.SendUserID, jsondata.Message, jsondata.SendTime, jsondata.Recipients, jsondata.SendUserName, jsondata.SendUserPicture, "", jsondata.Type, jsondata.EnterpriseID, jsondata.Time, jsondata.Status]);
		})
	}
};
//修改发送消息
function PushUpdateSendMessage(jsondata, callback) {
	if (jsondata) {
		dbBase.OpenTransaction(function(tx) {
			dbBase.SaveOrUpdateTable(tx, "tb_UserMessage", ['Status'], ['1'], "MessageID", [jsondata.MessageID], function(adata) {
				if (adata) {
					if (typeof(callback) == "function") {
						callback();
					}
				}
			});
		})
	}
};
//请求失败，删除消息
function PushDeleteSendMessage(jsondata, callback) {
	if (jsondata) {
		dbBase.OpenTransaction(function(tx) {
			dbBase.DeleteTable(tx, "tb_UserMessage", ['MessageID'], [jsondata.MessageID], function(adata) {
				if (adata) {
					if (typeof(callback) == "function") {
						callback();
					}
				}
			});
		})
	}
};
//返回登陆页面
function ExitToLoading(type) {
	if ($.mobile.activePage.attr("id") != 'Page_Login') {
		if (workerPost) {
			workerPost.terminate(); //关闭worker
			workerPost = null;
		}
		PSO = PSSAP = PSMA = false; //同步数据设为false
		var _length = userInfo.FunctionIDs.length;
		for (var i = 0; i < _length; i++) {
			var _str = userInfo.FunctionIDs[i].split('.');
			var _permission = _str[_str.length - 1];
			if (userInfo[_permission] != undefined) {
				userInfo[_permission] = false;
			}
		}
		if (type == "0") {
			dbBase.OpenTransaction(function(tx) { //清除登陆Token
				dbBase.SaveOrUpdateTable(tx, "tb_CurrentUsers", ['Token'], [""], "Token", userInfo.Token, function(data) {
					if (data) {
						ChangePage("../index.html"); //返回登陆页面
					}
					PostLoginOut();
				});
			})
		} else {
			ChangePage("../index.html"); //返回登陆页面
		}
	}
};
//未发送现场写入数据库
function PushAddNoSendScene(jsondata) {
	dbBase.OpenTransaction(function(tx) {
		dbBase.SaveOrUpdateTable(tx, "tb_Scenes", ['SceneID', 'ParentID', 'SceneName', 'ProjectID', 'SceneWorker', 'SceneState', 'SceneType', 'Address', 'BeginDate', 'EndDate', 'SendStatus', 'HasData'], [jsondata.SceneID, jsondata.ParentID, jsondata.SceneName, jsondata.ProjectID, jsondata.SceneWorker, jsondata.SceneState, jsondata.SceneType, jsondata.Address, jsondata.BeginDate, jsondata.EndDate, jsondata.SendStatus, jsondata.HasData], "SceneID", jsondata.SceneID);
	})
};
//修改发送现场发送状态和ID
function PushUpdateSendScene(jsondata, guid, callback) {
	dbBase.OpenTransaction(function(tx) {
		if (jsondata.SceneID == undefined) {
			jsondata.SceneID = guid;
		}
		dbBase.SaveOrUpdateTable(tx, "tb_Scenes", ['SceneID', 'SendStatus'], [jsondata.SceneID, '1'], "SceneID", guid, function(adata) {
			if (adata) {
				if (typeof(callback) == 'function') {
					callback();
				}
			}
		});
	})
};
//删除发送现场
function PushDeleteSendScene(guid) {
	dbBase.OpenTransaction(function(tx) {
		dbBase.DeleteTable(tx, "tb_Scenes", ['SceneID'], [guid]);
	})
};
//完工修改现场状态
function PushUpdateSceneState(sceneid) {
	dbBase.OpenTransaction(function(tx) {
		dbBase.SaveOrUpdateTable(tx, "tb_Scenes", ['SceneState'], [3], "SceneID", sceneid, function(adata) {
			if (adata) {
				CompleteCurrentScene(sceneid, 3);
			}
		})
	})
}