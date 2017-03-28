var url = '/JSONServices/Login.aspx';

$.ajax({
	url: url,
	type: 'POST',
	dataType: 'json',
	data: '{"uname":"Rick","pw":"test"}',
	success: function(dataResponse) 
	{
		var yyy;
		yyy = 0;
	},
	error: function(arg1, arg2, arg3) 
	{
		var yyy;
		yyy = 0;
	}
});
/*
var url = '/JSONServices/AddUser.aspx';

$.ajax({
	url: url,
	type: 'POST',
	dataType: 'json',
	data:'{"username":"Astr0nautical","email":"spacecombat@gmail.com","pw":"wow","age": "23"}',
	success: function(dataResponse) 
	{
		var yyy;
		yyy = 0;
	},
	error: function(arg1, arg2, arg3) 
	{
		var yyy;
		yyy = 0;
	}
});*/