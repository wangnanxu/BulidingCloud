//自己发送的消息模板
var CHATITEM_TEMM = "<li id='{messageid}'><div class='MyMessageLi'><div class='MyMessageLi'><span class='MessageTitle'>{time}</span></div><div class='MyMessageBody'><img id={picid} onload=LoadPicture(this.id,'{pic}') onerror=\"this.src='../img/head.jpg'\" src='{pic}' class='MyMessagebody-img'/><div class='MyMessagebody-link'></div><div class='MyMessagebody-div'><pre class='MyMessagebody-pre'>{data}</pre></div></div></div></li>";
//别人发送的消息模板
var CHATITEM_TEMY = "<li id='{messageid}'><div class='YouMessageLi'><div class='YouMessageLi'><span class='MessageTitle'>{time}</span></div><div class='YouMessageBody'><img id={picid} onload=LoadPicture(this.id,'{pic}') onerror=\"this.src='../img/head.jpg'\" src='{pic}' class='YouMessagebody-img' /><div class='YouMessagebody-link'></div><div class='YouMessagebody-div'><pre class='YouMessagebody-pre'>{data}<a href='#' onclick=ResponseMessage('{userid}') style='color: cornflowerblue;'>回复</a></pre></div></div></div></li>";
//公告模板
var CHATITEM_TEM = "<li id='{messageid}'><div class='YouMessageLi'><div class='YouMessageLi'><span class='MessageTitle'>{time}</span></div><div class='YouMessageBody'><img id={picid} onload=LoadPicture(this.id,'{pic}') onerror=\"this.src='../img/head.jpg'\" src='{pic}' class='YouMessagebody-img' /><div class='YouMessagebody-link'></div><div class='YouMessagebody-div'><pre class='YouMessagebody-pre'>{data}</pre></div></div></div></li>";
//未发送消息模板
var CHATITEM_SEND = "<li id='{messageid}'><div class='MyMessageLi'><div class='MyMessageLi'><span class='MessageTitle'>{time}</span></div><div class='MyMessageBody'><img id={picid} onload=LoadPicture(this.id,'{pic}') onerror=\"this.src='../img/head.jpg'\" src='{pic}' class='MyMessagebody-img'/><div class='MyMessagebody-link'></div><div class='MyMessagebody-div'><pre class='MyMessagebody-pre'>{data}</pre></div><div id={sendtime}><img src='../img/loadimg.gif' class='sendMessagePic'/></div></div></div></li>";

//空数据模板
var CHATNULLITEM_TEM = "<div><li></li></div>"
var message_Control; //消息控制
var sendMessageData; //发送消息
//加载showMessage.html成功后调用
$(document).on("pageshow", "#Page_ShowMessage", function() {
	if (workerPost == null) {
		AddWorker();
	}
	if (message_Control == null) {
		message_Control = new MessageControl();
	}
	message_Control.SelectMessage(true);
});
//进入发送数据页面
function SendMessage() {
	ChangePage('sendMessage.html');
	//测试message_View.AddChangeScene("");
};

