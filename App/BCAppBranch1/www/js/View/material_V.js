var MATERIAL_TEM="<li ><a alt='{materialid}' href='#' onclick='ShowMaterial({down})'><img  src='{pic}' alt='France' class='ui-li-icon ui-corner-none'>{name}</a></li>"
var UPDATEMATERIAL_TEM="<a alt='{materialid}' href='#' onclick='ShowMaterial({down})'><img  src='{pic}' alt='France' class='ui-li-icon ui-corner-none'>{name}</a>"
//文件类型图片
var TYPEJSON={
	word:"../img/word.png",
	excel:"../img/excel.gif",
	pdf:"../img/pdf.png"
}
$(document).on("pageshow", "#Page_Material", function() {
	SelectAllMaterial();
});
//显示所有
function ShowMaterialList(adata){
	if(adata){
		var _len=adata.length;
		var _alltem=new Array();
		for(var i=0;i<_len;i++){
			var _tem=MATERIAL_TEM;
			_tem=_tem.replace(/{down}/g,'"'+adata.item(i).Download+'"');
			var _type=adata.item(i).Type;
			_tem=_tem.replace(/{pic}/g,TYPEJSON[_type]);
			_tem=_tem.replace(/{name}/g,adata.item(i).MaterialName);
			_tem=_tem.replace(/{materialid}/g,adata.item(i).MaterialID);
			_alltem.push(_tem);
		}
		var el = document.getElementById('temp_Material');
		var _html=el.innerHTML;
		el.innerHTML=_html+_alltem.join("");
		$('#temp_Material').listview('refresh');
		AddPaneScroll("down");//加入滑动模块（@down开始在上面向下滑动）
	}
}
//跳转到具体下载页
function ShowMaterial(url){
	ChangePage("dialog.html?src="+url);
}
//添加资料
function ShowAddMaterial(jsondata){
	if(jsondata){
		var _alltem=new Array();
		var _len=jsondata.length;
		for(var i=0;i<_len;i++){
			var _tem=MATERIAL_TEM;
			_tem=_tem.replace(/{down}/g,'"'+jsondata(i).Download+'"');
			var _type=jsondata(i).Type;
			_tem=_tem.replace(/{pic}/g,TYPEJSON[_type]);
			_tem=_tem.replace(/{name}/g,jsondata(i).MaterialName);
			_tem=_tem.replace(/{materialid}/g,jsondata(i).MaterialID);
			_alltem.push(_tem);
		}
		var el = document.getElementById('temp_Material');
		var _html=el.innerHTML;
		el.innerHTML=_html+_alltem.join("");
		$('#temp_Material').listview('refresh');
		if(myScroll){
			myScroll.refresh();
		}
	}
}
//修改资料
function ShowUpdateMaterial(jsondata){
	if(jsondata && ($.mobile.activePage.attr("id")=='Page_Material')){
		var _len=jsondata.length;
		for(var i=0;i<_len;i++){
		var _litem=$("#temp_Material li a[alt='"+jsondata(i).MaterialID+"']");
		var _tem=UPDATEMATERIAL_TEM;
			_tem=_tem.replace(/{down}/g,'"'+jsondata(i).Download+'"');
			var _type=jsondata(i).Type;
			_tem=_tem.replace(/{pic}/g,TYPEJSON[_type]);
			_tem=_tem.replace(/{name}/g,jsondata(i).MaterialName);
			_tem=_tem.replace(/{materialid}/g,jsondata(i).MaterialID);
			_litem.innerHTML=_tem;
		}
		$('#temp_Material').listview('refresh');
	}
}
//删除资料
function ShowDeleteMaterial(jsondata){
	if(jsondata && ($.mobile.activePage.attr("id")=='Page_Material')){
		
	}
}

