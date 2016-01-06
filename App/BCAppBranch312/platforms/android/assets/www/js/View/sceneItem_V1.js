//完整模板
var SCENEITEM_TEM = "<li id='{MessageID}' class='scenemessage' data-role='none' style='padding:0'><div class='sceneclass'><div class='headimage'><div class='headimagediv'><img src='' onerror='this.src=\"../img/head.jpg\"' id='Head_{MessageID}' onload=\"LoadPictureNew('{SendUserHeadURI}','Head_{MessageID}')\"  style='border:none' /></div></div><div class='scenecontent'><div class='scenecontentdiv'><div class='fontA fontline'><span>{SendUserName} </span><span>{Time}</span></div><div class='fontB fontline'><span> 地址:</span><span id='Address_{MessageID}'>{Address}</span></div><div><span> 类型:</span><span class='fontB fontline'>{Sign}</span></div><div><span> 状态:</span><span class='fontB fontline'>{Status}</span></div><div><div class='fontline'><span>描述:</span><span class='opclass'>{Content}</span></div><div id='img_{MessageID}' class='contentimage faceul'><ul id='Gallery_{MessageID}' class='gallery'>{Image}</ul></div></div><div class='clear'></div></div><div id='Op_{MessageID}' class='contentop'>{OPTION}</div><div><ul id='Comment_{MessageID}' class='comment'>{CommentContent}</ul></div></div></div><div class='clear'></div></div></li>";
//图片模板
var ADDSCENEIMG_TEM = "<div><a id='{imgid}' onclick=\"LoadPicture('{imgid}','{orgimgurl}',{IsNew},true);return false;\"  href='{orgimgurl}' class='swipebox'><img src='{localthumig}' onerror='this.src=\"../img/404.jpg\"' id='loadpicture_{imgid}' onload=\"LoadPicture('loadpicture_{imgid}','{localthumig}',{IsNew})\" alt='image'></a></div>";
//弹出单张图片
var ONE_ADDSCENEIMG_TEM = "<a href='#{ImageName}' data-rel='popup'  data-position-to='window' data-transition='fade'><img class='popphoto' id='thu_{ImageName}' src='../img/loadimg.gif' onload=\"LoadPictureNew('{ImageURI}','thu_{ImageName}');return false;\" onerror='this.src=\"../img/loadimg.gif\"' alt='pic'></a><div data-role='popup' id='{ImageName}' data-overlay-theme='b' data-theme='b' data-corners='false'><a href='#' data-rel='back' class='ui-btn ui-corner-all ui-shadow ui-btn-a ui-icon-delete ui-btn-icon-notext ui-btn-right'>Close</a><img class='popphoto' id='org_{ImageName}' src='../img/loadimg.gif'  onload=\"LoadPictureNew('{ImageOrgURI}','org_{ImageName}');return false;\" onerror='this.src=\"../img/loadimg.gif\"'  style='max-height:512px;' alt=''> </div>";
//签到模板
var ADDSCENESIGN_TEM = "<li id='{MessageID}' class='scenemessage' data-role='none' style='padding:0' ><div class='sceneclass'><div class='headimage'><div class='headimagediv'><img src='' onerror='this.src=\"../img/head.jpg\"' id='Head_{MessageID}' onload=\"LoadPictureNew('{SendUserHeadURI}','Head_{MessageID}')\"/></div></div><div class='scenecontent'><div class='scenecontentdiv'><div class='fontA fontline'><span>{SendUserName} </span><span>{Time}</span></div><div><span> 地址:</span><span id='Address_{MessageID}'class='fontB fontline'>{Address}</span></div><div><span> 类型:</span><span class='fontB fontline'>{Sign}</span></div></div></div><div class='clear'></div></div></li>";
//加载过程模板
var LOADIMG = "<div id='load_{MessageID}' class='loadimg' data-role='none'><img src='../img/loadimg.gif'/></div>";
//归档模板
var ADDSCENEG_TEM = "<li id='{MessageID}' class='scenemessage' data-role='none' style='padding:0'><div class='sceneclass'><div class='headimage'><div class='headimagediv'><img src='{SendUserHeadURI}' onerror='this.src=\"../img/head.jpg\"' id='Head_{MessageID}' onload=\"LoadPicture('{SendUserHeadURI}','Head_{MessageID}')\"  style='border:none'/></div></div><div class='scenecontent'><div class='scenecontentdiv'><div class='fontA fontline'><span>{SendUserName} </span><span>{Time}</span></div><div class='fontB fontline'><span> 地址:</span><span>{Address}</span></div><div><span> 类型:</span><span class='fontB fontline'>{Sign}</span></div><div><span> 状态:</span><span class='fontB fontline'>{Status}</span></div><div><div class='fontline'><span>描述:</span><span class='opclass'>{Content}</span></div><div id='img_{MessageID}' class='contentimage'><ul id='Gallery_{MessageID}' class='gallery'>{Image}</ul></div><div class='clear'></div></div><div><ul id='Comment_{MessageID}' class='comment'>{CommentContent}</ul></div></div></div><div class='clear'></div></div></li>";
//评论模板
var ADDSCENELI_TEM = "<li id='{CommentGuid}'><span>{CommentUser}:{CommentContent} {CommentUpdate}</span><span class='commenttime'> {CommentTime}</span>{DeleteText}</li>";

