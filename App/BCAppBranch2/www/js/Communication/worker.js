var xmlhttp; // XMLHttpRequest请求对象
var timeSpace = 3000; //请求间隔时间
var countDown; //定时器
var token;
var baseurl;
//开始执行线程
onmessage = function(evt) {
	countdown = setInterval(TimingRequest, timeSpace);
	token=evt.data.Token;
	baseurl=evt.data.baseUrl;
	//TimingRequest();
};
//请求服务器函数
var TimingRequest = function() {
	xmlhttp = new XMLHttpRequest();
	var _url = baseurl+"/APIS/SyncData/GetUserSyncMessages/?Token="+token; /*服务器请求地址*/
	if (xmlhttp != null) {
		xmlhttp.onreadystatechange = CallBack;
		xmlhttp.open("POST", _url, true);
		xmlhttp.setRequestHeader("Content-type","application/x-www-form-urlencoded");
		xmlhttp.send();
	}
};
//请求回调函数
var CallBack = function() {
	if (xmlhttp.readyState == 4) {
		if (xmlhttp.status == 200 || xmlhttp.status == 500) {
			var response = xmlhttp.response;
			if (response) {
				postMessage(response)
			}
		}
	}
};