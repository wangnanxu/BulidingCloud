//图片模板
var ADDSECENEIMAGE_TEM = "<a href='#{ImageName}' data-rel='popup' data-position-to='window' data-transition='fade'><img class='popphoto' src='{ImageURI}' alt='pic' style='width:100px'></a><div data-role='popup' id='{ImageName}' data-overlay-theme='b' data-theme='b' data-corners='false'><a href='#' data-rel='back' class='ui-btn ui-corner-all ui-shadow ui-btn-a ui-icon-delete ui-btn-icon-notext ui-btn-right'>Close</a><img class='popphoto' src='{ImageURI}' style='max-height:512px;' alt=''> </div>"
	//图片模板
var ADDSECENEIMAGE_TEM_PHOTOSWIPE = "<img id='{ImageName}' src='{ImageURI}' alt='{ImageName}' onclick=OpenPhotoSwipeAdd('{index}') style='width:100px;height:100px' />";

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
//添加一张图片
function AddOneImgSwipe(ImgName, ImgURI) {
	var _tem = ADDSECENEIMAGE_TEM_PHOTOSWIPE;
	_tem = _tem.replace(/{ImageName}/g, ImgName);
	_tem = _tem.replace(/{ImageURI}/g, ImgURI);
	_tem = _tem.replace(/{index}/g, $("#Gallery img").length);
	var $addpic = $("#Gallery");
	$addpic.append(_tem);
};
//获取描述的内容

function GetDescription() {
		var $description = $("#description");
		var _description = $description.val()
		if(_description==""){
			return "内容为空";
		}else{
			return _description;
		}
};
	//获取拍摄的图片

function GetImages() {
	var _imgjsons = [];
	var $a_addpic = $("#Gallery img");
	$.each($a_addpic, function(index, element) {
		var _image = {
			ThumbnailPicture: $(this).attr("src"),
			OriginalPicture: $(this).attr("src")
		}
		_imgjsons[index] = _image;
	})
	return JSON.stringify(_imgjsons);
}

function NewGetImages(messageid) {
	if (!messageid) {
		messageid = "Gallery";
	}
	var $a_addpic = $("#" + messageid + " img");
	var _length = $a_addpic.length;
	var _items = []
	for (var i = 0; i < _length; i++) {
		var img = new Image();
		img.src = $a_addpic[i].src;
		var _item = {
			src: img.src,
			w: img.width,
			h: img.height,
			id: $a_addpic[i].id,
			isOrg:false
		}
		_items.push(_item);
	}
	return _items;
}

//跳转到sceneItem页面，重置全局变量ImgJsons , Description
function GotoSceneItem() {
	ImgJsons = "";
	Description = "";
	ChangePage('sceneItem.html?parentid=' + GetUrlParam("parentid") + "&projectid=" + GetUrlParam("projectid") + "&sceneid=" + GetUrlParam("sceneid"));
};

var gallery;
/*页面加载完成执行*/
$(document).on("pageshow", "#Page_SendSpace", function() {
	if (!CheckPlatform()) {
		$(".removetag").remove();
	}
	var _optype = GetUrlParam("optype");
	if(_optype&&_optype!=""){
		var option =$("#phototype").find("option[value='"+_optype+"']");
		option.attr("selected",true);
		$("#phototype-button span").html(option.html());
		$("#phototype-button").parent("div[class='ui-select']").attr("disabled","disabled");
		var li =$("#phototype-menu li>a");
		li.removeClass("ui-btn-active");
		$("#phototype-menu").find("li[data-option-index='"+option.attr("tabindex")+"']").children('a').addClass("ui-btn-active");
	}
	//	var items = [];
});

function OpenPhotoSwipeAdd(_index) {
	var pswpElement = document.querySelectorAll('.pswp')[0];

	var items = NewGetImages();

	// define options (if needed)
	var options = {
		// history & focus options are disabled on CodePen        
		history: false,
		focus: false,

		showAnimationDuration: 0,
		hideAnimationDuration: 0

	};

	gallery = new PhotoSwipe(pswpElement, PhotoSwipeUI_Default, items, options);
	gallery.init();
	gallery.goTo(parseInt(_index));
	gallery.listen('shareLinkClick', function(e, target) {
		DeletePictureAdd();
	});
};

function DeletePictureAdd() {
	var _index = gallery.getCurrentIndex();
	var _id = gallery.items[_index].id;
	gallery.items.splice(_index, 1);
	$("#" + _id).remove();
	if (gallery.items.length > 0) {
		gallery.invalidateCurrItems();
		gallery.updateSize(true);
	} else {
		gallery.close();
	}
	gallery.goTo(gallery.getCurrentIndex());
}