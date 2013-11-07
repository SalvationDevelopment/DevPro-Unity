/* globals $,UnityObject2, jQuery,showUnsupported, alert, document,isChecked,randomString, console, clearInterval, setInterval, setTimeout */
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
var i = 0; // counter for forLoops.

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
    $.getJSON("http://ygopro.de/cardreader/index.php?folder=English&callback=?", function (data) {
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
        console.log(compl);


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
    $('#igofirst').on("click", function () {
        $('#opponentfirst').toggle();
        u.getUnity().SendMessage("GameClient", 'SelectFirstPlayer', 1);

    });
    $('#messagerbox .close').on('click', function () {
        $('#messagerbox').css('height', '0px');
    });
    $("#jquery_jplayer_1").jPlayer("play", 0);
});






function MessageBrowser(message) {
    console.log(message);
}

function MessagePopUp(message) {
    console.log(message);
}

function LoginAccept(username) {
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
                console.log(decklist);
                for ( i = 0; i < decklist.length; i++) {
                    $("#selectdeck").append('<option value="' + i + '">' + decklist[i].name + '</option>');
                }
            },
            error: function (er) {
                console.log('Uh Oh!');
                console.log(currenterror = er);
            },

        });

    }
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
    console.log('PlayerLeave: ' + pos);
    $('#lobbyplayer' + pos).html("");
    $('#lobbystart').attr('class', 'button ready0');
}

function UpdatePlayer(pos, newpos) {
    console.log('UpdatePlayer: ' + pos + ' to :');
    var UpdatePlayerposscache = $('#lobbyplayer' + pos).html();
    $('#lobbyplayer' + pos).html("");
    $('#lobbyplayer' + newpos).html(UpdatePlayerposscache);
}

function PlayerReady(pos, ready) {
    ready = (ready) ? 1 : 0;
    console.log('PlayerReady: ' + pos + ' is :' + ready);
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