var SWIPE_ADDSCENEIMG_TEM = "<li id='{ImageName}'><a id='org_{ImageName}' href='../img/photoloader.gif'><img id='thu_{ImageName}' src='../img/loadimg.gif' alt='{ImageOrgURI};org_{ImageName};{MessageID}' onclick=\"bindPhotoSwipe('{MessageID}')\" onload=\"LoadPictureNew('{ImageURI}','thu_{ImageName}');return false;\" onerror='this.src=\"../img/loadimg.gif\"' style='width:100px;height:100px' /></a></li>";
var PHOTOSWIPE_ADDSCENEIMG_TEM = "<img id='thu_{ImageName}' src='../img/loadimg.gif' alt='{ImageOrgURI}' onclick=openPhotoSwipe('{index}','Gallery_{MessageID}') onload=\"LoadPictureNew('{ImageURI}','thu_{ImageName}');return false;\" onerror='this.src=\"../img/loadimg.gif\"' style='width:100px;height:100px' />";
/*显示除评论和头像外的数据
 * jsondata:json数据
 * temid：模板id
 */
function ShowData(jsondatas, islocal) {
	var _length = jsondatas.length;
	var _arrayTem = [];
	for (var i = _length - 1; i >= 0; i--) {
		var jsondata = jsondatas[i];
		var _tem = CheckTem(jsondata.Status, jsondata.Type, jsondata.State);
		if (_tem != "") {
			var messageid = "";
			if (islocal) {
				messageid = jsondata.MessageID;
			} else {
				messageid = jsondata.Id;
			}
			_tem = _tem.replace(/{MessageID}/g, messageid);
			_tem = _tem.replace(/{SendUserHeadURI}/g, jsondata.UserPicture);
			if (jsondata.UserPicture.substr(0, 7) == "http://") {
				_tem = _tem.replace(/{IsNew}/g, false);
			} else {
				_tem = _tem.replace(/{IsNew}/g, true);
			}
			//			_tem = _tem.replace(/{SendUserID}/g, jsondata.UserID);
			_tem = _tem.replace(/{SendUserName}/g, jsondata.UserName);
			_tem = _tem.replace(/{Time}/g, jsondata.CreateTime);
			_tem = _tem.replace(/{Address}/g, jsondata.Address);
			var _description = jsondata.Description == null ? "" : jsondata.Description;
			_tem = _tem.replace(/{Content}/g, _description);
			_tem = _tem.replace(/{OPTION}/g, ShowOption(messageid, jsondata.UserID, jsondata.Type, jsondata.Status, jsondata.IsExamine)); //组装按钮
			_tem = _tem.replace(/{Image}/g, ShowImages(jsondata, islocal)); //组装图片
			_tem = _tem.replace(/{CommentContent}/g, ShowComment(jsondata, messageid, islocal, jsondata.Type)); //Messageid api不同 组装评论

			if (jsondata.Type == "1") {
				_tem = _tem.replace(/{Sign}/g, "签到");
			} else if (jsondata.Type == "2") {
				_tem = _tem.replace(/{Sign}/g, "过程照");
			} else if (jsondata.Type == "4") {
				_tem = _tem.replace(/{Sign}/g, "安全照");
			} else if (jsondata.Type == "8") {
				_tem = _tem.replace(/{Sign}/g, "临检照");
			} else if (jsondata.Type == "16") {
				_tem = _tem.replace(/{Sign}/g, "交底照");
			} else if (jsondata.Type == "32") {
				_tem = _tem.replace(/{Sign}/g, "签退");
			} else if (jsondata.Type == "64") {
				_tem = _tem.replace(/{Sign}/g, "完工照");
			} else {
				_tem = _tem.replace(/{Sign}/g, ""); {
					Status
				}
			}

			if (jsondata.Status == "0") {
				_tem = _tem.replace(/{Status}/g, "正常状态");
			} else if (jsondata.Status == "1") {
				_tem = _tem.replace(/{Status}/g, "审核通过");
			} else if (jsondata.Status == "2" && (jsondata.IsExamine == "false" || !jsondata.IsExamine)) {
				_tem = _tem.replace(/{Status}/g, "需要整改");
			} else if (jsondata.Status == "2" && (jsondata.IsExamine == "true" || jsondata.IsExamine)) {
				_tem = _tem.replace(/{Status}/g, "已整改");
			} else if (jsondata.Status == "3") {
				_tem = _tem.replace(/{Status}/g, "已归档");
			}
		}
		_arrayTem.push(_tem);
	}
	return _arrayTem.join("");
};
/*选择模板*/
function CheckTem(status, type, state) {
	var _status = parseInt(state);
	if (GetUrlParam("state") != "3") {
		if (state != 4) {
			var _status = parseInt(status);
			var _type = parseInt(type);
			if (_type == 1 || _type == 32) {
				return ADDSCENESIGN_TEM; //签到签退
			} else if (_type != 1 && _type != 32 && _status == 3) {
				return ADDSCENEG_TEM; //归档模板
			} else {
				return SCENEITEM_TEM;
			}
		} else {
			return "";
		}
	} else {
		return ADDSCENEG_TEM; //归档模板
	}
};

