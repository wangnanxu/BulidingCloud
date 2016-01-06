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
		document.addEventListener("backbutton", fBack, false); //返回按钮事件（Android）
		//		document.addEventListener('touchmove', function(e) {
		//			e.preventDefault();
		//		}, false);
	}
};
app.initialize();

var fBack = function() {
	switch ($.mobile.activePage.attr("id")) {
		case "Page_Login":
			break;
		case "Page_ShowMessage":
			break;
		case "Page_Project":
			break;
		case "Page_Material":
			break;
		case "Page_UserInformation":
			break;
		case "Page_Send":
			ChangePage('showMessage.html');
			break;
		default:
			break;
	}
}