/* globals $,UnityObject2, jQuery,showUnsupported, alert, document,isChecked,randomString, console, clearInterval, setInterval, setTimeout,shuffle, cardplace, cardlocations, enumPhase, animateState, animateChaining, animateRemoveChaining */

/* DEAR GOD ACCESSDENIED LEARN TO USE A FOR LOOP!
for (i = 0; i < size; i++) {
}
*/

//Define all the globals you are going to use. Avoid using to many globals. All Globals should be databases of sorts.
var saftey = false;
var Unityconsole = false;
var u = new UnityObject2();
var playerStart = [0, 0];
var cardIndex = {};
var cardData;
var deckData;
var decklistData;
var decklist = [];
var currenterror;
var player1StartLP;
var player2StartLP;
var i = 0; // counter for forLoops.
var duelData;
var activeroom ='DevPro-English';
var servermessagecount = 0;
var serverlocations = [];

//This Global defines the duel state at all times via update functions. It has no impact on the DOM but may be referenced to provide information to the user or draw images.
var duel = {
    'p0': {
        'Deck': [],
        'Hand': [],
        'MonsterZone': [],
        'SpellZone': [],
        'Grave': [],
        'Removed': [],
        'Extra': [],
        'Overlay': [],
        'Onfield': []
    },
    'p1': {
        'Deck': [],
        'Hand': [],
        'MonsterZone': [],
        'SpellZone': [],
        'Grave': [],
        'Removed': [],
        'Extra': [],
        'Overlay': [],
        'Onfield': []
    }
};

var autoscroll = 0;
setInterval(function() {
        
  if( autoscroll >= 0  ){
          try{autoscroll1();
          }catch(e){}
  }
         autoscroll--;
}, 500);
autoscroll1 = function(){var elem = document.getElementById(activeroom);elem.scrollTop = elem.scrollHeight;};


