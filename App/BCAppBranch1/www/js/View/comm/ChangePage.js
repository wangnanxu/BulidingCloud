/*
 * 换页
 * @page具体页面
 */
function ChangePage(page){
	$.mobile.changePage(page, {
		transition: 'none',
		reloadpage: true
	});
}
