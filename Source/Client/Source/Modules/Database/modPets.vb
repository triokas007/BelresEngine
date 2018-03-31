Imports System.Drawing
Imports ASFW

Module ModPets
#Region "Globals etc"
    Friend Pet() As PetRec

    Friend Const PetbarTop As Byte = 2
    Friend Const PetbarLeft As Byte = 2
    Friend Const PetbarOffsetX As Byte = 4
    Friend Const MaxPetbar As Byte = 7
    Friend Const PetHpBarWidth As Integer = 129
    Friend Const PetMpBarWidth As Integer = 129

    Friend PetSkillBuffer As Integer
    Friend PetSkillBufferTimer As Integer
    Friend PetSkillCd() As Integer

    Friend ShowPetStats As Boolean

    'Pet Constants
    Friend Const PetBehaviourFollow As Byte = 0 'The pet will attack all npcs around
    Friend Const PetBehaviourGoto As Byte = 1 'If attacked, the pet will fight back
    Friend Const PetAttackBehaviourAttackonsight As Byte = 2 'The pet will attack all npcs around
    Friend Const PetAttackBehaviourGuard As Byte = 3 'If attacked, the pet will fight back
    Friend Const PetAttackBehaviourDonothing As Byte = 4 'The pet will not attack even if attacked

    Friend Structure PetRec
        Dim Num As Integer
        Dim Name As String
        Dim Sprite As Integer

        Dim Range As Integer

        Dim Level As Integer

        Dim MaxLevel As Integer
        Dim ExpGain As Integer
        Dim LevelPnts As Integer

        Dim StatType As Byte '1 for set stats, 2 for relation to owner's stats
        Dim LevelingType As Byte '0 for leveling on own, 1 for not leveling

        Dim Stat() As Byte

        Dim Skill() As Integer

        Dim Evolvable As Byte
        Dim EvolveLevel As Integer
        Dim EvolveNum As Integer
    End Structure

    Friend Structure PlayerPetRec
        Dim Num As Integer
        Dim Health As Integer
        Dim Mana As Integer
        Dim Level As Integer
        Dim Stat() As Byte
        Dim Skill() As Integer
        Dim Points As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim Dir As Integer
        Dim MaxHp As Integer
        Dim MaxMp As Integer
        Dim Alive As Byte
        Dim AttackBehaviour As Integer
        Dim Exp As Integer
        Dim Tnl As Integer

        'Client Use Only
        Dim XOffset As Integer
        Dim YOffset As Integer
        Dim Moving As Byte
        Dim Attacking As Byte
        Dim AttackTimer As Integer
        Dim Steps As Byte
        Dim Damage As Integer
    End Structure
#End Region

#Region "Database"
    Sub ClearPet(index as integer)

        Pet(Index).Name = ""

        ReDim Pet(Index).Stat(StatType.Count - 1)
        ReDim Pet(Index).Skill(4)
    End Sub

    Sub ClearPets()
        Dim i As Integer

        ReDim Pet(MAX_PETS)
        ReDim PetSkillCD(4)

        For i = 1 To MAX_PETS
            ClearPet(i)
        Next

    End Sub
#End Region

#Region "Outgoing Packets"
    Friend Sub SendPetBehaviour(index as integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CSetBehaviour)

        Buffer.WriteInt32(Index)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Sub SendTrainPetStat(statNum As Byte)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CPetUseStatPoint)

        Buffer.WriteInt32(StatNum)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Sub SendRequestPets()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestPets)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Sub SendUsePetSkill(skill As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CPetSkill)
        Buffer.WriteInt32(skill)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

        PetSkillBuffer = skill
        PetSkillBufferTimer = GetTickCount()
    End Sub

    Sub SendSummonPet()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CSummonPet)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Sub SendReleasePet()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CReleasePet)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub
#End Region

