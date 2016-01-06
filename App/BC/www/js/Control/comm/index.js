var Notification = null;
var app = {
	initialize: function() {
		this.bindEvents();
	},
	bindEvents: function() {
		document.addEventListener('deviceready', this.onDeviceReady, false);
	},
	onDeviceReady: function() {
		CreateFolder();
		pictureSource = navigator.camera.PictureSourceType; //相机获取图片
		destinationType = navigator.camera.DestinationType; //相机
		document.addEventListener("backbutton", ButtonBack, false); //返回按钮事件（Android）
		//app.receivedEvent('deviceready');
//				document.addEventListener('touchmove', function(e) {
//					e.preventDefault();
//				}, false);
	}
};
app.initialize();
$(document).on("mobileinit",function(){
	$.support.cors = true;
	$.mobile.allowCrossDomainPages=true;
})
//手机自带返回按钮事件
var ButtonBack = function() {
	switch ($.mobile.activePage.attr("id")) {
		case "Page_Login":
			Quite();
			break;
		case "Page_ShowMessage":
			Quite();
			break;
		case "Page_Project":
			Quite();
			break;
		case "Page_Material":
			Quite();
			break;
		case "Page_UserInformation":
			Quite();
			break;
		case "Page_SendMessage":
			ChangePage('showMessage.html');
			break;
		case "Page_SendSpace":
			ChangePage('showMessage.html?sceneid=' + GetUrlParam("sceneid"));
			break;
		case "Page_Dialog":
			ChangePage('material.html?');
		default:
			break;
	}
}