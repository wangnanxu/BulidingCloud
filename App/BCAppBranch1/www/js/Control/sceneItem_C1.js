/*页面加载完成执行*/
$(document).on("pageshow", "#Page_SceneItem", function() {
	ChangeShow();
	InitScentItem();
});

function InitScentItem() {
	var _sceneid = GetUrlParam("sceneid");
	var _time = new Date(3015, 0, 1, 10, 10, 10).format("yyyy-MM-dd hh:mm:ss");
	IsLoadHistory[_sceneid] = _time;
	SelectDB(_time, _sceneid, function(data) {
		CheckInitDBData(data, _sceneid, true);
	});
};
/*初始化处理当前现场数据*/
function PullScentItem() {}
	/*获取数据库数据*/

function SelectDB(time, sceneid, callback) {
	var _sql = "select * from tb_SceneMessageComments where SceneID = ? and State != 0 and State !=2 and CreateTime < ? order by CreateTime desc limit 0,10";
	dbBase.OpenTransaction(function(tx) {
		dbBase.SelectTable(tx, _sql, [sceneid, time], function(data) {
			callback(data);
		})
	})
};
/*获取数据库数据 */
function SelectDBUpload(sceneid, state, callback) {
	var _length = state.length;
	var _state = [];
	var _pramas = [sceneid, userInfo.UserID];
	for (var i = 0; i < _length; i++) {
		_state.push("State = ?");
		_pramas.push(state[i]);
	}
	var _sql = "select * from tb_SceneMessageComments where SceneID = ? and UserID = ? and (" + _state.join(' or ') + ") order by CreateTime desc";
	dbBase.OpenTransaction(function(tx) {
		dbBase.SelectTable(tx, _sql, _pramas, function(data) {
			callback(data);
		})
	})
};
/*检测是否有待发送的数据*/
function CheckDBUploadData(data) {
	if (data) {
		var _jsondata = ChangeSelectToJsondata(data);
		var _refresh = new RefreshSceneItem()
		_refresh.Append(ShowData(_jsondata, true), false);
		var _length = _jsondata.length;
		ChangeState(_jsondata, 2);
		for (var i = 0; i < _length; i++) {
			if (parseInt(_jsondata[i].State) == 2 || parseInt(_jsondata[i].State) == 0) {
				LoadImg(_jsondata[i].MessageID); //添加loading效果
			}
			if (parseInt(_jsondata[i].State) == 0) {
				UploadSceneItem(_jsondata[i]);
			}
		}
	}
};
/*检测数据是否有10条，不足获取服务器，足够显示*/
function CheckInitDBData(data, sceneid, isinit) {
	var _time = IsLoadHistory[sceneid];
	if (data) {
		ShowLocal();
	}
	if (!data || isinit) {
		if (_time != false) {
			PostHistoryData(sceneid, _time, function(callbackdata, count) {
				if (callbackdata) {
					if (callbackdata.length < 10) {
						IsLoadHistory[sceneid] = false;
					} else {
						IsLoadHistory[sceneid] = callbackdata[callbackdata.length - 1].CreateTime;
					}
					var _length = callbackdata.length;
					for (var i = 0; i < _length; i++) {
						var $temp_SceneItem = $("#temp_SceneItem");
						var $messageid = $("#" + callbackdata[i].Id);
						var _tem = ShowData([callbackdata[i]], false);
						if ($messageid.length > 0) {
							$messageid.replaceWith(_tem);
						} else {
							$temp_SceneItem.prepend(_tem);
						}
					}
					var $temp_SceneItem = $("#temp_SceneItem");
					$(".contentimage").trigger('create');
					$temp_SceneItem.listview('refresh');
					if (isinit) {
						AddPaneScroll("up", PullSceneItem);
					} else {
						myScroll.refresh();
					}
				}
			});
		}
	}
	if (isinit) {
		SelectDBUpload(sceneid, [0, 2], CheckDBUploadData);
	}
	//显示本地数据
	function ShowLocal() {
		var _jsondata = ChangeSelectToJsondata(data);
		IsLoadHistory[sceneid] = _jsondata[_jsondata.length - 1].CreateTime;
		var _refresh = new RefreshSceneItem();
		_refresh.Prepend(ShowData(_jsondata, true), !isinit);
	}
};
/*从服务器获取数据*/
var IsLoadHistory = {};

