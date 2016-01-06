/*解析地址*/
function AnalyzeIMGName(fileurl, type) {
	var fileurls = type == 0 ? fileurl.split('=') : fileurl.split('/');
	return DOWNLOADDIR[type] + "/" + fileurls[fileurls.length - 1];
};
/*保存路径*/
var DOWNLOADDIR = new Array("BuildingCloud/image", "BuildingCloud/head", "BuildingCloud/data");
/*
 * 下载
 * fileURL:文件路径
 * type:0 图片  1头像
 * pramas：过渡参数
 * Callback:回调函数
 */

var DownLoad_New = function(fileURL, type, pramas, Callback) {
	if (typeof(Callback) == "function") {
		var _fileURI = AnalyzeIMGName(fileURL, type);
		window.requestFileSystem(LocalFileSystem.PERSISTENT, 0, function(fileSystem) {
			//检测BuildingCloud文件夹是否存在
			fileSystem.root.getDirectory(DOWNLOADDIR[type], {
				create: true,
				exclusive: false
			}, function() {
				DealFile(true);
			}, function(error) {
				if (error.code == 1) {
					CreateFolder(DealFile);
				}
				alert(fileErrorcode(error.code));
			});

			function DealFile(isTrue) {
				if (isTrue == true) {
					//检测本地是否存在该文件，存在则返回
					fileSystem.root.getFile(_fileURI, {
						create: true,
						exclusive: false
					}, function(fileEntry) {
						fileEntry.file(function(downloadFile) {
							if (downloadFile.size == 0) {
								var ft = new FileTransfer();
								var URL = encodeURI(fileURL);
								ft.download(URL, fileEntry.toURI(), function(downloadEntry) {
									Callback(downloadEntry.toURI(), pramas);
								}, function(error) {
									alert("下载网络图片出现错误" + fileErrorcode(error.code));
								});
							} else {
								Callback(fileEntry.toURI(), pramas);
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
};