//加载数据
function AddMessageItem(jsondata) {
	var el = document.getElementById('temp_Message');
	var _temall = new Array();
	if (jsondata == null) {
		_schatitem = CHATNULLITEM_TEM;
		_temall.push(_schatitem);
	} else {
		var _len = jsondata.length;
		for (var i = 0; i < _len; i++) {
			var _schatitem = AssembleShowHtml(jsondata[i]);
			_temall.unshift(_schatitem);
		}
		_temall.push(el.innerHTML);
	}
	el.innerHTML = _temall.join("");
	if (!_isrefresh) {
		$("#pullDown").hide();
	}
	//刷新滚动条
	if (myScroll) {
		myScroll.refresh();
	}
};
//修改信息页面头像tag=userid
function UpdateHeadPicture(uerid, picurl) {
	$("img[tag=" + uerid + "]").attr("src", picurl);
};
//回复数据
function ResponseMessage(userid) {
	navigator.notification.prompt(
		'回复内容!', // message
		BackMessage, // callback to invoke with index of button pressed
		'回复', // title
		['发送', '取消'],
		'' // buttonLabels
	);
	//点击发送
	function BackMessage(result) {
		if (result.buttonIndex == 1) { //发送
			dbBase.OpenTransaction(function(tx) {
				dbBase.SelectTable(tx, "select Time from tb_UserMessage order by Time DESC", [], function(adata) {
					var _str = new Date().secondFormat("yyyy-MM-dd hh:mm:ss");
					var _time=_str;
					if(adata){
						var _date=new Date(adata.item(0).Time);
						_time=new Date(Math.abs(_date)+(30*1000)).secondFormat("yyyy-MM-dd hh:mm:ss");
					}
					sendMessageData = {
						MessageID: NewGuid(),
						Token: userInfo.Token,
						Message: result.input1,
						SendUserID: userInfo.UserID,
						Recipients: userid + "|" + userInfo.UserID,
						SendUserName: userInfo.UserName,
						SendUserPicture: userInfo.HeadImage,
						SendTime: _str,
						Time: _time,
						EnterpriseID: userInfo.EnterpriseID,
						Status: '0'
					};
					HandleSendMessage(sendMessageData);
					//显示添加数据
					ShowAddUserMessage(sendMessageData);
				})
			})
		}
	}
};
//处理发送数据
function HandleSendMessage(data, callback) {
	//添加数据库
	PushAddSendMessage(data);
	//发送数据
	PostSendMessage(data, callback);
};
//显示添加数据
function ShowAddUserMessage(adata) {
	if (adata && ($.mobile.activePage.attr("id") == "Page_ShowMessage")) {
		var el = document.getElementById('temp_Message');
		var _temall = new Array();
		var _len = adata.length;
		if (_len) {
			for (var i = 0; i < _len; i++) {
				var _schatitem = AssembleShowHtml(adata[i]);
				_temall.unshift(_schatitem);
			}
		} else {
			var _schatitem = AssembleShowHtml(adata);
			_temall.unshift(_schatitem);
		}
		if (el) {
			var _html = el.innerHTML + _temall.join("");
			el.innerHTML = _html;
		}
		//刷新滚动条(此处不需要listview刷新)
		AddPaneScroll("up", message_Control.SelectMessage, false);
	}
};
//组装显示数据
function AssembleShowHtml(adata) {
	if (adata) {
		if ($("#" + adata.MessageID + adata.SendUserID).length > 0) {
			return;
		}
		var _schatitem;
		if (adata.Type == "公告") { //公告
			_schatitem = CHATITEM_TEM;
		} else if (adata.SendUserID != userInfo.UserID) { //别人
			_schatitem = CHATITEM_TEMY;
		} else { //自己
			_schatitem = CHATITEM_TEMM;
		}
		if (adata.Status == 0 || adata.Status == '0') {
			_schatitem = CHATITEM_SEND;
			_schatitem = _schatitem.replace(/{sendtime}/g, adata.MessageID);
		}
		_schatitem = _schatitem.replace(/{pic}/g, adata.SendUserPicture);
		_schatitem = _schatitem.replace(/{picid}/g, NewGuid());
		_schatitem = _schatitem.replace(/{messageid}/g, adata.MessageID + adata.SendUserID);
		_schatitem = _schatitem.replace(/{userid}/g, adata.SendUserID);
		_schatitem = _schatitem.replace(/{time}/g, adata.SendUserName + " " + adata.SendTime);
		_schatitem = _schatitem.replace(/{data}/g, adata.Message);
		return _schatitem;
	}
};
//修改发送成功数据
function UpdateSendDataShow(adata) {
	if (adata) {
		if ($("#" + adata.MessageID)) {
			$("#" + adata.MessageID).hide();
		}
		if (myScroll) {
			myScroll.refresh();
		}
	}
};
//删除发送数据
function DeleteSendDataShow(adata) {
	if (adata) {
		if ($("#" + adata.MessageID + adata.SendUserID)) {
			$("#" + adata.MessageID + adata.SendUserID).remove();
		}
		if (myScroll) {
			myScroll.refresh();
		}
	}
}