﻿Imports System.IO
Imports System.Threading
Imports System.Windows.Forms

Module ModGeneral

    Friend Started As Boolean

    Friend Function GetTickCount() As Integer
        Return Environment.TickCount
    End Function

    Sub Startup()
        SFML.CSFML.Activate()

        SetStatus(Strings.Get("loadscreen", "loading"))

        FrmMenu.Visible = True

        ReDim CharSelection(3)

        ReDim Player(MAX_PLAYERS)

        For i = 1 To MAX_PLAYERS
            ClearPlayer(i)
        Next

        ClearAutotiles()

        'Housing
        ReDim House(MaxHouses)
        ReDim HouseConfig(MaxHouses)

        'quests
        ClearQuests()

        'npc's
        ClearNpcs()
        ReDim Map.Npc(MAX_MAP_NPCS)
        ReDim MapNpc(MAX_MAP_NPCS)
        For i = 0 To MAX_MAP_NPCS
            For x = 0 To VitalType.Count - 1
                ReDim MapNpc(i).Vital(x)
            Next
        Next

        ClearShops()

        ClearAnimations()

        ClearAnimInstances()

        ClearBank()

        ReDim MapProjectiles(MaxProjectiles)
        ReDim Projectiles(MaxProjectiles)

        ClearItems()

        'craft
        ClearRecipes()

        'party
        ClearParty()

        'pets
        ClearPets()

        GettingMap = True
        vbQuote = Chr(34) ' "

        ' Update the form with the game's name before it's loaded
        frmGame.Text = GameName

        SetStatus(Strings.Get("loadscreen", "options"))

        ' load options
        If File.Exists(Application.StartupPath & "\Data\Config.xml") Then
            LoadOptions()
        Else
            CreateOptions()
        End If

        ' randomize rnd's seed
        Randomize()

        SetStatus(Strings.Get("loadscreen", "network"))

        FrmMenu.Text = GameName

        ' DX7 Master Object is already created, early binding
        SetStatus(Strings.Get("loadscreen", "graphics"))
        CheckTilesets()
        CheckCharacters()
        CheckPaperdolls()
        CheckAnimations()
        CheckItems()
        CheckResources()
        CheckSkillIcons()
        CheckFaces()
        CheckFog()
        CacheMusic()
        CacheSound()
        CheckEmotes()
        CheckPanoramas()
        CheckFurniture()
        CheckProjectiles()
        CheckParallax()

        InitGraphics()

        ' check if we have main-menu music
        If Options.Music = 1 AndAlso Len(Trim$(Options.MenuMusic)) > 0 Then
            PlayMusic(Trim$(Options.MenuMusic))
            MusicPlayer.Volume() = 100
        End If

        ' Reset values
        Ping = -1

        ' set values for directional blocking arrows
        DirArrowX(1) = 12 ' up
        DirArrowY(1) = 0
        DirArrowX(2) = 12 ' down
        DirArrowY(2) = 23
        DirArrowX(3) = 0 ' left
        DirArrowY(3) = 12
        DirArrowX(4) = 23 ' right
        DirArrowY(4) = 12

        'set gui switches
        HUDVisible = True

        SetStatus(Strings.Get("loadscreen", "starting"))
        started = True
        frmmenuvisible = True
        pnlloadvisible = False

        pnlInventoryVisible = True

        InitNetwork()

        GameLoop()
    End Sub

    Friend Function IsLoginLegal(username As String, password As String) As Boolean
        Return Len(Trim$(Username)) >= 3 AndAlso Len(Trim$(Password)) >= 3
    End Function

    Friend Function IsStringLegal(sInput As String) As Boolean
        Dim i As Integer

        ' Prevent high ascii chars
        For i = 1 To Len(sInput)

            If (Asc(Mid$(sInput, i, 1))) < 32 OrElse Asc(Mid$(sInput, i, 1)) > 126 Then
                MsgBox(Strings.Get("mainmenu", "stringlegal"), vbOKOnly, GameName)
                IsStringLegal = False
                Exit Function
            End If

        Next

        IsStringLegal = True
    End Function

    Sub GameInit()
        pnlloadvisible = False

        ' Set the focus
        frmGame.picscreen.Focus()

        'stop the song playing
        StopMusic()
    End Sub

    Friend Sub SetStatus(caption As String)
        FrmMenu.lblStatus.Text = Caption
    End Sub

    Friend Sub MenuState(state As Integer)
        pnlloadvisible = True
        frmmenuvisible = False
        Select Case State
            Case MenuStateAddchar
                pnlCharCreateVisible = False
                pnlLoginVisible = False
                pnlRegisterVisible = False
                pnlCreditsVisible = False

                If ConnectToServer(1) Then
                    SetStatus(Strings.Get("mainmenu", "sendaddchar"))

                    If FrmMenu.rdoMale.Checked = True Then
                        SendAddChar(SelectedChar, FrmMenu.txtCharName.Text, SexType.Male, FrmMenu.cmbClass.SelectedIndex + 1, newCharSprite)
                    Else
                        SendAddChar(SelectedChar, FrmMenu.txtCharName.Text, SexType.Female, FrmMenu.cmbClass.SelectedIndex + 1, newCharSprite)
                    End If
                End If

            Case MenuStateNewaccount
                pnlLoginVisible = False
                pnlCharCreateVisible = False
                pnlRegisterVisible = False
                pnlCreditsVisible = False

                If ConnectToServer(1) Then
                    SetStatus(Strings.Get("mainmenu", "sendnewacc"))
                    SendNewAccount(FrmMenu.txtRuser.Text, FrmMenu.txtRPass.Text)
                End If

            Case MenuStateLogin
                pnlLoginVisible = False
                pnlCharCreateVisible = False
                pnlRegisterVisible = False
                pnlCreditsVisible = False
                TempUserName = FrmMenu.txtboxLogin.Text
                TempPassword = FrmMenu.txtboxPassword.Text

                If ConnectToServer(1) Then
                    SetStatus(Strings.Get("mainmenu", "sendlogin"))
                    SendLogin(FrmMenu.txtboxLogin.Text, FrmMenu.txtboxPassword.Text)
                    Exit Sub
                End If
        End Select

    End Sub

    Friend Function ConnectToServer(i As Integer) As Boolean
        Dim until As Integer
        ConnectToServer = False

        ' Check to see if we are already connected, if so just exit
        If Socket.IsConnected() Then
            ConnectToServer = True
            Exit Function
        End If

        If i = 4 Then Exit Function
        until = GetTickCount() + 3500

        Connect()

        SetStatus(Strings.Get("mainmenu", "connectserver", i))

        ' Wait until connected or a few seconds have passed and report the server being down
        Do While (Not Socket.IsConnected()) AndAlso (GetTickCount() <= until)
            Application.DoEvents()
            Thread.Sleep(10)
        Loop

        ' return value
        If Socket.IsConnected() Then
            ConnectToServer = True
        End If

        If Not ConnectToServer Then
            ConnectToServer(i + 1)
        End If

    End Function

    Friend Sub RePositionGui()

        'first change the tiles
        If Options.ScreenSize = 0 Then ' 800x600
            ScreenMapx = 25
            ScreenMapy = 19
        ElseIf Options.ScreenSize = 1 Then '1024x768
            ScreenMapx = 31
            ScreenMapy = 24
        ElseIf Options.ScreenSize = 2 Then
            ScreenMapx = 35
            ScreenMapy = 26
        End If

        'then the window
        frmGame.ClientSize = New Drawing.Size((ScreenMapx) * PicX + PicX, (ScreenMapy) * PicY + PicY)
        frmGame.picscreen.Width = (ScreenMapx) * PicX + PicX
        frmGame.picscreen.Height = (ScreenMapy) * PicY + PicY

        HalfX = ((ScreenMapx) \ 2) * PicX
        HalfY = ((ScreenMapy) \ 2) * PicY
        ScreenX = (ScreenMapx) * PicX
        ScreenY = (ScreenMapy) * PicY

        GameWindow.SetView(New SFML.Graphics.View(New SFML.Graphics.FloatRect(0, 0, (ScreenMapx) * PicX + PicX, (ScreenMapy) * PicY + PicY)))

        'Then we can recalculate the positions

        'chatwindow
        ChatWindowX = 1
        ChatWindowY = frmGame.Height - ChatWindowGFXInfo.Height - 65

        MyChatX = 1
        MyChatY = frmGame.Height - 60

        'hotbar
        If Options.ScreenSize = 0 Then
            HotbarX = HUDWindowX + HUDPanelGFXInfo.Width + 20
            HotbarY = 5

            'petbar
            PetbarX = HotbarX
            PetbarY = HotbarY + 34
        Else
            HotbarX = ChatWindowX + MyChatWindowGFXInfo.Width + 50
            HotbarY = frmGame.Height - HotBarGFXInfo.Height - 45

            'petbar
            PetbarX = HotbarX
            PetbarY = HotbarY - 34
        End If

        'action panel
        ActionPanelX = frmGame.Width - ActionPanelGFXInfo.Width - 25
        ActionPanelY = frmGame.Height - ActionPanelGFXInfo.Height - 45

        'Char Window
        CharWindowX = frmGame.Width - CharPanelGFXInfo.Width - 26
        CharWindowY = frmGame.Height - CharPanelGFXInfo.Height - ActionPanelGFXInfo.Height - 50

        'inv Window
        InvWindowX = frmGame.Width - InvPanelGFXInfo.Width - 26
        InvWindowY = frmGame.Height - InvPanelGFXInfo.Height - ActionPanelGFXInfo.Height - 50

        'skill window
        SkillWindowX = frmGame.Width - SkillPanelGFXInfo.Width - 26
        SkillWindowY = frmGame.Height - SkillPanelGFXInfo.Height - ActionPanelGFXInfo.Height - 50

        'petstat window
        PetStatX = PetbarX
        PetStatY = PetbarY - PetStatsGFXInfo.Height - 10
    End Sub

    Friend Sub DestroyGame()
        'SendLeaveGame()
        ' break out of GameLoop
        InGame = False

        DestroyGraphics()
        GameDestroyed = True
        DestroyNetwork()
        Application.Exit()
        End
    End Sub

    Friend Sub CheckDir(dirPath As String)

        If Not IO.Directory.Exists(DirPath) Then
            IO.Directory.CreateDirectory(DirPath)
        End If

    End Sub

    Friend Function GetExceptionInfo(ex As Exception) As String
        Dim result As String
        Dim hr As Integer = Runtime.InteropServices.Marshal.GetHRForException(ex)
        Result = ex.GetType.ToString & "(0x" & hr.ToString("X8") & "): " & ex.Message & Environment.NewLine & ex.StackTrace & Environment.NewLine
        Dim st As StackTrace = New StackTrace(ex, True)
        For Each sf As StackFrame In st.GetFrames
            If sf.GetFileLineNumber() > 0 Then
                Result &= "Line:" & sf.GetFileLineNumber() & " Filename: " & IO.Path.GetFileName(sf.GetFileName) & Environment.NewLine
            End If
        Next
        Return Result
    End Function
End Module