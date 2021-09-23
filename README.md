# Aoe2DEOverlay
Aoe2 DE ELO Overlay

## Install App

For an easy installation the latest version can be downloaded from github [releases](https://github.com/kickass-panda/Aoe2DEOverlay/releases). There is a x64 (64-bit CPU) or x86 (32-bit CPU) version. Unzip the file and copy the folder to a desired location. 

## Setup ProfileId

First we need the ProfileId from aoe2.net. On the website under Leaderboards you find the choice between `Random Map`, `Team Random Map`, `Empires Wars`, `Team Empires Wars` and `Unranked`. Select one of the game modes in which you have played at least 10 games. After that you can search for your player name with the search input field. Then click on your name and a dialog will open. In the dialog click on profiles to get to the profiles page. In the address bar you will find the ProfileId. Copy it and paste it in the setting.json as value (replace null) for `profileId`. You can open the setting.json file with the windows editor.

Example: `profileId: 123`

## Start

Run the `Aoe2DEOverlay.exe` to start the overlay. Know the last or currently playing match is automatically displayed. The update can sometimes take a few seconds after the match start. 

## Layouting

By default it is displayed in the top center. You can adjust the `horizontal` alignment by set the `left`, `right`, or `center` values in the `setting.json`. For the `vertical` alignment you can use the values `top`, `bottom`, or `center`.

Example:

    "vertical": 'top',
    "horizontal": 'center',

You can also adjust the positioning by set the values `top`, `left`, `right` or `button` in `setting.json`.

Example:

    "bottom": 50,
    "right": 700,


## Formatting

It automated dedect if you play RM or EW so you can just use `1v1` or `team` to display the correct data.

`{slot}`   
`{name}` Name of the Player  
`{country}` Country of the Player  
`{1v1.rank}` The leaderboard rank position of the Player  
`{1v1.elo}`  The elo points of the Player
`{1v1.rate}`  
`{1v1.streak}`  
`{1v1.games}`  
`{1v1.wins}`  
`{1v1.losses}`  
`{team.rank}`  
`{team.elo}`  
`{team.rate}`  
`{team.streak}`  
`{team.games}`  
`{mTeam.wins}`  
`{mTeam.losses}`  
