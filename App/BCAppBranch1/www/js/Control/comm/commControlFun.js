//当前用户个人信息
var userInfo = {
	UserID: '', //用户ID
	UserName: '', //用户姓名
	NickName: '', //昵称
	RoleIDs: '', //用户权限ID(还不清楚具体权限存放在哪一个表)
	HeadPictureName: '', //用户头像地址
	HeadPictureURI: '../img/head.png', //用户头像地址
	EnterpriseID: '', //企业ID
	EnterpriseName: '', //企业名
	DepartmentID: '', //用户所在部门ID
	FunctionIDs: '',
	Token: '', //唯一编码
	Sign: '1',
	CheckSceneData: false, //
	ArchiveSceneData: false, //归档现场数据
	VerifySceneData: false, //审核现场数据
	AchieveScene: false, //完工现场数据
	AddSceneData: false, //添加现场
	EditScene: false, //编辑现场
	InspectScene: false, //临检现场数据
	SceneBuilding: false //发布现场数据
};
var onLine = true; //是否在线，true（在线），false（离线）
var workerPost; //数据同步worker
var const_state=["","未开始","进行中","已完工"];
/*
 * 获取用户个人信息
 * @adata当前用户login返回信息（json对象）
 * @fun跳转页面（第一次登陆跳转到数据加载页面，多次登陆直接跳转到信息页面）
 */
function SetCurrentUser(obj, fun) {
	if (obj) {
		var _data = obj;
		userInfo.HeadImage = _data.HeadImage;
		userInfo.HeadPictureURI = _data.HeadPictureURI;
		userInfo.UserID = _data.UserID;
		userInfo.UserName = _data.UserName;
		userInfo.NickName = "";
		userInfo.RoleIDs = _data.RoleIDs; //测试数据
		userInfo.EnterpriseID = _data.EnterpriseID;
		userInfo.EnterpriseName = _data.EnterpriseName;
		userInfo.DepartmentID = _data.DepartmentID;
		var _FunctionIDs = _data.FunctionIDs;
		if (typeof(_data.FunctionIDs) == "string") {
			_FunctionIDs = JSON.parse(_data.FunctionIDs);
		}
		userInfo.FunctionIDs = _FunctionIDs;
		var _length = userInfo.FunctionIDs.length;
		var _isMa = false;
		var _isEdit = false;
		for (var i = 0; i < _length; i++) {
			var _str = userInfo.FunctionIDs[i].split('.');
			var _permission = _str[_str.length - 1];
			if (userInfo[_permission] != undefined) {
				userInfo[_permission] = true;
			}
		}
		userInfo.Token = _data.Token;
		if (_data.Sign) {
			userInfo.Sign = _data.Sign;
		} else {
			userInfo.Sign = "1";
		}
		if (typeof(fun) == "function") {
			fun();
		}
	}
};
//自动识别网络状态
(function(win) {
	function BBNetwork(callback) {
		this.navigator = win.navigator;
		this.callback = callback;
		this._init();
	};
	var bbNetworkProto = BBNetwork.prototype;
	bbNetworkProto._init = function() {
		var that = this;
		win.addEventListener("online", function() {
			that._fnNetworkHandler();
		}, true);
		win.addEventListener("offline", function() {
			that._fnNetworkHandler();
		}, true);
	};
	bbNetworkProto._fnNetworkHandler = function() {
		this.callback && this.callback(this.navigator.onLine ? "online" : "offline");
	};
	bbNetworkProto.isOnline = function() {
		return this.navigator.onLine;
	};
	win.BBNetwork = BBNetwork;
})(window);
$(function() {
	var bbNetwork = new BBNetwork(function(status) {
		var tipMsg = "";
		if ("online" != status) {
			setTimeout(function() {
				onLine = false;
			}, 6000)

			//alert("目前处于离线状态~~~~(>_<)~~~~ ");
		} else {

			setTimeout(function() {
				onLine = true;
				OfflineScentItem();
				//连网自动发送消息
				AutoSendMessage();
				//连网自动发送现场
				AutoSendScene();
				//				alert("网络已连接");
			}, 6000)

		}
	});
	if (!bbNetwork.isOnline()) {
		setTimeout(function() {
				onLine = false;
			}, 6000)
			//alert("目前处于离线状态~~~~(>_<)~~~~ ");
	}
});
//添加Woker线程用于定时请求
function AddWorker() {
	if (typeof("Worker") != null) {
		workerPost = new Worker("../js/Communication/worker.js");
		var _workerdata = {
			Token: userInfo.Token,
			baseUrl: baseurl
		}
		workerPost.postMessage(_workerdata); //启动worker
		workerPost.onmessage = function(evt) {
			//worker返回数据处理
			HandleWorkerBackData(evt.data);
		}
	} else {
		alert("设备不支持数据同步(worker)");
	}
};
//将查询返回转换为json对象处理
function ChangeSelectToJsondata(adata) {
	if (adata) {
		var _len = adata.length;
		var _jsondata = [];
		for (var i = 0; i < _len; i++) {
			_jsondata[i] = adata.item(i);
		}
		return _jsondata;
	}
};
//获取sguid

