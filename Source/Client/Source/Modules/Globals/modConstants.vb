﻿Imports System.Drawing

Module ModConstants

    'Chatbubble
    Friend Const ChatBubbleWidth As Integer = 100

    Friend Const EffectTypeFadein As Integer = 1
    Friend Const EffectTypeFadeout As Integer = 2
    Friend Const EffectTypeFlash As Integer = 3
    Friend Const EffectTypeFog As Integer = 4
    Friend Const EffectTypeWeather As Integer = 5
    Friend Const EffectTypeTint As Integer = 6

    ' path constants
    Friend Const SoundPath As String = "\Data\sound\"
    Friend Const MusicPath As String = "\Data\music\"

    ' Font variables
    Friend Const FontName As String = "Arial.ttf"
    Friend Const FontSize As Byte = 13

    ' Log Path and variables
    Friend Const LogDebug As String = "debug.txt"
    Friend Const LogPath As String = "\Data\logs\"

    ' Gfx Path and variables
    Friend Const GfxPath As String = "\Data\graphics\"
    Friend Const GfxGuiPath As String = "\Data\graphics\gui\"
    Friend Const GfxExt As String = ".png"

    ' Menu states
    Friend Const MenuStateNewaccount As Byte = 0
    Friend Const MenuStateDelaccount As Byte = 1
    Friend Const MenuStateLogin As Byte = 2
    Friend Const MenuStateGetchars As Byte = 3
    Friend Const MenuStateNewchar As Byte = 4
    Friend Const MenuStateAddchar As Byte = 5
    Friend Const MenuStateDelchar As Byte = 6
    Friend Const MenuStateUsechar As Byte = 7
    Friend Const MenuStateInit As Byte = 8

    ' Number of tiles in width in tilesets
    Friend Const TilesheetWidth As Integer = 15 ' * PIC_X pixels

    Friend MapGrid As Boolean

    ' Speed moving vars
    Friend Const WalkSpeed As Byte = 6
    Friend Const RunSpeed As Byte = 10

    'Inertia stuff
    Friend Const INERTING_NORMAL As Byte = 1
    Friend Const INERTING_WATER As Byte = 2

    ' Tile size constants
    Friend Const PicX As Integer = 32
    Friend Const PicY As Integer = 32

    ' Sprite, item, skill size constants
    Friend Const SizeX As Integer = 32
    Friend Const SizeY As Integer = 32

    ' ********************************************************
    ' * The values below must match with the server's values *
    ' ********************************************************

    ' General constants
    Friend GameName As String = "Orion+"

    ' Website
    Friend Const GameWebsite As String = "http://ascensiongamedev.com/index.php"

    ' Map constants
    Friend ScreenMapx As Byte = 35
    Friend ScreenMapy As Byte = 26

    Friend ItemRarityColor0 = SFML.Graphics.Color.White ' white
    Friend ItemRarityColor1 = New SFML.Graphics.Color(102, 255, 0) ' green
    Friend ItemRarityColor2 = New SFML.Graphics.Color(73, 151, 208) ' blue
    Friend ItemRarityColor3 = New SFML.Graphics.Color(255, 0, 0) ' red
    Friend ItemRarityColor4 = New SFML.Graphics.Color(159, 0, 197) ' purple
    Friend ItemRarityColor5 = New SFML.Graphics.Color(255, 215, 0) ' gold

    Friend HalfX As Integer = ((ScreenMapx + 1) \ 2) * PicX
    Friend HalfY As Integer = ((ScreenMapy + 1) \ 2) * PicY
    Friend ScreenX As Integer = (ScreenMapx + 1) * PicX
    Friend ScreenY As Integer = (ScreenMapy + 1) * PicY

    'dialog types
    Friend Const DialogueTypeBuyhome As Byte = 1
    Friend Const DialogueTypeVisit As Byte = 2
    Friend Const DialogueTypeParty As Byte = 3
    Friend Const DialogueTypeQuest As Byte = 4
    Friend Const DialogueTypeTrade As Byte = 5

End Module
