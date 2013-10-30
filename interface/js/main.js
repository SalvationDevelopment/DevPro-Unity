var u = new UnityObject2();
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
	u.initPlugin(jQuery("#unityPlayer")[0], "DevProWeb.unity3d");

});

function messageUnity(functionName, message ){
	u.getUnity().SendMessage("HubClient", functionName, message);
}
$('.downloadbutton, #lobbycancel').on('click',function(){
	$('#intro').toggle();
	$('.game').toggle();
	$('header').toggle();
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
deckpositionx = 735


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
cardmargin(deckpositionx);
var shuffler, fix;
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

positions = {extra : {x : 25}}