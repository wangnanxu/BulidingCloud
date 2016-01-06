//var baseurl = "http://192.168.1.12:8088"; //服务器请求域名，需在config中配置
//var baseurl = "http://192.168.0.51:7081"; //服务器请求域名，需在config中配置
//var baseurl = "http://61.128.186.141:80"; //服务器请求域名，需在config中配置
//var baseurl = "http://115.29.110.41:8082"; //服务器请求域名，需在config中配置
var baseurl = "http://192.168.0.51:7081"; //服务器请求域名，需在config中配置
/*
 * 登陆数据请求(因当前用户信息需写入两个表，所以单独列出)
 * @postData请求参数
 * @callback请求成功回调函数
 */
function PostLoginData(postData, callback) {
	if (onLine) {
		$.post(baseurl + '/APIS/Account/Login', postData, function(adata) {
			if (adata.Success) { //请求成功
				if (typeof(callback) == "function") {
					callback(adata.Value);
				}
			} else {
				$.mobile.loading("hide");
				alert("服务器登陆错误：" + adata.Message) //NotificationAlert(adata.Message,"登陆错误"); //输出请求错误信息
			}
		})
	} else {
		$.mobile.loading("hide");
		alert("网络已断开，请连接");
	}
};
//验证是否超过3天登陆
function PostConfirmDays(postdata, callback) {
	if (onLine) {
		$.post(baseurl + '/APIS/Account/ChekLoginTime', postdata, function(adata) {
			if (adata.Success) {
				if (typeof(callback) == "function") {
					callback(adata);
				}
			} else {
				$.mobile.loading("hide");
				alert("服务器错误:" + adata.Message);
			}
		})
	} else {
		$.mobile.loading("hide");
		if (typeof(callback) == "function") {
			callback(null);
		}
	}
};

