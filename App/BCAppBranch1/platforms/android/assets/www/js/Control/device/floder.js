//创建文件夹
function CreateFolder(fCallBack) {
	window.requestFileSystem(LocalFileSystem.PERSISTENT, 0, CreateFloderSuccess, CreateFloderFailed);
	//请求文件系统成功
	function CreateFloderSuccess(fileSystem) {
		//创建文件路径
		var _sFloderDir = new Array("BuildingCloud","BuildingCloud/image","BuildingCloud/head","BuildingCloud/data");
		var _nFlag = 0
		for (var i = 0; i < _sFloderDir.length; i++) {
			fileSystem.root.getDirectory(_sFloderDir[i], {
				create: true,
				exclusive: false
			}, function(DirectoryEntry) {
				_nFlag += 1;
				if (_nFlag==_sFloderDir.length&&typeof(fCallBack)=="function"){
					fCallBack(true);
				}
			}, function(error) {
				alert("创建 " + _sFloderDir[i] + " error：" + fileErrorcode(error));
			});
		}
	}

	//请求文件系统失败
	function CreateFloderFailed(error) {
		alert("创建文件夹 失败" + fileErrorcode(error));
	}
}