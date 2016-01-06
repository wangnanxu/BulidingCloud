/*下载文件 
 * fileName：文件名  
 * fCallback:回调函数，参数为文件路径
 * saveDir :文件保存路径，参数  0:BuildingCloud/image  1:BuildingCloud/head  2:BuildingCloud/data
 * 示例：
 * 回调函数为下载保存后的文件路径
 * function fCallback(src){
 * 	alert(src);
 * }
 * Download("image_2015-05-06-17-29-41.jpg", fCallback,1);
 */
var DOWNLOADDIR = new Array("BuildingCloud/image", "BuildingCloud/head", "BuildingCloud/data");

function Download(fileName, fCallback, saveDir) {
	var fileURL = "http://115.29.110.41:82/Content/Files/" + fileName;
	if (DOWNLOADDIR[saveDir] == undefined) {
		alert("地址参数错误")
		return;
	}
	var folderDir = DOWNLOADDIR[saveDir];
	window.requestFileSystem(LocalFileSystem.PERSISTENT, 0, function(fileSystem) {
		//检测BuildingCloud文件夹是否存在
		fileSystem.root.getDirectory(folderDir, {
			create: false,
			exclusive: false
		}, function() {
			_dealFile(true);
		}, function(error) {
			if (error.code == 1) {
				CreateFolder(_dealFile);
			}
			alert(fileErrorcode(error.code));
		});

		function _dealFile(isTrue) {
			if (isTrue == true) {
				//检测本地是否存在该文件，存在则返回
				var fileDir = folderDir + "/" + fileName;
				fileSystem.root.getFile(fileDir, {
					create: true,
					exclusive: false
				}, function(fileEntry) {
					fileEntry.file(function(downloadFile) {
						if (downloadFile.size == 0) {
							var ft = new FileTransfer();
							var URL = encodeURI(fileURL);
							ft.download(URL, fileEntry.toURI(), function(downloadEntry) {
								if (typeof(fCallback) == "function") {
									fCallback(downloadEntry.toURI());
								}
							}, function(error) {
								alert("下载网络图片出现错误" + fileErrorcode(error.code));
							});
						} else {
							if (typeof(fCallback) == "function") {
								fCallback(fileEntry.toURI());
							}
						}
					}, function(error) {
						alert(fileErrorcode(error.code));
					})
				}, function(error) {
					alert(fileErrorcode(error.code));
				});
			} else {
				alert("本地存储失败！");
			}
		}
	}, function(error) {
		alert(fileErrorcode(error.code));
	});
}