//刷新
function RefreshSceneItem() {};
RefreshSceneItem.prototype = {
	Append: function(tem, isrefresh) {
		var $temp_SceneItem = $("#temp_SceneItem");
		$temp_SceneItem.append(tem);
		this.Refresh($temp_SceneItem, isrefresh);
	},
	Prepend: function(tem, isrefresh) {
		var $temp_SceneItem = $("#temp_SceneItem");
		$temp_SceneItem.prepend(tem);
		this.Refresh($temp_SceneItem, isrefresh);
	},
	Refresh: function($temp_SceneItem, isrefresh) {
		$temp_SceneItem.listview('refresh');
		$(".contentimage").trigger('create');
		if (isrefresh) {
			myScroll.refresh();
		} else {
			AddPaneScroll("up", PullSceneItem);
		}
	}
};

function ShowOption(messageid, userid, type, status, isexamine) {
	if (CheckPermission(false)) {
		var _optemarray = [];
		var _optem = "";
		if (userInfo.VerifySceneData) {
			_optem = "<a href='#' onclick=ExamineSceneMessage('{MessageID}','" + type + "')>[审核]</a>";
			_optemarray.push(_optem.replace(/{MessageID}/g, messageid));
		}
		_optem = "<a href='#' onclick=CommentSceneMessage('{MessageID}')>[评论]</a>";
		_optemarray.push(_optem.replace(/{MessageID}/g, messageid));
		if (userInfo.UserID == userid) {
			_optem = "<a href='#' onclick=DeleteSceneMessage('{MessageID}')>[删除]</a>";
			_optemarray.push(_optem.replace(/{MessageID}/g, messageid));
		}
		if (userInfo.ArchiveSceneData) {
			var _optem = "<a href='#' onclick=ArchiveSceneMessage('{MessageID}','{Status}','{IsExamine}')>[归档]</a>";
			_optem = _optem.replace(/{MessageID}/g, messageid);
			_optem = _optem.replace(/{Status}/g, status);
			_optem = _optem.replace(/{IsExamine}/g, isexamine);
			_optemarray.push(_optem.replace(/{MessageID}/g, messageid));
		}
		return _optemarray.join("");
	} else {
		return "";
	}
};
//解析要显示的图片，返回模板字符串

