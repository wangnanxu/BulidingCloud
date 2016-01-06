//服务器数据通讯
function PostCommand(postData, callback) {
	$.post('http://115.29.110.41:82/handler/StudentHandler.ashx', postData, function(data) {
		if(typeof(callback)=="function"){
			callback(data);
		}
	})
}
var PostData = function(callback) {
	$.post('http://115.29.110.41:82/handler/StudentHandler.ashx', {
		key: oCallback.item(0).key
	}, function(data) {
		callback();
	})
}