#Region "Incoming Packets"
    Friend Sub Packet_UpdatePlayerPet(ByRef data() As Byte)
        Dim n As Integer, i As Integer
        dim buffer as New ByteStream(Data)
        n = Buffer.ReadInt32

        'pet
        Player(n).Pet.Num = Buffer.ReadInt32
        Player(n).Pet.Health = Buffer.ReadInt32
        Player(n).Pet.Mana = Buffer.ReadInt32
        Player(n).Pet.Level = Buffer.ReadInt32

        For i = 1 To StatType.Count - 1
            Player(n).Pet.Stat(i) = Buffer.ReadInt32
        Next

        For i = 1 To 4
            Player(n).Pet.Skill(i) = Buffer.ReadInt32
        Next

        Player(n).Pet.X = Buffer.ReadInt32
        Player(n).Pet.Y = Buffer.ReadInt32
        Player(n).Pet.Dir = Buffer.ReadInt32

        Player(n).Pet.MaxHp = Buffer.ReadInt32
        Player(n).Pet.MaxMP = Buffer.ReadInt32

        Player(n).Pet.Alive = Buffer.ReadInt32

        Player(n).Pet.AttackBehaviour = Buffer.ReadInt32
        Player(n).Pet.Points = Buffer.ReadInt32
        Player(n).Pet.Exp = Buffer.ReadInt32
        Player(n).Pet.TNL = Buffer.ReadInt32

        Buffer.Dispose()
    End Sub

    Friend Sub Packet_UpdatePet(ByRef data() As Byte)
        Dim n As Integer, i As Integer
        dim buffer as New ByteStream(Data)
        n = Buffer.ReadInt32

        With Pet(n)
            .Num = Buffer.ReadInt32
            .Name = Buffer.ReadString
            .Sprite = Buffer.ReadInt32
            .Range = Buffer.ReadInt32
            .Level = Buffer.ReadInt32
            .MaxLevel = Buffer.ReadInt32
            .ExpGain = Buffer.ReadInt32
            .LevelPnts = Buffer.ReadInt32
            .StatType = Buffer.ReadInt32
            .LevelingType = Buffer.ReadInt32
            For i = 1 To StatType.Count - 1
                .Stat(i) = Buffer.ReadInt32
            Next

            For i = 1 To 4
                .Skill(i) = Buffer.ReadInt32
            Next

            .Evolvable = Buffer.ReadInt32
            .EvolveLevel = Buffer.ReadInt32
            .EvolveNum = Buffer.ReadInt32
        End With

        Buffer.Dispose()

    End Sub

    Friend Sub Packet_PetMove(ByRef data() As Byte)
        Dim i As Integer, x As Integer, y As Integer
        Dim dir As Integer, movement As Integer
        dim buffer as New ByteStream(Data)
        i = Buffer.ReadInt32
        X = Buffer.ReadInt32
        Y = Buffer.ReadInt32
        dir = Buffer.ReadInt32
        Movement = Buffer.ReadInt32

        With Player(i).Pet
            .X = X
            .Y = Y
            .Dir = dir
            .XOffset = 0
            .YOffset = 0
            .Moving = Movement

            Select Case .Dir
                Case DirectionType.Up
                    .YOffset = PicY
                Case DirectionType.Down
                    .YOffset = PicY * -1
                Case DirectionType.Left
                    .XOffset = PicX
                Case DirectionType.Right
                    .XOffset = PicX * -1
            End Select
        End With

        Buffer.Dispose()
    End Sub

    Friend Sub Packet_PetDir(ByRef data() As Byte)
        Dim i As Integer
        Dim dir As Integer
        dim buffer as New ByteStream(Data)
        i = Buffer.ReadInt32
        dir = Buffer.ReadInt32

        Player(i).Pet.Dir = dir

        Buffer.Dispose()
    End Sub

    Friend Sub Packet_PetVital(ByRef data() As Byte)
        Dim i As Integer
        dim buffer as New ByteStream(Data)
        i = Buffer.ReadInt32

        If Buffer.ReadInt32 = 1 Then
            Player(i).Pet.MaxHp = Buffer.ReadInt32
            Player(i).Pet.Health = Buffer.ReadInt32
        Else
            Player(i).Pet.MaxMP = Buffer.ReadInt32
            Player(i).Pet.Mana = Buffer.ReadInt32
        End If

        Buffer.Dispose()

    End Sub

    Friend Sub Packet_ClearPetSkillBuffer(ByRef data() As Byte)
        PetSkillBuffer = 0
        PetSkillBufferTimer = 0
    End Sub

    Friend Sub Packet_PetAttack(ByRef data() As Byte)
        Dim i As Integer
        dim buffer as New ByteStream(Data)
        i = Buffer.ReadInt32

        ' Set pet to attacking
        Player(i).Pet.Attacking = 1
        Player(i).Pet.AttackTimer = GetTickCount()

        Buffer.Dispose()
    End Sub

    Friend Sub Packet_PetXY(ByRef data() As Byte)
        Dim i As Integer
        dim buffer as New ByteStream(Data)
        Player(i).Pet.X = Buffer.ReadInt32
        Player(i).Pet.Y = Buffer.ReadInt32

        Buffer.Dispose()
    End Sub

    Friend Sub Packet_PetExperience(ByRef data() As Byte)
        dim buffer as New ByteStream(Data)
        Player(MyIndex).Pet.Exp = Buffer.ReadInt32
        Player(MyIndex).Pet.TNL = Buffer.ReadInt32

        Buffer.Dispose()
    End Sub
