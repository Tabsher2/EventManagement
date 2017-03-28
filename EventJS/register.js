var app = angular.module('newUserApp', []).controller('newUserViewModel', function($scope, $http) {
    
	/***************************
	*** Variable Declaration ***
	***************************/
	
	$scope.users = [];
	$scope.taken = false;
	$scope.invalid = false;
	
	/****************************
	**** Function Prototypes ****
	*****************************/
	
	$scope.createAccount = createAccount;
	
	function createAccount() {
		var flag = false;
		
		var tmpstr1 = document.getElementById('email').value;
		if(/^[a-z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i.test(tmpstr1)){
			var email = tmpstr1;
			document.getElementById('emailError').style.display = "none";
		}
		else{
			document.getElementById('emailError').style.display = "block";
			flag = true;
		}
		
		var tmpstr3 = document.getElementById('pw').value;
		if(/^(?=.*[0-9])(?=.*[a-z])[a-z0-9!@#$%^&*?]{6,18}$/i.test(tmpstr3)){
			var pw = tmpstr3;
			document.getElementById('passwordError').style.display = "none";
		}
		else{
			document.getElementById('passwordError').style.display = "block";
			flag = true;
		}
		
		if( flag )
			return;
		
		//check valid
		
		var url = '/JSONServices/Register.aspx';
		
//		alert( "" + email + " -- " + username );

		var dt = '{\"email\":\"' + email + '\",\"pw\": \"' + pw + '\"}';
		
		
		$.ajax({
			url: url,
			type: 'POST',
			dataType: 'json',
			data: dt,
			success: function(response) 
			{
				if(response.error === "1")
				{
					document.getElementById('feedback').innerHTML = "The email is already in use.";
					document.getElementById('feedback').style.display = "block";
				}
				else
				{
					document.getElementById('send').style.display = "none";
					document.getElementById('feedback').innerHTML = "Account successfully made!";
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