function PostInitSyncState(callback) {
	if (onLine) {
		var _postData = {
			Token: userInfo.Token
		}
		$.post(baseurl + '/APIS/SyncData/InitSyncState', _postData, function(adata) {
			if (typeof(callback) == "function") {
				callback(adata.Success);
			}
		})
	} else {
		alert("网络已断开，请连接");
	}
};
//第一次同步部门、企业
function PostSyncOrganization(callback) {
	if (onLine) {
		var _postData = {
			Token: userInfo.Token
		};
		$.post(baseurl + '/APIS/SyncData/SyncOrganization', _postData, function(adata) {
			if (adata.Success) { //请求成功
				if (typeof(callback) == "function") {
					callback(adata.Value);
				}
			} else {
				alert("服务器同步组织错误：" + adata.Message); //输出请求错误信息
			}
		})
	} else {
		alert("网络已断开，请连接");
	}
};
//第一次同步现场、项目
function PostSyncSceneAndProject(callback) {
	if (onLine) {
		var _postData = {
			Token: userInfo.Token
		};
		$.post(baseurl + '/APIS/SyncData/SyncProjectAndScene', _postData, function(adata) {
			if (adata.Success) { //请求成功
				if (typeof(callback) == "function") {
					callback(adata.Value);
				}
			} else {
				alert("服务器同步项目、现场错误：" + adata.Message); //输出请求错误信息
			}
		})
	} else {
		alert("网络已断开，请连接");
	}
};
//第一次同步资料
function PostSyncMaterial(callback) {
	if (onLine) {
		var _postData = {
			Token: userInfo.Token
		};
		$.post(baseurl + '/APIS/InitData/SyncMaterial', _postData, function(adata) {
			if (adata.Success) { //请求成功
				if (typeof(callback) == "function") {
					callback(adata.Value);
				}
			} else {
				alert("服务器同步资料错误：" + adata.Message); //输出请求错误信息
			}
		})
	} else {
		alert("网络已断开，请连接");
	}
};
//同步具体现场数据
function PostSyncSceneItem(callback) {
	if (onLine) {
		var _postData = {
			Token: userInfo.Token,
			SceneId: _sceneid
		};
		$.post(baseurl + '/APIS/Scene/SceneItem', _postData, function(adata) {
			if (adata.Success) { //请求成功
				if (typeof(callback) == "function") {
					callback(adata.Value);
				}
			} else {
				alert("服务器同步具体现场错误：" + adata.Message); //输出请求错误信息
			}
		})
	} else {
		alert("网络已断开，请连接");
	}
};
//请求消息历史数据
function PostHistoryMessage(postdata, callback, type) {
	if (onLine) {
		$.post(baseurl + '/APIS/ChatMessage/GetChatMessage', postdata, function(adata) {
			if (adata.Success) { //请求成功
				if (type != "0") {
					PushAddMessage(adata.Value.Data); //写数据库
				}
				if (typeof(callback) == "function") {
					callback(adata.Value.Data);
				}
			} else {
				alert("服务器请求历史消息错误：" + adata.Message); //输出请求错误信息
			}
		})
	} else {
		alert("网络已断开，请连接");
	}
};
//发送消息
function PostSendMessage(postdata, callback) {
	if (onLine) {
		$.post(baseurl + '/APIS/ChatMessage/Send', postdata, function(adata) {
			if (adata.Success) { //请求成功
				//修改数据库
				PushUpdateSendMessage(postdata, callback);
				//修改页面显示
				UpdateSendDataShow(postdata);
			} else {
				PushDeleteSendMessage(postdata);
				alert("服务器发送消息错误：" + adata.Message); //输出请求错误信息
			}
		})
	} else {
		alert("网络已断开，请连接");
		if (typeof(callback) == "function") {
			callback();
		}
	}
};
//添加现场
function PostAddScene(postdata, callback) {
	if (onLine) {
		$.post(baseurl + '/APIS/Scene/AddScene', postdata, function(adata) {
			if (adata.Success) { //请求成功
				PushUpdateSendScene(adata.Value, postdata.SceneID, callback);
			} else {
				PushDeleteSendScene(postdata.SceneID);
				alert("服务器添加现场错误：" + adata.Message); //输出请求错误信息
			}
		})
	} else {
		alert("网络已断开，请连接");
		if (typeof(callback) == "function") {
			callback();
		}
	}
};
//修改现场
function PostUpdateScene(postdata, callback) {
	if (onLine) {
		$.post(baseurl + '/APIS/Scene/UpdateScene', postdata, function(adata) {
			if (adata.Success) { //请求成功
				PushUpdateSendScene(adata.Value, postdata.SceneID, callback);
			} else {
				alert("服务器添加现场错误：" + adata.Message); //输出请求错误信息
			}
		})
	} else {
		alert("网络已断开，请连接");
		if (typeof(callback) == "function") {
			callback();
		}
	}
};
//发送一条具体现场消息数据
function PostAddSceneItem(postdata, status, callback) {
	if (onLine) {
		$.post(baseurl + '/APIS/SceneData/AddSceneData', postdata, function(adata) {
			if (adata.Success) {
				callback(adata.Value);
			} else {
				DeleteSceneItem(postdata.Guid);
				$("#" + postdata.Guid).remove();
			}
		}).error(function(xhr, errorText, errorType) {
			DeleteSceneItem(postdata.Guid);
			$("#" + postdata.Guid).remove();
		})
	} else {
		UpdateStatus(postdata.Guid, status, 2);
		alert("网络已断开，请连接");
	}
};
//获取现场历史数据
function PostHistoryData(sceneid, time, callback) {
	if (onLine) {
		var postdata = {
			SceneID: sceneid,
			Token: userInfo.Token,
			Time: time,
			PageSize: 10,
			Status: 4
		}

		$.post(baseurl + '/APIS/SceneData/GetSceneData', postdata, function(adata) {
			var _data = JSON.stringify(adata);
			if (adata.Success) {
				callback(adata.Value.Data, adata.Value.Count);
				PushHistoryData(adata.Value.Data); //写入数据库
			} else {
				alert("服务器获取现场历史数据错误：" + adata.Message);
				callback(false, 0);
			}
		})
	} else {
		alert("网络已断开，请连接");
	}
};
//具体现场操作归档
function PostUpdateStatus(postdata, callback) {
	if (onLine) {
		$.post(baseurl + '/APIS/SceneData/UpdateStatus', postdata, function(adata) {
			if (adata.Success) {
				callback();
			} else {
				alert("服务器归档消息错误：" + adata.Message);
			}
		})
	} else {
		alert("网络已断开，请连接");
	}
};
//具体现场操作删除
function PostDeleteSceneItem(postdata, callback) {
	if (onLine) {
		$.post(baseurl + '/APIS/SceneData/DeleteSceneItem', postdata, function(adata) {
			if (adata.Success) {
				callback();
			} else {
				alert("服务器删除现场错误：" + adata.Message);
			}
		})
	} else {
		alert("网络已断开，请连接");
	}
};
//具体现场操作审核，评论
function PostComment(postdata, callback) {
	if (onLine) {
		$.post(baseurl + '/APIS/SceneData/Comment', postdata, function(adata) {
			if (adata.Success) {
				callback(adata);
			} else {
				alert("服务器评论与审核错误：" + adata.Message);
			}
		})
	} else {
		alert("网络已断开，请连接");
	}
};
//具体现场删除评论
function PostDeleteSceneComment(postdata, callback) {
	if (onLine) {
		$.post(baseurl + '/APIS/SceneData/DeleteSceneComment', postdata, function(adata) {
			if (adata.Success) {
				callback();
			} else {
				alert("服务器删除评论错误：" + adata.Message);
			}
		})
	} else {
		alert("网络已断开，请连接");
	}
};

