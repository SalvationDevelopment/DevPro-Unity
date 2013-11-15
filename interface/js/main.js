/* globals $,UnityObject2, jQuery,showUnsupported, alert, document,isChecked,randomString, console, clearInterval, setInterval, setTimeout,shuffle, cardplace, cardpositions, enumPhase, animateState */
/* DEAR GOD ACCESSDENIED LEARN TO USE A FOR LOOP!
for (i = 0; i < size; i++) {
}
*/
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


function deck(filename, main, side, extra) {
    //    if (typeof name  !== "string"){console.log('name must be a string'); return false}
    //    if (typeof main  !== "array") {console.log('main must be a array'); return false}
    //    if (typeof side  !== "array") {console.log('side must be a array'); return false}
    //    if (typeof extra !== "array") {console.log('extra must be a array'); return false}
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
            $('body').css({
                'background': 'url(http://ygopro.de/img/bg_black.png)'
            });
        } else {
            alert('just one moment, server connection system is loading.');

        }
    });
    $('#loginbutton').on('click', function () {
        u.getUnity().SendMessage("HubClient", "Login", "{'Username' : '" + ($('#username').val()) + "', 'Password' : '" + ($('#password').val()) + "', 'UID' : 'Unity'}");
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
        u.getUnity().SendMessage("GameClient", 'SelectFirstPlayer', 0);

    });
    $('#opponentfirst').on("click", function () {
        $('#selectduelist').toggle();
        u.getUnity().SendMessage("GameClient", 'SelectFirstPlayer', 1);

    });
    $('#messagerbox .close').on('click', function () {
        $('#messagerbox').css('height', '0px');
    });
    $("#jquery_jplayer_1").jPlayer("play", 0);
});


function joinroom(roomtojoin){
        activeroom = roomtojoin;
        
         u.getUnity().SendMessage("HubClient", '', 9, roomtojoin);
        $('#chatbox').append('<ul class="room active" id=room-'+roomtojoin+'></ul>');
        $('#chatrooms').append('<li class="active" id=control-'+roomtojoin+'>'+roomtojoin+'</li>');
        $('#chatbox ul, #chatrooms li').not('#'+roomtojoin).removeClass('active');
        var currentroom = 'room-'+roomtojoin;
}




function MessageBrowser(message) {
    console.log(message);
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
//            chatserver.socket.send(JSON.stringify({
//                id: 6,
//                content: ''
//            }));
//            joinroom(activeroom);
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
                    $('#room-' + json.content.channel)
                        .append('<li id="linecount-' + servermessagecount + '">' + name + json.content.message + '</li>');
                    $('#linecount-' + servermessagecount)
                        .urlize();

                }
                break;
            case 2:
                {
                    $('#room-' + activeroom)
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
            alert('new data');
        }
    }
    
    servermessagecount = servermessagecount + 1;
}









function LoginAccept(username){

}

function toggleConsole() {
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

function PosUpdate(pos) {
    console.log('PosUpdate: ' + pos);
}

function PlayerEnter(username, pos) {
    console.log('PlayerEnter: ' + username + ", " + pos);
    $('#lobbyplayer' + pos).html(username);
}

function PlayerLeave(pos) {
    $('#lobbyplayer' + pos).html("");
    $('#lobbystart').attr('class', 'button ready0');
}

function UpdatePlayer(pos, newpos) {
    var UpdatePlayerposscache = $('#lobbyplayer' + pos).html();
    $('#lobbyplayer' + pos).html("");
    $('#lobbyplayer' + newpos).html(UpdatePlayerposscache);
}

function PlayerReady(pos, ready) {
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

function PlayerMessage(player, message) {
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

function IsLoaded() {
    saftey = true;
    $('.downloadbutton').toggle();
    $('.originloading').toggle();
    toggleConsole();
    $("#jquery_jplayer_1").jPlayer("play", 0);
}

function messageUnity(functionName, message) {
    u.getUnity().SendMessage("HubClient", functionName, message);
}

function DeckError(card) {
    MessagePopUp(cardIndex(card).name + " is not legal for this game format");
}

function SelectRPS(value) {
    $('#rps').toggle();

}

function SelectFirstPlayer(value) {
    $('#selectduelist').toggle();

}

function StartDuel(data) {
    var duelData = JSON.parse(data);
    console.log(duelData);
    player1StartLP = duelData.LifePoints[0];
    player2StartLP = duelData.LifePoints[1];

    $('#player1lp').html("div class='width' style='width:" + (duelData.LifePoints[0] / player1StartLP) + "'></div>" + duelData.LifePoints[0] + "</div>");
    $('#player2lp').html("div class='width' style='width:" + (duelData.LifePoints[1] / player2StartLP) + "'></div>" + duelData.LifePoints[1] + "</div>");

    var a = new DOMWriter(duelData.PlayerOneDeckSize, 'Deck', 'p0');
    var b = new DOMWriter(duelData.PlayerTwoDeckSize, 'Deck', 'p1');
    var c = new DOMWriter(duelData.PlayerOneExtraSize, 'Extra', 'p0');
    var d = new DOMWriter(duelData.PlayerTwoExtraSize, 'Extra', 'p1');
    shuffle();
}

function DOMWriter(size, movelocation, player) {
    for (i = 0; i < size; i++) {
        animateState('none', 'unknown', i, player, movelocation, i, 'FaceDownAttack');
        //animateState(player, clocation, index, moveplayer, movelocation, movezone, moveposition){
    }

}

function UpdateCards(player, clocation, data) {
    var update = JSON.parse(data);
    player = 'p' + player;
    var place = cardplace[clocation];
    console.log("Updating Multiple Card Positions for", player + "'s", place);
    try {
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


    duel[player][cardplace[clocation]][index] = update;

}

function DrawCard(player, numberOfCards) {
    console.log("p" + player + " drew " + numberOfCards + " card(s)");
}

function NewPhase(phase) {
    console.log(enumPhase[phase]);
}

function NewTurn(turn) {
    console.log("It is now p" + turn + "'s turn.");
}

function MoveCard(player, clocation, index, moveplayer, movelocation, movezone, moveposition) {
    console.log('p' + player + "'s' ", cardplace[clocation], index, "Moved to p" + moveplayer + "s", cardplace[movelocation], movezone, moveposition);
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

    console.log(main);
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
    var cardIDs = JSON.stringify(cards);
    var cardDescripts = JSON.stringify(desc);
    console.log(cardIDs, cardDescripts);

}