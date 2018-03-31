Imports System.IO

Module modEditors

#Region "Animation Editor"
    Friend Sub AnimationEditorInit()

        If frmAnimation.Visible = False Then Exit Sub

        EditorIndex = frmAnimation.lstIndex.SelectedIndex + 1

        With Animation(EditorIndex)

            ' find the music we have set
            frmAnimation.cmbSound.Items.Clear()
            frmAnimation.cmbSound.Items.Add("None")

            If UBound(SoundCache) > 0 Then
                For i = 1 To UBound(SoundCache)
                    frmAnimation.cmbSound.Items.Add(SoundCache(i))
                Next
            End If

            If Trim$(Animation(EditorIndex).Sound) = "None" OrElse Trim$(Animation(EditorIndex).Sound) = "" Then
                frmAnimation.cmbSound.SelectedIndex = 0
            Else
                For i = 1 To frmAnimation.cmbSound.Items.Count
                    If frmAnimation.cmbSound.Items(i - 1).ToString = Trim$(.Sound) Then
                        frmAnimation.cmbSound.SelectedIndex = i - 1
                        Exit For
                    End If
                Next
            End If
            frmAnimation.txtName.Text = Trim$(.Name)

            frmAnimation.nudSprite0.Value = .Sprite(0)
            frmAnimation.nudFrameCount0.Value = .Frames(0)
            frmAnimation.nudLoopCount0.Value = .LoopCount(0)
            frmAnimation.nudLoopTime0.Value = .LoopTime(0)

            frmAnimation.nudSprite1.Value = .Sprite(1)
            frmAnimation.nudFrameCount1.Value = .Frames(1)
            frmAnimation.nudLoopCount1.Value = .LoopCount(1)
            frmAnimation.nudLoopTime1.Value = .LoopTime(1)

            EditorIndex = frmAnimation.lstIndex.SelectedIndex + 1
        End With

        EditorAnim_DrawAnim()
        Animation_Changed(EditorIndex) = True
    End Sub

    Friend Sub AnimationEditorOk()
        Dim i As Integer

        For i = 1 To MAX_ANIMATIONS
            If Animation_Changed(i) Then
                SendSaveAnimation(i)
            End If
        Next

        frmAnimation.Visible = False
        Editor = 0
        ClearChanged_Animation()
    End Sub

    Friend Sub AnimationEditorCancel()
        Editor = 0
        frmAnimation.Visible = False
        ClearChanged_Animation()
        ClearAnimations()
        SendRequestAnimations()
    End Sub

    Friend Sub ClearChanged_Animation()
        For i = 0 To MAX_ANIMATIONS
            Animation_Changed(i) = False
        Next
    End Sub

#End Region

