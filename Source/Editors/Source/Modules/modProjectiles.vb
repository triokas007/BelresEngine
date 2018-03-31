﻿
Imports System.IO
Imports ASFW

Friend Module modProjectiles
#Region "Defines"
    Friend Const MAX_PROJECTILES As Integer = 255
    Friend Projectiles(MAX_PROJECTILES) As ProjectileRec
    Friend MapProjectiles(MAX_PROJECTILES) As MapProjectileRec
    Friend NumProjectiles As Integer
    Friend InitProjectileEditor As Boolean
    Friend Const EDITOR_PROJECTILE As Byte = 10
    Friend Projectile_Changed(MAX_PROJECTILES) As Boolean
#End Region

#Region "Types"
    Friend Structure ProjectileRec
        Dim Name As String
        Dim Sprite As Integer
        Dim Range As Byte
        Dim Speed As Integer
        Dim Damage As Integer
    End Structure

    Friend Structure MapProjectileRec
        Dim ProjectileNum As Integer
        Dim Owner As Integer
        Dim OwnerType As Byte
        Dim X As Integer
        Dim Y As Integer
        Dim dir As Byte
        Dim Range As Integer
        Dim TravelTime As Integer
        Dim Timer As Integer
    End Structure
#End Region

#Region "Sending"
    Sub SendRequestEditProjectiles()
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(EditorPackets.RequestEditProjectiles)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Sub SendSaveProjectile(ProjectileNum As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.SaveProjectile)
        Buffer.WriteInt32(ProjectileNum)

        Buffer.WriteString(Trim(Projectiles(ProjectileNum).Name))
        Buffer.WriteInt32(Projectiles(ProjectileNum).Sprite)
        Buffer.WriteInt32(Projectiles(ProjectileNum).Range)
        Buffer.WriteInt32(Projectiles(ProjectileNum).Speed)
        Buffer.WriteInt32(Projectiles(ProjectileNum).Damage)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Sub SendRequestProjectiles()
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ClientPackets.CRequestProjectiles)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Sub SendClearProjectile(ProjectileNum As Integer, Collisionindex as integer, CollisionType As Byte, CollisionZone As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ClientPackets.CClearProjectile)
        Buffer.WriteInt32(ProjectileNum)
        Buffer.WriteInt32(CollisionIndex)
        Buffer.WriteInt32(CollisionType)
        Buffer.WriteInt32(CollisionZone)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

#End Region

#Region "Recieving"
    Friend Sub HandleProjectileEditor(ByRef data() As Byte)

        InitProjectileEditor = True

    End Sub

    Friend Sub HandleUpdateProjectile(ByRef data() As Byte)
        Dim ProjectileNum As Integer
        dim buffer as New ByteStream(Data)
        ProjectileNum = Buffer.ReadInt32

        Projectiles(ProjectileNum).Name = Buffer.ReadString
        Projectiles(ProjectileNum).Sprite = Buffer.ReadInt32
        Projectiles(ProjectileNum).Range = Buffer.ReadInt32
        Projectiles(ProjectileNum).Speed = Buffer.ReadInt32
        Projectiles(ProjectileNum).Damage = Buffer.ReadInt32

        Buffer.Dispose()

    End Sub

    Friend Sub HandleMapProjectile(ByRef data() As Byte)
        Dim i As Integer
        dim buffer as New ByteStream(Data)
        i = Buffer.ReadInt32

        With MapProjectiles(i)
            .ProjectileNum = Buffer.ReadInt32
            .Owner = Buffer.ReadInt32
            .OwnerType = Buffer.ReadInt32
            .dir = Buffer.ReadInt32
            .X = Buffer.ReadInt32
            .Y = Buffer.ReadInt32
            .Range = 0
            .Timer = GetTickCount() + 60000
        End With

        Buffer.Dispose()

    End Sub

#End Region

#Region "Database"
    Sub ClearProjectiles()
        Dim i As Integer

        For i = 1 To MAX_PROJECTILES
            Call ClearProjectile(i)
        Next

    End Sub

    Sub ClearProjectile(index as integer)

        Projectiles(Index).Name = ""
        Projectiles(Index).Sprite = 0
        Projectiles(Index).Range = 0
        Projectiles(Index).Speed = 0
        Projectiles(Index).Damage = 0

    End Sub

    Sub ClearMapProjectile(ProjectileNum As Integer)

        MapProjectiles(ProjectileNum).ProjectileNum = 0
        MapProjectiles(ProjectileNum).Owner = 0
        MapProjectiles(ProjectileNum).OwnerType = 0
        MapProjectiles(ProjectileNum).X = 0
        MapProjectiles(ProjectileNum).Y = 0
        MapProjectiles(ProjectileNum).dir = 0
        MapProjectiles(ProjectileNum).Timer = 0

    End Sub

#End Region

#Region "Drawing"

    Friend Sub CheckProjectiles()
        Dim i As Integer

        i = 1

        While File.Exists(Application.StartupPath & GFX_PATH & "projectiles\" & i & GFX_EXT)

            NumProjectiles = NumProjectiles + 1
            i = i + 1
        End While

        If NumProjectiles = 0 Then Exit Sub

    End Sub

    Friend Sub EditorProjectile_DrawProjectile()
        Dim iconnum As Integer

        iconnum = frmProjectile.nudPic.Value

        If iconnum < 1 OrElse iconnum > NumProjectiles Then
            frmProjectile.picProjectile.BackgroundImage = Nothing
            Exit Sub
        End If

        If File.Exists(Application.StartupPath & GFX_PATH & "Projectiles\" & iconnum & GFX_EXT) Then
            frmProjectile.picProjectile.BackgroundImage = Image.FromFile(Application.StartupPath & GFX_PATH & "Projectiles\" & iconnum & GFX_EXT)
        End If

    End Sub

#End Region

#Region "Projectile Editor"

    Friend Sub ProjectileEditorInit()

        If frmProjectile.Visible = False Then Exit Sub
        EditorIndex = frmProjectile.lstIndex.SelectedIndex + 1

        With Projectiles(EditorIndex)
            frmProjectile.txtName.Text = Trim$(.Name)
            frmProjectile.nudPic.Value = .Sprite
            frmProjectile.nudRange.Value = .Range
            frmProjectile.nudSpeed.Value = .Speed
            frmProjectile.nudDamage.Value = .Damage
        End With

        Projectile_Changed(EditorIndex) = True

    End Sub

    Friend Sub ProjectileEditorOk()
        Dim i As Integer

        For i = 1 To MAX_PROJECTILES
            If Projectile_Changed(i) Then
                Call SendSaveProjectile(i)
            End If
        Next

        frmProjectile.Dispose()
        Editor = 0
        ClearChanged_Projectile()

    End Sub

    Friend Sub ProjectileEditorCancel()

        Editor = 0
        frmProjectile.Dispose()
        ClearChanged_Projectile()
        ClearProjectiles()
        SendRequestProjectiles()

    End Sub

    Friend Sub ClearChanged_Projectile()
        Dim i As Integer

        For i = 0 To MAX_PROJECTILES
            Projectile_Changed(i) = False
        Next

    End Sub

#End Region

End Module
