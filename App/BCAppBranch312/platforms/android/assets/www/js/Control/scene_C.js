var departments; //项目涉及部门
var roles; //项目涉及角色
var isHaveScene; //是否有子现场
//现场管理
var SceneControl = function() {
	/*
	 * 查询所有一级现场数据
	 * @projectid项目ID
	 */
	this.SelectAllScene = function(projectid) {
			dbBase.OpenTransaction(function(tx) {
				if (projectmanager && projectmanager.indexOf(userInfo.UserID) >= 0) { //项目管理员可以查看所有现场
					if ($.inArray("Root.AppPermission.SceneManage.ViewAchievedScene", userInfo.FunctionIDs) != -1) {
						dbBase.SelectTable(tx, "select * from tb_Scenes where ProjectID=? And ParentID=?", [projectid, "-1"], scene_View.ShowAllScenes);
					} else {
						dbBase.SelectTable(tx, "select * from tb_Scenes where ProjectID=? And ParentID=? And SceneState!=?", [projectid, "-1", 3], scene_View.ShowAllScenes);
					}
				} else { //不是项目管理员只能查看自己的工作现场
					if ($.inArray("Root.AppPermission.SceneManage.ViewAchievedScene", userInfo.FunctionIDs) != -1) {
						dbBase.SelectTable(tx, "select * from tb_Scenes where ProjectID=? And ParentID=? And AllWorkers like '%" + userInfo.UserID + "%'", [projectid, "-1"], scene_View.ShowAllScenes);
					} else {
						dbBase.SelectTable(tx, "select * from tb_Scenes where ProjectID=? And ParentID=? And SceneState!=? And AllWorkers like '%" + userInfo.UserID + "%'", [projectid, "-1", 3], scene_View.ShowAllScenes);
					}
				}
				dbBase.SelectTable(tx, "select Departments,ProjectRoles from tb_Projects where ProjectID=?", [projectid], function(adata) {
					if (adata) {
						departments = adata.item(0).Departments;
						roles = adata.item(0).ProjectRoles;
					}
				});
			})
		},
		/*
		 * 查询子现场数据
		 * @sceneid现场id
		 */
		this.SelectChildScene = function(sceneid) {
			isHaveScene=false;
			dbBase.OpenTransaction(function(tx) {
				if (projectmanager && projectmanager.indexOf(userInfo.UserID) >= 0) {
					if ($.inArray("Root.AppPermission.SceneManage.ViewAchievedScene", userInfo.FunctionIDs) != -1) {
						dbBase.SelectTable(tx, "select * from tb_Scenes where ProjectID=? And ParentID=?", [projectId, "" + sceneid], scene_View.ShowScenes);
					} else {
						dbBase.SelectTable(tx, "select * from tb_Scenes where ProjectID=? And ParentID=? And SceneState!=?", [projectId, "" + sceneid, 3], scene_View.ShowScenes);
					}
				} else {
					if ($.inArray("Root.AppPermission.SceneManage.ViewAchievedScene", userInfo.FunctionIDs) != -1) {
						dbBase.SelectTable(tx, "select * from tb_Scenes where ProjectID=? And ParentID=?", [projectId, "" + sceneid], function(adata) {
							if (adata) {
								parentId=adata.item(0).ParentID;
								isHaveScene = true;
							}else{
								isHaveScene=false;
							}
							dbBase.SelectTable(tx, "select * from tb_Scenes where ProjectID=? And ParentID=? And AllWorkers like '%" + userInfo.UserID + "%'", [projectId, "" + sceneid], scene_View.ShowScenes);
						})

					} else {
						dbBase.SelectTable(tx, "select * from tb_Scenes where ProjectID=? And ParentID=?", [projectId, "" + sceneid], function(adata) {
							if (adata) {
								parentId=adata.item(0).ParentID;
								isHaveScene = true;
							}
							else{
								isHaveScene=false;
							}
							dbBase.SelectTable(tx, "select * from tb_Scenes where ProjectID=? And ParentID=? And SceneState!=? And AllWorkers like '%" + userInfo.UserID + "%'", [projectId, "" + sceneid, 3], scene_View.ShowScenes);
						})
					}
				}
			})
		},
		/*
		 * 返回父现场数据
		 * @parentid父现场sceneid
		 */
		this.SelectParentScene = function(parentid) {
			dbBase.OpenTransaction(function(tx) {
				if ($.inArray("Root.AppPermission.SceneManage.ViewAchievedScene", userInfo.FunctionIDs) != -1) {
					dbBase.SelectTable(tx, "select * from tb_Scenes where SceneID=?", ["" + parentid], function(adata) {
						if (adata) {
							var _parentid = adata.item(0).ParentID;
							scene_Control.SelectChildScene(_parentid);
						}
					})
				} else {
					dbBase.SelectTable(tx, "select * from tb_Scenes where SceneID=? And SceneState!=?", ["" + parentid, 3], function(adata) {
						if (adata) {
							var _parentid = adata.item(0).ParentID;
							scene_Control.SelectChildScene(_parentid);
						}
					})
				}
			})
		}
}