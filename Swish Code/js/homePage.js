var app = angular.module('homeApp', []).controller('homePageViewModel', function($scope, $http) {
    
	/***************************
	*** Variable Declaration ***
	***************************/
	
	$scope.realUsers = [];
	$scope.winStreakUsers = [];
	
	/****************************
	**** Function Prototypes ****
	*****************************/
	
	$scope.getWinStreaks = getWinStreaks;
	
	
	//Eventually this is how we will connect to the database when we can
	/*var url = '/JSONServices/AddToLeaderboard.aspx';
	for (var i = 0; i < $scope.winStreakUsers.length; i++)
	{
		$http({
			url: url,
			method: "POST",
			data: {"id":$scope.winStreakUsers[i].id, "wins":$scope.winStreakUsers[i].wins}
		}).then(function(response) {
			alert("It may have worked");
		}, function(response) {
			alert("Add to Leaderboard error");
		});
	}*/
	
	function getWinStreaks() {
		//Retrieve our information from the database
		var url = '/JSONServices/HomePage.aspx';
		$.ajax({
			url: url,
			type: 'GET',
			success: function(response) 
			{
				$scope.winStreakUsers = response.indivs;
				populateTable();
				console.log("Extra line for debug");
			},
			error: function(response) 
			{
				alert('Failed to retrieve users');
			}
		});


	}
	
	function populateTable(){
		for( var i = 0; i < 10; i++ ) {
			var curUser = "user".concat((i+1).toString());
			var curScore = "score".concat((i+1).toString());
			document.getElementById(curUser).innerHTML = $scope.winStreakUsers[i].username;
			document.getElementById(curScore).innerHTML = $scope.winStreakUsers[i].score;
		}
	}
	
});