//上传数据
function UploadData(oData) {
		if (oData == undefined)
			var oData = {
				Method: "Add",
				Name: "刘备123",
				Age: 10,
				Phone: "1871245454 "
			}
		$.post('http://115.29.110.41:82/handler/StudentHandler.ashx', oData, function(oCallBack) {
			alert(oCallBack);
		});
	}
//上传文件 参数
//fileName:文件名
//uploadDIr:参数为0,1,2,代表不同文件路径	 0:BuildingCloud/image  1:BuildingCloud/head  2:BuildingCloud/data 
var UPLOADDIR = new Array("BuildingCloud/image/", "BuildingCloud/head/", "BuildingCloud/data/");
function UploadFile(fileName,uploadDir) {
	if (UPLOADDIR[uploadDir] == undefined) {
		alert("地址参数错误")
		return;
	}
	var fileDir = UPLOADDIR[uploadDir] + fileName;
	window.requestFileSystem(LocalFileSystem.PERSISTENT, 0, FileSystemSuccess, function(error) {
		alert("文件查找失败");
	});

	function FileSystemSuccess(fileSystem) {
		fileSystem.root.getFile(fileDir, {
			create: false,
			exclusive: false
		}, function(fileEntry) {
			_fUpload(fileEntry);
		}, function(error) {
			alert(fileErrorcode(error))
		});
	}

	function _fUpload(fileEntry) {
		var options = new FileUploadOptions();
		fileEntry.file(function(oFile) {
			options.fileKey = "file";
			options.fileName = oFile.name;
			options.mimeType = oFile.type;
			//文件参数
			var params = new Object();
			params.value1 = "test";
			params.value2 = "param";

			options.params = params;
		}, function(error) {
			alert("fileEntry.file:" + fileErrorcode(error));
		});
		var ft = new FileTransfer();
		var path = fileEntry.toURI();

		ft.upload(path, "http://115.29.110.41:82/handler/UploadFile.ashx",
			function(result) {
				alert('Upload success: ' + result.responseCode);
				alert(result.bytesSent + ' bytes sent');
				$("#progressnum").html("100%");
				$("#progressbar").animate({width:"100%"});
				$("#Progressnum").hide();
			},
			function(error) {
				alert('Error uploading file ' + path + ': ' + fileErrorcode(error));
			}, options);
		ft.onprogress = function(progressEvent) {
			if (progressEvent.lengthComputable) {
				$("#Progress").show();
				//已经上传 
				var loaded = progressEvent.sloaded;
				//文件总长度 
				var total = progressEvent.total;
				//计算百分比，用于显示进度条 
				var percent = parseInt((loaded / total) * 100);
				//换算成MB
				loaded = (loaded / 1024 / 1024).toFixed(2);
				total = (total / 1024 / 1024).toFixed(2);
				$("#progressnum").html(percent+"%");
				$("#progressbar").animate({width:percent+"%"});
			}
		};
	}
}