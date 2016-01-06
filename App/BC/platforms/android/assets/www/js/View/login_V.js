//登录页面显示前，检测数据库是否存在唯一码，存在直接登录
var AppVersion = "1.0.5";
var NewVersion = {
	Version: AppVersion,
	Url: "",
	IsNew: false
};
$(document).on("pagebeforeshow", "#Page_Login", function() {
	CheckAccount();
	if (!NewVersion.IsNew) {
		PostGetAppVersion(function(data) {
			NewVersion.Version = data.Version;
			NewVersion.Url = data.Url;
			NewVersion.IsNew = true;
			$(".NewVersion").html(NewVersion.Version);
		})
	}
	FastClick.attach(document.body); //执行快速点击
});
//登录页面去除iOS下清除缓存返回index，导致的标签重复
$(document).on("pageshow", "#Page_Login", function() {
	if ($(".login_input").length > 2) {
		$.each($(".login_input"), function(index, elem) {
			if (index == 0 || index == 1) {
				$(this).remove();
			}
		});
	}

	$(".OldVersion").html(AppVersion);
	$(".NewVersion").html(NewVersion.Version);
});
//获取登录页面的账户和密码 返回参数 json   {account:_saccount,password:_spassword}
function GetLoginData() {
	var _saccount = $("#account").val();
	var _spassword = $("#password").val();
	if (_saccount == null || _saccount == "") {
		NotificationAlert("账号不能为空", "登陆错误");
		return null;
	}
	if (_spassword == null || _spassword == "") {
		NotificationAlert("密码不能为空", "登陆错误");
		return null;
	}
	var _deviceid = CheckPlatform() == true ? "351BBHJ9RRVB11" : device.uuid;
	return {
		UserName: _saccount, //用户名
		Password: _spassword, //用户密码
		DeviceId: _deviceid, //设备ID
		LastTime: ""
			//		Token:""//唯一标示符
	};
}