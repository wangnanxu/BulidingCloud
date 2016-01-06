///*参数*/
//var HSICurrentPage = 1;
//var HSICurrentNum = 0;
//var HSIPerSize = 10;
//var IsLoadHistory = {};
//var TotalPage = 0;
////页面显示后,获取地址栏参数
//$(document).on("pageshow", "#Page_SceneItem", function() {
//	TotalPage = 0;
//	InitSceneItem(IsAddSceneItem);
//
//	function IsAddSceneItem() {
//		var _type = GetUrlParam("type");
//		if (_type) {
//			UploadSceneShow(_type);
//		}
//	}
//});
///*
// * 解析本地图片
// */
//function AnalyzeImg(imgjson, istrue) {
//	if (imgjson != null) {
//		var _image = [];
//		var _length = imgjson.length;
//		for (var i = 0; i < _length; i++) {
//			_image.push(imgjson[i])
//		}
//		return _image.join("|");
//	} else {
//		return "";
//	}
//};
//
//var TEMCHCHE = "";
///*
// * 初始化数据
// */
//function InitSceneItem(callback) {
//	ChangeShow(); //根据权限，显示不同选项
//	HSICurrentNum = 0; //当前显示页
//	HSICurrentPage = 1;
//	IsLoadHistory[GetUrlParam("sceneid")] = true;
//	//	GetDataSceneItem(HSICurrentPage, HSIPerSize, callback);
//};
///*
// * 读取数据库数据，不存在则获取服务器
// * pageindex：当前页
// * pagesize:每页条数
// */
//function GetDataSceneItem(pageindex, pagesize, callback) {
//	GetData((pageindex - 1) * pagesize, pagesize, GetSceneItemCallBack); //查询前10条记录
//
//	function GetSceneItemCallBack(callbackdata) {
//		if (callbackdata.length == 0) {
//			if (TotalPage == 0 || pageindex <= TotalPage) {
//				PostServer();
//			}
//		} else {
//			if (callbackdata.length == pagesize) {
//				TotalPage += 1;
//				HSICurrentNum += callbackdata.length;
//				DealSceneItemData(callbackdata, true, callback);
//			} else {
//				//HSICurrentNum += callbackdata.length;
//				//DealSceneItemData(callbackdata, true, callback);
//				if (TotalPage == 0 || pageindex < TotalPage) {
//					PostServer();
//				}
//			}
//		}
//
//		function PostServer() {
//			if (IsLoadHistory[GetUrlParam("sceneid")] != false) {
//				var _jsonData = {
//					SceneID: GetUrlParam('sceneid'),
//					Token: userInfo.Token,
//					PageIndex: pageindex,
//					PageSize: pagesize
//				}
//				PostHistoryData(_jsonData, function(postcallback, count) {
//					if (count % 10 == 0) {
//						TotalPage = count / 10;
//					} else {
//						TotalPage = count / 10 + 1;
//					}
//					var _length = postcallback.length;
//					if (_length < pagesize) {
//						IsLoadHistory[GetUrlParam("sceneid")] = false;
//					}
//					HSICurrentNum += _length;
//					DealSceneItemData(postcallback, false, callback)
//				});
//			}
//		}
//	}
//};
///*
// * 处理数据
// * jsondata:数据
// * islocal：是否为本地数据
// */
//function DealSceneItemData(jsondata, islocal, callback) {
//	var _rsi = new RefreshSceneItem();
//	var _tem = AddSqliteTOSceneItem(jsondata);
//	if (islocal) {
//		if (typeof(callback) == "function") {
//			_rsi.Prepend(_tem, false);
//			callback();
//		} else {
//			_rsi.Prepend(_tem, true);
//		}
//	} else {
//		_rsi.Prepend(_tem, false);
//		if (typeof(callback) == "function") {
//			callback();
//		}
//	}
//};
///*
// * 处理数据单数据
// * jsondata:数据
// * islocal：是否为本地数据
// */
//function DealSceneItemDataFor(jsondata, islocal, callback) {
//	var _rsi = new RefreshSceneItem();
//	var _tem = AddSqliteTOSceneItemFor(jsondata);
//	if (islocal) {
//		if (typeof(callback) == "function") {
//			_rsi.Prepend(_tem, false);
//			callback();
//		} else {
//			_rsi.Prepend(_tem, true);
//		}
//	} else {
//		_rsi.Append(_tem, false);
//		if (typeof(callback) == "function") {
//			callback();
//		}
//	}
//};
///*
// * 根据SceneID查询数据库
// * limitmin:起始数
// * pagesize:数据条数
// * callback:回调函数
// */
//function GetData(limitmin, pagesize, callback) {
//	var _sql = "select tb_SceneMessage.Id,SceneID,UserID,UserPicture,UserPictureURI,UserName,Address,CreateTime,Description,Images,Status,Type,PictureGuid,CommentGuid,tb_Comment.CUserID,tb_Comment.CUserName,Content,Time from tb_SceneMessage left join tb_Comment on(tb_SceneMessage.Id = tb_Comment.Id) where SceneID=? order by CreateTime ";
//	dbBase.OpenTransaction(function(tx) {
//		dbBase.SelectTable(tx, _sql, [GetUrlParam("sceneid")], function(sqldata) {
//			var _jsondata = AssembleJson(sqldata, limitmin, limitmin + pagesize);
//			callback(_jsondata);
//		})
//	})
//};
//
///*
// * 组装Json,生成模板
// */
//function AssembleJson(sqldata, limitmin, limitmax) {
//	var _length = sqldata.length;
//	var _cache = {};
//	var _tem = [];
//	for (var i = _length - 1; i >= 0; i--) {
//		var _comment = [];
//		var _jsoncomment;
//		if (sqldata.item(i).CommentGuid != null) {
//			_jsoncomment = {
//				CommentGuid: sqldata.item(i).CommentGuid,
//				Id: sqldata.item(i).Id,
//				UserID: sqldata.item(i).CUserID,
//				UserName: sqldata.item(i).CUserName,
//				Content: sqldata.item(i).Content,
//				Time: sqldata.item(i).Time
//			}
//			_comment.unshift(_jsoncomment);
//		}
//		if (_cache[sqldata.item(i).Id] == undefined) {
//			var _item = sqldata.item(i);
//			var _arrayimgs = _item.Images.split('|');
//			var _newjson = {
//				Id: _item.Id,
//				SceneID: _item.SceneID,
//				UserID: _item.UserID,
//				UserPicture: _item.UserPicture,
//				UserPictureURI: _item.UserPictureURI,
//				UserName: _item.UserName,
//				Address: _item.Address,
//				CreateTime: _item.CreateTime,
//				Description: _item.Description,
//				Images: _arrayimgs,
//				Status: _item.Status,
//				Type: _item.Type,
//				PictureGuid: _item.PictureGuid,
//				Comments: _comment
//			}
//			_cache[_item.Id] = _newjson;
//		} else {
//			if (_jsoncomment != undefined) {
//				_cache[sqldata.item(i).Id].Comments.unshift(_jsoncomment);
//			}
//		}
//	}
//	var _num = 0;
//	for (x in _cache) {
//		if (_num >= limitmin && _num < limitmax) {
//			_tem.push(_cache[x]);
//		}
//		_num += 1;
//	}
//	return _tem;
//};
////上拉操作
//function PullSceneItem() {
//	//	var _remainder = HSICurrentNum % 10;//余数
//	//	var _quotient = HSICurrentNum / 10;//商数
//
//	HSICurrentPage = parseInt(HSICurrentNum / HSIPerSize) + 1;
//	GetDataSceneItem(HSICurrentPage, HSIPerSize);
//};
//
////界面切换签到签退状态
//function SceneItemSign() {
//	userInfo.Sign = userInfo.Sign == "1" ? "32" : "1";
//	var _signdata = userInfo.Sign == "1" ? "签到" : "签退";
//	$("#scene_Sign").html(_signdata);
//	UploadSceneShow(userInfo.Sign); //上传服务器
//};
////上传服务器前界面显示数据
//function UploadSceneShow(type) {
//	var datetime = new Date(); //本地时间
//	var _guid = NewGuid(); //生成Guid
//	var Json = { //post数据
//		Id: _guid,
//		SceneID: GetUrlParam("sceneid"),
//		UserID: userInfo.UserID,
//		UserPicture: userInfo.HeadPictureName,
//		SendUserHeadURI: userInfo.HeadPictureURI,
//		UserName: userInfo.UserName,
//		Address: "",
//		CreateTime: datetime.toLocaleDateString() + " " + datetime.toLocaleTimeString(),
//		Description: Description,
//		Images: ImgJsons,
//		Comments: "",
//		Status: "0",
//		Type: type
//	}
//	var _rsi = new RefreshSceneItem();
//	_rsi.Append(AddSqliteTOSceneItem([Json])); //显示数据，刷新滑动效果
//	UploadScene(Json); //上传
//};
////上传具体现场数据到服务器
//function UploadScene(Json) {
//	LoadImg(Json.Id); //添加loading效果
//	var imgnum = ImgJsons == null ? 0 : ImgJsons.length; //检测图片数量
//	if (imgnum > 0) { //存在图片，上传图片，成功后上传数据
//		UpLoadNum = imgnum;
//		for (var i = 0; i < imgnum; i++) {
//			UploadFile(ImgJsons[i], Json.Id, userInfo.Token, UploadCallback, 0);
//		}
//	} else {
//		UploadCallback(); //上传数据
//	}
//
//	function UploadCallback() { //闭包上传函数
//		GetPosition(function(_position) { //获取经纬度
//			var _jsondata = { //Post数据
//				SceneID: GetUrlParam("sceneid"),
//				Token: userInfo.Token,
//				Time: Json.CreateTime,
//				Address: _position.latitude + "|" + _position.longitude,
//				Type: Json.Type,
//				Content: Json.Description,
//				Count: imgnum,
//				Guid: Json.Id
//			};
//			PostAddSceneItem(_jsondata, _UPCallback); //Post操作
//		});
//	}
//
//	function _UPCallback(upvalue) { //Post成功回调函数
//		ReplaceMessageID(Json.Id, upvalue.MessageID, upvalue.Address); //替换数据操作
//		ImgJsons = null;
//		Description = "";
//	}
//};