function DealServerData(data, count) {
	if (IsLoadHistory[sceneid] != false) {

	};
};
/*界面切换签到签退状态*/
function SceneItemSign() {
	if (CheckPermission(true)) {
		var _signtext = $("#scene_Sign").html();
		if (CheckPlatform()) {
			SignConfirm(1);
		} else {
			NotificationConfirm("确定要" + _signtext + "吗?", "签到提示!", _signtext + ",取消", SignConfirm);
		}

		function SignConfirm(button) {
			if (button == 1) {
				var _sign = 1;
				if (_signtext.indexOf("签到") != -1) {
					_sign = 1;
					$("#scene_Sign").html("签退");
				} else {
					_sign = 32;
					$("#scene_Sign").html("签到");
				}
				AddComplate(_sign, function(messageid) {
					var _getData = new OpLoaclComment();
					_getData.GetComment(messageid, function(data) {
						var $temp_SceneItem = $("#temp_SceneItem");
						var _tem = ShowData([data.item(0)], true);
						$temp_SceneItem.append(_tem);
						$temp_SceneItem.listview('refresh');
						AddPaneScroll("up", PullSceneItem);
						if (parseInt(data.item(0).State) == 2 || parseInt(data.item(0).State) == 0) {
							LoadImg(data.item(0).MessageID); //添加loading效果
						}
						UploadSceneItem(data.item(0));
					})
				});
			}
		}
	}
};

/*下拉历史数据*/
function PullSceneItem() {
	var _sceneid = GetUrlParam("sceneid");
	if (IsLoadHistory[_sceneid] != false) {
		SelectDB(IsLoadHistory[_sceneid], _sceneid, function(data) {
			CheckInitDBData(data, _sceneid, false);
		});
	}
};
/*联网同步*/
function OfflineScentItem() {
	var _sceneid = GetUrlParam("sceneid")
	SelectDBUpload(_sceneid, [2, 3, 4], function(jsondatas) {
		var _length = jsondatas.length;
		var _jsondata = ChangeSelectToJsondata(jsondatas);
		for (var i = 0; i < _length; i++) {
			if (_jsondata[i].State == 2) {
				UploadSceneItem(_jsondata[i]);
			} else if (_jsondata[i].State == 3) {
				PostOfflineCommentsData(_jsondata[i].MessageID, _jsondata[i].Comments, _jsondata[i].Status, function() {
					ChangeState(_jsondata[i], 1);
				})
			} else if (_jsondata[i].State == 4) {
				var _datetime = new Date().format("yyyy-MM-dd hh:mm:ss");
				var _postJsonData = {
					MessageID: _jsondata[i].MessageID,
					Token: userInfo.Token,
					Time: _datetime
				}
				PostDeleteSceneItem(_postJsonData, function() {
					DeleteSceneItem(_postJsonData.MessageID);
				});
			}
		};
	});
};
/*上传数据*/
function UploadSceneItem(jsondata) {
	GetPosition(function(_position) { //获取经纬度
		if (_position != null) {
			var _length = 0;
			if (jsondata.Images != "" && jsondata.Images != "[]") {
				var _images = JSON.parse(jsondata.Images);
				_length = _images.length;
				var _fileNum = _length;
				for (var i = 0; i < _length; i++) {
					var _uploadjson = {
						Type: 0, //上传类型，0图片，1头像
						URI: _images[i].ThumbnailPicture, //上传文件地址   
						Pramas: "", //过渡参数
						UploadPramas: {
							Token: userInfo.Token,
							Guid: jsondata.MessageID
						}, //上传参数
						Callback: UploadImgCallback //回调函数
					}
					UploadFile(_uploadjson);

					function UploadImgCallback() {
						_fileNum -= 1;
						if (_fileNum == 0) {
							UploadCallback();
						}
					};
				}
			} else {
				UploadCallback(); //上传数据
			}

			function UploadCallback() { //闭包上传函数
				var _jsondata = { //Post数据
					SceneID: GetUrlParam("sceneid"),
					Token: userInfo.Token,
					Time: jsondata.CreateTime,
					Address: _position.latitude + "|" + _position.longitude,
					Type: jsondata.Type,
					Content: jsondata.Description,
					Count: _length,
					Guid: jsondata.MessageID
				};
				PostAddSceneItem(_jsondata, jsondata.Status, _UPCallback); //Post操作
			}
		}

		function _UPCallback(upvalue) { //Post成功回调函数
			dbBase.OpenTransaction(function(tx) {
				dbBase.SaveOrUpdateTable(tx, "tb_SceneMessageComments", ['MessageID', 'Address', 'State'], [upvalue.MessageID, upvalue.Address, 1], "MessageID", jsondata.MessageID, function(istrue) {
					dbBase.SelectTable(tx, "select * from tb_SceneMessageComments where MessageID=?", [upvalue.MessageID], function(repdata) {
						var $messageid = $("#" + jsondata.MessageID);
						var arrayrep = ChangeSelectToJsondata(repdata);
						var _tem = ShowData(arrayrep, true);
						if ($messageid.length > 0) {
							$messageid.replaceWith(_tem);
						}
						var $temp_SceneItem = $("#temp_SceneItem");
						$(".contentimage").trigger('create');
						$temp_SceneItem.listview('refresh');
						myScroll.refresh();
					})
				})
			});
		}
	})
};

