var options = {loop:false};
		var instance;
		(function(window, $, PhotoSwipe){			
			$(document).ready(function(){								
				$("#loadB").bind("click",loadDatas);		
				bindPhotoSwipe();
			});
			
			/*
				初始化photoSwipe
			*/
			function bindPhotoSwipe(){
					instance = $("#Gallery a").photoSwipe(options);
					var size = $("#Gallery a").length;
					// onDisplayImage
					instance.addEventHandler(PhotoSwipe.EventTypes.onDisplayImage, function(e){
						console.log('onDisplayImage{');
						console.log('onDisplayImage - e.action = ' + e.action);
						console.log('onDisplayImage - e.index = ' + e.index);
						console.log(instance.getCurrentImage());
						console.log('onDisplayImage}');
					
						if(e.index==size-1){
							//到了最后一张图片加载更多数据					
								alert("加载更多数据中...");
								detatch();
								loadDatas();	
								instance.show(e.index);
							
						}
					});
							
			}	
			/*
				加载数据
			*/
		   function loadDatas(){
			//add datas
				for(var i=1;i<6;i++){
					$("#Gallery").append("<li><a href=\"images/full/00"+i+".jpg\"><img src=\"images/thumb/00"+i+".jpg\" alt=\"Image 00"+i+"\" /></a></li>")
				}
				bindPhotoSwipe();
			}
			/*
				解绑
			*/
			function detatch(){
				PhotoSwipe.detatch(instance);
				PhotoSwipe.activeInstances = [];	
			}
			
		}(window, window.jQuery, window.Code.PhotoSwipe));