#Region "Map Editor"

    Friend Sub MapPropertiesInit()
        Dim X As Integer, Y As Integer, i As Integer

        frmMapEditor.txtName.Text = Trim$(Map.Name)

        ' find the music we have set
        frmMapEditor.lstMusic.Items.Clear()
        frmMapEditor.lstMusic.Items.Add("None")

        If UBound(MusicCache) > 0 Then
            For i = 1 To UBound(MusicCache)
                frmMapEditor.lstMusic.Items.Add(MusicCache(i))
            Next
        End If

        If Trim$(Map.Music) = "None" Then
            frmMapEditor.lstMusic.SelectedIndex = 0
        Else
            For i = 1 To frmMapEditor.lstMusic.Items.Count
                If frmMapEditor.lstMusic.Items(i - 1).ToString = Trim$(Map.Music) Then
                    frmMapEditor.lstMusic.SelectedIndex = i - 1
                    Exit For
                End If
            Next
        End If

        ' rest of it
        frmMapEditor.nudUp.Value = Map.Up
        frmMapEditor.nudDown.Value = Map.Down
        frmMapEditor.nudLeft.Value = Map.Left
        frmMapEditor.nudRight.Value = Map.Right
        frmMapEditor.cmbMoral.SelectedIndex = Map.Moral
        frmMapEditor.nudSpawnMap.Value = Map.BootMap
        frmMapEditor.nudSpawnX.Value = Map.BootX
        frmMapEditor.nudSpawnY.Value = Map.BootY

        If Map.Instanced = 1 Then
            frmMapEditor.chkInstance.Checked = True
        Else
            frmMapEditor.chkInstance.Checked = False
        End If

        frmMapEditor.lstMapNpc.Items.Clear()

        For X = 1 To MAX_MAP_NPCS
            If Map.Npc(X) = 0 Then
                frmMapEditor.lstMapNpc.Items.Add("No NPC")
            Else
                frmMapEditor.lstMapNpc.Items.Add(X & ": " & Trim$(Npc(Map.Npc(X)).Name))
            End If

        Next

        frmMapEditor.cmbNpcList.Items.Clear()
        frmMapEditor.cmbNpcList.Items.Add("No NPC")

        For Y = 1 To MAX_NPCS
            frmMapEditor.cmbNpcList.Items.Add(Y & ": " & Trim$(Npc(Y).Name))
        Next

        frmMapEditor.lblMap.Text = "Current Map: " & Map.MapNum
        frmMapEditor.nudMaxX.Value = Map.MaxX
        frmMapEditor.nudMaxY.Value = Map.MaxY

        frmMapEditor.cmbTileSets.SelectedIndex = 0
        frmMapEditor.cmbLayers.SelectedIndex = 0
        frmMapEditor.cmbAutoTile.SelectedIndex = 0

        frmMapEditor.cmbWeather.SelectedIndex = Map.WeatherType
        frmMapEditor.nudFog.Value = Map.FogIndex
        frmMapEditor.nudIntensity.Value = Map.WeatherIntensity

        SelectedTab = 1

        GameWindow.SetView(New SFML.Graphics.View(New SFML.Graphics.FloatRect(0, 0, frmMapEditor.picScreen.Width, frmMapEditor.picScreen.Height)))

        frmMapEditor.tslCurMap.Text = "Map: " & Map.MapNum

        ' show the form
        frmMapEditor.Visible = True

        GameStarted = True

        frmMapEditor.picScreen.Focus()

        InitMapEditor = False
    End Sub

    Friend Sub MapEditorInit()
        ' we're in the map editor
        InMapEditor = True

        ' set the scrolly bars
        If Map.tileset = 0 Then Map.tileset = 1
        If Map.tileset > NumTileSets Then Map.tileset = 1

        EditorTileSelStart = New Point(0, 0)
        EditorTileSelEnd = New Point(1, 1)

        'clear memory
        'ReDim TileSetImgsLoaded(NumTileSets)
        'For i = 0 To NumTileSets
        '    TileSetImgsLoaded(i) = False
        'Next

        ' set the scrollbars
        frmMapEditor.scrlPictureY.Maximum = (frmMapEditor.picBackSelect.Height \ PIC_Y) \ 2 ' \2 is new, lets test
        frmMapEditor.scrlPictureX.Maximum = (frmMapEditor.picBackSelect.Width \ PIC_X) \ 2

        'set map names
        frmMapEditor.cmbMapList.Items.Clear()
        FrmVisualWarp.lstMaps.Items.Clear()

        For i = 1 To MAX_MAPS
            frmMapEditor.cmbMapList.Items.Add(i & ": " & MapNames(i))
            FrmVisualWarp.lstMaps.Items.Add(i & ": " & MapNames(i))
        Next

        If Map.MapNum > 0 Then
            frmMapEditor.cmbMapList.SelectedIndex = Map.MapNum - 1
        Else
            frmMapEditor.cmbMapList.SelectedIndex = 0
        End If

        ' set shops for the shop attribute
        frmMapEditor.cmbShop.Items.Add("None")
        For i = 1 To MAX_SHOPS
            frmMapEditor.cmbShop.Items.Add(i & ": " & Shop(i).Name)
        Next
        ' we're not in a shop
        frmMapEditor.cmbShop.SelectedIndex = 0

        frmMapEditor.optBlocked.Checked = True

        frmMapEditor.cmbTileSets.Items.Clear()
        For i = 1 To NumTileSets
            frmMapEditor.cmbTileSets.Items.Add("Tileset " & i)
        Next

        frmMapEditor.cmbTileSets.SelectedIndex = 0
        frmMapEditor.cmbLayers.SelectedIndex = 0

        InitMapProperties = True

        If MapData = True Then GettingMap = False

    End Sub

    Friend Sub MapEditorTileScroll()
        frmMapEditor.picBackSelect.Top = (frmMapEditor.scrlPictureY.Value * PIC_Y) * -1
        frmMapEditor.picBackSelect.Left = (frmMapEditor.scrlPictureX.Value * PIC_X) * -1
    End Sub

    Friend Sub MapEditorChooseTile(Button As Integer, X As Single, Y As Single)

        If Button = MouseButtons.Left Then 'Left Mouse Button

            EditorTileWidth = 1
            EditorTileHeight = 1

            If frmMapEditor.cmbAutoTile.SelectedIndex > 0 Then
                Select Case frmMapEditor.cmbAutoTile.SelectedIndex
                    Case 1 ' autotile
                        EditorTileWidth = 2
                        EditorTileHeight = 3
                    Case 2 ' fake autotile
                        EditorTileWidth = 1
                        EditorTileHeight = 1
                    Case 3 ' animated
                        EditorTileWidth = 6
                        EditorTileHeight = 3
                    Case 4 ' cliff
                        EditorTileWidth = 2
                        EditorTileHeight = 2
                    Case 5 ' waterfall
                        EditorTileWidth = 2
                        EditorTileHeight = 3
                End Select
            End If

            EditorTileX = X \ PIC_X
            EditorTileY = Y \ PIC_Y

            EditorTileSelStart = New Point(EditorTileX, EditorTileY)
            EditorTileSelEnd = New Point(EditorTileX + EditorTileWidth, EditorTileY + EditorTileHeight)

        End If

    End Sub

    Friend Sub MapEditorDrag(Button As Integer, X As Single, Y As Single)

        If Button = MouseButtons.Left Then 'Left Mouse Button
            ' convert the pixel number to tile number
            X = (X \ PIC_X) + 1
            Y = (Y \ PIC_Y) + 1
            ' check it's not out of bounds
            If X < 0 Then X = 0
            If X > frmMapEditor.picBackSelect.Width / PIC_X Then X = frmMapEditor.picBackSelect.Width / PIC_X
            If Y < 0 Then Y = 0
            If Y > frmMapEditor.picBackSelect.Height / PIC_Y Then Y = frmMapEditor.picBackSelect.Height / PIC_Y
            ' find out what to set the width + height of map editor to
            If X > EditorTileX Then ' drag right
                'EditorTileWidth = X
                EditorTileWidth = X - EditorTileX
            Else ' drag left
                ' TO DO
            End If
            If Y > EditorTileY Then ' drag down
                'EditorTileHeight = Y
                EditorTileHeight = Y - EditorTileY
            Else ' drag up
                ' TO DO
            End If

            EditorTileSelStart = New Point(EditorTileX, EditorTileY)
            EditorTileSelEnd = New Point(EditorTileWidth, EditorTileHeight)
        End If

    End Sub

    Friend Sub MapEditorMouseDown(Button As Integer, X As Integer, Y As Integer, Optional movedMouse As Boolean = True)
        Dim i As Integer
        Dim CurLayer As Integer

        CurLayer = frmMapEditor.cmbLayers.SelectedIndex + 1

        If Not IsInBounds() Then Exit Sub
        If Button = MouseButtons.Left Then
            If SelectedTab = 1 Then
                ' (EditorTileSelEnd.X - EditorTileSelStart.X) = 1 AndAlso (EditorTileSelEnd.Y - EditorTileSelStart.Y) = 1 Then 'single tile
                If EditorTileWidth = 1 AndAlso EditorTileHeight = 1 Then

                    MapEditorSetTile(CurX, CurY, CurLayer, False, frmMapEditor.cmbAutoTile.SelectedIndex)
                Else ' multi tile!
                    If frmMapEditor.cmbAutoTile.SelectedIndex = 0 Then
                        MapEditorSetTile(CurX, CurY, CurLayer, True)
                    Else
                        MapEditorSetTile(CurX, CurY, CurLayer, , frmMapEditor.cmbAutoTile.SelectedIndex)
                    End If
                End If
            ElseIf SelectedTab = 2 Then
                With Map.Tile(CurX, CurY)
                    ' blocked tile
                    If frmMapEditor.optBlocked.Checked = True Then .Type = TileType.Blocked
                    ' warp tile
                    If frmMapEditor.optWarp.Checked = True Then
                        .Type = TileType.Warp
                        .Data1 = EditorWarpMap
                        .Data2 = EditorWarpX
                        .Data3 = EditorWarpY
                    End If
                    ' item spawn
                    If frmMapEditor.optItem.Checked = True Then
                        .Type = TileType.Item
                        .Data1 = ItemEditorNum
                        .Data2 = ItemEditorValue
                        .Data3 = 0
                    End If
                    ' npc avoid
                    If frmMapEditor.optNpcAvoid.Checked = True Then
                        .Type = TileType.NpcAvoid
                        .Data1 = 0
                        .Data2 = 0
                        .Data3 = 0
                    End If
                    ' key
                    If frmMapEditor.optKey.Checked = True Then
                        .Type = TileType.Key
                        .Data1 = KeyEditorNum
                        .Data2 = KeyEditorTake
                        .Data3 = 0
                    End If
                    ' key open
                    If frmMapEditor.optKeyOpen.Checked = True Then
                        .Type = TileType.KeyOpen
                        .Data1 = KeyOpenEditorX
                        .Data2 = KeyOpenEditorY
                        .Data3 = 0
                    End If
                    ' resource
                    If frmMapEditor.optResource.Checked = True Then
                        .Type = TileType.Resource
                        .Data1 = ResourceEditorNum
                        .Data2 = 0
                        .Data3 = 0
                    End If
                    ' door
                    If frmMapEditor.optDoor.Checked = True Then
                        .Type = TileType.Door
                        .Data1 = EditorWarpMap
                        .Data2 = EditorWarpX
                        .Data3 = EditorWarpY
                    End If
                    ' npc spawn
                    If frmMapEditor.optNpcSpawn.Checked = True Then
                        .Type = TileType.NpcSpawn
                        .Data1 = SpawnNpcNum
                        .Data2 = SpawnNpcDir
                        .Data3 = 0
                    End If
                    ' shop
                    If frmMapEditor.optShop.Checked = True Then
                        .Type = TileType.Shop
                        .Data1 = EditorShop
                        .Data2 = 0
                        .Data3 = 0
                    End If
                    ' bank
                    If frmMapEditor.optBank.Checked = True Then
                        .Type = TileType.Bank
                        .Data1 = 0
                        .Data2 = 0
                        .Data3 = 0
                    End If
                    ' heal
                    If frmMapEditor.optHeal.Checked = True Then
                        .Type = TileType.Heal
                        .Data1 = MapEditorHealType
                        .Data2 = MapEditorHealAmount
                        .Data3 = 0
                    End If
                    ' trap
                    If frmMapEditor.optTrap.Checked = True Then
                        .Type = TileType.Trap
                        .Data1 = MapEditorHealAmount
                        .Data2 = 0
                        .Data3 = 0
                    End If
                    'Housing
                    If frmMapEditor.optHouse.Checked Then
                        .Type = TileType.House
                        .Data1 = HouseTileIndex
                        .Data2 = 0
                        .Data3 = 0
                    End If
                    'craft tile
                    If frmMapEditor.optCraft.Checked Then
                        .Type = TileType.Craft
                        .Data1 = 0
                        .Data2 = 0
                        .Data3 = 0
                    End If
                    If frmMapEditor.optLight.Checked Then
                        .Type = TileType.Light
                        .Data1 = 0
                        .Data2 = 0
                        .Data3 = 0
                    End If
                End With
            ElseIf SelectedTab = 4 Then
                If movedMouse Then Exit Sub
                ' find what tile it is
                X = X - ((X \ PIC_X) * PIC_X)
                Y = Y - ((Y \ PIC_Y) * PIC_Y)
                ' see if it hits an arrow
                For i = 1 To 4
                    If X >= DirArrowX(i) AndAlso X <= DirArrowX(i) + 8 Then
                        If Y >= DirArrowY(i) AndAlso Y <= DirArrowY(i) + 8 Then
                            ' flip the value.
                            SetDirBlock(Map.Tile(CurX, CurY).DirBlock, (i), Not IsDirBlocked(Map.Tile(CurX, CurY).DirBlock, (i)))
                            Exit Sub
                        End If
                    End If
                Next
            ElseIf SelectedTab = 5 Then
                If frmEvents.Visible = False Then
                    AddEvent(CurX, CurY)
                End If
            End If
        End If

        If Button = MouseButtons.Right Then
            If SelectedTab = 1 Then

                With Map.Tile(CurX, CurY)
                    ' clear layer
                    .Layer(CurLayer).X = 0
                    .Layer(CurLayer).Y = 0
                    .Layer(CurLayer).Tileset = 0
                    If .Layer(CurLayer).AutoTile > 0 Then
                        .Layer(CurLayer).AutoTile = 0
                        ' do a re-init so we can see our changes
                        InitAutotiles()
                    End If
                    CacheRenderState(X, Y, CurLayer)
                End With

            ElseIf SelectedTab = 2 Then
                With Map.Tile(CurX, CurY)
                    ' clear attribute
                    .Type = 0
                    .Data1 = 0
                    .Data2 = 0
                    .Data3 = 0
                End With
            ElseIf SelectedTab = 5 Then
                DeleteEvent(CurX, CurY)
            End If
        End If

    End Sub

    Friend Sub MapEditorCancel()
        InMapEditor = False
        frmMapEditor.Visible = False
        GettingMap = True

        InitAutotiles()

    End Sub

    Friend Sub MapEditorSend()
        SendEditorMap()
        InMapEditor = False
        frmMapEditor.Visible = False
        GettingMap = True

    End Sub

    Friend Sub MapEditorSetTile(X As Integer, Y As Integer, CurLayer As Integer, Optional multitile As Boolean = False, Optional theAutotile As Byte = 0)
        Dim x2 As Integer, y2 As Integer

        If theAutotile > 0 Then
            With Map.Tile(X, Y)
                ' set layer
                .Layer(CurLayer).X = EditorTileX
                .Layer(CurLayer).Y = EditorTileY
                .Layer(CurLayer).Tileset = frmMapEditor.cmbTileSets.SelectedIndex + 1
                .Layer(CurLayer).AutoTile = theAutotile
                CacheRenderState(X, Y, CurLayer)
            End With
            ' do a re-init so we can see our changes
            InitAutotiles()
            Exit Sub
        End If

        If Not multitile Then ' single
            With Map.Tile(X, Y)
                ' set layer
                .Layer(CurLayer).X = EditorTileX
                .Layer(CurLayer).Y = EditorTileY
                .Layer(CurLayer).Tileset = frmMapEditor.cmbTileSets.SelectedIndex + 1
                .Layer(CurLayer).AutoTile = 0
                CacheRenderState(X, Y, CurLayer)
            End With
        Else ' multitile
            y2 = 0 ' starting tile for y axis
            For Y = CurY To CurY + EditorTileHeight - 1
                x2 = 0 ' re-set x count every y loop
                For X = CurX To CurX + EditorTileWidth - 1
                    If X >= 0 AndAlso X <= Map.MaxX Then
                        If Y >= 0 AndAlso Y <= Map.MaxY Then
                            With Map.Tile(X, Y)
                                .Layer(CurLayer).X = EditorTileX + x2
                                .Layer(CurLayer).Y = EditorTileY + y2
                                .Layer(CurLayer).Tileset = frmMapEditor.cmbTileSets.SelectedIndex + 1
                                .Layer(CurLayer).AutoTile = 0
                                CacheRenderState(X, Y, CurLayer)
                            End With
                        End If
                    End If
                    x2 = x2 + 1
                Next
                y2 = y2 + 1
            Next
        End If
    End Sub

    Friend Sub MapEditorClearLayer()
        Dim X As Integer
        Dim Y As Integer
        Dim CurLayer As Integer

        CurLayer = frmMapEditor.cmbLayers.SelectedIndex + 1

        If CurLayer = 0 Then Exit Sub

        ' ask to clear layer
        If MsgBox("Are you sure you wish to clear this layer?", vbYesNo, "MapEditor") = vbYes Then
            For X = 0 To Map.MaxX
                For Y = 0 To Map.MaxY
                    With Map.Tile(X, Y)
                        .Layer(CurLayer).X = 0
                        .Layer(CurLayer).Y = 0
                        .Layer(CurLayer).Tileset = 0
                        .Layer(CurLayer).AutoTile = 0
                        CacheRenderState(X, Y, CurLayer)
                    End With
                Next
            Next
        End If
    End Sub

    Friend Sub MapEditorFillLayer(Optional theAutotile As Byte = 0)
        Dim X As Integer
        Dim Y As Integer
        Dim CurLayer As Integer

        CurLayer = frmMapEditor.cmbLayers.SelectedIndex + 1

        If MsgBox("Are you sure you wish to fill this layer?", vbYesNo, "Map Editor") = vbYes Then
            If theAutotile > 0 Then
                For X = 0 To Map.MaxX
                    For Y = 0 To Map.MaxY
                        Map.Tile(X, Y).Layer(CurLayer).X = EditorTileX
                        Map.Tile(X, Y).Layer(CurLayer).Y = EditorTileY
                        Map.Tile(X, Y).Layer(CurLayer).Tileset = frmMapEditor.cmbTileSets.SelectedIndex + 1
                        Map.Tile(X, Y).Layer(CurLayer).AutoTile = theAutotile
                        CacheRenderState(X, Y, CurLayer)
                    Next
                Next

                ' do a re-init so we can see our changes
                InitAutotiles()
            Else
                For X = 0 To Map.MaxX
                    For Y = 0 To Map.MaxY
                        Map.Tile(X, Y).Layer(CurLayer).X = EditorTileX
                        Map.Tile(X, Y).Layer(CurLayer).Y = EditorTileY
                        Map.Tile(X, Y).Layer(CurLayer).Tileset = frmMapEditor.cmbTileSets.SelectedIndex + 1
                        CacheRenderState(X, Y, CurLayer)
                    Next
                Next
            End If
        End If
    End Sub

    Friend Sub ClearAttributeDialogue()

        With frmMapEditor
            .fraNpcSpawn.Visible = False
            .fraResource.Visible = False
            .fraMapItem.Visible = False
            .fraMapKey.Visible = False
            .fraKeyOpen.Visible = False
            .fraMapWarp.Visible = False
            .fraShop.Visible = False
            .fraHeal.Visible = False
            .fraTrap.Visible = False
            .fraBuyHouse.Visible = False
        End With

    End Sub

    Friend Sub MapEditorClearAttribs()
        Dim X As Integer
        Dim Y As Integer

        If MsgBox("Are you sure you wish to clear the attributes on this map?", vbYesNo, "MapEditor") = vbYes Then

            For X = 0 To Map.MaxX
                For Y = 0 To Map.MaxY
                    Map.Tile(X, Y).Type = 0
                Next
            Next

        End If

    End Sub

    Friend Sub MapEditorLeaveMap()

        If InMapEditor Then
            If MsgBox("Save changes to current map?", vbYesNo) = vbYes Then
                MapEditorSend()
            Else
                MapEditorCancel()
            End If
        End If

    End Sub
