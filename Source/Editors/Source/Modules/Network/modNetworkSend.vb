Imports System.Windows.Forms
Imports ASFW
Imports ASFW.IO

Module modNetworkSend
    Friend Sub SendEditorLogin(Name As String, Password As String)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.EditorLogin)
        Buffer.WriteString(EKeyPair.EncryptString(Name))
        Buffer.WriteString(EKeyPair.EncryptString(Password))
        Buffer.WriteString(EKeyPair.EncryptString(Application.ProductVersion))
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendEditorMap()
        Dim X As Integer, Y As Integer, i As Integer
        Dim data() As Byte
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(Map.MapNum)

        Buffer.WriteString(Trim$(Map.Name))
        Buffer.WriteString(Trim$(Map.Music))
        Buffer.WriteInt32(Map.Moral)
        Buffer.WriteInt32(Map.tileset)
        Buffer.WriteInt32(Map.Up)
        Buffer.WriteInt32(Map.Down)
        Buffer.WriteInt32(Map.Left)
        Buffer.WriteInt32(Map.Right)
        Buffer.WriteInt32(Map.BootMap)
        Buffer.WriteInt32(Map.BootX)
        Buffer.WriteInt32(Map.BootY)
        Buffer.WriteInt32(Map.MaxX)
        Buffer.WriteInt32(Map.MaxY)
        Buffer.WriteInt32(Map.WeatherType)
        Buffer.WriteInt32(Map.FogIndex)
        Buffer.WriteInt32(Map.WeatherIntensity)
        Buffer.WriteInt32(Map.FogAlpha)
        Buffer.WriteInt32(Map.FogSpeed)
        Buffer.WriteInt32(Map.HasMapTint)
        Buffer.WriteInt32(Map.MapTintR)
        Buffer.WriteInt32(Map.MapTintG)
        Buffer.WriteInt32(Map.MapTintB)
        Buffer.WriteInt32(Map.MapTintA)
        Buffer.WriteInt32(Map.Instanced)
        Buffer.WriteInt32(Map.Panorama)
        Buffer.WriteInt32(Map.Parallax)

        For i = 1 To MAX_MAP_NPCS
            Buffer.WriteInt32(Map.Npc(i))
        Next

        For X = 0 To Map.MaxX
            For Y = 0 To Map.MaxY
                Buffer.WriteInt32(Map.Tile(X, Y).Data1)
                Buffer.WriteInt32(Map.Tile(X, Y).Data2)
                Buffer.WriteInt32(Map.Tile(X, Y).Data3)
                Buffer.WriteInt32(Map.Tile(X, Y).DirBlock)
                For i = 0 To LayerType.Count - 1
                    Buffer.WriteInt32(Map.Tile(X, Y).Layer(i).Tileset)
                    Buffer.WriteInt32(Map.Tile(X, Y).Layer(i).X)
                    Buffer.WriteInt32(Map.Tile(X, Y).Layer(i).Y)
                    Buffer.WriteInt32(Map.Tile(X, Y).Layer(i).AutoTile)
                Next
                Buffer.WriteInt32(Map.Tile(X, Y).Type)
            Next
        Next

        'Event Data
        Buffer.WriteInt32(Map.EventCount)
        If Map.EventCount > 0 Then
            For i = 1 To Map.EventCount
                With Map.Events(i)
                    Buffer.WriteString(Trim$(.Name))
                    Buffer.WriteInt32(.Globals)
                    Buffer.WriteInt32(.X)
                    Buffer.WriteInt32(.Y)
                    Buffer.WriteInt32(.PageCount)
                End With
                If Map.Events(i).PageCount > 0 Then
                    For X = 1 To Map.Events(i).PageCount
                        With Map.Events(i).Pages(X)
                            Buffer.WriteInt32(.chkVariable)
                            Buffer.WriteInt32(.VariableIndex)
                            Buffer.WriteInt32(.VariableCondition)
                            Buffer.WriteInt32(.VariableCompare)
                            Buffer.WriteInt32(.chkSwitch)
                            Buffer.WriteInt32(.SwitchIndex)
                            Buffer.WriteInt32(.SwitchCompare)
                            Buffer.WriteInt32(.chkHasItem)
                            Buffer.WriteInt32(.HasItemIndex)
                            Buffer.WriteInt32(.HasItemAmount)
                            Buffer.WriteInt32(.chkSelfSwitch)
                            Buffer.WriteInt32(.SelfSwitchIndex)
                            Buffer.WriteInt32(.SelfSwitchCompare)
                            Buffer.WriteInt32(.GraphicType)
                            Buffer.WriteInt32(.Graphic)
                            Buffer.WriteInt32(.GraphicX)
                            Buffer.WriteInt32(.GraphicY)
                            Buffer.WriteInt32(.GraphicX2)
                            Buffer.WriteInt32(.GraphicY2)
                            Buffer.WriteInt32(.MoveType)
                            Buffer.WriteInt32(.MoveSpeed)
                            Buffer.WriteInt32(.MoveFreq)
                            Buffer.WriteInt32(Map.Events(i).Pages(X).MoveRouteCount)
                            Buffer.WriteInt32(.IgnoreMoveRoute)
                            Buffer.WriteInt32(.RepeatMoveRoute)
                            If .MoveRouteCount > 0 Then
                                For Y = 1 To .MoveRouteCount
                                    Buffer.WriteInt32(.MoveRoute(Y).Index)
                                    Buffer.WriteInt32(.MoveRoute(Y).Data1)
                                    Buffer.WriteInt32(.MoveRoute(Y).Data2)
                                    Buffer.WriteInt32(.MoveRoute(Y).Data3)
                                    Buffer.WriteInt32(.MoveRoute(Y).Data4)
                                    Buffer.WriteInt32(.MoveRoute(Y).Data5)
                                    Buffer.WriteInt32(.MoveRoute(Y).Data6)
                                Next
                            End If
                            Buffer.WriteInt32(.WalkAnim)
                            Buffer.WriteInt32(.DirFix)
                            Buffer.WriteInt32(.WalkThrough)
                            Buffer.WriteInt32(.ShowName)
                            Buffer.WriteInt32(.Trigger)
                            Buffer.WriteInt32(.CommandListCount)
                            Buffer.WriteInt32(.Position)
                            Buffer.WriteInt32(.Questnum)

                            Buffer.WriteInt32(.chkPlayerGender)
                        End With
                        If Map.Events(i).Pages(X).CommandListCount > 0 Then
                            For Y = 1 To Map.Events(i).Pages(X).CommandListCount
                                Buffer.WriteInt32(Map.Events(i).Pages(X).CommandList(Y).CommandCount)
                                Buffer.WriteInt32(Map.Events(i).Pages(X).CommandList(Y).ParentList)
                                If Map.Events(i).Pages(X).CommandList(Y).CommandCount > 0 Then
                                    For z = 1 To Map.Events(i).Pages(X).CommandList(Y).CommandCount
                                        With Map.Events(i).Pages(X).CommandList(Y).Commands(z)
                                            Buffer.WriteInt32(.Index)
                                            Buffer.WriteString(Trim$(.Text1))
                                            Buffer.WriteString(Trim$(.Text2))
                                            Buffer.WriteString(Trim$(.Text3))
                                            Buffer.WriteString(Trim$(.Text4))
                                            Buffer.WriteString(Trim$(.Text5))
                                            Buffer.WriteInt32(.Data1)
                                            Buffer.WriteInt32(.Data2)
                                            Buffer.WriteInt32(.Data3)
                                            Buffer.WriteInt32(.Data4)
                                            Buffer.WriteInt32(.Data5)
                                            Buffer.WriteInt32(.Data6)
                                            Buffer.WriteInt32(.ConditionalBranch.CommandList)
                                            Buffer.WriteInt32(.ConditionalBranch.Condition)
                                            Buffer.WriteInt32(.ConditionalBranch.Data1)
                                            Buffer.WriteInt32(.ConditionalBranch.Data2)
                                            Buffer.WriteInt32(.ConditionalBranch.Data3)
                                            Buffer.WriteInt32(.ConditionalBranch.ElseCommandList)
                                            Buffer.WriteInt32(.MoveRouteCount)
                                            If .MoveRouteCount > 0 Then
                                                For w = 1 To .MoveRouteCount
                                                    Buffer.WriteInt32(.MoveRoute(w).Index)
                                                    Buffer.WriteInt32(.MoveRoute(w).Data1)
                                                    Buffer.WriteInt32(.MoveRoute(w).Data2)
                                                    Buffer.WriteInt32(.MoveRoute(w).Data3)
                                                    Buffer.WriteInt32(.MoveRoute(w).Data4)
                                                    Buffer.WriteInt32(.MoveRoute(w).Data5)
                                                    Buffer.WriteInt32(.MoveRoute(w).Data6)
                                                Next
                                            End If
                                        End With
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
            Next
        End If
        'End Event Data

        data = Buffer.ToArray

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(EditorPackets.EditorSaveMap)
        Buffer.WriteBlock(Compression.CompressBytes(data))

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendRequestItems()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestItems)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendSaveItem(itemNum As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.SaveItem)
        Buffer.WriteInt32(itemNum)
        Buffer.WriteInt32(Item(itemNum).AccessReq)

        For i = 0 To StatType.Count - 1
            Buffer.WriteInt32(Item(itemNum).Add_Stat(i))
        Next

        Buffer.WriteInt32(Item(itemNum).Animation)
        Buffer.WriteInt32(Item(itemNum).BindType)
        Buffer.WriteInt32(Item(itemNum).ClassReq)
        Buffer.WriteInt32(Item(itemNum).Data1)
        Buffer.WriteInt32(Item(itemNum).Data2)
        Buffer.WriteInt32(Item(itemNum).Data3)
        Buffer.WriteInt32(Item(itemNum).TwoHanded)
        Buffer.WriteInt32(Item(itemNum).LevelReq)
        Buffer.WriteInt32(Item(itemNum).Mastery)
        Buffer.WriteString(Trim$(Item(itemNum).Name))
        Buffer.WriteInt32(Item(itemNum).Paperdoll)
        Buffer.WriteInt32(Item(itemNum).Pic)
        Buffer.WriteInt32(Item(itemNum).Price)
        Buffer.WriteInt32(Item(itemNum).Rarity)
        Buffer.WriteInt32(Item(itemNum).Speed)

        Buffer.WriteInt32(Item(itemNum).Randomize)
        Buffer.WriteInt32(Item(itemNum).RandomMin)
        Buffer.WriteInt32(Item(itemNum).RandomMax)

        Buffer.WriteInt32(Item(itemNum).Stackable)
        Buffer.WriteString(Trim$(Item(itemNum).Description))

        For i = 0 To StatType.Count - 1
            Buffer.WriteInt32(Item(itemNum).Stat_Req(i))
        Next

        Buffer.WriteInt32(Item(itemNum).Type)
        Buffer.WriteInt32(Item(itemNum).SubType)

        Buffer.WriteInt32(Item(itemNum).ItemLevel)

        'Housing
        Buffer.WriteInt32(Item(itemNum).FurnitureWidth)
        Buffer.WriteInt32(Item(itemNum).FurnitureHeight)

        For i = 1 To 3
            For x = 1 To 3
                Buffer.WriteInt32(Item(itemNum).FurnitureBlocks(i, x))
                Buffer.WriteInt32(Item(itemNum).FurnitureFringe(i, x))
            Next
        Next

        Buffer.WriteInt32(Item(itemNum).KnockBack)
        Buffer.WriteInt32(Item(itemNum).KnockBackTiles)

        Buffer.WriteInt32(Item(itemNum).Projectile)
        Buffer.WriteInt32(Item(itemNum).Ammo)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendRequestEditItem()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.RequestEditItem)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendRequestEditResource()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.RequestEditResource)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendRequestResources()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestResources)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendSaveResource(ResourceNum As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.SaveResource)

        Buffer.WriteInt32(ResourceNum)
        Buffer.WriteInt32(Resource(ResourceNum).Animation)
        Buffer.WriteString(Trim(Resource(ResourceNum).EmptyMessage))
        Buffer.WriteInt32(Resource(ResourceNum).ExhaustedImage)
        Buffer.WriteInt32(Resource(ResourceNum).Health)
        Buffer.WriteInt32(Resource(ResourceNum).ExpReward)
        Buffer.WriteInt32(Resource(ResourceNum).ItemReward)
        Buffer.WriteString(Trim(Resource(ResourceNum).Name))
        Buffer.WriteInt32(Resource(ResourceNum).ResourceImage)
        Buffer.WriteInt32(Resource(ResourceNum).ResourceType)
        Buffer.WriteInt32(Resource(ResourceNum).RespawnTime)
        Buffer.WriteString(Trim(Resource(ResourceNum).SuccessMessage))
        Buffer.WriteInt32(Resource(ResourceNum).LvlRequired)
        Buffer.WriteInt32(Resource(ResourceNum).ToolRequired)
        Buffer.WriteInt32(Resource(ResourceNum).Walkthrough)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendRequestEditNpc()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.RequestEditNpc)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendSaveNpc(NpcNum As Integer)
        dim buffer as New ByteStream(4), i As Integer

        Buffer.WriteInt32(EditorPackets.SaveNpc)
        Buffer.WriteInt32(NpcNum)

        Buffer.WriteInt32(Npc(NpcNum).Animation)
        Buffer.WriteString(Npc(NpcNum).AttackSay)
        Buffer.WriteInt32(Npc(NpcNum).Behaviour)
        For i = 1 To 5
            Buffer.WriteInt32(Npc(NpcNum).DropChance(i))
            Buffer.WriteInt32(Npc(NpcNum).DropItem(i))
            Buffer.WriteInt32(Npc(NpcNum).DropItemValue(i))
        Next

        Buffer.WriteInt32(Npc(NpcNum).Exp)
        Buffer.WriteInt32(Npc(NpcNum).Faction)
        Buffer.WriteInt32(Npc(NpcNum).Hp)
        Buffer.WriteString(Npc(NpcNum).Name)
        Buffer.WriteInt32(Npc(NpcNum).Range)
        Buffer.WriteInt32(Npc(NpcNum).SpawnTime)
        Buffer.WriteInt32(Npc(NpcNum).SpawnSecs)
        Buffer.WriteInt32(Npc(NpcNum).Sprite)

        For i = 0 To StatType.Count - 1
            Buffer.WriteInt32(Npc(NpcNum).Stat(i))
        Next

        Buffer.WriteInt32(Npc(NpcNum).QuestNum)

        For i = 1 To MAX_NPC_SKILLS
            Buffer.WriteInt32(Npc(NpcNum).Skill(i))
        Next

        Buffer.WriteInt32(Npc(NpcNum).Level)
        Buffer.WriteInt32(Npc(NpcNum).Damage)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendRequestNPCS()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestNPCS)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendRequestEditSkill()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.RequestEditSkill)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendRequestSkills()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestSkills)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendSaveSkill(skillnum As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.SaveSkill)
        Buffer.WriteInt32(skillnum)

        Buffer.WriteInt32(Skill(skillnum).AccessReq)
        Buffer.WriteInt32(Skill(skillnum).AoE)
        Buffer.WriteInt32(Skill(skillnum).CastAnim)
        Buffer.WriteInt32(Skill(skillnum).CastTime)
        Buffer.WriteInt32(Skill(skillnum).CdTime)
        Buffer.WriteInt32(Skill(skillnum).ClassReq)
        Buffer.WriteInt32(Skill(skillnum).Dir)
        Buffer.WriteInt32(Skill(skillnum).Duration)
        Buffer.WriteInt32(Skill(skillnum).Icon)
        Buffer.WriteInt32(Skill(skillnum).Interval)
        Buffer.WriteInt32(Skill(skillnum).IsAoE)
        Buffer.WriteInt32(Skill(skillnum).LevelReq)
        Buffer.WriteInt32(Skill(skillnum).Map)
        Buffer.WriteInt32(Skill(skillnum).MpCost)
        Buffer.WriteString(Skill(skillnum).Name)
        Buffer.WriteInt32(Skill(skillnum).Range)
        Buffer.WriteInt32(Skill(skillnum).SkillAnim)
        Buffer.WriteInt32(Skill(skillnum).StunDuration)
        Buffer.WriteInt32(Skill(skillnum).Type)
        Buffer.WriteInt32(Skill(skillnum).Vital)
        Buffer.WriteInt32(Skill(skillnum).X)
        Buffer.WriteInt32(Skill(skillnum).Y)

        Buffer.WriteInt32(Skill(skillnum).IsProjectile)
        Buffer.WriteInt32(Skill(skillnum).Projectile)

        Buffer.WriteInt32(Skill(skillnum).KnockBack)
        Buffer.WriteInt32(Skill(skillnum).KnockBackTiles)

        Socket.SendData(Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendRequestShops()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestShops)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendSaveShop(shopnum As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.SaveShop)
        Buffer.WriteInt32(shopnum)

        Buffer.WriteInt32(Shop(shopnum).BuyRate)
        Buffer.WriteString(Shop(shopnum).Name)
        Buffer.WriteInt32(Shop(shopnum).Face)

        For i = 0 To MAX_TRADES
            Buffer.WriteInt32(Shop(shopnum).TradeItem(i).CostItem)
            Buffer.WriteInt32(Shop(shopnum).TradeItem(i).CostValue)
            Buffer.WriteInt32(Shop(shopnum).TradeItem(i).Item)
            Buffer.WriteInt32(Shop(shopnum).TradeItem(i).ItemValue)
        Next

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Friend Sub SendRequestEditShop()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.RequestEditShop)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendSaveAnimation(Animationnum As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.SaveAnimation)
        Buffer.WriteInt32(Animationnum)

        For i = 0 To UBound(Animation(Animationnum).Frames)
            Buffer.WriteInt32(Animation(Animationnum).Frames(i))
        Next

        For i = 0 To UBound(Animation(Animationnum).LoopCount)
            Buffer.WriteInt32(Animation(Animationnum).LoopCount(i))
        Next

        For i = 0 To UBound(Animation(Animationnum).LoopTime)
            Buffer.WriteInt32(Animation(Animationnum).LoopTime(i))
        Next

        Buffer.WriteString(Trim$(Animation(Animationnum).Name))
        Buffer.WriteString(Trim$(Animation(Animationnum).Sound))

        For i = 0 To UBound(Animation(Animationnum).Sprite)
            Buffer.WriteInt32(Animation(Animationnum).Sprite(i))
        Next

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendRequestAnimations()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestAnimations)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendRequestEditAnimation()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.RequestEditAnimation)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendRequestMapreport()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CMapReport)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendRequestClasses()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestClasses)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendRequestEditClass()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.RequestEditClasses)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendSaveClasses()
        Dim i As Integer, n As Integer, q As Integer
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.SaveClasses)

        Buffer.WriteInt32(Max_Classes)

        For i = 1 To Max_Classes
            Buffer.WriteString(Trim$(Classes(i).Name))
            Buffer.WriteString(Trim$(Classes(i).Desc))

            ' set sprite array size
            n = UBound(Classes(i).MaleSprite)

            ' send array size
            Buffer.WriteInt32(n)

            ' loop around sending each sprite
            For q = 0 To n
                Buffer.WriteInt32(Classes(i).MaleSprite(q))
            Next

            ' set sprite array size
            n = UBound(Classes(i).FemaleSprite)

            ' send array size
            Buffer.WriteInt32(n)

            ' loop around sending each sprite
            For q = 0 To n
                Buffer.WriteInt32(Classes(i).FemaleSprite(q))
            Next

            Buffer.WriteInt32(Classes(i).Stat(StatType.Strength))
            Buffer.WriteInt32(Classes(i).Stat(StatType.Endurance))
            Buffer.WriteInt32(Classes(i).Stat(StatType.Vitality))
            Buffer.WriteInt32(Classes(i).Stat(StatType.Intelligence))
            Buffer.WriteInt32(Classes(i).Stat(StatType.Luck))
            Buffer.WriteInt32(Classes(i).Stat(StatType.Spirit))

            For q = 1 To 5
                Buffer.WriteInt32(Classes(i).StartItem(q))
                Buffer.WriteInt32(Classes(i).StartValue(q))
            Next

            Buffer.WriteInt32(Classes(i).StartMap)
            Buffer.WriteInt32(Classes(i).StartX)
            Buffer.WriteInt32(Classes(i).StartY)

            Buffer.WriteInt32(Classes(i).BaseExp)
        Next

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendLeaveGame()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CQuit)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendEditorRequestMap(mapNum as Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.EditorRequestMap)
        Buffer.WriteInt32(MapNum)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendRequestAutoMapper()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.RequestAutoMap)
        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendSaveAutoMapper()
        Dim myXml As New XmlClass With {
            .Filename = Application.StartupPath & "\Data\AutoMapper.xml",
            .Root = "Options"
        }
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.SaveAutoMap)

        Buffer.WriteInt32(MapStart)
        Buffer.WriteInt32(MapSize)
        Buffer.WriteInt32(MapX)
        Buffer.WriteInt32(MapY)
        Buffer.WriteInt32(SandBorder)
        Buffer.WriteInt32(DetailFreq)
        Buffer.WriteInt32(ResourceFreq)

        'send ini info
        Buffer.WriteString(myXml.ReadString("Resources", "ResourcesNum"))

        For Prefab = 1 To TilePrefab.Count - 1
            For Layer = 1 To LayerType.Count - 1
                If Val(myXml.ReadString("Prefab" & Prefab, "Layer" & Layer & "Tileset")) > 0 Then
                    Buffer.WriteInt32(Layer)
                    Buffer.WriteInt32(Val(myXml.ReadString("Prefab" & Prefab, "Layer" & Layer & "Tileset")))
                    Buffer.WriteInt32(Val(myXml.ReadString("Prefab" & Prefab, "Layer" & Layer & "X")))
                    Buffer.WriteInt32(Val(myXml.ReadString("Prefab" & Prefab, "Layer" & Layer & "Y")))
                    Buffer.WriteInt32(Val(myXml.ReadString("Prefab" & Prefab, "Layer" & Layer & "Autotile")))
                End If
            Next
            Buffer.WriteInt32(Val(myXml.ReadString("Prefab" & Prefab, "Type")))
        Next

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub
End Module
