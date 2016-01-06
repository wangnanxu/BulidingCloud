function CameraOrLoaclCallback(fileobj) {
	//上传
	var _uploadjson = {
		Type: 1, //上传类型，0图片，1头像
		URI: fileobj.URI, //上传文件地址   
		Pramas: "", //过渡参数
		UploadPramas: {
			Token: userInfo.Token
		}, //上传参数
		Callback: UploadCallback //回调函数
	}
	UploadFile(_uploadjson);

	function UploadCallback(response) {
		DownLoad_New(response.Value, 1, "", DownladCallback);

		function DownladCallback(URI, pramas) {
			dbBase.OpenTransaction(function(tx) {
				dbBase.UpdateTable(tx, "tb_CurrentUsers", ["HeadPictureURI", "HeadPictureName"], [URI, response.Value], "UserID=?", userInfo.UserID, function() {
					userInfo.HeadPictureURI = URI;
					SetImgSrc();
				})
			});
		}
	}
};

function AlertDeleteTable() {
	if (CheckPlatform()) {
		DeleteTable(1);
	} else {
		NotificationConfirm("清除缓存", "是否清除缓存?", "确定,取消", DeleteTable);
	}
}

function DeleteTable(button) {
	if (button == 1) {
		PostClearCache();
		dbBase.OpenTransaction(function(tx) {
			dbBase.DropTable(tx, "tb_CurrentUsers");
			dbBase.DropTable(tx, "tb_UserMessage");
			dbBase.DropTable(tx, "tb_Departments");
			dbBase.DropTable(tx, "tb_Projects");
			dbBase.DropTable(tx, "tb_Scenes");
			dbBase.DropTable(tx, "tb_SceneMessage");
			dbBase.DropTable(tx, "tb_SceneMessageComments");
			dbBase.DropTable(tx, "tb_Materials");
			dbBase.DropTable(tx, "tb_UserRoles");
			dbBase.DropTable(tx, "tb_Comment");
			dbBase.DropTable(tx, "tb_FrontUsers");
			dbBase.DropTable(tx, "tb_SceneTypes");
		})
		setTimeout(ExitToLoading("1"), 3000);
	}
};

function UpdatePassword() {
	$("#msgoldpsw").html("");
	$("#msgnewpsw").html("");
	$("#msgcfmpsw").html("");
	var _oldpsw = $("#oldpsw").val();
	var _newpsw = $("#newpsw").val();
	var _cfmpsw = $("#cfmpsw").val();
	if (_oldpsw == "") {
		$("#msgoldpsw").html("原始密码不能为空");
		return;
	}
	if (_newpsw == "") {
		$("#msgnewpsw").html("新密码不能为空");
		return;
	}
	if (_cfmpsw == "") {
		$("#msgcfmpsw").html("确认密码不能为空");
		return;
	}
	if ($("#newpsw").val() != $("#cfmpsw").val()) {
		$("#msgcfmpsw").html("两次密码不同");
		return;
	}
	PostUpdatePassword(_oldpsw, _newpsw, function() {
		//		$('#popupMenuPsw').popup('close');
		//		NotificationAlert("");
		alert("更改密码成功");
		ChangePage('userInformation.html');
	})
};

/*关闭弹窗*/
function ClosePopup() {
	$('#popupMenuPsw').popup('close');
}