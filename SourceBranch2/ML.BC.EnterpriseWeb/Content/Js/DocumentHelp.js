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
    var clientHeight = Math.max(document.documentElement.clientHeight, document.body.clientHeight);
    return clientHeight;
}
