Imports System.IO
Imports ASFW

Friend Module ServerHousing
#Region "Globals & Types"
    Friend MAX_HOUSES As Integer = 100

    Friend HouseConfig() As HouseRec

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

#Region "DataBase"
    Sub LoadHouses()
        Dim i As Integer
        Dim myXml As New XmlClass With {
            .Filename = Path.Combine(Application.StartupPath, "data", "houseconfig.xml"),
            .Root = "Config"
        }

        For i = 1 To MAX_HOUSES

            HouseConfig(i).BaseMap = Val(myXml.ReadString("House" & i, "BaseMap"))
            HouseConfig(i).ConfigName = Trim$(myXml.ReadString("House" & i, "Name"))
            HouseConfig(i).MaxFurniture = Val(myXml.ReadString("House" & i, "MaxFurniture"))
            HouseConfig(i).Price = Val(myXml.ReadString("House" & i, "Price"))
            HouseConfig(i).X = Val(myXml.ReadString("House" & i, "X"))
            HouseConfig(i).Y = Val(myXml.ReadString("House" & i, "Y"))
            Application.DoEvents()
        Next
        For i = 1 To GetPlayersOnline()
            If IsPlaying(i) Then
                SendHouseConfigs(i)
            End If
        Next

    End Sub

    Sub SaveHouse(index as integer)
        Dim myXml As New XmlClass With {
            .Filename = Path.Combine(Application.StartupPath, "data", "houseconfig.xml"),
            .Root = "Config"
        }

        If Index > 0 AndAlso Index <= MAX_HOUSES Then
            myXml.WriteString("House" & Index, "BaseMap", HouseConfig(Index).BaseMap)
            myXml.WriteString("House" & Index, "Name", HouseConfig(Index).ConfigName)
            myXml.WriteString("House" & Index, "MaxFurniture", HouseConfig(Index).MaxFurniture)
            myXml.WriteString("House" & Index, "Price", HouseConfig(Index).Price)
            myXml.WriteString("House" & Index, "X", HouseConfig(Index).X)
            myXml.WriteString("House" & Index, "Y", HouseConfig(Index).Y)
        End If
        LoadHouses()

    End Sub

    Sub SaveHouses()
        Dim i As Integer

        For i = 1 To MAX_HOUSES
            SaveHouse(i)
        Next

    End Sub
#End Region

