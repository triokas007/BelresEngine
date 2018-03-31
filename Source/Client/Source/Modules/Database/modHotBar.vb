﻿Imports System.Drawing
Imports ASFW

Friend Module ModHotBar
    Friend SelHotbarSlot As Integer
    Friend SelSkillSlot As Boolean

    Friend Const MaxHotbar As Byte = 7

    'hotbar constants
    Friend Const HotbarTop As Byte = 2
    Friend Const HotbarLeft As Byte = 2
    Friend Const HotbarOffsetX As Byte = 2

    Friend Structure HotbarRec
        Dim Slot As Integer
        Dim SType As Byte
    End Structure

    Friend Function IsHotBarSlot(x As Single, y As Single) As Integer
        Dim tempRec As Rect
        Dim i As Integer

        IsHotBarSlot = 0

        For i = 1 To MaxHotbar
            With tempRec
                .Top = HotbarY + HotbarTop
                .Bottom = .Top + PicY
                .Left = HotbarX + HotbarLeft + ((HotbarOffsetX + 32) * (((i - 1) Mod MaxHotbar)))
                .Right = .Left + PicX
            End With

            If X >= tempRec.Left AndAlso X <= tempRec.Right Then
                If Y >= tempRec.Top AndAlso Y <= tempRec.Bottom Then
                    IsHotBarSlot = i
                    Exit Function
                End If
            End If
        Next

    End Function

    Friend Sub SendSetHotbarSlot(slot As Integer, num As Integer, type As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CSetHotbarSlot)

        Buffer.WriteInt32(Slot)
        Buffer.WriteInt32(Num)
        Buffer.WriteInt32(Type)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendDeleteHotbar(slot As Integer)
        dim buffer as New ByteStream(4)
        Buffer.WriteInt32(ClientPackets.CDeleteHotbarSlot)

        Buffer.WriteInt32(Slot)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendUseHotbarSlot(slot As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CUseHotbarSlot)

        Buffer.WriteInt32(Slot)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub DrawHotbar()
        Dim i As Integer, num As Integer, pic As Integer
        Dim rec As Rectangle, recPos As Rectangle

        RenderSprite(HotBarSprite, GameWindow, HotbarX, HotbarY, 0, 0, HotBarGFXInfo.Width, HotBarGFXInfo.Height)

        For i = 1 To MaxHotbar
            If Player(MyIndex).Hotbar(i).sType = 1 Then
                num = PlayerSkills(Player(MyIndex).Hotbar(i).Slot)

                If num > 0 Then
                    pic = Skill(num).Icon

                    If SkillIconsGFXInfo(pic).IsLoaded = False Then
                        LoadTexture(pic, 9)
                    End If

                    'seeying we still use it, lets update timer
                    With SkillIconsGFXInfo(pic)
                        .TextureTimer = GetTickCount() + 100000
                    End With

                    With rec
                        .Y = 0
                        .Height = 32
                        .X = 0
                        .Width = 32
                    End With

                    If Not SkillCD(i) = 0 Then
                        rec.X = 32
                        rec.Width = 32
                    End If

                    With recPos
                        .Y = HotbarY + HotbarTop
                        .Height = PicY
                        .X = HotbarX + HotbarLeft + ((HotbarOffsetX + 32) * (((i - 1))))
                        .Width = PicX
                    End With

                    RenderSprite(SkillIconsSprite(pic), GameWindow, recPos.X, recPos.Y, rec.X, rec.Y, rec.Width, rec.Height)
                End If

            ElseIf Player(MyIndex).Hotbar(i).sType = 2 Then
                num = PlayerInv(Player(MyIndex).Hotbar(i).Slot).Num

                If num > 0 Then
                    pic = Item(num).Pic

                    If ItemsGFXInfo(pic).IsLoaded = False Then
                        LoadTexture(pic, 4)
                    End If

                    'seeying we still use it, lets update timer
                    With ItemsGFXInfo(pic)
                        .TextureTimer = GetTickCount() + 100000
                    End With

                    With rec
                        .Y = 0
                        .Height = 32
                        .X = 0
                        .Width = 32
                    End With

                    With recPos
                        .Y = HotbarY + HotbarTop
                        .Height = PicY
                        .X = HotbarX + HotbarLeft + ((HotbarOffsetX + 32) * (((i - 1))))
                        .Width = PicX
                    End With

                    RenderSprite(ItemsSprite(pic), GameWindow, recPos.X, recPos.Y, rec.X, rec.Y, rec.Width, rec.Height)
                End If
            End If
        Next

    End Sub
End Module