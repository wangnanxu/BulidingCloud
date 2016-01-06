//界面初始化查找所有资料
function SelectAllMaterial(){
	dbBase.OpenTransaction(function(tx){
		dbBase.SelectTable(tx,"select * from tb_Materials",[],ShowMaterialList);
	})
}
////动态加入资料
//function SelectOneMaterial(id){
//	dbBase.OpenTransaction(function(tx){
//		dbBase.SelectTable(tx,"tb_Materials","*","MatterialID=?",[id],ShowMaterialList);
//	})
//}