function ShowImages(jsondata, local) {
	var _imagescontent = [];
	if (jsondata.Images != "" && jsondata.Images != null) {
		var _arrayimg;
		if (local == true) {
			_arrayimg = JSON.parse(jsondata.Images);
		} else {
			_arrayimg = jsondata.Images;
		}
		var _length = _arrayimg.length;
		for (var i = 0; i < _length; i++) {
			//			var _imgtem = ONE_ADDSCENEIMG_TEM;
			//			var _imgtem = SWIPE_ADDSCENEIMG_TEM;
			var _imgtem = PHOTOSWIPE_ADDSCENEIMG_TEM;
			if (local == true) {
				_imgtem = _imgtem.replace(/{ImageName}/g, jsondata.MessageID + i);
				_imgtem = _imgtem.replace(/{MessageID}/g, jsondata.MessageID);
			} else {
				_imgtem = _imgtem.replace(/{ImageName}/g, jsondata.Id + i);
				_imgtem = _imgtem.replace(/{MessageID}/g, jsondata.Id);
			}
			_imgtem = _imgtem.replace(/{index}/g, i);
			_imgtem = _imgtem.replace(/{ImageURI}/g, _arrayimg[i].ThumbnailPicture);
			_imgtem = _imgtem.replace(/{ImageOrgURI}/g, _arrayimg[i].OriginalPicture);
			_imagescontent.push(_imgtem);
		};
	}
	return _imagescontent.join('');
};
//解析要显示的评论，返回模板字符串
function ShowComment(jsondata, messageid, islocal, type) {
	var _commentcontent = "";
	if (jsondata.Comments != "" && jsondata.Comments != null) {
		var _arraycomments;
		if (islocal) {
			_arraycomments = JSON.parse(jsondata.Comments);
		} else {
			_arraycomments = jsondata.Comments;
		}
		var _length = _arraycomments.length;
		var _arraytem = [];
		for (var i = 0; i < _length; i++) {
			var _comments = _arraycomments[i];
			var _temcomment = ADDSCENELI_TEM;
			_temcomment = _temcomment.replace(/{CommentGuid}/g, _comments.CommentGuid);
			_temcomment = _temcomment.replace(/{MessageID}/g, messageid);
			_temcomment = _temcomment.replace(/{CommentUser}/g, _comments.UserName);
			_temcomment = _temcomment.replace(/{CommentContent}/g, _comments.Content);
			if (jsondata.Comments[i].UserID == userInfo.UserID && CheckPermission(false) && parseInt(jsondata.Status) != 3) {
				var _texta = "<a tag='commentdeid' onclick=\"DeleteOneComment('" + messageid + "','" + _comments.CommentGuid + "')\">删除</a>"
				_temcomment = _temcomment.replace(/{DeleteText}/g, _texta);
			} else {
				_temcomment = _temcomment.replace(/{DeleteText}/g, "");
			}
			var _time = _comments.Time;
			if (!islocal) {
				_time = _time.substring(0, _time.length - 3);
			}
			_temcomment = _temcomment.replace(/{CommentTime}/g, _comments.Time);
			if (_comments.Content.indexOf("需要整改") == 0 && jsondata.UserID == userInfo.UserID && (jsondata.IsExamine == "false" || !jsondata.IsExamine)) {
				_optem = "<a href='#' onclick=GotoAddSceneMessage('" + type + "','" + messageid + "')>&nbsp;[整改]</a>";
				_temcomment = _temcomment.replace(/{CommentUpdate}/g, _optem);
			} else {
				_temcomment = _temcomment.replace(/{CommentUpdate}/g, "");
			}
			_arraytem.push(_temcomment);
		}
		_commentcontent = _arraytem.join("");
	}
	return _commentcontent;
};
//加载图片
function LoadImg(messageid) {
	var _loadtem = LOADIMG.replace(/{MessageID}/g, messageid);
	$("#" + messageid).append(_loadtem);
	myScroll.refresh();
};
//替换guid为MessageID
function ReplaceMessageID(guid, messageid, address) {
	$("#load_" + guid).remove();
	$("#" + guid).attr("id", messageid)
	$("#Address_" + guid).html(address);
	var _htmldata = $("#" + messageid).html();
	var reg = new RegExp(guid, "ig");
	_htmldata = _htmldata.replace(reg, messageid);
	$("#" + messageid).html(_htmldata);
	myScroll.refresh();
	$(".contentimage").trigger('create');
};