#End Region

#Region "Item Editor"
    Friend Sub ItemEditorPreInit()
        Dim i As Integer

        With frmItem
            Editor = EDITOR_ITEM
            .lstIndex.Items.Clear()

            ' Add the names
            For i = 1 To MAX_ITEMS
                .lstIndex.Items.Add(i & ": " & Trim$(Item(i).Name))
            Next

            .Show()
            .lstIndex.SelectedIndex = 0
            ItemEditorInit()
        End With
    End Sub

    Friend Sub ItemEditorInit()
        Dim i As Integer

        If frmItem.Visible = False Then Exit Sub
        EditorIndex = frmItem.lstIndex.SelectedIndex + 1

        With Item(EditorIndex)
            'populate combo boxes
            frmItem.cmbAnimation.Items.Clear()
            frmItem.cmbAnimation.Items.Add("None")
            For i = 1 To MAX_ANIMATIONS
                frmItem.cmbAnimation.Items.Add(i & ": " & Animation(i).Name)
            Next

            frmItem.cmbAmmo.Items.Clear()
            frmItem.cmbAmmo.Items.Add("None")
            For i = 1 To MAX_ITEMS
                frmItem.cmbAmmo.Items.Add(i & ": " & Item(i).Name)
            Next

            frmItem.cmbProjectile.Items.Clear()
            frmItem.cmbProjectile.Items.Add("None")
            For i = 1 To MAX_PROJECTILES
                frmItem.cmbProjectile.Items.Add(i & ": " & Projectiles(i).Name)
            Next

            frmItem.cmbSkills.Items.Clear()
            frmItem.cmbSkills.Items.Add("None")
            For i = 1 To MAX_SKILLS
                frmItem.cmbSkills.Items.Add(i & ": " & Skill(i).Name)
            Next

            frmItem.cmbPet.Items.Clear()
            frmItem.cmbPet.Items.Add("None")
            For i = 1 To MAX_PETS
                frmItem.cmbPet.Items.Add(i & ": " & Pet(i).Name)
            Next

            frmItem.cmbRecipe.Items.Clear()
            frmItem.cmbRecipe.Items.Add("None")
            For i = 1 To MAX_RECIPE
                frmItem.cmbRecipe.Items.Add(i & ": " & Recipe(i).Name)
            Next

            frmItem.txtName.Text = Trim$(.Name)
            frmItem.txtDescription.Text = Trim$(.Description)

            If .Pic > frmItem.nudPic.Maximum Then .Pic = 0
            frmItem.nudPic.Value = .Pic
            If .Type > ItemType.Count - 1 Then .Type = 0
            frmItem.cmbType.SelectedIndex = .Type
            frmItem.cmbAnimation.SelectedIndex = .Animation

            If .ItemLevel = 0 Then .ItemLevel = 1
            frmItem.nudItemLvl.Value = .ItemLevel

            ' Type specific settings
            If (frmItem.cmbType.SelectedIndex = ItemType.Equipment) Then
                frmItem.fraEquipment.Visible = True
                frmItem.cmbProjectile.SelectedIndex = .Data1
                frmItem.nudDamage.Value = .Data2
                frmItem.cmbTool.SelectedIndex = .Data3

                frmItem.cmbSubType.SelectedIndex = .SubType

                If .Speed < 100 Then .Speed = 100
                If .Speed > frmItem.nudSpeed.Maximum Then .Speed = frmItem.nudSpeed.Maximum
                frmItem.nudSpeed.Value = .Speed

                frmItem.nudStrength.Value = .Add_Stat(StatType.Strength)
                frmItem.nudEndurance.Value = .Add_Stat(StatType.Endurance)
                frmItem.nudIntelligence.Value = .Add_Stat(StatType.Intelligence)
                frmItem.nudVitality.Value = .Add_Stat(StatType.Vitality)
                frmItem.nudLuck.Value = .Add_Stat(StatType.Luck)
                frmItem.nudSpirit.Value = .Add_Stat(StatType.Spirit)

                If .KnockBack = 1 Then
                    frmItem.chkKnockBack.Checked = True
                Else
                    frmItem.chkKnockBack.Checked = False
                End If
                frmItem.cmbKnockBackTiles.SelectedIndex = .KnockBackTiles

                If .Randomize = 1 Then
                    frmItem.chkRandomize.Checked = True
                Else
                    frmItem.chkRandomize.Checked = False
                End If

                'If .RandomMin = 0 Then .RandomMin = 1
                'frmEditor_Item.numMin.Value = .RandomMin

                'If .RandomMax <= 1 Then .RandomMax = 2
                'frmEditor_Item.numMax.Value = .RandomMax

                frmItem.nudPaperdoll.Value = .Paperdoll

                frmItem.cmbProjectile.SelectedIndex = .Projectile
                frmItem.cmbAmmo.SelectedIndex = .Ammo
            Else
                frmItem.fraEquipment.Visible = False
            End If

            If (frmItem.cmbType.SelectedIndex = ItemType.Consumable) Then
                frmItem.fraVitals.Visible = True
                frmItem.nudVitalMod.Value = .Data1
            Else
                frmItem.fraVitals.Visible = False
            End If

            If (frmItem.cmbType.SelectedIndex = ItemType.Skill) Then
                frmItem.fraSkill.Visible = True
                frmItem.cmbSkills.SelectedIndex = .Data1
            Else
                frmItem.fraSkill.Visible = False
            End If

            If frmItem.cmbType.SelectedIndex = ItemType.Furniture Then
                frmItem.fraFurniture.Visible = True
                If Item(EditorIndex).Data2 > 0 AndAlso Item(EditorIndex).Data2 <= NumFurniture Then
                    frmItem.nudFurniture.Value = Item(EditorIndex).Data2
                Else
                    frmItem.nudFurniture.Value = 1
                End If
                frmItem.cmbFurnitureType.SelectedIndex = Item(EditorIndex).Data1
            Else
                frmItem.fraFurniture.Visible = False
            End If

            If (frmItem.cmbType.SelectedIndex = ItemType.Pet) Then
                frmItem.fraPet.Visible = True
                frmItem.cmbPet.SelectedIndex = .Data1
            Else
                frmItem.fraPet.Visible = False
            End If

            ' Basic requirements
            frmItem.cmbAccessReq.SelectedIndex = .AccessReq
            frmItem.nudLevelReq.Value = .LevelReq

            frmItem.nudStrReq.Value = .Stat_Req(StatType.Strength)
            frmItem.nudVitReq.Value = .Stat_Req(StatType.Vitality)
            frmItem.nudLuckReq.Value = .Stat_Req(StatType.Luck)
            frmItem.nudEndReq.Value = .Stat_Req(StatType.Endurance)
            frmItem.nudIntReq.Value = .Stat_Req(StatType.Intelligence)
            frmItem.nudSprReq.Value = .Stat_Req(StatType.Spirit)

            ' Build cmbClassReq
            frmItem.cmbClassReq.Items.Clear()
            frmItem.cmbClassReq.Items.Add("None")

            For i = 1 To Max_Classes
                frmItem.cmbClassReq.Items.Add(Classes(i).Name)
            Next

            frmItem.cmbClassReq.SelectedIndex = .ClassReq
            ' Info
            frmItem.nudPrice.Value = .Price
            frmItem.cmbBind.SelectedIndex = .BindType
            frmItem.nudRarity.Value = .Rarity

            If .Stackable = 1 Then
                frmItem.chkStackable.Checked = True
            Else
                frmItem.chkStackable.Checked = False
            End If

            EditorIndex = frmItem.lstIndex.SelectedIndex + 1
        End With

        frmItem.nudPic.Maximum = NumItems

        If NumPaperdolls > 0 Then
            frmItem.nudPaperdoll.Maximum = NumPaperdolls + 1
        End If

        EditorItem_DrawItem()
        EditorItem_DrawPaperdoll()
        EditorItem_DrawFurniture()
        Item_Changed(EditorIndex) = True

    End Sub

    Friend Sub ItemEditorCancel()
        Editor = 0
        frmItem.Visible = False
        ClearChanged_Item()
        ClearItems()
        SendRequestItems()
    End Sub

    Friend Sub ItemEditorOk()
        Dim i As Integer

        For i = 1 To MAX_ITEMS
            If Item_Changed(i) Then
                SendSaveItem(i)
            End If
        Next

        frmItem.Visible = False
        Editor = 0
        ClearChanged_Item()
    End Sub
