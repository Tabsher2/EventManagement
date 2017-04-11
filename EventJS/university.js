var app = angular.module('universityApp', []).controller('universityViewModel', function($scope, $http) {
    
	/***************************
	*** Variable Declaration ***
	***************************/
	
	$scope.users = [];
	$scope.taken = false;
	$scope.invalid = false;
	$scope.userID = 3;
	
	/****************************
	**** Function Prototypes ****
	*****************************/
	
	$scope.createUniversity = createUniversity;
	
	function createUniversity() {
		var flag = false;
		var name = document.getElementById('name').value;
		var address = document.getElementById('location').value;
		var description = document.getElementById('description').value;
		var numStudents = document.getElementById('numStudents').value;
		var universityAdmin = $scope.userID;
		//check valid
		
		var url = '/EventJSON/AddUniversity.aspx';
		
//		alert( "" + email + " -- " + username );

		var dt = '{\"name\":\"' + name + '\",\"location\": \"' + address + '\",\"description\": \"' + 
					description + '\",\"numStudents\": \"' + numStudents + '\",\"admin\": \"' + universityAdmin + '\"}';
		
		
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
					sendImages();
					document.getElementById('feedback').innerHTML = "University successfully made!";
					document.getElementById('feedback').style.display = "block";
				}
			},
			error: function(arg1, arg2, arg3) 
			{
				alert("Uh-Oh. Something went wrong.")
			}
		});
	}

	function sendImages()
	{
		var url = '/EventJSON/UploadImages.aspx';
		
		
		var foo = document.getElementById('name').value;
		var bar = document.getElementById('uniPic').value;
		bar = escape(bar);
		var dt = '{\"name\":\"' + foo + '\",\"picture\": \"' + bar + '\"}';
		
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
					document.getElementById('feedback').innerHTML = "University successfully made!";
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