//根据权限显示页面

function ChangeShow() {
	//用户权限，测试数据
	if (CheckPermission(false)) {
		//		if (userInfo.InspectScene) {
		//			$("#dfooterul").remove();
		//			$("#pfooterul").listview().listview("refresh");
		//		} else {
		//			$("#pfooterul").remove();
		//			$("#dfooterul").listview().listview("refresh");
		//		}
		if (userInfo.EditScene) {
			$("#scene_Sign").remove();
		} else {
			$("#scene_Update").remove();
		}
		//		if ($("#scene_Sign").length > 0) {
		//			var _sql = "select * from tb_SceneMessageComments where SceneID = ? and (Type = 1 or Type =32) and UserID = ? order by CreateTime desc limit 0,1";
		//			dbBase.OpenTransaction(function(tx) {
		//				dbBase.SelectTable(tx, _sql, [GetUrlParam("sceneid"), userInfo.UserID], function(data) {
		//					if (data) {
		//						if (parseInt(data.item(0).Type) == 32) {
		//							$("#scene_Sign").html("签到")
		//						} else {
		//							$("#scene_Sign").html("签退")
		//						}
		//					} else {
		//						$("#scene_Sign").html("签到")
		//					}
		//				})
		//			})
		//		}
	} else {
		$("a[data-icon]").remove();
	}
};

var OPSI = new OpSceneItem(); //服务器请求
var OPLC = new OpLoaclComment(); //操作本地数据库
//删除消息
function DeleteSceneMessage(messageid) {
	NotificationConfirm("确定要删除吗?", "删除提醒!", "删除,取消", DeleteConfirm);
	//	DeleteConfirm(1);

	function DeleteConfirm(button) {
		if (button == 1) {
			var _time = new Date().format("yyyy-MM-dd hh:mm:ss");
			UpdateStatus(messageid, 4, 4);
			var SOP = new ShowOp();
			SOP.Delete(messageid);
			OPSI.Delete(messageid, _time);
		}
	}
};

//归档消息
function ArchiveSceneMessage(messageid, status, isexamine) {
	NotificationConfirm("确定要归档吗?", "归档提醒!", "归档,取消", ArchiveConfirm);
	//	ArchiveConfirm(1);

	function ArchiveConfirm(button) {
		if (button == 1) {
			if (status == "2" && (isexamine == "false" || !isexamine)) {
				alert("需要整改，无法归档");
			} else {
				//			RecursiveArchive(messageid)
				var _time = new Date().format("yyyy-MM-dd hh:mm:ss");
				UpdateStatus(messageid, 3, 3);
				var SOP = new ShowOp();
				SOP.Update(messageid);
				OPSI.Update(messageid, _time);
				$("#" + messageid + " a[tag='commentdeid']").remove();
			}
		}
	}

};

function RecursiveArchive(messageid) {
	var _time = new Date().format("yyyy-MM-dd hh:mm:ss");
	UpdateStatus(messageid, 3, 3);
	var SOP = new ShowOp();
	SOP.Update(messageid);
	OPSI.Update(messageid, _time);
	$("#" + messageid + " a[tag='commentdeid']").remove();
	var _sql = "select * from tb_SceneMessageComments where MessageID = ?";
	dbBase.OpenTransaction(function(tx) {
		dbBase.SelectTable(tx, _sql, [messageid], function(data) {
			var _relationmessageid = data.item(0).Relation;
			if (_relationmessageid && _relationmessageid != "" && _relationmessageid != null) {
				RecursiveArchive(_relationmessageid);
			}
		})
	});
};

