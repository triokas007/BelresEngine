Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports ASFW

Friend Class FrmMenu
    Inherits Form
#Region "Form Functions"
    ''' <summary>
    ''' clean up and close the game.
    ''' </summary>
    Private Sub FrmMenu_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        DestroyGame()
    End Sub

    ''' <summary>
    ''' On load, get GUI ready.
    ''' </summary>
    Private Sub Frmmenu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim srnWidth As Integer
        Dim srnHeight As Integer
        Dim space As Integer

        Strings.Init(0, "English")

        LoadMenuGraphics()

        'Set Space
        space = 10

        'Width
        srnWidth = Screen.PrimaryScreen.Bounds.Width
        'Height
        srnHeight = Screen.PrimaryScreen.Bounds.Height

        'Setting panel size
        pnlLoad.Width = srnWidth
        pnlLoad.Height = srnHeight

        'Making login visible
        pnlLogin.Visible = True

        'Positioning the login form
        pnlLogin.Left = (srnWidth / 2) - (pnlLogin.Width / 2)
        pnlLogin.Top = (srnHeight / 2) - (pnlLogin.Height / 2)

        'Class select pos
        pnlNewChar.Left = 0
        pnlNewChar.Top = 0

        'exit button pos
        btnExit.Left = srnWidth - (btnExit.Width + space)
        btnExit.Top = srnHeight - (btnExit.Height + space)

        'Register button pos
        btnRegister.Left = srnWidth - (btnRegister.Width + space)
        btnRegister.Top = srnHeight - (btnExit.Height + (space * 2) + btnRegister.Height)

        'Make buttons invisible
        btnPlay.Visible = False
        'btnRegister.Visible = False
        btnCredits.Visible = False

        'Placement of the game logo
        picLogo.Left = (srnWidth / 2) - (picLogo.Width / 2)
        picLogo.Top = pnlLogin.Top - (picLogo.Height)

        If Started = False Then Call Startup()

        Connect()

    End Sub

    ''' <summary>
    ''' Draw Char select when its needed.
    ''' </summary>
    Private Sub PnlCharSelect_VisibleChanged(sender As Object, e As EventArgs) Handles pnlCharSelect.VisibleChanged
        DrawCharacterSelect()
    End Sub

    ''' <summary>
    ''' Shows the IP config.
    ''' </summary>
    Private Sub LblServerStatus_Click(sender As Object, e As EventArgs)
        PnlCreditsVisible = False
        PnlLoginVisible = False
        PnlRegisterVisible = False
        PnlCharCreateVisible = False

        txtIP.Text = Options.Ip
        txtPort.Text = Options.Port

        pnlIPConfig.Visible = True
    End Sub
#End Region