//离线同步评论操作
function PostOfflineCommentsData(messageid, comments, status, callback) {
	if (onLine) {
		var postdata = {
			MessageID: messageid,
			Comments: comments,
			Status: status,
			Token: userInfo.Token
		}
		$.post(baseurl + '/APIS/SceneData/SyncSceneDataToServer', postdata, function(adata) {
			if (adata.Success) {
				if (typeof(callback) == "function") {
					callback();
				}
			} else {
				alert(adata.Message);
			}
		})
	} else {
		alert("网络已断开，请连接");
	}
};
/*修改密码*/
function PostUpdatePassword(oldpsw, newpsw, callback) {
	if (onLine) {
		var postdata = {
			OldPassword: oldpsw,
			NewPassword: newpsw,
			Token: userInfo.Token
		}
		$.post(baseurl + '/APIS/Account/SetUserPassword', postdata, function(adata) {
			if (adata.Success && typeof(callback) == "function") {
				callback();
			} else {
				alert(adata.Message);
			}
		})
	} else {
		alert("网络已断开，请连接");
	}
};
//worker Active处理
var HandleActionPost = function() {
	//同步部门、企业
	this.SyncOrganization = function(postdata, data, callback) {
			if (onLine) {
				$.post(baseurl + '/APIS/SyncData/SyncOrganization', postdata, function(adata) {
					if (adata.Success) { //请求成功
						ConfirmSync(data); //确认完成
						if (typeof(callback) == "function") {
							callback(adata.Value);
						}
					} else {
						alert("同步组织信息错误" + adata.Message); //输出请求错误信息
					}
				})
			}
		},
		//同步现场、项目
		this.SyncSceneAndProject = function(postdata, data, callback) {
			if (onLine) {
				$.post(baseurl + '/APIS/SyncData/SyncProjectAndScene', postdata, function(adata) {
					if (adata.Success) { //请求成功
						ConfirmSync(data); //确认完成
						if (typeof(callback) == "function") {
							callback(adata.Value);
						}
					} else {
						alert("服务器同步错误：" + adata.Message); //输出请求错误信息
					}
				})
			} else {
				alert("网络已断开，请连接");
			}
		},
		//同步具体现场数据
		this.SyncSceneData = function(postdata, data, callback) {
			if (onLine) {
				$.post(baseurl + '/APIS/SyncData/SyncSceneData', postdata, function(adata) {
					if (adata.Success) { //请求成功
						ConfirmSync(data); //确认完成
						if (typeof(callback) == "function") {
							callback(adata.Value);
							WorkerAddOrUpdateSceneItem(adata.Value);
						}
					} else {
						alert("服务器错误：" + adata.Message); //输出请求错误信息
					}
				})
			} else {
				alert("网络已断开，请连接");
			}
		}
};

