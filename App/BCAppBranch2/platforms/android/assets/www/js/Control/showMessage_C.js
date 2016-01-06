//消息控制
var _isrefresh; //是否可以刷新历史
var messagePageSize = 10; //每次刷新条数
var MessageControl = function() {
	var _isfirst = true; //是否为第一次加载true是，false否
	var _historytime; //最旧显示消息时间
	this.SelectMessage = function(bool) {
			_isfirst = bool;
			//查询信息
			if (_isfirst) { //第一次加载
				//初始化
				message_Control.InitUserMessage();
			} else {
				//加载历史记录
				message_Control.AddHistoryMessage();
			}
		},
		//第一次加载页面
		this.InitUserMessage = function() {
			_isrefresh = true;
			dbBase.OpenTransaction(function(tx) {
				//查询数据库
				dbBase.SelectTable(tx, "select * from (select * from tb_UserMessage where EnterpriseID=? And (SendUserID=? OR Recipients like '%"+userInfo.UserID+"%') order by Time DESC) limit 0," + messagePageSize, [userInfo.EnterpriseID,userInfo.UserID], CallBack)
			})

			function CallBack(adata) {
				if (adata) {
					var _len = adata.length;
					_historytime = adata.item(_len - 1).Time;
					var _jsondata = ChangeSelectToJsondata(adata);
					AddMessageItem(_jsondata);
					if (_len < messagePageSize) {
						_isrefresh = false;
					}
				}
				//加入滑动模块（@up开始在最末端向上滑动）
				AddPaneScroll("up", message_Control.SelectMessage, false);
				//提前加载发送消息树形模块数据
				sendMessage_Control = new SendMessageControl();
				sendMessage_Control.SelectDepartment();
			}
		},
		//下拉加载历史记录，
		this.AddHistoryMessage = function() {
			if (_isrefresh) {
				dbBase.OpenTransaction(function(tx) {
					//查询历史数据
					dbBase.SelectTable(tx, "select * from (select * from tb_UserMessage where EnterpriseID=? And Time<? And (SendUserID=? Or Recipients like '%"+userInfo.UserID+"%' )order by Time DESC) limit 0," + messagePageSize, [userInfo.EnterpriseID,_historytime,userInfo.UserID], CallBackHistory);
				})
			}
			function CallBackHistory(adata) {
				if (adata) {
					var _len = adata.length;
					_historytime = adata.item(_len - 1).Time;
					var _jsondata = ChangeSelectToJsondata(adata);
					AddMessageItem(_jsondata);
					if (_len < messagePageSize) { //不足条数向服务器请求
						var _data={
							Token:userInfo.Token,
							QueryTime:_historytime,
							PageSize:messagePageSize
						}
						PostHistoryMessage(_data, ShowHistoryMessage);
					}
				}else{
					var _data={
							Token:userInfo.Token,
							QueryTime:_historytime,
							PageSize:messagePageSize
						}
						PostHistoryMessage(_data, ShowHistoryMessage);
				}
			}
		}
};
//服务器请求历史数据显示
function ShowHistoryMessage(jsondata) {
	if (jsondata) {
		//显示加载数据
		var _length = jsondata.length;
		if(_length>=1){
			_historytime = jsondata[_length - 1].Time;
			AddMessageItem(jsondata);
		}
		if (_length < messagePageSize) {
			_isrefresh = false;
			$("#pullDown").hide();
		}
	}
};
//连网自动发送消息
function AutoSendMessage() {
	dbBase.OpenTransaction(function(tx) {
		dbBase.SelectTable(tx,"select * from tb_UserMessage where SendUserID=? And Status=?",[userInfo.UserID,"0"], function(adata) {
			if (adata) {
				var _len = adata.length;
				for (var i = 0; i < _len; i++) {
					var _data={
						MessageID: adata.item(i).MessageID,
						Token: userInfo.Token,
						Message: adata.item(i).Message,
						Recipients: adata.item(i).Recipients,
						SendTime: adata.item(i).SendTime
					}
					PostSendMessage(_data);
				}
			}
		})
	})
}