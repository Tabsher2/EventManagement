var app = angular.module('newRSOApp', []).controller('newRSOViewModel', function($scope, $http) {
    
	/***************************
	*** Variable Declaration ***
	***************************/
	
	$scope.users = [];
	$scope.taken = false;
	$scope.invalid = false;
	$scope.userID = 3;
	$scope.userEmail = "tester@knights.ucf.edu";
	
	/****************************
	**** Function Prototypes ****
	*****************************/
	
	$scope.createRSO = createRSO;
	
	function createRSO() {
		var flag = false;
		var name = document.getElementById('name').value;
		var description = document.getElementById('description').value;
		var member1 = document.getElementById('member1').value;
		var member2 = document.getElementById('member2').value;
		var member3 = document.getElementById('member3').value;
		var member4 = document.getElementById('member4').value;
		var member5 = document.getElementById('member5').value;
		var rsoAdmin = $scope.userID;
		var adminEmail = $scope.userEmail;
		var atIndex = adminEmail.length - adminEmail.indexOf('@');
		if (member1.substring(member1.length - atIndex, member1.length) != adminEmail.substring(adminEmail.length - atIndex, adminEmail.length))
			flag = true;
		if (member2.substring(member2.length - atIndex, member2.length) != adminEmail.substring(adminEmail.length - atIndex, adminEmail.length))
			flag = true;
		if (member3.substring(member3.length - atIndex, member3.length) != adminEmail.substring(adminEmail.length - atIndex, adminEmail.length))
			flag = true;
		if (member4.substring(member4.length - atIndex, member4.length) != adminEmail.substring(adminEmail.length - atIndex, adminEmail.length))
			flag = true;
		if (member5.substring(member5.length - atIndex, member5.length) != adminEmail.substring(adminEmail.length - atIndex, adminEmail.length))
			flag = true;

		if (flag)
			alert("The students must attend the same university");
		//check valid
		
		var url = '/EventJSON/AddRSO.aspx';
		
//		alert( "" + email + " -- " + username );

		var dt = '{\"name\":\"' + name + '\",\"description\": \"' + description + '\",\"admin\": \"' + rsoAdmin +
					'\",\"member1\":\"' + member1 + '\",\"member2\": \"' + member2 + '\",\"member3\": \"' + member3 + 
					'\",\"member4\":\"' + member4 + '\",\"member5\": \"' + member5 + '\"}';
		
		
		$.ajax({
			url: url,
			type: 'POST',
			dataType: 'json',
			data: dt,
			success: function(response) 
			{
				if(response.error === "1")
				{
					document.getElementById('feedback').innerHTML = "The name is already in use.";
					document.getElementById('feedback').style.display = "block";
				}
				else
				{
					document.getElementById('send').style.display = "none";
					document.getElementById('feedback').innerHTML = "Organization successfully registered!";
					document.getElementById('feedback').style.display = "block";
				}
			},
			error: function(arg1, arg2, arg3) 
			{
				alert("Uh-Oh. Something went wrong.")
			}
		});
	}
});