//审核消息
function ExamineSceneMessage(messageid, type) {
	navigator.notification.prompt(
		'审核意见',
		ExamineConfirm,
		'审核', ['通过', '整改', '取消']
	);
	//	ExamineConfirm({
	//		buttonIndex: 2,
	//		input1: "时代复汪汪汪"
	//	});

	function ExamineConfirm(results) {
		var _time = new Date().format("yyyy-MM-dd hh:mm:ss");
		var _guid = NewGuid();
		_optem = "<a href='#' onclick=GotoAddSceneMessage('" + type + "','" + messageid + "')>&nbsp;[整改]</a>";
		var _inputdata = results.input1 == "" ? "" : "，" + results.input1;
		var _jsondata = {
			MessageID: messageid,
			Guid: _guid,
			UserName: userInfo.UserName,
			Content: results.buttonIndex == 1 ? "审核通过" + _inputdata : "需要整改" + _inputdata + "&nbsp;" + _optem,
			Time: _time
		}
		var SOP = new ShowOp();
		if (results.buttonIndex == 1) {
			OPLC.AddComment(messageid, _guid, _time, "审核通过" + _inputdata, 1);
			SOP.Comment(_jsondata);
			OPSI.Comment(messageid, _guid, "审核通过" + _inputdata, 1, _time) //通过
		} else if (results.buttonIndex == 2) {
			OPLC.AddComment(messageid, _guid, _time, "需要整改" + _inputdata, 2); //本地数据库操作
			SOP.Comment(_jsondata); //界面操作
			OPSI.Comment(messageid, _guid, "需要整改" + _inputdata, 2, _time) //服务器请求
		} else if (results.buttonIndex == 3) {

		}
	}
};
//评论消息
function CommentSceneMessage(messageid) {
	navigator.notification.prompt(
		'输入评论内容!',
		CommentConfirm,
		'评论', ['确认', '取消']
	);
	//	CommentConfirm({
	//		buttonIndex: 1,
	//		input1: "时代复汪汪汪"
	//	});

	function CommentConfirm(results) {
		if (results.buttonIndex == 1) {
			if (results.input1 != "") {
				var _guid = NewGuid();
				var _time = new Date().format("yyyy-MM-dd hh:mm:ss");
				var _jsondata = {
					MessageID: messageid,
					Guid: _guid,
					UserName: userInfo.UserName,
					Content: results.input1,
					Time: _time
				}
				OPLC.AddComment(messageid, _guid, _time, results.input1, 0);
				var SOP = new ShowOp();
				SOP.Comment(_jsondata);
				OPSI.Comment(messageid, _guid, results.input1, 0, _time) //整改
			} else {
				alert("评论内容不能为空");
			}
		}
	}
};

//删除评论
function DeleteOneComment(messageid, commentguid) {
	NotificationConfirm("确定要删除评论吗?", "删除提醒!", "删除,取消", DeleteConfirm);
	//	DeleteConfirm(1);

	function DeleteConfirm(button) {
		if (button == 1) {
			if ($("#Op_" + messageid).length > 0) {
				var _time = new Date().format("yyyy-MM-dd hh:mm:ss");
				var SOP = new ShowOp();
				SOP.DeleteComment(commentguid);
				OPLC.DeleteComment(messageid, commentguid);
				OPSI.DeleteComment(messageid, commentguid, _time);
			} else {
				alert("消息已归档");
			}
		}
	}
};

//界面显示操作
function ShowOp() {

}
ShowOp.prototype = {
	Delete: function(messageid) { //界面显示：删除消息
		var $messageid = $("#" + messageid);
		if ($messageid) {
			$messageid.remove();
			$messageid = null;
			myScroll.refresh();
		}
	},
	Update: function(messageid) { //界面显示：归档消息
		var $messageid = $("#Op_" + messageid);
		$messageid.html("");
		$messageid = null;
		$("#temp_SceneItem").listview('refresh');
		myScroll.refresh();
	},
	Comment: function(jsonData) { //界面显示：添加评论或者审核意见
		var $messageid = $("#Comment_" + jsonData.MessageID);
		var _tem = ADDSCENELI_TEM;
		_tem = _tem.replace(/{MessageID}/g, jsonData.MessageID);
		_tem = _tem.replace(/{CommentGuid}/g, jsonData.Guid);
		_tem = _tem.replace(/{CommentUser}/g, userInfo.UserName);
		_tem = _tem.replace(/{CommentContent}/g, jsonData.Content);
		_tem = _tem.replace(/{CommentTime}/g, jsonData.Time);
		_tem = _tem.replace(/{CommentUpdate}/g, "");
		var _texta = "<a tag='commentdeid' onclick=\"DeleteOneComment('" + jsonData.MessageID + "','" + jsonData.Guid + "')\">删除</a>"
		_tem = _tem.replace(/{DeleteText}/g, _texta);
		$messageid.append(_tem);
		var cachedata = {
			CommentID: jsonData.Guid,
			MessageID: jsonData.MessageID,
			CommentUser: userInfo.UserName,
			CommentContent: jsonData.Content,
			CommentTime: jsonData.Time
		}
		myScroll.refresh();
	},
	DeleteComment: function(guid) { //界面显示：删除一条评论
		var $messageid = $("#" + guid);
		$messageid.remove();
		$messageid = null;
		myScroll.refresh();
	}
};

