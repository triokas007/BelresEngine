Imports System.IO
Imports System.Windows.Forms
Imports ASFW
Imports SFML.Graphics
Imports SFML.Window

Friend Module ModHousing
#Region "Globals & Types"
    Friend MaxHouses As Integer = 100

    Friend FurnitureCount As Integer
    Friend FurnitureHouse As Integer
    Friend FurnitureSelected As Integer
    Friend HouseTileindex as integer

    Friend House() As HouseRec
    Friend HouseConfig() As HouseRec
    Friend Furniture() As FurnitureRec
    Friend NumFurniture As Integer
    Friend HouseChanged(MaxHouses) As Boolean
    Friend HouseEdit As Boolean

    Structure HouseRec
        Dim ConfigName As String
        Dim BaseMap As Integer
        Dim Price As Integer
        Dim MaxFurniture As Integer
        Dim X As Integer
        Dim Y As Integer
    End Structure

    Structure FurnitureRec
        Dim ItemNum As Integer
        Dim X As Integer
        Dim Y As Integer
    End Structure

    Structure PlayerHouseRec
        Dim Houseindex as integer
        Dim FurnitureCount As Integer
        Dim Furniture() As FurnitureRec
    End Structure
#End Region

#Region "Incoming Packets"
    Sub Packet_HouseConfigurations(ByRef data() As Byte)
        Dim i As Integer
        dim buffer as New ByteStream(Data)
        For i = 1 To MaxHouses
            HouseConfig(i).ConfigName = Buffer.ReadString
            HouseConfig(i).BaseMap = Buffer.ReadInt32
            HouseConfig(i).MaxFurniture = Buffer.ReadInt32
            HouseConfig(i).Price = Buffer.ReadInt32
        Next

        Buffer.Dispose()

    End Sub

    Sub Packet_HouseOffer(ByRef data() As Byte)
        Dim i As Integer
        dim buffer as New ByteStream(Data)
        i = Buffer.ReadInt32

        Buffer.Dispose()

        DialogType = DialogueTypeBuyhome
        If HouseConfig(i).MaxFurniture > 0 Then
            ' ask to buy house
            DialogMsg1 = "Would you like to buy the house: " & Trim$(HouseConfig(i).ConfigName)
            DialogMsg2 = "Cost: " & HouseConfig(i).Price
            DialogMsg3 = "Furniture Limit: " & HouseConfig(i).MaxFurniture
        Else
            DialogMsg1 = "Would you like to buy the house: " & Trim$(HouseConfig(i).ConfigName)
            DialogMsg2 = "Cost: " & HouseConfig(i).Price
            DialogMsg3 = "Furniture Limit: None."
        End If

        UpdateDialog = True

        Buffer.Dispose()

    End Sub

    Sub Packet_Visit(ByRef data() As Byte)
        Dim i As Integer
        dim buffer as New ByteStream(Data)
        i = Buffer.ReadInt32

        DialogType = DialogueTypeVisit

        DialogMsg1 = "You have been invited to visit " & Trim$(GetPlayerName(i)) & "'s house."
        DialogMsg2 = ""
        DialogMsg3 = ""

        Buffer.Dispose()

        UpdateDialog = True

    End Sub

    Sub Packet_Furniture(ByRef data() As Byte)
        Dim i As Integer
        dim buffer as New ByteStream(Data)
        FurnitureHouse = Buffer.ReadInt32
        FurnitureCount = Buffer.ReadInt32

        ReDim Furniture(FurnitureCount)
        If FurnitureCount > 0 Then
            For i = 1 To FurnitureCount
                Furniture(i).ItemNum = Buffer.ReadInt32
                Furniture(i).X = Buffer.ReadInt32
                Furniture(i).Y = Buffer.ReadInt32
            Next
        End If

        Buffer.Dispose()

    End Sub

    Sub Packet_EditHouses(ByRef data() As Byte)
        dim buffer as New ByteStream(Data)
        Dim i As Integer
        For i = 1 To MaxHouses
            With House(i)
                .ConfigName = Trim$(Buffer.ReadString)
                .BaseMap = Buffer.ReadInt32
                .X = Buffer.ReadInt32
                .Y = Buffer.ReadInt32
                .Price = Buffer.ReadInt32
                .MaxFurniture = Buffer.ReadInt32
            End With
        Next

        HouseEdit = True

        Buffer.Dispose()

    End Sub
#End Region

#Region "Outgoing Packets"
    Friend Sub SendRequestEditHouse()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.RequestEditHouse)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Friend Sub SendBuyHouse(accepted As Byte)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CBuyHouse)
        Buffer.WriteInt32(Accepted)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendInvite(name As String)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CVisit)
        Buffer.WriteString(Name)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendVisit(accepted As Byte)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CAcceptVisit)
        Buffer.WriteInt32(Accepted)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub
#End Region

#Region "Drawing"
    Friend Sub CheckFurniture()
        Dim i As Integer
        i = 1

        While File.Exists(Application.StartupPath & GfxPath & "Furniture\" & i & GfxExt)
            NumFurniture = NumFurniture + 1
            i = i + 1
        End While

        If NumFurniture = 0 Then Exit Sub
    End Sub

    Friend Sub DrawFurniture(index as integer, layer As Integer)
        Dim i As Integer, itemNum As Integer
        Dim x As Integer, y As Integer, width As Integer, height As Integer, x1 As Integer, y1 As Integer

        ItemNum = Furniture(Index).ItemNum

        If Item(ItemNum).Type <> ItemType.Furniture Then Exit Sub

        i = Item(ItemNum).Data2

        If FurnitureGFXInfo(i).IsLoaded = False Then
            LoadTexture(i, 10)
        End If

        'seeying we still use it, lets update timer
        With SkillIconsGFXInfo(i)
            .TextureTimer = GetTickCount() + 100000
        End With

        Width = Item(ItemNum).FurnitureWidth
        Height = Item(ItemNum).FurnitureHeight

        If Width > 4 Then Width = 4
        If Height > 4 Then Height = 4
        If i <= 0 OrElse i > NumFurniture Then Exit Sub

        ' make sure it's not out of map
        If Furniture(Index).X > Map.MaxX Then Exit Sub
        If Furniture(Index).Y > Map.MaxY Then Exit Sub

        For X1 = 0 To Width - 1
            For Y1 = 0 To Height
                If Item(Furniture(Index).ItemNum).FurnitureFringe(X1, Y1) = Layer Then
                    ' Set base x + y, then the offset due to size
                    X = (Furniture(Index).X * 32) + (X1 * 32)
                    Y = (Furniture(Index).Y * 32 - (Height * 32)) + (Y1 * 32)
                    X = ConvertMapX(X)
                    Y = ConvertMapY(Y)

                    Dim tmpSprite As Sprite = New Sprite(FurnitureGFX(i))
                    tmpSprite.TextureRect = New IntRect(0 + (X1 * 32), 0 + (Y1 * 32), 32, 32)
                    tmpSprite.Position = New Vector2f(X, Y)
                    GameWindow.Draw(tmpSprite)
                End If
            Next
        Next

    End Sub
#End Region

End Module