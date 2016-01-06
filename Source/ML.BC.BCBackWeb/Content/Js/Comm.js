//function initpage() {
//    var bodywith = GetdocumentClientWidth();
//    var contentwidth = bodywith - $(".leftmenu").width() - 10;
//   // contentwidth = bodywith -50;
//    $("#Content,.Content").width(contentwidth);
//    var bodyheight = GetdocumentClientHeight();
//    var contentheight = bodyheight// - $("#Header").height() - $("#Headerbotton").height();
//  $("#Content,.Content").height(contentheight);
//    alert(contentheight);
//  $("#MenuContent,.MenuContent").height(contentheight - $(".btnbartitle").height());
//    $.parser.parse(); //解析元素
//    $("#dlg").dialog('close'); //关闭弹出窗口

//}
//function initpage() {
//    var bodywith = GetdocumentClientWidth();
//    var contentwidth = bodywith - $(".leftmenu").width() - 10;
//    var bodyheight = GetdocumentClientHeight();
//    var contentheight = bodyheight-150 ;
//  // $("#Content,.Content").height(300);
//   // alert(contentheight);
//    $("#MenuContent,.MenuContent").height(bodyheight - $("#btnbartitle").height()-50); //菜单
//    if ($.parser.parse == false) {
//        $.parser.parse(); //解析元素
//    }
//    $("#dlg").dialog('close'); //关闭弹出窗口
//}
function EasyuiParser() {
    if ($.parser.auto == false) {
        $.parser.parse(); //解析元素
    }
}
 
//实际宽度包括类容
function GetdocumentScrollWidth() {
    var scrollWidth = Math.max(document.documentElement.scrollWidth, document.body.scrollWidth);
    return scrollWidth;
}
//实际宽度包括类容高度
function GetdocumentScrollHeight() {
    var scrollHeight = Math.max(document.documentElement.scrollHeight, document.body.scrollHeight);
    return scrollHeight;
}
//可见宽度
function GetdocumentClientWidth() {
    var clientWidth = Math.max(document.documentElement.clientWidth, document.body.clientWidth);
     return clientWidth;
}
//可见高度
function GetdocumentClientHeight() {
    //alert(document.documentElement.clientHeight + ';' + document.body.clientHeight);

    var height1 = document.documentElement.clientHeight;
    var height2 = document.body.clientHeight;
    var _height = 0;
    if (self != top) {
        //在ifrmae中
        _height = Math.max(height1, height2);
    } else {
        if (IsWebKit()) {
        //WebKit浏览器
            _height = Math.min(height1, height2);
            if (_height == 0) {
                _height = Math.max(height1, height2);
            }
        }
        else {
      
            _height = Math.max(height1, height2);
        }
    }
    return _height;
}
function IsWebKit() {
    var flag = navigator.userAgent.toLowerCase().match(/WebKit/ig) != null;
    //alert(navigator.userAgent);
    return flag;
}
function IsChrome() {
    var isChrome = navigator.userAgent.toLowerCase().match(/chrome/ig) != null;
    //alert(navigator.userAgent);
    return isChrome;
}
