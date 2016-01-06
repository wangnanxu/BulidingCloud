function Current_iframeID() {
}

document.onkeydown = function (e) {
    if (!e) e = window.event;
    if ((e.keyCode || e.which) == 13) {
        var btnSearch = document.getElementById("btnSearch");
        if (btnSearch != null) {
            btnSearch.focus();
            btnSearch.click();
        }
    }
}

$(document).ready(function () {
    Loading(true);
});

//切换验证吗
function ToggleCode(obj, codeurl) {
    $("#txtCode").val("");
    $("#" + obj).attr("src", codeurl + "?time=" + Math.random());
}
function windowload() {
    rePage();
}
function rePage() {
    Loading(true);
    window.location.href = window.location.href.replace('#', '');
    return false;
}
function Replace() {
    Loading(true);
    window.location = window.location;
    return false;
}
function bntback() {
    window.history.back(-1);
    Loading(true)
}
function Urlhref(url) {
    Loading(true);
    window.location.href = url;
    return false;
}
//关闭窗口
function ThisCloseTab() {
    var tab = top.$('#ContentPannel').tabs('getSelected');
    if (tab) {
        var index = top.$('#ContentPannel').tabs('getTabIndex', tab);
        top.$('#ContentPannel').tabs('close', index);
    }
}
function Loading(bool) {
    if (bool) {
        $("#Loading").show();
    } else {
        setTimeout(loadinghide, 900);
    }
}
function loadinghide() {
    $("#Loading").hide();
}

//内页加载动画控制
function showLoadingDlg() {
    $("#Loading-small-dlg").show();
}
function hideLoadingDlg() {
    $("#Loading-small-dlg").hide();
}
var showtime = 1000; var hidetime = 1000;
//在显示顶部消息_2:msg:消息内容,time:时间秒,type:消息类型:0成功的,1等待中的,2错误类的,3普通or其他的提示
function showTopMsg(msg, time, type) {
    MsgTips(msg, time, 500, type);
}
function BeautifulGreetings() {
    var now = new Date();
    var hour = now.getHours();
    if (hour < 3) {
        return ("夜深了,早点休息吧！")
    } else if (hour < 9) {
        return ("早上好！")
    } else if (hour < 12) {
        return ("上午好！")
    } else if (hour < 14) {
        return ("中午好！")
    } else if (hour < 18) {
        return ("下午好！")
    } else if (hour < 23) {
        return ("晚上好！")
    } else {
        return ("夜深了,早点休息吧！")
    }
}

//在显示顶部消息_3:msg:消息内容,time:时间秒,type:消息类型:0成功的,1等待中的,2错误类的,3普通or其他的提示
function showTipsMsg(msg, time, type) {
    top.showTopMsg(msg, time, type);
}

//显示对话框
function showFaceMsg(msg) {
    top.art.dialog({
        id: 'faceId',
        title: '温馨提醒',
        content: msg,
        icon: 'face-smile',
        time: 10,
        background: '#000',
        opacity: 0.1,
        lock: true,
        okVal: '关闭',
        ok: true
    });
}
//显示等待信息,对话框
function showWarningMsg(msg) {
    top.art.dialog({
        id: 'warningId',
        title: '系统提示',
        content: msg,
        icon: 'warning',
        time: 10,
        background: '#000',
        opacity: 0.1,
        lock: true,
        okVal: '关闭',
        ok: true
    });
}
//显示确认框
function showConfirmMsg(msg, callBack) {

}

//编辑时候调用 id:需要验证的值
function IsEditData(id) {
    var isOK = true;
    if (typeof (id) == "undefined" || id == "") {
        isOK = false;
        showTipsMsg("很抱歉，您当前未选中任何一行！", 2000, 2);
    } else if (id.split(",").length > 1) {
        isOK = false;
        showTipsMsg("很抱歉，一次只能选择一条记录！", 2000, 2);
    }
    return isOK;
}
//删除时候
function IsDelData(id) {
    var isOK = true;
    if (id == undefined || id == "") {
        isOK = false;
        showTipsMsg("很抱歉，您当前未选中任何一行！", 2000, 2);
    }
    return isOK;
}

//是否为空值
function IsNullOrEmpty(str) {
    var isOK = true;
    if (typeof (str) == "undefined" || str == "") {
        isOK = false;
    }
    return isOK;
}

function getAjax(url, parm, callBack) {
    $.ajax({
        type: 'post',
        dataType: "text",
        url: url,
        data: parm,
        cache: false,
        async: false,
        success: function (msg) {
            callBack(msg);
        }
    });
}

//获取复选框的值
function CheckboxValue() {
    var reVal = '';
    $('[type = checkbox]').each(function () {
        if ($(this).attr("checked")) {
            reVal += $(this).val() + ",";
        }
    });
    reVal = reVal.substr(0, reVal.length - 1);
    return reVal;
}
function divresize(element, height) {
    resizeU();
    $(window).resize(resizeU);
    function resizeU() {
        $(element).css("height", $(window).height() - height);
    }
}

