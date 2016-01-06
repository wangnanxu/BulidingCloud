//添加线程用于定时请求
var AddWorker = function() {
	if (typeof("Worker") != null) {
		var _worker = new Worker("../www/js/comm/worker.js");
		_worker.postMessage("bbbb");
		_worker.onmessage = function(evt) {
			//alert(evt.data);
			//worker返回数据处理
		}
	}
}