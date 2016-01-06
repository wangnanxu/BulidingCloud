var SendUserMessage=function(message){
	var dDateTime = new Date();
	var sTime = dDateTime.getFullYear() + "-" + (dDateTime.getMonth() + 1) + "-" + dDateTime.getDay() + " " + dDateTime.getHours() + ":" + dDateTime.getMinutes() + ":" + dDateTime.getSeconds();
	dbbase.InsertTable("tb_UserMessage", [,"UserID", "Message", "SendTime"], ["1", message,sTime]);
}