function iframeresize(height) {
    if (height == undefined) {
        height = 0;
    }
    resizeU();
    $(window).resize(resizeU);
    function resizeU() {
        var iframeMain = $(window).height();
        $("#iframeMainContent").height(iframeMain - height);
    }
}

//在显示顶部消息_1:msg:消息内容,time:时间秒,speed:显示和隐藏的速度,type:消息类型:0成功的,1等待中的,2错误类的,3普通or其他的提示
function MsgTips(msg, time, speed, type) {
    var stime = showtime;
    var htime = hidetime;
    if (speed != 0) {
        stime = 0;//直接显示
        htime = 0;
    }
    if (time == 0) {
        time = 4000;
    }
    var msgid = "SysMsg_";
    if (type >= 0) {
        msgid = msgid + type;
    }
    if (type == 0) {
    } else if (type == 1) {
    } else if (type == 2) {
    } else if (type == 3) {
    } else {
    }
    top.$("#" + msgid).hide();
    //top.$("#" + msgid).stop(); 

    top.$("#" + msgid).html(msg);
    top.$("#" + msgid).show(stime, function () {
        //隐藏消息
        setTimeout(function () {
            top.$("#" + msgid).hide(htime); //alert('隐藏:' + msgid);
        }, time);
    });

}

//显示是否有效
function GetAvailableText(value, row, index) {
    if (typeof (value) == "undefined") {
        return "否";
    }
    if (value == true || (value + "").toLocaleLowerCase() == "true") {
        return "是";
    }
    return "否";
}
//设置下拉框文本信息
function setAvailable(id, value) {
    if (typeof (value) == "undefined" || typeof (id) == "") return "";
    var text = value == true ? "是" : "否";
    $("#" + id).combobox("setText", text);
}
//转换时间
function GetDateTime(value, row, index) {
    if (typeof (value) == "undefined" || value == null) { return; }
    return ConvertDateForMillisecond(value, 'yyyy-MM-dd');
}
function GetDateTimeYYYY_MM_HH_mm_ss(value, row, index) {
    if (typeof (value) == "undefined" || value == null) { return; }
    return ConvertDateForMillisecond(value, 'yyyy-MM-dd HH:mm:ss');
}
//字符串到数组
function GetStrToArry(source, sp) {
    var _sp = sp;
    if (typeof (sp) == "undefined" || sp == null || sp.length == 0) {
        _sp = '|';
    }
    var result = [];
    if (typeof (source) != "undefined" && source != null) {
        result = source.split(_sp);
    }
    return result;
}


function DeepCopy(source) {
    var result = {};
    for (var key in source) {
        if (source[key] == null) {
            result[key] = null;
        }
        else {
            result[key] = typeof source[key] === 'object' ? DeepCopy(source[key]) : source[key];
        }
    }
    return result;
}
Array.prototype.insertAt = function (index, value)
{
    var part1 = this.slice(0, index);
    var part2 = this.slice(index);
    part1.push(value);
    return (part1.concat(part2));
};
Array.prototype.removeAt = function (index)
{
    var part1 = this.slice(0, index);
    var part2 = this.slice(index);
    part1.pop();
    return (part1.concat(part2));
}
function Initeasyui_comboboxStyle() {
    $(".easyui-combobox").css({ 'padding-top': '1px', 'padding-bottom': '1px' });
}

function InitDataContentDataOption(curorientation) {
    if (curorientation == false) {
        return;
    }
    var dataC = $(".DataContent");
    //alert(dataC.length);
    if (dataC.length > 0) {
        for (var i = 0; i < dataC.length; i++) {
            SetDataContentDataOption(dataC[i], curorientation);
        }
    }
}

function SetDataContentDataOption(obj, curorientation)
{
    if (obj == null || typeof (obj) == "undefined") return;
    var dataoption = $(obj).attr("data-options");
    if (dataoption == "" || dataoption == undefined || typeof (dataoption) == "undefined") {
        return;
    }
    dataoption = dataoption.replace(/\s+/g, "");
    if (curorientation == 1) {
        //横屏
        //启用自动填充列
        if (dataoption.indexOf("fitColumns") >= 0) {
            dataoption = dataoption.replace("fitColumns:false", "fitColumns:true");
        } else {
            dataoption = dataoption + ",fitColumns:true";
        }
    } else {
        if (dataoption.indexOf("fitColumns") >= 0) {
            dataoption = dataoption.replace("fitColumns:true", "fitColumns:false");
        } else {
            dataoption = dataoption + ",fitColumns:false";
        }
    }

    $(obj).attr("data-options", dataoption);
}