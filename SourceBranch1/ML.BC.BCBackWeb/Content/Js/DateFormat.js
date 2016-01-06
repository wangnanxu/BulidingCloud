Date.prototype.Format = function (fmt) {
    if (!fmt) fmt = "yyyy-MM-DD HH:mm:SS";
    var dt = this;
    if (dt != null && dt != undefined) {
        var YY = dt.getFullYear(); var MM = dt.getMonth()+1; var DD = dt.getDate();
        var HH = dt.getHours(); var _MM = dt.getMinutes(); var SS = dt.getSeconds();
        if (MM < 10) MM = "0" + MM; if (DD < 10) DD = "0" + DD; if (HH < 10) HH = "0" + HH;
        if (_MM < 10) _MM = "0" + _MM; if (SS < 10) SS = "0" + SS;
        var newDTStr = fmt;
        newDTStr = newDTStr.replace(/Y+/ig, YY); newDTStr = newDTStr.replace(/M+/, MM); newDTStr = newDTStr.replace(/D+/ig, DD);
        newDTStr = newDTStr.replace(/H+/ig, HH); newDTStr = newDTStr.replace(/m+/, _MM); newDTStr = newDTStr.replace(/S+/ig, SS);
//alert(dt.toLocaleString()+';newDTStr'+newDTStr);
        return newDTStr;
    }
}

function ConvertDateForString(str, fmt) {
    if (!str) return "";
    var _str = str.replace(/\./ig, "-");
     try {
        var dt = new Date(_str);
        return dt.Format("yyyy-MM-DD");
} catch (e) {
return "";
    }
  
        
}

//// fmt:YYYY-MM-DD HH:mm:SS
function ConvertDateForMillisecond(str, fmt) {
    if (!str) return "";
    if (!fmt) fmt = "yyyy-MM-DD HH:mm:SS";
    var _str = str;
    var _str = _str.replace(/\./ig, "-");
    _str = _str.replace(/\//ig, "-");
    var numreg = /\d+/;
    var dt = new Date();
     if (_str.indexOf(".") > 0 || _str.indexOf("-") > 0 || _str.indexOf("/") > 0) {
        //2015-05-03 12:03:11
        // dt = new Date(2015,6,8,12,3,6);
        var YY = ""; var MM = ""; var DD = ""; var HH = ""; var _MM = ""; var SS = "";
        var arry0=_str.split(" ");
        var arry1 = arry0[0].split("-");
        YY = arry1[0]; MM = arry1[1]-1; DD = arry1[2];
        var arry2 = arry0[1].split(":");
        HH = arry2[0]; _MM = arry2[1]; SS = arry2[2];
         dt = new Date(YY, MM, DD, HH, _MM, SS);
    } else {
        //  /Date(1406908800000)/
        var value = _str.match(numreg);
        if (value != null && value != undefined) {
            dt.setTime(value);
        }
    }
    return dt.Format(fmt);
}