#Region "Draw Functions"
    ''' <summary>
    ''' Preload the images in the menu.
    ''' </summary>
    Friend Sub LoadMenuGraphics()

        'main menu
        If File.Exists(Application.StartupPath & GfxGuiPath & "Menu\menu" & GfxExt) Then
            BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\menu" & GfxExt)
        End If

        'main menu buttons
        If File.Exists(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt) Then
            btnCredits.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
            btnExit.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
            btnLogin.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
            btnPlay.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
            btnRegister.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
            btnNewChar.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
            btnUseChar.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
            btnDelChar.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
            btnCreateAccount.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
            btnSaveIP.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
        End If

        'main menu panels
        If File.Exists(Application.StartupPath & GfxGuiPath & "Menu\panel" & GfxExt) Then
            pnlLogin.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\login" & GfxExt)
            pnlNewChar.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\class_none" & GfxExt)
            pnlCharSelect.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\panel" & GfxExt)
            pnlRegister.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\panel" & GfxExt)
            pnlCredits.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\panel" & GfxExt)
            pnlIPConfig.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\panel" & GfxExt)
        End If

        'logo
        If File.Exists(Application.StartupPath & GfxGuiPath & "Menu\logo" & GfxExt) Then
            picLogo.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\logo" & GfxExt)
        End If

        'set text for controls from language file

        'main
        btnPlay.Text = Strings.Get("mainmenu", "buttonplay")
        btnRegister.Text = Strings.Get("mainmenu", "buttonregister")
        btnCredits.Text = Strings.Get("mainmenu", "buttoncredits")
        btnExit.Text = Strings.Get("mainmenu", "buttonexit")

        'logon panel
        'lblLoginName.Text = Strings.Get("mainmenu", "loginname")
        ' lblLoginPass.Text = Strings.Get("mainmenu", "loginpass")
        btnLogin.Text = Strings.Get("mainmenu", "loginbutton")

        'new char panel
        'lblNewChar.Text = Strings.Get("mainmenu", "newchar")
        rdoMale.Text = Strings.Get("mainmenu", "newcharmale")
        rdoFemale.Text = Strings.Get("mainmenu", "newcharfemale")
        'lblNewCharSprite.Text = Strings.Get("mainmenu", "newcharsprite")
        btnCreateCharacter.Text = Strings.Get("mainmenu", "newcharbutton")

        'char select
        lblCharSelect.Text = Strings.Get("mainmenu", "selchar")
        btnNewChar.Text = Strings.Get("mainmenu", "selcharnew")
        btnUseChar.Text = Strings.Get("mainmenu", "selcharuse")
        btnDelChar.Text = Strings.Get("mainmenu", "selchardel")

        'new account
        lblNewAccount.Text = Strings.Get("mainmenu", "newacc")
        lblNewAccName.Text = Strings.Get("mainmenu", "newaccname")
        lblNewAccPass.Text = Strings.Get("mainmenu", "newaccpass")
        lblNewAccPass2.Text = Strings.Get("mainmenu", "newaccpass2")

        'credits
        lblCreditsTop.Text = Strings.Get("mainmenu", "credits")

        'ip config
        lblIpConfig.Text = Strings.Get("mainmenu", "ipconfig")
        lblIpAdress.Text = Strings.Get("mainmenu", "ipconfigadres")
        lblPort.Text = Strings.Get("mainmenu", "ipconfigport")
    End Sub

    ''' <summary>
    ''' Draw the Character for new char creation.
    ''' </summary>
    Sub DrawCharacter()
        If pnlNewChar.Visible = True Then
            Dim g As Graphics = pnlNewChar.CreateGraphics
            Dim filename As String
            Dim srcRect As Rectangle
            Dim destRect As Rectangle
            Dim charwidth As Integer
            Dim charheight As Integer

            If NewCharClass = 0 Then NewCharClass = 1
            If NewCharSprite = 0 Then NewCharSprite = 1

            If rdoMale.Checked = True Then
                filename = Application.StartupPath & GfxPath & "characters\" & Classes(NewCharClass).MaleSprite(NewCharSprite) & GfxExt
            Else
                filename = Application.StartupPath & GfxPath & "characters\" & Classes(NewCharClass).FemaleSprite(NewCharSprite) & GfxExt
            End If

            Dim charsprite As Bitmap = New Bitmap(filename)

            charwidth = charsprite.Width / 4
            charheight = charsprite.Height / 4

            srcRect = New Rectangle(0, 0, charwidth, charheight)

            charsprite.MakeTransparent(charsprite.GetPixel(0, 0))

            If charwidth > 32 Then
                Lblnextcharleft = (100 - (64 - charwidth))
            Else
                Lblnextcharleft = 100
            End If
            pnlNewChar.Refresh()
            g.DrawImage(charsprite, destRect, srcRect, GraphicsUnit.Pixel)
            g.Dispose()
        End If
    End Sub

    ''' <summary>
    ''' Draw the character for the char select screen.
    ''' </summary>
    Sub DrawCharacterSelect()
        Dim g As Graphics
        Dim srcRect As Rectangle
        Dim destRect As Rectangle

        If pnlCharSelect.Visible = True Then
            Dim filename As String
            Dim charwidth As Integer, charheight As Integer

            'first
            If CharSelection(1).Sprite > 0 Then
                g = picChar1.CreateGraphics

                filename = Application.StartupPath & GfxPath & "characters\" & CharSelection(1).Sprite & GfxExt

                Dim charsprite As Bitmap = New Bitmap(filename)

                charwidth = charsprite.Width / 4
                charheight = charsprite.Height / 4

                srcRect = New Rectangle(0, 0, charwidth, charheight)
                destRect = New Rectangle(0, 0, charwidth, charheight)

                charsprite.MakeTransparent(charsprite.GetPixel(0, 0))

                picChar1.Refresh()
                g.DrawImage(charsprite, destRect, srcRect, GraphicsUnit.Pixel)

                If SelectedChar = 1 Then
                    g.DrawRectangle(Pens.Red, New Rectangle(0, 0, charwidth - 1, charheight))
                End If

                g.Dispose()
            Else
                picChar1.BorderStyle = BorderStyle.FixedSingle
                picChar1.Refresh()
            End If

            'second
            If CharSelection(2).Sprite > 0 Then
                g = picChar2.CreateGraphics

                filename = Application.StartupPath & GfxPath & "characters\" & CharSelection(2).Sprite & GfxExt

                Dim charsprite As Bitmap = New Bitmap(filename)

                charwidth = charsprite.Width / 4
                charheight = charsprite.Height / 4

                srcRect = New Rectangle(0, 0, charwidth, charheight)
                destRect = New Rectangle(0, 0, charwidth, charheight)

                charsprite.MakeTransparent(charsprite.GetPixel(0, 0))

                picChar2.Refresh()
                g.DrawImage(charsprite, destRect, srcRect, GraphicsUnit.Pixel)

                If SelectedChar = 2 Then
                    g.DrawRectangle(Pens.Red, New Rectangle(0, 0, charwidth - 1, charheight))
                End If

                g.Dispose()
            Else
                picChar2.BorderStyle = BorderStyle.FixedSingle
                picChar2.Refresh()
            End If

            'third
            If CharSelection(3).Sprite > 0 Then
                g = picChar3.CreateGraphics

                filename = Application.StartupPath & GfxPath & "characters\" & CharSelection(3).Sprite & GfxExt

                Dim charsprite As Bitmap = New Bitmap(filename)

                charwidth = charsprite.Width / 4
                charheight = charsprite.Height / 4

                srcRect = New Rectangle(0, 0, charwidth, charheight)
                destRect = New Rectangle(0, 0, charwidth, charheight)

                charsprite.MakeTransparent(charsprite.GetPixel(0, 0))

                picChar3.Refresh()
                g.DrawImage(charsprite, destRect, srcRect, GraphicsUnit.Pixel)

                If SelectedChar = 3 Then
                    g.DrawRectangle(Pens.Red, New Rectangle(0, 0, charwidth - 1, charheight))
                End If

                g.Dispose()
            Else
                picChar3.BorderStyle = BorderStyle.FixedSingle
                picChar3.Refresh()
            End If

        End If
    End Sub

    ''' <summary>
    ''' Stop the NewChar panel from repainting itself.
    ''' </summary>
    Private Sub PnlNewChar_Paint(sender As Object, e As PaintEventArgs) Handles pnlNewChar.Paint
        'nada here
    End Sub