#Region "Incoming Packets"
    Sub Packet_BuyHouse(index as integer, ByRef data() As Byte)
        Dim i As Integer, price As Integer
        dim buffer as New ByteStream(data)
        i = Buffer.ReadInt32

        If i = 1 Then
            If TempPlayer(Index).BuyHouseIndex > 0 Then
                price = HouseConfig(TempPlayer(Index).BuyHouseIndex).Price
                If HasItem(Index, 1) >= price Then
                    TakeInvItem(Index, 1, price)
                    Player(Index).Character(TempPlayer(Index).CurChar).House.HouseIndex = TempPlayer(Index).BuyHouseIndex
                    PlayerMsg(Index, "You just bought the " & Trim$(HouseConfig(TempPlayer(Index).BuyHouseIndex).ConfigName) & " house!", ColorType.BrightGreen)
                    Player(Index).Character(TempPlayer(Index).CurChar).LastMap = GetPlayerMap(Index)
                    Player(Index).Character(TempPlayer(Index).CurChar).LastX = GetPlayerX(Index)
                    Player(Index).Character(TempPlayer(Index).CurChar).LastY = GetPlayerY(Index)
                    Player(Index).Character(TempPlayer(Index).CurChar).InHouse = Index

                    PlayerWarp(Index, HouseConfig(Player(Index).Character(TempPlayer(Index).CurChar).House.HouseIndex).BaseMap, HouseConfig(Player(Index).Character(TempPlayer(Index).CurChar).House.HouseIndex).X, HouseConfig(Player(Index).Character(TempPlayer(Index).CurChar).House.HouseIndex).Y, True)
                    SavePlayer(Index)
                Else
                    PlayerMsg(Index, "You cannot afford this house!", ColorType.BrightRed)
                End If
            End If
        End If

        TempPlayer(Index).BuyHouseIndex = 0

        Buffer.Dispose()

    End Sub

    Sub Packet_InviteToHouse(index as integer, ByRef data() As Byte)
        Dim invitee As Integer, Name As String
        dim buffer as New ByteStream(data)
        Name = Trim$(Buffer.ReadString)
        invitee = FindPlayer(Name)
        Buffer.Dispose()

        If invitee = 0 Then
            PlayerMsg(Index, "Player not found.", ColorType.BrightRed)
            Exit Sub
        End If

        If Index = invitee Then
            PlayerMsg(Index, "You cannot invite yourself to you own house!", ColorType.BrightRed)
            Exit Sub
        End If

        If TempPlayer(invitee).InvitationIndex > 0 Then
            If TempPlayer(invitee).InvitationTimer > GetTimeMs() Then
                PlayerMsg(Index, Trim$(GetPlayerName(invitee)) & " is currently busy!", ColorType.Yellow)
                Exit Sub
            End If
        End If

        If Player(Index).Character(TempPlayer(Index).CurChar).House.HouseIndex > 0 Then
            If Player(Index).Character(TempPlayer(Index).CurChar).InHouse > 0 Then
                If Player(Index).Character(TempPlayer(Index).CurChar).InHouse = Index Then
                    If Player(invitee).Character(TempPlayer(invitee).CurChar).InHouse > 0 Then
                        If Player(invitee).Character(TempPlayer(invitee).CurChar).InHouse = Index Then
                            PlayerMsg(Index, Trim$(GetPlayerName(invitee)) & " is already in your house!", ColorType.Yellow)
                        Else
                            PlayerMsg(Index, Trim$(GetPlayerName(invitee)) & " is already visiting someone elses house!", ColorType.Yellow)
                        End If
                    Else
                        'Send invite
                        Buffer = New ByteStream(4)
                        Buffer.WriteInt32(ServerPackets.SVisit)
                        Buffer.WriteInt32(Index)
                        Socket.SendDataTo(invitee, Buffer.Data, Buffer.Head)
                        TempPlayer(invitee).InvitationIndex = Index
                        TempPlayer(invitee).InvitationTimer = GetTimeMs() + 15000
                        Buffer.Dispose()
                    End If
                Else
                    PlayerMsg(Index, "Only the house owner can invite other players into their house.", ColorType.BrightRed)
                End If
            Else
                PlayerMsg(Index, "You must be inside your house before you can invite someone to visit!", ColorType.BrightRed)
            End If
        Else
            PlayerMsg(Index, "You do not have a house to invite anyone to!", ColorType.BrightRed)
        End If

    End Sub

    Sub Packet_AcceptInvite(index as integer, ByRef data() As Byte)
        Dim response As Integer
        dim buffer as New ByteStream(data)
        response = Buffer.ReadInt32
        Buffer.Dispose()

        If response = 1 Then
            If TempPlayer(Index).InvitationIndex > 0 Then
                If TempPlayer(Index).InvitationTimer > GetTimeMs() Then
                    'Accept this invite
                    If IsPlaying(TempPlayer(Index).InvitationIndex) Then
                        Player(Index).Character(TempPlayer(Index).CurChar).InHouse = TempPlayer(Index).InvitationIndex
                        Player(Index).Character(TempPlayer(Index).CurChar).LastX = GetPlayerX(Index)
                        Player(Index).Character(TempPlayer(Index).CurChar).LastY = GetPlayerY(Index)
                        Player(Index).Character(TempPlayer(Index).CurChar).LastMap = GetPlayerMap(Index)
                        TempPlayer(Index).InvitationTimer = 0
                        PlayerWarp(Index, Player(TempPlayer(Index).InvitationIndex).Character(TempPlayer(Index).CurChar).Map, HouseConfig(Player(TempPlayer(Index).InvitationIndex).Character(TempPlayer(TempPlayer(Index).InvitationIndex).CurChar).House.HouseIndex).X, HouseConfig(Player(TempPlayer(Index).InvitationIndex).Character(TempPlayer(TempPlayer(Index).InvitationIndex).CurChar).House.HouseIndex).Y, True)
                    Else
                        TempPlayer(Index).InvitationTimer = 0
                        PlayerMsg(Index, "Cannot find player!", ColorType.BrightRed)
                    End If
                Else
                    PlayerMsg(Index, "Your invitation has expired, have your friend re-invite you.", ColorType.Yellow)
                End If
            End If
        Else
            If IsPlaying(TempPlayer(Index).InvitationIndex) Then
                TempPlayer(Index).InvitationTimer = 0
                PlayerMsg(TempPlayer(Index).InvitationIndex, Trim$(GetPlayerName(Index)) & " rejected your invitation", ColorType.BrightRed)
            End If
        End If

    End Sub

    Sub Packet_PlaceFurniture(index as integer, ByRef data() As Byte)
        Dim i As Integer, x As Integer, y As Integer, invslot As Integer
        Dim ItemNum As Integer, x1 As Integer, y1 As Integer, widthoffset As Integer

        dim buffer as New ByteStream(data)
        x = Buffer.ReadInt32
        y = Buffer.ReadInt32
        invslot = Buffer.ReadInt32
        Buffer.Dispose()

        ItemNum = Player(Index).Character(TempPlayer(Index).CurChar).Inv(invslot).Num

        ' Prevent hacking
        If ItemNum < 1 OrElse ItemNum > MAX_ITEMS Then Exit Sub

        If Player(Index).Character(TempPlayer(Index).CurChar).InHouse = Index Then
            If Item(ItemNum).Type = ItemType.Furniture Then
                ' stat requirements
                For i = 1 To StatType.Count - 1
                    If GetPlayerRawStat(Index, i) < Item(ItemNum).Stat_Req(i) Then
                        PlayerMsg(Index, "You do not meet the stat requirements to use this item.", ColorType.BrightRed)
                        Exit Sub
                    End If
                Next

                ' level requirement
                If GetPlayerLevel(Index) < Item(ItemNum).LevelReq Then
                    PlayerMsg(Index, "You do not meet the level requirement to use this item.", ColorType.BrightRed)
                    Exit Sub
                End If

                ' class requirement
                If Item(ItemNum).ClassReq > 0 Then
                    If Not GetPlayerClass(Index) = Item(ItemNum).ClassReq Then
                        PlayerMsg(Index, "You do not meet the class requirement to use this item.", ColorType.BrightRed)
                        Exit Sub
                    End If
                End If

                ' access requirement
                If Not GetPlayerAccess(Index) >= Item(ItemNum).AccessReq Then
                    PlayerMsg(Index, "You do not meet the access requirement to use this item.", ColorType.BrightRed)
                    Exit Sub
                End If

                'Ok, now we got to see what can be done about this furniture :/
                If Player(Index).Character(TempPlayer(Index).CurChar).InHouse <> Index Then
                    PlayerMsg(Index, "You must be inside your house to place furniture!", ColorType.Yellow)
                    Exit Sub
                End If

                If Player(Index).Character(TempPlayer(Index).CurChar).House.FurnitureCount >= HouseConfig(Player(Index).Character(TempPlayer(Index).CurChar).House.HouseIndex).MaxFurniture Then
                    If HouseConfig(Player(Index).Character(TempPlayer(Index).CurChar).House.HouseIndex).MaxFurniture > 0 Then
                        PlayerMsg(Index, "Your house cannot hold any more furniture!", ColorType.BrightRed)
                        Exit Sub
                    End If
                End If

                If x < 0 OrElse x > Map(GetPlayerMap(Index)).MaxX Then Exit Sub
                If y < 0 OrElse y > Map(GetPlayerMap(Index)).MaxY Then Exit Sub

                If Item(ItemNum).FurnitureWidth > 2 Then
                    x1 = x + (Item(ItemNum).FurnitureWidth / 2)
                    widthoffset = x1 - x
                    x1 = x1 - (Item(ItemNum).FurnitureWidth - widthoffset)
                Else
                    x1 = x
                End If

                x1 = x
                widthoffset = 0

                y1 = y

                If widthoffset > 0 Then

                    For x = x1 To x1 + widthoffset
                        For y = y1 To y1 - Item(ItemNum).FurnitureHeight + 1 Step -1
                            If Map(GetPlayerMap(Index)).Tile(x, y).Type = TileType.Blocked Then Exit Sub

                            For i = 1 To GetPlayersOnline()
                                If IsPlaying(i) AndAlso i <> Index AndAlso GetPlayerMap(i) = GetPlayerMap(Index) Then
                                    If Player(i).Character(TempPlayer(i).CurChar).InHouse = Player(Index).Character(TempPlayer(Index).CurChar).InHouse Then
                                        If Player(i).Character(TempPlayer(i).CurChar).X = x AndAlso Player(i).Character(TempPlayer(i).CurChar).Y = y Then
                                            Exit Sub
                                        End If
                                    End If
                                End If
                            Next

                            If Player(Index).Character(TempPlayer(Index).CurChar).House.FurnitureCount > 0 Then
                                For i = 1 To Player(Index).Character(TempPlayer(Index).CurChar).House.FurnitureCount
                                    If x >= Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).X AndAlso x <= Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).X + Item(Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).ItemNum).FurnitureWidth - 1 Then
                                        If y <= Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).Y AndAlso y >= Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).Y - Item(Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).ItemNum).FurnitureHeight + 1 Then
                                            'Blocked!
                                            Exit Sub
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    Next

                    For x = x1 To x1 - (Item(ItemNum).FurnitureWidth - widthoffset) Step -1
                        For y = y1 To y1 - Item(ItemNum).FurnitureHeight + 1 Step -1
                            If Map(GetPlayerMap(Index)).Tile(x, y).Type = TileType.Blocked Then Exit Sub

                            For i = 1 To GetPlayersOnline()
                                If IsPlaying(i) AndAlso i <> Index AndAlso GetPlayerMap(i) = GetPlayerMap(Index) Then
                                    If Player(i).Character(TempPlayer(i).CurChar).InHouse = Player(Index).Character(TempPlayer(Index).CurChar).InHouse Then
                                        If Player(i).Character(TempPlayer(i).CurChar).X = x AndAlso Player(i).Character(TempPlayer(i).CurChar).Y = y Then
                                            Exit Sub
                                        End If
                                    End If
                                End If
                            Next

                            If Player(Index).Character(TempPlayer(Index).CurChar).House.FurnitureCount > 0 Then
                                For i = 1 To Player(Index).Character(TempPlayer(Index).CurChar).House.FurnitureCount
                                    If x >= Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).X AndAlso x <= Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).X + Item(Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).ItemNum).FurnitureWidth - 1 Then
                                        If y <= Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).Y AndAlso y >= Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).Y - Item(Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).ItemNum).FurnitureHeight + 1 Then
                                            'Blocked!
                                            Exit Sub
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    Next
                Else
                    For x = x1 To x1 + Item(ItemNum).FurnitureWidth - 1
                        For y = y1 To y1 - Item(ItemNum).FurnitureHeight + 1 Step -1
                            If Map(GetPlayerMap(Index)).Tile(x, y).Type = TileType.Blocked Then Exit Sub

                            For i = 1 To GetPlayersOnline()
                                If IsPlaying(i) AndAlso i <> Index AndAlso GetPlayerMap(i) = GetPlayerMap(Index) Then
                                    If Player(i).Character(TempPlayer(i).CurChar).InHouse = Player(Index).Character(TempPlayer(Index).CurChar).InHouse Then
                                        If Player(i).Character(TempPlayer(i).CurChar).X = x AndAlso Player(i).Character(TempPlayer(i).CurChar).Y = y Then
                                            Exit Sub
                                        End If
                                    End If
                                End If
                            Next

                            If Player(Index).Character(TempPlayer(Index).CurChar).House.FurnitureCount > 0 Then
                                For i = 1 To Player(Index).Character(TempPlayer(Index).CurChar).House.FurnitureCount
                                    If x >= Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).X AndAlso x <= Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).X + Item(Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).ItemNum).FurnitureWidth - 1 Then
                                        If y <= Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).Y AndAlso y >= Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).Y - Item(Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).ItemNum).FurnitureHeight + 1 Then
                                            'Blocked!
                                            Exit Sub
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    Next
                End If

                x = x1
                y = y1

                'If all checks out, place furniture and send the update to everyone in the player's house.
                Player(Index).Character(TempPlayer(Index).CurChar).House.FurnitureCount = Player(Index).Character(TempPlayer(Index).CurChar).House.FurnitureCount + 1
                ReDim Preserve Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(Player(Index).Character(TempPlayer(Index).CurChar).House.FurnitureCount)
                Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(Player(Index).Character(TempPlayer(Index).CurChar).House.FurnitureCount).ItemNum = ItemNum
                Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(Player(Index).Character(TempPlayer(Index).CurChar).House.FurnitureCount).X = x
                Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(Player(Index).Character(TempPlayer(Index).CurChar).House.FurnitureCount).Y = y

                TakeInvItem(Index, ItemNum, 0)

                SendFurnitureToHouse(Player(Index).Character(TempPlayer(Index).CurChar).InHouse)

                SavePlayer(Index)
            End If
        Else
            PlayerMsg(Index, "You cannot place furniture unless you are in your own house!", ColorType.BrightRed)
        End If

    End Sub

    Sub Packet_RequestEditHouse(index as integer, ByRef data() As Byte)
        dim buffer as ByteStream, i As Integer

        ' Prevent hacking
        If GetPlayerAccess(Index) < AdminType.Mapper Then Exit Sub

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SHouseEdit)
        For i = 1 To MAX_HOUSES
            Buffer.WriteString(Trim$(HouseConfig(i).ConfigName))
            Buffer.WriteInt32(HouseConfig(i).BaseMap)
            Buffer.WriteInt32(HouseConfig(i).X)
            Buffer.WriteInt32(HouseConfig(i).Y)
            Buffer.WriteInt32(HouseConfig(i).Price)
            Buffer.WriteInt32(HouseConfig(i).MaxFurniture)
        Next
        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Sub Packet_SaveHouses(index as integer, ByRef data() As Byte)
        Dim i As Integer, x As Integer, Count As Integer, z As Integer

        ' Prevent hacking
        If GetPlayerAccess(Index) < AdminType.Mapper Then Exit Sub

        dim buffer as New ByteStream(data)
        Count = Buffer.ReadInt32
        If Count > 0 Then
            For z = 1 To Count
                i = Buffer.ReadInt32
                HouseConfig(i).ConfigName = Trim$(Buffer.ReadString)
                HouseConfig(i).BaseMap = Buffer.ReadInt32
                HouseConfig(i).X = Buffer.ReadInt32
                HouseConfig(i).Y = Buffer.ReadInt32
                HouseConfig(i).Price = Buffer.ReadInt32
                HouseConfig(i).MaxFurniture = Buffer.ReadInt32
                SaveHouse(i)

                For x = 1 To GetPlayersOnline()
                    If IsPlaying(x) AndAlso Player(x).Character(TempPlayer(x).CurChar).InHouse = i Then
                        SendFurnitureToHouse(i)
                    End If
                Next
            Next
        End If

        Buffer.Dispose()

    End Sub

    Sub Packet_SellHouse(index as integer, ByRef data() As Byte)
        Dim i As Integer, refund As Integer
        Dim Tmpindex as integer
        dim buffer as New ByteStream(data)
        TmpIndex = Player(Index).Character(TempPlayer(Index).CurChar).House.HouseIndex
        If TmpIndex > 0 Then
            'get some money back
            refund = HouseConfig(TmpIndex).Price / 2

            Player(Index).Character(TempPlayer(Index).CurChar).House.HouseIndex = 0
            Player(Index).Character(TempPlayer(Index).CurChar).House.FurnitureCount = 0
            ReDim Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(Player(Index).Character(TempPlayer(Index).CurChar).House.FurnitureCount)

            For i = 0 To Player(Index).Character(TempPlayer(Index).CurChar).House.FurnitureCount
                Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).ItemNum = 0
                Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).X = 0
                Player(Index).Character(TempPlayer(Index).CurChar).House.Furniture(i).Y = 0
            Next

            If Player(Index).Character(TempPlayer(Index).CurChar).InHouse = TmpIndex Then
                PlayerWarp(Index, Player(Index).Character(TempPlayer(Index).CurChar).LastMap, Player(Index).Character(TempPlayer(Index).CurChar).LastX, Player(Index).Character(TempPlayer(Index).CurChar).LastY)
            End If

            SavePlayer(Index)

            PlayerMsg(Index, "You sold your House for " & refund & " Gold!", ColorType.BrightGreen)
            GiveInvItem(Index, 1, refund)
        Else
            PlayerMsg(Index, "You dont own a House!", ColorType.BrightRed)
        End If

        Buffer.Dispose()

    End Sub

