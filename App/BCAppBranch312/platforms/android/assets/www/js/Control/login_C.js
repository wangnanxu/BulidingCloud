//Pgae显示前，检测数据库是否有key

//本地数据库通讯
var dbBase;
var enterpriseids; //已经登录过得所有用户企业id
/*
 * 检查本地数据库和账号信息
 */
function CheckAccount() {
	if (dbBase == null || dbBase == undefined) {
		dbBase = new DataBase();
	}
	if (dbBase.db == null) {
		//初始化数据库
		dbBase.OpenTransaction(function(tx) {
			dbBase.InitDB(tx);
		});
	}
	//LoginSuccess();//测试数据
	//查询账户信息获取登陆唯一码
	dbBase.OpenTransaction(function(tx) {
		dbBase.SelectTable(tx, "select * from tb_CurrentUsers where Token!=?", [""], _fCallback);
	});

	function _fCallback(callback) {
		$.mobile.loading("show", {            
			text: "登录中",
			            textVisible: true,
			            theme: $.mobile.loader.prototype.options.theme,
			            textonly: false,
			            html: ""    
		});
		if (callback.length == 1) {
			var _obj = callback.item(0);
			var _data = {
				Token: _obj.Token,
				LastTime: _obj.LastTime
			}
			PostConfirmDays(_data, postcallback)

			function postcallback(adata) {
				if (adata && adata.Value.Overdue) {
					AlertOverDue(adata);
				} else {
					if (CheckPlatform()) {
						_obj["HeadPictureURI"] = "../img/head.jpg";
						$.mobile.loading("hide");
						SetCurrentUser(_obj, LoginSuccess);
					} else {
						DownLoad_New(_obj.HeadImage, 1, "", function(URI) {
							if (URI == null) {
								URI = "../img/head.jpg";
							}
							_obj["HeadPictureURI"] = URI;
							SetCurrentUser(_obj, LoginSuccess);
						})
					}
				}
			}

		} else {
			$.mobile.loading("hide");
			return;
		}
	}
}

//账号登录
function Login() {
	$.mobile.loading("show", {            
		text: "登录中",
		            textVisible: true,
		            theme: $.mobile.loader.prototype.options.theme,
		            textonly: false,
		            html: ""    
	});
	var _logindata = GetLoginData(); //获取Login页面  账户，密码
	if (_logindata != null) {
		enterpriseids = new Array();
		//发送请求
		dbBase.OpenTransaction(function(tx) {
			dbBase.SelectTable(tx, "select EnterpriseID from tb_CurrentUsers order by EnterpriseID", [], function(adata) {
				if (adata) {
					var _len = adata.length;
					for (var i = 0; i < _len; i++) {
						enterpriseids[i] = adata.item(i).EnterpriseID;
					}
				}
				dbBase.SelectTable(tx, "select * from tb_CurrentUsers where UserID=?", [_logindata.UserName], function(adata) {
					if (adata) {
						_logindata.LastTime = adata.item(0).LastTime;
					}
					PostLoginData(_logindata, HandleLoginData);
				})
			})
		})
	} else {
		$.mobile.loading("hide");
		return;
	}
};
/*
 * 处理第一次请求数据
@adata为json对象数据
 */

