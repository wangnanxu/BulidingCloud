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
    Loading(false);
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
    window.location.reload();
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
function Loading(bool) {
    if (bool) {
        top.$("#Loading").show();
    } else {
        setInterval(loadinghide, 900);
    }
}
function loadinghide() {
    if (top.document.getElementById("Loading") != null) {
        top.$("#Loading").hide();
    }
}
var showtime = 1000;var hidetime = 1000;
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
        showTipsMsg("很抱歉，您当前未选中任何一行！",2000, 2);
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
        stime = speed;
        htime = speed;
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
    top.$("#" + msgid).stop(); 
  
   top.$("#" + msgid).html(msg);
   top.$("#" + msgid).show(stime, function () { 
    //隐藏消息
        setTimeout(function () {
            top.$("#" + msgid).hide(htime); //alert('隐藏:' + msgid);
        }, time);
   });
 
       
     
}
 