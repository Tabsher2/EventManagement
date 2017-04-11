var app = angular.module('newRSOApp', []).controller('newRSOViewModel', function($scope, $http) {
    
	/***************************
	*** Variable Declaration ***
	***************************/
	
	$scope.users = [];
	$scope.taken = false;
	$scope.invalid = false;
	$scope.userID = 4;
	$scope.userEmail = "tesla@knights.ucf.edu";
	
	/****************************
	**** Function Prototypes ****
	*****************************/
	
	$scope.createEvent = createEvent;
	$scope.makeDropdown = makeDropdown;
	
	function makeDropdown() {
		flag = false;
		
		var userID = 2;
		
		if( flag )
			return;
		
		var url = '/EventJSON/getRSOForDropdown.aspx';
		
//		alert( "" + email + " -- " + username );
	
		var dt = '{\"userID\":\"' + userID + '\"}';
		console.log(dt);
		
		$.ajax({
			url: url,
			type: 'POST',
			dataType: 'json',
			data: dt,
			success: function(response) 
			{
				if(response.error == "")
				{
					var select = document.getElementById("rso");

					for (var i = 0; i < response.rsos.length; i++) {
						var opt = response.rsos[i];
						var el = document.createElement("option");
						el.textContent = opt.name;
						el.value = opt.id;
						select.appendChild(el);
					}
				}
				else
				{
					var lol = 0;
				}
			},
			error: function(arg1, arg2, arg3) 
			{
				alert("Uh-Oh. Something went wrong. ):")
			}
		});
		
	}
	function createEvent() {
		
		var rso = document.getElementById('rso').value;
		var name = document.getElementById('name').value;
		var category = document.getElementById('category').value;
		var description = document.getElementById('description').value;
		var time = document.getElementById('time').value;
		var date = document.getElementById('date').value;
		var loc = document.getElementById('location').value;
		var phone = document.getElementById('contactPhone').value;
		var email = document.getElementById('contactEmail').value;
		var privacy = document.getElementById('privacy').value;
		//var adminEmail = $scope.userEmail;

		if( rso == "" || name == "" || category == "" || description == "" || time == "" || date == "" || loc == "" || phone == "" || email == "" || privacy == "" ){
			alert("Please fill in all entry fields before submitting.");
			return;
		}
		
		
		var url = '/EventJSON/newEvent.aspx';
		
//		alert( "" + email + " -- " + username );

		var dt = '{\"name\":\"' + name + '\",\"rso\": \"' + rso + '\",\"category\": \"' + category + '\",\"description\": \"' + description + 
					'\",\"time\": \"' + time + '\",\"date\": \"' + date + 
					'\",\"loc\":\"' + loc + '\",\"phone\": \"' + phone + '\",\"email\": \"' + email + 
					'\",\"privacy\":\"' + privacy + '\"}';
		
		
		$.ajax({
			url: url,
			type: 'POST',
			dataType: 'json',
			data: dt,
			success: function(response) 
			{
				if(response.error == "1")
				{
					var lol = 101;
				}
				else
				{
					document.getElementById('send').style.display = "none";
					document.getElementById('feedback').innerHTML = "Event successfully created!";
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