#End Region

#Region "Credits"
    ''' <summary>
    ''' This timer handles the scrolling credits.
    ''' </summary>
    Private Sub TmrCredits_Tick(sender As Object, e As EventArgs) Handles tmrCredits.Tick
        Dim credits As String
        Dim filepath As String
        filepath = Application.StartupPath & "\Data\credits.txt"
        lblScrollingCredits.Top = 177
        If PnlCreditsVisible = True Then
            tmrCredits.Enabled = False
            credits = GetFileContents(filepath)
            lblScrollingCredits.Text = "" & vbNewLine & credits
            Do Until PnlCreditsVisible = False
                lblScrollingCredits.Top = lblScrollingCredits.Top - 1
                If lblScrollingCredits.Bottom <= lblCreditsTop.Bottom Then
                    lblScrollingCredits.Top = 177
                End If
                Application.DoEvents()
                Threading.Thread.Sleep(100)
            Loop
        End If
    End Sub
#End Region

#Region "Login"
    ''' <summary>
    ''' Handles press enter on login name txtbox.
    ''' </summary>
    Private Sub TxtLogin_KeyDown(sender As Object, e As KeyEventArgs) Handles txtLogin.KeyDown
        If e.KeyCode = Keys.Enter Then
            BtnLogin_Click(Me, Nothing)
        End If
    End Sub
    ''' <summary>
    ''' Handles press enter on login password txtbox.
    ''' </summary>
    Private Sub TxtPassword_KeyDown(sender As Object, e As KeyEventArgs) Handles txtPassword.KeyDown
        If e.KeyCode = Keys.Enter Then
            BtnLogin_Click(Me, Nothing)
        End If
    End Sub

#End Region

#Region "Char Creation"
    ''' <summary>
    ''' Changes selected class.
    ''' </summary>
    Private Sub CmbClass_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbClass.SelectedIndexChanged
        NewCharClass = cmbClass.SelectedIndex + 1
        'Print(NewCharClass)
        'txtDescription.Text = Classes(NewCharClass).Desc
        'DrawCharacter()
    End Sub

    ''' <summary>
    ''' Switches to male gender.
    ''' </summary>
    Private Sub RdoMale_CheckedChanged(sender As Object, e As EventArgs) Handles rdoMale.CheckedChanged
        DrawCharacter()
    End Sub

    ''' <summary>
    ''' Switches to female gender.
    ''' </summary>
    Private Sub RdoFemale_CheckedChanged(sender As Object, e As EventArgs) Handles rdoFemale.CheckedChanged
        DrawCharacter()
    End Sub

    ''' <summary>
    ''' Switches sprite for selected class to next one, if any.
    ''' </summary>
    Private Sub LblNextChar_Click(sender As Object, e As EventArgs)
        NewCharSprite = NewCharSprite + 1
        If rdoMale.Checked = True Then
            If NewCharSprite > Classes(NewCharClass).MaleSprite.Length - 1 Then NewCharSprite = 1
        ElseIf rdoFemale.Checked = True Then
            If NewCharSprite > Classes(NewCharClass).FemaleSprite.Length - 1 Then NewCharSprite = 1
        End If
        DrawCharacter()
    End Sub

    ''' <summary>
    ''' Switches sprite for selected class to previous one, if any.
    ''' </summary>
    Private Sub LblPrevChar_Click(sender As Object, e As EventArgs)
        NewCharSprite = NewCharSprite - 1
        If rdoMale.Checked = True Then
            If NewCharSprite = 0 Then NewCharSprite = Classes(NewCharClass).MaleSprite.Length - 1
        ElseIf rdoFemale.Checked = True Then
            If NewCharSprite = 0 Then NewCharSprite = Classes(NewCharClass).FemaleSprite.Length - 1
        End If
        DrawCharacter()
    End Sub

    ''' <summary>
    ''' Initial drawing of new char.
    ''' </summary>
    Private Sub PnlNewChar_VisibleChanged(sender As Object, e As EventArgs) Handles pnlNewChar.VisibleChanged
        DrawCharacter()
    End Sub
#End Region

#Region "Buttons"
    ''' <summary>
    ''' Handle Play button press.
    ''' </summary>
    Private Sub BtnPlay_Click(sender As Object, e As EventArgs) Handles btnPlay.Click
        If Socket.IsConnected() = True Then
            PlaySound("Click.ogg")
            PnlRegisterVisible = False
            PnlLoginVisible = True
            PnlCharCreateVisible = False
            PnlCreditsVisible = False
            pnlIPConfig.Visible = False
            txtboxLogin.Focus()
            If Options.SavePass = True Then
                txtboxLogin.Text = Options.Username
                txtboxPassword.Text = Options.Password
            End If
        End If
    End Sub

    ''' <summary>
    ''' Changes to hover state on button.
    ''' </summary>
    Private Sub BtnPlay_MouseEnter(sender As Object, e As EventArgs) Handles btnPlay.MouseEnter
        btnPlay.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button_hover" & GfxExt)
    End Sub

    ''' <summary>
    ''' Changes to normal state on button.
    ''' </summary>
    Private Sub BtnPlay_MouseLeave(sender As Object, e As EventArgs) Handles btnPlay.MouseLeave
        btnPlay.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
    End Sub

    ''' <summary>
    ''' Handle Register button press.
    ''' </summary>
    Private Sub BtnRegister_Click(sender As Object, e As EventArgs) Handles btnRegister.Click

        PlaySound("Click.ogg")
        PnlRegisterVisible = True
        PnlLoginVisible = False
        PnlCharCreateVisible = False
        PnlCreditsVisible = False
        pnlIPConfig.Visible = False

    End Sub

    ''' <summary>
    ''' Changes to hover state on button.
    ''' </summary>
    Private Sub BtnRegister_MouseEnter(sender As Object, e As EventArgs) Handles btnRegister.MouseEnter
        btnRegister.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button_hover" & GfxExt)
    End Sub

    ''' <summary>
    ''' Changes to normal state on button.
    ''' </summary>
    Private Sub BtnRegister_MouseLeave(sender As Object, e As EventArgs) Handles btnRegister.MouseLeave
        btnRegister.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
    End Sub

    ''' <summary>
    ''' Handle Credits button press.
    ''' </summary>
    Private Sub BtnCredits_Click(sender As Object, e As EventArgs) Handles btnCredits.Click
        PlaySound("Click.ogg")
        If PnlCreditsVisible = False Then
            tmrCredits.Enabled = True
        End If
        PnlCreditsVisible = True
        PnlLoginVisible = False
        PnlRegisterVisible = False
        PnlCharCreateVisible = False
        pnlIPConfig.Visible = False
    End Sub

    ''' <summary>
    ''' Changes to hover state on button.
    ''' </summary>
    Private Sub BtnCredits_MouseEnter(sender As Object, e As EventArgs) Handles btnCredits.MouseEnter
        btnCredits.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button_hover" & GfxExt)
    End Sub

    ''' <summary>
    ''' Changes to normal state on button.
    ''' </summary>
    Private Sub BtnCredits_MouseLeave(sender As Object, e As EventArgs) Handles btnCredits.MouseLeave
        btnCredits.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
    End Sub

    ''' <summary>
    ''' Handles Exit button press.
    ''' </summary>
    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        PlaySound("Click.ogg")
        DestroyGame()
    End Sub

    ''' <summary>
    ''' Changes to hover state on button.
    ''' </summary>
    Private Sub BtnExit_MouseEnter(sender As Object, e As EventArgs) Handles btnExit.MouseEnter
        btnExit.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button_hover" & GfxExt)
    End Sub

    ''' <summary>
    ''' Changes to normal state on button.
    ''' </summary>
    Private Sub BtnExit_MouseLeave(sender As Object, e As EventArgs) Handles btnExit.MouseLeave
        btnExit.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
    End Sub

    ''' <summary>
    ''' Handles Login button press.
    ''' </summary>
    Private Sub BtnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        If IsLoginLegal(txtboxLogin.Text, txtboxPassword.Text) Then
            MenuState(MenuStateLogin)
        End If
    End Sub

    ''' <summary>
    ''' Changes to hover state on button.
    ''' </summary>
    Private Sub BtnLogin_MouseEnter(sender As Object, e As EventArgs) Handles btnLogin.MouseEnter
        btnLogin.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button_hover" & GfxExt)
    End Sub

    ''' <summary>
    ''' Changes to normal state on button.
    ''' </summary>
    Private Sub BtnLogin_MouseLeave(sender As Object, e As EventArgs) Handles btnLogin.MouseLeave
        btnLogin.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
    End Sub

    ''' <summary>
    ''' Handles CreateAccount button press.
    ''' </summary>
    Private Sub BtnCreateAccount_Click(sender As Object, e As EventArgs) Handles btnCreateAccount.Click
        Dim name As String
        Dim password As String
        Dim passwordAgain As String
        name = Trim$(txtRuser.Text)
        password = Trim$(txtRPass.Text)
        passwordAgain = Trim$(txtRPass2.Text)

        If IsLoginLegal(name, password) Then
            If password <> passwordAgain Then
                MsgBox("Passwords don't match.")
                Exit Sub
            End If

            If Not IsStringLegal(name) Then Exit Sub

            MenuState(MenuStateNewaccount)
        End If
    End Sub

    ''' <summary>
    ''' Changes to hover state on button.
    ''' </summary>
    Private Sub BtnCreateAccount_MouseEnter(sender As Object, e As EventArgs) Handles btnCreateAccount.MouseEnter
        btnCreateAccount.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button_hover" & GfxExt)
    End Sub

    ''' <summary>
    ''' Changes to normal state on button.
    ''' </summary>
    Private Sub BtnCreateAccount_MouseLeave(sender As Object, e As EventArgs) Handles btnCreateAccount.MouseLeave
        btnCreateAccount.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
    End Sub

    ''' <summary>
    ''' Handles CreateCharacter button press.
    ''' </summary>
    Private Sub BtnCreateCharacter_Click(sender As Object, e As EventArgs) Handles btnCreateCharacter.Click
        MenuState(MenuStateAddchar)
    End Sub

    ''' <summary>
    ''' Changes to hover state on button.
    ''' </summary>
    Private Sub BtnCreateCharacter_MouseEnter(sender As Object, e As EventArgs) Handles btnCreateCharacter.MouseEnter
        btnCreateCharacter.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button_hover" & GfxExt)
    End Sub

    ''' <summary>
    ''' Changes to normal state on button.
    ''' </summary>
    Private Sub BtnCreateCharacter_MouseLeave(sender As Object, e As EventArgs) Handles btnCreateCharacter.MouseLeave
        btnCreateCharacter.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\button" & GfxExt)
    End Sub

    ''' <summary>
    ''' Handles SaveIP button press.
    ''' </summary>
    Private Sub BtnSaveIP_Click(sender As Object, e As EventArgs) Handles btnSaveIP.Click
        Options.Ip = txtIP.Text
        Options.Port = txtPort.Text

        pnlIPConfig.Visible = False
        SaveOptions()
    End Sub

    ''' <summary>
    ''' Handles selecting character 1.
    ''' </summary>
    Private Sub PicChar1_Click(sender As Object, e As EventArgs) Handles picChar1.Click
        SelectedChar = 1
        DrawCharacterSelect()
    End Sub

    ''' <summary>
    ''' Handles selecting character 2.
    ''' </summary>
    Private Sub PicChar2_Click(sender As Object, e As EventArgs) Handles picChar2.Click
        SelectedChar = 2
        DrawCharacterSelect()
    End Sub

    ''' <summary>
    ''' Handles selecting character 3.
    ''' </summary>
    Private Sub PicChar3_Click(sender As Object, e As EventArgs) Handles picChar3.Click
        SelectedChar = 3
        DrawCharacterSelect()
    End Sub

    ''' <summary>
    ''' Handles NewChar button press.
    ''' </summary>
    Private Sub BtnNewChar_Click(sender As Object, e As EventArgs) Handles btnNewChar.Click
        Dim i As Integer, newSelectedChar As Byte

        newSelectedChar = 0

        For i = 1 To MaxChars
            If CharSelection(i).Name = "" Then
                newSelectedChar = i
                Exit For
            End If
        Next

        If newSelectedChar > 0 Then
            SelectedChar = newSelectedChar
        End If

        PnlCharCreateVisible = True
        PnlCharSelectVisible = False
        DrawChar = True
    End Sub

    ''' <summary>
    ''' Handles UseChar button press.
    ''' </summary>
    Private Sub BtnUseChar_Click(sender As Object, e As EventArgs) Handles btnUseChar.Click
        Pnlloadvisible = True
        Frmmenuvisible = False

        Dim buffer As ByteStream
        buffer = New ByteStream(8)
        buffer.WriteInt32(ClientPackets.CUseChar)
        buffer.WriteInt32(SelectedChar)
        Socket.SendData(buffer.Data, buffer.Head)

        buffer.Dispose()
    End Sub

    ''' <summary>
    ''' Handles DelChar button press.
    ''' </summary>
    Private Sub BtnDelChar_Click(sender As Object, e As EventArgs) Handles btnDelChar.Click
        Dim result1 As DialogResult = MessageBox.Show("Sure you want to delete character " & SelectedChar & "?", "You sure?", MessageBoxButtons.YesNo)
        If result1 = DialogResult.Yes Then
            Dim buffer As New ByteStream(4)
            buffer.WriteInt32(ClientPackets.CDelChar)
            buffer.WriteInt32(SelectedChar)
            Socket.SendData(buffer.Data, buffer.Head)
            buffer.Dispose()
        End If
    End Sub

    Private Sub picLogo_Click(sender As Object, e As EventArgs) Handles picLogo.Click

    End Sub

    Private Sub lblLoginPass_Click(sender As Object, e As EventArgs) Handles lblLoginPass.Click

    End Sub

    Private Sub txtLogin_TextChanged(sender As Object, e As EventArgs) Handles txtLogin.TextChanged

    End Sub

    Private Sub txtboxPassword_Click(sender As Object, e As EventArgs) Handles txtboxPassword.Click
        txtboxPassword.Focus()
        txtboxPassword.Select()
    End Sub

    Private Sub txtboxLogin_Click(sender As Object, e As EventArgs) Handles txtboxLogin.Click
        txtboxLogin.Focus()
        txtboxLogin.Select()
    End Sub

    Private Sub lblLoginName_Click(sender As Object, e As EventArgs) Handles lblLoginName.Click

    End Sub

    Private Sub txtPassword_TextChanged(sender As Object, e As EventArgs) Handles txtPassword.TextChanged

    End Sub

    Private Sub txtboxPassword_TextChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub txtboxLogin_TextChanged(sender As Object, e As EventArgs) Handles txtboxLogin.TextChanged

    End Sub

    Private Sub ClassSelect01_Click(sender As Object, e As EventArgs) Handles ClassSelect01.Click
        cmbClass.SelectedItem = cmbClass.Items(0)
        pnlNewChar.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\class_mage" & GfxExt)
    End Sub

    Private Sub ClassSelect02_Click(sender As Object, e As EventArgs) Handles ClassSelect02.Click
        cmbClass.SelectedItem = cmbClass.Items(1)
        pnlNewChar.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\class_sold" & GfxExt)
    End Sub

    Private Sub ClassSelect03_Click(sender As Object, e As EventArgs) Handles ClassSelect03.Click
        cmbClass.SelectedItem = cmbClass.Items(2)
        pnlNewChar.BackgroundImage = Image.FromFile(Application.StartupPath & GfxGuiPath & "Menu\class_burg" & GfxExt)
    End Sub

#End Region

End Class