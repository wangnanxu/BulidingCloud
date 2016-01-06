  1 /**
  2  * @fileoverview 百度地图浏览区域限制类，对外开放。
  3  * 允许开发者输入限定浏览的地图区域的Bounds值，
  4  * 则地图浏览者只能在限定区域内浏览地图。
  5  * 基于Baidu Map API 1.2。
  6  *
  7  * @author Baidu Map Api Group 
  8  * @version 1.2
  9  */
 10 
 11 /** 
 12  * @namespace BMap的所有library类均放在BMapLib命名空间下
 13  */
 14 var BMapLib = window.BMapLib = BMapLib || {};
 15 
 16 (function() {
 17 
 18     /** 
 19      * @exports AreaRestriction as BMapLib.AreaRestriction 
 20      */
 21     var AreaRestriction =
 22         /**
 23          * AreaRestriction类，静态类，不用实例化
 24          * @class AreaRestriction类提供的都是静态方法，勿需实例化即可使用。     
 25          */
 26         BMapLib.AreaRestriction = function(){
 27         }
 28     
 29     /**
 30      * 是否已经对区域进行过限定的标识
 31      * @private
 32      * @type {Boolean}
 33      */
 34     var _isRestricted = false;
 35 
 36     /**
 37      * map对象
 38      * @private
 39      * @type {BMap}
 40      */
 41     var _map = null;
 42 
 43     /**
 44      * 开发者需要限定的区域
 45      * @private
 46      * @type {BMap.Bounds}
 47      */
 48     var _bounds = null;
 49 
 50     /**
 51      * 对可浏览地图区域的限定方法
 52      * @param {BMap} map map对象
 53      * @param {BMap.Bounds} bounds 开发者需要限定的区域
 54      *
 55      * @return {Boolean} 完成了对区域的限制即返回true，否则为false
 56      */
 57     AreaRestriction.setBounds = function(map, bounds){
 58         // 验证输入值的合法性
 59         if (!map || 
 60             !bounds || 
 61             !(bounds instanceof BMap.Bounds)) {
 62                 throw "请检查传入参数值的合法性";
 63                 return false;
 64         }
 65         
 66         if (_isRestricted) {
 67             this.clearBounds();
 68         }
 69         _map = map;
 70         _bounds = bounds;
 71 
 72         // 添加地图的moving事件，用以对浏览区域的限制
 73         _map.addEventListener("moveend", this._mapMoveendEvent);
 74         _isRestricted = true;
 75         return true;
 76     };
 77 
 78     /**
 79      * 需要绑定在地图移动事件中的操作，主要控制出界时的地图重新定位
 80      * @param {Event} e e对象
 81      *
 82      * @return 无返回值
 83      */
 84     AreaRestriction._mapMoveendEvent = function(e) {
 85         // 如果当前完全没有出界，则无操作
 86         if (_bounds.containsBounds(_map.getBounds())) {
 87             return;
 88         }
 89 
 90         // 两个需要对比的bound区域的边界值
 91         var curBounds = _map.getBounds(),
 92               curBoundsSW = curBounds.getSouthWest(),
 93               curBoundsNE = curBounds.getNorthEast(),
 94               _boundsSW = _bounds.getSouthWest(),
 95               _boundsNE = _bounds.getNorthEast();
 96 
 97         // 需要计算定位中心点的四个边界
 98         var boundary = {n : 0, e : 0, s : 0, w : 0};
 99         
100         // 计算需要定位的中心点的上方边界
101         boundary.n = (curBoundsNE.lat < _boundsNE.lat) ? 
102                                     curBoundsNE.lat :
103                                     _boundsNE.lat;
104 
105         // 计算需要定位的中心点的右边边界
106         boundary.e = (curBoundsNE.lng < _boundsNE.lng) ? 
107                                     curBoundsNE.lng :
108                                     _boundsNE.lng;
109 
110         // 计算需要定位的中心点的下方边界
111         boundary.s = (curBoundsSW.lat < _boundsSW.lat) ? 
112                                     _boundsSW.lat :
113                                     curBoundsSW.lat;
114 
115         // 计算需要定位的中心点的左边边界
116         boundary.w = (curBoundsSW.lng < _boundsSW.lng) ? 
117                                     _boundsSW.lng :
118                                     curBoundsSW.lng;
119         
120         // 设置新的中心点
121        var center = new BMap.Point(boundary.w + (boundary.e - boundary.w) / 2,
122                                                          boundary.s + (boundary.n - boundary.s) / 2);
123        setTimeout(function() {
124             _map.panTo(center, {noAnimation : "no"});
125         }, 1);
126     };
127 
128     /**
129      * 清除对地图浏览区域限定的状态
130      * @return 无返回值
131      */
132     AreaRestriction.clearBounds = function(){
133         if (!_isRestricted) {
134             return;
135         }
136         _map.removeEventListener("moveend", this._mapMoveendEvent);
137         _isRestricted = false;
138     };
139 
140 })();