#End Region

#Region "Movement"
    Sub ProcessPetMovement(index as integer)

        ' Check if pet is walking, and if so process moving them over

        If Player(Index).Pet.Moving = MovementType.Walking Then

            Select Case Player(Index).Pet.Dir
                Case DirectionType.Up
                    Player(Index).Pet.YOffset = Player(Index).Pet.YOffset - ((ElapsedTime / 1000) * (WalkSpeed * SizeX))
                    If Player(Index).Pet.YOffset < 0 Then Player(Index).Pet.YOffset = 0

                Case DirectionType.Down
                    Player(Index).Pet.YOffset = Player(Index).Pet.YOffset + ((ElapsedTime / 1000) * (WalkSpeed * SizeX))
                    If Player(Index).Pet.YOffset > 0 Then Player(Index).Pet.YOffset = 0

                Case DirectionType.Left
                    Player(Index).Pet.XOffset = Player(Index).Pet.XOffset - ((ElapsedTime / 1000) * (WalkSpeed * SizeX))
                    If Player(Index).Pet.XOffset < 0 Then Player(Index).Pet.XOffset = 0

                Case DirectionType.Right
                    Player(Index).Pet.XOffset = Player(Index).Pet.XOffset + ((ElapsedTime / 1000) * (WalkSpeed * SizeX))
                    If Player(Index).Pet.XOffset > 0 Then Player(Index).Pet.XOffset = 0

            End Select

            ' Check if completed walking over to the next tile
            If Player(Index).Pet.Moving > 0 Then
                If Player(Index).Pet.Dir = DirectionType.Right OrElse Player(Index).Pet.Dir = DirectionType.Down Then
                    If (Player(Index).Pet.XOffset >= 0) AndAlso (Player(Index).Pet.YOffset >= 0) Then
                        Player(Index).Pet.Moving = 0
                        If Player(Index).Pet.Steps = 1 Then
                            Player(Index).Pet.Steps = 2
                        Else
                            Player(Index).Pet.Steps = 1
                        End If
                    End If
                Else
                    If (Player(Index).Pet.XOffset <= 0) AndAlso (Player(Index).Pet.YOffset <= 0) Then
                        Player(Index).Pet.Moving = 0
                        If Player(Index).Pet.Steps = 1 Then
                            Player(Index).Pet.Steps = 2
                        Else
                            Player(Index).Pet.Steps = 1
                        End If
                    End If
                End If
            End If
        End If

    End Sub

    Friend Sub PetMove(x As Integer, y As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CPetMove)

        Buffer.WriteInt32(X)
        Buffer.WriteInt32(Y)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

#End Region

