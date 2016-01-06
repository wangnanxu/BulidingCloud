var BUTTON_TEM = "<button id='{btnid}' class='ui-btn ui-btn-inline' onclick=DeleteSelect('{btnid}')>{btnname}</button>";
var zNodes = new Array();
var setting = {
	view: {
		selectedMulti: false,
		showLine: false
	},
	check: {
		enable: true
	},
	data: {
		simpleData: {
			enable: true
		}
	},
	callback: {
		onCheck: OnCheck,
		onExpand: RefeshScroll
	}
};
var sendMessage_Control; //发送消息控制
//显示页面时调用
$(document).on("pageshow", "#Page_SendMessage", function() {
	InitTree();
});

function InitTree() {
	//setTimeout(function() {
	var _str = zNodes.join(",");
	var _json = eval('[' + _str + ']');
	$.fn.zTree.init($("#treeDemo"), setting, _json);
	SetCheck();
	AddPaneScroll("down"); //加入滑动模块（@down开始在上面向下滑动）
	//}, 1000)
};

function RefeshScroll(event, treeId, treeNode) {
	if (myScroll) {
		myScroll.refresh();
	}
};

function OnCheck(e, treeId, treeNode) {
	ShowSelect(treeNode);
};
//发送数据
function SendMessageFun() {
	var _sdata = $("#sendData_id").val();
	var arr = new Array();
	$("#div_show").children().each(function() {
		try {
			var _str = $(this).attr('id');
			if (_str) {
				var _reg= /^[A-Za-z]+$/;
				var _id=$(this).attr('id').substr(0,1);
				if (_reg.test(_id) && _str.length >= 6) {//区别部门与人员
					arr.push($(this).attr('id'));
				}
			}
		} catch (e) {

		}
	})
	sendMessage_Control.SendUserMessage(_sdata, arr);
	$("#sendData_id").val("");
};

function SetCheck() {
	var _ztree = $.fn.zTree.getZTreeObj("treeDemo");
	_ztree.setting.check.chkboxType = {
		"Y": "ps",
		"N": "ps"
	};
};
//显示选中
function ShowSelect(treenode) {
	if (treenode.isParent == true) {
		var _nodes = treenode.children;
		for (var i = 0; i < _nodes.length; i++) {
			ShowSelect(_nodes[i]);
		}
	} else {
		var _reg= /^[A-Za-z]+$/;
		var _str=treenode.id.substr(0,1);
		if (!_reg.test(_str)) {
			treenode.checked = false;
			return;
		}
		if (treenode.checked == true) {
			var _tem = BUTTON_TEM;
			_tem = _tem.replace(/{btnid}/g, treenode.id);
			_tem = _tem.replace(/{btnname}/g, treenode.name);
			$("#div_show").append(_tem).trigger('create');
		}
		if (treenode.checked == false) {
			$("#" + treenode.id).remove();
		}
	}
};
/*
 * 点击名字删除选中
 * @id点击对象id
 */
function DeleteSelect(id) {
	var _ztree = $.fn.zTree.getZTreeObj("treeDemo");
	_ztree.checkNode(_ztree.getNodeByParam('id', id), false, true);
	var obj = document.getElementById(id);
	obj.remove();
};
var nodeItem = "{id:'{id}',pId:'{pid}',name:'{name}'}";
//组装企业
function AssembleEnterpriseTree() {
	if(zNodes){
		zNodes=null;
		zNodes=new Array();
	}
	if (userInfo) {
		var _item = nodeItem;
		_item = _item.replace(/{id}/g, userInfo.EnterpriseID);
		_item = _item.replace(/{name}/g, userInfo.EnterpriseName);
		_item = _item.replace(/{pid}/g, "0");
		zNodes.push(_item);
	}
};
//组装部门
function AssembleDepartmentTree(adata) {
	if (adata) {
		var _item = nodeItem;
		_item = _item.replace(/{id}/g, adata.DepartmentID);
		if (adata.ParentID == 0 || adata.ParentID == "0") {
			_item = _item.replace(/{pid}/g, userInfo.EnterpriseID);
		} else {
			_item = _item.replace(/{pid}/g, adata.ParentID);
		}
		_item = _item.replace(/{name}/g, adata.DepartmentName);
		zNodes.push(_item);
	}
};
//组装人员
function AssemblePersonTree(adata) {
	if (adata) {
		var _len = adata.length;
		for (var i = 0; i < _len; i++) {
			var _item = nodeItem;
			_item = _item.replace(/{id}/g, adata.item(i).UserID);
			if(adata.item(i).DepartmentID){
				_item = _item.replace(/{pid}/g, adata.item(i).DepartmentID);
			}else{
				_item = _item.replace(/{pid}/g, userInfo.EnterpriseID);
			}
			_item = _item.replace(/{name}/g, adata.item(i).UserName);
			zNodes.push(_item);
		}
	}
};
//测试数据
/*var zNodes= [{
	id: 1,
	pId: 0,
	name: "公司 1",
	open: true
}, {
	id: 11,
	pId: 1,
	name: "项目部",
	open: true
}, {
	id: 111,
	pId: 11,
	name: "张三"
}, {
	id: 112,
	pId: 11,
	name: "李四"
}, {
	id: 12,
	pId: 1,
	name: "工程部",
	open: true
}, {
	id: 121,
	pId: 12,
	name: "王五"
}, {
	id: 11,
	pId: 12,
	name: "赵六"
}, {
	id: 2,
	pId: 0,
	name: "公司 2",
	open: true
}, {
	id: 21,
	pId: 2,
	name: "工程部"
}, {
	id: 22,
	pId: 2,
	name: "田七",
	open: true
}, {
	id: 221,
	pId: 22,
	name: "刘八",
}, {
	id: 222,
	pId: 22,
	name: "孙九"
}, {
	id: 23,
	pId: 2,
	name: "运营部"
}];*/