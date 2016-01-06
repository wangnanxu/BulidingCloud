/*
 * 获取文件路径及父级路径
 * fineName ：文件名
 * callback :回调函数
 * return : json
 *	{
 * 		ownname: _entryName,  			//文件名
 *		parentname: _entryParentName, 	//文件URI路径
 *		ownURI: _entryURI,				//父级文件名
 *		parentURI: _entryParentURI		//父级文件URI路径
 * }
 */
function FlieSystemReader(fileName, callback) {
	if (fileName == undefined || fileName == "" || fileName == null) {
		alert("文件名为空");
		return;
	}
	if (typeof(callback) != "function") {
		alert("回调函数不存在")
		return;
	}

	var FilePath = "BuildingCloud"; //最上级目录
	window.requestFileSystem(LocalFileSystem.PERSISTENT, 0, function(fileSystem) {
		fileSystem.root.getDirectory(FilePath, null, function(DirectoryEntry) {
			RecursiveFileSystem(DirectoryEntry, CallbackDeal);
		}, function(error) {
			alert("打开文件夹失败" + error.code);
		});
	}, function(error) {
		alert("打开文件系统失败");
	});

	var CallbackDeal = function(_entryName, _entryParentName, _entryURI, _entryParentURI) {
		callback({
			ownname: _entryName,
			parentname: _entryParentName,
			ownURI: _entryURI,
			parentURI: _entryParentURI
		});
	}

	function RecursiveFileSystem(Entry, CallbackDeal) {
		if (Entry.isDirectory) {
			var _directoryReader = Entry.createReader();
			_directoryReader.readEntries(_ReadEntriesSuccess, function(error) {
				alert("读取文件列表失败:" + error.code);
			})

			function _ReadEntriesSuccess(entries) {
				for (var i = 0; i < entries.length; i++) {
					if (fileName == entries[i].name && typeof(CallbackDeal) == "function") {
						CallbackDeal(entries[i].name, Entry.name, entries[i].toURI(), Entry.toURI())
					}
					RecursiveFileSystem(entries[i], CallbackDeal);
				}
			}
		}
	}
}