function ConfirmSync(data) {
	if (onLine) {
		var _data = {
			syncTime: data.Time,
			Token: userInfo.Token,
			syncAction: data.Action
		}
		$.post(baseurl + '/APIS/SyncData/SyncConfirm', _data);
	} else {
		alert("网络已断开，请连接");
	}
};
//本地清除缓存后请求服务器清除同步状态
function PostClearCache() {
	if (onLine) {
		var _data = {
			Token: userInfo.Token
		}
		$.post(baseurl + '/APIS/SyncData/ClearCache', _data);
	} else {
		alert("网络已断开，请连接");
	}
};
//现场完工
function PostChangeSceneState(postdata) {
		if (onLine) {
			$.post(baseurl + '/APIS/Scene/SetSceneStatus', postdata, function(adata) {
				if (adata.Success && adata.Value.IsUpdate) {
					PushUpdateSceneState(postdata.SceneID);
					ArchiveAllData();
				} else {
					alert(adata.Message);
				}
			});
		} else {
			alert("网络已断开，请连接");
		}
	}
	//定义全局ajax请求函数
$(document).ajaxError(function(event, xhr, settings, thrownError) {
	try {
		var _jsondata = JSON.parse(xhr.responseText);
		if (_jsondata.LoginFailed && _jsondata.LoginFailed == true) {
			ExitToLoading("0");
			alert(_jsondata.Message);
		}
	} catch (e) {

	}
});
$(document).ajaxSuccess(function(e, xhr, settings) {
	try {
		var _jsondata = JSON.parse(xhr.responseText);
		if (_jsondata.LoginFailed && _jsondata.LoginFailed == true) {
			ExitToLoading("0");
			alert(_jsondata.Message);
		}
	} catch (e) {

	}
});

function PostLoginOut() {
	$.post(baseurl + '/APIS/Account/Logout', {
		Token: userInfo.Token
	}, function(adata) {});
};

function PostDeletePicture(messageid, orgname) {
	$.post(baseurl + '/APIS/SceneData/DeleteScenePicture', {
		Token: userInfo.Token,
		SceneItemID: messageid,
		OrgPictureName: orgname
	}, function() {});
};

function PostGetMaterial(callback) {
	if (onLine) {
		$.post(baseurl + '/APIS/KnowlegeBase/GetKnowlegeList', {
			Token: userInfo.Token
		}, function(adata) {
			if (adata.Success) { //请求成功
				if (typeof(callback) == "function") {
					callback(adata.Value.KnowlegeList,true);
				}
			} else {
				alert("服务器请求资料失败：" + adata.Message); //输出请求错误信息
			}
		});
	} else {
		alert("网络已断开，请连接");
	}
};

function PostDownloadMaterial(FileName,callback) {
	if (onLine) {
		$.post(baseurl + '/APIS/KnowlegeBase/DownloadKnowlegeList', {
			Token: userInfo.Token,
			FileName:FileName
		}, function(adata) {
			if (adata.Success) { //请求成功
				if (typeof(callback) == "function") {
					callback(adata.Value.File);
				}
			} else {
				alert("服务器请求资料失败：" + adata.Message); //输出请求错误信息
			}
		});
	} else {
		alert("网络已断开，请连接");
	}
};