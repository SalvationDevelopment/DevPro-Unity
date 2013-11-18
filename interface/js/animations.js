/* globals $,UnityObject2, jQuery,showUnsupported, alert, document,isChecked,randomString, console, clearInterval, setInterval, setTimeout, duel, cardPositions*/
var deckpositionx = 735;
var currenterror;
var positions = {
    extra: {
        x: 25
    }
};
var shuffler, fix; 

$(document).ready(function(){
       $('.card').on('click', function () {
        complete(deckpositionx);
    });
                  });


// Animation functions

function shuffle(player) {
    function cardmargin(player) {
        $('.'+player+'.deck').each(function (i) {
            var decklocationx = (i / 2) + cardlocations[player].Deck.x_origin;
            var decklocationy = (i / 2) + cardlocations[player].Deck.y_origin;
            $(this).css({
                'bottom': decklocationy + 'px',
                'left': decklocationx + 'px'
            });
    
        });
    }
    cardmargin(player);
    $($(player+".card.Deck").get().reverse()).each(function (i) {
        var cache = $(this).css('left');
        var spatical = Math.floor((Math.random() * 150) - 75);
        $(this).css('left', '-=' + spatical + 'px');
    });
    fix = setTimeout(function () {
        cardmargin(player);
    }, 50);
}

function complete(x) {
    var started = Date.now();

    // make it loop every 100 milliseconds

    var interval = setInterval(function () {

        // for 1.5 seconds
        if (Date.now() - started > 500) {

            // and then pause it
            clearInterval(interval);

        } else {

            // the thing to do every 100ms
            shuffle('p0');

        }
    }, 100); // every 100 milliseconds
}


function animateDrawCard(player, amount){
    var c = $('.'+player+'.Deck').splice(0,amount);
    console.log('.'+player+'.Deck');
    console.log(c.length);
    $(c).each(function(i){
        $(this).attr('class', "card "+player+' '+'Hand i'+(i+duel[player].Hand.length)+' AttackFaceUp')
        .attr('style','');
    }
);}

function animateState(player, clocation, index, moveplayer, movelocation, movezone, moveposition, count){
    if (count === undefined) {count = 1;}
    var query = "."+player+"."+clocation+".i"+index;
    console.log(moveposition, cardPositions[moveposition]);
    $(query).slice(0,count).attr('class', "card "+ moveplayer+" "+ movelocation +" i"+movezone+" "+cardPositions[moveposition])
    .attr('style','');
}
function animateChaining(player,clocation,index){
    $(player+'.'+clocation+'.i'+index).addClass('chainable');
}
function animateRemoveChaining(){
    $('.chainable').removeClass('chainable');
}
