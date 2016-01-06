//现场动态加载模板
var SCENE_TEM = "<li ><a href='#' alt='{sFilid}' onclick=ClickSceneItem('{sFilid}','{statusid}')>{proname}<span class='ui-li-count'>{status}</span></a></li>";
var UPDATESCENE_TEM="<a href='#' alt='{sFilid}' onclick=ClickSceneItem('{sFilid}','{statusid}')>{proname}<span class='ui-li-count'>{status}</span></a>"
var EMPET_ITEM="<li><div>无现场</div></li>"
var projectId; //项目ID
var sceneId; //点击现场ID
var parentId; //父现场ID没有父现场ID为0;
var projectmanager;//项目管理者
var sceneState;//现场状态
var scene_Control; //现场控制
var scene_View; //现场视图
//跳转到添加现场页面
function GotoAddScene() {
	ChangePage('addScene.html');
};
//project.html成功后调用
$(document).on("pagebeforeshow", "#Page_Scene", function() {
	if (scene_Control == null) {
		scene_Control = new SceneControl();
	}
	if (scene_View == null) {
		scene_View = new SceneView();
	}
	if(GetUrlParam('manager')){
		projectmanager=GetUrlParam('manager')
	}
	projectId = GetUrlParam('projectid');
	sceneId = GetUrlParam('parentid');
	if (sceneId) {
		scene_Control.SelectChildScene(sceneId);
	} else {
		sceneId = "";
		scene_Control.SelectAllScene(projectId);
	}
	if($.inArray("Root.AppPermission.SceneManage.AddScene", userInfo.FunctionIDs)!=-1){
		$("#id_addScene").show();
	}else{
		$("#id_addScene").hide();
	}
});
var SceneView = function() {
	//显示现场
	this.ShowAllScenes = function(adata) {
			if (adata) {
				$("#temp_Scene").val = "";
				scene_View.AddSceneData(adata);
			}else{
				$("#temp_Scene").val = "";
				$("#temp_Scene").html(EMPET_ITEM);
				$('#temp_Scene').listview('refresh');
			}
		},
		//显示子现场

		this.ShowScenes = function(adata) {
			if (adata == false && isHaveScene==false) {
				GotoSceneItem();
				return;
			} else {
				$("#temp_Scene").val = "";
				if (adata.length >= 1) {
					scene_View.AddSceneData(adata);
				}else{
					$("#temp_Scene").html(EMPET_ITEM);
					$('#temp_Scene').listview('refresh');
				}
			}
		},
		//显示现场

		this.AddSceneData = function(adata) {
			var _temall = new Array();
			for (var i = 0; i < adata.length; i++) {
				var _tem = SCENE_TEM;
				_tem = _tem.replace(/{sFilid}/g, adata.item(i).SceneID);
				_tem = _tem.replace(/{proname}/g, "[现场]" + adata.item(i).SceneName);
				_tem = _tem.replace(/{status}/g, const_state[adata.item(i).SceneState]);
				_tem = _tem.replace(/{statusid}/g, adata.item(i).SceneState);
				parentId = adata.item(i).ParentID;
				_temall.push(_tem);
			}
			$("#temp_Scene").html(_temall.join(""));
			$('#temp_Scene').listview('refresh');
			AddPaneScroll("down");
		}
};
//选中现场

function ClickSceneItem(sceneid,state) {
	sceneId = sceneid;
	sceneState=state;
	scene_Control.SelectChildScene(sceneId);
};
//跳转到具体现场页

function GotoSceneItem() {
	ChangePage('sceneItem.html?parentid=' + parentId + "&projectid=" + projectId + "&sceneid=" + sceneId+"&state="+sceneState);
};
//返回父现场

function GoBackScene() {
	if (parentId == -1 || parentId == "-1" || parentId == null) {
		ChangePage('project.html');
	} else {
		scene_Control.SelectParentScene(parentId);
	}
};
//添加现场
function ShowAddScene(jsondata) {
	if (jsondata && ($.mobile.activePage.attr("id") == 'Page_Project')) {
		var _len = jsondata.length;
		for (var i = 0; i < _len; i++) {
			if (jsondata(i).ProjectID == projectId) { //确定在该项目下
				if (sceneId == null || sceneId == "") { //第一级现场
					AddScene(jsondata(i));
				} else if (jsondata(i).ParentID == sceneId) { //显示现场下的子现场
					AddScene(jsondata(i));
				}
			}
		}
	}
};
//修改现场
function ShowUpdateScene(jsondata) {
	if (jsondata && ($.mobile.activePage.attr("id") == 'Page_Project')) {
		var _len=jsondata.length;
		for(var i=0;i<_len;i++){
			var _li=$("li a[alt='"+jsondata(i).SecneID+"']");
			var _tem=UPDATESCENE_TEM;
			_tem = _tem.replace(/{sFilid}/g, jsondata(i).SceneID);
			_tem = _tem.replace(/{proname}/g, "[现场]" + jsondata(i).SceneName);
			_tem = _tem.replace(/{status}/g, const_state[jsondata(i).SceneState]);
			_tem = _tem.replace(/{statusid}/g, adata.item(i).SceneState);
			_li.parentElement.innerHTML=_tem;
		}
		$('#temp_Scene').listview('refresh');
	}
};
//删除现场
function ShowDeleteScene(jsondata) {
	if (jsondata && ($.mobile.activePage.attr("id") == 'Page_Project')) {
		var _len=jsondata.length;
		for(var i=0;i<_len;i++){
			var _li=$("li a[alt='"+jsondata(i).SecneID+"']");
			_li.parentElement.remove();
		}
	}
};
//追加现场
function AddScene(jsondata) {
	var _tem = SCENE_TEM;
	_tem = _tem.replace(/{sFilid}/g, jsondata.SceneID);
	_tem = _tem.replace(/{proname}/g, "[现场]" + jsondata.SceneName);
	_tem = _tem.replace(/{status}/g, const_state[jsondata(i).SceneState]);
	_tem = _tem.replace(/{statusid}/g, adata.item(i).SceneState);
	parentId = jsondata.ParentID;
	_tem = $("#temp_Scene").innerHTML + _tem;
	$("#temp_Scene").html(_tem);
	$('#temp_Scene').listview('refresh');
	if(myScroll){
			myScroll.refresh();
		}
}