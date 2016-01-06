var MATERIAL_TEM = "<li data-icon='gear' id='{MaterialID}'><a onclick=OpenApp('{ID}')><img src='{src}'><h2>{MaterialName}</h2><p>更新时间：{UpdateTime}</p><p>大小：{Size}</p></a><a href='#purchase' data-rel='popup' data-position-to='origin' data-transition='pop' onclick=SetDeleteMaterial('{ID}')>delete</a></li>"
var MATERIALALL_TEM = "<li id='{MaterialID}'><a><img src='{src}'><h2>{MaterialName}</h2><p>更新时间：{UpdateTime}</p><p>大小：{Size}</p></a><a onclick=GoDownload({Material})>delete</a></li>"
	//文件类型图片
var TYPEJSON = {
	1: "../img/excel.gif",
	2: "../img/word.png",
	3: "../img/ppt.jpg",
	4: "../img/pdf.png"
};
$(document).on("pageshow", "#Page_Material", function() {
	SelectDownloadMaterial(ShowMaterial);
});

function PullMaterial() {

}

function ShowMaterial(data, idsll) {
	var _tems = [];
	if (data) {
		var _length = data.length;
		for (var i = 0; i < _length; i++) {
			var _tem = MATERIAL_TEM;
			if (idsll) {
				_tem = MATERIALALL_TEM;
				AllMaterial = data;
			}
			_tem = _tem.replace(/{MaterialName}/g, data[i].Name);
			_tem = _tem.replace(/{UpdateTime}/g, data[i].UpdateTime);
			_tem = _tem.replace(/{Size}/g, data[i].DocumentSize);
			_tem = _tem.replace(/{ID}/g, data[i].ID);
			_tem = _tem.replace(/{MaterialID}/g, data[i].ID.replace(".",""));
			_tem = _tem.replace(/{src}/g, TYPEJSON[data[i].DocumentType]);
			_tem = _tem.replace(/{Material}/g, i);
			_tems.push(_tem);
		}
	}
	$("#materialul").html(_tems.join("")).listview('refresh');
	AddPaneScroll("down", PullMaterial);
};

var initDownload = false;

$(document).on("pageshow", "#Page_MaterialAll", function() {
	if (!initDownload) {
		PostGetMaterial(ShowMaterial);
		initDownload = true;
	}
	else{
		ShowMaterial(AllMaterial,true)
	}
});
$(document).on("pageshow", "#Page_Download", function() {
	if(!localMaterial[DownloadFile.ID]){
		$("#btndown").show();
	}
	else{
		$("#btnopen").show();
		var date1 = new Date(localMaterial[DownloadFile.ID]);
		var date2 = new Date(DownloadFile.UpdateTime);
		var datec = date2-date1;
		if(date2>date1){
			$("#btnnew").show();
		}
	}
	$("#downloadimg").attr("src", TYPEJSON[DownloadFile.DocumentType]);
	$("#downloadMaterialName").html(DownloadFile.Name);
	$("#downloadSize").html(DownloadFile.DocumentSize);
});

function BackToSceneItem() {
	ChangePage('sceneItem.html?parentid=' + GetUrlParam("parentid") + "&projectid=" + GetUrlParam("projectid") + "&sceneid=" + GetUrlParam("sceneid"));
};
function GotoMaterialAllScene(){
	ChangePage('materiallscene.html?parentid=' + GetUrlParam("parentid") + "&projectid=" + GetUrlParam("projectid") + "&sceneid=" + GetUrlParam("sceneid"));
}