/*操作本地评论*/
function OpLoaclComment() {}
OpLoaclComment.prototype = {
	GetComment: function(messageid, callback) {
		var _sql = "select * from tb_SceneMessageComments where MessageID = ?";
		dbBase.OpenTransaction(function(tx) {
			dbBase.SelectTable(tx, _sql, [messageid], function(data) {
				if (typeof(callback) == "function")
					callback(data);
			})
		});
	},
	AddComment: function(messageid, commentid, time, content, status) {
		this.GetComment(messageid, function(data) {
			if (data) {
				var _jsondata = data.item(0).Comments == "" ? [] : JSON.parse(data.item(0).Comments);
				var _newjsondata = {
					CommentGuid: commentid,
					UserID: userInfo.UserID,
					UserName: userInfo.UserName,
					Time: time,
					Stime: null,
					Content: content
				}
				_jsondata.push(_newjsondata);
				var _strdata = JSON.stringify(_jsondata);
				status = status == 0 ? data.item(0).Status : status;
				SetComment(messageid, status, _strdata);
			}
		})
	},
	DeleteComment: function(messageid, commentid) {
		this.GetComment(messageid, function(data) {
			if (data) {
				var _jsondata = JSON.parse(data.item(0).Comments);
				var _length = _jsondata.length;
				for (var i = 0; i < _length; i++) {
					if (_jsondata[i].indexOf(commentid) != -1) {
						_jsondata.splice(i, 1);
						break;
					}
				}
				SetComment(messageid, data.item(0).Status, JSON.stringify(_jsondata));
			}
		})
	}
};
/*修改评论*/
function SetComment(messageid, status, jsondate) {
	dbBase.OpenTransaction(function(tx) {
		dbBase.UpdateTable(tx, "tb_SceneMessageComments", ['Comments', 'Status', 'State'], [jsondate, status, 3], "MessageID=?", messageid, function(istrue) {
			//			alert(istrue);
		})
	});
};
/*改变状态*/
function ChangeState(jsondata, state) {
	dbBase.OpenTransaction(function(tx) {
		var _length = jsondata.length;
		for (var i = 0; i < _length; i++) {
			dbBase.UpdateTable(tx, "tb_SceneMessageComments", ['State'], [state], "MessageID=?", jsondata[i].MessageID, function(istrue) {
				//				alert(istrue);
			})
		}
	});
};
/*归档和删除*/
function UpdateStatus(messageid, status, state) {
	dbBase.OpenTransaction(function(tx) {
		dbBase.UpdateTable(tx, "tb_SceneMessageComments", ['Status', 'state'], [status, state], "MessageID=?", messageid, function(istrue) {
			//			alert(istrue);
		})
	});
};
/*离线同步删除具体现场数据*/
function DeleteSceneItem(messageid) {
	dbBase.OpenTransaction(function(tx) {
		//		alert(messageid);
		dbBase.DeleteTable(tx, "tb_SceneMessageComments", "MessageID=?", [messageid], function(istrue) {
			//			alert("离线同步删除具体现场数据" + istrue);
		})
	});
};
/*
 * 操作具体现场数据：审核、评论、删除、归档、删除评论
 */
