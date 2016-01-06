var setting = {
	view: {
		selectedMulti: false,
		showLine: false,
		
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
		onCheck: onCheck,
	}
};
var BUTTON_TEM = "<button id='{btnid}' class='ui-btn ui-btn-inline'>{btnname}</button>";

function onCheck(e, treeId, treeNode) {
	ShowCheck(treeNode);
}

var ShowCheck = function(treeNode) {
	if (treeNode.isParent == true) {
		var nodes = treeNode.children;
		for (var i = 0; i < nodes.length; i++) {
			ShowCheck(nodes[i]);
		}
	} else {
		if (treeNode.checked == true) {
			var tem = BUTTON_TEM;
			tem = tem.replace(/{btnid}/g, treeNode.id);
			tem = tem.replace(/{btnname}/g, treeNode.name);
			$("#div_show").append(tem).trigger('create');
		}
		if (treeNode.checked == false) {
			$("#" + treeNode.id).remove();
		}
	}
}

function setCheck() {
	var zTree = $.fn.zTree.getZTreeObj("treeDemo");
	zTree.setting.check.chkboxType = {
		"Y": "ps",
		"N": "ps"
	};
}
var zNodes = [{
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
	id: 122,
	pId: 12,
	name: "赵六"
}, {
	id: 2,
	pId: 0,
	name: "公司 2",
	checked: true,
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
	checked: true
}, {
	id: 222,
	pId: 22,
	name: "孙九"
}, {
	id: 23,
	pId: 2,
	name: "运营部"
}];

var code;