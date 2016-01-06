var UPLOADDIR = new Array("BuildingCloud/image/", "BuildingCloud/head/", "BuildingCloud/data/");
var UPLOADAPIS = new Array("/APIS/SceneData/UploadImage", "/APIS/Account/SetUserPicture");
/*
 * 上传文件 参数
 * @filename:文件名
 * @uploaddir:参数为0,1,2,代表不同文件路径	 0:BuildingCloud/image  1:BuildingCloud/head  2:BuildingCloud/data
 */
var UploadFile = function(jsondata) {
	var _jsondata = AnalyzeUploadFileJson(jsondata);
	window.requestFileSystem(LocalFileSystem.PERSISTENT, 0, FileSystemSuccess, function(error) {
		alert("文件系统创建失败");
	});
	//支持文件系统
	//@filesystem文件系统
	function FileSystemSuccess(filesystem) {
			filesystem.root.getFile(_jsondata.FileDir, {
				create: false,
				exclusive: false
			}, function(fileentry) {
				Upload(fileentry);
			}, function(error) {
				alert(fileErrorcode(error))
			});
		}
		//上传
		//@fileentry文件地址

	function Upload(fileentry) {
		var options = new FileUploadOptions();
		fileentry.file(function(oFile) {
			options.fileKey = "file";
			options.fileName = oFile.name;
			options.mimeType = oFile.type;
			//文件参数
			options.params = _jsondata.UploadPramas;
			var ft = new FileTransfer();
			var _path = fileentry.toURI();
			ft.upload(_path, _jsondata.URL,
				function(result) {
					var _response = JSON.parse(result.response);
					if (result.responseCode == 200 && _response && _response.Success) {
							_jsondata.Callback(_response,_jsondata.Pramas);
					} else {
						alert("上传图片失败");
					}
				},
				function(error) {
					alert('Error uploading file ' + _path + ': ' + fileErrorcode(error));
				}, options, true);

		}, function(error) {
			alert("fileentry.file:" + fileErrorcode(error));
		});
	}
};
/*解析地址*/
function AnalyzeLocalFileName(fileurl, num) {
		var _fileurls = fileurl.split('/');
		return _fileurls[_fileurls.length - 1];
	}
	/*解析数据*/

function AnalyzeUploadFileJson(filejson) {
	var _json = {
		FileDir: "",
		URL: "",
		Pramas: "",
		UploadPramas: {},
		Callback: null
	}
	for (var key in filejson) {
		if (key == "Type") {
			if (filejson.Type == 0) {
				_json.FileDir = UPLOADDIR[0] + AnalyzeLocalFileName(filejson.URI, 0);
				_json.URL = baseurl + UPLOADAPIS[0];
			} else if (filejson.Type == 1) {
				_json.FileDir = UPLOADDIR[1] + AnalyzeLocalFileName(filejson.URI, 1);
				_json.URL = baseurl + UPLOADAPIS[1];
			}
		} else if (key == "Pramas") {
			_json.Pramas = filejson.Pramas
		} else if (key == "UploadPramas") {
			_json.UploadPramas = filejson.UploadPramas;
			_json.UploadPramas["FileName"] = AnalyzeLocalFileName(filejson.URI, 0);
		} else if (key == "Callback") {
			if (typeof(filejson.Callback) == "function")
				_json.Callback = filejson.Callback
		}
	}
	return _json;
}