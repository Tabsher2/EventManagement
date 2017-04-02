var app = angular.module('joinRSOApp', []).controller('joinRSOViewModel', function($scope, $http) {
    
	/***************************
	*** Variable Declaration ***
	***************************/
	
	$scope.rsos = [];
	$scope.taken = false;
	$scope.invalid = false;
	$scope.userID = 2;
	$scope.userUni = 1;
	$scope.userEmail = "tester@knights.ucf.edu";
	
	/****************************
	**** Function Prototypes ****
	*****************************/
	
	$scope.requestJoinRSO = requestJoinRSO;
	$scope.loadRSOs = loadRSOs;

	function loadRSOs() {
		var userUni = $scope.userEmail;
		//check valid
		
		var url = '/EventJSON/LoadRSO.aspx';
		
//		alert( "" + email + " -- " + username );

		var dt = '{\"uniID\":\"' + userUni + '\"}';
		
		$.ajax({
			url: url,
			type: 'POST',
			dataType: 'json',
			data: dt,
			success: function(response) 
			{
				$scope.rsos = response.rsos;
				var select = document.getElementById("rsoSelect");

				for(var i = 0; i < $scope.rsos.length; i++) {
					var opt = $scope.rsos[i];
					var el = document.createElement("option");
					el.textContent = opt;
					el.value = opt;
					select.appendChild(el);
				}
			},
			error: function(arg1, arg2, arg3) 
			{
				alert("Uh-Oh. Something went wrong.")
			}
		});
	}
	
	function requestJoinRSO() {
		var requestUser = $scope.userID;
		var e = document.getElementById("rsoSelect");
		var rso = e.options[e.selectedIndex].value;
		//check valid
		
		var url = '/EventJSON/JoinRSO.aspx';
		
//		alert( "" + email + " -- " + username );

		var dt = '{\"userID\":\"' + requestUser + '\",\"rsoName\":\"' + rso + '\"}';
		
		
		$.ajax({
			url: url,
			type: 'POST',
			dataType: 'json',
			data: dt,
			success: function(response) 
			{
					document.getElementById('send').style.display = "none";
					document.getElementById("rsoSelect").disabled = true;
					document.getElementById('feedback').innerHTML = "Request sent!";
					document.getElementById('feedback').style.display = "block";
			},
			error: function(arg1, arg2, arg3) 
			{
				alert("Uh-Oh. Something went wrong.")
			}
		});
	}
});