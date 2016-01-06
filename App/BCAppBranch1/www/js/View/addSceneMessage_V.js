//图片模板
var ADDSECENEIMAGE_TEM = "<a href='#{ImageName}' data-rel='popup' data-position-to='window' data-transition='fade'><img class='popphoto' src='{ImageURI}' alt='pic' style='width:100px'></a><div data-role='popup' id='{ImageName}' data-overlay-theme='b' data-theme='b' data-corners='false'><a href='#' data-rel='back' class='ui-btn ui-corner-all ui-shadow ui-btn-a ui-icon-delete ui-btn-icon-notext ui-btn-right'>Close</a><img class='popphoto' src='{ImageURI}' style='max-height:512px;' alt=''> </div>"

//添加一张图片
function AddOneImg(ImgName, ImgURI) {
	var _tem = ADDSECENEIMAGE_TEM;
	_tem = _tem.replace(/{ImageName}/g, ImgName);
	_tem = _tem.replace(/{ImageURI}/g, ImgURI);
	var $addpic = $("#a_addpic");
	$addpic.before(_tem);
	var $div_addpic = $("#div_addpic");
	$div_addpic.trigger('create');
};
//获取描述的内容

function GetDescription() {
		var $description = $("#description");
		return $description.val();
	}
	//获取拍摄的图片

function GetImages() {
	var _imgjsons = [];
	var $a_addpic = $("#a_addpic").siblings("a");
	$.each($a_addpic, function(index, element) {
		var _image = {
			ThumbnailPicture: $(this).children("img").attr("src"),
			OriginalPicture: $(this).children("img").attr("src")
		}
		_imgjsons[index] = _image;
	})
	return JSON.stringify(_imgjsons);
}

//跳转到sceneItem页面，重置全局变量ImgJsons , Description
function GotoSceneItem() {
	ImgJsons = "";
	Description = "";
	ChangePage('sceneItem.html?parentid=' + GetUrlParam("parentid") + "&projectid=" + GetUrlParam("projectid") + "&sceneid=" + GetUrlParam("sceneid"));
};

/*页面加载完成执行*/
$(document).on("pageshow", "#Page_SendSpace", function() {
	if (!CheckPlatform()) {
		$(".removetag").remove();
	}
});