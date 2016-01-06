//启用相加，保存在image文件夹
function GetByCamera() {
	GetCamera(0, COLCallback);
	//
	//	var fileobj = {
	//		name: "sssdfsdf.jso",
	//		URI: "../img/images/thumb/009.jpg"
	//	};
	//	COLCallback(fileobj);
};
//启用相册，复制在image文件夹

function GetByLocal() {
	GetLocalPic(0, COLCallback);
};
//启动相机相册成功过回调函数

function COLCallback(fileobj) {
	AddOneImgSwipe(fileobj.name.replace(".", ""), fileobj.URI);
};

var ImgJsons = "";
var Description = "";
//添加一条说说完成
function AddComplate(SignType, callback) {
	var _guid = NewGuid();
	var _sceneid = GetUrlParam("sceneid");
	var _time = new Date().secondFormat("yyyy-MM-dd hh:mm:ss"); //本地时间
	var _description = GetDescription();
	var _images = GetImages();
	var _status = 0;
	var _type = $("#phototype").val();
	var _state = 0; //_state 0:待上传，1:上传成功
	var _relation = GetUrlParam("relation");
	var _isexamine = false;
	if (SignType == 1 || SignType == 32) {
		_description = "";
		_images = "";
		_type = SignType;
	}
	dbBase.OpenTransaction(function(tx) {
		dbBase.SaveOrUpdateTable(tx, "tb_SceneMessageComments", ['MessageID', 'SceneID', 'UserID', 'UserPicture', 'UserPictureURI', 'UserName', 'Address', 'CreateTime', 'Description', 'Images', 'Comments', 'Status', 'Type', 'State', 'PictureGuid', 'Relation', 'IsExamine'], [_guid, _sceneid, userInfo.UserID, userInfo.HeadPictureURI, "", userInfo.UserName, "", _time, _description, _images, "", _status, _type, _state, "", _relation, _isexamine], "MessageID", _guid, function(istrue) {
			if (istrue) {
				if (typeof(callback) == 'function') {
					callback(_guid);
				} else {
					ChangePage("sceneItem.html?parentid=" + GetUrlParam("parentid") + "&sceneid=" + GetUrlParam("sceneid") + "&projectid=" + GetUrlParam("projectid"));
				}
			} else {
				alert("发送失败,重新点击发送");
			}
		});
	})
};