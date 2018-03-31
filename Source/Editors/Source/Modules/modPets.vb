﻿Imports ASFW

Module modPets
#Region "Globals etc"
    Friend Pet(MAX_PETS) As PetRec
    Friend Const EDITOR_PET As Byte = 7
    Friend Pet_Changed() As Boolean

    Friend Const PetHpBarWidth As Integer = 129
    Friend Const PetMpBarWidth As Integer = 129

    Friend PetSkillBuffer As Integer
    Friend PetSkillBufferTimer As Integer
    Friend PetSkillCD() As Integer

    Friend InitPetEditor As Boolean

    'Pet Constants
    Friend Const PET_BEHAVIOUR_FOLLOW As Byte = 0 'The pet will attack all npcs around
    Friend Const PET_BEHAVIOUR_GOTO As Byte = 1 'If attacked, the pet will fight back
    Friend Const PET_ATTACK_BEHAVIOUR_ATTACKONSIGHT As Byte = 2 'The pet will attack all npcs around
    Friend Const PET_ATTACK_BEHAVIOUR_GUARD As Byte = 3 'If attacked, the pet will fight back
    Friend Const PET_ATTACK_BEHAVIOUR_DONOTHING As Byte = 4 'The pet will not attack even if attacked

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
        Dim MaxMP As Integer
        Dim Alive As Byte
        Dim AttackBehaviour As Integer
        Dim Exp As Integer
        Dim TNL As Integer

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

#Region "Outgoing Packets"
    Sub SendRequestPets()
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ClientPackets.CRequestPets)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Friend Sub SendRequestEditPet()
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.CRequestEditPet)

        Socket.SendData(Buffer.Data, Buffer.Head)

        Buffer.Dispose()

    End Sub

    Friend Sub SendSavePet(petNum As Integer)
        dim buffer as ByteStream
        Dim i As Integer

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(EditorPackets.CSavePet)
        Buffer.WriteInt32(petNum)

        With Pet(petNum)
            Buffer.WriteInt32(.Num)
            Buffer.WriteString(Trim$(.Name))
            Buffer.WriteInt32(.Sprite)
            Buffer.WriteInt32(.Range)
            Buffer.WriteInt32(.Level)
            Buffer.WriteInt32(.MaxLevel)
            Buffer.WriteInt32(.ExpGain)
            Buffer.WriteInt32(.LevelPnts)
            Buffer.WriteInt32(.StatType)
            Buffer.WriteInt32(.LevelingType)

            For i = 1 To StatType.Count - 1
                Buffer.WriteInt32(.Stat(i))
            Next

            For i = 1 To 4
                Buffer.WriteInt32(.Skill(i))
            Next

            Buffer.WriteInt32(.Evolvable)
            Buffer.WriteInt32(.EvolveLevel)
            Buffer.WriteInt32(.EvolveNum)
        End With

        Socket.SendData(Buffer.Data, Buffer.Head)

        Buffer.Dispose()

    End Sub
#End Region

#Region "Incoming Packets"
    Friend Sub Packet_PetEditor(ByRef data() As Byte)
        InitPetEditor = True
    End Sub

    Friend Sub Packet_UpdatePet(ByRef data() As Byte)
        Dim n As Integer, i As Integer
        dim buffer as New ByteStream(Data)
        n = Buffer.ReadInt32

        ReDim Pet(n).Stat(StatType.Count - 1)
        ReDim Pet(n).Skill(4)

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

#End Region

#Region "DataBase"
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

