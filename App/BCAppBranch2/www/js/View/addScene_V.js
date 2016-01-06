//列表选择模板
var OPTION_TEM = "<option value='{val}'>{name}</option>";
//角色人员列表模板
var PERSON_TEM = "<div class='ui-field-contain'><div class='SelectLable'><label for='select-native-1'>{type}</label></div><div class='SelectItem'><select id='{typeid}' name='select-native-1' id='select-native-1' data-native-menu='false' multiple='multiple'>{option}</select></div></div>"
	//没有角色人员列表模板
var NOPERSON_TEM = "<div class='ui-field-contain'><div class='SelectLable'><label for='select-native-1'>{type}</label></div><div class='SelectItem'><select id='{typeid}' name='select-native-1' id='select-native-1' onclick='Tanchukuang()'>{option}</select></div></div>"

var addScene_Control; //增加现场控制
var alltem = "";
var sceneWorker; //现场原始worker
var isShowPerson; //是否显示角色
$(document).on("pageshow", "#Page_AddScene", function() {
	if (addScene_Control == null) {
		addScene_Control = new AddSceneControl();
	}
	AddSceneTypeControl();
	ChoiceDepartments();
	var _sceneid = GetUrlParam('sceneid');
	if (_sceneid) { //修改现场可选择父级
		$("#scene_add").remove();
	} else { //新增现场不能选择父级
		$("#scene_update").remove();
		addScene_Control.SelectProjectAndScenes();
	}
	isShowPerson = false;
	$("#id_ScenePerson").hide();
	//刷新滚动条
	AddPaneScroll("down");
});
//显示项目现场(未完成)
var ProjectAndScene = function() {
	this.AddProject = function(adata) {
		var _tem = OPTION_TEM;
		_tem = _tem.replace(/{name}/g, "[项目]" + adata.item(0).ProjectName);
		_tem = _tem.replace(/{val}/g, adata.item(0).ProjectID + "|-1");
		alltem += _tem;
		$("#id_parent").html(alltem);
		$("#id_parent").selectedIndex = 0;
		//刷新选择列表
		$("#id_parent").selectmenu('refresh');
	}
	this.AddScene = function(adata) {
		//value值存放格式为：projectID+"|"+sceneID;
		if (adata.length >= 1) {
			for (var i = 0; i < adata.length; i++) {
				var _tem = OPTION_TEM;
				_tem = _tem.replace(/{name}/g, "[现场]" + adata.item(i).SceneName);
				_tem = _tem.replace(/{val}/g, adata.item(i).ProjectID + "|" + adata.item(i).SceneID);
				alltem += _tem;
			}
			alltem = $("#id_parent").innerHTML + alltem;
			$("#id_parent").html(alltem);
			//默认为父级显示
			$("#id_parent")[0].selectedIndex = 0;
			//刷新选择列表
			$("#id_parent").selectmenu('refresh');
		}
	}
};
//显示人员类型(未完成)
var Person = function() {
	this.AddOptions = function(adata) {
		//alert("Options=" + adata.user);
		var _users = adata.user;
		var _len = _users.length;
		var _tem = "";
		var _alltem = new Array();
		if (_len > 0) {
			_tem = PERSON_TEM;
			for (var i = 0; i < _len; i++) {
				var _option = OPTION_TEM;
				_option = _option.replace(/{val}/g, _users[i].UserID);
				_option = _option.replace(/{name}/g, _users[i].UserName);
				_alltem.push(_option);
			}
			_tem = _tem.replace(/{option}/g, _alltem.join(""));
		} else {
			_tem = NOPERSON_TEM;
			_tem = _tem.replace(/{option}/g, "无角色人员");
		}
		_tem = _tem.replace(/{type}/g, adata.type);
		_tem = _tem.replace(/{typeid}/g, adata.typeID);
		
		$("#id_ScenePerson").append(_tem);
		$("#temp_AddScene").trigger('create');
		//刷新滚动条
		if (myScroll) {
			myScroll.refresh();
		}
	}
};
//显示现场类型()
function ShowSceneType(adata) {
	if (adata) {
		var _len = adata.length;
		var _allitem = new Array();
		for (var i = 0; i < _len; i++) {
			var _option = OPTION_TEM;
			_option = _option.replace(/{val}/g, adata.item(i).SceneTypeID);
			_option = _option.replace(/{name}/g, adata.item(i).SceneTypeName);
			_allitem.push(_option);
		}
		$("#id_scenetype").html(_allitem);
		$("#id_scenetype")[0].selectedIndex = 0;
		//刷新选择列表
		$("#id_scenetype").selectmenu('refresh');
	}
	if (myScroll) {
		myScroll.refresh();
	}
};
//添加现场
function AddSceneComplete() {
	var _data = AssemblePostData('0');
	if (_data != null) {
		//写入本地数据库
		PushAddNoSendScene(_data);
		//向服务器请求添加现场
		PostAddScene(_data, BackScenes);
	}
};
//修改现场
function UpdateSceneComplete() {
	var _data = AssemblePostData('2');
	if (_data != null) {
		//写入本地数据库
		PushAddNoSendScene(_data);
		//向服务器请求添加现场
		PostUpdateScene(_data, BackScenes);
	}
};
//组装数据
function AssemblePostData(status) {
	var _sendData = {
		SceneID: '',
		ProjectID: '',
		ParentID: '',
		SceneName: '',
		Address: '',
		BeginDate: '',
		EndDate: '',
		SceneType: '',
		SceneWorker: '',
		SceneState: '',
		SendStatus: status,
		HasData: 'false',
		EnterpriseID: userInfo.EnterpriseID,
		Token: userInfo.Token
	}
	if (GetUrlParam('sceneid')) {
		_sendData.SceneID = GetUrlParam('sceneid');
	} else {
		_sendData.SceneID = NewGuid();
	}
	_sendData.SceneName = $("#id_name").val(); //现场名
	var _str = $("#id_parent").find("option:selected").val();
	var _arr = _str.split("|");
	if (_arr.length >= 2) {
		_sendData.ProjectID = _arr[0];
		_sendData.ParentID = _arr[1];
	}
	_sendData.Address = $("#id_address").val(); //现场地址
	_sendData.SceneType = $("#id_scenetype").val().join("|"); //现场类型
	_sendData.SceneState = $("#id_status").find("option:selected").val(); //现场状态
	_sendData.BeginDate = $("#id_beginDate").val(); //开始时间
	_sendData.EndDate = $("#id_endDate").val(); //结束时间
	if (isShowPerson) {
		var _isNull = false;
		var _worker = new Array(); //工作人员UserID
		//遍历所有选择列表(未完成)
		$("#id_ScenePerson .ui-field-contain").each(function() {
			var _data = {
				roleId: '',
				UserID: ''
			}
			var _arr = new Array();
			_data.roleId = $(this).find('select').attr("id");
			var _str = $(this).find('select').val();
			_data.UserID = _str;
			if ((_data.UserID == "" || _data.UserID == null) && GetUrlParam('sceneid')) {
				_isNull = true;
			}
			_worker.push(_data);
		})
		if (_isNull && GetUrlParam('sceneid') && sceneWorker) {
			alert("请选择人员");
			return null;
		} else {
			_sendData.SceneWorker = JSON.stringify(_worker);
		}
	} else {
		if (sceneWorker) {
			_sendData.SceneWorker = sceneWorker;
		}
	}
	if (_sendData.SceneName == "") {
		alert("现场名为空");
		return null;
	}
	if (_sendData.ProjectID == "" || _sendData.ParentID == "") {
		alert("请选择项目或现场");
		return null;
	}
	if (_sendData.Address == "") {
		alert("请填写现场地址");
		return null;
	}
	if (_sendData.SceneType == "" || _sendData.SceneType == null) {
		alert("请选择现场类型");
		return;
	}
	//alert(JSON.stringify(_sendData))
	return _sendData;
};
//返回进入现场页面
function BackScenes() {
	if (parentId && parentId != "-1") {
		ChangePage('scene.html?projectid=' + projectId + '&parentid=' + parentId + '&sceneid=' + GetUrlParam('sceneid'));
	} else {
		ChangePage('scene.html?projectid=' + projectId);
	}
};
//修改显示现场名
function ShowSceneInfo(adata) {
	if (adata) {
		var _jsondata = adata.item(0);
		$("#id_name").val(_jsondata.SceneName);
		$("#id_address").val(_jsondata.Address);
		if (_jsondata.BeginDate) {
			var _begin = _jsondata.BeginDate.substring(0, 10)
			$("#id_beginDate").val(_begin);
		}
		if (_jsondata.EndDate) {
			var _end = _jsondata.EndDate.substring(0, 10)
			$("#id_endDate").val(_end);
		}
		//设置类型
		if (_jsondata.SceneType) {
			var _arr = _jsondata.SceneType.split("|");
			$("#id_scenetype").val(_arr);
			$("#id_scenetype").selectmenu('refresh');
		}
		//设置状态
		if (_jsondata.SceneState) {
			SetSelectItem($("#id_status option"), _jsondata.SceneState);
			//刷新选择列表
			$("#id_status").selectmenu('refresh');
		}
		//设置worker
		if (_jsondata.SceneWorker) {
			sceneWorker = _jsondata.SceneWorker;
			try {
				var _arr = JSON.parse(_jsondata.SceneWorker);
				var _len = _arr.length;
				for (var i = 0; i < _len; i++) {
					var typeid = _arr[i].roleId;
					var _value = _arr[i].UserID
					$("#" + typeid).val(_value);
					$("#" + typeid).selectmenu('refresh');
				}
			} catch (e) {

			}
		}
	}
};
/*
 * 设置状态、类型、worker
 * @selobj设置对象
 * @selval设置值
 */
function SetSelectItem(selobj, selval) {
	for (var i = 0; i < selobj.size(); i++) {
		if (selobj[i].value == selval) {
			selobj[i].selected = true;
			return;
		}
	}
};

function ToggleShowPerson() {
	if ($("#id_isShow").is(":checked")) {
		isShowPerson = true;
		$("#id_ScenePerson").show();
	} else {
		isShowPerson = false;
		$("#id_ScenePerson").hide();
	}
	//刷新滚动条
	AddPaneScroll("up");
};

function Tanchukuang(){
	NotificationAlert("无角色人员","提示");
}
