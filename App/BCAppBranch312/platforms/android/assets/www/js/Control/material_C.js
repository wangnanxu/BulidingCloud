//获取所有已下载资料

function SelectDownloadMaterial(callback) {
	var _sql = "select * from tb_Materials where EnterpriseID = ?";
	dbBase.OpenTransaction(function(tx) {
		dbBase.SelectTable(tx, _sql, [userInfo.EnterpriseID], function(data) {
			if (data) {
				var data = ChangeSelectToJsondata(data)
				if (typeof(callback) == "function") {
					callback(data);
				}
				if ($.isEmptyObject(localMaterial)) {
					var _length = data.length;
					for (var i = 0; i < _length; i++) {
						localMaterial[data[i].ID] = data[i].UpdateTime;
					}
				}
			}
		})
	})
};

var localMaterial = {};
var deleteMaterial = {};

function SetDeleteMaterial(ID) {
	deleteMaterial.ID = ID;
}

function DeleteMaterial(IsDownload) {
	$('#purchase').popup('close');
	dbBase.OpenTransaction(function(tx) {
		dbBase.DeleteTable(tx, "tb_Materials", "ID=?", [deleteMaterial.ID], function(data) {
			$("#" + deleteMaterial.ID.replace(".", "")).remove();
			myScroll.refresh();
			localMaterial[deleteMaterial.ID] = false;
		})
	})
};

function DownloadMaterial() {
	DownLoadFile(DownloadFile.ID,DownloadFile.DocumentSize, function(fileurl) {
		if (fileurl) {
			dbBase.OpenTransaction(function(tx) {
				dbBase.SaveOrUpdateTable(tx, "tb_Materials", ['ID', 'Name', 'KnowledgeType', 'DocumentType', 'EnterpriseID', 'UpdateTime', 'DocumentSize'], [DownloadFile.ID, DownloadFile.Name, DownloadFile.KnowledgeType, DownloadFile.DocumentType, DownloadFile.EnterpriseID, DownloadFile.UpdateTime, DownloadFile.DocumentSize], "ID", DownloadFile.ID, function(istrue) {
					localMaterial[DownloadFile.ID]=DownloadFile.UpdateTime;
					$("#btndown").hide();
					$("#btnopen").show();
				})
			})
		} else {
			alert("下载文件失败");
		}
	})

};
var AllMaterial = [];
var DownloadFile = {};

function GoDownload(i) {
	DownloadFile = AllMaterial[parseInt(i)];
	var sceneid = GetUrlParam("sceneid");
	if (sceneid) {
		ChangePage('downloadscene.html?parentid=' + GetUrlParam("parentid") + "&projectid=" + GetUrlParam("projectid") + "&sceneid=" + GetUrlParam("sceneid"));
	} else {
		ChangePage('download.html');
	}
};


function OpenApp(MaterialName) {

	function success() {
		
	}

	function error(code) {
		if (code === 1) {
			//			alert('文件');
		} else {
			//			alert('Undefined error');
		}
	}
	if(!MaterialName){
		MaterialName=DownloadFile.ID;
	}
	GetDataFile(MaterialName, function(fileurl) {
		var openDataFile = cordova.plugins.disusered.open;
		openDataFile(decodeURI(fileurl), success, error);
	})
}