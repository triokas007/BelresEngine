Imports System.IO

Module modAutoMap
    ' Automapper System
    ' Version: 1.0
    ' Author: Lucas Tardivo (boasfesta)
    ' Map analysis and tips: Richard Johnson, Luan Meireles (Alenzinho)

    Private _mapOrientation() As MapOrientationRec

    Friend MapStart As Integer = 1
    Friend MapSize As Integer = 4
    Friend MapX As Integer = 50
    Friend MapY As Integer = 50

    Friend SandBorder As Integer = 4
    Friend DetailFreq As Integer = 10
    Friend ResourceFreq As Integer = 20

    Friend DetailsChecked As Boolean = True
    Friend PathsChecked As Boolean = True
    Friend RiversChecked As Boolean = True
    Friend MountainsChecked As Boolean = True
    Friend OverGrassChecked As Boolean = True
    Friend ResourcesChecked As Boolean = True

    Enum TilePrefab
        Water = 1
        Sand
        Grass
        Passing
        Overgrass
        River
        Mountain
        Count
    End Enum

    'Distance between mountains and the map limit, so the player can walk freely when teleport between maps
    Private Const MountainBorder As Byte = 5

    Friend Tile(TilePrefab.Count - 1) As TileRec
    Friend Detail() As DetailRec
    Friend ResourcesNum As String
    Private _resources() As String

    Enum MountainTile
        UpLeftBorder = 0
        UpMidBorder
        UpRightBorder
        MidLeftBorder
        Middle
        MidRightBorder
        BottomLeftBorder
        BottomMidBorder
        BottomRightBorder
        LeftBody
        MiddleBody
        RightBody
        LeftFoot
        MiddleFoot
        RightFoot
    End Enum

    Enum MapPrefab
        Undefined = 0
        UpLeftQuarter
        UpBorder
        UpRightQuarter
        RightBorder
        DownRightQuarter
        BottomBorder
        DownLeftQuarter
        LeftBorder
        Common
    End Enum

    Structure DetailRec
        Dim DetailBase As Byte
        Dim Tile As TileRec
    End Structure

    Structure MapOrientationRec
        Dim Prefab As Integer
        Dim TileStartX As Integer
        Dim TileStartY As Integer
        Dim TileEndX As Integer
        Dim TileEndY As Integer
        Dim Tile(,) As TilePrefab
    End Structure

    ''' <summary>
    ''' Loads TilePrefab from the Automapper.ini
    ''' </summary>
    Sub LoadTilePrefab()
        Dim prefab As Integer, layer As Integer

        Dim myXml As New XmlClass With {
            .Filename = Path.Combine(Application.StartupPath, "Data", "AutoMapper.xml"),
            .Root = "Options"
        }

        ReDim Tile(TilePrefab.Count - 1)
        For prefab = 1 To TilePrefab.Count - 1

            ReDim Tile(prefab).Layer(LayerType.Count - 1)
            For layer = 1 To LayerType.Count - 1
                Tile(prefab).Layer(layer).Tileset = Val(myXml.ReadString("Prefab" & prefab, "Layer" & layer & "Tileset"))
                Tile(prefab).Layer(layer).X = Val(myXml.ReadString("Prefab" & prefab, "Layer" & layer & "X"))
                Tile(prefab).Layer(layer).Y = Val(myXml.ReadString("Prefab" & prefab, "Layer" & layer & "Y"))
                Tile(prefab).Layer(layer).AutoTile = Val(myXml.ReadString("Prefab" & prefab, "Layer" & layer & "Autotile"))
            Next layer
            Tile(prefab).Type = Val(myXml.ReadString("Prefab" & prefab, "Type"))
        Next prefab

        ResourcesNum = myXml.ReadString("Resources", "ResourcesNum")
        _resources = Split(ResourcesNum, ";")

    End Sub

    ''' <summary>
    ''' Load details to the rec
    ''' </summary>
    ''' <param name="prefab">Which TilePrefab to use.</param>
    ''' <param name="tileset">Tileset number to use.</param>
    ''' <param name="x">The X coordinate, where the tiles start on the tilesheet.</param>
    ''' <param name="y">The Y coordinate, where the tiles start on the tilesheet.</param>
    ''' <param name="tileType">Which TileType to use, if any, blocked, None, etc</param>
    ''' <param name="endX">The X coordinate, where the tiles end on the tilesheet.</param>
    ''' <param name="endY">The Y coordinate, where the tiles end on the tilesheet.</param>
    Sub LoadDetail(prefab As TilePrefab, tileset As Integer, x As Integer, y As Integer, Optional tileType As Integer = 0, Optional endX As Integer = 0, Optional endY As Integer = 0)
        If EndX = 0 Then EndX = X
        If EndY = 0 Then EndY = Y

        Dim pX As Integer, pY As Integer
        For pX = X To EndX
            For pY = Y To EndY
                AddDetail(Prefab, Tileset, pX, pY, TileType)
            Next
        Next

    End Sub

    ''' <summary>
    ''' Load details to memory for mapping.
    ''' </summary>
    ''' <param name="prefab">Which TilePrefab to use.</param>
    ''' <param name="tileset">Tileset number to use.</param>
    ''' <param name="x">The X coordinate, where the tiles start on the tilesheet.</param>
    ''' <param name="y">The Y coordinate, where the tiles start on the tilesheet.</param>
    ''' <param name="tileType">Which TileType to use, if any, blocked, None, etc.</param>
    Sub AddDetail(prefab As TilePrefab, tileset As Integer, x As Integer, y As Integer, tileType As Integer)
        Dim detailCount As Integer

        detailCount = UBound(Detail) + 1

        ReDim Preserve Detail(detailCount)
        ReDim Preserve Detail(detailCount).Tile.Layer(LayerType.Count - 1)

        Detail(detailCount).DetailBase = Prefab
        Detail(detailCount).Tile.Type = TileType
        Detail(detailCount).Tile.Layer(LayerType.Mask2).Tileset = Tileset
        Detail(detailCount).Tile.Layer(LayerType.Mask2).x = X
        Detail(detailCount).Tile.Layer(LayerType.Mask2).y = Y
    End Sub

    ''' <summary>
    ''' Here a user can define which details to add
    ''' </summary>
    Sub LoadDetails()
        ReDim Detail(1)

        'Detail config area
        'Use: LoadDetail TilePrefab, Tileset, StartTilesetX, StartTilesetY, TileType, EndTilesetX, EndTilesetY
        LoadDetail(TilePrefab.Grass, 9, 0, 0, TileType.None, 9, 15)

        LoadDetail(TilePrefab.Sand, 10, 0, 13, TileType.None, 7, 14)
        LoadDetail(TilePrefab.Sand, 11, 0, 0, TileType.None, 1, 1)
    End Sub

    ''' <summary>
    ''' Check for details
    ''' </summary>
    ''' <param name="prefab"></param>
    ''' <returns></returns>
    Function HaveDetails(prefab As TilePrefab) As Boolean
        HaveDetails = Not (Prefab = TilePrefab.Water OrElse Prefab = TilePrefab.River)
    End Function

    Sub AddTile(prefab As TilePrefab, mapNum as Integer, x As Integer, y As Integer)
        Dim tileDest As TileRec
        Dim cleanNextTiles As Boolean
        Dim i As Integer

        'DetailFreq = Val(frmAutoMapper.txtDetail.Text)

        tileDest = Map(MapNum).Tile(X, Y)
        tileDest.Type = Tile(Prefab).Type

        ReDim Preserve tileDest.Layer(LayerType.Count - 1)

        For i = 1 To LayerType.Count - 1
            If Tile(Prefab).Layer(i).Tileset <> 0 OrElse cleanNextTiles Then
                tileDest.Layer(i) = Tile(Prefab).Layer(i)
                If Not HaveDetails(Prefab) Then cleanNextTiles = True
            End If
        Next i

        If Prefab = TilePrefab.Grass OrElse Prefab = TilePrefab.Sand Then
            If DetailsChecked = True Then
                If Random(1, DetailFreq) = 1 Then
                    Dim detailNum As Integer
                    Dim details() As Integer
                    ReDim details(1)
                    For i = 1 To UBound(Detail)
                        If Detail(i).DetailBase = Prefab Then
                            ReDim Preserve details(UBound(details) + 1)
                            details(UBound(details)) = i
                        End If
                    Next i
                    If UBound(details) > 1 Then
                        detailNum = details(Random(2, UBound(details)))
                        If Detail(detailNum).DetailBase = Prefab Then
                            tileDest.Layer(3) = Detail(detailNum).Tile.Layer(3)
                            tileDest.Type = Detail(detailNum).Tile.Type
                        End If
                    End If
                End If
            End If
        End If

        Map(MapNum).Tile(X, Y) = tileDest
        _mapOrientation(MapNum).Tile(X, Y) = Prefab
    End Sub

    Function CanPlaceResource(mapNum as Integer, X As Integer, Y As Integer) As Boolean
        If _mapOrientation(MapNum).Tile(X, Y) = TilePrefab.Grass OrElse _mapOrientation(MapNum).Tile(X, Y) = TilePrefab.Overgrass OrElse (_mapOrientation(MapNum).Tile(X, Y) = TilePrefab.Mountain AndAlso Not Map(MapNum).Tile(X, Y).Type = TileType.Blocked) Then
            CanPlaceResource = True
        End If
    End Function

    Function CanPlaceOvergrass(mapNum as Integer, X As Integer, Y As Integer) As Boolean
        If _mapOrientation(MapNum).Tile(X, Y) = TilePrefab.Grass OrElse (_mapOrientation(MapNum).Tile(X, Y) = TilePrefab.Mountain AndAlso Not Map(MapNum).Tile(X, Y).Type = TileType.Blocked) Then
            CanPlaceOvergrass = True
        End If
    End Function

    Sub MakeResource(mapNum as Integer)
        Dim x As Integer, y As Integer

        For x = 1 To Map(MapNum).MaxX - 1
            For y = 1 To Map(MapNum).MaxY - 1
                If CanPlaceResource(MapNum, x, y) AndAlso CanPlaceResource(MapNum, x - 1, y) AndAlso CanPlaceResource(MapNum, x + 1, y) AndAlso CanPlaceResource(MapNum, x, y - 1) AndAlso CanPlaceResource(MapNum, x, y + 1) Then
                    Dim resourceNum As Integer

                    If Random(1, ResourceFreq) = 1 Then
                        resourceNum = Val(_resources(Random(1, UBound(_resources))))
                        Map(MapNum).Tile(x, y).Type = TileType.Resource
                        Map(MapNum).Tile(x, y).Data1 = resourceNum
                    End If
                End If
            Next y
        Next x
    End Sub

    Sub MakeResources(mapStart As Integer, size As Integer)
        Dim i As Integer
        Dim totalMaps As Integer
        Dim tick As Integer

        Console.WriteLine("Working...")
        Application.DoEvents()
        tick = GetTimeMs()
        totalMaps = Size * Size

        For i = MapStart To MapStart + totalMaps - 1
            MakeResource(i)
            CacheResources(i)
        Next i

        tick = GetTimeMs() - tick
        Console.WriteLine("Done and cached resources in " & CDbl(tick / 1000) & "s")
        Application.DoEvents()
    End Sub

    Sub MakeOvergrasses(mapStart As Integer, size As Integer)
        Dim i As Integer
        Dim totalMaps As Integer
        Dim tick As Integer

        Console.WriteLine("Working...")
        Application.DoEvents()
        tick = GetTimeMs()
        totalMaps = Size * Size

        For i = MapStart To MapStart + totalMaps - 1
            MakeOvergrass(i)
        Next i

        tick = GetTimeMs() - tick
        Console.WriteLine("Done overgrasses in " & CDbl(tick / 1000) & "s")
        Application.DoEvents()
    End Sub

    Sub MakeOvergrass(mapNum as Integer)
        Dim startX As Integer, startY As Integer
        Dim totalOvergrass As Integer
        'Dim MapSize As Integer
        Dim x As Integer, y As Integer
        Dim grassCount As Integer
        Dim overgrassCount As Integer
        Dim nextDir As Integer
        Dim brushSize As Integer
        Dim foundBorder As Boolean

        With Map(MapNum)
            For x = 0 To .MaxX
                For y = 0 To .MaxY
                    If CanPlaceOvergrass(MapNum, x, y) Then
                        grassCount = grassCount + 1
                    End If
                Next y
            Next x

            totalOvergrass = Random(Int(grassCount / 100), Int(grassCount / 50))

            Do Until overgrassCount >= totalOvergrass
                Dim totalWalk As Integer
                brushSize = Random(1, 2)
                startX = Random(0, .MaxX)
                startY = Random(0, .MaxY)

                If CanPlaceOvergrass(MapNum, startX, startY) Then
                    PaintOvergrass(MapNum, startX, startY, brushSize, brushSize)
                    x = startX
                    y = startY
                    totalWalk = 1
                    nextDir = 0
                    Do While nextDir <> 5 AndAlso totalWalk < 15
                        If foundBorder Then
                            nextDir = Random(0, 5)
                        Else
                            nextDir = Random(0, 4)
                        End If
                        Select Case nextDir
                            Case DirectionType.Up : y = y - 1
                            Case DirectionType.Down : y = y + 1
                            Case DirectionType.Left : x = x - 1
                            Case DirectionType.Right : x = x + 1
                        End Select
                        If nextDir < 5 Then
                            If x > 0 AndAlso x < .MaxX Then
                                If y > 0 AndAlso y < .MaxY Then
                                    If CanPlaceOvergrass(MapNum, x, y) Then
                                        brushSize = Random(0, 2)
                                        PaintOvergrass(MapNum, x, y, brushSize, brushSize)
                                        totalWalk = totalWalk + 1
                                        foundBorder = True
                                    Else
                                        If _mapOrientation(MapNum).Tile(x, y) = TilePrefab.Overgrass Then
                                            foundBorder = False
                                        Else
                                            nextDir = 5
                                        End If
                                    End If
                                Else
                                    nextDir = 5
                                End If
                            Else
                                nextDir = 5
                            End If
                        End If
                    Loop
                    overgrassCount = overgrassCount + 1
                End If
            Loop

        End With

    End Sub

    Sub PaintOvergrass(mapNum as Integer, x As Integer, y As Integer, brushSizeX As Integer, brushSizeY As Integer)
        Dim pX As Integer, pY As Integer

        For pX = X - BrushSizeX To X + BrushSizeX
            For pY = Y - BrushSizeY To Y + BrushSizeY
                If pX >= 0 AndAlso pX <= Map(MapNum).MaxX Then
                    If pY >= 0 AndAlso pY <= Map(MapNum).MaxY Then
                        If _mapOrientation(MapNum).Tile(pX, pY) <> TilePrefab.Overgrass Then
                            If CanPlaceOvergrass(MapNum, pX, pY) Then
                                If Random(1, 100) >= 50 Then
                                    AddTile(TilePrefab.Overgrass, MapNum, pX, pY)
                                End If
                            End If
                        End If
                    End If
                End If
            Next pY
        Next pX
    End Sub

    Sub PaintTile(prefab As TilePrefab, mapNum as Integer, x As Integer, y As Integer, brushSizeX As Integer, brushSizeY As Integer, Optional humanMade As Boolean = True, Optional onlyTo As TilePrefab = 0)
        Dim pX As Integer, pY As Integer
        For pX = X - BrushSizeX To X + BrushSizeX
            For pY = Y - BrushSizeY To Y + BrushSizeY
                If pX >= 0 AndAlso pX <= Map(MapNum).MaxX Then
                    If pY >= 0 AndAlso pY <= Map(MapNum).MaxY Then
                        If _mapOrientation(MapNum).Tile(pX, pY) <> Prefab Then
                            Dim canPaint As Boolean
                            canPaint = True
                            If OnlyTo <> 0 Then
                                If _mapOrientation(MapNum).Tile(pX, pY) <> OnlyTo Then canPaint = False
                            End If
                            If canPaint Then
                                If HumanMade Then
                                    AddTile(Prefab, MapNum, pX, pY)
                                Else
                                    If Random(1, 100) >= 50 Then
                                        AddTile(Prefab, MapNum, pX, pY)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            Next pY
        Next pX
    End Sub

    Sub PaintRiver(mapNum as Integer, X As Integer, Y As Integer, riverDir As Byte, riverWidth As Integer)
        Dim pX As Integer, pY As Integer
        If RiverDir = DirectionType.Down Then
            For pX = X - RiverWidth To X + RiverWidth
                If pX > 0 AndAlso pX < Map(MapNum).MaxX Then
                    AddTile(TilePrefab.River, MapNum, pX, Y)
                End If
            Next pX
        End If
        If RiverDir = DirectionType.Left OrElse RiverDir = DirectionType.Right Then
            For pY = Y - RiverWidth To Y + RiverWidth
                If pY > 0 AndAlso pY < Map(MapNum).MaxY Then
                    AddTile(TilePrefab.River, MapNum, X, pY)
                End If
            Next pY
        End If
    End Sub

    Sub MakeRivers(mapStart As Integer, size As Integer)
        'Dim RiverType As Integer
        Dim riverMap As Integer
        Dim totalRivers As Integer
        Dim totalMaps As Integer
        Dim madeRivers As Integer
        Dim riverX As Integer, riverY As Integer
        Dim riverWidth As Integer, riverHeight As Integer
        Dim riverDir As Byte
        Dim riverBorder As Integer
        Dim riverFlowWidth As Integer
        Dim riverEnd As Boolean
        Dim riverSteps As Integer
        Dim tick As Integer

        Console.WriteLine("Working...")
        Application.DoEvents()
        tick = GetTimeMs()
        riverBorder = 4
        madeRivers = 0
        totalMaps = Size * Size
        totalRivers = Int(totalMaps / 4)

        Do While madeRivers <= totalRivers
            'Start river
SelectMap:
            riverMap = Random(MapStart, MapStart + totalMaps - 1)
            If _mapOrientation(riverMap).Prefab <> MapPrefab.Common Then GoTo SelectMap
            riverX = Random(riverBorder, Map(riverMap).MaxX - riverBorder)
            riverY = Random(riverBorder, Map(riverMap).MaxY - riverBorder)
            riverWidth = Random(2, 3)
            riverHeight = Random(2, 3)
            AddTile(TilePrefab.River, riverMap, riverX, riverY)
            PaintTile(TilePrefab.River, riverMap, riverX, riverY, riverWidth, riverHeight, False)
            riverDir = Random(1, 3)
            riverEnd = False
            riverSteps = 0
            riverFlowWidth = Random(1, 3)
            Do Until _mapOrientation(riverMap).Tile(riverX, riverY) <> TilePrefab.River
                If riverDir = DirectionType.Left Then
                    riverX = riverX - 1
                    If riverX < 0 Then
                        riverX = 0
                        riverDir = DirectionType.Right
                    End If
                End If
                If riverDir = DirectionType.Down Then
                    riverY = riverY + 1
                    If riverY > Map(riverMap).MaxY Then
                        riverY = Map(riverMap).MaxY
                        riverDir = Random(2, 3)
                    End If
                End If
                If riverDir = DirectionType.Right Then
                    riverX = riverX + 1
                    If riverX > Map(riverMap).MaxX Then
                        riverX = Map(riverMap).MaxX
                        riverDir = DirectionType.Left
                    End If
                End If
            Loop
            Do While Not riverEnd
                riverSteps = riverSteps + 1
                If riverX < 0 Then riverX = 0
                If riverX > Map(riverMap).MaxX Then riverX = Map(riverMap).MaxX
                If riverY < 0 Then riverY = 0
                If riverY > Map(riverMap).MaxY Then riverY = Map(riverMap).MaxY
                PaintRiver(riverMap, riverX, riverY, riverDir, riverFlowWidth)
                Select Case riverDir
                    Case DirectionType.Left : riverY = riverY + Random(-1, 1)
                    Case DirectionType.Down : riverX = riverX + Random(-1, 1)
                    Case DirectionType.Right : riverY = riverY + Random(-1, 1)
                End Select

                If Random(1, 100) < 5 Then 'Change dir
                    riverDir = Random(1, 3)
                End If
                Select Case riverDir
                    Case DirectionType.Left : riverX = riverX - 1
                    Case DirectionType.Down : riverY = riverY + 1
                    Case DirectionType.Right : riverX = riverX + 1
                End Select
                If riverDir = DirectionType.Down Then
                    If _mapOrientation(Map(riverMap).Down).Prefab = MapPrefab.Common Then
                        If riverY > Map(riverMap).MaxY Then
                            riverMap = Map(riverMap).Down
                            riverY = 0
                        End If
                    Else
                        If riverY > Map(riverMap).MaxY / 2 Then
                            PaintTile(TilePrefab.River, riverMap, riverX, riverY, Random(2, 3), Random(3, 4), False)
                            riverEnd = True
                        End If
                    End If
                End If
                If riverDir = DirectionType.Left Then
                    If _mapOrientation(Map(riverMap).Left).Prefab = MapPrefab.Common Then
                        If riverX < 0 Then
                            'MapCache_Create RiverMap
                            riverMap = Map(riverMap).Left
                            riverX = Map(riverMap).MaxX
                        End If
                    Else
                        If riverX < Map(riverMap).MaxX / 2 Then
                            PaintTile(TilePrefab.River, riverMap, riverX, riverY, Random(2, 3), Random(3, 4), False)
                            riverEnd = True
                        End If
                    End If
                End If
                If riverDir = DirectionType.Right Then
                    If _mapOrientation(Map(riverMap).Right).Prefab = MapPrefab.Common Then
                        If riverX > Map(riverMap).MaxX Then
                            'MapCache_Create RiverMap
                            riverMap = Map(riverMap).Right
                            riverX = 0
                        End If
                    Else
                        If riverX > Map(riverMap).MaxX / 2 Then
                            PaintTile(TilePrefab.River, riverMap, riverX, riverY, Random(2, 3), Random(3, 4), False)
                            riverEnd = True
                        End If
                    End If
                End If
            Loop
            madeRivers = madeRivers + 1
        Loop

        tick = GetTimeMs() - tick
        Console.WriteLine("Done " & totalRivers & " rivers in " & CDbl(tick / 1000) & "s")
        Application.DoEvents()
    End Sub

    Sub PlaceMountain(mapNum as Integer, x As Integer, y As Integer, mountainPrefab As MountainTile)
        Dim oldX As Integer, oldY As Integer

        oldX = Tile(TilePrefab.Mountain).Layer(2).X
        oldY = Tile(TilePrefab.Mountain).Layer(2).Y
        Tile(TilePrefab.Mountain).Layer(2).X = oldX + (MountainPrefab Mod 3)
        Tile(TilePrefab.Mountain).Layer(2).Y = oldY + (Int(MountainPrefab / 3))
        AddTile(TilePrefab.Mountain, MapNum, X, Y)
        Tile(TilePrefab.Mountain).Layer(2).X = oldX
        Tile(TilePrefab.Mountain).Layer(2).Y = oldY
    End Sub


    Sub MarkMountain(mapNum as Integer, x As Integer, y As Integer, width As Integer, height As Integer)
        Dim pX As Integer, pY As Integer
        For pX = X - Int(Width / 2) To X + Int(Width / 2)
            For pY = Y - Int(Height / 2) To Y + Int(Height / 2)
                If pX > MountainBorder AndAlso pX < Map(MapNum).MaxX - MountainBorder Then
                    If pY > MountainBorder AndAlso pY < Map(MapNum).MaxY - MountainBorder Then
                        _mapOrientation(MapNum).Tile(pX, pY) = TilePrefab.Mountain
                    End If
                End If
            Next pY
        Next pX
    End Sub

    Sub MakeMapMountains(mapNum as Integer)
        Dim mountainMinAreaWidth As Integer, MountainMinAreaHeight As Integer
        Dim mountainMinSize As Integer, MountainMinArea As Integer
        Dim mountainSize As Integer
        Dim x As Integer, y As Integer
        'Dim ScanX As Integer, ScanY As Integer
        'Dim FoundPlace As Boolean
        Dim totalGrass As Integer
        Dim totalMountains As Integer
        Dim i As Integer
        Dim positionTries As Integer
        Dim mountainSteps As Integer
        mountainMinAreaWidth = 5
        MountainMinAreaHeight = 5
        MountainMinArea = 4
        mountainMinSize = 10

        For x = MountainBorder To Map(MapNum).MaxX - MountainBorder
            For y = MountainBorder To Map(MapNum).MaxY - MountainBorder
                If _mapOrientation(MapNum).Tile(x, y) = TilePrefab.Grass Then
                    totalGrass = totalGrass + 1
                End If
            Next y
        Next x

        totalMountains = Random(0, (totalGrass / (mountainMinAreaWidth * MountainMinAreaHeight)))

        If totalMountains > 0 Then
            For i = 1 To totalMountains
                positionTries = 0
Retry:
                If positionTries < 5 Then
                    x = Random(mountainMinAreaWidth + MountainBorder, Map(MapNum).MaxX - mountainMinAreaWidth - MountainBorder)
                    y = Random(MountainMinAreaHeight + MountainBorder, Map(MapNum).MaxY - MountainMinAreaHeight - MountainBorder)
                    If Not ValidMountainPosition(MapNum, x, y, mountainMinAreaWidth, MountainMinAreaHeight) Then
                        positionTries = positionTries + 1
                        GoTo Retry
                    End If
                    MarkMountain(MapNum, x, y, mountainMinAreaWidth, MountainMinAreaHeight)

                    mountainSteps = 0
                    mountainSize = Random(mountainMinSize, mountainMinSize * (totalMountains / i))

                    Do While mountainSteps < mountainSize
                        Dim OldX As Integer, OldY As Integer
                        OldX = x
                        OldY = y
                        x = x + (Random(0, 2) - 1)
                        y = y + (Random(0, 2) - 1)
                        If ValidMountainPosition(MapNum, x, y, 3, 5) Then
                            MarkMountain(MapNum, x, y, 3, 5)
                        Else
                            'Return
                            x = OldX
                            y = OldY
                        End If
                        mountainSteps = mountainSteps + 1
                    Loop

                End If
            Next i

            'Fill Mountain
            For x = MountainBorder To Map(MapNum).MaxX - MountainBorder
                For y = MountainBorder To Map(MapNum).MaxY - MountainBorder
                    If _mapOrientation(MapNum).Tile(x, y) = TilePrefab.Mountain Then
                        Dim mountainPrefab As MountainTile
                        mountainPrefab = GetMountainPrefab(MapNum, x, y)
                        'Exceptions
                        If Not _mapOrientation(MapNum).Tile(x, y - 1) <> TilePrefab.Mountain Then
                            If ((GetMountainPrefab(MapNum, x - 1, y) = MountainTile.MiddleFoot OrElse GetMountainPrefab(MapNum, x - 1, y) = MountainTile.LeftFoot) OrElse (GetMountainPrefab(MapNum, x - 1, y) = MountainTile.LeftBody OrElse GetMountainPrefab(MapNum, x - 1, y) = MountainTile.MiddleBody)) AndAlso Not (mountainPrefab = MountainTile.MiddleBody OrElse mountainPrefab = MountainTile.MiddleFoot OrElse mountainPrefab = MountainTile.RightBody OrElse mountainPrefab = MountainTile.RightFoot) Then
                                mountainPrefab = MountainTile.MidLeftBorder
                            End If
                            If GetMountainPrefab(MapNum, x, y + 1) = MountainTile.LeftFoot Then
                                mountainPrefab = MountainTile.LeftBody
                                GoTo Important
                            End If
                            If GetMountainPrefab(MapNum, x, y + 2) = MountainTile.LeftFoot Then
                                mountainPrefab = MountainTile.BottomLeftBorder
                                GoTo Important
                            End If
                            If ((GetMountainPrefab(MapNum, x + 1, y) = MountainTile.MiddleFoot OrElse GetMountainPrefab(MapNum, x + 1, y) = MountainTile.RightFoot) OrElse (GetMountainPrefab(MapNum, x + 1, y) = MountainTile.RightBody OrElse GetMountainPrefab(MapNum, x + 1, y) = MountainTile.MiddleBody)) AndAlso Not (mountainPrefab = MountainTile.MiddleBody OrElse mountainPrefab = MountainTile.MiddleFoot OrElse mountainPrefab = MountainTile.LeftBody OrElse mountainPrefab = MountainTile.LeftFoot) Then
                                mountainPrefab = MountainTile.MidRightBorder
                            End If
                            If GetMountainPrefab(MapNum, x, y + 1) = MountainTile.RightFoot Then
                                mountainPrefab = MountainTile.RightBody
                                GoTo Important
                            End If
                            If GetMountainPrefab(MapNum, x, y + 2) = MountainTile.RightFoot Then
                                mountainPrefab = MountainTile.BottomRightBorder
                                GoTo Important
                            End If
                        End If

Important:
                        If mountainPrefab >= 0 Then PlaceMountain(MapNum, x, y, mountainPrefab)
                    End If
                Next y
            Next x
        End If
    End Sub

    Function GetMountainPrefab(mapNum as Integer, x As Integer, y As Integer) As MountainTile
        Dim verticalPos As Byte
        Dim mountainPrefab As MountainTile
        If _mapOrientation(MapNum).Tile(X, Y) = TilePrefab.Mountain Then
            verticalPos = 1
            If _mapOrientation(MapNum).Tile(X - 1, Y) <> TilePrefab.Mountain Then
                verticalPos = 0
            End If
            If _mapOrientation(MapNum).Tile(X + 1, Y) <> TilePrefab.Mountain Then
                verticalPos = 2
            End If
            mountainPrefab = -1
            If _mapOrientation(MapNum).Tile(X, Y - 1) = TilePrefab.Mountain Then
                'Its not the top
                If Y + 3 < Map(MapNum).MaxY Then
                    If _mapOrientation(MapNum).Tile(X, Y + 3) <> TilePrefab.Mountain AndAlso _mapOrientation(MapNum).Tile(X, Y + 2) = TilePrefab.Mountain Then
                        'Inferior
                        Select Case verticalPos
                            Case 0 : mountainPrefab = MountainTile.BottomLeftBorder
                            Case 1 : mountainPrefab = MountainTile.BottomMidBorder
                            Case 2 : mountainPrefab = MountainTile.BottomRightBorder
                        End Select
                    Else
                        If _mapOrientation(MapNum).Tile(X, Y + 2) <> TilePrefab.Mountain AndAlso _mapOrientation(MapNum).Tile(X, Y + 1) = TilePrefab.Mountain Then
                            'Body
                            Select Case verticalPos
                                Case 0 : mountainPrefab = MountainTile.LeftBody
                                Case 1 : mountainPrefab = MountainTile.MiddleBody
                                Case 2 : mountainPrefab = MountainTile.RightBody
                            End Select
                        Else
                            If _mapOrientation(MapNum).Tile(X, Y + 1) <> TilePrefab.Mountain Then
                                'Foots
                                Select Case verticalPos
                                    Case 0 : mountainPrefab = MountainTile.LeftFoot
                                    Case 1 : mountainPrefab = MountainTile.MiddleFoot
                                    Case 2 : mountainPrefab = MountainTile.RightFoot
                                End Select
                            Else
                                'Mid
                                Select Case verticalPos
                                    Case 0 : mountainPrefab = MountainTile.MidLeftBorder
                                    Case 2 : mountainPrefab = MountainTile.MidRightBorder
                                End Select
                            End If
                        End If
                    End If
                End If
            Else
                'Its top
                Select Case verticalPos
                    Case 0 : mountainPrefab = MountainTile.UpLeftBorder
                    Case 1 : mountainPrefab = MountainTile.UpMidBorder
                    Case 2 : mountainPrefab = MountainTile.UpRightBorder
                End Select
            End If
            GetMountainPrefab = mountainPrefab
        Else
            GetMountainPrefab = -1
        End If
    End Function

    Function ValidMountainPosition(mapNum as Integer, x As Integer, y As Integer, width As Integer, height As Integer) As Boolean
        Dim pX As Integer, pY As Integer
        ValidMountainPosition = True
        For pX = X - Int(Width / 2) To X + Int(Width / 2)
            For pY = Y - Int(Height / 2) To Y + Int(Height / 2)
                If pX < 1 OrElse pX > Map(MapNum).MaxX - 1 Then ValidMountainPosition = False
                If pY < 1 OrElse pY >= Map(MapNum).MaxY - 3 Then ValidMountainPosition = False
                If ValidMountainPosition Then
                    If _mapOrientation(MapNum).Tile(pX, pY) <> TilePrefab.Grass AndAlso _mapOrientation(MapNum).Tile(pX, pY) <> TilePrefab.Overgrass AndAlso _mapOrientation(MapNum).Tile(pX, pY) <> TilePrefab.Mountain Then ValidMountainPosition = False
                End If
            Next pY
        Next pX
    End Function

    Sub MakeMountains(mapStart As Integer, size As Integer)
        Dim i As Integer
        Dim totalMaps As Integer
        Dim tick As Integer
        Dim mapCount As Integer
        Console.WriteLine("Working...")
        Application.DoEvents()
        tick = GetTimeMs()
        totalMaps = Size * Size
        mapCount = 0
        For i = MapStart To MapStart + totalMaps - 1
            If _mapOrientation(i).Prefab = MapPrefab.Common Then
                MakeMapMountains(i)
                mapCount = mapCount + 1
            End If
        Next i
        tick = GetTimeMs() - tick
        Console.WriteLine("Done mountains in " & (mapCount) & " maps in " & CDbl(tick / 1000) & "s")
        Application.DoEvents()
    End Sub

    Sub MakeMap(mapNum as Integer, prefab As MapPrefab)
        Dim x As Integer, y As Integer
        Dim tileX As Integer, tileY As Integer
        Dim tileStartX As Integer, tileStartY As Integer
        Dim tileEndX As Integer, tileEndY As Integer
        Dim change As Integer
        Dim changed As Boolean

        _mapOrientation(MapNum).Prefab = Prefab

        With Map(MapNum)
            If Prefab <> MapPrefab.Common Then
                For x = 0 To .MaxX
                    For y = 0 To .MaxY
                        AddTile(TilePrefab.Water, MapNum, x, y)
                    Next y
                Next x
            Else
                For x = 0 To .MaxX
                    For y = 0 To .MaxY
                        AddTile(TilePrefab.Grass, MapNum, x, y)
                    Next y
                Next x
            End If
            If Prefab = MapPrefab.UpLeftQuarter Then
                tileStartX = Int(.MaxX / 2) - Random(0, Int(.MaxX / 4))
                tileStartY = .MaxY
                tileEndX = .MaxX
                tileEndY = Int(.MaxY / 2) - Random(0, Int(.MaxY / 4))
                tileX = tileStartX

                For y = tileStartY To tileEndY Step -1
                    If y <> tileStartY Then tileX = tileX + Random(0, 2)
                    If tileX >= tileEndX Then
                        tileEndY = y
                        Exit For
                    Else
                        For x = tileX To tileEndX
                            If x < tileX + SandBorder OrElse y < tileEndY + SandBorder Then
                                AddTile(TilePrefab.Sand, MapNum, x, y)
                            Else
                                AddTile(TilePrefab.Grass, MapNum, x, y)
                            End If
                        Next x
                    End If
                Next y
            End If
            If Prefab = MapPrefab.UpBorder Then
                tileStartX = 0
                tileStartY = _mapOrientation(.Left).TileEndY
                tileEndX = .MaxX
                tileY = tileStartY
                changed = True
                For x = tileStartX To tileEndX
                    If changed = False Then
                        change = Random(-1, 1)
                        If change <> 0 Then
                            changed = True
                            tileY = tileY + change
                        End If
                    Else
                        changed = False
                    End If
                    If tileY < Int(.MaxY / 4) Then tileY = Int(.MaxY / 4)
                    If tileY > Int(.MaxY / 2) + Int(.MaxY / 4) Then tileY = Int(.MaxY / 2) + Int(.MaxY / 4)
                    For y = tileY To .MaxY
                        If y < tileY + SandBorder Then
                            AddTile(TilePrefab.Sand, MapNum, x, y)
                        Else
                            AddTile(TilePrefab.Grass, MapNum, x, y)
                        End If
                    Next y
                Next x

                _mapOrientation(MapNum).TileEndY = tileY
            End If
            If Prefab = MapPrefab.UpRightQuarter Then
                tileStartX = Random(4, 8)
                tileStartY = _mapOrientation(.Left).TileEndY
                tileEndX = 0
                tileEndY = .MaxY

                tileX = tileStartX
                For y = tileStartY To tileEndY
                    If y <> tileStartY Then tileX = tileX + Random(0, 2)
                    If tileX > .MaxX Then tileX = .MaxX
                    For x = tileX To tileEndX Step -1
                        If x > tileX - SandBorder OrElse y < tileY + SandBorder Then
                            AddTile(TilePrefab.Sand, MapNum, x, y)
                        Else
                            AddTile(TilePrefab.Grass, MapNum, x, y)
                        End If
                    Next x
                Next y

                tileEndX = tileX
            End If
            If Prefab = MapPrefab.LeftBorder Then
                If .Up = MapStart Then
                    tileStartX = _mapOrientation(.Up).TileStartX
                Else
                    tileStartX = _mapOrientation(.Up).TileEndX
                End If
                tileStartY = 0
                tileEndX = .MaxX
                tileEndY = .MaxY
                tileX = tileStartX
                changed = True
                For y = tileStartY To tileEndY
                    If changed = False Then
                        change = Random(-1, 1)
                        If change <> 0 Then
                            changed = True
                            tileX = tileX + change
                        End If
                    Else
                        changed = False
                    End If
                    If tileX < Int(.MaxX / 4) Then tileX = Int(.MaxX / 4)
                    If tileX > Int(.MaxX / 2) + Int(.MaxX / 4) Then tileX = Int(.MaxX / 2) + Int(.MaxX / 4)
                    For x = tileX To tileEndX
                        If x < tileX + SandBorder Then
                            AddTile(TilePrefab.Sand, MapNum, x, y)
                        Else
                            AddTile(TilePrefab.Grass, MapNum, x, y)
                        End If
                    Next x
                Next y

                _mapOrientation(MapNum).TileEndX = tileX
            End If
            If Prefab = MapPrefab.RightBorder Then
                If .Up = MapStart Then
                    tileStartX = _mapOrientation(.Up).TileStartX
                Else
                    tileStartX = _mapOrientation(.Up).TileEndX
                End If
                tileStartY = 0
                tileEndX = .MaxX
                tileEndY = .MaxY
                tileX = tileStartX
                changed = True
                For y = tileStartY To tileEndY
                    If changed = False Then
                        change = Random(-1, 1)
                        If change <> 0 Then
                            changed = True
                            tileX = tileX + change
                        End If
                    Else
                        changed = False
                    End If
                    If tileX < Int(.MaxX / 4) Then tileX = Int(.MaxX / 4)
                    If tileX > Int(.MaxX / 2) + Int(.MaxX / 4) Then tileX = Int(.MaxX / 2) + Int(.MaxX / 4)
                    For x = tileX To 0 Step -1
                        If x > tileX - SandBorder Then
                            AddTile(TilePrefab.Sand, MapNum, x, y)
                        Else
                            AddTile(TilePrefab.Grass, MapNum, x, y)
                        End If
                    Next x
                Next y

                _mapOrientation(MapNum).TileEndX = tileX
            End If
            If Prefab = MapPrefab.DownLeftQuarter Then
                tileStartX = _mapOrientation(.Up).TileEndX
                tileEndX = .MaxX
                tileStartY = 0
                tileEndY = Int(.MaxY / 2) + Random(0, Int(.MaxY / 4))

                tileX = tileStartX
                For y = tileStartY To tileEndY
                    If y <> tileStartY Then tileX = tileX + Random(0, 2)
                    If tileX >= tileEndX Then
                        tileEndY = y
                        Exit For
                    Else
                        For x = tileX To tileEndX
                            If x < tileX + SandBorder OrElse y > tileEndY - SandBorder Then
                                AddTile(TilePrefab.Sand, MapNum, x, y)
                            Else
                                AddTile(TilePrefab.Grass, MapNum, x, y)
                            End If
                        Next x
                    End If
                Next y
            End If
            If Prefab = MapPrefab.BottomBorder Then
                tileStartX = 0
                tileEndX = .MaxX
                tileStartY = _mapOrientation(.Left).TileEndY

                tileY = tileStartY
                changed = True
                For x = tileStartX To tileEndX
                    If changed = False Then
                        change = Random(-1, 1)
                        If change <> 0 Then
                            changed = True
                            tileY = tileY + change
                        End If
                    Else
                        changed = False
                    End If
                    If tileY < Int(.MaxY / 4) Then tileY = Int(.MaxY / 4)
                    If tileY > Int(.MaxY / 2) + Int(.MaxY / 4) Then tileY = Int(.MaxY / 2) + Int(.MaxY / 4)
                    For y = tileY To 0 Step -1
                        If y > tileY - SandBorder Then
                            AddTile(TilePrefab.Sand, MapNum, x, y)
                        Else
                            AddTile(TilePrefab.Grass, MapNum, x, y)
                        End If
                    Next y
                Next x

                _mapOrientation(MapNum).TileEndY = tileY
            End If
            If Prefab = MapPrefab.DownRightQuarter Then
                tileStartY = _mapOrientation(.Left).TileEndY
                tileEndY = 0
                tileStartX = 0
                tileEndX = _mapOrientation(.Up).TileEndX
                tileY = tileStartY

                For x = tileStartX To tileEndX
                    If x <> tileStartX Then tileY = tileY - Random(0, 1)
                    For y = tileY To tileEndY Step -1
                        If y > tileY - SandBorder OrElse x > tileEndX - SandBorder Then
                            AddTile(TilePrefab.Sand, MapNum, x, y)
                        Else
                            AddTile(TilePrefab.Grass, MapNum, x, y)
                        End If
                    Next y
                Next x
            End If
        End With

        If _mapOrientation(MapNum).TileStartX = 0 Then _mapOrientation(MapNum).TileStartX = tileStartX
        If _mapOrientation(MapNum).TileStartY = 0 Then _mapOrientation(MapNum).TileStartY = tileStartY
        If _mapOrientation(MapNum).TileEndX = 0 Then _mapOrientation(MapNum).TileEndX = tileEndX
        If _mapOrientation(MapNum).TileEndY = 0 Then _mapOrientation(MapNum).TileEndY = tileEndY

    End Sub

    Sub MakePath(mapNum as Integer, x As Integer, y As Integer, dir As Byte, Optional steps As Integer = 1)
        Dim pathEnd As Boolean
        Dim brushX As Integer, brushY As Integer
        Dim i As Byte
        If Not _mapOrientation(MapNum).Prefab = MapPrefab.Common Then Exit Sub
        pathEnd = False
        Do While Not pathEnd
            If Steps Mod Map(MapNum).MaxX = 0 Then
                Dim oldDir As Integer
                oldDir = Dir
ChangeDir:
                Dir = Random(0, 3)
                'Avoid invert direction
                If (oldDir = DirectionType.Up AndAlso Dir = DirectionType.Down) OrElse (oldDir = DirectionType.Down AndAlso Dir = DirectionType.Up) OrElse (oldDir = DirectionType.Right AndAlso Dir = DirectionType.Left) OrElse (oldDir = DirectionType.Left AndAlso Dir = DirectionType.Right) Then GoTo ChangeDir
            End If
            Select Case Dir
                Case DirectionType.Up
                    brushX = 1
                    brushY = 0
                    Y = Y - 1
                    X = X + Random(0, 2) - 1
                    If X <= 1 Then X = 1
                    If X >= Map(MapNum).MaxX - 1 Then X = Map(MapNum).MaxX - 1
                Case DirectionType.Down
                    brushX = 1
                    brushY = 0
                    Y = Y + 1
                    X = X + Random(0, 2) - 1
                    If X <= 1 Then X = 1
                    If X >= Map(MapNum).MaxX - 1 Then X = Map(MapNum).MaxX - 1
                Case DirectionType.Left
                    brushX = 0
                    brushY = 1
                    Y = Y + Random(0, 2) - 1
                    X = X - 1
                    If Y <= 1 Then Y = 1
                    If Y >= Map(MapNum).MaxY - 1 Then Y = Map(MapNum).MaxY - 1
                Case DirectionType.Right
                    brushX = 0
                    brushY = 1
                    Y = Y + Random(0, 2) - 1
                    X = X + 1
                    If Y <= 1 Then Y = 1
                    If Y >= Map(MapNum).MaxY - 1 Then Y = Map(MapNum).MaxY - 1
            End Select
            If X <= 0 Then
                If Map(MapNum).Left > 0 Then
                    If _mapOrientation(Map(MapNum).Left).Prefab = MapPrefab.Common Then
                        PaintTile(TilePrefab.Passing, MapNum, X, Y, brushX, brushY, , TilePrefab.Grass)
                        PaintTile(TilePrefab.Passing, Map(MapNum).Left, Val(Map(MapNum).MaxX), Y, brushX, brushY, , TilePrefab.Grass)
                        MakePath(Map(MapNum).Left, Map(MapNum).MaxX, Y, Dir, Steps)
                    End If
                End If
                Exit Sub
            End If
            If X >= Map(MapNum).MaxX Then
                If Map(MapNum).Right > 0 Then
                    If _mapOrientation(Map(MapNum).Right).Prefab = MapPrefab.Common Then
                        PaintTile(TilePrefab.Passing, MapNum, X, Y, brushX, brushY, , TilePrefab.Grass)
                        PaintTile(TilePrefab.Passing, Map(MapNum).Right, 0, Y, brushX, brushY, , TilePrefab.Grass)
                        MakePath(Map(MapNum).Right, 0, Y, Dir, Steps)
                    End If
                End If
                Exit Sub
            End If
            If Y <= 0 Then
                If Map(MapNum).Up > 0 Then
                    If _mapOrientation(Map(MapNum).Up).Prefab = MapPrefab.Common Then
                        PaintTile(TilePrefab.Passing, MapNum, X, Y, brushX, brushY, , TilePrefab.Grass)
                        PaintTile(TilePrefab.Passing, Map(MapNum).Up, X, Val(Map(MapNum).MaxY), brushX, brushY, , TilePrefab.Grass)
                        MakePath(Map(MapNum).Up, X, Map(MapNum).MaxY, Dir, Steps)
                    End If
                End If
                Exit Sub
            End If
            If Y >= Map(MapNum).MaxY Then
                If Map(MapNum).Down > 0 Then
                    If _mapOrientation(Map(MapNum).Down).Prefab = MapPrefab.Common Then
                        PaintTile(TilePrefab.Passing, MapNum, X, Y, brushX, brushY, , TilePrefab.Grass)
                        PaintTile(TilePrefab.Passing, Map(MapNum).Down, X, 0, brushX, brushY, , TilePrefab.Grass)
                        MakePath(Map(MapNum).Down, X, 0, Dir, Steps)
                    End If
                End If
                Exit Sub
            End If

            If CheckPath(MapNum, X, Y, Dir) Then
                PaintTile(TilePrefab.Passing, MapNum, X, Y, brushX, brushY, , TilePrefab.Grass)
                Steps = Steps + 1
            Else
                For i = 0 To 3
                    If i <> Dir Then
                        If CheckPath(MapNum, X, Y, i) Then
                            Dir = i
                            Exit For
                        End If
                    End If
                Next i
            End If
        Loop
    End Sub

    Function CheckPath(mapNum as Integer, X As Integer, Y As Integer, Dir As Byte) As Boolean
        Dim sizeX As Integer, sizeY As Integer
        Select Case Dir
            Case DirectionType.Up, DirectionType.Down : sizeX = 1
            Case DirectionType.Left, DirectionType.Right : sizeY = 1
        End Select

        CheckPath = True

        Dim pX As Integer, pY As Integer
        For pX = X - sizeX To X + sizeX
            For pY = Y - sizeY To Y + sizeY
                If pX >= 0 AndAlso pX <= Map(MapNum).MaxX Then
                    If pY >= 0 AndAlso pY <= Map(MapNum).MaxY Then
                        If _mapOrientation(MapNum).Tile(pX, pY) <> TilePrefab.Grass AndAlso _mapOrientation(MapNum).Tile(pX, pY) <> TilePrefab.Passing Then
                            CheckPath = False
                            Exit Function
                        End If
                    End If
                End If
            Next pY
        Next pX
    End Function

    Function SearchForPreviousPaths(mapNum as Integer, X As Integer, Y As Integer) As Boolean
        Dim pX As Integer, pY As Integer
        For pX = X - 10 To X + 10
            For pY = Y - 10 To Y + 10
                If pX >= 0 AndAlso pX <= Map(MapNum).MaxX Then
                    If pY >= 0 AndAlso pY <= Map(MapNum).MaxY Then
                        If _mapOrientation(MapNum).Tile(pX, pY) = TilePrefab.Passing Then
                            SearchForPreviousPaths = True
                            Exit Function
                        End If
                    End If
                End If
            Next pY
        Next pX
    End Function

    Sub MakeMapPaths(mapNum as Integer)
        Dim x As Integer, y As Integer
        Dim startX() As Integer = {0}, startY() As Integer = {0}
        Dim locationCount As Integer
        Dim totalPaths As Integer
        Dim maxTries As Integer
        Dim tries As Integer
        Dim tick As Integer

        Console.WriteLine("Working...")
        Application.DoEvents()
        tick = GetTimeMs()

        maxTries = 30
        totalPaths = Random(Map(MapNum).MaxX / 20, Map(MapNum).MaxX / 10)

        Do While locationCount < totalPaths AndAlso tries < maxTries
            x = Random(1, Map(MapNum).MaxX - 1)
            y = Random(1, Map(MapNum).MaxY - 1)
            If _mapOrientation(MapNum).Tile(x, y) = TilePrefab.Grass AndAlso _mapOrientation(MapNum).Tile(x + 1, y) = TilePrefab.Grass AndAlso _mapOrientation(MapNum).Tile(x, y + 1) = TilePrefab.Grass AndAlso _mapOrientation(MapNum).Tile(x + 1, y + 1) = TilePrefab.Grass Then
                If Not SearchForPreviousPaths(MapNum, x, y) Then
                    PaintTile(TilePrefab.Passing, MapNum, x, y, 1, 1, , TilePrefab.Grass)
                    locationCount = locationCount + 1
                    ReDim Preserve startX(locationCount)
                    ReDim Preserve startY(locationCount)
                    startX(locationCount) = x
                    startY(locationCount) = y
                End If
            End If
            tries = tries + 1
        Loop

        If locationCount > 0 Then
            Dim i As Integer
            Dim dir As Integer
            For i = 1 To locationCount
                If startX(i) < Map(MapNum).MaxX / 2 Then
                    If startY(i) < Map(MapNum).MaxY / 2 Then
                        If Random(1, 2) = 1 Then
                            dir = DirectionType.Left
                        Else
                            dir = DirectionType.Up
                        End If
                    Else
                        If Random(1, 2) = 1 Then
                            dir = DirectionType.Left
                        Else
                            dir = DirectionType.Down
                        End If
                    End If
                Else
                    If startY(i) < Map(MapNum).MaxY / 2 Then
                        If Random(1, 2) = 1 Then
                            dir = DirectionType.Right
                        Else
                            dir = DirectionType.Up
                        End If
                    Else
                        If Random(1, 2) = 1 Then
                            dir = DirectionType.Right
                        Else
                            dir = DirectionType.Down
                        End If
                    End If
                End If
                MakePath(MapNum, startX(i) + 1, startY(i), dir)
            Next i
        End If

        tick = GetTimeMs() - tick
        Console.WriteLine("Done " & totalPaths & " paths in " & CDbl(tick / 1000) & "s")
        Application.DoEvents()
    End Sub

    Sub MakePaths(mapStart As Integer, size As Integer)
        Dim totalMaps As Integer  = Size * Size

        If totalMaps Mod 2 = 1 Then
            MakeMapPaths(Int(totalMaps / 2) + 1)
        Else
            MakeMapPaths(Int(totalMaps / 2) - (Size / 2))
            MakeMapPaths(Int(totalMaps / 2) - (Size / 2) + 1)
            MakeMapPaths(Int(totalMaps / 2) - (Size / 2) + Size)
            MakeMapPaths(Int(totalMaps / 2) - (Size / 2) + Size + 1)
        End If

    End Sub

    Sub StartAutomapper(mapStart As Integer, size As Integer, mapX As Integer, mapY As Integer)
        Dim startTick As Integer
        Dim tick As Integer
        startTick = GetTimeMs()
        LoadTilePrefab()
        LoadDetails()

        Dim mapNum as Integer
        Dim totalMaps As Integer = (Size * Size)

        ReDim _mapOrientation(MapStart + totalMaps)

        tick = GetTimeMs()

        For mapnum = MapStart To MapStart + totalMaps - 1
            ClearMap(mapnum)

            Map(mapnum).MaxX = MapX
            Map(mapnum).MaxY = MapY
            ReDim Map(mapnum).Tile(Map(mapnum).MaxX,Map(mapnum).MaxY)
            ReDim _mapOrientation(mapnum).Tile(Map(mapnum).MaxX,Map(mapnum).MaxY)
            ClearTempTile(mapnum)

            ' // Down teleport \\
            If mapnum <= MapStart - 1 + totalMaps - Size Then
                Map(mapnum).Down = mapnum + Size
            End If
            ' \\ Down teleport //

            ' // Left teleport \\
            If mapnum - MapStart + 1 Mod Size <> 1 Then
                Map(mapnum).Left = mapnum - 1
            End If
            ' \\ Left teleport //

            ' // Up teleport \\
            If mapnum - MapStart + 1 > Size Then
                Map(mapnum).Up = mapnum - Size
            End If
            ' \\ Up teleport //

            ' // Right teleport \\
            If mapnum - MapStart + 1 Mod Size <> 0 Then
                Map(mapnum).Right = mapnum + 1
            End If
            ' \\ Right teleport //

            Dim Prefab As MapPrefab
            Prefab = MapPrefab.Undefined
            If mapnum = MapStart Then
                Prefab = MapPrefab.UpLeftQuarter
            End If
            If mapnum > MapStart AndAlso mapnum < MapStart - 1 + Size Then
                Prefab = MapPrefab.UpBorder
            End If
            If mapnum = MapStart - 1 + Size Then
                Prefab = MapPrefab.UpRightQuarter
            End If
            If mapnum > MapStart - 1 + Size AndAlso mapnum <= MapStart - 1 + totalMaps - Size Then
                If (mapnum - MapStart + 1) Mod Size = 1 Then
                    Prefab = MapPrefab.LeftBorder
                Else
                    If (mapnum - MapStart + 1) Mod Size = 0 Then
                        Prefab = MapPrefab.RightBorder
                    Else
                        Prefab = MapPrefab.Common
                    End If
                End If
            End If
            If mapnum > MapStart - 1 + totalMaps - Size Then
                If (mapnum - MapStart + 1) Mod Size = 1 Then
                    Prefab = MapPrefab.DownLeftQuarter
                Else
                    If (mapnum - MapStart + 1) Mod Size = 0 Then
                        Prefab = MapPrefab.DownRightQuarter
                    Else
                        Prefab = MapPrefab.BottomBorder
                    End If
                End If
            End If

            MakeMap(mapnum, Prefab)
        Next mapnum

        tick = GetTimeMs() - tick
        Console.WriteLine("Done " & totalMaps & " maps models in " & CDbl(tick / 1000) & "s")
        Application.DoEvents()

        If PathsChecked = True Then MakePaths(MapStart, Size)
        If RiversChecked = True Then MakeRivers(MapStart, Size)
        If MountainsChecked = True Then MakeMountains(MapStart, Size)
        If OverGrassChecked = True Then MakeOvergrasses(MapStart, Size)
        If ResourcesChecked = True Then MakeResources(MapStart, Size)

        tick = GetTimeMs()
        Console.WriteLine("Working...")
        Application.DoEvents()

        For mapnum = MapStart To MapStart + totalMaps - 1
            SaveMap(mapnum)
            'MapCache_Create mapnum
        Next mapnum

        tick = GetTimeMs() - tick
        startTick = GetTimeMs() - startTick

        Console.WriteLine("Cached all maps in " & CDbl(tick / 1000) & "s (" & ((tick / startTick) * 100) & "%)")
        Console.WriteLine("Done " & totalMaps & " maps in " & CDbl(startTick / 1000) & "s")

    End Sub

End Module
