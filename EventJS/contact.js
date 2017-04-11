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
	
	$scope.makeDropdown = makeDropdown;
	$scope.sendMail = sendMail;
	
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
					//var newSelect=document.createElement('select');
					//ar selectHTML="";
					//for(i=0; i<response.rsos.Count; i=i+1){
					//	selectHTML+= "<option value='"+response.rsos[i].id+"'>"+response.rsos[i].name+"</option>";
					//}
					//newSelect.innerHTML= selectHTML;
					//document.getElementById('rso').innerHTML= selectHTML;
				}
				else
				{
					var lol = 101;
				}
			},
			error: function(arg1, arg2, arg3) 
			{
				alert("Uh-Oh. Something went wrong. ):")
			}
		});
		
	}
	
	function sendMail() {
		flag = false;
		
		var rso = document.getElementById('rso').value;
		var name = document.getElementById('name').value;
		var subject = document.getElementById('subject').value;
		var body = document.getElementById('body').value;
		var email;
	
		var tmpstr = document.getElementById('email').value;
		if(/^[a-z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i.test(tmpstr)){
			email = tmpstr;
			document.getElementById('emailError').style.display = "none";
		}
		else{
			document.getElementById('emailError').style.display = "block";
			flag = true;
		}
		
		if( name === "" || tmpstr === "" || subject === "" || body === "" ){
			document.getElementById('blankError').style.display = "block";
			flag = true;
		}
		else{
			document.getElementById('blankError').style.display = "none";
			
		}
		
		
		if( flag )
			return;
		
		//check valid
		
		var url = '/EventJSON/Contact.aspx';
		
//		alert( "" + email + " -- " + username );
	
		var dt = '{\"name\":\"' + name + '\",\"rso\":\"' + rso + '\",\"email\":\"' + email + '\",\"subject\":\"' + subject + '\",\"body\": \"' + body + '\"}';
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