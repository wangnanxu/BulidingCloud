//项目列表模板
var PROJECT_TEM = "<li ><a href='#' alt='{proid}' onclick=GotoScene('{proid}','{manager}')>{proname}<span class='ui-li-count'>{status}</span></a></li>";

/*
 * 进入具体项目，显示现场页
 * @projectId进入项目ID
 */
function GotoScene(projectId,manager) {
		ChangePage('scene.html?projectid=' + projectId+"&manager="+manager);
	}
	//project.html成功后调用
$(document).on("pageshow", "#Page_Project", function() {
	$("#NullPro li").show();
	SelectProject();
});
//加载显示数据
function ShowProject(adata) {
	if (adata == false) {
		return;
	} else if (adata.length >= 1) {
		var _temall =new Array();
		for (var i = 0; i < adata.length; i++) {
			var _tem = PROJECT_TEM;
			//按照用户权限、项目状态、是否有现场划分是否显示项目
			if(userInfo.RoleID=="11" && (adata.item(i).Status=="完成" || adata.item(i).HaveScene==false)){
				continue;
			}
			_tem = _tem.replace(/{proid}/g, adata.item(i).ProjectID);//项目ID
			_tem = _tem.replace(/{proname}/g,"[项目]"+adata.item(i).ProjectName);//项目名称
			_tem = _tem.replace(/{status}/g, const_state[adata.item(i).ProjectState]);//项目状态
			_tem = _tem.replace(/{manager}/g, adata.item(i).Manager);//项目状态
			_temall.push(_tem);
		}
		$("#temp_Project").html(_temall.join(""));
		$('#temp_Project').listview('refresh');
		$("#NullPro").hide();
	}
	AddPaneScroll("down");//加入滑动模块（@down开始在上面向下滑动）
}
//显示添加
function ShowAddProject(jsondata){
	if(jsondata && ($.mobile.activePage.attr("id")=='Page_Project')){
		var _temall =new Array();
		var _len=jsondata.length;
		for(var i=0;i<_len;i++){
			var _tem=PROJECT_TEM;
			if(userInfo.RoleID=="11" && (jsondata(i).Status=="完成" || jsondata(i).HaveScene==false)){
				continue;
			}
			_tem = _tem.replace(/{proid}/g, jsondata(i).ProjectID);//项目ID
			_tem = _tem.replace(/{proname}/g,"[项目]"+jsondata(i).ProjectName);//项目名称
			_tem = _tem.replace(/{status}/g, const_state[jsondata(i).ProjectState]);//项目状态
			_tem = _tem.replace(/{manager}/g, adata.item(i).Manager);//项目状态
			_temall.unshift(_tem);
		}
		_temall=$("#temp_Project").innerHTML+_temall;
		$("#temp_Project").html(_temall.join(""));
		$('#temp_Project').listview('refresh');
		if(myScroll){
			myScroll.refresh();
		}
	}
}
//显示变更
function ShowUpdateProject(jsondata){
	if(jsondata && ($.mobile.activePage.attr("id")=='Page_Project')){
		var _len=jsondata.length;
		for(var i=0;i<_len;i++){
			var _item=$("#temp_Project li a[alt='"+jsondata(i).ProjectID+"']");
			_item.attr("value",jsondata(i).ProjectName);//变更项目名
			var _itemspan=$("#temp_Project li a[alt='"+jsondata(i).ProjectID+"'] span");
			_itemspan.attr("value",jsondata(i).Status);//变更状态
		}
		$('#temp_Project').listview('refresh');
	}
}
//显示删除
function ShowDeleteProject(jsondata){
	if(jsondata  && ($.mobile.activePage.attr("id")=='Page_Project')){
		var _len=jsondata.length;
		for(var i=0;i<_len;i++){
			var _item=$("#temp_Project li a[alt='"+jsondata(i).ProjectID+"']");
		 _item.parentElement.remove();//删除项目
		}
	}
}
