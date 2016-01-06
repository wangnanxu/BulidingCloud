var pictureSource;
var destinationType;
var bIsMove = false;
var CameraCallBack = null;
/*********************************************************************/
/*
 * 照相
 * 使用方法：
 * var CallBack = function(src){}
 * GetCamera(CallBack);
 */
function GetCamera(CallBack) {
	bIsMove = true;
	CameraCallBack = CallBack;
	navigator.camera.getPicture(OnSuccessPic, function(error) {}, {
		quality: 50,
		destinationType: destinationType.FILE_URI
	});
}
/*
 * 打开相册
 * 使用方法：
 * var CallBack = function(src){}
 * GetCamera(CallBack);
 */
function GetLocalPic(CallBack) {
	bIsMove = false;
	CameraCallBack = CallBack;
	navigator.camera.getPicture(OnSuccessPic, function(error) {}, {
		quality: 50,
		destinationType: destinationType.FILE_URI,
		sourceType: pictureSource.PHOTOLIBRARY
	});
}

/*********************************************************************/
//获取图像成功
function OnSuccessPic(sImageURI) {
	window.resolveLocalFileSystemURI(sImageURI, DealFileEntry, function(error) {
		alert(fileErrorcode(error))
	});
}

//操作图片
function DealFileEntry(fileEntry) {
	var _tDatetime = new Date();
	var _sName = _tDatetime.getFullYear().toString() + (_tDatetime.getMonth() + 1) + _tDatetime.getDate() + _tDatetime.getHours() + _tDatetime.getMinutes() + _tDatetime.getSeconds() + ".jpg"; //新的文件名称
	var _sDirName = "BuildingCloud/image";

	window.requestFileSystem(LocalFileSystem.PERSISTENT, 0,
		function(fileSystem) {
			var direc = fileSystem.root.getDirectory(_sDirName, {
					create: true
				},
				function(parent) {
					if (bIsMove) {
						fileEntry.moveTo(parent, _sName,
							function(oFileEntery) {
								if (typeof(CameraCallBack) == "function")
									CameraCallBack(oFileEntery.toURI());//对保存到本地文件夹的照片处理
							}, function(error) {
								alert("moveTo:" + fileErrorcode(error))
							});
					} else {
						fileEntry.copyTo(parent, _sName,
							function(oFileEntery) {
								if (typeof(CameraCallBack) == "function")
									CameraCallBack(oFileEntery.toURI());//对保存到本地文件夹的照片处理
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