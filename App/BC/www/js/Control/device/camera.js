var pictureSource;
var destinationType;
var bIsMove = false;
var cameraCallBack = null;
var CAMERAPIC = new Array("BuildingCloud/image", "BuildingCloud/head", "BuildingCloud/data");
var CameraID = 0;
/*********************************************************************/
/*
 * 照相
 * 使用方法：
 * var CallBack = function(fileobj){
 * 	fileobj.name 文件名
 * 	fileobj.URI 文件路径
 * }
 * GetCamera(CallBack);
 * 参数  id(0,1,2)  0:
 */

function GetCamera(id, callback) {
	if (id >= 0 || id < CAMERAPIC.length) {
		CameraID = id;
		bIsMove = true;
		cameraCallBack = callback;
		navigator.camera.getPicture(OnSuccessPic, function(error) {}, {
			quality: 30,
			correctOrientation: true,
			destinationType: destinationType.FILE_URI
		});
	} else {
		alert("id参数错误")
		return;
	}
}

/*
 * 打开相册
 * 使用方法：
 * var callback = function(fileobj){
 * 	fileobj.name 文件名
 * 	fileobj.URI 文件路径
 * }
 * GetCamera(callback);
 */

function GetLocalPic(id, callback) {
	if (id >= 0 || id < CAMERAPIC.length) {
		CameraID = id;
		bIsMove = false;
		cameraCallBack = callback;
		navigator.camera.getPicture(OnSuccessPic, function(error) {}, {
			quality: 80,
			targetWidth: 1200,
			targetHeight: 1200,
			destinationType: destinationType.FILE_URI,
			sourceType: pictureSource.PHOTOLIBRARY
		});
	} else {
		alert("id参数错误")
		return;
	}
}

/*********************************************************************/
//获取图像成功
function OnSuccessPic(sImageURI) {
	window.resolveLocalFileSystemURI(sImageURI, DealFileEntry, function(error) {
		alert(fileErrorcode(error))
	});
}

//操作图片
function DealFileEntry(fileentry) {
	var _tdatetime = new Date();
	var _sname = _tdatetime.getFullYear().toString() + (_tdatetime.getMonth() + 1) + _tdatetime.getDate() + _tdatetime.getHours() + _tdatetime.getMinutes() + _tdatetime.getSeconds() + _tdatetime.getMilliseconds() + ".jpg"; //新的文件名称
	var _sdirname = CAMERAPIC[CameraID];

	window.requestFileSystem(LocalFileSystem.PERSISTENT, 0,
		function(fileSystem) {
			var _direc = fileSystem.root.getDirectory(_sdirname, {
					create: true
				},
				function(parent) {
					if (bIsMove) {
						fileentry.moveTo(parent, _sname,
							function(oFileEntery) {
								if (typeof(cameraCallBack) == "function")
									cameraCallBack({
										name: oFileEntery.name,
										URI: oFileEntery.toURI()
									}); //对保存到本地文件夹的照片处理
							}, function(error) {
								alert("moveTo:" + fileErrorcode(error))
							});
					} else {
						fileentry.copyTo(parent, _sname,
							function(oFileEntery) {
								if (typeof(cameraCallBack) == "function")
									cameraCallBack({
										name: oFileEntery.name,
										URI: oFileEntery.toURI()
									}); //对保存到本地文件夹的照片处理
							}, function(error) {
								alert("copyTo:" + fileErrorcode(error))
							});
					}
				}, function(error) {
					alert("getDirectory:" + fileErrorcode(error))
				});
		}, function(error) {
			alert("requestFileSystem:" + fileErrorcode(error))
		});
}