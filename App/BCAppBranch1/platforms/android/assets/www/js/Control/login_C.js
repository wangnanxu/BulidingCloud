//Pgae显示前，检测数据库是否有key

//本地数据库通讯
var dbbase;
var Check = function() {
	if (dbbase == null || dbbase == undefined) {
		dbbase = new DataBase();
	}
	if (dbbase.db == null) {
		dbbase.InitDB();
	}
	dbbase.SelectTable("tb_CurrentUser", "UserKey", "ID=?", [1], _fCallback);

	function _fCallback(oCallback) {
		if (oCallback.length == 1) {

			PostCommand({
				UserKey: oCallback.item(0).UserKey
			}, CallBack);

			function CallBack(oCallback) {
				if (oCallback) {
					GoMessagePage();
				}
			}
		} else {
			return;
		}
	}
}

//登录
var Login = function() {
	var postData = GetLoginData(); //获取Login页面  账户，密码
	if (postData != null) {
		postData.uuid = "device.uuid";
		PostCommand(postData, CallBack);

		function CallBack(oCallback) {
			if (oCallback) {
				dbbase.SaveOrUpdateTable("tb_CurrentUser", ["UserKey", "UserID"], [postData.account + postData.password, postData.account], "ID", 1, _fCallback);
			}
		}

		function _fCallback(oCallback) {
			if (oCallback == true) {
				GoMessagePage();
			} else {
				alert("保存本地数据失败");
			}
		}
	} else {
		return;
	}
}