function NewGuid() {
	var guid = "";
	for (var i = 1; i <= 32; i++) {
		var n = Math.floor(Math.random() * 16.0).toString(16);
		guid += n;
		if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
			guid += "-";
	}
	return guid;
};
/*
 * 解析页面跳转参数
 * @name参数属性
 */
function GetUrlParam(name) {
	var reg = new RegExp("(^|&)" +
		name + "=([^&]*)(&|$)");
	var r = window.location.search.substr(1).match(reg);
	if (r != null) return unescape(r[2]);
	return null;
};
/*
 * 检测网络连接状态
 */
function CheckConnection() {
	var networkState = navigator.connection.type;
	var states = {};
	states[Connection.UNKNOWN] = '未知网络连接';
	states[Connection.ETHERNET] = 'Ethernet connection';
	states[Connection.WIFI] = 'WIFI连接';
	states[Connection.CELL_2G] = '2G网络';
	states[Connection.CELL_3G] = '2G网络';
	states[Connection.CELL_4G] = '2G网络';
	states[Connection.CELL] = 'Cell generic connection';
	states[Connection.NONE] = '网络连接断开';
	if (networkState == Connection.NONE) {
		navigator.notification.alert(
			states[networkState],
			null,
			'网络连接',
			'确认'
		);
	}
};

/*
 * Notification消息提示Alert
 */
function NotificationAlert(content, title) {
	navigator.notification.alert(
		content,
		null,
		title,
		'确定'
	);
};

/*
 * Notification消息提示Confirm
 */
function NotificationConfirm(content, title, buttonLabels, Confirm) {
	navigator.notification.confirm(
		content,
		Confirm,
		title,
		buttonLabels
	);
};

//退出程序
function Quite() {
	NotificationConfirm("按确定退出程序!", "确定要退出程序吗?", "退出,取消", Confirm);

	function Confirm(button) {
		if (button == 1) navigator.app.exitApp(); //选择了确定才执行退出
	}
};

//获取GPS地址,IOS使用Google地图，Android使用百度地图
function GetPosition(PositionCallBack) {
	if (typeof(PositionCallBack) != "function") {
		return NotificationAlert("参数类型错误", "错误提示");
	}
	if (CheckPlatform()) {
		var Position = {
			"longitude": 106.555177,
			"latitude": 29.558933
		};
		PositionCallBack(Position);
	} else {
		if (device.platform == "iOS") {
			navigator.geolocation.getCurrentPosition(function(position) {
				var Position = {
					"longitude": position.coords.longitude,
					"latitude": position.coords.latitude
				};
				PositionCallBack(Position);
			}, function(error) {
				alert("未找到GPS" + error);
				PositionCallBack(null);
			});
		} else if (device.platform == "Android") {
			var noop = function() {}
			window.locationService.getCurrentPosition(function(pos) {
				var Position = {
					"longitude": pos.coords.longitude,
					"latitude": pos.coords.latitude
				};
				PositionCallBack(Position);
				window.locationService.stop(noop, noop);
			}, function(e) {
				alert("未找到GPS");
				PositionCallBack(null);
				window.locationService.stop(noop, noop)
			});
		}
	}
};
//取两数组交集
function GetArrayCros(arr1, arr2) {
		var hash = {},
			result = [];
		for (var i = 0; arr1[i] != null; i++) hash[arr1[i]] = true;
		for (var i = 0; arr2[i] != null; i++) {
			if (hash[arr2[i]]) {
				result.push(arr2[i])
			}
		}
		return result
	}
	//检测平台