#End Region

#Region "Npc Editor"
    Friend Sub NpcEditorInit()
        Dim i As Integer

        If frmNPC.Visible = False Then Exit Sub
        EditorIndex = frmNPC.lstIndex.SelectedIndex + 1
        frmNPC.cmbDropSlot.SelectedIndex = 0
        If Npc(EditorIndex).AttackSay Is Nothing Then Npc(EditorIndex).AttackSay = ""
        If Npc(EditorIndex).Name Is Nothing Then Npc(EditorIndex).Name = ""

        With frmNPC
            'populate combo boxes
            .cmbAnimation.Items.Clear()
            .cmbAnimation.Items.Add("None")
            For i = 1 To MAX_ANIMATIONS
                .cmbAnimation.Items.Add(i & ": " & Animation(i).Name)
            Next

            .cmbQuest.Items.Clear()
            .cmbQuest.Items.Add("None")
            For i = 1 To MAX_QUESTS
                .cmbQuest.Items.Add(i & ": " & Quest(i).Name)
            Next

            .cmbItem.Items.Clear()
            .cmbItem.Items.Add("None")
            For i = 1 To MAX_ITEMS
                .cmbItem.Items.Add(i & ": " & Item(i).Name)
            Next

            .txtName.Text = Trim$(Npc(EditorIndex).Name)
            .txtAttackSay.Text = Trim$(Npc(EditorIndex).AttackSay)

            If Npc(EditorIndex).Sprite < 0 OrElse Npc(EditorIndex).Sprite > .nudSprite.Maximum Then Npc(EditorIndex).Sprite = 0
            .nudSprite.Value = Npc(EditorIndex).Sprite
            .nudSpawnSecs.Value = Npc(EditorIndex).SpawnSecs
            .cmbBehaviour.SelectedIndex = Npc(EditorIndex).Behaviour
            .cmbFaction.SelectedIndex = Npc(EditorIndex).Faction
            .nudRange.Value = Npc(EditorIndex).Range
            .nudChance.Value = Npc(EditorIndex).DropChance(frmNPC.cmbDropSlot.SelectedIndex + 1)
            .cmbItem.SelectedIndex = Npc(EditorIndex).DropItem(frmNPC.cmbDropSlot.SelectedIndex + 1)

            .nudAmount.Value = Npc(EditorIndex).DropItemValue(frmNPC.cmbDropSlot.SelectedIndex + 1)

            .nudHp.Value = Npc(EditorIndex).Hp
            .nudExp.Value = Npc(EditorIndex).Exp
            .nudLevel.Value = Npc(EditorIndex).Level
            .nudDamage.Value = Npc(EditorIndex).Damage

            .cmbQuest.SelectedIndex = Npc(EditorIndex).QuestNum
            .cmbSpawnPeriod.SelectedIndex = Npc(EditorIndex).SpawnTime

            .nudStrength.Value = Npc(EditorIndex).Stat(StatType.Strength)
            .nudEndurance.Value = Npc(EditorIndex).Stat(StatType.Endurance)
            .nudIntelligence.Value = Npc(EditorIndex).Stat(StatType.Intelligence)
            .nudSpirit.Value = Npc(EditorIndex).Stat(StatType.Spirit)
            .nudLuck.Value = Npc(EditorIndex).Stat(StatType.Luck)
            .nudVitality.Value = Npc(EditorIndex).Stat(StatType.Vitality)

            .cmbSkill1.Items.Clear()
            .cmbSkill2.Items.Clear()
            .cmbSkill3.Items.Clear()
            .cmbSkill4.Items.Clear()
            .cmbSkill5.Items.Clear()
            .cmbSkill6.Items.Clear()

            .cmbSkill1.Items.Add("None")
            .cmbSkill2.Items.Add("None")
            .cmbSkill3.Items.Add("None")
            .cmbSkill4.Items.Add("None")
            .cmbSkill5.Items.Add("None")
            .cmbSkill6.Items.Add("None")

            For i = 1 To MAX_SKILLS
                If Len(Skill(i).Name) > 0 Then
                    .cmbSkill1.Items.Add(Skill(i).Name)
                    .cmbSkill2.Items.Add(Skill(i).Name)
                    .cmbSkill3.Items.Add(Skill(i).Name)
                    .cmbSkill4.Items.Add(Skill(i).Name)
                    .cmbSkill5.Items.Add(Skill(i).Name)
                    .cmbSkill6.Items.Add(Skill(i).Name)
                End If
            Next

            .cmbSkill1.SelectedIndex = Npc(EditorIndex).Skill(1)
            .cmbSkill2.SelectedIndex = Npc(EditorIndex).Skill(2)
            .cmbSkill3.SelectedIndex = Npc(EditorIndex).Skill(3)
            .cmbSkill4.SelectedIndex = Npc(EditorIndex).Skill(4)
            .cmbSkill5.SelectedIndex = Npc(EditorIndex).Skill(5)
            .cmbSkill6.SelectedIndex = Npc(EditorIndex).Skill(6)
        End With

        EditorNpc_DrawSprite()
        NPC_Changed(EditorIndex) = True
    End Sub

    Friend Sub NpcEditorOk()
        Dim i As Integer

        For i = 1 To MAX_NPCS
            If NPC_Changed(i) Then
                SendSaveNpc(i)
            End If
        Next

        frmNPC.Visible = False
        Editor = 0
        ClearChanged_NPC()
    End Sub

    Friend Sub NpcEditorCancel()
        Editor = 0
        frmNPC.Visible = False
        ClearChanged_NPC()
        ClearNpcs()
        SendRequestNPCS()
    End Sub

    Friend Sub ClearChanged_NPC()
        For i = 1 To MAX_NPCS
            NPC_Changed(i) = False
        Next
    End Sub
