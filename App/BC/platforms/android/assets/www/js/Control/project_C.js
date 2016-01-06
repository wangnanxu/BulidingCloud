//查询数据库
function SelectProject() {
	dbBase.OpenTransaction(function(tx) {
		if ($.inArray("Root.AppPermission.SceneManage.ViewAchievedScene", userInfo.FunctionIDs) != -1) {
			//找出所有项目
			dbBase.SelectTable(tx, "select tb_Projects.ProjectID,tb_Projects.Manager,tb_Scenes.SceneWorker from tb_Projects left join tb_Scenes on tb_Projects.ProjectID=tb_Scenes.ProjectID where tb_Projects.EnterpriseID=?", [userInfo.EnterpriseID], SelectWorker);
		} else {
			//施工员权限管理
			dbBase.SelectTable(tx, "select tb_Projects.ProjectID,tb_Projects.Manager,tb_Scenes.SceneWorker from tb_Projects left join tb_Scenes on tb_Projects.ProjectID=tb_Scenes.ProjectID where tb_Projects.EnterpriseID=? And ProjectState!=?", [userInfo.EnterpriseID, 3], SelectWorker);
		}
	})
};
//查找worker
function SelectWorker(adata) {
	if (adata) {
		//alert(JSON.stringify(ChangeSelectToJsondata(adata)))
		var _arr = new Array();
		var _len = adata.length;
		for (var i = 0; i < _len; i++) {
			var _str = adata.item(i).Manager + adata.item(i).SceneWorker;
			if (_str.indexOf(userInfo.UserID) >= 0 && $.inArray(adata.item(i).ProjectID, _arr) == -1) {
				_arr.push("'"+adata.item(i).ProjectID+"'");
			}
		}
		var _arrstr = "(" + _arr.join(",") + ")";
		//拼凑查询格式
		dbBase.OpenTransaction(function(tx) {
			dbBase.SelectTable(tx, "select * from tb_Projects where EnterpriseID=? And ProjectID in " + _arrstr,[userInfo.EnterpriseID], ShowProject);
		})
	}
}