function HandleLoginData(obj) {
	if (obj) {
		var _data = obj;
		var _isfirst = true;
		var _FunctionIDs = JSON.stringify(_data.FunctionIDs);
		dbBase.OpenTransaction(function(tx) {
			dbBase.SelectTable(tx, "select * from tb_CurrentUsers where UserID=?", [_data.UserID], function(adata) {
				if (adata) {
					_isfirst = false;
				}
				dbBase.SaveOrUpdateTable(tx, "tb_CurrentUsers", ['UserID', 'UserName', 'NickName', 'RoleIDs', 'EnterpriseID', 'EnterpriseName', 'DepartmentID', 'HeadImage', 'Token', 'FunctionIDs'], [_data.UserID, _data.UserName, '', _data.RoleIDs, _data.EnterpriseID, _data.EnterpriseName, _data.DepartmentID, _data.HeadImage, _data.Token, _FunctionIDs], 'UserID', _data.UserID, callback);
			});
		})

		function callback(adata) {
			if (CheckPlatform()) {
				_data["HeadPictureURI"] = "../img/head.jpg";
				if (adata) {
					SetCurrentUser(_data);
					if (_data.Overdue) {
						OverThreeDays(_data.UserID); //清除除登录用户已外的所有数据
						setTimeout(FirstLoginSuccess("0"), 2000);
					} else {
						if (_isfirst) {
							if ($.inArray(_data.EnterpriseID, enterpriseids) != -1) {
								FirstLoginSuccess("1"); //已有同企业账号登陆
							} else {
								FirstLoginSuccess("0"); //企业账号第一次登陆
							}
						} else {
							LoginSuccess(); //账号有登陆记录
						}
					}
				}
			} else {
				DownLoad_New(_data.HeadImage, 1, "", LoginCallback);

				function LoginCallback(URI) {
					if (URI == null) {
						URI = "../img/head.jpg";
					}
					_data["HeadPictureURI"] = URI;
					if (adata) {
						SetCurrentUser(_data);
						if (_data.Overdue) {
							OverThreeDays(_data.UserID); //清除除登录用户意外的所有数据
							setTimeout(FirstLoginSuccess("0"), 2000);
						} else {
							if (_isfirst) {
								if ($.inArray(_data.EnterpriseID, enterpriseids) != -1) {
									FirstLoginSuccess("1");
								} else {
									FirstLoginSuccess("0");
								}
							} else {
								LoginSuccess();
							}
						}
					} else {
						$.mobile.loading("hide");
					}
				}
			}

		}
	}
};
//多次登陆跳转到信息页面

function LoginSuccess() {
	$.mobile.loading("hide");
	ChangePage("page/showMessage.html");
}
//第一次登陆跳转到数据同步页面

function FirstLoginSuccess(issameenterprise) {
	$.mobile.loading("hide");
	ChangePage("page/syncData.html?isSame=" + issameenterprise);
};

function OpenBrowser() {
	window.open(baseurl, "_system");
};

function AlertOverDue(adata) {
	if (adata && adata.Value.Overdue) {
		if (CheckPlatform()) {
			OverDueDelete(1);
		} else {
			NotificationConfirm("该用户超过3天未登陆，请重新登陆", "提示", "确定,取消", OverDueDelete);
		}
	}
};

function OverDueDelete(button) {
	if (button == 1) {
		dbBase.OpenTransaction(function(tx) {
			dbBase.DeleteTable(tx, "tb_CurrentUsers");
			dbBase.DeleteTable(tx, "tb_UserMessage");
			dbBase.DeleteTable(tx, "tb_Departments");
			dbBase.DeleteTable(tx, "tb_Projects");
			dbBase.DeleteTable(tx, "tb_Scenes");
			dbBase.DeleteTable(tx, "tb_SceneMessage");
			dbBase.DeleteTable(tx, "tb_SceneMessageComments");
			dbBase.DeleteTable(tx, "tb_Materials");
			dbBase.DeleteTable(tx, "tb_UserRoles");
			dbBase.DeleteTable(tx, "tb_Comment");
			dbBase.DeleteTable(tx, "tb_FrontUsers");
			dbBase.DeleteTable(tx, "tb_SceneTypes");
		})
		setTimeout(ExitToLoading("1"), 6000);
	}
};
//账号密码超时3天登陆调用（清除除登录信息之外的所有数据）
function OverThreeDays(userid) {
	dbBase.OpenTransaction(function(tx) {
		dbBase.DeleteTable(tx, "tb_CurrentUsers", 'UserID !=?', [userid]);
		dbBase.DeleteTable(tx, "tb_UserMessage");
		dbBase.DeleteTable(tx, "tb_Departments");
		dbBase.DeleteTable(tx, "tb_Projects");
		dbBase.DeleteTable(tx, "tb_Scenes");
		dbBase.DeleteTable(tx, "tb_SceneMessage");
		dbBase.DeleteTable(tx, "tb_SceneMessageComments");
		dbBase.DeleteTable(tx, "tb_Materials");
		dbBase.DeleteTable(tx, "tb_UserRoles");
		dbBase.DeleteTable(tx, "tb_Comment");
		dbBase.DeleteTable(tx, "tb_FrontUsers");
		dbBase.DeleteTable(tx, "tb_SceneTypes");
	})
}