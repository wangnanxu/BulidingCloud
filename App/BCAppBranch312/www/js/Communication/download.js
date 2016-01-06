/*解析地址*/
function AnalyzeFileName(fileurl, type) {
	var fileurls = type != false ? fileurl.split('=') : fileurl.split('/');
	return fileurls[fileurls.length - 1];
};
/*解析数据*/
function AnalyzeFileJson(filejson) {
		var DOWNLOADDIR = new Array("BuildingCloud/image", "BuildingCloud/head", "BuildingCloud/data");
		var _json = {
			URL: "",
			FileName: "",
			FloderDir: "",
			SaveDir: DOWNLOADDIR[1],
			Pramas: "",
			URI: "",
			Callback: null
		}
		for (var key in filejson) {
			if (key == "URL") {
				_json.URL = filejson.URL;
				_json.FileName = AnalyzeFileName(filejson.URL, false);
			} else if (key == "SaveDir") {
				if (filejson.SaveDir < 3 && filejson.SaveDir >= 0) {
					_json.FloderDir = DOWNLOADDIR[parseInt(filejson.SaveDir)];
					_json.SaveDir = _json.FloderDir + "/" + _json.FileName;
				}
			} else if (key == "Pramas") {
				_json.Pramas = filejson.Pramas
			} else if (key == "Callback") {
				if (typeof(filejson.Callback) == "function")
					_json.Callback = filejson.Callback
			}
		}
		return _json;
	}
	/*下载*/
var DownLoad = function(jsondata) {
	var _jsondata = AnalyzeFileJson(jsondata);
	//	return _jsondata.Callback("../img/logo.png", _jsondata.Pramas);
	window.requestFileSystem(LocalFileSystem.PERSISTENT, 0, function(fileSystem) {
		//检测BuildingCloud文件夹是否存在
		fileSystem.root.getDirectory(_jsondata.FloderDir, {
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
				fileSystem.root.getFile(_jsondata.SaveDir, {
					create: true,
					exclusive: false
				}, function(fileEntry) {
					fileEntry.file(function(downloadFile) {
						if (downloadFile.size == 0) {
							var ft = new FileTransfer();
							var URL = encodeURI(_jsondata.URL);
							ft.download(URL, fileEntry.toURI(), function(downloadEntry) {
								_jsondata.Callback(downloadEntry.toURI(), _jsondata.Pramas);
							}, function(error) {
								alert("下载网络图片出现错误" + FileErrorcode(error));
							});
						} else {
							_jsondata.Callback(fileEntry.toURI(), _jsondata.Pramas);
						}
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
};