#Region "Drawing"
    Friend Sub DrawPet(index as integer)
        Dim anim As Byte, x As Integer, y As Integer
        Dim sprite As Integer, spriteleft As Integer
        Dim srcrec As Rectangle
        Dim attackspeed As Integer

        Sprite = Pet(Player(Index).Pet.Num).Sprite

        If Sprite < 1 OrElse Sprite > NumCharacters Then Exit Sub

        attackspeed = 1000

        ' Reset frame
        If Player(Index).Pet.Steps = 3 Then
            Anim = 0
        ElseIf Player(Index).Pet.Steps = 1 Then
            Anim = 2
        ElseIf Player(Index).Pet.Steps = 2 Then
            Anim = 3
        End If

        ' Check for attacking animation
        If Player(Index).Pet.AttackTimer + (attackspeed / 2) > GetTickCount() Then
            If Player(Index).Pet.Attacking = 1 Then
                Anim = 3
            End If
        Else
            ' If not attacking, walk normally
            Select Case Player(Index).Pet.Dir
                Case DirectionType.Up
                    If (Player(Index).Pet.YOffset > 8) Then Anim = Player(Index).Pet.Steps
                Case DirectionType.Down
                    If (Player(Index).Pet.YOffset < -8) Then Anim = Player(Index).Pet.Steps
                Case DirectionType.Left
                    If (Player(Index).Pet.XOffset > 8) Then Anim = Player(Index).Pet.Steps
                Case DirectionType.Right
                    If (Player(Index).Pet.XOffset < -8) Then Anim = Player(Index).Pet.Steps
            End Select
        End If

        ' Check to see if we want to stop making him attack
        With Player(Index).Pet
            If .AttackTimer + attackspeed < GetTickCount() Then
                .Attacking = 0
                .AttackTimer = 0
            End If
        End With

        ' Set the left
        Select Case Player(Index).Pet.Dir
            Case DirectionType.Up
                spriteleft = 3
            Case DirectionType.Right
                spriteleft = 2
            Case DirectionType.Down
                spriteleft = 0
            Case DirectionType.Left
                spriteleft = 1
        End Select

        srcrec = New Rectangle((Anim) * (CharacterGFXInfo(Sprite).Width / 4), spriteleft * (CharacterGFXInfo(Sprite).Height / 4), (CharacterGFXInfo(Sprite).Width / 4), (CharacterGFXInfo(Sprite).Height / 4))

        ' Calculate the X
        X = Player(Index).Pet.X * PicX + Player(Index).Pet.XOffset - ((CharacterGFXInfo(Sprite).Width / 4 - 32) / 2)

        ' Is the player's height more than 32..?
        If (CharacterGFXInfo(Sprite).Height / 4) > 32 Then
            ' Create a 32 pixel offset for larger sprites
            Y = Player(Index).Pet.Y * PicY + Player(Index).Pet.YOffset - ((CharacterGFXInfo(Sprite).Width / 4) - 32)
        Else
            ' Proceed as normal
            Y = Player(Index).Pet.Y * PicY + Player(Index).Pet.YOffset
        End If

        ' render the actual sprite
        DrawCharacter(Sprite, X, Y, srcrec)

    End Sub

    Friend Sub DrawPlayerPetName(index as integer)
        Dim textX As Integer
        Dim textY As Integer
        Dim color As SFML.Graphics.Color, backcolor As SFML.Graphics.Color
        Dim name As String

        ' Check access level
        If GetPlayerPK(Index) = False Then

            Select Case GetPlayerAccess(Index)
                Case 0
                    color = SFML.Graphics.Color.Red
                    backcolor = SFML.Graphics.Color.Black
                Case 1
                    color = SFML.Graphics.Color.Black
                    backcolor = SFML.Graphics.Color.White
                Case 2
                    color = SFML.Graphics.Color.Cyan
                    backcolor = SFML.Graphics.Color.Black
                Case 3
                    color = SFML.Graphics.Color.Green
                    backcolor = SFML.Graphics.Color.Black
                Case 4
                    color = SFML.Graphics.Color.Yellow
                    backcolor = SFML.Graphics.Color.Black
            End Select

        Else
            color = SFML.Graphics.Color.Red
        End If

        Name = Trim$(GetPlayerName(Index)) & "'s " & Trim$(Pet(Player(Index).Pet.Num).Name)
        ' calc pos
        TextX = ConvertMapX(Player(Index).Pet.X * PicX) + Player(Index).Pet.XOffset + (PicX \ 2) - GetTextWidth(Name) / 2
        If Pet(Player(Index).Pet.Num).Sprite < 1 OrElse Pet(Player(Index).Pet.Num).Sprite > NumCharacters Then
            TextY = ConvertMapY(Player(Index).Pet.Y * PicY) + Player(Index).Pet.YOffset - 16
        Else
            ' Determine location for text
            TextY = ConvertMapY(Player(Index).Pet.Y * PicY) + Player(Index).Pet.YOffset - (CharacterGFXInfo(Pet(Player(Index).Pet.Num).Sprite).Height / 4) + 16
        End If

        ' Draw name
        DrawText(TextX, TextY, Trim$(Name), color, backcolor, GameWindow)

    End Sub

    Sub DrawPetBar()
        Dim skillnum As Integer, skillpic As Integer
        Dim rec As Rectangle, recPos As Rectangle

        If Not HasPet(MyIndex) Then Exit Sub

        If Not PetAlive(MyIndex) Then
            RenderSprite(PetBarSprite, GameWindow, PetbarX, PetbarY, 0, 0, 32, PetbarGFXInfo.Height)
        Else
            RenderSprite(PetBarSprite, GameWindow, PetbarX, PetbarY, 0, 0, PetbarGFXInfo.Width, PetbarGFXInfo.Height)

            For i = 1 To 4
                skillnum = Player(MyIndex).Pet.Skill(i)

                If skillnum > 0 Then
                    skillpic = Skill(skillnum).Icon

                    If SkillIconsGFXInfo(skillpic).IsLoaded = False Then
                        LoadTexture(skillpic, 9)
                    End If

                    'seeying we still use it, lets update timer
                    With SkillIconsGFXInfo(skillpic)
                        .TextureTimer = GetTickCount() + 100000
                    End With

                    With rec
                        .Y = 0
                        .Height = 32
                        .X = 0
                        .Width = 32
                    End With

                    If Not PetSkillCD(i) = 0 Then
                        rec.X = 32
                        rec.Width = 32
                    End If

                    With recPos
                        .Y = PetbarY + PetbarTop
                        .Height = PicY
                        .X = PetbarX + PetbarLeft + ((PetbarOffsetX - 2) + 32) * (((i - 1) + 3))
                        .Width = PicX
                    End With

                    RenderSprite(SkillIconsSprite(skillpic), GameWindow, recPos.X, recPos.Y, rec.X, rec.Y, rec.Width, rec.Height)
                End If
            Next
        End If

    End Sub

    Sub DrawPetStats()
        Dim sprite As Integer, rec As Rectangle

        If Not HasPet(MyIndex) Then Exit Sub

        If Not ShowPetStats Then Exit Sub

        'draw panel
        RenderSprite(PetStatsSprite, GameWindow, PetStatX, PetStatY, 0, 0, PetStatsGFXInfo.Width, PetStatsGFXInfo.Height)

        'lets get player sprite to render
        sprite = Pet(Player(MyIndex).Pet.Num).Sprite

        With rec
            .Y = 0
            .Height = CharacterGFXInfo(sprite).Height / 4
            .X = 0
            .Width = CharacterGFXInfo(sprite).Width / 4
        End With

        Dim petname As String = Trim$(Pet(Player(MyIndex).Pet.Num).Name)

        DrawText(PetStatX + 70, PetStatY + 10, petname & " Lvl: " & Player(MyIndex).Pet.Level, SFML.Graphics.Color.Yellow, SFML.Graphics.Color.Black, GameWindow)

        RenderSprite(CharacterSprite(sprite), GameWindow, PetStatX + 10, PetStatY + 10 + (PetStatsGFXInfo.Height / 4) - (rec.Height / 2), rec.X, rec.Y, rec.Width, rec.Height)

        'stats
        DrawText(PetStatX + 65, PetStatY + 50, "Strength: " & Player(MyIndex).Pet.Stat(StatType.Strength), SFML.Graphics.Color.Yellow, SFML.Graphics.Color.Black, GameWindow)
        DrawText(PetStatX + 65, PetStatY + 65, "Endurance: " & Player(MyIndex).Pet.Stat(StatType.Endurance), SFML.Graphics.Color.Yellow, SFML.Graphics.Color.Black, GameWindow)
        DrawText(PetStatX + 65, PetStatY + 80, "Vitality: " & Player(MyIndex).Pet.Stat(StatType.Vitality), SFML.Graphics.Color.Yellow, SFML.Graphics.Color.Black, GameWindow)

        DrawText(PetStatX + 165, PetStatY + 50, "Luck: " & Player(MyIndex).Pet.Stat(StatType.Luck), SFML.Graphics.Color.Yellow, SFML.Graphics.Color.Black, GameWindow)
        DrawText(PetStatX + 165, PetStatY + 65, "Intelligence: " & Player(MyIndex).Pet.Stat(StatType.Intelligence), SFML.Graphics.Color.Yellow, SFML.Graphics.Color.Black, GameWindow)
        DrawText(PetStatX + 165, PetStatY + 80, "Spirit: " & Player(MyIndex).Pet.Stat(StatType.Spirit), SFML.Graphics.Color.Yellow, SFML.Graphics.Color.Black, GameWindow)

        DrawText(PetStatX + 65, PetStatY + 95, "Experience: " & Player(MyIndex).Pet.Exp & "/" & Player(MyIndex).Pet.TNL, SFML.Graphics.Color.Yellow, SFML.Graphics.Color.Black, GameWindow)
    End Sub

#End Region

#Region "Misc"
    Friend Function PetAlive(index as integer) As Boolean
        PetAlive = False

        If Player(index).Pet.Alive = 1 Then
            PetAlive = True
        End If

    End Function

    Friend Function HasPet(index as integer) As Boolean
        HasPet = False

        If Player(index).Pet.Num > 0 Then
            HasPet = True
        End If
    End Function

    Friend Function IsPetBarSlot(x As Single, y As Single) As Integer
        Dim tempRec As Rect
        Dim i As Integer

        IsPetBarSlot = 0

        For i = 1 To MaxPetbar

            With tempRec
                .Top = PetbarY + PetbarTop
                .Bottom = .Top + PicY
                .Left = PetbarX + PetbarLeft + ((PetbarOffsetX + 32) * (((i - 1) Mod MaxPetbar)))
                .Right = .Left + PicX
            End With

            If X >= tempRec.Left AndAlso X <= tempRec.Right Then
                If Y >= tempRec.Top AndAlso Y <= tempRec.Bottom Then
                    IsPetBarSlot = i
                    Exit Function
                End If
            End If
        Next

    End Function
#End Region

End Module