//同步删除数据
function WorkerDeleteSceneItem(messageid) {
	if ($.mobile.activePage.attr("id") == "Page_SceneItem" && messageid) {
		var _sop = new ShowOp();
		_sop.Delete(messageid);
	}
};
//同步添加、修改现场数据

function WorkerAddOrUpdateSceneItem(jsondata) {
	if ($.mobile.activePage.attr("id") == "Page_SceneItem" && jsondata) {
		var _len = jsondata.length;
		//		alert(_len);
		for (var i = 0; i < _len; i++) {
			if (GetUrlParam("sceneid") == jsondata[i].SceneID) {
				var $messageid = $("#" + jsondata[i].Id);
				if ($messageid.length > 0) {
					var _tem = ShowData([jsondata[i]], false);
					$messageid.replaceWith(_tem);
					var $temp_SceneItem = $("#temp_SceneItem");
					$(".contentimage").trigger('create');
					$temp_SceneItem.listview('refresh');
					myScroll.refresh();
				} else {
					var _refresh = new RefreshSceneItem()
					_refresh.Append(ShowData([jsondata[i]], false), false);
				}
			}
		}
	}
};
//所有具体现场消息归档
function ArchiveAllData() {
	$(".contentop").remove();
	$("a[tag='commentdeid']").remove();
};

//跳转到现场界面
function BackScene() {
	ChangePage('scene.html?parentid=' + GetUrlParam("parentid") + "&projectid=" + GetUrlParam("projectid") + "&sceneid=" + GetUrlParam("sceneid"));
};

//跳转到添加具体现场消息界面
function GotoAddSceneMessage(optype, relationid) {
	var _optype = optype == undefined ? "" : optype
	var _relationid = relationid ? relationid : "";
	if (CheckPermission(true)) {
		ChangePage('addSceneMessage.html?parentid=' + GetUrlParam("parentid") + "&projectid=" + GetUrlParam("projectid") + '&&optype=' + _optype + "&sceneid=" + GetUrlParam("sceneid") + '&&relation=' + _relationid);
	}
};
//修改现场
function UpdateScene() {
	if (CheckPermission(true)) {
		ChangePage("addScene.html?sceneid=" + GetUrlParam("sceneid"));
	}
};

//当前现场被删除
function DeleteCurrentScene(sceneid) {
	if (sceneid == GetUrlParam("sceneid") && $.mobile.activePage.attr("id") == "Page_SceneItem") {
		ChangePage('project.html');
	}
};
//当前现场完工
function CompleteCurrentScene(sceneid, state) {
	if (sceneid == GetUrlParam("sceneid") && $.mobile.activePage.attr("id") == "Page_SceneItem" && GetUrlParam("state") != 3 && state == 3) {
		alert("现场已完工!");
		ChangePage('project.html');
	}
};

//跳转到sceneItem页面，重置全局变量ImgJsons , Description
function GotoMaterialScene() {
	ChangePage('materialscene.html?parentid=' + GetUrlParam("parentid") + "&projectid=" + GetUrlParam("projectid") + "&sceneid=" + GetUrlParam("sceneid"));
};
//跳转到sceneItem页面，重置全局变量ImgJsons , Description
function GotoMaterialAllScene() {
	ChangePage('materialallscene.html?parentid=' + GetUrlParam("parentid") + "&projectid=" + GetUrlParam("projectid") + "&sceneid=" + GetUrlParam("sceneid"));
};