#End Region

#Region "Resource Editor"
    Friend Sub ResourceEditorInit()
        Dim i As Integer

        If frmResource.Visible = False Then Exit Sub
        EditorIndex = frmResource.lstIndex.SelectedIndex + 1

        With frmResource
            'populate combo boxes
            .cmbRewardItem.Items.Clear()
            .cmbRewardItem.Items.Add("None")
            For i = 1 To MAX_ITEMS
                .cmbRewardItem.Items.Add(i & ": " & Item(i).Name)
            Next

            .cmbAnimation.Items.Clear()
            .cmbAnimation.Items.Add("None")
            For i = 1 To MAX_ANIMATIONS
                .cmbAnimation.Items.Add(i & ": " & Animation(i).Name)
            Next

            .nudExhaustedPic.Maximum = NumResources
            .nudNormalPic.Maximum = NumResources
            .nudRespawn.Maximum = 1000000
            .txtName.Text = Trim$(Resource(EditorIndex).Name)
            .txtMessage.Text = Trim$(Resource(EditorIndex).SuccessMessage)
            .txtMessage2.Text = Trim$(Resource(EditorIndex).EmptyMessage)
            .cmbType.SelectedIndex = Resource(EditorIndex).ResourceType
            .nudNormalPic.Value = Resource(EditorIndex).ResourceImage
            .nudExhaustedPic.Value = Resource(EditorIndex).ExhaustedImage
            .cmbRewardItem.SelectedIndex = Resource(EditorIndex).ItemReward
            .nudRewardExp.Value = Resource(EditorIndex).ExpReward
            .cmbTool.SelectedIndex = Resource(EditorIndex).ToolRequired
            .nudHealth.Value = Resource(EditorIndex).Health
            .nudRespawn.Value = Resource(EditorIndex).RespawnTime
            .cmbAnimation.SelectedIndex = Resource(EditorIndex).Animation
            .nudLvlReq.Value = Resource(EditorIndex).LvlRequired
        End With


        frmResource.Visible = True

        EditorResource_DrawSprite()

        Resource_Changed(EditorIndex) = True
    End Sub

    Friend Sub ResourceEditorOk()
        Dim i As Integer

        For i = 1 To MAX_RESOURCES
            If Resource_Changed(i) Then
                SendSaveResource(i)
            End If
        Next

        frmResource.Visible = False
        Editor = 0
        ClearChanged_Resource()
    End Sub

    Friend Sub ResourceEditorCancel()
        Editor = 0
        frmResource.Visible = False
        ClearChanged_Resource()
        ClearResources()
        SendRequestResources()
    End Sub
