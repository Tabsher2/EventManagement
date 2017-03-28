var app = angular.module('contactApp', []).controller('contactViewModel', function($scope, $http) {
    
	/***************************
	*** Variable Declaration ***
	***************************/
	
	$scope.users = [];
	$scope.taken = false;
	$scope.invalid = false;
	
	/****************************
	**** Function Prototypes ****
	*****************************/
	
	$scope.sendMail = sendMail;
	
	function sendMail() {
		flag = false;
		
		var name = document.getElementById('name').value;
		var subject = document.getElementById('subject').value;
		var body = document.getElementById('body').value;
		
		if( name === "" || email === "" || subject === "" || body === "" ){
			document.getElementById('blankError').style.display = "block";
			flag = true;
		}
		else{
			document.getElementById('blankError').style.display = "none";
			
			var tmpstr = document.getElementById('email').value;
			if(/^[a-z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i.test(tmpstr)){
				var email = tmpstr;
				document.getElementById('emailError').style.display = "none";
			}
			else{
				document.getElementById('emailError').style.display = "block";
				flag = true;
			}
		}
		
		
		if( flag )
			return;
		
		//check valid
		
		var url = '/JSONServices/Contact.aspx';
		
//		alert( "" + email + " -- " + username );
	
		var dt = '{\"name\":\"' + name + '\",\"email\":\"' + email + '\",\"subject\":\"' + subject + '\",\"body\": \"' + body + '\"}';
		console.log(dt);
		
		$.ajax({
			url: url,
			type: 'POST',
			dataType: 'json',
			data: dt,
			success: function(response) 
			{
				if(response.error != "")
				{
					document.getElementById('feedback').innerHTML = "<br>Message was unable to be sent.";
					document.getElementById('feedback').style.display = "block";
				}
				else
				{
					document.getElementById('send').style.display = "none";
					document.getElementById('feedback').innerHTML = "Message sent.";
					document.getElementById('feedback').style.display = "block";
				}
			},
			error: function(arg1, arg2, arg3) 
			{
				alert("Uh-Oh. Something went wrong. ):")
			}
		});
	}
	
});