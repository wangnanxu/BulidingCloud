$(document).on("pageshow", "#Page_UserInformation", function() {
	ShowUserInformation();
});

function ShowUserInformation() {
	$("#useraccount").html(userInfo.UserID);
	$("#username").html(userInfo.UserName);
	$("#headimg").attr("src", userInfo.HeadPictureURI);
}


function SetImgSrc() {
	$("#headimg").attr("src", userInfo.HeadPictureURI);
}

function GetHeadByCamera() {
	GetCamera(1, CameraOrLoaclCallback);
}

function GetHeadByLocal() {
	GetLocalPic(1, CameraOrLoaclCallback);
}