function OpSceneItem() {
	this.datetime = new Date();
	this.SOP = new ShowOp();
}
OpSceneItem.prototype = {
	Delete: function(messageid, time) { //删除
		var _datetime = new Date();
		var _jsonData = {
			MessageID: messageid,
			Token: userInfo.Token,
			Time: time
		}
		PostDeleteSceneItem(_jsonData, function() {
			PushDeleteSceneItem(messageid);
		});
	},
	Update: function(messageid, time) { //归档
		var _jsonData = {
			MessageID: messageid,
			Token: userInfo.Token,
			Time: time,
			Status: 3
		}
		PostUpdateStatus(_jsonData, function() {})
	},
	Comment: function(messageid, guid, content, status, time) { //审核，评论
		var _jsonData = {
			MessageID: messageid,
			Token: userInfo.Token,
			Status: status,
			Time: time,
			Guid: guid,
			Content: content
		}
		PostComment(_jsonData, function(callback) {});
	},
	DeleteComment: function(messageid, commentguid, time) { //删除评论
		var _jsonData = {
			Token: userInfo.Token,
			MessageID: messageid,
			CommentGuid: commentguid,
			Time: time
		}
		PostDeleteSceneComment(_jsonData, function() {});
	}
};
//下载图片
function DownLocalPicture(orgimgurl, id, messageid, localthumig) {
	if (orgimgurl) {
		$("#" + id).attr("href", orgimgurl);
		var _downloadjson = {
			URL: orgimgurl, //下载地址
			SaveDir: 0, //保存路径
			Pramas: "",
			Callback: callback //回调函数

		}
		DownLoad(_downloadjson); //下载图片

		function callback(_uri) {
			//var _uri="http://192.168.1.108:8998/Handlers/Image.ashx?fileName=org201506171744230993.jpg";
			$("#" + id).attr("href", _uri);
			dbBase.OpenTransaction(function(tx) {
				dbBase.SelectTable(tx, "select Images from tb_SceneMessageComments where MessageID=?", [messageid], function(adata) {
					if (adata) {
						var _json = DealJson(adata.item(0).Images);
						var _len = _json.length;
						for (var i = 0; i < _len; i++) {
							if (_json[i].ThumbnailPicture == localthumig || _json[i].ThumbnailPictureURI == localthumig) {
								_json[i]["OriginalPictureURI"] = _uri;
								break;
							}
						}
						var _str = CDealJson(_json);
						dbBase.SaveOrUpdateTable(tx, "tb_SceneMessageComments", ["Images"], [_str], "MessageID", messageid);
					}
				})
			})
		}
	}
};

function RefreshImg() {
	jQuery(function($) {
		$(".swipebox").swipebox({
			useCSS: true, // false will force the use of jQuery for animations
			hideBarsDelay: 3000 // 0 to always show caption and action bar
		});
	});
};
//点击完工
function SetSceneState() {
	if (CheckPermission('AchieveScene')) {
		var _data = {
			SceneID: GetUrlParam("sceneid"),
			Status: 3,
			Token: userInfo.Token
		}
		NotificationConfirm("确定完工现场？", "提示", "确定,取消", function(button) {
			if(button==1){
			PostChangeSceneState(_data);
			}
		})
	}
};
//检测是否有该权限
function CheckPermission(permission) {
	if (permission == false || permission == true || userInfo[permission]) {
		if (GetUrlParam('state') != "3") {
			return true;
		} else {
			if (permission == true) {
				alert("项目已完工！");
			}
			return false;
		}
	} else {
		alert("无该项操作权限!");
		return false;
	}
};