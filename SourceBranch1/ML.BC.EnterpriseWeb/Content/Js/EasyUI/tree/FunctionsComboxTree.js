/*获取功能数据*/
var  FunctionsDataUrl = '/Account/FunctionManagement/GetFunclistByTreeJson';
var FunctionsData = [];
function GetFunctionsData(oldvalue) {
    if (FunctionsData == null || FunctionsData.length == 0) {
        $.post(FunctionsDataUrl, "", function (json) {
            if (json != null) {
                //json = eval("(" + json + ")");
                FunctionsData = json;
                if (FunctionsData !=null) {
                    initTree(oldvalue)
                }
                if (FunctionsData.length == 0) {
                    showTipsMsg("没有功能数据!", 2000, 3);
                }
                 
            } else {
                //没有数据
                showTipsMsg("没有获取到功能数据!", 2000, 3);
            }
        });
    } else {
        initTree(oldvalue);
    }
    function initTree(oldvalue) {
        if (FunctionsData!=null) {
             //data-options="method:'get',animate:true,lines:true,id:id,text:Name,parentId:_parentId" 
            //parentId: '_parentId'
            $('#treeFunctions').combotree({
                animate: true, lines: true, checkbox: true, cascadeCheck: true
            });
            $('#treeFunctions').combotree("clear");
            $('#treeFunctions').combotree("loadData", FunctionsData);
            if (typeof (oldvalue) != "undefined") {
                $('#treeFunctions').combotree("setValues", oldvalue);//设置value
            }
        }
    }
}

//获取选择的功能
function GetSelectFunctions() {
    var t = $('#treeFunctions').combotree('tree');
    //t.tree('getSelected');
    var nodes = t.tree('getChecked');
    var s = '';
    //s = nodes.text;
    for (var i = 0; i < nodes.length; i++) {
        if (s != '') s += '|';
        s += nodes[i].id;
    }
    return s;
}