#End Region

#Region "OutGoing Packets"
    Sub SendHouseConfigs(index as integer)
        dim buffer as New ByteStream(4), i As Integer

        Buffer.WriteInt32(ServerPackets.SHouseConfigs)

        For i = 1 To MAX_HOUSES
            Buffer.WriteString(Trim(HouseConfig(i).ConfigName))
            Buffer.WriteInt32(HouseConfig(i).BaseMap)
            Buffer.WriteInt32(HouseConfig(i).MaxFurniture)
            Buffer.WriteInt32(HouseConfig(i).Price)
        Next

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Sub SendFurnitureToHouse(Houseindex as integer)
        dim buffer as New ByteStream(4), i As Integer

        Buffer.WriteInt32(ServerPackets.SFurniture)
        Buffer.WriteInt32(HouseIndex)
        Buffer.WriteInt32(Player(HouseIndex).Character(TempPlayer(HouseIndex).CurChar).House.FurnitureCount)
        If Player(HouseIndex).Character(TempPlayer(HouseIndex).CurChar).House.FurnitureCount > 0 Then
            For i = 1 To Player(HouseIndex).Character(TempPlayer(HouseIndex).CurChar).House.FurnitureCount
                Buffer.WriteInt32(Player(HouseIndex).Character(TempPlayer(HouseIndex).CurChar).House.Furniture(i).ItemNum)
                Buffer.WriteInt32(Player(HouseIndex).Character(TempPlayer(HouseIndex).CurChar).House.Furniture(i).X)
                Buffer.WriteInt32(Player(HouseIndex).Character(TempPlayer(HouseIndex).CurChar).House.Furniture(i).Y)
            Next
        End If

        For i = 1 To GetPlayersOnline()
            If IsPlaying(i) Then
                If Player(i).Character(TempPlayer(i).CurChar).InHouse = HouseIndex Then
                    Socket.SendDataTo(i, Buffer.Data, Buffer.Head)
                End If
            End If
        Next

        Buffer.Dispose()

    End Sub
#End Region

End Module