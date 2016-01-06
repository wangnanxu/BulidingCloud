var xmlhttp;
var timeSpace = 2000;
var countDown;
var onmessage = function(evt) {
		countdown = setInterval(TimingRequest, timeSpace); //每隔几秒调用一次方法
	}
	//
var TimingRequest = function() {
	xmlhttp = new XMLHttpRequest();
	if (xmlhttp != null) {
		var url = "http://115.29.110.41:82/Handler/UploadFile.ashx?Method=list";
		xmlhttp.onreadystatechange = CallBack;
		xmlhttp.open("GET", url, true);
		xmlhttp.send();
	}
}
var CallBack = function() {
		if (xmlhttp.readyState == 4) {
			if (xmlhttp.status == 200) {
				var response = xmlhttp.responseText;
				if (response) {
					AnalyseDataByCallBack(response);
				}
			}
		}
	}
	//分析命令队列返回数据
var AnalyseDataByCallBack = function(data) {
	if (data) {
		//数据分析
		DataRequest(data);
	}
}
var DataRequest = function(data) {
		xmlhttp = new XMLHttpRequest();
		if (xmlhttp != null) {
			var url = "http://115.29.110.41:82/Handler/UploadFile.ashx?Method=list";
			xmlhttp.onreadystatechange = SendToMainThread;
			xmlhttp.open("GET", url, true);
			xmlhttp.send(data);
		}
	}
	//将请求数据发回主线程
var SendToMainThread = function() {
	if (xmlhttp.readyState == 4) {
		if (xmlhttp.status == 200) {
			var response = xmlhttp.responseText;
			//数据分析
			postMessage(response);
		}
	}

}