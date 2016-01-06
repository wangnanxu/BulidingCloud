var myScroll;
/*
 * 增加页面滑动模块
 * @direct滑动方向
 */
function AddPaneScroll(direct,endfun,param) {
		var el = document.getElementById("wrapper");
		if (el == null) {
			return;
		}
		if (myScroll) {
			myScroll.destroy();
			myScroll = null;
		}
		if (myScroll == null) {
			myScroll = new IScroll('#wrapper', {
				scrollbars: true,
				mouseWheel: true,
				vScrollbar: false, //隐藏竖直滚动条
				interactiveScrollbars: true,
				shrinkScrollbars: 'scale',
				fadeScrollbars: true,
				preventDefault: false,//配置点击事件
				preventDefaultException: { tagName: /^(INPUT|TEXTAREA|BUTTON|SELECT|A)$/ }//ios、android4.4+以上需配置点击事件
			}, direct,endfun,param);
		}
	}
	//绑定事件
document.addEventListener('touchmove', function(e) {
	e.preventDefault();
}, false);