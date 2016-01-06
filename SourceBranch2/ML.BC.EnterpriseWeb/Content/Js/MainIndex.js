$(function () {
    $(".sub-menu div").click(function () {
        $('.sub-menu div').removeClass("selected");
        $(this).addClass("selected");
    }).hover(function () {
        $(this).addClass("navHover");
    },
    function () {
        $(this).removeClass("navHover");
    });

    writeDateInfo();
    initNav();
});

//样式
function readyIndex() {
    $("#toolbar img").hover(function () {
        $(this).addClass("pageBase_Div");
    }, function () {
        $(this).removeClass("pageBase_Div");
    });
    $("#topnav li").click(function () {
        $("#topnav li").find('a').removeClass("onnav")
        $(this).find('a').addClass("onnav");
    });
    $(".sub-menu div").click(function () {
        $('.sub-menu div').removeClass("selected")
        $(this).addClass("selected");
    }).hover(function () {
        $(this).addClass("navHover");
    }, function () {
        $(this).removeClass("navHover");
    });
$(".sub-menu").hover(function () {
        $(this).css("overflow", "auto");
    }, function () {
        $(this).css("overflow", "hidden");
    });
}


/**自应高度**/
var MainContent_subtract = 122;
var Sidebar_subtract = 148;
function iframeresize() {
    resizeU();
    $(window).resize(resizeU);
    function resizeU() {
        var divkuangH = $(window).height();
        $("#MainContent").height(divkuangH - MainContent_subtract - 1);
    }
}
/**安全退出**/
function LoginOut() {
    $.messager.confirm("系统提示", "您确认退出吗?", function (a) {
        if (!a) return;
        Loading(true);
        top.window.location.href = '/Account/Account/Logout';
    });

}
//当前日期
function writeDateInfo() {
    var now = new Date();
    var year = now.getFullYear(); //getFullYear getYear
    var month = now.getMonth();
    var date = now.getDate();
    var day = now.getDay();
    var hour = now.getHours();
    var minu = now.getMinutes();
    var sec = now.getSeconds();
    var week;
    month = month + 1;
    if (month < 10) month = "0" + month;
    if (date < 10) date = "0" + date;
    if (hour < 10) hour = "0" + hour;
    if (minu < 10) minu = "0" + minu;
    if (sec < 10) sec = "0" + sec;
    var arr_week = new Array("星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六");
    week = arr_week[day];
    var time = "";
    time = year + "年" + month + "月" + date + "日" + " " + hour + ":" + minu + ":" + sec;

    $("#datetime").text(time);
    var timer = setTimeout("writeDateInfo()", 1000);
}
var Contentheight = "";
var Contentwidth = "";
var FixedTableHeight = "";
//最大化
function Maximize() {
    $("#Header").hide();
    $("#full_screen").attr('src', '/content/Images/16/arrow_inout.gif').attr('title', '还原').attr('onclick', 'Fullrestore()');
    MainContent_subtract = MainContent_subtract - 70;
    Sidebar_subtract = Sidebar_subtract - 70;
    iframeresize();
}
//还原
function Fullrestore() {
    $("#Header").show();
    $("#full_screen").attr('src', '/content/Images/16/arrow_out.gif').attr('title', '最大化').attr('onclick', 'Maximize()');
    MainContent_subtract = MainContent_subtract + 70;
    Sidebar_subtract = Sidebar_subtract + 70;
    iframeresize();
}
//=================动态菜单tab标签========================
var _ifametmp = '<iframe width="100%" height="100%" scrolling="auto" frameborder="0" src="{Url}" name="tabs_iframe" id="{TabId}"></iframe>';

function AddTabMenu(tabid, url, name, iconClass, Isclose, IsReplace) {
    if (url == "" || url == "#") {
        url = "/ErrorPage/404.aspx";
    }
    var tabid = 'tabs_iframe_' + tabid;
    var content = _ifametmp;
    content = content.replace("{Url}", url);
    content = content.replace("{TabId}", tabid);
    var ishave = $('#ContentPannel').tabs("exists", name);
    if (ishave) {
        $('#ContentPannel').tabs("select", name);
    } else { 
     $('#ContentPannel').tabs('add', {
        title: name,
        content: content,
        closable: Isclose,
       iconCls:iconClass,
    });
    }
 }

function initNav()
{
    var topnav = $("#topnav");
    if (topnav.length > 0)
    {
        var lis = $("#topnav li");
        $.each(lis, function (i,item) {
            $(item).bind("click", function () {
                $("#topnav li a").attr("class", "");
                $(item).children("a").attr("class", "onnav");
            });
        });

    }
}