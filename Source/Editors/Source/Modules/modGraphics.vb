﻿Imports System.IO
Imports SFML.Graphics
Imports SFML.Window

Module modGraphics
    Friend GameWindow As RenderWindow
    Friend TilesetWindow As RenderWindow

    Friend EditorItem_Furniture As RenderWindow

    Friend EditorSkill_Icon As RenderWindow

    Friend EditorAnimation_Anim1 As RenderWindow
    Friend EditorAnimation_Anim2 As RenderWindow

    Friend TmpItemWindow As RenderWindow

    Friend SFMLGameFont As Font

    'TileSets
    Friend TileSetTexture() As Texture
    Friend TileSetSprite() As Sprite
    Friend TileSetTextureInfo() As GraphicInfo
    'Characters
    Friend CharacterGFX() As Texture
    Friend CharacterSprite() As Sprite
    Friend CharacterGFXInfo() As GraphicInfo
    'Paperdolls
    Friend PaperDollGFX() As Texture
    Friend PaperDollSprite() As Sprite
    Friend PaperDollGFXInfo() As GraphicInfo
    'Items
    Friend ItemsGFX() As Texture
    Friend ItemsSprite() As Sprite
    Friend ItemsGFXInfo() As GraphicInfo
    'Resources
    Friend ResourcesGFX() As Texture
    Friend ResourcesSprite() As Sprite
    Friend ResourcesGFXInfo() As GraphicInfo
    'Animations
    Friend AnimationsGFX() As Texture
    Friend AnimationsSprite() As Sprite
    Friend AnimationsGFXInfo() As GraphicInfo
    'Skills
    Friend SkillIconsGFX() As Texture
    Friend SkillIconsSprite() As Sprite
    Friend SkillIconsGFXInfo() As GraphicInfo
    'Housing
    Friend FurnitureGFX() As Texture
    Friend FurnitureSprite() As Sprite
    Friend FurnitureGFXInfo() As GraphicInfo
    'Faces
    Friend FacesGFX() As Texture
    Friend FacesSprite() As Sprite
    Friend FacesGFXInfo() As GraphicInfo
    'Projectiles
    Friend ProjectileGFX() As Texture
    Friend ProjectileSprite() As Sprite
    Friend ProjectileGFXInfo() As GraphicInfo
    'Fogs
    Friend FogGFX() As Texture
    Friend FogSprite() As Sprite
    Friend FogGFXInfo() As GraphicInfo
    'Door
    Friend DoorGFX As Texture
    Friend DoorSprite As Sprite
    Friend DoorGFXInfo As GraphicInfo
    'Directions
    Friend DirectionsGfx As Texture
    Friend DirectionsSprite As Sprite
    Friend DirectionsGFXInfo As GraphicInfo
    'Weather
    Friend WeatherGFX As Texture
    Friend WeatherSprite As Sprite
    Friend WeatherGFXInfo As GraphicInfo

    ' Number of graphic files
    Friend MapEditorBackBuffer As Bitmap

    Friend MapTintSprite As Sprite

    Friend NumTileSets As Integer
    Friend NumCharacters As Integer
    Friend NumPaperdolls As Integer
    Friend NumItems As Integer
    Friend NumResources As Integer
    Friend NumAnimations As Integer
    Friend NumSkillIcons As Integer
    Friend NumFaces As Integer
    Friend NumFogs As Integer

    Friend NightGfx As New RenderTexture(245, 245)
    Friend NightSprite As Sprite
    Friend NightGfxInfo As GraphicInfo

    Friend LightGfx As Texture
    Friend LightSprite As Sprite
    Friend LightGfxInfo As GraphicInfo

    Friend Structure GraphicInfo
        Dim width As Integer
        Dim height As Integer
        Dim IsLoaded As Boolean
        Dim TextureTimer As Integer
    End Structure

    Friend Structure Graphics_Tiles
        Dim Tile(,) As Texture
    End Structure

    Sub InitGraphics()

        GameWindow = New RenderWindow(frmMapEditor.picScreen.Handle)
        GameWindow.SetFramerateLimit(FPS_LIMIT)

        TilesetWindow = New RenderWindow(frmMapEditor.picBackSelect.Handle)

        EditorItem_Furniture = New RenderWindow(frmItem.picFurniture.Handle)

        EditorSkill_Icon = New RenderWindow(frmSkill.picSprite.Handle)

        EditorAnimation_Anim1 = New RenderWindow(frmAnimation.picSprite0.Handle)
        EditorAnimation_Anim2 = New RenderWindow(frmAnimation.picSprite1.Handle)

        SFMLGameFont = New Font(Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\" + FONT_NAME)

        'this stuff only loads when needed :)

        ReDim TileSetTexture(NumTileSets)
        ReDim TileSetSprite(NumTileSets)
        ReDim TileSetTextureInfo(NumTileSets)

        ReDim CharacterGFX(NumCharacters)
        ReDim CharacterSprite(NumCharacters)
        ReDim CharacterGFXInfo(NumCharacters)

        ReDim PaperDollGFX(NumPaperdolls)
        ReDim PaperDollSprite(NumPaperdolls)
        ReDim PaperDollGFXInfo(NumPaperdolls)

        ReDim ItemsGFX(NumItems)
        ReDim ItemsSprite(NumItems)
        ReDim ItemsGFXInfo(NumItems)

        ReDim ResourcesGFX(NumResources)
        ReDim ResourcesSprite(NumResources)
        ReDim ResourcesGFXInfo(NumResources)

        ReDim AnimationsGFX(NumAnimations)
        ReDim AnimationsSprite(NumAnimations)
        ReDim AnimationsGFXInfo(NumAnimations)

        ReDim SkillIconsGFX(NumSkillIcons)
        ReDim SkillIconsSprite(NumSkillIcons)
        ReDim SkillIconsGFXInfo(NumSkillIcons)

        ReDim FacesGFX(NumFaces)
        ReDim FacesSprite(NumFaces)
        ReDim FacesGFXInfo(NumFaces)

        ReDim FurnitureGFX(NumFurniture)
        ReDim FurnitureSprite(NumFurniture)
        ReDim FurnitureGFXInfo(NumFurniture)

        ReDim ProjectileGFX(NumProjectiles)
        ReDim ProjectileSprite(NumProjectiles)
        ReDim ProjectileGFXInfo(NumProjectiles)

        ReDim FogGFX(NumFogs)
        ReDim FogSprite(NumFogs)
        ReDim FogGFXInfo(NumFogs)

        'sadly, gui shit is always needed, so we preload it :/
        DoorGFXInfo = New GraphicInfo
        If File.Exists(Application.StartupPath & GFX_PATH & "Misc\Door" & GFX_EXT) Then
            'Load texture first, dont care about memory streams (just use the filename)
            DoorGFX = New Texture(Application.StartupPath & GFX_PATH & "Misc\Door" & GFX_EXT)
            DoorSprite = New Sprite(DoorGFX)

            'Cache the width and height
            DoorGFXInfo.width = DoorGFX.Size.X
            DoorGFXInfo.height = DoorGFX.Size.Y
        End If

        DirectionsGFXInfo = New GraphicInfo
        If File.Exists(Application.StartupPath & GFX_PATH & "Misc\Direction" & GFX_EXT) Then
            'Load texture first, dont care about memory streams (just use the filename)
            DirectionsGfx = New Texture(Application.StartupPath & GFX_PATH & "Misc\Direction" & GFX_EXT)
            DirectionsSprite = New Sprite(DirectionsGfx)

            'Cache the width and height
            DirectionsGFXInfo.width = DirectionsGfx.Size.X
            DirectionsGFXInfo.height = DirectionsGfx.Size.Y
        End If

        WeatherGFXInfo = New GraphicInfo
        If File.Exists(Application.StartupPath & GFX_PATH & "Misc\Weather" & GFX_EXT) Then
            'Load texture first, dont care about memory streams (just use the filename)
            WeatherGFX = New Texture(Application.StartupPath & GFX_PATH & "Misc\Weather" & GFX_EXT)
            WeatherSprite = New Sprite(WeatherGFX)

            'Cache the width and height
            WeatherGFXInfo.width = WeatherGFX.Size.X
            WeatherGFXInfo.height = WeatherGFX.Size.Y
        End If

        LightGfxInfo = New GraphicInfo
        If File.Exists(Application.StartupPath & GFX_PATH & "Misc\Light" & GFX_EXT) Then
            LightGfx = New Texture(Application.StartupPath & GFX_PATH & "Misc\Light" & GFX_EXT)
            LightSprite = New Sprite(LightGfx)

            'Cache the width and height
            LightGfxInfo.width = LightGfx.Size.X
            LightGfxInfo.height = LightGfx.Size.Y
        End If

    End Sub

    Friend Sub LoadTexture(index as integer, TexType As Byte)

        If TexType = 1 Then 'tilesets
            If Index <= 0 OrElse Index > NumTileSets Then Exit Sub

            'Load texture first, dont care about memory streams (just use the filename)
            TileSetTexture(Index) = New Texture(Application.StartupPath & GFX_PATH & "tilesets\" & Index & GFX_EXT)
            TileSetSprite(Index) = New Sprite(TileSetTexture(Index))

            'Cache the width and height
            With TileSetTextureInfo(Index)
                .width = TileSetTexture(Index).Size.X
                .height = TileSetTexture(Index).Size.Y
                .IsLoaded = True
                .TextureTimer = GetTickCount() + 100000
            End With

        ElseIf TexType = 2 Then 'characters
            If Index <= 0 OrElse Index > NumCharacters Then Exit Sub

            'Load texture first, dont care about memory streams (just use the filename)
            CharacterGFX(Index) = New Texture(Application.StartupPath & GFX_PATH & "characters\" & Index & GFX_EXT)
            CharacterSprite(Index) = New Sprite(CharacterGFX(Index))

            'Cache the width and height
            With CharacterGFXInfo(Index)
                .width = CharacterGFX(Index).Size.X
                .height = CharacterGFX(Index).Size.Y
                .IsLoaded = True
                .TextureTimer = GetTickCount() + 100000
            End With

        ElseIf TexType = 3 Then 'paperdoll
            If Index <= 0 OrElse Index > NumPaperdolls Then Exit Sub

            'Load texture first, dont care about memory streams (just use the filename)
            PaperDollGFX(Index) = New Texture(Application.StartupPath & GFX_PATH & "Paperdolls\" & Index & GFX_EXT)
            PaperDollSprite(Index) = New Sprite(PaperDollGFX(Index))

            'Cache the width and height
            With PaperDollGFXInfo(Index)
                .width = PaperDollGFX(Index).Size.X
                .height = PaperDollGFX(Index).Size.Y
                .IsLoaded = True
                .TextureTimer = GetTickCount() + 100000
            End With

        ElseIf TexType = 4 Then 'items
            If Index <= 0 OrElse Index > NumItems Then Exit Sub

            'Load texture first, dont care about memory streams (just use the filename)
            ItemsGFX(Index) = New Texture(Application.StartupPath & GFX_PATH & "items\" & Index & GFX_EXT)
            ItemsSprite(Index) = New Sprite(ItemsGFX(Index))

            'Cache the width and height
            With ItemsGFXInfo(Index)
                .width = ItemsGFX(Index).Size.X
                .height = ItemsGFX(Index).Size.Y
                .IsLoaded = True
                .TextureTimer = GetTickCount() + 100000
            End With

        ElseIf TexType = 5 Then 'resources
            If Index <= 0 OrElse Index > NumResources Then Exit Sub

            'Load texture first, dont care about memory streams (just use the filename)
            ResourcesGFX(Index) = New Texture(Application.StartupPath & GFX_PATH & "resources\" & Index & GFX_EXT)
            ResourcesSprite(Index) = New Sprite(ResourcesGFX(Index))

            'Cache the width and height
            With ResourcesGFXInfo(Index)
                .width = ResourcesGFX(Index).Size.X
                .height = ResourcesGFX(Index).Size.Y
                .IsLoaded = True
                .TextureTimer = GetTickCount() + 100000
            End With

        ElseIf TexType = 6 Then 'animations
            If Index <= 0 OrElse Index > NumAnimations Then Exit Sub

            'Load texture first, dont care about memory streams (just use the filename)
            AnimationsGFX(Index) = New Texture(Application.StartupPath & GFX_PATH & "Animations\" & Index & GFX_EXT)
            AnimationsSprite(Index) = New Sprite(AnimationsGFX(Index))

            'Cache the width and height
            With AnimationsGFXInfo(Index)
                .width = AnimationsGFX(Index).Size.X
                .height = AnimationsGFX(Index).Size.Y
                .IsLoaded = True
                .TextureTimer = GetTickCount() + 100000
            End With

        ElseIf TexType = 7 Then 'faces
            If Index <= 0 OrElse Index > NumFaces Then Exit Sub

            'Load texture first, dont care about memory streams (just use the filename)
            FacesGFX(Index) = New Texture(Application.StartupPath & GFX_PATH & "Faces\" & Index & GFX_EXT)
            FacesSprite(Index) = New Sprite(FacesGFX(Index))

            'Cache the width and height
            With FacesGFXInfo(Index)
                .width = FacesGFX(Index).Size.X
                .height = FacesGFX(Index).Size.Y
                .IsLoaded = True
                .TextureTimer = GetTickCount() + 100000
            End With

        ElseIf TexType = 8 Then 'fogs
            If Index <= 0 OrElse Index > NumFogs Then Exit Sub

            'Load texture first, dont care about memory streams (just use the filename)
            FogGFX(Index) = New Texture(Application.StartupPath & GFX_PATH & "Fogs\" & Index & GFX_EXT)
            FogSprite(Index) = New Sprite(FogGFX(Index))

            'Cache the width and height
            With FogGFXInfo(Index)
                .width = FogGFX(Index).Size.X
                .height = FogGFX(Index).Size.Y
                .IsLoaded = True
                .TextureTimer = GetTickCount() + 100000
            End With

        ElseIf TexType = 9 Then 'skill icons
            If Index <= 0 OrElse Index > NumSkillIcons Then Exit Sub

            'Load texture first, dont care about memory streams (just use the filename)
            SkillIconsGFX(Index) = New Texture(Application.StartupPath & GFX_PATH & "SkillIcons\" & Index & GFX_EXT)
            SkillIconsSprite(Index) = New Sprite(SkillIconsGFX(Index))

            'Cache the width and height
            With SkillIconsGFXInfo(Index)
                .width = SkillIconsGFX(Index).Size.X
                .height = SkillIconsGFX(Index).Size.Y
                .IsLoaded = True
                .TextureTimer = GetTickCount() + 100000
            End With
        ElseIf TexType = 10 Then 'furniture
            If Index <= 0 OrElse Index > NumFurniture Then Exit Sub

            'Load texture first, dont care about memory streams (just use the filename)
            FurnitureGFX(Index) = New Texture(Application.StartupPath & GFX_PATH & "Furniture\" & Index & GFX_EXT)
            FurnitureSprite(Index) = New Sprite(FurnitureGFX(Index))

            'Cache the width and height
            With FurnitureGFXInfo(Index)
                .width = FurnitureGFX(Index).Size.X
                .height = FurnitureGFX(Index).Size.Y
                .IsLoaded = True
                .TextureTimer = GetTickCount() + 100000
            End With
        ElseIf TexType = 11 Then 'projectiles
            If Index <= 0 OrElse Index > NumProjectiles Then Exit Sub

            'Load texture first, dont care about memory streams (just use the filename)
            ProjectileGFX(Index) = New Texture(Application.StartupPath & GFX_PATH & "Projectiles\" & Index & GFX_EXT)
            ProjectileSprite(Index) = New Sprite(ProjectileGFX(Index))

            'Cache the width and height
            With ProjectileGFXInfo(Index)
                .width = ProjectileGFX(Index).Size.X
                .height = ProjectileGFX(Index).Size.Y
                .IsLoaded = True
                .TextureTimer = GetTickCount() + 100000
            End With
        End If

    End Sub

    Friend Sub RenderSprite(TmpSprite As Sprite, Target As RenderWindow, DestX As Integer, DestY As Integer, SourceX As Integer, SourceY As Integer,
           SourceWidth As Integer, SourceHeight As Integer)

        TmpSprite.TextureRect = New IntRect(SourceX, SourceY, SourceWidth, SourceHeight)
        TmpSprite.Position = New Vector2f(DestX, DestY)
        Target.Draw(TmpSprite)
    End Sub

    Friend Sub DrawDirections(X As Integer, Y As Integer)
        Dim rec As Rectangle, i As Integer

        ' render grid
        rec.Y = 24
        rec.X = 0
        rec.Width = 32
        rec.Height = 32

        RenderSprite(DirectionsSprite, GameWindow, ConvertMapX(X * PIC_X), ConvertMapY(Y * PIC_Y), rec.X, rec.Y, rec.Width, rec.Height)

        ' render dir blobs
        For i = 1 To 4
            rec.X = (i - 1) * 8
            rec.Width = 8
            ' find out whether render blocked or not
            If Not IsDirBlocked(Map.Tile(X, Y).DirBlock, (i)) Then
                rec.Y = 8
            Else
                rec.Y = 16
            End If
            rec.Height = 8

            RenderSprite(DirectionsSprite, GameWindow, ConvertMapX(X * PIC_X) + DirArrowX(i), ConvertMapY(Y * PIC_Y) + DirArrowY(i), rec.X, rec.Y, rec.Width, rec.Height)
        Next
    End Sub

    ' BitWise Operators for directional blocking
    Friend Sub SetDirBlock(ByRef blockvar As Byte, ByRef Dir As Byte, block As Boolean)
        If block Then
            blockvar = blockvar Or (2 ^ Dir)
        Else
            blockvar = blockvar And Not (2 ^ Dir)
        End If
    End Sub

    Friend Function IsDirBlocked(ByRef blockvar As Byte, ByRef Dir As Byte) As Boolean
        Return Not (Not blockvar AndAlso (2 ^ Dir))
    End Function

    Friend Function ConvertMapX(X As Integer) As Integer
        ConvertMapX = X - (TileView.Left * PIC_X) - Camera.Left
    End Function

    Friend Function ConvertMapY(Y As Integer) As Integer
        ConvertMapY = Y - (TileView.Top * PIC_Y) - Camera.Top
    End Function

    Friend Sub DrawNpc(MapNpcNum As Integer)
        Dim anim As Byte
        Dim X As Integer
        Dim Y As Integer
        Dim Sprite As Integer, spriteleft As Integer
        Dim destrec As Rectangle
        Dim srcrec As Rectangle
        Dim attackspeed As Integer

        If MapNpc(MapNpcNum).Num = 0 Then Exit Sub ' no npc set

        If MapNpc(MapNpcNum).X < TileView.Left OrElse MapNpc(MapNpcNum).X > TileView.Right Then Exit Sub
        If MapNpc(MapNpcNum).Y < TileView.Top OrElse MapNpc(MapNpcNum).Y > TileView.Bottom Then Exit Sub

        Sprite = Npc(MapNpc(MapNpcNum).Num).Sprite

        If Sprite < 1 OrElse Sprite > NumCharacters Then Exit Sub

        attackspeed = 1000

        ' Reset frame
        anim = 0

        ' Check for attacking animation
        If MapNpc(MapNpcNum).AttackTimer + (attackspeed / 2) > GetTickCount() Then
            If MapNpc(MapNpcNum).Attacking = 1 Then
                anim = 3
            End If
        Else
            ' If not attacking, walk normally
            Select Case MapNpc(MapNpcNum).Dir
                Case DirectionType.Up
                    If (MapNpc(MapNpcNum).YOffset > 8) Then anim = MapNpc(MapNpcNum).Steps
                Case DirectionType.Down
                    If (MapNpc(MapNpcNum).YOffset < -8) Then anim = MapNpc(MapNpcNum).Steps
                Case DirectionType.Left
                    If (MapNpc(MapNpcNum).XOffset > 8) Then anim = MapNpc(MapNpcNum).Steps
                Case DirectionType.Right
                    If (MapNpc(MapNpcNum).XOffset < -8) Then anim = MapNpc(MapNpcNum).Steps
            End Select
        End If

        ' Check to see if we want to stop making him attack
        With MapNpc(MapNpcNum)
            If .AttackTimer + attackspeed < GetTickCount() Then
                .Attacking = 0
                .AttackTimer = 0
            End If
        End With

        ' Set the left
        Select Case MapNpc(MapNpcNum).Dir
            Case DirectionType.Up
                spriteleft = 3
            Case DirectionType.Right
                spriteleft = 2
            Case DirectionType.Down
                spriteleft = 0
            Case DirectionType.Left
                spriteleft = 1
        End Select

        srcrec = New Rectangle((anim) * (CharacterGFXInfo(Sprite).width / 4), spriteleft * (CharacterGFXInfo(Sprite).height / 4), (CharacterGFXInfo(Sprite).width / 4), (CharacterGFXInfo(Sprite).height / 4))

        ' Calculate the X
        X = MapNpc(MapNpcNum).X * PIC_X + MapNpc(MapNpcNum).XOffset - ((CharacterGFXInfo(Sprite).width / 4 - 32) / 2)

        ' Is the player's height more than 32..?
        If (CharacterGFXInfo(Sprite).height / 4) > 32 Then
            ' Create a 32 pixel offset for larger sprites
            Y = MapNpc(MapNpcNum).Y * PIC_Y + MapNpc(MapNpcNum).YOffset - ((CharacterGFXInfo(Sprite).height / 4) - 32)
        Else
            ' Proceed as normal
            Y = MapNpc(MapNpcNum).Y * PIC_Y + MapNpc(MapNpcNum).YOffset
        End If

        destrec = New Rectangle(X, Y, CharacterGFXInfo(Sprite).width / 4, CharacterGFXInfo(Sprite).height / 4)

        DrawCharacter(Sprite, X, Y, srcrec)

    End Sub

    Friend Sub DrawResource(Resource As Integer, dx As Integer, dy As Integer, rec As Rectangle)
        If Resource < 1 OrElse Resource > NumResources Then Exit Sub
        Dim X As Integer
        Dim Y As Integer
        Dim width As Integer
        Dim height As Integer

        X = ConvertMapX(dx)
        Y = ConvertMapY(dy)
        width = (rec.Right - rec.Left)
        height = (rec.Bottom - rec.Top)

        If rec.Width < 0 OrElse rec.Height < 0 Then Exit Sub

        If ResourcesGFXInfo(Resource).IsLoaded = False Then
            LoadTexture(Resource, 5)
        End If

        'seeying we still use it, lets update timer
        With ResourcesGFXInfo(Resource)
            .TextureTimer = GetTickCount() + 100000
        End With

        RenderSprite(ResourcesSprite(Resource), GameWindow, X, Y, rec.X, rec.Y, rec.Width, rec.Height)
    End Sub

    Friend Sub DrawMapResource(Resource_num As Integer)
        Dim Resource_master As Integer

        Dim Resource_state As Integer
        Dim Resource_sprite As Integer
        Dim rec As Rectangle
        Dim X As Integer, Y As Integer

        If GettingMap Then Exit Sub
        If MapData = False Then Exit Sub

        If MapResource(Resource_num).X > Map.MaxX OrElse MapResource(Resource_num).Y > Map.MaxY Then Exit Sub
        ' Get the Resource type
        Resource_master = Map.Tile(MapResource(Resource_num).X, MapResource(Resource_num).Y).Data1

        If Resource_master = 0 Then Exit Sub

        If Resource(Resource_master).ResourceImage = 0 Then Exit Sub

        ' Get the Resource state
        Resource_state = MapResource(Resource_num).ResourceState

        If Resource_state = 0 Then ' normal
            Resource_sprite = Resource(Resource_master).ResourceImage
        ElseIf Resource_state = 1 Then ' used
            Resource_sprite = Resource(Resource_master).ExhaustedImage
        End If

        ' src rect
        With rec
            .Y = 0
            .Height = ResourcesGFXInfo(Resource_sprite).height
            .X = 0
            .Width = ResourcesGFXInfo(Resource_sprite).width
        End With

        ' Set base x + y, then the offset due to size
        X = (MapResource(Resource_num).X * PIC_X) - (ResourcesGFXInfo(Resource_sprite).width / 2) + 16
        Y = (MapResource(Resource_num).Y * PIC_Y) - ResourcesGFXInfo(Resource_sprite).height + 32

        DrawResource(Resource_sprite, X, Y, rec)
    End Sub

    Friend Sub DrawItem(itemnum As Integer)

        Dim srcrec As Rectangle
        Dim destrec As Rectangle
        Dim PicNum As Integer
        Dim x As Integer, y As Integer
        PicNum = Item(MapItem(itemnum).Num).Pic

        If PicNum < 1 OrElse PicNum > NumItems Then Exit Sub

        If ItemsGFXInfo(PicNum).IsLoaded = False Then
            LoadTexture(PicNum, 4)
        End If

        'seeying we still use it, lets update timer
        With ItemsGFXInfo(PicNum)
            .TextureTimer = GetTickCount() + 100000
        End With

        With MapItem(itemnum)
            If .X < TileView.Left OrElse .X > TileView.Right Then Exit Sub
            If .Y < TileView.Top OrElse .Y > TileView.Bottom Then Exit Sub
        End With

        If ItemsGFXInfo(PicNum).width > 32 Then ' has more than 1 frame
            srcrec = New Rectangle((MapItem(itemnum).Frame * 32), 0, 32, 32)
            destrec = New Rectangle(ConvertMapX(MapItem(itemnum).X * PIC_X), ConvertMapY(MapItem(itemnum).Y * PIC_Y), 32, 32)
        Else
            srcrec = New Rectangle(0, 0, PIC_X, PIC_Y)
            destrec = New Rectangle(ConvertMapX(MapItem(itemnum).X * PIC_X), ConvertMapY(MapItem(itemnum).Y * PIC_Y), PIC_X, PIC_Y)
        End If

        x = ConvertMapX(MapItem(itemnum).X * PIC_X)
        y = ConvertMapY(MapItem(itemnum).Y * PIC_Y)

        RenderSprite(ItemsSprite(PicNum), GameWindow, x, y, srcrec.X, srcrec.Y, srcrec.Width, srcrec.Height)
    End Sub

    Friend Sub DrawCharacter(Sprite As Integer, x2 As Integer, y2 As Integer, rec As Rectangle)
        Dim X As Integer
        Dim y As Integer
        Dim width As Integer
        Dim height As Integer
        On Error Resume Next

        If Sprite < 1 OrElse Sprite > NumCharacters Then Exit Sub

        If CharacterGFXInfo(Sprite).IsLoaded = False Then
            LoadTexture(Sprite, 2)
        End If

        'seeying we still use it, lets update timer
        With CharacterGFXInfo(Sprite)
            .TextureTimer = GetTickCount() + 100000
        End With

        X = ConvertMapX(x2)
        y = ConvertMapY(y2)
        width = (rec.Width)
        height = (rec.Height)

        RenderSprite(CharacterSprite(Sprite), GameWindow, X, y, rec.X, rec.Y, rec.Width, rec.Height)
    End Sub

    Friend Sub DrawMapTile(X As Integer, Y As Integer)
        Dim i As Integer
        Dim srcrect As New Rectangle(0, 0, 0, 0)

        If GettingMap Then Exit Sub
        If MapData = False Then Exit Sub

        With Map.Tile(X, Y)
            For i = LayerType.Ground To LayerType.Mask2
                ' skip tile if tileset isn't set
                If .Layer(i).Tileset > 0 AndAlso .Layer(i).Tileset <= NumTileSets Then
                    If TileSetTextureInfo(.Layer(i).Tileset).IsLoaded = False Then
                        LoadTexture(.Layer(i).Tileset, 1)
                    End If
                    ' we use it, lets update timer
                    With TileSetTextureInfo(i)
                        .TextureTimer = GetTickCount() + 100000
                    End With
                    If Autotile(X, Y).Layer(i).renderState = RENDER_STATE_NORMAL Then
                        With srcrect
                            .X = Map.Tile(X, Y).Layer(i).X * 32
                            .Y = Map.Tile(X, Y).Layer(i).Y * 32
                            .Width = 32
                            .Height = 32
                        End With

                        RenderSprite(TileSetSprite(.Layer(i).Tileset), GameWindow, ConvertMapX(X * PIC_X), ConvertMapY(Y * PIC_Y), srcrect.X, srcrect.Y, srcrect.Width, srcrect.Height)

                    ElseIf Autotile(X, Y).Layer(i).renderState = RENDER_STATE_AUTOTILE Then
                        ' Draw autotiles
                        DrawAutoTile(i, ConvertMapX(X * PIC_X), ConvertMapY(Y * PIC_Y), 1, X, Y, 0, False)
                        DrawAutoTile(i, ConvertMapX(X * PIC_X) + 16, ConvertMapY(Y * PIC_Y), 2, X, Y, 0, False)
                        DrawAutoTile(i, ConvertMapX(X * PIC_X), ConvertMapY(Y * PIC_Y) + 16, 3, X, Y, 0, False)
                        DrawAutoTile(i, ConvertMapX(X * PIC_X) + 16, ConvertMapY(Y * PIC_Y) + 16, 4, X, Y, 0, False)
                    End If
                End If
            Next
        End With

    End Sub

    Friend Sub DrawMapFringeTile(X As Integer, Y As Integer)
        Dim i As Integer
        Dim srcrect As New Rectangle(0, 0, 0, 0)
        Dim dest As Rectangle = New Rectangle(frmMapEditor.PointToScreen(frmMapEditor.picScreen.Location), New Size(32, 32))
        'Dim tmpSprite As Sprite

        If GettingMap Then Exit Sub
        If MapData = False Then Exit Sub

        With Map.Tile(X, Y)
            For i = LayerType.Fringe To LayerType.Fringe2
                ' skip tile if tileset isn't set
                If .Layer(i).Tileset > 0 AndAlso .Layer(i).Tileset <= NumTileSets Then
                    If TileSetTextureInfo(.Layer(i).Tileset).IsLoaded = False Then
                        LoadTexture(.Layer(i).Tileset, 1)
                    End If

                    ' we use it, lets update timer
                    With TileSetTextureInfo(.Layer(i).Tileset)
                        .TextureTimer = GetTickCount() + 100000
                    End With

                    ' render
                    If Autotile(X, Y).Layer(i).renderState = RENDER_STATE_NORMAL Then
                        With srcrect
                            .X = Map.Tile(X, Y).Layer(i).X * 32
                            .Y = Map.Tile(X, Y).Layer(i).Y * 32
                            .Width = 32
                            .Height = 32
                        End With

                        RenderSprite(TileSetSprite(.Layer(i).Tileset), GameWindow, ConvertMapX(X * PIC_X), ConvertMapY(Y * PIC_Y), srcrect.X, srcrect.Y, srcrect.Width, srcrect.Height)

                    ElseIf Autotile(X, Y).Layer(i).renderState = RENDER_STATE_AUTOTILE Then
                        ' Draw autotiles
                        DrawAutoTile(i, ConvertMapX(X * PIC_X), ConvertMapY(Y * PIC_Y), 1, X, Y, 0, False)
                        DrawAutoTile(i, ConvertMapX(X * PIC_X) + 16, ConvertMapY(Y * PIC_Y), 2, X, Y, 0, False)
                        DrawAutoTile(i, ConvertMapX(X * PIC_X), ConvertMapY(Y * PIC_Y) + 16, 3, X, Y, 0, False)
                        DrawAutoTile(i, ConvertMapX(X * PIC_X) + 16, ConvertMapY(Y * PIC_Y) + 16, 4, X, Y, 0, False)
                    End If
                End If
            Next
        End With

    End Sub

    Friend Function IsValidMapPoint(X As Integer, Y As Integer) As Boolean
        IsValidMapPoint = False

        If X < 0 Then Exit Function
        If Y < 0 Then Exit Function
        If X > Map.MaxX Then Exit Function
        If Y > Map.MaxY Then Exit Function
        IsValidMapPoint = True
    End Function

    'Friend Sub UpdateCamera()
    '    Dim offsetX As Integer, offsetY As Integer
    '    Dim StartX As Integer, StartY As Integer
    '    Dim EndX As Integer, EndY As Integer

    '    'offsetX = Player(MyIndex).XOffset + PIC_X
    '    'offsetY = Player(MyIndex).YOffset + PIC_Y
    '    'StartX = GetPlayerX(MyIndex) - ((SCREEN_MAPX + 1) \ 2) - 1
    '    'StartY = GetPlayerY(MyIndex) - ((SCREEN_MAPY + 1) \ 2) - 1

    '    If StartX < 0 Then
    '        offsetX = 0

    '        'If StartX = -1 Then
    '        '    If Player(MyIndex).XOffset > 0 Then
    '        '        offsetX = Player(MyIndex).XOffset
    '        '    End If
    '        'End If

    '        StartX = 0
    '    End If

    '    If StartY < 0 Then
    '        offsetY = 0

    '        If StartY = -1 Then
    '            'If Player(MyIndex).YOffset > 0 Then
    '            '    offsetY = Player(MyIndex).YOffset
    '            'End If
    '        End If

    '        StartY = 0
    '    End If

    '    EndX = StartX + (SCREEN_MAPX + 1) + 1
    '    EndY = StartY + (SCREEN_MAPY + 1) + 1

    '    If EndX > Map.MaxX Then
    '        offsetX = 32

    '        If EndX = Map.MaxX + 1 Then
    '            'If Player(MyIndex).XOffset < 0 Then
    '            '    offsetX = Player(MyIndex).XOffset + PIC_X
    '            'End If
    '        End If

    '        EndX = Map.MaxX
    '        StartX = EndX - SCREEN_MAPX - 1
    '    End If

    '    If EndY > Map.MaxY Then
    '        offsetY = 32

    '        If EndY = Map.MaxY + 1 Then
    '            'If Player(MyIndex).YOffset < 0 Then
    '            '    offsetY = Player(MyIndex).YOffset + PIC_Y
    '            'End If
    '        End If

    '        EndY = Map.MaxY
    '        StartY = EndY - SCREEN_MAPY - 1
    '    End If

    '    With TileView
    '        .top = StartY
    '        .bottom = EndY
    '        .left = StartX
    '        .right = EndX
    '    End With

    '    With Camera
    '        .Y = offsetY
    '        .Height = ScreenY + 32
    '        .X = offsetX
    '        .Width = ScreenX + 32
    '    End With

    '    UpdateDrawMapName()

    'End Sub

    Friend Sub UpdateCamera()

        With TileView
            .Top = EditorViewY
            .Bottom = Map.MaxY
            .Left = EditorViewX
            .Right = Map.MaxX
        End With

        With Camera
            .Y = 0
            .Height = Map.MaxY * 32
            .X = 0
            .Width = Map.MaxX * 32
        End With

        UpdateDrawMapName()

    End Sub

    Sub ClearGFX()

        'clear tilesets
        For I = 1 To NumTileSets
            If TileSetTextureInfo(I).IsLoaded Then
                If TileSetTextureInfo(I).TextureTimer < GetTickCount() Then
                    TileSetTexture(I).Dispose()
                    TileSetTextureInfo(I).IsLoaded = False
                    TileSetTextureInfo(I).TextureTimer = 0
                End If
            End If
        Next

        'clear characters
        For I = 1 To NumCharacters
            If CharacterGFXInfo(I).IsLoaded Then
                If CharacterGFXInfo(I).TextureTimer < GetTickCount() Then
                    CharacterGFX(I).Dispose()
                    CharacterGFXInfo(I).IsLoaded = False
                    CharacterGFXInfo(I).TextureTimer = 0
                End If
            End If
        Next

        'clear paperdoll
        For I = 1 To NumPaperdolls
            If PaperDollGFXInfo(I).IsLoaded Then
                If PaperDollGFXInfo(I).TextureTimer < GetTickCount() Then
                    PaperDollGFX(I).Dispose()
                    PaperDollGFXInfo(I).IsLoaded = False
                    PaperDollGFXInfo(I).TextureTimer = 0
                End If
            End If
        Next

        'clear items
        For I = 1 To NumItems
            If ItemsGFXInfo(I).IsLoaded Then
                If ItemsGFXInfo(I).TextureTimer < GetTickCount() Then
                    ItemsGFX(I).Dispose()
                    ItemsGFXInfo(I).IsLoaded = False
                    ItemsGFXInfo(I).TextureTimer = 0
                End If
            End If
        Next

        'clear resources
        For I = 1 To NumResources
            If ResourcesGFXInfo(I).IsLoaded Then
                If ResourcesGFXInfo(I).TextureTimer < GetTickCount() Then
                    ResourcesGFX(I).Dispose()
                    ResourcesGFXInfo(I).IsLoaded = False
                    ResourcesGFXInfo(I).TextureTimer = 0
                End If
            End If
        Next

        'clear faces
        For I = 1 To NumFaces
            If FacesGFXInfo(I).IsLoaded Then
                If FacesGFXInfo(I).TextureTimer < GetTickCount() Then
                    FacesGFX(I).Dispose()
                    FacesGFXInfo(I).IsLoaded = False
                    FacesGFXInfo(I).TextureTimer = 0
                End If
            End If
        Next
    End Sub

    Friend Sub Render_Graphics()
        Dim X As Integer, Y As Integer, I As Integer

        'Don't Render IF
        If GettingMap Then Exit Sub

        'lets get going

        'update view around player
        UpdateCamera()

        'let program do other things
        Application.DoEvents()

        frmMapEditor.picScreen.Width = (Map.MaxX * PIC_X) + PIC_X
        frmMapEditor.picScreen.Height = (Map.MaxY * PIC_Y) + PIC_Y

        'Clear each of our render targets
        GameWindow.DispatchEvents()
        GameWindow.Clear(Color.Black)

        GameWindow.SetView(New View(New FloatRect(0, 0, frmMapEditor.picScreen.Width, frmMapEditor.picScreen.Height)))
        TilesetWindow.SetView(New View(New FloatRect(0, 0, frmMapEditor.picBackSelect.Width, frmMapEditor.picBackSelect.Height)))

        'clear any unused gfx
        ClearGFX()

        ' update animation editor
        'If Editor = EDITOR_ANIMATION Then
        '    EditorAnim_DrawAnim()
        'End If

        If InMapEditor AndAlso MapData = True Then
            ' blit lower tiles
            If NumTileSets > 0 Then
                For X = TileView.Left To TileView.Right + 1
                    For Y = TileView.Top To TileView.Bottom + 1
                        If IsValidMapPoint(X, Y) Then
                            DrawMapTile(X, Y)
                        End If
                    Next
                Next
            End If

            ' events
            If Map.CurrentEvents > 0 AndAlso Map.CurrentEvents <= Map.EventCount Then

                For I = 1 To Map.CurrentEvents
                    If Map.MapEvents(I).Position = 0 Then
                        DrawEvent(I)
                    End If
                Next
            End If

            ' Draw out the items
            If NumItems > 0 Then
                For I = 1 To MAX_MAP_ITEMS

                    If MapItem(I).Num > 0 Then
                        DrawItem(I)
                    End If

                Next
            End If

            'Draw sum d00rs.
            For X = TileView.Left To TileView.Right
                For Y = TileView.Top To TileView.Bottom

                    If IsValidMapPoint(X, Y) Then
                        If Map.Tile(X, Y).Type = TileType.Door Then
                            DrawDoor(X, Y)
                        End If
                    End If

                Next
            Next

            ' Y-based render. Renders Players, Npcs and Resources based on Y-axis.
            For Y = 0 To Map.MaxY

                If NumCharacters > 0 Then

                    ' Npcs
                    For I = 1 To MAX_MAP_NPCS
                        If MapNpc(I).Y = Y Then
                            DrawNpc(I)
                        End If
                    Next

                    ' events
                    If Map.CurrentEvents > 0 AndAlso Map.CurrentEvents <= Map.EventCount Then

                        For I = 1 To Map.CurrentEvents
                            If Map.MapEvents(I).Position = 1 Then
                                If Y = Map.MapEvents(I).Y Then
                                    DrawEvent(I)
                                End If
                            End If
                        Next
                    End If

                End If

                ' Resources
                If NumResources > 0 Then
                    If Resources_Init Then
                        If Resource_Index > 0 Then
                            For I = 1 To Resource_Index
                                If MapResource(I).Y = Y Then
                                    DrawMapResource(I)
                                End If
                            Next
                        End If
                    End If
                End If
            Next

            'events
            If Map.CurrentEvents > 0 AndAlso Map.CurrentEvents <= Map.EventCount Then

                For I = 1 To Map.CurrentEvents
                    If Map.MapEvents(I).Position = 2 Then
                        DrawEvent(I)
                    End If
                Next
            End If

            ' blit out upper tiles
            If NumTileSets > 0 Then
                For X = TileView.Left To TileView.Right + 1
                    For Y = TileView.Top To TileView.Bottom + 1
                        If IsValidMapPoint(X, Y) Then
                            DrawMapFringeTile(X, Y)
                        End If
                    Next
                Next
            End If

            DrawWeather()
            DrawThunderEffect()
            DrawMapTint()

            ' Draw out a square at mouse cursor
            If MapGrid = True Then
                DrawGrid()
            End If

            If SelectedTab = 4 Then
                For X = TileView.Left To TileView.Right
                    For Y = TileView.Top To TileView.Bottom
                        If IsValidMapPoint(X, Y) Then
                            DrawDirections(X, Y)
                        End If
                    Next
                Next
            End If

            'draw event names
            For I = 0 To Map.CurrentEvents
                If Map.MapEvents(I).Visible = 1 Then
                    If Map.MapEvents(I).ShowName = 1 Then
                        DrawEventName(I)
                    End If
                End If
            Next

            ' draw npc names
            For I = 1 To MAX_MAP_NPCS
                If MapNpc(I).Num > 0 Then
                    DrawNPCName(I)
                End If
            Next

            If CurrentFog > 0 Then
                DrawFog()
            End If

            ' Blit out map attributes
            If InMapEditor Then
                DrawMapAttributes()
                DrawTileOutline()
            End If

            If InMapEditor AndAlso SelectedTab = 5 Then
                DrawEvents()
                EditorEvent_DrawGraphic()
            End If

            ' Draw map name
            DrawMapName()
        End If

        'and finally show everything on screen
        GameWindow.Display()
    End Sub

    Sub DrawMapName()
        DrawText(DrawMapNameX, DrawMapNameY, Map.Name, DrawMapNameColor, Color.Black, GameWindow)
    End Sub

    Friend Sub DrawDoor(X As Integer, Y As Integer)
        Dim rec As Rectangle

        Dim x2 As Integer, y2 As Integer

        ' sort out animation
        With TempTile(X, Y)
            If .DoorAnimate = 1 Then ' opening
                If .DoorTimer + 100 < GetTickCount() Then
                    If .DoorFrame < 4 Then
                        .DoorFrame = .DoorFrame + 1
                    Else
                        .DoorAnimate = 2 ' set to closing
                    End If
                    .DoorTimer = GetTickCount()
                End If
            ElseIf .DoorAnimate = 2 Then ' closing
                If .DoorTimer + 100 < GetTickCount() Then
                    If .DoorFrame > 1 Then
                        .DoorFrame = .DoorFrame - 1
                    Else
                        .DoorAnimate = 0 ' end animation
                    End If
                    .DoorTimer = GetTickCount()
                End If
            End If

            If .DoorFrame = 0 Then .DoorFrame = 1
        End With

        With rec
            .Y = 0
            .Height = DoorGFXInfo.height
            .X = ((TempTile(X, Y).DoorFrame - 1) * DoorGFXInfo.width / 4)
            .Width = DoorGFXInfo.width / 4
        End With

        x2 = (X * PIC_X)
        y2 = (Y * PIC_Y) - (DoorGFXInfo.height / 2) + 4

        RenderSprite(DoorSprite, GameWindow, ConvertMapX(X * PIC_X), ConvertMapY(Y * PIC_Y), rec.X, rec.Y, rec.Width, rec.Height)
    End Sub

    Friend Sub DrawTileOutline()
        Dim rec As Rectangle, tileset As Integer
        If SelectedTab = 4 OrElse HideCursor = True Then Exit Sub

        With rec
            .Y = 0
            .Height = PIC_Y
            .X = 0
            .Width = PIC_X
        End With

        tileset = frmMapEditor.cmbTileSets.SelectedIndex + 1

        ' exit out if doesn't exist
        If tileset <= 0 OrElse tileset > NumTileSets Then Exit Sub

        Dim rec2 As New RectangleShape
        rec2.OutlineColor = New Color(Color.Blue)
        rec2.OutlineThickness = 0.6
        rec2.FillColor = New Color(Color.Transparent)

        If SelectedTab = 2 Then
            'RenderTexture(MiscGFX, GameWindow, ConvertMapX(CurX * PIC_X), ConvertMapY(CurY * PIC_Y), rec.X, rec.Y, rec.Width, rec.Height)
            rec2.Size = New Vector2f(rec.Width, rec.Height)
        Else
            If TileSetTextureInfo(frmMapEditor.cmbTileSets.SelectedIndex + 1).IsLoaded = False Then
                LoadTexture(frmMapEditor.cmbTileSets.SelectedIndex + 1, 1)
            End If
            ' we use it, lets update timer
            With TileSetTextureInfo(frmMapEditor.cmbTileSets.SelectedIndex + 1)
                .TextureTimer = GetTickCount() + 100000
            End With

            If EditorTileWidth = 1 AndAlso EditorTileHeight = 1 Then
                RenderSprite(TileSetSprite(frmMapEditor.cmbTileSets.SelectedIndex + 1), GameWindow, ConvertMapX(CurX * PIC_X), ConvertMapY(CurY * PIC_Y), EditorTileSelStart.X * PIC_X, EditorTileSelStart.Y * PIC_Y, rec.Width, rec.Height)
                rec2.Size = New Vector2f(rec.Width, rec.Height)
            Else
                If frmMapEditor.cmbAutoTile.SelectedIndex > 0 Then
                    RenderSprite(TileSetSprite(frmMapEditor.cmbTileSets.SelectedIndex + 1), GameWindow, ConvertMapX(CurX * PIC_X), ConvertMapY(CurY * PIC_Y), EditorTileSelStart.X * PIC_X, EditorTileSelStart.Y * PIC_Y, rec.Width, rec.Height)
                    rec2.Size = New Vector2f(rec.Width, rec.Height)
                Else
                    RenderSprite(TileSetSprite(frmMapEditor.cmbTileSets.SelectedIndex + 1), GameWindow, ConvertMapX(CurX * PIC_X), ConvertMapY(CurY * PIC_Y), EditorTileSelStart.X * PIC_X, EditorTileSelStart.Y * PIC_Y, EditorTileSelEnd.X * PIC_X, EditorTileSelEnd.Y * PIC_Y)
                    rec2.Size = New Vector2f(EditorTileSelEnd.X * PIC_X, EditorTileSelEnd.Y * PIC_Y)
                End If

            End If

        End If

        rec2.Position = New Vector2f(ConvertMapX(CurX * PIC_X), ConvertMapY(CurY * PIC_Y))
        GameWindow.Draw(rec2)
    End Sub

    Friend Sub DrawGrid()

        Dim rec As New RectangleShape

        For x = TileView.Left To TileView.Right ' - 1

            For y = TileView.Top To TileView.Bottom ' - 1

                If IsValidMapPoint(x, y) Then

                    rec.OutlineColor = New Color(Color.White)
                    rec.OutlineThickness = 0.6
                    rec.FillColor = New Color(Color.Transparent)
                    rec.Size = New Vector2f((x * PIC_X), (y * PIC_X))
                    rec.Position = New Vector2f(ConvertMapX((x - 1) * PIC_X), ConvertMapY((y - 1) * PIC_Y))

                    GameWindow.Draw(rec)
                End If

            Next

        Next

    End Sub

    Friend Sub DrawMapTint()
        'If InMapEditor Then Exit Sub

        If Map.HasMapTint = 0 Then Exit Sub

        MapTintSprite = New Sprite(New Texture(New SFML.Graphics.Image((Map.MaxX * PIC_X), (Map.MaxY * PIC_Y), SFML.Graphics.Color.White)))
        MapTintSprite.Color = New Color(CurrentTintR, CurrentTintG, CurrentTintB, CurrentTintA)
        MapTintSprite.TextureRect = New IntRect(0, 0, (Map.MaxX * PIC_X) + PIC_X, (Map.MaxY * PIC_Y) + PIC_Y)

        MapTintSprite.Position = New Vector2f(0, 0)

        GameWindow.Draw(MapTintSprite)

    End Sub

    Friend Sub EditorMap_DrawTileset()
        Dim height As Integer
        Dim width As Integer
        Dim tileset As Byte

        TilesetWindow.DispatchEvents()
        TilesetWindow.Clear(Color.Black)

        ' find tileset number
        tileset = frmMapEditor.cmbTileSets.SelectedIndex + 1

        ' exit out if doesn't exist
        If tileset <= 0 OrElse tileset > NumTileSets Then Exit Sub

        Dim rec2 As New RectangleShape With {
            .OutlineColor = New Color(Color.Red),
            .OutlineThickness = 0.6,
            .FillColor = New Color(Color.Transparent)
        }

        If TileSetTextureInfo(tileset).IsLoaded = False Then
            LoadTexture(tileset, 1)
        End If
        ' we use it, lets update timer
        With TileSetTextureInfo(tileset)
            .TextureTimer = GetTickCount() + 100000
        End With

        height = TileSetTextureInfo(tileset).height
        width = TileSetTextureInfo(tileset).width
        frmMapEditor.picBackSelect.Height = height
        frmMapEditor.picBackSelect.Width = width

        TilesetWindow.SetView(New SFML.Graphics.View(New SFML.Graphics.FloatRect(0, 0, width, height)))

        ' change selected shape for autotiles
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

        RenderSprite(TileSetSprite(tileset), TilesetWindow, 0, 0, 0, 0, width, height)

        rec2.Size = New Vector2f(EditorTileWidth * PIC_X, EditorTileHeight * PIC_Y)

        rec2.Position = New Vector2f(EditorTileSelStart.X * PIC_X, EditorTileSelStart.Y * PIC_Y)
        TilesetWindow.Draw(rec2)

        'and finally show everything on screen
        TilesetWindow.Display()

        LastTileset = tileset
    End Sub

    Sub DestroyGraphics()

        ' Number of graphic files
        If Not MapEditorBackBuffer Is Nothing Then MapEditorBackBuffer.Dispose()

        For i = 0 To NumAnimations
            If Not AnimationsGFX(i) Is Nothing Then AnimationsGFX(i).Dispose()
        Next i

        For i = 0 To NumCharacters
            If Not CharacterGFX(i) Is Nothing Then CharacterGFX(i).Dispose()
        Next

        For i = 0 To NumItems
            If Not ItemsGFX(i) Is Nothing Then ItemsGFX(i).Dispose()
        Next

        For i = 0 To NumPaperdolls
            If Not PaperDollGFX(i) Is Nothing Then PaperDollGFX(i).Dispose()
        Next

        For i = 0 To NumResources
            If Not ResourcesGFX(i) Is Nothing Then ResourcesGFX(i).Dispose()
        Next

        For i = 0 To NumSkillIcons
            If Not SkillIconsGFX(i) Is Nothing Then SkillIconsGFX(i).Dispose()
        Next

        For i = 0 To NumTileSets
            'If Not TileSetImgsGFX(i) Is Nothing Then TileSetImgsGFX(i).Dispose()
            If Not TileSetTexture(i) Is Nothing Then TileSetTexture(i).Dispose()
        Next i

        For i = 0 To NumFurniture
            If Not FurnitureGFX(i) Is Nothing Then FurnitureGFX(i).Dispose()
        Next

        For i = 0 To NumFaces
            If Not FacesGFX(i) Is Nothing Then FacesGFX(i).Dispose()
        Next

        For i = 0 To NumFogs
            If Not FogGFX(i) Is Nothing Then FogGFX(i).Dispose()
        Next

        If Not DoorGFX Is Nothing Then DoorGFX.Dispose()
        If Not DirectionsGfx Is Nothing Then DirectionsGfx.Dispose()
        If Not WeatherGFX Is Nothing Then WeatherGFX.Dispose()

    End Sub

    Friend Sub EditorMap_DrawMapItem()
        Dim itemnum As Integer
        itemnum = Item(frmMapEditor.scrlMapItem.Value).Pic

        If itemnum < 1 OrElse itemnum > NumItems Then
            frmMapEditor.picMapItem.BackgroundImage = Nothing
            Exit Sub
        End If

        If File.Exists(Application.StartupPath & GFX_PATH & "items\" & itemnum & GFX_EXT) Then
            frmMapEditor.picMapItem.BackgroundImage = Drawing.Image.FromFile(Application.StartupPath & GFX_PATH & "items\" & itemnum & GFX_EXT)
        End If

    End Sub

    Friend Sub EditorMap_DrawKey()
        Dim itemnum As Integer

        itemnum = Item(frmMapEditor.scrlMapKey.Value).Pic

        If itemnum < 1 OrElse itemnum > NumItems Then
            frmMapEditor.picMapKey.BackgroundImage = Nothing
            Exit Sub
        End If

        If File.Exists(Application.StartupPath & GFX_PATH & "items\" & itemnum & GFX_EXT) Then
            frmMapEditor.picMapKey.BackgroundImage = Drawing.Image.FromFile(Application.StartupPath & GFX_PATH & "items\" & itemnum & GFX_EXT)
        End If

    End Sub

    Friend Sub EditorItem_DrawItem()
        Dim itemnum As Integer
        itemnum = frmItem.nudPic.Value

        If itemnum < 1 OrElse itemnum > NumItems Then
            frmItem.picItem.BackgroundImage = Nothing
            Exit Sub
        End If

        If File.Exists(Application.StartupPath & GFX_PATH & "items\" & itemnum & GFX_EXT) Then
            frmItem.picItem.BackgroundImage = Drawing.Image.FromFile(Application.StartupPath & GFX_PATH & "items\" & itemnum & GFX_EXT)
        End If

    End Sub

    Friend Sub EditorItem_DrawPaperdoll()
        Dim Sprite As Integer

        Sprite = frmItem.nudPaperdoll.Value

        If Sprite < 1 OrElse Sprite > NumPaperdolls Then
            frmItem.picPaperdoll.BackgroundImage = Nothing
            Exit Sub
        End If

        If File.Exists(Application.StartupPath & GFX_PATH & "paperdolls\" & Sprite & GFX_EXT) Then
            frmItem.picPaperdoll.BackgroundImage = Drawing.Image.FromFile(Application.StartupPath & GFX_PATH & "paperdolls\" & Sprite & GFX_EXT)
        End If
    End Sub

    Friend Sub EditorItem_DrawFurniture()
        Dim Furniturenum As Integer
        Dim sRECT As Rectangle
        Dim dRECT As Rectangle
        Furniturenum = frmItem.nudFurniture.Value

        If Furniturenum < 1 OrElse Furniturenum > NumFurniture Then
            EditorItem_Furniture.Clear(ToSFMLColor(frmItem.picFurniture.BackColor))
            EditorItem_Furniture.Display()
            Exit Sub
        End If

        If FurnitureGFXInfo(Furniturenum).IsLoaded = False Then
            LoadTexture(Furniturenum, 10)
        End If

        'seeying we still use it, lets update timer
        With FurnitureGFXInfo(Furniturenum)
            .TextureTimer = GetTickCount() + 100000
        End With

        ' rect for source
        With sRECT
            .Y = 0
            .Height = FurnitureGFXInfo(Furniturenum).height
            .X = 0
            .Width = FurnitureGFXInfo(Furniturenum).width
        End With

        ' same for destination as source
        dRECT = sRECT

        EditorItem_Furniture.Clear(ToSFMLColor(frmItem.picFurniture.BackColor))

        RenderSprite(FurnitureSprite(Furniturenum), EditorItem_Furniture, dRECT.X, dRECT.Y, sRECT.X, sRECT.Y, sRECT.Width, sRECT.Height)

        If frmItem.optSetBlocks.Checked = True Then
            For X = 0 To 3
                For Y = 0 To 3
                    If X <= (FurnitureGFXInfo(Furniturenum).width / 32) - 1 Then
                        If Y <= (FurnitureGFXInfo(Furniturenum).height / 32) - 1 Then
                            If Item(EditorIndex).FurnitureBlocks(X, Y) = 1 Then
                                DrawText(X * 32 + 8, Y * 32 + 8, "X", Color.Red, Color.Black, EditorItem_Furniture)
                            Else
                                DrawText(X * 32 + 8, Y * 32 + 8, "O", Color.Blue, Color.Black, EditorItem_Furniture)
                            End If
                        End If
                    End If
                Next
            Next
        ElseIf frmItem.optSetFringe.Checked = True Then
            For X = 0 To 3
                For Y = 0 To 3
                    If X <= (FurnitureGFXInfo(Furniturenum).width / 32) - 1 Then
                        If Y <= (FurnitureGFXInfo(Furniturenum).height / 32) Then
                            If Item(EditorIndex).FurnitureFringe(X, Y) = 1 Then
                                DrawText(X * 32 + 8, Y * 32 + 8, "O", Color.Blue, Color.Black, EditorItem_Furniture)
                            End If
                        End If
                    End If
                Next
            Next
        End If
        EditorItem_Furniture.Display()
    End Sub

    Friend Sub EditorNpc_DrawSprite()
        Dim Sprite As Integer

        Sprite = frmNPC.nudSprite.Value

        If Sprite < 1 OrElse Sprite > NumCharacters Then
            frmNPC.picSprite.BackgroundImage = Nothing
            Exit Sub
        End If

        If File.Exists(Application.StartupPath & GFX_PATH & "characters\" & Sprite & GFX_EXT) Then
            frmNPC.picSprite.Width = Drawing.Image.FromFile(Application.StartupPath & GFX_PATH & "characters\" & Sprite & GFX_EXT).Width / 4
            frmNPC.picSprite.Height = Drawing.Image.FromFile(Application.StartupPath & GFX_PATH & "characters\" & Sprite & GFX_EXT).Height / 4
            frmNPC.picSprite.BackgroundImage = Drawing.Image.FromFile(Application.StartupPath & GFX_PATH & "characters\" & Sprite & GFX_EXT)
        End If
    End Sub

    Friend Sub EditorResource_DrawSprite()
        Dim Sprite As Integer

        ' normal sprite
        Sprite = frmResource.nudNormalPic.Value

        If Sprite < 1 OrElse Sprite > NumResources Then
            frmResource.picNormalpic.BackgroundImage = Nothing
        Else
            If File.Exists(Application.StartupPath & GFX_PATH & "resources\" & Sprite & GFX_EXT) Then
                frmResource.picNormalpic.BackgroundImage = Drawing.Image.FromFile(Application.StartupPath & GFX_PATH & "resources\" & Sprite & GFX_EXT)
            End If
        End If

        ' exhausted sprite
        Sprite = frmResource.nudExhaustedPic.Value

        If Sprite < 1 OrElse Sprite > NumResources Then
            frmResource.picExhaustedPic.BackgroundImage = Nothing
        Else
            If File.Exists(Application.StartupPath & GFX_PATH & "resources\" & Sprite & GFX_EXT) Then
                frmResource.picExhaustedPic.BackgroundImage = Drawing.Image.FromFile(Application.StartupPath & GFX_PATH & "resources\" & Sprite & GFX_EXT)
            End If
        End If
    End Sub

    Friend Sub EditorSkill_BltIcon()
        Dim iconnum As Integer
        Dim sRECT As Rectangle
        Dim dRECT As Rectangle
        iconnum = frmSkill.nudIcon.Value

        If iconnum < 1 OrElse iconnum > NumSkillIcons Then
            EditorSkill_Icon.Clear(ToSFMLColor(frmSkill.picSprite.BackColor))
            EditorSkill_Icon.Display()
            Exit Sub
        End If

        If SkillIconsGFXInfo(iconnum).IsLoaded = False Then
            LoadTexture(iconnum, 9)
        End If

        'seeying we still use it, lets update timer
        With SkillIconsGFXInfo(iconnum)
            .TextureTimer = GetTickCount() + 100000
        End With

        With sRECT
            .Y = 0
            .Height = PIC_Y
            .X = 0
            .Width = PIC_X
        End With

        'drect is the same, so just copy it
        dRECT = sRECT

        EditorSkill_Icon.Clear(ToSFMLColor(frmSkill.picSprite.BackColor))

        RenderSprite(SkillIconsSprite(iconnum), EditorSkill_Icon, dRECT.X, dRECT.Y, sRECT.X, sRECT.Y, sRECT.Width, sRECT.Height)

        EditorSkill_Icon.Display()
    End Sub

    Friend Sub EditorAnim_DrawAnim()
        Dim Animationnum As Integer
        Dim sRECT As Rectangle
        Dim dRECT As Rectangle
        Dim width As Integer, height As Integer
        Dim looptime As Integer
        Dim FrameCount As Integer
        Dim ShouldRender As Boolean

        Animationnum = frmAnimation.nudSprite0.Value

        If Animationnum < 1 OrElse Animationnum > NumAnimations Then
            EditorAnimation_Anim1.Clear(ToSFMLColor(frmAnimation.picSprite0.BackColor))
            EditorAnimation_Anim1.Display()
        Else
            If AnimationsGFXInfo(Animationnum).IsLoaded = False Then
                LoadTexture(Animationnum, 6)
            End If

            'seeying we still use it, lets update timer
            With AnimationsGFXInfo(Animationnum)
                .TextureTimer = GetTickCount() + 100000
            End With

            looptime = frmAnimation.nudLoopTime0.Value
            FrameCount = frmAnimation.nudFrameCount0.Value

            ShouldRender = False

            ' check if we need to render new frame
            If AnimEditorTimer(0) + looptime <= GetTickCount() Then
                ' check if out of range
                If AnimEditorFrame(0) >= FrameCount Then
                    AnimEditorFrame(0) = 1
                Else
                    AnimEditorFrame(0) = AnimEditorFrame(0) + 1
                End If
                AnimEditorTimer(0) = GetTickCount()
                ShouldRender = True
            End If

            If ShouldRender Then
                If frmAnimation.nudFrameCount0.Value > 0 Then
                    ' total width divided by frame count
                    height = AnimationsGFXInfo(Animationnum).height
                    width = AnimationsGFXInfo(Animationnum).width / frmAnimation.nudFrameCount0.Value

                    With sRECT
                        .Y = 0
                        .Height = height
                        .X = (AnimEditorFrame(0) - 1) * width
                        .Width = width
                    End With

                    With dRECT
                        .Y = 0
                        .Height = height
                        .X = 0
                        .Width = width
                    End With

                    EditorAnimation_Anim1.Clear(ToSFMLColor(frmAnimation.picSprite0.BackColor))

                    RenderSprite(AnimationsSprite(Animationnum), EditorAnimation_Anim1, dRECT.X, dRECT.Y, sRECT.X, sRECT.Y, sRECT.Width, sRECT.Height)

                    EditorAnimation_Anim1.Display()
                End If
            End If
        End If

        Animationnum = frmAnimation.nudSprite1.Value

        If Animationnum < 1 OrElse Animationnum > NumAnimations Then
            EditorAnimation_Anim2.Clear(ToSFMLColor(frmAnimation.picSprite1.BackColor))
            EditorAnimation_Anim2.Display()
        Else
            If AnimationsGFXInfo(Animationnum).IsLoaded = False Then
                LoadTexture(Animationnum, 6)
            End If

            'seeying we still use it, lets update timer
            With AnimationsGFXInfo(Animationnum)
                .TextureTimer = GetTickCount() + 100000
            End With

            looptime = frmAnimation.nudLoopTime1.Value
            FrameCount = frmAnimation.nudFrameCount1.Value

            ShouldRender = False

            ' check if we need to render new frame
            If AnimEditorTimer(1) + looptime <= GetTickCount() Then
                ' check if out of range
                If AnimEditorFrame(1) >= FrameCount Then
                    AnimEditorFrame(1) = 1
                Else
                    AnimEditorFrame(1) = AnimEditorFrame(1) + 1
                End If
                AnimEditorTimer(1) = GetTickCount()
                ShouldRender = True
            End If

            If ShouldRender Then
                If frmAnimation.nudFrameCount1.Value > 0 Then
                    ' total width divided by frame count
                    height = AnimationsGFXInfo(Animationnum).height
                    width = AnimationsGFXInfo(Animationnum).width / frmAnimation.nudFrameCount1.Value

                    With sRECT
                        .Y = 0
                        .Height = height
                        .X = (AnimEditorFrame(1) - 1) * width
                        .Width = width
                    End With

                    With dRECT
                        .Y = 0
                        .Height = height
                        .X = 0
                        .Width = width
                    End With

                    EditorAnimation_Anim2.Clear(ToSFMLColor(frmAnimation.picSprite1.BackColor))

                    RenderSprite(AnimationsSprite(Animationnum), EditorAnimation_Anim2, dRECT.X, dRECT.Y, sRECT.X, sRECT.Y, sRECT.Width, sRECT.Height)
                    EditorAnimation_Anim2.Display()

                End If
            End If
        End If
    End Sub

    Friend Sub UpdateDrawMapName()
        Dim g As Graphics = Graphics.FromImage(New Bitmap(1, 1))
        Dim width As Integer
        width = g.MeasureString(Trim$(Map.Name), New Drawing.Font(FONT_NAME, FONT_SIZE, FontStyle.Bold, GraphicsUnit.Pixel)).Width
        DrawMapNameX = ((SCREEN_MAPX + 1) * PIC_X / 2) - width + 32
        DrawMapNameY = 1

        Select Case Map.Moral
            Case MapMoralType.None
                DrawMapNameColor = Color.Red
            Case MapMoralType.Safe
                DrawMapNameColor = Color.Green
            Case Else
                DrawMapNameColor = Color.White
        End Select
        g.Dispose()
    End Sub

    Friend Function ToSFMLColor(ToConvert As Drawing.Color) As Color
        Return New Color(ToConvert.R, ToConvert.G, ToConvert.G, ToConvert.A)
    End Function

End Module