/* create Unity object */
u.observeProgress(function (progress) {

    var $missingScreen = jQuery(progress.targetEl).find(".missing");
    switch (progress.pluginStatus) {
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

//The following jquery events define user interactions; When they click, answer questions, the creation and movement of screens.
$(document).ready(function () {
    u.initPlugin(jQuery("#unityPlayer")[0], "http://unity.devpro.org/DevProWeb.unity3d");
    $.getJSON("http://unity.devpro.org/cardreader/?folder=English", function (data) {
        cardData = data;
        for (var i = data.length - 1; i >= 0; i--) { /* this might be backwards? */
            var l = "c" + data[i][0];
            cardIndex[l] = i;
            /* c80009998 would be cardData[cardIndex.c80009998] */
        }

    });

    $('.downloadbutton, #lobbycancel').on('click', function () {
        $("#jquery_jplayer_1").jPlayer("stop", 0);
        if (saftey) {
            $('#intro').toggle();
            $('.login').toggle();
            $('header').toggle();
//            $('body').css({
//                'background': 'url(http://ygopro.de/img/bg_black.png)'
//            });
        } else {
            alert('just one moment, server connection system is loading.');

        }
    });
    $('#loginbutton').on('click', function () {
        u.getUnity().SendMessage("HubClient", "Login", "{'Username' : '" + ($('#username').val()) + "', 'Password' : '" + ($('#password').val()) + "', 'UID' : 'Unity', 'Version' : 198000 }");
    });

    $('#lobbylock, #majorpopup').on('click', function () {
        $('#majorpopup').toggle();

    });

    $("#lobbylock").on("click", function () {
        var selecteddeck = $("#selectdeck").val();
        var tovaliditycheck = (decklist[selecteddeck].data);
        u.getUnity().SendMessage("GameClient", 'UpdateDeck', tovaliditycheck);
        u.getUnity().SendMessage("GameClient", 'SetReady', 1);

    });

    $('body').keypress(function (event) {
        if (event.which == 96) {
            toggleConsole();
        }
    });
    $('#creategamebutton').on('click', function () {
        $('#launcher').toggle();
        $('#creategame').toggle();
    });
    $('#creategameok').on('click', function () {
        var string, prio, checkd, shuf, rp, stnds, pass, compl;
        string = "" + $('#creategamecardpool').val() + $('#creategameduelmode').val() + $('#creategametimelimit').val();
        prio = isChecked('#enableprio') ? ("F") : ("O");
        checkd = isChecked('#discheckdeck') ? ("F") : ("O");
        shuf = isChecked('#disshuffledeck') ? ("F") : ("O");
        rp = ($('#creategamepassword').val().length > 0) ? ("L") : ("");
        stnds = "," + $('#creategamebanlist').val() + ',5,1,' + $('input:radio[name=ranked]:checked').val() + rp + ',';
        pass = $('#creategamepassword').val() || randomString(5);
        compl = string + prio + checkd + shuf + $('#creategamelp').val() + stnds + pass;
        // console.log(compl);


        if ($('#creategamecardpool').val() == 2 && $('input:radio[name=ranked]:checked').val() == 'R') {
            MessagePopUp('OCG/TCG is not a valid mode for ranked, please select a different mode for ranked play');
            return false;
        }
        if (prio + checkd + shuf !== "OOO" && $('input:radio[name=ranked]:checked').val() == 'R') {
            MessagePopUp('You may not cheet on DevPro');
            return false;
        }


        u.getUnity().SendMessage("GameClient", 'CreateGame', compl);
        $('#creategame').toggle();
        $('.game').toggle();
        $('#lobbyforbidden').html($('#creategamebanlist option:selected').text());
        $('#lobbycardpool').html($('#creategamecardpool option:selected').text());
        $('#lobbymode').html($('#creategameduelmode option:selected').text());
        $('#lobbytime').html($('#creategametimelimit option:selected').text());
        $('#lobbystartlp').html($('#creategamelp').val() + "/Player");


    });
    $('.rps').on("click", function () {
        $('#rps').toggle();
        u.getUnity().SendMessage("GameClient", 'SelectRPS', $(this).data('value'));

    });
    $('#igofirst').on("click", function () {
        $('#selectduelist').toggle();
        u.getUnity().SendMessage("GameClient", 'SelectFirstPlayer', 1);

    });
    $('#opponentfirst').on("click", function () {
        $('#selectduelist').toggle();
        u.getUnity().SendMessage("GameClient", 'SelectFirstPlayer', 0);

    });
    $('#messagerbox .close').on('click', function () {
        $('#messagerbox').css('height', '0px');
    });
    $("#jquery_jplayer_1").jPlayer("play", 0);
    $('#chatform').on('submit',function(event) {
         messageon();
         autoscroll1();
        console.log('message sent'); 
        
         
         
    });
});


function joinroom(roomtojoin){
        activeroom = roomtojoin;
        
         u.getUnity().SendMessage('HubClient','JoinChannel', roomtojoin);
        $('#chatbox').append('<ul class="room active" id=room-'+roomtojoin+'></ul>');
        $('#chatrooms').append('<li class="active" id=control-'+roomtojoin+'>'+roomtojoin+'</li>');
        $('#chatbox ul, #chatrooms li').not('#'+roomtojoin).removeClass('active');
        var currentroom = 'room-'+roomtojoin;
}


//Functions used by the Unity object

function MessageBrowser(message,int) {
    console.log(int,message);
}

function MessagePopUp(message) {
    alert(message);
}

function OnHubMessage(type, data) {
    
    var update = JSON.parse(data);
    console.log(type, update);
    //console.log(update);
    var rank = "";
    var json = {
        id: type,
        content: update
    };
    //console.log(json);
    switch (json.id) {
    case 0:
        {
            //gamelist
        }
        break;
        //--------------
    case 1:
        {
            //remove room
            $('#' + json.content).remove();

        }
        break;
        //--------------
    case 2:
        {
            //update rooms players
            //console.log(json);
            $('#' + json.content.Command).html(json.content.Data);
        }
        break;
        //--------------
    case 3:
        {   
            //console.log(json);
            var username = json.content.Username;
            //console.log('Login accepted', json);
            if ($('#username').val() == username) {
    $('.login').toggle();
    $('#launcher').toggle();
    $.ajax({
        type: 'GET',
        url: 'http://ygopro.de/deckreader/all.php?username=benblub&callback=',
        dataType: 'jsonp',
        success: function (data) {
            decklistData = data;
            function deck(filename, main, side, extra) {
                var name = filename.substring(0, (filename.length - 4));
                this.name = name;
                this.main = main;
                this.side = side;
                this.extra = extra;
                this.data = JSON.stringify({
                    main: this.main,
                    side: this.side,
                    extra: this.extra
                });
            
            }
            for (var i = 0; i < decklistData.decknames.length; i++) {
                decklist.push(
                    new deck(decklistData.decknames[i],
                        decklistData.Maindeck[i],
                        decklistData.Sidedeck[i],
                        decklistData.extradeck[i]));

            }
            //console.log(decklist);
            for (i = 0; i < decklist.length; i++) {
                $("#selectdeck").append('<option value="' + i + '">' + decklist[i].name + '</option>');
            }
        },
        error: function (er) {
            console.log('Uh Oh!');
            console.log(currenterror = er);
        },

    });

}

            joinroom(activeroom);
        }
        break;
        //--------------
    case 4:
        {
            alert('Invalid Login, Try again.');
            
        }
        break;
        //--------------
    case 10:
        {
            //ping
        }
        break;
        //--------------
    case 11:
        {
            $('#' + json.content).addClass('roomstarted');
            //roomstart
        }
        break;
        //--------------
    case 12:
        {
            for (var i = json.content.length - 1; i >= 0; i--) {
                if (json.content[i].rank >= 1) {
                    rank = "[<span class='dev-" + json.content[i].rank + "''>Dev</span>]";
                }
                $('#users')
                    .append('<li id="userlist-' + json.content[i].username + '"">' + rank + json.content[i].username + '</li>');
            }
            //sortdevs();
            //sortusers();
            //usercount();
        }
        break;
        //--------------
    case 13:
        {
            if (json.content.rank >= 1) {
                rank = "[<span class='dev-" + json.content.rank + "''>Dev</span>]";
            } else {
                rank = "";
            }
            $('#users')
                .append('<li id="userlist-' + json.content.username + '"">' + rank + json.content.username + '</li>');

            
        }
        break;
        //--------------
    case 14:
        {
            $('#userlist-' + json.content.username).remove();
        }
        break;
        //--------------
    case 15:
        {
            console.log('Friends list recived:' + json);
        }
        break;
        //--------------
    case 16:
        {
            console.log('Joined ' + json.content);
        }
        break;
    case 17:
        {
            var name = "";
            switch (json.content.type) {
            case 1:
                {
                    if (json.content.from.rank >= 1) {
                        rank = "[<span class='dev-" + json.content.from.rank + "''>Dev</span>]";
                    } else {
                        rank = "";
                    }
                    if (json.content.command == 1) {
                        name = '<em>' + rank;
                    } else {
                        name = '<strong>' + rank + json.content.from.username + ':</strong> ';
                    }
                    $('#room')
                        .append('<li id="linecount-' + servermessagecount + '">' + name + json.content.message + '</li>');
                    $('#linecount-' + servermessagecount)
                        .urlize();

                }
                break;
            case 2:
                {
                    $('#room')
                        .append('<li class="servermessage" id="linecount-' + servermessagecount + '">Server : ' + json.content.message + '</li>');
                }
                break;
            default:
                {
                    console.log('Unknown ID:17');
                    console.log(json);
                }
            }
        }
        break;
    case 29:
        {
            // server list
            
            for (var server_i = json.content.length - 1; server_i >= 0; server_i--) {
                serverlocations.push(json.content[server_i]);
            }
        }
        break;
        //--------------
    case 37:
        {
            //create room
            //console.log(json);
            var rankunrank;
            if (json.content.isRanked) {
                rankunrank = 'ranked';
            } else {
                rankunrank = 'unranked';
            }
            $('#' + rankunrank).append('<li id="' + json.content.server + '-' + json.content.roomName + '">' + json.content.playerList.concat() + '</li>');

        }
        break;
        //--------------
    default:
        {
            console.log(json);
            
        }
    }
    
    servermessagecount = servermessagecount + 1;
}










function toggleConsole() { // Togggle the console for Unity on and off.
    if (Unityconsole) {
        $('#unityPlayer').css('height', '25%');
        Unityconsole = false;
    } else {
        $('#unityPlayer').css('height', '1px');
        Unityconsole = true;
    }
}

function SetRoomInfo(info) { 
    info = JSON.parse(info);
}

function PosUpdate(pos) { // Used in the lobby to notify the viewer of who is in the lobby.
    console.log('PosUpdate: ' + pos);
}

function PlayerEnter(username, pos) { // Used in the lobby to notify the viewer of who is in the lobby.
    console.log('PlayerEnter: ' + username + ", " + pos);
    $('#lobbyplayer' + pos).html(username);
}

function PlayerLeave(pos) { // Used in the lobby to notify the viewer of who is in the lobby.
    $('#lobbyplayer' + pos).html("");
    $('#lobbystart').attr('class', 'button ready0');
}

function UpdatePlayer(pos, newpos) { // Used in the lobby to notify the viewer of who is in the lobby.
    var UpdatePlayerposscache = $('#lobbyplayer' + pos).html();
    $('#lobbyplayer' + pos).html("");
    $('#lobbyplayer' + newpos).html(UpdatePlayerposscache);
}

function PlayerReady(pos, ready) { // Used in the lobby to notify the viewer of who is in the lobby.
    ready = (ready) ? 1 : 0;
    playerStart[pos] = ready;
    var state = playerStart[0] + playerStart[1];
    $('#lobbyplayer' + pos).toggleClass('ready');
    console.log('button ready' + state);
    $('#lobbystart').attr('class', 'button ready' + state);
    if (state === 2) {
        $('.button.ready2').on('click', function () {
            u.getUnity().SendMessage("GameClient", 'StartDuel', '');
            $('.game').toggle();
            $('.field').toggle();

        });
    }


}
function KickPlayer(pos){
    pos = (pos) ? pos : 1;
    u.getUnity.SendMessage("GameClient","KickPlayer", pos);
}

function PlayerMessage(player, message) { //YGOPro messages.
    var playername;
    if (player) {
        playername = $('#lobbyplayer' + player).html();
    } else {
        playername = 'Spectator';
    }
    $('#messagerbox').css('height', '150px');
    $('#messagerbox ul').append('<li>' + playername + ": " + message + '</li>');
    $('#messagerbox ul, #messagerbox').animate({
        scrollTop: $('#messagerbox ul').height()
    }, "fast");
    console.log(playername + " :" + message);
}

function IsLoaded() { // Disengage saftey mechinisms to prevent the user from accessing the login before unity is loaded.
    saftey = true;
    $('.downloadbutton').toggle();
    $('.originloading').toggle();
    toggleConsole();
    $("#jquery_jplayer_1").jPlayer("play", 0);
}

function DeckError(card) { //When you have an illegal card in your deck.
    MessagePopUp(cardIndex('c'+card).name + " is not legal for this game format");
}

function SelectRPS(value) { // Toggle RPS Screen. Screen will diengage when used.
    $('#rps').toggle();

}

function SelectFirstPlayer(value) { // Select the player that goes first.
    $('#selectduelist').toggle();

}

function StartDuel(data) { // Interface signalled the game has started
    var duelData = JSON.parse(data);
    console.log(duelData);
    player1StartLP = duelData.LifePoints[0];
    player2StartLP = duelData.LifePoints[1];

    $('#player1lp').html("div class='width' style='width:" + (duelData.LifePoints[0] / player1StartLP) + "'></div>" + duelData.LifePoints[0] + "</div>");
    $('#player2lp').html("div class='width' style='width:" + (duelData.LifePoints[1] / player2StartLP) + "'></div>" + duelData.LifePoints[1] + "</div>");

    DOMWriter(duelData.PlayerOneDeckSize, 'Deck', 'p0');
    DOMWriter(duelData.PlayerTwoDeckSize, 'Deck', 'p1');
    DOMWriter(duelData.PlayerOneExtraSize, 'Extra', 'p0');
    DOMWriter(duelData.PlayerTwoExtraSize, 'Extra', 'p1');
    shuffle('p0', 'Deck');
    shuffle('p1', 'Deck');
    shuffle('p0', 'Extra');
    shuffle('p1', 'Extra');
     layouthand('p0');
     layouthand('p1');
}

function DOMWriter(size, movelocation, player) {
    for (i = 0; i < size; i++) {
        animateState('none', 'unknown', 0, player, movelocation, i, 1);
        //animateState(player, clocation, index, moveplayer, movelocation, movezone, moveposition){
    }

}

function UpdateCards(player, clocation, data) { //YGOPro is constantly sending data about game state, this function stores and records that information to allow access to a properly understood gamestate for reference. 
    var update = JSON.parse(data);
    player = 'p' + player;
    var place = cardplace[clocation];
    console.log("Updating Multiple Card Positions for", player + "'s", place);
    try {
        if (update != duel[player][place]){
           // console.log(update);     
        }
        duel[player][place] = update;
        //console.log(duel);
    } catch (error) {
        console.log(error);
        console.log(duel, player, place, update);
    }



}

function UpdateCard(player, clocation, index, data) {
    var update = JSON.parse(data);
    player = 'p' + player;
    console.log("Updating Single Card Position", update, player + " ", "Card : " + index, cardplace[clocation]);
    if (duel[player][cardplace[clocation]][index] != update){
        $('.card.'+player+'.'+[cardplace[clocation]]+'.i'+index+' .front').css('background',
            "yellow url(http://ygopro.de/img/cardpics/"+update.Id+'.jpg) no-repeat auto 0 0 cover');
    }
    
    duel[player][cardplace[clocation]][index] = update;

}

function DrawCard(player, numberOfCards) {
    console.log("p" + player + " drew " + numberOfCards + " card(s)");
    animateDrawCard("p"+player,numberOfCards);
    layouthand('p'+player);
}

function NewPhase(phase) {
    console.log(enumPhase[phase]);
}

function NewTurn(turn) {
    console.log("It is now p" + turn + "'s turn.");
}

function MoveCard(player, clocation, index, moveplayer, movelocation, movezone, moveposition) {
    console.log('p' + player + "'s' ", cardplace[clocation], index, "Moved to p" + moveplayer + "s", cardplace[movelocation], movezone, moveposition);
    animateState('p'+player, cardplace[clocation], index, 'p'+moveplayer, cardplace[movelocation], movezone, moveposition);
    //animateState(player, clocation, index, moveplayer, movelocation, movezone, moveposition);
     layouthand('p'+moveplayer);
}

function OnWin(result) {
    console.log("Function OnWin: " + result);
}

function SelectCards(cards, min, max, cancelable) {
    var debugObject = {
        cards: cards,
        min: min,
        max: max,
        cancelable: cancelable
    };
    console.log('Function SelectCards:' + JSON.stringify(debugObject));
}

function DuelEnd() {
    console.log('Duel has ended.');
}

function SelectYn(description) {
    console.log("Function SelectYn :" + description);
}

function IdleCommands(main) {
    var update = JSON.parse(main); 
    console.log('IdleCommands', update);
    u.getUnity().SendMessage("GameClient", 'NewTurn', 1);
    
}

function SelectPosition(positions) {
    var debugObject = JSON.Strigify(positions);
    console.log(debugObject);
}

function SelectOption(options) {
    var debugObject = JSON.Strigify(options);
    console.log(debugObject);
}

function AnnounceCard() {
    //Select a card from all known cards.
    console.log('AnnounceCard');
}

function OnChaining(cards, desc, forced) {
    var cardIDs = JSON.parse(cards);
    
    for(var i=0; i < cardIDs.length; i++){
        animateChaining(('p'+cardIDs[i].Player), cardplace[cardIDs[i].location],cardIDs[i].Index);
    }
    
    //auto say no
    if (forced){
        //modual Q
         u.getUnity().SendMessage('GameClient', 'SendOnChain', 0);
        animateRemoveChaining();
    }

    else {
        //modual Q
         u.getUnity().SendMessage('GameClient', 'SendOnChain', -1);
        animateRemoveChaining();
    }
    console.log('chaining', cardIDs, desc);

}
function ShuffleDeck(player){
    console.log(player);
    shuffle('p'+player, 'Deck');
}
function debugField(){
    $('.field').toggle();
     DOMWriter(40, 'Deck', 'p0');
     DOMWriter(40, 'Deck', 'p1');
     DOMWriter(15, 'Extra', 'p0');
     DOMWriter(15, 'Extra', 'p1');
     DrawCard('p0', 5);
     DrawCard('p1', 5);
     layouthand('p0');
     layouthand('p1');
    
     
}





//chat

function messageon(private){
                private  = 0;
                if ( $('#chatform input').val() === ""){return false;}
                messagesend = $('#chatform input').val();
                $('#chatform input').val(""); 

                var tosend= {location : activeroom, message :  messagesend, isprivate: false};
                tosend = JSON.stringify(tosend);
                u.getUnity().SendMessage('HubClient','ChatMessage', tosend);
                autoscroll = 60;
    e.preventDefault();            
    return true;

        }