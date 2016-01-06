//查找父级现场/项目
var AddSceneControl = function() {
	var _show = new ProjectAndScene();
	this.SelectProjectAndScenes = function() {
			//获取项目
			if (parentId == -1 || parentId == "-1" || parentId == null) {
				alltem = "";
				dbBase.OpenTransaction(function(tx) {
						dbBase.SelectTable(tx, "select * from tb_Projects where ProjectID=?", [projectId], AddScene);
					})
					//将项目信息放在最前面

				function AddScene(adata) {
					if (adata.length >= 1) {
						//添加角色人员
						_show.AddProject(adata);
						var _projectid = adata.item(0).ProjectID;
						addScene_Control.SelectScene("ProjectID=? And ParentID=? And HasData=?", [_projectid, "-1", 'false'], _show.AddScene)
					}
				}
			} else {
				alltem = "";
				addScene_Control.SelectScene("SceneID=?", [parentId], AddChild);

				function AddChild(adata) {
					_show.AddScene(adata);
					addScene_Control.SelectScene("ProjectID=? And ParentID=? And HasData=?", [projectId, parentId, 'false'], _show.AddScene);
				};
			}
		},
		this.SelectScene = function(wherefile, whereparam, callback) {
			dbBase.OpenTransaction(function(tx) {
				dbBase.SelectTable(tx, "select * from tb_Scenes where " + wherefile, whereparam, callback);
			})
		},
		//修改现场显示
		this.SelectOneScene = function(sceneid) {
			alltem = "";
			dbBase.OpenTransaction(function(tx) {
				dbBase.SelectTable(tx, "select * from tb_Scenes where SceneID=?", [sceneid], function(adata) {
					if (adata) {
						if (adata.item(0).ParentID == "-1" || adata.item(0).ParentID == -1) {
							dbBase.SelectTable(tx, "select * from tb_Projects where ProjectID=?", [adata.item(0).ProjectID], _show.AddProject);
						} else {
							dbBase.SelectTable(tx, "select * from tb_Scenes where SceneID=?", [adata.item(0).ParentID], _show.AddScene);
						}
						ShowSceneInfo(adata);
					}
				})
			})
		}
};
//添加现场类型
function AddSceneTypeControl() {
	dbBase.OpenTransaction(function(tx) {
		dbBase.SelectTable(tx,"select * from tb_SceneTypes where EnterpriseID=?",[userInfo.EnterpriseID],ShowSceneType);
	})
};
/*
 * 查找项目涉及并与属于当前操作者有关部门
 */
function ChoiceDepartments() {
	if(departments){
		var _departments = departments.split("|");
	}else{
		$("#id_fenpei").hide();
		return;
	}
	dbBase.OpenTransaction(function(tx) {
		if (userInfo.DepartmentID == null || userInfo.DepartmentID == "") {
			dbBase.SelectTable(tx, "select * from tb_Departments where EnterpriseID=? ", [userInfo.EnterpriseID], callEnterpriseBack);
		} else {
			dbBase.SelectTable(tx, "select * from tb_Departments where ParentID=? ", [userInfo.DepartmentID], callDepartmentBack);
		}
	});

	function callEnterpriseBack(adata) {
		if (adata) {
			var _len = adata.length;
			var _arr = new Array();
			for (var i = 0; i < _len; i++) {
				_arr.push(adata.item(i).DepartmentID);
			}
			var _bool = GetArrayCros(_arr, _departments);
			if (_bool && _bool.length > 0) { //判断部门是否有操作权限
				ChioceRoles(_arr);
			}
		}
	}

	function callDepartmentBack(adata) {
		var _arr = new Array();
		if (adata) {
			var _len = adata.length;
			for (var i = 0; i < _len; i++) {
				_arr.push(adata.item(i).DepartmentID);
			}
		}
		_arr.push(userInfo.DepartmentID);
		var _bool = GetArrayCros(_arr, _departments);
		if (_bool && _bool.length > 0) { //判断部门是否有操作权限
			ChioceRoles(_arr);
		}else{
			$("#id_fenpei").hide();
			return;
		}
	}
};
//查找对应角色人员
function ChioceRoles(departments) {
	if(roles){
		var _roles = roles.split("|");
	}else{
		$("#id_fenpei").hide();
		return;
	}
	var _len = _roles.length;
	var _showperson = new Person();

	dbBase.OpenTransaction(function(tx) {
		for (var i = 0; i < _len; i++) {
			dbBase.SelectTable(tx, "select * from tb_UserRoles where RoleID=? group by RoleName", [_roles[i]], callback);
		}

		function callback(adata) {
			if (adata) {
				var _adata = {
					type: '',
					typeID: '',
					user: ''
				};
				_adata.type = adata.item(0).RoleName;
				_adata.typeID = adata.item(0).RoleID;
				var str = "(" + departments.join(",") + ")";
				dbBase.SelectTable(tx, "select * from tb_FrontUsers where UserID in (select UserID from tb_UserRoles where RoleID=?) And DepartmentID in " + str, [adata.item(0).RoleID], function(adata) {
					if (adata) {
						var _user = new Array();
						var _length = adata.length;
						for (var i = 0; i < _length; i++) {
							_user.push(adata.item(i));
						}
						_adata.user = _user;
						_showperson.AddOptions(_adata);
					} else {
						_showperson.AddOptions(_adata); //角色无人员
					}
				});
			}
		}
	})
	addScene_Control.SelectOneScene(GetUrlParam('sceneid'));
};
//连网自动发送消息
function AutoSendScene() {
	dbBase.OpenTransaction(function(tx) {
		dbBase.SelectTable(tx, "select * from tb_Scenes where SendStatus=?", ["0"], function(adata) {
			if (adata) {
				var _len = adata.length;
				for (var i = 0; i < _len; i++) {
					var _data = adata.item(i);
					_data.EnterpriseID = userInfo.EnterpriseID;
					_data.Token = userInfo.Token;
					PostAddScene(_data);
				}
			}
		});
		dbBase.SelectTable(tx, "select * from tb_Scenes where SendStatus=?", ["2"], function(adata) {
			if (adata) {
				var _len = adata.length;
				for (var i = 0; i < _len; i++) {
					var _data = adata.item(i);
					_data.EnterpriseID = userInfo.EnterpriseID;
					_data.Token = userInfo.Token;
					PostUpdateScene(_data);
				}
			}
		})
	})
}