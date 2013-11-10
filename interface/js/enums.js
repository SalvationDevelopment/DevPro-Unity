var cardpositions = {
    
    'p0' : { 
        Deck : {
            x_origin :735, // player 1
            y_origin : 43
        },
        Hand : {
            x_origin : 124,
            y_origin : -10
        },
        ExtraDeck : {
            x_origin :22,
            y_origin :43
        },
        Field :{
            x_origin :22,
            y_origin :181
        },
        Spells : {
            zone1: {
                x_origin : 144,
                y_origin : 188
            },
            zone2: {
                x_origin : 261,
                y_origin : 188
            },
            zone3: {
                x_origin : 379,
                y_oirgin : 188
            },
            zone4: {
                x_origin : 497,
                y_origin : 188
            },
            zone5: {
                x_origin : 614,
                y_origin : 188
            }
        },
        MonsterZone : {
            zone1: {
                x_origin : 144,
                y_origin : 250
            },
            zone2: {
                x_origin : 261,
                y_origin : 250
            },
            zone3: {
                x_origin : 379,
                y_oirgin : 250
            },
            zone4: {
                x_origin : 497,
                y_origin : 250
            },
            zone5: {
                x_origin : 614,
                y_origin : 250
            }
        }
    
    
    },
    'p1' : { 
        Deck : {
            x_origin :735, // player 2
            y_origin : 686
        },
        Hand : {
            x_origin : 755,
            y_origin : 643
        },
        ExtraDeck : {
            x_origin :735,
            y_origin :686
        },
        Field :{
            x_origin :735,
            y_origin :546
        },
        Spells : {
            zone1: {
                x_origin : 614,
                y_origin : 610
            },
            zone2: {
                x_origin : 497,
                y_origin : 610
            },
            zone3: {
                x_origin : 379,
                y_oirgin : 610
            },
            zone4: {
                x_origin : 261,
                y_origin : 610
            },
            zone5: {
                x_origin : 144,
                y_origin : 610
            }
        },
        MonsterZone : {
            zone1: {
                x_origin : 614,
                y_origin : 548
            },
            zone2: {
                x_origin : 497,
                y_origin : 548
            },
            zone3: {
                x_origin : 379,
                y_oirgin : 548
            },
            zone4: {
                x_origin : 261,
                y_origin : 548
            },
            zone5: {
                x_origin : 144,
                y_origin : 548
            }
        }
    
    
    }
    
};
var cardplace =  {
        1 :  'Deck' ,
        2 :  'Hand',
        4 :  'MonsterZone',
        8 :  'SpellZone',
        16:  'Grave',
        32:  'Removed',
        64:  'Extra',
        128: 'Overlay',
        12:  'Onfield'        
    };
var enumPhase =
    {
        1:  'Draw Phase',
        2:  'Standby Phase',
        4:  'Main Phase 1',
        8:  'Battle Phase',
        16: 'Battle Calculation',
        32: 'Main Phase 2',
        64: 'End Phase'
    };
var winconditions = ['Opponent Quit', 'time'];