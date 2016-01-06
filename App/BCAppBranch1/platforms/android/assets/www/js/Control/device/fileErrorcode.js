function fileErrorcode(error) {
	var codeAnalysis = "";
	switch (error.code) {
		case 1:
			codeAnalysis = "没有找到相应的文件或目录的错误";
			break;
		case 2:
			codeAnalysis = "所有没被其他错误类型所涵盖的安全错误，包括：当前文件在Web应用中被访问是不安全的；对文件资源过多的访问等。";
			break;
		case 3:
			codeAnalysis = "中止错误";
			break;
		case 4:
			codeAnalysis = "文件或目录无法读取的错误，通常是由于另外一个应用已经获取了当前文件的引用并使用了并发锁";
			break;
		case 5:
			codeAnalysis = "编码错误";
			break;
		case 6:
			codeAnalysis = "修改拒绝的错误，当试图写入一个底层文件系统状态决定其不能修改的文件或目录时";
			break;
		case 7:
			codeAnalysis = "无效状态错误";
			break;
		case 8:
			codeAnalysis = "语法错误，用于File Writer对象";
			break;
		case 9:
			codeAnalysis = "无非法的修改请求错误，例如同级移动（将一个文件或目录移动到它的父目录中）时没有提供和当前名称不同的名称时";
			break;
		case 10:
			codeAnalysis = "超过配额错误，当操作会导致应用程序超过系统所分配的存储配额时";
			break;
		case 11:
			codeAnalysis = "类型不匹配错误，当试图查找文件或目录而请求的对象类型错误时";
			break;
		case 12:
			codeAnalysis = "路径已存在错误，当试图创建路径已经存在的文件或目录时";
			break;
		default:
			break;
	}
	return codeAnalysis;
}