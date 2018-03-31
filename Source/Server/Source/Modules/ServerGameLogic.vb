Imports ASFW

Module ServerGameLogic
    Function GetTotalMapPlayers(mapNum as Integer) As Integer
        Dim i As Integer, n As Integer
        n = 0

        For i = 1 To GetPlayersOnline()
            If IsPlaying(i) AndAlso GetPlayerMap(i) = MapNum Then
                n = n + 1
            End If
        Next

        GetTotalMapPlayers = n
    End Function

    Friend Function GetPlayersOnline() As Integer
        Dim x As Integer
        x = 0
        For i As Integer = 1 To Socket.HighIndex
            If TempPlayer(i).InGame = True Then
                x = x + 1
            End If
        Next
        GetPlayersOnline = x
    End Function

    Function GetNpcMaxVital(NpcNum As Integer, Vital As VitalType) As Integer
        GetNpcMaxVital = 0

        ' Prevent subscript out of range
        If NpcNum <= 0 OrElse NpcNum > MAX_NPCS Then Exit Function

        Select Case Vital
            Case VitalType.HP
                GetNpcMaxVital = Npc(NpcNum).Hp
            Case VitalType.MP
                GetNpcMaxVital = Npc(NpcNum).Stat(StatType.Intelligence) * 2
            Case VitalType.SP
                GetNpcMaxVital = Npc(NpcNum).Stat(StatType.Spirit) * 2
        End Select

    End Function

    Function FindPlayer(Name As String) As Integer
        Dim i As Integer

        For i = 1 To GetPlayersOnline()
            If IsPlaying(i) Then
                ' Make sure we dont try to check a name thats to small
                If Len(GetPlayerName(i)) >= Len(Trim$(Name)) Then
                    If UCase$(Mid$(GetPlayerName(i), 1, Len(Trim$(Name)))) = UCase$(Trim$(Name)) Then
                        FindPlayer = i
                        Exit Function
                    End If
                End If
            End If
        Next

        FindPlayer = 0
    End Function

    Sub SpawnItem(itemNum As Integer, ItemVal As Integer, mapNum as Integer, x As Integer, y As Integer)
        Dim i As Integer

        ' Check for subscript out of range
        If itemNum < 1 OrElse itemNum > MAX_ITEMS OrElse MapNum <= 0 OrElse MapNum > MAX_CACHED_MAPS Then Exit Sub

        ' Find open map item slot
        i = FindOpenMapItemSlot(MapNum)

        If i = 0 Then Exit Sub

        SpawnItemSlot(i, itemNum, ItemVal, MapNum, x, y)
    End Sub

    Sub SpawnItemSlot(MapItemSlot As Integer, itemNum As Integer, ItemVal As Integer, mapNum as Integer, x As Integer, y As Integer)
        Dim i As Integer
        dim buffer as New ByteStream(4)

        ' Check for subscript out of range
        If MapItemSlot <= 0 OrElse MapItemSlot > MAX_MAP_ITEMS OrElse itemNum < 0 OrElse itemNum > MAX_ITEMS OrElse MapNum <= 0 OrElse MapNum > MAX_CACHED_MAPS Then Exit Sub

        i = MapItemSlot

        If i <> 0 Then
            If itemNum >= 0 AndAlso itemNum <= MAX_ITEMS Then
                MapItem(MapNum, i).Num = itemNum
                MapItem(MapNum, i).Value = ItemVal
                MapItem(MapNum, i).X = x
                MapItem(MapNum, i).Y = y

                Buffer.WriteInt32(ServerPackets.SSpawnItem)
                Buffer.WriteInt32(i)
                Buffer.WriteInt32(itemNum)
                Buffer.WriteInt32(ItemVal)
                Buffer.WriteInt32(x)
                Buffer.WriteInt32(y)

                Addlog("Sent SMSG: SSpawnItem MapItemSlot", PACKET_LOG)
                Console.WriteLine("Sent SMSG: SSpawnItem MapItemSlot")

                SendDataToMap(MapNum, Buffer.Data, Buffer.Head)
            End If

        End If

        Buffer.Dispose()
    End Sub

    Function FindOpenMapItemSlot(mapNum as Integer) As Integer
        Dim i As Integer
        FindOpenMapItemSlot = 0

        ' Check for subscript out of range
        If MapNum <= 0 OrElse MapNum > MAX_CACHED_MAPS Then Exit Function

        For i = 1 To MAX_MAP_ITEMS
            If MapItem(MapNum, i).Num = 0 Then
                FindOpenMapItemSlot = i
                Exit Function
            End If
        Next

    End Function

    Sub SpawnAllMapsItems()
        Dim i As Integer

        For i = 1 To MAX_CACHED_MAPS
            SpawnMapItems(i)
        Next

    End Sub

    Sub SpawnMapItems(mapNum as Integer)
        Dim x As Integer
        Dim y As Integer

        ' Check for subscript out of range
        If MapNum <= 0 OrElse MapNum > MAX_CACHED_MAPS Then Exit Sub

        ' Spawn what we have
        For x = 0 To Map(MapNum).MaxX
            For y = 0 To Map(MapNum).MaxY
                ' Check if the tile type is an item or a saved tile incase someone drops something
                If (Map(MapNum).Tile(x, y).Type = TileType.Item) Then

                    ' Check to see if its a currency and if they set the value to 0 set it to 1 automatically
                    If Item(Map(MapNum).Tile(x, y).Data1).Type = ItemType.Currency OrElse Item(Map(MapNum).Tile(x, y).Data1).Stackable = 1 Then
                        If Map(MapNum).Tile(x, y).Data2 <= 0 Then
                            SpawnItem(Map(MapNum).Tile(x, y).Data1, 1, MapNum, x, y)
                        Else
                            SpawnItem(Map(MapNum).Tile(x, y).Data1, Map(MapNum).Tile(x, y).Data2, MapNum, x, y)
                        End If
                    Else
                        SpawnItem(Map(MapNum).Tile(x, y).Data1, Map(MapNum).Tile(x, y).Data2, MapNum, x, y)
                    End If
                End If
            Next
        Next

    End Sub

    Friend Sub SpawnNpc(mapNpcNum As Integer, mapNum as Integer)
        dim buffer as New ByteStream(4)
        Dim npcNum As Integer
        Dim x As Integer
        Dim y As Integer
        Dim i = 0
        Dim spawned As Boolean

        ' Check for subscript out of range
        If MapNpcNum <= 0 OrElse MapNpcNum > MAX_MAP_NPCS OrElse MapNum <= 0 OrElse MapNum > MAX_CACHED_MAPS Then Exit Sub

        npcNum = Map(MapNum).Npc(MapNpcNum)

        If npcNum > 0 Then
            If Not Npc(npcNum).SpawnTime = Time.Instance.TimeOfDay AndAlso Not Npc(npcNum).SpawnTime = 4 Then Exit Sub

            MapNpc(MapNum).Npc(MapNpcNum).Num = npcNum
            MapNpc(MapNum).Npc(MapNpcNum).Target = 0
            MapNpc(MapNum).Npc(MapNpcNum).TargetType = 0 ' clear

            MapNpc(MapNum).Npc(MapNpcNum).Vital(VitalType.HP) = GetNpcMaxVital(npcNum, VitalType.HP)
            MapNpc(MapNum).Npc(MapNpcNum).Vital(VitalType.MP) = GetNpcMaxVital(npcNum, VitalType.MP)
            MapNpc(MapNum).Npc(MapNpcNum).Vital(VitalType.SP) = GetNpcMaxVital(npcNum, VitalType.SP)

            MapNpc(MapNum).Npc(MapNpcNum).Dir = Int(Rnd() * 4)

            'Check if theres a spawn tile for the specific npc
            For x = 0 To Map(MapNum).MaxX
                For y = 0 To Map(MapNum).MaxY
                    If Map(MapNum).Tile(x, y).Type = TileType.NpcSpawn Then
                        If Map(MapNum).Tile(x, y).Data1 = MapNpcNum Then
                            MapNpc(MapNum).Npc(MapNpcNum).X = x
                            MapNpc(MapNum).Npc(MapNpcNum).Y = y
                            MapNpc(MapNum).Npc(MapNpcNum).Dir = Map(MapNum).Tile(x, y).Data2
                            spawned = True
                            Exit For
                        End If
                    End If
                Next y
            Next x

            If Not spawned Then
                ' Well try 100 times to randomly place the sprite
                While i < 100
                    x = Random(0, Map(MapNum).MaxX)
                    y = Random(0, Map(MapNum).MaxY)

                    If x > Map(MapNum).MaxX Then x = Map(MapNum).MaxX
                    If y > Map(MapNum).MaxY Then y = Map(MapNum).MaxY

                    ' Check if the tile is walkable
                    If NpcTileIsOpen(MapNum, x, y) Then
                        MapNpc(MapNum).Npc(MapNpcNum).X = x
                        MapNpc(MapNum).Npc(MapNpcNum).Y = y
                        spawned = True
                        Exit while
                    End If
                    i += 1
                End While
            End If

            ' Didn't spawn, so now we'll just try to find a free tile
            If Not spawned Then
                For x = 0 To Map(MapNum).MaxX
                    For y = 0 To Map(MapNum).MaxY
                        If NpcTileIsOpen(MapNum, x, y) Then
                            MapNpc(MapNum).Npc(MapNpcNum).X = x
                            MapNpc(MapNum).Npc(MapNpcNum).Y = y
                            spawned = True
                        End If
                    Next
                Next
            End If

            ' If we suceeded in spawning then send it to everyone
            If spawned Then
                buffer.WriteInt32(ServerPackets.SSpawnNpc)
                buffer.WriteInt32(MapNpcNum)
                buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).Num)
                buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).X)
                buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).Y)
                buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).Dir)

                Addlog("Recieved SMSG: SSpawnNpc", PACKET_LOG)
                Console.WriteLine("Recieved SMSG: SSpawnNpc")

                For i = 1 To VitalType.Count - 1
                    buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).Vital(i))
                Next

                SendDataToMap(MapNum, buffer.Data, buffer.Head)
            End If

            SendMapNpcVitals(MapNum, MapNpcNum)
        End If

        buffer.Dispose()
    End Sub

    Friend Function Random(low As Int32, high As Int32) As Integer
        Static randomNumGen As New Random
        Return randomNumGen.Next(low, high + 1)
    End Function

    Friend Function NpcTileIsOpen(mapNum as Integer, x As Integer, y As Integer) As Boolean
        Dim LoopI As Integer
        NpcTileIsOpen = True

        If PlayersOnMap(MapNum) Then
            For LoopI = 1 To Socket.HighIndex
                If GetPlayerMap(LoopI) = MapNum AndAlso GetPlayerX(LoopI) = x AndAlso GetPlayerY(LoopI) = y Then
                    NpcTileIsOpen = False
                    Exit Function
                End If
            Next
        End If

        For LoopI = 1 To MAX_MAP_NPCS
            If MapNpc(MapNum).Npc(LoopI).Num > 0 AndAlso MapNpc(MapNum).Npc(LoopI).X = x AndAlso MapNpc(MapNum).Npc(LoopI).Y = y Then
                NpcTileIsOpen = False
                Exit Function
            End If
        Next

        If Map(MapNum).Tile(x, y).Type <> TileType.None AndAlso Map(MapNum).Tile(x, y).Type <> TileType.NpcSpawn AndAlso Map(MapNum).Tile(x, y).Type <> TileType.Item Then
            NpcTileIsOpen = False
        End If

    End Function

    Friend Function CheckGrammar(Word As String, Optional Caps As Byte = 0) As String
        Dim FirstLetter As String

        FirstLetter = LCase$(Left$(Word, 1))

        If FirstLetter = "$" Then
            CheckGrammar = (Mid$(Word, 2, Len(Word) - 1))
            Exit Function
        End If

        If FirstLetter Like "*[aeiou]*" Then
            If Caps Then CheckGrammar = "An " & Word Else CheckGrammar = "an " & Word
        Else
            If Caps Then CheckGrammar = "A " & Word Else CheckGrammar = "a " & Word
        End If
    End Function

    Function CanNpcMove(mapNum as Integer, MapNpcNum As Integer, Dir As Byte) As Boolean
        Dim i As Integer
        Dim n As Integer
        Dim x As Integer
        Dim y As Integer

        ' Check for subscript out of range
        If MapNum <= 0 OrElse MapNum > MAX_CACHED_MAPS OrElse MapNpcNum <= 0 OrElse MapNpcNum > MAX_MAP_NPCS OrElse Dir < DirectionType.Up OrElse Dir > DirectionType.Right Then
            Exit Function
        End If

        x = MapNpc(MapNum).Npc(MapNpcNum).X
        y = MapNpc(MapNum).Npc(MapNpcNum).Y
        CanNpcMove = True

        Select Case Dir
            Case DirectionType.Up

                ' Check to make sure not outside of boundries
                If y > 0 Then
                    n = Map(MapNum).Tile(x, y - 1).Type

                    ' Check to make sure that the tile is walkable
                    If n <> TileType.None AndAlso n <> TileType.Item AndAlso n <> TileType.NpcSpawn Then
                        CanNpcMove = False
                        Exit Function
                    End If

                    ' Check to make sure that there is not a player in the way
                    For i = 1 To GetPlayersOnline()
                        If IsPlaying(i) Then
                            If (GetPlayerMap(i) = MapNum) AndAlso (GetPlayerX(i) = MapNpc(MapNum).Npc(MapNpcNum).X) AndAlso (GetPlayerY(i) = MapNpc(MapNum).Npc(MapNpcNum).Y - 1) Then
                                CanNpcMove = False
                                Exit Function
                            End If
                        End If
                    Next

                    ' Check to make sure that there is not another npc in the way
                    For i = 1 To MAX_MAP_NPCS
                        If (i <> MapNpcNum) AndAlso (MapNpc(MapNum).Npc(i).Num > 0) AndAlso (MapNpc(MapNum).Npc(i).X = MapNpc(MapNum).Npc(MapNpcNum).X) AndAlso (MapNpc(MapNum).Npc(i).Y = MapNpc(MapNum).Npc(MapNpcNum).Y - 1) Then
                            CanNpcMove = False
                            Exit Function
                        End If
                    Next
                Else
                    CanNpcMove = False
                End If

            Case DirectionType.Down

                ' Check to make sure not outside of boundries
                If y < Map(MapNum).MaxY Then
                    n = Map(MapNum).Tile(x, y + 1).Type

                    ' Check to make sure that the tile is walkable
                    If n <> TileType.None AndAlso n <> TileType.Item AndAlso n <> TileType.NpcSpawn Then
                        CanNpcMove = False
                        Exit Function
                    End If

                    ' Check to make sure that there is not a player in the way
                    For i = 1 To GetPlayersOnline()
                        If IsPlaying(i) Then
                            If (GetPlayerMap(i) = MapNum) AndAlso (GetPlayerX(i) = MapNpc(MapNum).Npc(MapNpcNum).X) AndAlso (GetPlayerY(i) = MapNpc(MapNum).Npc(MapNpcNum).Y + 1) Then
                                CanNpcMove = False
                                Exit Function
                            End If
                        End If
                    Next

                    ' Check to make sure that there is not another npc in the way
                    For i = 1 To MAX_MAP_NPCS
                        If (i <> MapNpcNum) AndAlso (MapNpc(MapNum).Npc(i).Num > 0) AndAlso (MapNpc(MapNum).Npc(i).X = MapNpc(MapNum).Npc(MapNpcNum).X) AndAlso (MapNpc(MapNum).Npc(i).Y = MapNpc(MapNum).Npc(MapNpcNum).Y + 1) Then
                            CanNpcMove = False
                            Exit Function
                        End If
                    Next
                Else
                    CanNpcMove = False
                End If

            Case DirectionType.Left

                ' Check to make sure not outside of boundries
                If x > 0 Then
                    n = Map(MapNum).Tile(x - 1, y).Type

                    ' Check to make sure that the tile is walkable
                    If n <> TileType.None AndAlso n <> TileType.Item AndAlso n <> TileType.NpcSpawn Then
                        CanNpcMove = False
                        Exit Function
                    End If

                    ' Check to make sure that there is not a player in the way
                    For i = 1 To GetPlayersOnline()
                        If IsPlaying(i) Then
                            If (GetPlayerMap(i) = MapNum) AndAlso (GetPlayerX(i) = MapNpc(MapNum).Npc(MapNpcNum).X - 1) AndAlso (GetPlayerY(i) = MapNpc(MapNum).Npc(MapNpcNum).Y) Then
                                CanNpcMove = False
                                Exit Function
                            End If
                        End If
                    Next

                    ' Check to make sure that there is not another npc in the way
                    For i = 1 To MAX_MAP_NPCS
                        If (i <> MapNpcNum) AndAlso (MapNpc(MapNum).Npc(i).Num > 0) AndAlso (MapNpc(MapNum).Npc(i).X = MapNpc(MapNum).Npc(MapNpcNum).X - 1) AndAlso (MapNpc(MapNum).Npc(i).Y = MapNpc(MapNum).Npc(MapNpcNum).Y) Then
                            CanNpcMove = False
                            Exit Function
                        End If
                    Next
                Else
                    CanNpcMove = False
                End If

            Case DirectionType.Right

                ' Check to make sure not outside of boundries
                If x < Map(MapNum).MaxX Then
                    n = Map(MapNum).Tile(x + 1, y).Type

                    ' Check to make sure that the tile is walkable
                    If n <> TileType.None AndAlso n <> TileType.Item AndAlso n <> TileType.NpcSpawn Then
                        CanNpcMove = False
                        Exit Function
                    End If

                    ' Check to make sure that there is not a player in the way
                    For i = 1 To GetPlayersOnline()
                        If IsPlaying(i) Then
                            If (GetPlayerMap(i) = MapNum) AndAlso (GetPlayerX(i) = MapNpc(MapNum).Npc(MapNpcNum).X + 1) AndAlso (GetPlayerY(i) = MapNpc(MapNum).Npc(MapNpcNum).Y) Then
                                CanNpcMove = False
                                Exit Function
                            End If
                        End If
                    Next

                    ' Check to make sure that there is not another npc in the way
                    For i = 1 To MAX_MAP_NPCS
                        If (i <> MapNpcNum) AndAlso (MapNpc(MapNum).Npc(i).Num > 0) AndAlso (MapNpc(MapNum).Npc(i).X = MapNpc(MapNum).Npc(MapNpcNum).X + 1) AndAlso (MapNpc(MapNum).Npc(i).Y = MapNpc(MapNum).Npc(MapNpcNum).Y) Then
                            CanNpcMove = False
                            Exit Function
                        End If
                    Next
                Else
                    CanNpcMove = False
                End If

        End Select

        If MapNpc(MapNum).Npc(MapNpcNum).SkillBuffer > 0 Then CanNpcMove = False

    End Function

    Sub NpcMove(mapNum as Integer, MapNpcNum As Integer, Dir As Integer, Movement As Integer)
        dim buffer as New ByteStream(4)

        ' Check for subscript out of range
        If MapNum <= 0 OrElse MapNum > MAX_CACHED_MAPS OrElse MapNpcNum <= 0 OrElse MapNpcNum > MAX_MAP_NPCS OrElse Dir < DirectionType.Up OrElse Dir > DirectionType.Right OrElse Movement < 1 OrElse Movement > 2 Then
            Exit Sub
        End If

        MapNpc(MapNum).Npc(MapNpcNum).Dir = Dir

        Select Case Dir
            Case DirectionType.Up
                MapNpc(MapNum).Npc(MapNpcNum).Y = MapNpc(MapNum).Npc(MapNpcNum).Y - 1

                Buffer.WriteInt32(ServerPackets.SNpcMove)
                Buffer.WriteInt32(MapNpcNum)
                Buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).X)
                Buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).Y)
                Buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).Dir)
                Buffer.WriteInt32(Movement)

                Addlog("Sent SMSG: SNpcMove Up", PACKET_LOG)
                Console.WriteLine("Sent SMSG: SNpcMove Up")

                SendDataToMap(MapNum, Buffer.Data, Buffer.Head)
            Case DirectionType.Down
                MapNpc(MapNum).Npc(MapNpcNum).Y = MapNpc(MapNum).Npc(MapNpcNum).Y + 1

                Buffer.WriteInt32(ServerPackets.SNpcMove)
                Buffer.WriteInt32(MapNpcNum)
                Buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).X)
                Buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).Y)
                Buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).Dir)
                Buffer.WriteInt32(Movement)

                Addlog("Sent SMSG: SNpcMove Down", PACKET_LOG)
                Console.WriteLine("Sent SMSG: SNpcMove Down")

                SendDataToMap(MapNum, Buffer.Data, Buffer.Head)
            Case DirectionType.Left
                MapNpc(MapNum).Npc(MapNpcNum).X = MapNpc(MapNum).Npc(MapNpcNum).X - 1

                Buffer.WriteInt32(ServerPackets.SNpcMove)
                Buffer.WriteInt32(MapNpcNum)
                Buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).X)
                Buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).Y)
                Buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).Dir)
                Buffer.WriteInt32(Movement)

                Addlog("Sent SMSG: SNpcMove Left", PACKET_LOG)
                Console.WriteLine("Sent SMSG: SNpcMove Left")

                SendDataToMap(MapNum, Buffer.Data, Buffer.Head)
            Case DirectionType.Right
                MapNpc(MapNum).Npc(MapNpcNum).X = MapNpc(MapNum).Npc(MapNpcNum).X + 1

                Buffer.WriteInt32(ServerPackets.SNpcMove)
                Buffer.WriteInt32(MapNpcNum)
                Buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).X)
                Buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).Y)
                Buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).Dir)
                Buffer.WriteInt32(Movement)

                Addlog("Sent SMSG: SNpcMove Right", PACKET_LOG)
                Console.WriteLine("Sent SMSG: SNpcMove Right")

                SendDataToMap(MapNum, Buffer.Data, Buffer.Head)
        End Select

        Buffer.Dispose()
    End Sub

    Sub NpcDir(mapNum as Integer, MapNpcNum As Integer, Dir As Integer)
        dim buffer as New ByteStream(4)

        ' Check for subscript out of range
        If MapNum <= 0 OrElse MapNum > MAX_CACHED_MAPS OrElse MapNpcNum <= 0 OrElse MapNpcNum > MAX_MAP_NPCS OrElse Dir < DirectionType.Up OrElse Dir > DirectionType.Right Then
            Exit Sub
        End If

        MapNpc(MapNum).Npc(MapNpcNum).Dir = Dir

        Buffer.WriteInt32(ServerPackets.SNpcDir)
        Buffer.WriteInt32(MapNpcNum)
        Buffer.WriteInt32(Dir)

        Addlog("Sent SMSG: SNpcDir", PACKET_LOG)
        Console.WriteLine("Sent SMSG: SNpcDir")

        SendDataToMap(MapNum, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SpawnAllMapNpcs()
        Dim i As Integer

        For i = 1 To MAX_CACHED_MAPS
            SpawnMapNpcs(i)
        Next

    End Sub

    Sub SpawnMapNpcs(mapNum as Integer)
        Dim i As Integer

        For i = 1 To MAX_MAP_NPCS
            SpawnNpc(i, MapNum)
        Next

    End Sub

    Sub SendMapNpcsToMap(mapNum as Integer)
        Dim i As Integer
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SMapNpcData)

        Addlog("Sent SMSG: SMapNpcData", PACKET_LOG)
        Console.WriteLine("Sent SMSG: SMapNpcData")

        For i = 1 To MAX_MAP_NPCS
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).Num)
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).X)
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).Y)
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).Dir)
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).Vital(VitalType.HP))
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).Vital(VitalType.MP))
        Next

        SendDataToMap(MapNum, Buffer.Data, Buffer.Head)

        Buffer.Dispose
    End Sub
End Module