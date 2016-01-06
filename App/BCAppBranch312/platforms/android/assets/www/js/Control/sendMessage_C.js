var SendMessageControl = function() {
	/*
	 * 发送信息
	 * @message信息类容
	 * @arr接收者userid数组
	 * @callback回调函数，当回复信息时需要显示有回调，当换页发送时无回调函数
	 */
	this.SendUserMessage = function(message, arr) {
			if (arr == null || arr.length < 1) {
				alert("请选择人员");
				return;
			}
			if (message == "" || message == null) {
				alert("发送类容为空");
				return;
			}
			arr.push(userInfo.UserID);
			dbBase.OpenTransaction(function(tx) {
				dbBase.SelectTable(tx, "select Time from tb_UserMessage order by Time DESC",[],function(adata){
					var _str = new Date().secondFormat("yyyy-MM-dd hh:mm:ss");
					var _time=_str;
					if(adata){
						var _date=new Date(adata.item(0).Time);
						_time=new Date(Math.abs(_date)+(30*1000)).secondFormat("yyyy-MM-dd hh:mm:ss");
					}
					sendMessageData = null;
					sendMessageData = {
						MessageID: NewGuid(),
						Token: userInfo.Token,
						Message: message,
						SendUserID: userInfo.UserID,
						Recipients: arr.join("|"),
						SendUserName: userInfo.UserName,
						SendUserPicture: userInfo.HeadImage,
						SendTime: _str,
						Time: _time,
						EnterpriseID: userInfo.EnterpriseID,
						Status: '0'
					};
					HandleSendMessage(sendMessageData, callback);
				})
			})

			function callback() {
				ChangePage("showMessage.html");
			}

		}
		/*
		 *
		 */
	this.SelectDepartment = function() {
		//组装企业
		AssembleEnterpriseTree();
		dbBase.OpenTransaction(function(tx) {
			dbBase.SelectTable(tx, "select * from tb_Departments where EnterpriseID=?", [userInfo.EnterpriseID], SelectPerson);
			/*
			 * 查询人员
			 */
			function SelectPerson(adata) {
				if (adata) {
					var _len = adata.length;
					for (var i = 0; i < _len; i++) {
						//组装部门
						AssembleDepartmentTree(adata.item(i));
					}
					dbBase.SelectTable(tx, "select * from tb_FrontUsers where EnterpriseID=? And UserID!=?", [userInfo.EnterpriseID, userInfo.UserID], AssemblePersonTree)

				}
			}

		})
	}
}