#End Region

#Region "Skill Editor"
    Friend Sub SkillEditorInit()
        Dim i As Integer

        If frmSkill.Visible = False Then Exit Sub
        EditorIndex = frmSkill.lstIndex.SelectedIndex + 1

        If Skill(EditorIndex).Name Is Nothing Then Skill(EditorIndex).Name = ""

        With frmSkill
            ' set max values
            .nudAoE.Maximum = Byte.MaxValue
            .nudRange.Maximum = Byte.MaxValue
            .nudMap.Maximum = MAX_MAPS

            ' build class combo
            .cmbClass.Items.Clear()
            .cmbClass.Items.Add("None")
            For i = 1 To Max_Classes
                .cmbClass.Items.Add(Trim$(Classes(i).Name))
            Next
            .cmbClass.SelectedIndex = 0

            .cmbProjectile.Items.Clear()
            .cmbProjectile.Items.Add("None")
            For i = 1 To MAX_PROJECTILES
                .cmbProjectile.Items.Add(Trim$(Projectiles(i).Name))
            Next
            .cmbProjectile.SelectedIndex = 0

            .cmbAnimCast.Items.Clear()
            .cmbAnimCast.Items.Add("None")
            .cmbAnim.Items.Clear()
            .cmbAnim.Items.Add("None")
            For i = 1 To MAX_ANIMATIONS
                .cmbAnimCast.Items.Add(Trim$(Animation(i).Name))
                .cmbAnim.Items.Add(Trim$(Animation(i).Name))
            Next
            .cmbAnimCast.SelectedIndex = 0
            .cmbAnim.SelectedIndex = 0

            ' set values
            .txtName.Text = Trim$(Skill(EditorIndex).Name)
            .cmbType.SelectedIndex = Skill(EditorIndex).Type
            .nudMp.Value = Skill(EditorIndex).MpCost
            .nudLevel.Value = Skill(EditorIndex).LevelReq
            .cmbAccessReq.SelectedIndex = Skill(EditorIndex).AccessReq
            .cmbClass.SelectedIndex = Skill(EditorIndex).ClassReq
            .nudCast.Value = Skill(EditorIndex).CastTime
            .nudCool.Value = Skill(EditorIndex).CdTime
            .nudIcon.Value = Skill(EditorIndex).Icon
            .nudMap.Value = Skill(EditorIndex).Map
            .nudX.Value = Skill(EditorIndex).X
            .nudY.Value = Skill(EditorIndex).Y
            .cmbDir.SelectedIndex = Skill(EditorIndex).Dir
            .nudVital.Value = Skill(EditorIndex).Vital
            .nudDuration.Value = Skill(EditorIndex).Duration
            .nudInterval.Value = Skill(EditorIndex).Interval
            .nudRange.Value = Skill(EditorIndex).Range

            .chkAoE.Checked = Skill(EditorIndex).IsAoE

            .nudAoE.Value = Skill(EditorIndex).AoE
            .cmbAnimCast.SelectedIndex = Skill(EditorIndex).CastAnim
            .cmbAnim.SelectedIndex = Skill(EditorIndex).SkillAnim
            .nudStun.Value = Skill(EditorIndex).StunDuration

            If Skill(EditorIndex).IsProjectile = 1 Then
                .chkProjectile.Checked = True
            Else
                .chkProjectile.Checked = False
            End If
            .cmbProjectile.SelectedIndex = Skill(EditorIndex).Projectile

            If Skill(EditorIndex).KnockBack = 1 Then
                .chkKnockBack.Checked = True
            Else
                .chkKnockBack.Checked = False
            End If
            .cmbKnockBackTiles.SelectedIndex = Skill(EditorIndex).KnockBackTiles
        End With

        EditorSkill_BltIcon()

        Skill_Changed(EditorIndex) = True
    End Sub

    Friend Sub SkillEditorOk()
        Dim i As Integer

        For i = 1 To MAX_SKILLS
            If Skill_Changed(i) Then
                SendSaveSkill(i)
            End If
        Next

        frmSkill.Visible = False
        Editor = 0
        ClearChanged_Skill()
    End Sub

    Friend Sub SkillEditorCancel()
        Editor = 0
        frmSkill.Visible = False
        ClearChanged_Skill()
        ClearSkills()
        SendRequestSkills()
    End Sub

    Friend Sub ClearChanged_Skill()
        For i = 1 To MAX_SKILLS
            Skill_Changed(i) = False
        Next
    End Sub
