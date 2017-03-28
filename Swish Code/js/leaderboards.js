var app = angular.module('leaderboardApp', []).controller('leaderboardViewModel', function($scope, $http) {

	/***************************
	*** Variable Declaration ***
	***************************/

	$scope.selectedLeaderboard;
	$scope.selectedTime;
	$scope.displayedUser = [];
	
	/****************************
	**** Function Prototypes ****
	*****************************/
	
	$scope.selectTable = selectTable;

	//Clears all tables except one on load of the page
	function clear() {
		var i, tableTab;
		tableTab = document.getElementsByClassName("tableTabs");
		for (i = 0; i < tableTab.length; i++) {
			tableTab[i].style.display = "none";
		}
		document.getElementById('allTimeWins').style.display = "block";
	}
	
	function selectTable() {
		$scope.selectedLeaderboard = document.getElementById("leaderboards").options[document.getElementById("leaderboards").selectedIndex].value;
		$scope.selectedTime = document.getElementById("times").options[document.getElementById("times").selectedIndex].value;
		
		/*var url = '/JSONServices/AddToLeaderboard.aspx';
			$http({
				url: url,
				method: "POST",
				data: {"id":$scope.winStreakUsers[i].id, "wins":$scope.winStreakUsers[i].wins}
			}).then(function(response) {
				alert("It may have worked");
			}, function(response) {
				alert("Add to Leaderboard error");
			});
		*/

	}

});