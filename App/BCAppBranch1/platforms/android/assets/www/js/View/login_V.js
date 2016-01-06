
$(document).on("pagebeforeshow", "#Page_Login", function() {
	Check();
});
function GetLoginData() {
	var sAccount = $("#account").val();
	var sPassword = $("#password").val();
	if (sAccount == null || sAccount == "") {
		alert("账号不能为空");
		return null;
	}
	if (sPassword == null || sPassword == "") {
		alert("密码不能为空");
		return null;
	}
	return {
		account: sAccount,
		password: sPassword
	};
}

var GoMessagePage = function() {
	ChangePage('page/Test.html');
//	ChangePage('page/showMessage.html');
}