#Region "Editor"
    Friend Sub PetEditorInit()
        Dim i As Integer

        If frmPet.Visible = False Then Exit Sub
        EditorIndex = frmPet.lstIndex.SelectedIndex + 1

        With frmPet
            'populate skill combo's
            .cmbSkill1.Items.Clear()
            .cmbSkill2.Items.Clear()
            .cmbSkill3.Items.Clear()
            .cmbSkill4.Items.Clear()

            .cmbSkill1.Items.Add("None")
            .cmbSkill2.Items.Add("None")
            .cmbSkill3.Items.Add("None")
            .cmbSkill4.Items.Add("None")

            For i = 1 To MAX_SKILLS
                .cmbSkill1.Items.Add(i & ": " & Skill(i).Name)
                .cmbSkill2.Items.Add(i & ": " & Skill(i).Name)
                .cmbSkill3.Items.Add(i & ": " & Skill(i).Name)
                .cmbSkill4.Items.Add(i & ": " & Skill(i).Name)
            Next
            .txtName.Text = Trim$(Pet(EditorIndex).Name)
            If Pet(EditorIndex).Sprite < 0 OrElse Pet(EditorIndex).Sprite > .nudSprite.Maximum Then Pet(EditorIndex).Sprite = 0

            .nudSprite.Value = Pet(EditorIndex).Sprite
            .EditorPet_DrawPet()

            .nudRange.Value = Pet(EditorIndex).Range

            .nudStrength.Value = Pet(EditorIndex).Stat(StatType.Strength)
            .nudEndurance.Value = Pet(EditorIndex).Stat(StatType.Endurance)
            .nudVitality.Value = Pet(EditorIndex).Stat(StatType.Vitality)
            .nudLuck.Value = Pet(EditorIndex).Stat(StatType.Luck)
            .nudIntelligence.Value = Pet(EditorIndex).Stat(StatType.Intelligence)
            .nudSpirit.Value = Pet(EditorIndex).Stat(StatType.Spirit)
            .nudLevel.Value = Pet(EditorIndex).Level

            If Pet(EditorIndex).StatType = 1 Then
                .optCustomStats.Checked = True
                .pnlCustomStats.Visible = True
            Else
                .optAdoptStats.Checked = True
                .pnlCustomStats.Visible = False
            End If

            .nudPetExp.Value = Pet(EditorIndex).ExpGain

            .nudPetPnts.Value = Pet(EditorIndex).LevelPnts

            .nudMaxLevel.Value = Pet(EditorIndex).MaxLevel

            'Set skills
            .cmbSkill1.SelectedIndex = Pet(EditorIndex).Skill(1)

            .cmbSkill2.SelectedIndex = Pet(EditorIndex).Skill(2)

            .cmbSkill3.SelectedIndex = Pet(EditorIndex).Skill(3)

            .cmbSkill4.SelectedIndex = Pet(EditorIndex).Skill(4)

            If Pet(EditorIndex).LevelingType = 1 Then
                .optLevel.Checked = True

                .pnlPetlevel.Visible = True
                .pnlPetlevel.BringToFront()
                .nudPetExp.Value = Pet(EditorIndex).ExpGain
                If Pet(EditorIndex).MaxLevel > 0 Then .nudMaxLevel.Value = Pet(EditorIndex).MaxLevel
                .nudPetPnts.Value = Pet(EditorIndex).LevelPnts
            Else
                .optDoNotLevel.Checked = True

                .pnlPetlevel.Visible = False
                .nudPetExp.Value = Pet(EditorIndex).ExpGain
                .nudMaxLevel.Value = Pet(EditorIndex).MaxLevel
                .nudPetPnts.Value = Pet(EditorIndex).LevelPnts
            End If

            If Pet(EditorIndex).Evolvable = 1 Then
                .chkEvolve.Checked = True
            Else
                .chkEvolve.Checked = False
            End If

            .nudEvolveLvl.Value = Pet(EditorIndex).EvolveLevel
            .cmbEvolve.SelectedIndex = Pet(EditorIndex).EvolveNum
        End With

        ClearChanged_Pet()

        Pet_Changed(EditorIndex) = True

    End Sub

    Friend Sub PetEditorOk()
        Dim i As Integer

        For i = 1 To MAX_PETS
            If Pet_Changed(i) Then
                SendSavePet(i)
            End If
        Next

        frmPet.Dispose()

        Editor = 0
        ClearChanged_Pet()

    End Sub

    Friend Sub PetEditorCancel()

        Editor = 0

        frmPet.Dispose()

        ClearChanged_Pet()
        ClearPets()
        SendRequestPets()

    End Sub

    Friend Sub ClearChanged_Pet()

        ReDim Pet_Changed(MAX_PETS)

    End Sub
#End Region



End Module