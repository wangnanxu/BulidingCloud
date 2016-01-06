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
		if (callback.length == 1) {
			var _obj = callback.item(0);
			if (CheckPlatform()) {
				_obj["HeadPictureURI"] = "../img/head.jpg";
				SetCurrentUser(_obj, LoginSuccess);
			} else {
				DownLoad_New(_obj.HeadImage, 1, "", function(URI) {
					_obj["HeadPictureURI"] = URI;
					SetCurrentUser(_obj, LoginSuccess);
				})

			}

		} else {
			return;
		}
	}
}

//账号登录
function Login() {
	var _logindata = GetLoginData(); //获取Login页面  账户，密码
	if (_logindata != null) {
		enterpriseids = new Array();
		//发送请求
		dbBase.OpenTransaction(function(tx) {
			dbBase.SelectTable(tx, "select EnterpriseID from tb_CurrentUsers order by EnterpriseID", [], function(adata) {
				if (adata) {
					var _len=adata.length;
					for(var i=0;i<_len;i++){
						enterpriseids[i]=adata.item(i).EnterpriseID;
					}
				}
				PostLoginData(_logindata, HandleLoginData);
			})
		})
	} else {
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
					SetCurrentUser(obj);
					if (_isfirst) {
						if ($.inArray(_data.EnterpriseID,enterpriseids)!=-1) {
							FirstLoginSuccess("1");//已有同企业账号登陆
						} else {
							FirstLoginSuccess("0");//企业账号第一次登陆
						}
					} else {
						LoginSuccess();//账号有登陆记录
					}
				}
			} else {
				DownLoad_New(_data.HeadImage, 1, "", function(URI) {
				_data["HeadPictureURI"] = URI;
				if (adata) {
					SetCurrentUser(obj);
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
				});
			}

		}
	}
};
//多次登陆跳转到信息页面

function LoginSuccess() {
		ChangePage("page/showMessage.html");
	}
	//第一次登陆跳转到数据同步页面

function FirstLoginSuccess(issameenterprise) {
	ChangePage("page/syncData.html?isSame=" + issameenterprise);
};

function OpenBrowser(){
	window.open(baseurl, "_system");
};
