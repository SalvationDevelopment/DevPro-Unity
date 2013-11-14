/* globals $,UnityObject2, jQuery,showUnsupported, alert, document,isChecked,randomString, console, clearInterval, setInterval, setTimeout */
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
function clearposition(card) {
    $('#' + card).removeClass('deck');
    $('#' + card).removeClass('hand');
    $('#' + card).removeClass('rear');
    $('#' + card).removeClass('head');
    cardmargin(deckpositionx);
}

function position(card, positionx) {
    $('#' + card).addClass('deckpositionx');
}

function cardmargin(x) {
    $('.card').each(function (i) {
        var decklocationx = (i / 2) + x;
        var decklocationy = (i / 2) + 43;
        $(this).css({
            'bottom': decklocationy + 'px',
            'left': decklocationx + 'px'
        });

    });
}

function shuffle() {
    $($(".card.deck").get().reverse()).each(function (i) {
        var cache = $(this).css('left');
        var spatical = Math.floor((Math.random() * 150) - 75);
       

        $(this).css('left', '-=' + spatical + 'px');


    });
    fix = setTimeout(function () {+
        cardmargin(deckpositionx);
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
            shuffle(x);

        }
    }, 100); // every 100 milliseconds
}


// initiation code
cardmargin(deckpositionx);


function animateDrawCard(player, amount){
    $('.'+player+' .deck').split(0,amount).removeClass('deck').addClass('hand').each(function(i){
        $(this).addClass("i"+i, 'Attack')
    };
}
function animateState(player, clocation, index, moveplayer, movelocation, movezone, moveposition){
        $('.'+player+' .'+clocation+" .i+"index+')').attr('class': "card'+" moveplayer" "+ movelocation +" z"+movezone+" " moveposition)
}