#End Region

#Region "Shop editor"
    Friend Sub ShopEditorInit()
        Dim i As Integer

        If frmShop.Visible = False Then Exit Sub
        EditorIndex = frmShop.lstIndex.SelectedIndex + 1

        frmShop.txtName.Text = Trim$(Shop(EditorIndex).Name)
        If Shop(EditorIndex).BuyRate > 0 Then
            frmShop.nudBuy.Value = Shop(EditorIndex).BuyRate
        Else
            frmShop.nudBuy.Value = 100
        End If

        frmShop.nudFace.Value = Shop(EditorIndex).Face
        If File.Exists(Application.StartupPath & GFX_PATH & "Faces\" & Shop(EditorIndex).Face & GFX_EXT) Then
            frmShop.picFace.BackgroundImage = Image.FromFile(Application.StartupPath & GFX_PATH & "Faces\" & Shop(EditorIndex).Face & GFX_EXT)
        End If

        frmShop.cmbItem.Items.Clear()
        frmShop.cmbItem.Items.Add("None")
        frmShop.cmbCostItem.Items.Clear()
        frmShop.cmbCostItem.Items.Add("None")

        For i = 1 To MAX_ITEMS
            frmShop.cmbItem.Items.Add(i & ": " & Trim$(Item(i).Name))
            frmShop.cmbCostItem.Items.Add(i & ": " & Trim$(Item(i).Name))
        Next

        frmShop.cmbItem.SelectedIndex = 0
        frmShop.cmbCostItem.SelectedIndex = 0

        UpdateShopTrade()

        Shop_Changed(EditorIndex) = True
    End Sub

    Friend Sub UpdateShopTrade()
        Dim i As Integer
        frmShop.lstTradeItem.Items.Clear()

        For i = 1 To MAX_TRADES
            With Shop(EditorIndex).TradeItem(i)
                ' if none, show as none
                If .Item = 0 AndAlso .CostItem = 0 Then
                    frmShop.lstTradeItem.Items.Add("Empty Trade Slot")
                Else
                    frmShop.lstTradeItem.Items.Add(i & ": " & .ItemValue & "x " & Trim$(Item(.Item).Name) & " for " & .CostValue & "x " & Trim$(Item(.CostItem).Name))
                End If
            End With
        Next

        frmShop.lstTradeItem.SelectedIndex = 0
    End Sub

    Friend Sub ShopEditorOk()
        Dim i As Integer

        For i = 1 To MAX_SHOPS
            If Shop_Changed(i) Then
                SendSaveShop(i)
            End If
        Next

        frmShop.Visible = False
        Editor = 0
        ClearChanged_Shop()
    End Sub

    Friend Sub ShopEditorCancel()
        Editor = 0
        frmShop.Visible = False
        ClearChanged_Shop()
        ClearShops()
        SendRequestShops()
    End Sub

    Friend Sub ClearChanged_Shop()
        For i = 1 To MAX_SHOPS
            Shop_Changed(i) = False
        Next
    End Sub
#End Region

#Region "Class Editor"

    Friend Sub ClassesEditorOk()
        SendSaveClasses()

        frmClasses.Visible = False
        Editor = 0
    End Sub

    Friend Sub ClassesEditorCancel()
        SendRequestClasses()
        frmClasses.Visible = False
        Editor = 0
    End Sub

    Friend Sub ClassEditorInit()
        Dim i As Integer

        frmClasses.lstIndex.Items.Clear()

        For i = 1 To Max_Classes
            frmClasses.lstIndex.Items.Add(Trim(Classes(i).Name))
        Next

        Editor = EDITOR_CLASSES

        frmClasses.nudMaleSprite.Maximum = NumCharacters
        frmClasses.nudFemaleSprite.Maximum = NumCharacters

        frmClasses.cmbItems.Items.Clear()

        frmClasses.cmbItems.Items.Add("None")
        For i = 1 To MAX_ITEMS
            frmClasses.cmbItems.Items.Add(Trim(Item(i).Name))
        Next

        frmClasses.lstIndex.SelectedIndex = 0

        frmClasses.Visible = True
    End Sub

    Friend Sub LoadClass()
        Dim i As Integer

        If EditorIndex <= 0 OrElse EditorIndex > Max_Classes Then Exit Sub

        frmClasses.txtName.Text = Classes(EditorIndex).Name
        frmClasses.txtDescription.Text = Classes(EditorIndex).Desc

        frmClasses.cmbMaleSprite.Items.Clear()

        For i = 0 To UBound(Classes(EditorIndex).MaleSprite)
            frmClasses.cmbMaleSprite.Items.Add("Sprite " & i + 1)
        Next

        frmClasses.cmbFemaleSprite.Items.Clear()

        For i = 0 To UBound(Classes(EditorIndex).FemaleSprite)
            frmClasses.cmbFemaleSprite.Items.Add("Sprite " & i + 1)
        Next

        frmClasses.nudMaleSprite.Value = Classes(EditorIndex).MaleSprite(0)
        frmClasses.nudFemaleSprite.Value = Classes(EditorIndex).FemaleSprite(0)

        frmClasses.cmbMaleSprite.SelectedIndex = 0
        frmClasses.cmbFemaleSprite.SelectedIndex = 0

        frmClasses.DrawPreview()

        For i = 1 To StatType.Count - 1
            If Classes(EditorIndex).Stat(i) = 0 Then Classes(EditorIndex).Stat(i) = 1
        Next

        frmClasses.nudStrength.Value = Classes(EditorIndex).Stat(StatType.Strength)
        frmClasses.nudLuck.Value = Classes(EditorIndex).Stat(StatType.Luck)
        frmClasses.nudEndurance.Value = Classes(EditorIndex).Stat(StatType.Endurance)
        frmClasses.nudIntelligence.Value = Classes(EditorIndex).Stat(StatType.Intelligence)
        frmClasses.nudVitality.Value = Classes(EditorIndex).Stat(StatType.Vitality)
        frmClasses.nudSpirit.Value = Classes(EditorIndex).Stat(StatType.Spirit)

        If Classes(EditorIndex).BaseExp < 10 Then
            frmClasses.nudBaseExp.Value = 10
        Else
            frmClasses.nudBaseExp.Value = Classes(EditorIndex).BaseExp
        End If

        frmClasses.lstStartItems.Items.Clear()

        For i = 1 To 5
            If Classes(EditorIndex).StartItem(i) > 0 Then
                frmClasses.lstStartItems.Items.Add(Item(Classes(EditorIndex).StartItem(i)).Name & " X " & Classes(EditorIndex).StartValue(i))
            Else
                frmClasses.lstStartItems.Items.Add("None")
            End If
        Next

        frmClasses.nudStartMap.Value = Classes(EditorIndex).StartMap
        frmClasses.nudStartX.Value = Classes(EditorIndex).StartX
        frmClasses.nudStartY.Value = Classes(EditorIndex).StartY
    End Sub

#End Region

End Module