function CheckPlatform() {
	//平台、设备和操作系统
	var system = {
		win: false,
		mac: false,
		xll: false
	};
	//检测平台
	var p = navigator.platform;
	system.win = p.indexOf("Win") == 0;
	system.mac = p.indexOf("Mac") == 0;
	system.x11 = (p == "X11") || (p.indexOf("Linux") == 0);
	//跳转语句
	if (system.win || system.mac) { //转向后台登陆页面
		return true;
	} else if (system.x11) {
		return false;
	}
};
//时间格式化
Date.prototype.format = function(format) {
	var o = {
		"M+": this.getMonth() + 1, //month 
		"d+": this.getDate(), //day 
		"h+": this.getHours(), //hour 
		"m+": this.getMinutes(), //minute 
		"s+": this.getSeconds(), //second 
		"q+": Math.floor((this.getMonth() + 3) / 3), //quarter 
		"S": this.getMilliseconds() //millisecond 
	}

	if (/(y+)/.test(format)) {
		format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
	}

	for (var k in o) {
		if (new RegExp("(" + k + ")").test(format)) {
			format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
		}
	}
	format = format.substring(0, format.length - 3);
	return format;
};
//时间格式化
Date.prototype.secondFormat = function(format) {
	var o = {
		"M+": this.getMonth() + 1, //month 
		"d+": this.getDate(), //day 
		"h+": this.getHours(), //hour 
		"m+": this.getMinutes(), //minute 
		"s+": this.getSeconds(), //second 
		"q+": Math.floor((this.getMonth() + 3) / 3), //quarter 
		"S": this.getMilliseconds() //millisecond 
	}

	if (/(y+)/.test(format)) {
		format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
	}

	for (var k in o) {
		if (new RegExp("(" + k + ")").test(format)) {
			format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
		}
	}
	//format = format.substring(0,format.length-3);
	return format;
};

/*序列化comments*/
function DealJson(data) {
	var _comments = [];
	if (data && data != "") {
		var _comments = data.split('|');
		var _length = _comments.length;
		for (var i = 0; i < _length; i++) {
			var _json = JSON.parse(_comments[i]);
			_comments[i] = _json;
		}
	}
	return _comments;
};
/*反序列化comments*/
function CDealJson(data) {
	if (data && data.length > 0) {
		var _length = data.length;
		for (var i = 0; i < _length; i++) {
			data[i] = JSON.stringify(data[i]);
		}
	}
	return data.join("|");
};

//判断图片是否存在 
function CheckImgExists(url, callback) {
	if (typeof(callback) == "function") {
		var httpRequest;

		if (window.XMLHttpRequest) { // Mozilla, Safari, ...

			httpRequest = new XMLHttpRequest();
			if (httpRequest.overrideMimeType) {
				httpRequest.overrideMimeType('text/xml');
			}
		} else if (window.ActiveXObject) { // IE

			try {
				httpRequest = new ActiveXObject("Msxml2.XMLHTTP");
			} catch (e) {
				try {
					httpRequest = new ActiveXObject("Microsoft.XMLHTTP");
				} catch (e) {}
			}
		}

		httpRequest.onreadystatechange = function() {
			if (httpRequest.readyState == 4 && httpRequest.status == 200)
				callback(url);
		};

		httpRequest.open('GET', url, true);
		httpRequest.send('');
	}
};

function LoadPicture(id, URL, IsNew, IsOrg) {
	try {
		var _type = 0;
		if (URL.indexOf("=") == -1) {
			_type = 1
		}
		var _pramas = {
			Id: id,
			Org: IsOrg
		}
		if (!IsNew) {
			DownLoad_New(URL, _type, _pramas, function(URI, pramas) {
				if (typeof(pramas.Org) != "undefined") {
					$("#" + pramas.Id).attr("href", URI);
				} else {
					$("#" + pramas.Id).attr("src", URI);
				}
			})
		}
	} catch (e) {
		alert(e);
	}

};

function LoadPictureNew(URL, id) {
	try {
		if (URL.substr(0, 8) == "file:///") {
			$("#" + id).attr("src", URL);
		} else {
			var _type = 0;
			if (URL.indexOf("=") == -1) {
				_type = 1
			}
			var _pramas = {
				Id: id
			}
			DownLoad_New(URL, _type, _pramas, function(URI, pramas) {
				$("#" + pramas.Id).attr("src", URI);
			})
		}

	} catch (e) {
		alert(e);
	}

};
var ImgURIjson = {};

function ImgGetPictureURI(imgurl, id, isorg) {
	if (imgurl && id && typeof(isorg) != "undefined") {
		if (ImgURIjson[id] && ImgURIjson[id].URL != imgurl) {
			return ImgURIjson[id].URI;
		} else {
			var _type = 0;
			if (imgurl.indexOf("=") == -1) {
				_type = 1
			}
			var _json = {
				ID: id,
				URL: imgurl,
				URI: "",
				IsOrg: isorg
			}
			DownLoad_New(_json.URL, _type, _json, function(URI, jsondata) {
				jsondata["URI"] = URI;
				//			var jsondata = _json
				//			jsondata["URI"] = "../img/head.jpg";
				ImgURIjson[jsondata.ID] = jsondata;
				if (jsondata.IsOrg) {
					$("#" + jsondata.ID).attr("href", jsondata.URI);
				} else {
					$("#" + jsondata.ID).attr("src", jsondata.URI);
				}
			})
		}
	}
	return false;
};