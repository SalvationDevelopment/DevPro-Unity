/* globals */
var saftey = false;
var Unityconsole = false
var u = new UnityObject2();

var cardIndex = {};
var cardData; 

var deckpositionx = 735
var positions = {extra : {x : 25}}
var shuffler, fix;

/* create Unity object */
u.observeProgress(function (progress) {
	var $missingScreen = jQuery(progress.targetEl).find(".missing");
	switch(progress.pluginStatus) {
		case "unsupported":
			showUnsupported();
		break;
		case "broken":
			alert("You will need to restart your browser after installation.");
		break;
		case "missing":
			$missingScreen.find("a").click(function (e) {
				e.stopPropagation();
				e.preventDefault();
				u.installPlugin();
				return false;
			});
			$missingScreen.show();
		break;
		case "installed":
			$missingScreen.remove();
		break;
		case "first":
		break;
	}
});
jQuery(function(){
	u.initPlugin(jQuery("#unityPlayer")[0], "http://unity.devpro.org/DevProWeb.unity3d");

});
$(document).ready(function() {
	$.getJSON("http://ygopro.de/cardreader/index.php?folder=English&callback=?", function(data){
		cardData = data;
		for (var i = data.length - 1; i >= 0; i--) { /* this might be backwards? */
			l = "c"+data[i][0];
			cardIndex[l] = i;
			/* c80009998 would be cardData[cardIndex.c80009998] */
		};
	});
	$('.downloadbutton, #lobbycancel').on('click',function(){
		if (saftey){
			$('#intro').toggle();
			$('.login').toggle();
			$('header').toggle();
		}else{
			alert('just one moment, server connection system is loading.');
		}
	});
	$('#loginbutton').on('click',function(){
			u.getUnity().SendMessage("HubClient", "Login",  "{'Username' : '"+($('#username').val())+"', 'Password' : '"+($('#password').val())+"', 'UID' : 'Unity'}");
		});
	$('#lobbylock, #majorpopup').on('click',function(){
		$('#majorpopup').toggle();
		$
	});
	$('#lobbystart').on('click',function(){
		$('.game').toggle();
		$('.field').toggle();
	});
	$('.card').on('click',function(){
		complete(deckpositionx);
	});
	$('body').keypress(function( event ) {
	if ( event.which == 96 ) {
		if (Unityconsole){
			$('#unityPlayer').css('height', 'auto');
			Unityconsole = false;
		}else{
			$('#unityPlayer').css('height', '1px');
			Unityconsole = true;
		}
	}
	});
	$('#creategamebutton').on('click',function(){
		$('#launcher').toggle();
		$('#creategame').toggle();
	});
	$('#creategameok').on('click',function(){
		string 	= ""+$('#creategamecardpool').val()+$('#creategameduelmode').val()+$('#creategametimelimit').val();
		prio 	= isChecked('#enableprio')		? ("F") : ("O");
		checkd 	= isChecked('#discheckdeck')	? ("F") : ("O");
		shuf 	= isChecked('#disshuffledeck')	? ("F") : ("O");
		rp 		= ($('#creategamepassword').val().length > 0 ) ? ("L") : ("");
		stnds	= ","+$('#creategamebanlist').val()+',5,1,'+$('input:radio[name=ranked]:checked').val()+rp+',';
		pass 	= $('#creategamepassword').val() || randomString(5);
		string	= string+prio+checkd+shuf+$('#creategamelp').val()+stnds+pass;
		console.log(string);
		u.getUnity().SendMessage("GameClient", 'CreateGame',string);
		$('#creategame').toggle();
		$('.game').toggle();
		$('#lobbyforbidden').html($('#creategamebanlist option:selected').text());
		$('#lobbycardpool').html($('#creategamecardpool option:selected').text());
		$('#lobbymode').html($('#creategameduelmode option:selected').text());
		$('#lobbytime').html($('#creategametimelimit option:selected').text());
		$('#lobbystartlp').html($('#creategamelp').val()+"/Player");
		
	});
});




// Animation functions
function clearposition(card){
	$('#'+card).removeClass('deck');
	$('#'+card).removeClass('hand');
	$('#'+card).removeClass('rear');
	$('#'+card).removeClass('head');
	cardmargin(deck);
}
function position(card, position){
	$('#'+card).addClass('deckpositionx');
}
function cardmargin(x){
	$('.card').each(function(i){
		decklocationx = (i/2) + x;
		decklocationy = (i/2) + 43;
		$(this).css({'bottom' : decklocationy+'px', 'left' : decklocationx+'px'});
		
	});
}
function shuffle(){
	$($(".card.deck").get().reverse()).each(function(i){
		cache =  $(this).css('left');
		spatical = Math.floor((Math.random()*150)-75);
		console.log(spatical);

		$(this).css( 'left' , '-='+spatical+'px');
		
		
	});
	fix = setTimeout(function(){cardmargin(deckpositionx);},50);
}
function complete(x){
var started = Date.now();

  // make it loop every 100 milliseconds

  var interval = setInterval(function(){

    // for 1.5 seconds
    if (Date.now() - started > 500) {

      // and then pause it
      clearInterval(interval);

    } else {

      // the thing to do every 100ms
      shuffle(x);

    }
  }, 100); // every 100 milliseconds
}


// initiation code
cardmargin(deckpositionx);



function MessageBrowser(message){
	console.log(message);
}
function MessagePopUp(message){
    }
function LoginAccept(username){
	if ($('#username').val() == username){
		$('.login').toggle();
		$('#launcher').toggle();
	}
}
function SetRoomInfo (info){
	eval("info = "+info);
}
function PosUpdate(pos){
	console.log('PosUpdate: '+pos);
}
function PlayerEnter(username, pos){
	console.log('PlayerEnter: '+username+", "+pos)
	$('#lobbyplayer'+pos).html(username);
}
function PlayerLeave(pos){
	console.log('PlayerLeave: '+pos);
	$('#lobbyplayer'+pos).html("");
}
function UpdatePlayer(pos, newpos){
	console.log('UpdatePlayer: '+pos+' to :');
	$('#lobbyplayer'+pos).html("");
	$('#lobbyplayer'+newpos).html(username);
}
function PlayerReady(pos, ready){
	console.log('PlayerReady: '+pos+' is :'+ready);	
	$('#lobbyplayer'+pos).toggleClass('ready');
}
function IsLoaded(){
	saftey = true;

	$('.downloadbutton').toggle();
}
function messageUnity(functionName, message ){
	u.getUnity().SendMessage("HubClient", functionName, message);
}