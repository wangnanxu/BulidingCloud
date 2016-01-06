////完整模板
//var ADDSCENE_TEM = "<li id='{MessageID}' class='scenemessage' data-role='none' style='padding:5px'><div class='headimage'><img src='{SendUserHeadURI}' alt=' {SendUserID}' style='border:none' /></div><div class='scenecontent'><div class='fontA'><span>{SendUserName} </span><span>{Time}</span></div><div class='fontB'><span> 地址:</span><span id='Address_{MessageID}'>{Address}</span></div><div><div><span>描述:</span><span class='opclass'>{Content}</span></div><div id='img_{MessageID}' class='contentimage faceul'>{Image}</div><div class='clear'></div></div><div id='Op_{MessageID}' class='contentop'><a href='#' onclick=\"ExamineSceneMessage('{MessageID}')\">[审核]</a><a href='#' onclick=\"CommentSceneMessage('{MessageID}')\">[评论]</a><a href='#' onclick=\"DeleteSceneMessage('{MessageID}')\">[删除]</a><a href='#' onclick=\"ArchiveSceneMessage('{MessageID}')\">[归档]</a></div><div><ul id='Comment_{MessageID}' class='comment'>{CommentContent}</ul></div></div><div class='clear'></div></li>";
////归档模板
//var ADDSCENEG_TEM = "<li id='{MessageID}' class='scenemessage' data-role='none' style='padding:5px'><div class='headimage'><img src='{SendUserHeadURI}' alt=' {SendUserID}' style='border:none'/></div><div class='scenecontent'><div class='fontA'><span>{SendUserName} </span><span>{Time}</span></div><div class='fontB'><span> 地址:</span><span>{Address}</span></div><div><div><span>描述:</span><span class='opclass'>{Content}</span></div><div id='Address_{MessageID}' class='contentimage'>{Image}</div><div class='clear'></div></div><div><ul id='Comment_{MessageID}' class='comment'>{CommentContent}</ul></div></div><div class='clear'></div></li>";
////评论模板
//var ADDSCENELI_TEM = "<li id='{CommentGuid}'><span>{CommentUser}:{CommentContent} </span><span class='commenttime'> {CommentTime}</span><a onclick=\"DeleteOneComment('{MessageID}','{CommentGuid}')\">		删除评论</a></li>";
////图片模板
//var ADDSCENEIMG_TEM = "<img src='{src}'>";
////签到模板
//var ADDSCENESIGN_TEM = "<li id='{MessageID}' class='scenemessage' data-role='none' style='padding:5px' ><div class='headimage'><img src='{SendUserHeadURI}' alt=' {SendUserID}' style='width: 50px;border:none' /></div><div class='scenecontent'><div class='fontA'><span>{SendUserName} </span><span>{Time}</span></div><div><span> {Sign}:</span><span id='Address_{MessageID}'class='fontB'>{Address}</span></div></div><div class='clear'></div></li>";
////加载过程模板
//var LOADIMG = "<div id='load_{MessageID}' class='loadimg' data-role='none'><img src='../img/loadimg.gif'/></div>";
//
////数据组装，返回组装后的字符串
//function AddSqliteTOSceneItem(jsondatas) {
//	var _jsonlength = jsondatas.length;
//	var _arrayTem = [];
//	for (var l = _jsonlength - 1; l >= 0; l--) {
//		jsons = jsondatas[l];
//		var _tem = CheckTem(jsons.Status, jsons.Type);
//		_tem = _tem.replace(/{MessageID}/g, jsons.Id);
//		if (jsons.UserPictureURI == undefined||jsons.UserPictureURI == "undefined") {
//			if (jsons.UserPicture == null || jsons.UserPicture == undefined || jsons.UserPicture == "undefined" || jsons.UserPicture == "") {
//			_tem = _tem.replace(/{SendUserHeadURI}/g, "../img/APP_logo.png");
//			}
//		} else {
//			_tem = _tem.replace(/{SendUserHeadURI}/g, jsons.UserPictureURI);
//		}
//		_tem = _tem.replace(/{SendUserID}/g, jsons.UserID);
//		_tem = _tem.replace(/{SendUserName}/g, jsons.UserName);
//		_tem = _tem.replace(/{Time}/g, jsons.CreateTime);
//		_tem = _tem.replace(/{Address}/g, jsons.Address);
//		_tem = _tem.replace(/{Content}/g, jsons.Description);
//
//		var _imagescontent = "";
//		if (jsons.Images != null) {
//			var _length = jsons.Images.length;
//			var _arrayimg = []
//			for (var i = 0; i < _length; i++) {
//				var _imgtem = ADDSCENEIMG_TEM;
//				_imgtem = _imgtem.replace(/{src}/g, jsons.Images[i]);
//				_arrayimg.push(_imgtem);
//			};
//			_imagescontent = _arrayimg.join("");
//		}
//		_tem = _tem.replace(/{Image}/g, _imagescontent);
//
//		var _commentcontent = "";
//		if (jsons.Comments != null) {
//			var _length = jsons.Comments.length;
//			var _arraycomment = [];
//			for (var i = 0; i < _length; i++) {
//				var _temcomment = ADDSCENELI_TEM;
//				_temcomment = _temcomment.replace(/{CommentGuid}/g, jsons.Comments[i].CommentGuid);
//				_temcomment = _temcomment.replace(/{MessageID}/g, jsons.Id);
//				_temcomment = _temcomment.replace(/{CommentUser}/g, jsons.Comments[i].UserName);
//				_temcomment = _temcomment.replace(/{CommentContent}/g, jsons.Comments[i].Content);
//				_temcomment = _temcomment.replace(/{CommentTime}/g, jsons.Comments[i].Time);
//				_arraycomment.push(_temcomment);
//			}
//			_commentcontent = _arraycomment.join("");
//		}
//		_tem = _tem.replace(/{CommentContent}/g, _commentcontent);
//
//		if (jsons.Type == "1") {
//			_tem = _tem.replace(/{Sign}/g, "签退");
//		} else {
//			_tem = _tem.replace(/{Sign}/g, "签到");
//		}
//		_arrayTem.push(_tem);
//	}
//	return _arrayTem.join("");
//};
////选择模板
//function CheckTem(state, type) {
//	var _state = parseInt(state);
//	var _type = parseInt(type);
//	if (_type == 1 || _type == 32) {
//		return ADDSCENESIGN_TEM; //签到签退
//	} else if (_type != 1 && _type != 32 && _state == 3) {
//		return ADDSCENEG_TEM; //归档模板
//	} else {
//		return ADDSCENE_TEM;
//	}
//};
//////刷新
////function RefreshSceneItem() {};
////RefreshSceneItem.prototype = {
////	Append: function(tem, isrefresh) {
////		var $temp_SceneItem = $("#temp_SceneItem");
////		$temp_SceneItem.append(tem);
////		this.Refresh($temp_SceneItem, isrefresh);
////	},
////	Prepend: function(tem, isrefresh) {
////		var $temp_SceneItem = $("#temp_SceneItem");
////		$temp_SceneItem.prepend(tem);
////		this.Refresh($temp_SceneItem, isrefresh);
////	},
////	Refresh: function($temp_SceneItem, isrefresh) {
////		$temp_SceneItem.listview('refresh');
////		if (isrefresh) {
////			myScroll.refresh();
////		} else {
////			AddPaneScroll("up", PullSceneItem);
////		}
////	}
////}
//
//
//
//
//
//
//
//var SILocalStorage = window.localStorage;
////存储到LocalStorage
//function LocalStorageInit() {
//	var $temp_SceneItem = $("#temp_SceneItem");
//	if (window.localStorage) {
//		$temp_SceneItem.html(SILocalStorage.i);
//	}
//};
////存储到LocalStorage
//
//function SaveLocalStorage() {
//	var $temp_SceneItem = $("#temp_SceneItem");
//	if (window.localStorage) {
//		SILocalStorage.i = $temp_SceneItem.html();
//	}
//}
//
////
////单数据组装
//function AddSqliteTOSceneItemFor(jsons) {
//		var _tem = CheckTem(jsons.Status, jsons.Type);
//		_tem = _tem.replace(/{MessageID}/g, jsons.Id);
//		if (jsons.UserPictureURI == undefined||jsons.UserPictureURI == "undefined") {
//			if (jsons.UserPicture == null) {
//			_tem = _tem.replace(/{SendUserHeadURI}/g, "../img/APP_logo.png");
//			}
//		} else {
//			_tem = _tem.replace(/{SendUserHeadURI}/g, jsons.UserPictureURI);
//		}
//		_tem = _tem.replace(/{SendUserID}/g, jsons.UserID);
//		_tem = _tem.replace(/{SendUserName}/g, jsons.UserName);
//		_tem = _tem.replace(/{Time}/g, jsons.CreateTime);
//		_tem = _tem.replace(/{Address}/g, jsons.Address);
//		_tem = _tem.replace(/{Content}/g, jsons.Description);
//		var _imagescontent = "";
//		if (jsons.Images != null) {
//			var _length = jsons.Images.length;
//			var _arrayimg = []
//			for (var i = 0; i < _length; i++) {
//				var _imgtem = ADDSCENEIMG_TEM;
//				//	/*测试代码*/	jsons.Images[i] = "http://192.168.0.187" + jsons.Images[i].substring(19, jsons.Images[i].length);
//				_imgtem = _imgtem.replace(/{src}/g, jsons.Images[i]);
//				_arrayimg.push(_imgtem);
//			};
//			_imagescontent = _arrayimg.join("");
//		}
//		_tem = _tem.replace(/{Image}/g, _imagescontent);
//
//		var _commentcontent = "";
//		if (jsons.Comments != null) {
//			var _length = jsons.Comments.length;
//			var _arraycomment = [];
//			for (var i = 0; i < _length; i++) {
//				var _temcomment = ADDSCENELI_TEM;
//				_temcomment = _temcomment.replace(/{CommentGuid}/g, jsons.Comments[i].CommentGuid);
//				_temcomment = _temcomment.replace(/{MessageID}/g, jsons.Id);
//				_temcomment = _temcomment.replace(/{CommentUser}/g, jsons.Comments[i].UserName);
//				_temcomment = _temcomment.replace(/{CommentContent}/g, jsons.Comments[i].Content);
//				_temcomment = _temcomment.replace(/{CommentTime}/g, jsons.Comments[i].Time);
//				_arraycomment.push(_temcomment);
//			}
//			_commentcontent = _arraycomment.join("");
//		}
//		_tem = _tem.replace(/{CommentContent}/g, _commentcontent);
//
//		if (jsons.Type == "1") {
//			_tem = _tem.replace(/{Sign}/g, "签退");
//		} else {
//			_tem = _tem.replace(/{Sign}/g, "签到");
//		}
//	return _tem;
//};
