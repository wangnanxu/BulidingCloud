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
 * type:0 图片  1头像 2资料
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
				alert(FileErrorcode(error.code));
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
									//									alert("下载网络图片出现错误:" + FileErrorcode(error));
									Callback(null, pramas);
								});
							} else {
								Callback(fileEntry.toURI(), pramas);
							}
						}, function(error) {
							//							alert(FileErrorcode(error));
						})
					}, function(error) {
						//						alert(FileErrorcode(error));
					});
				} else {
					alert("本地存储失败！");
				}
			}
		}, function(error) {
			alert(FileErrorcode(error));
		});
	}
};

var DownLoadFile = function(fileName, filesize, Callback) {
	if (typeof(Callback) == "function") {
		var fileURI = baseurl + "/Handlers/KnowlegeBase.ashx?fileName=" + fileName;
		var _localURI = DOWNLOADDIR[2] + "/" + fileName;
		window.requestFileSystem(LocalFileSystem.PERSISTENT, 0, function(fileSystem) {
			//检测BuildingCloud文件夹是否存在
			fileSystem.root.getDirectory(DOWNLOADDIR[2], {
				create: true,
				exclusive: false
			}, function() {
				DealFile(true);
			}, function(error) {
				if (error.code == 1) {
					CreateFolder(DealFile);
				}
				alert(FileErrorcode(error));
			});

			function DealFile(isTrue) {
				if (isTrue == true) {
					//检测本地是否存在该文件，存在则返回
					fileSystem.root.getFile(_localURI, {
						create: true,
						exclusive: false
					}, function(fileEntry) {
						fileEntry.file(function(downloadFile) {
							var ft = new FileTransfer();
							var URL = encodeURI(fileURI);
							ft.onprogress = function(progressEvent) {
								//已经上传 
								var loaded = progressEvent.loaded;
								//文件总长度 
								var total = filesize;
								//计算百分比，用于显示进度条 
								var percent = parseInt((loaded / total) * 100);
								//换算成MB
								loaded = (loaded / 1024 / 1024).toFixed(2);
								total = (total / 1024 / 1024).toFixed(2);
								if (fileName == DownloadFile.ID) {
									$('#pro').css('width', percent + '%').html(percent + '%');
								}

							};
							ft.download(URL, fileEntry.toURI(), function(downloadEntry) {
								if (fileName == DownloadFile.ID) {
									$('#pro').css('width', '100%').html('100%');
								}
								Callback(downloadEntry.toURI());
							}, function(error) {
								alert("下载错误:" + FileErrorcode(error));
								Callback(null, pramas);
							});
						}, function(error) {
							alert(FileErrorcode(error));
						})
					}, function(error) {
						alert(FileErrorcode(error));
					});
				} else {
					alert("本地存储失败！");
				}
			}
		}, function(error) {
			alert(FileErrorcode(error));
		});
	}
};