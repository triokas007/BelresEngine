Imports ASFW
Imports ASFW.IO

Module ServerNetworkSend
    Sub AlertMsg(index as integer, Msg As String)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SAlertMsg)
        Buffer.WriteString(Msg)
        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        AddDebug("Sent SMSG: SAlertMsg")

        buffer.Dispose()
    End Sub

    Sub GlobalMsg(Msg As String)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SGlobalMsg)
        'Buffer.WriteString(Msg)
        Buffer.WriteBytes(WriteUnicodeString(Msg))
        SendDataToAll(Buffer.Data, Buffer.Head)

        AddDebug("Sent SMSG: SGlobalMsg")

        buffer.Dispose()
    End Sub

    Sub PlayerMsg(index as integer, Msg As String, Colour As Integer)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SPlayerMsg)
        'Buffer.WriteString(Msg)
        Buffer.WriteBytes(WriteUnicodeString(Msg))
        Buffer.WriteInt32(Colour)

        AddDebug("Sent SMSG: SPlayerMsg")

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendAnimations(index as integer)
        Dim i As Integer

        For i = 1 To MAX_ANIMATIONS

            If Len(Trim$(Animation(i).Name)) > 0 Then
                SendUpdateAnimationTo(Index, i)
            End If

        Next

    End Sub

    Sub SendNewCharClasses(index as integer)
        Dim i As Integer, n As Integer, q As Integer
        dim buffer as ByteStream
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SNewCharClasses)
        Buffer.WriteInt32(Max_Classes)

        AddDebug("Sent SMSG: SNewCharClasses")

        For i = 1 To Max_Classes
            Buffer.WriteString(GetClassName(i))
            Buffer.WriteString(Trim$(Classes(i).Desc))

            Buffer.WriteInt32(GetClassMaxVital(i, VitalType.HP))
            Buffer.WriteInt32(GetClassMaxVital(i, VitalType.MP))
            Buffer.WriteInt32(GetClassMaxVital(i, VitalType.SP))

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
            Buffer.WriteInt32(Classes(i).Stat(StatType.Luck))
            Buffer.WriteInt32(Classes(i).Stat(StatType.Intelligence))
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

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendCloseTrade(index as integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SCloseTrade)
        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        AddDebug("Sent SMSG: SCloseTrade")

        buffer.Dispose()
    End Sub

    Sub SendExp(index as integer)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SPlayerEXP)
        Buffer.WriteInt32(Index)
        Buffer.WriteInt32(GetPlayerExp(Index))
        Buffer.WriteInt32(GetPlayerNextLevel(Index))

        AddDebug("Sent SMSG: SPlayerEXP")

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendLoadCharOk(index as integer)
        Dim Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SLoadCharOk)
        Buffer.WriteInt32(index)
        Socket.SendDataTo(index, Buffer.Data, Buffer.Head)

        AddDebug("Sent SMSG: SLoadCharOk")

        Buffer.Dispose()
    End Sub

    Sub SendEditorLoadOk(index as integer)
        Dim Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SLoginOk)
        Buffer.WriteInt32(index)
        Socket.SendDataTo(index, Buffer.Data, Buffer.Head)

        AddDebug("Sent SMSG: SLoginOk")

        Buffer.Dispose()
    End Sub

    Sub SendInGame(index as integer)
        Dim Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SInGame)
        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        AddDebug("Sent SMSG: SInGame")

        Buffer.Dispose()
    End Sub

    Sub SendClasses(index as integer)
        Dim i As Integer, n As Integer, q As Integer
        dim buffer as ByteStream
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SClassesData)
        Buffer.WriteInt32(Max_Classes)

        AddDebug("Sent SMSG: SClassesData")

        For i = 1 To Max_Classes
            Buffer.WriteString(Trim$(GetClassName(i)))
            Buffer.WriteString(Trim$(Classes(i).Desc))

            Buffer.WriteInt32(GetClassMaxVital(i, VitalType.HP))
            Buffer.WriteInt32(GetClassMaxVital(i, VitalType.MP))
            Buffer.WriteInt32(GetClassMaxVital(i, VitalType.SP))

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

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendClassesToAll()
        Dim i As Integer, n As Integer, q As Integer
        dim buffer as ByteStream
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SClassesData)
        Buffer.WriteInt32(Max_Classes)

        AddDebug("Sent SMSG: SClassesData To All")

        For i = 1 To Max_Classes
            Buffer.WriteString(GetClassName(i))
            Buffer.WriteString(Trim$(Classes(i).Desc))

            Buffer.WriteInt32(GetClassMaxVital(i, VitalType.HP))
            Buffer.WriteInt32(GetClassMaxVital(i, VitalType.MP))
            Buffer.WriteInt32(GetClassMaxVital(i, VitalType.SP))

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

        SendDataToAll(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendInventory(index as integer)
        Dim i As Integer, n As Integer
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SPlayerInv)

        AddDebug("Sent SMSG: SPlayerInv")

        For i = 1 To MAX_INV
            Buffer.WriteInt32(GetPlayerInvItemNum(Index, i))
            Buffer.WriteInt32(GetPlayerInvItemValue(Index, i))
            Buffer.WriteString(Player(Index).Character(TempPlayer(Index).CurChar).RandInv(i).Prefix)
            Buffer.WriteString(Player(Index).Character(TempPlayer(Index).CurChar).RandInv(i).Suffix)
            Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).RandInv(i).Rarity)
            For n = 1 To StatType.Count - 1
                Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).RandInv(i).Stat(n))
            Next
            Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).RandInv(i).Damage)
            Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).RandInv(i).Speed)
        Next

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendItems(index as integer)
        Dim i As Integer

        For i = 1 To MAX_ITEMS

            If Len(Trim$(Item(i).Name)) > 0 Then
                SendUpdateItemTo(Index, i)
            End If

        Next

    End Sub

    Sub SendUpdateItemTo(index as integer, itemNum As Integer)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SUpdateItem)
        Buffer.WriteInt32(itemNum)
        Buffer.WriteInt32(Item(itemNum).AccessReq)

        AddDebug("Sent SMSG: SUpdateItem")

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

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendUpdateItemToAll(itemNum As Integer)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SUpdateItem)
        Buffer.WriteInt32(itemNum)
        Buffer.WriteInt32(Item(itemNum).AccessReq)

        AddDebug("Sent SMSG: SUpdateItem To All")

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

        SendDataToAll(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendLeftMap(index as integer)
        dim buffer as New ByteStream(4)
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SLeftMap)
        Buffer.WriteInt32(Index)
        SendDataToAllBut(Index, Buffer.Data, Buffer.Head)

        AddDebug("Sent SMSG: SLeftMap")

        buffer.Dispose()
    End Sub

    Sub SendLeftGame(index as integer)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SLeftGame)

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendMapEquipment(index as integer)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SMapWornEq)
        Buffer.WriteInt32(Index)
        Buffer.WriteInt32(GetPlayerEquipment(Index, EquipmentType.Armor))
        Buffer.WriteInt32(GetPlayerEquipment(Index, EquipmentType.Weapon))
        Buffer.WriteInt32(GetPlayerEquipment(Index, EquipmentType.Helmet))
        Buffer.WriteInt32(GetPlayerEquipment(Index, EquipmentType.Shield))
        Buffer.WriteInt32(GetPlayerEquipment(Index, EquipmentType.Shoes))
        Buffer.WriteInt32(GetPlayerEquipment(Index, EquipmentType.Gloves))

        AddDebug("Sent SMSG: SMapWornEq")

        SendDataToMap(GetPlayerMap(Index), Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendMapEquipmentTo(PlayerNum As Integer, index as integer)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SMapWornEq)
        Buffer.WriteInt32(PlayerNum)
        Buffer.WriteInt32(GetPlayerEquipment(PlayerNum, EquipmentType.Armor))
        Buffer.WriteInt32(GetPlayerEquipment(PlayerNum, EquipmentType.Weapon))
        Buffer.WriteInt32(GetPlayerEquipment(PlayerNum, EquipmentType.Helmet))
        Buffer.WriteInt32(GetPlayerEquipment(PlayerNum, EquipmentType.Shield))
        Buffer.WriteInt32(GetPlayerEquipment(Index, EquipmentType.Shoes))
        Buffer.WriteInt32(GetPlayerEquipment(Index, EquipmentType.Gloves))

        AddDebug("Sent SMSG: SMapWornEq To")

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendNpcs(index as integer)
        Dim i As Integer

        For i = 1 To MAX_NPCS

            If Len(Trim$(Npc(i).Name)) > 0 Then
                SendUpdateNpcTo(Index, i)
            End If

        Next

    End Sub

    Sub SendUpdateNpcTo(index as integer, NpcNum As Integer)
        dim buffer as ByteStream, i As Integer
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SUpdateNpc)

        AddDebug("Sent SMSG: SUpdateNpc")

        buffer.WriteInt32(NpcNum)
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

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendUpdateNpcToAll(NpcNum As Integer)
        dim buffer as ByteStream, i As Integer
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SUpdateNpc)

        AddDebug("Sent SMSG: SUpdateNpc To All")

        buffer.WriteInt32(NpcNum)
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

        SendDataToAll(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendResourceCacheTo(index as integer, Resource_num As Integer)
        dim buffer as ByteStream
        Dim i As Integer
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SResourceCache)
        Buffer.WriteInt32(ResourceCache(GetPlayerMap(Index)).ResourceCount)

        AddDebug("Sent SMSG: SResourcesCahce")

        If ResourceCache(GetPlayerMap(Index)).ResourceCount > 0 Then

            For i = 0 To ResourceCache(GetPlayerMap(Index)).ResourceCount
                Buffer.WriteInt32(ResourceCache(GetPlayerMap(Index)).ResourceData(i).ResourceState)
                Buffer.WriteInt32(ResourceCache(GetPlayerMap(Index)).ResourceData(i).X)
                Buffer.WriteInt32(ResourceCache(GetPlayerMap(Index)).ResourceData(i).Y)
            Next

        End If

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendResources(index as integer)
        Dim i As Integer

        For i = 1 To MAX_RESOURCES

            If Len(Trim$(Resource(i).Name)) > 0 Then
                SendUpdateResourceTo(Index, i)
            End If

        Next

    End Sub

    Sub SendUpdateResourceTo(index as integer, ResourceNum As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SUpdateResource)
        Buffer.WriteInt32(ResourceNum)
        Buffer.WriteInt32(Resource(ResourceNum).Animation)
        Buffer.WriteString(Resource(ResourceNum).EmptyMessage)
        Buffer.WriteInt32(Resource(ResourceNum).ExhaustedImage)
        Buffer.WriteInt32(Resource(ResourceNum).Health)
        Buffer.WriteInt32(Resource(ResourceNum).ExpReward)
        Buffer.WriteInt32(Resource(ResourceNum).ItemReward)
        Buffer.WriteString(Resource(ResourceNum).Name)
        Buffer.WriteInt32(Resource(ResourceNum).ResourceImage)
        Buffer.WriteInt32(Resource(ResourceNum).ResourceType)
        Buffer.WriteInt32(Resource(ResourceNum).RespawnTime)
        Buffer.WriteString(Resource(ResourceNum).SuccessMessage)
        Buffer.WriteInt32(Resource(ResourceNum).LvlRequired)
        Buffer.WriteInt32(Resource(ResourceNum).ToolRequired)
        Buffer.WriteInt32(Resource(ResourceNum).Walkthrough)

        AddDebug("Sent SMSG: SUpdateResources")

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendShops(index as integer)
        Dim i As Integer

        For i = 1 To MAX_SHOPS

            If Len(Trim$(Shop(i).Name)) > 0 Then
                SendUpdateShopTo(Index, i)
            End If

        Next

    End Sub

    Sub SendUpdateShopTo(index as integer, shopNum As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SUpdateShop)
        Buffer.WriteInt32(shopNum)
        Buffer.WriteInt32(Shop(shopNum).BuyRate)
        Buffer.WriteString(Trim(Shop(shopNum).Name))
        Buffer.WriteInt32(Shop(shopNum).Face)

        AddDebug("Sent SMSG: SUpdateShop")

        For i = 0 To MAX_TRADES
            Buffer.WriteInt32(Shop(shopNum).TradeItem(i).CostItem)
            Buffer.WriteInt32(Shop(shopNum).TradeItem(i).CostValue)
            Buffer.WriteInt32(Shop(shopNum).TradeItem(i).Item)
            Buffer.WriteInt32(Shop(shopNum).TradeItem(i).ItemValue)
        Next

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendUpdateShopToAll(shopNum As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SUpdateShop)
        Buffer.WriteInt32(shopNum)
        Buffer.WriteInt32(Shop(shopNum).BuyRate)
        Buffer.WriteString(Shop(shopNum).Name)
        Buffer.WriteInt32(Shop(shopNum).Face)

        AddDebug("Sent SMSG: SUpdateShop To All")

        For i = 0 To MAX_TRADES
            Buffer.WriteInt32(Shop(shopNum).TradeItem(i).CostItem)
            Buffer.WriteInt32(Shop(shopNum).TradeItem(i).CostValue)
            Buffer.WriteInt32(Shop(shopNum).TradeItem(i).Item)
            Buffer.WriteInt32(Shop(shopNum).TradeItem(i).ItemValue)
        Next

        SendDataToAll(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendSkills(index as integer)
        Dim i As Integer

        For i = 1 To MAX_SKILLS

            If Len(Trim$(Skill(i).Name)) > 0 Then
                SendUpdateSkillTo(Index, i)
            End If

        Next

    End Sub

    Sub SendUpdateSkillTo(index as integer, skillnum As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SUpdateSkill)
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
        Buffer.WriteString(Trim(Skill(skillnum).Name))
        Buffer.WriteInt32(Skill(skillnum).Range)
        Buffer.WriteInt32(Skill(skillnum).SkillAnim)
        Buffer.WriteInt32(Skill(skillnum).StunDuration)
        Buffer.WriteInt32(Skill(skillnum).Type)
        Buffer.WriteInt32(Skill(skillnum).Vital)
        Buffer.WriteInt32(Skill(skillnum).X)
        Buffer.WriteInt32(Skill(skillnum).Y)

        AddDebug("Sent SMSG: SUpdateSkill")

        'projectiles
        buffer.WriteInt32(Skill(skillnum).IsProjectile)
        Buffer.WriteInt32(Skill(skillnum).Projectile)

        Buffer.WriteInt32(Skill(skillnum).KnockBack)
        Buffer.WriteInt32(Skill(skillnum).KnockBackTiles)

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendUpdateSkillToAll(skillnum As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SUpdateSkill)
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

        AddDebug("Sent SMSG: SUpdateSkill To All")

        'projectiles
        buffer.WriteInt32(Skill(skillnum).IsProjectile)
        Buffer.WriteInt32(Skill(skillnum).Projectile)

        Buffer.WriteInt32(Skill(skillnum).KnockBack)
        Buffer.WriteInt32(Skill(skillnum).KnockBackTiles)

        SendDataToAll(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendStats(index as integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SPlayerStats)
        Buffer.WriteInt32(Index)
        Buffer.WriteInt32(GetPlayerStat(Index, StatType.Strength))
        Buffer.WriteInt32(GetPlayerStat(Index, StatType.Endurance))
        Buffer.WriteInt32(GetPlayerStat(Index, StatType.Vitality))
        Buffer.WriteInt32(GetPlayerStat(Index, StatType.Luck))
        Buffer.WriteInt32(GetPlayerStat(Index, StatType.Intelligence))
        Buffer.WriteInt32(GetPlayerStat(Index, StatType.Spirit))
        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        AddDebug("Sent SMSG: SPlayerStats")

        buffer.Dispose()
    End Sub

    Sub SendUpdateAnimationTo(index as integer, AnimationNum As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SUpdateAnimation)
        Buffer.WriteInt32(AnimationNum)

        AddDebug("Sent SMSG: SUpdateAnimation")

        For i = 0 To UBound(Animation(AnimationNum).Frames)
            Buffer.WriteInt32(Animation(AnimationNum).Frames(i))
        Next

        For i = 0 To UBound(Animation(AnimationNum).LoopCount)
            Buffer.WriteInt32(Animation(AnimationNum).LoopCount(i))
        Next

        For i = 0 To UBound(Animation(AnimationNum).LoopTime)
            Buffer.WriteInt32(Animation(AnimationNum).LoopTime(i))
        Next

        Buffer.WriteString(Animation(AnimationNum).Name)
        Buffer.WriteString(Animation(AnimationNum).Sound)

        For i = 0 To UBound(Animation(AnimationNum).Sprite)
            Buffer.WriteInt32(Animation(AnimationNum).Sprite(i))
        Next

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendUpdateAnimationToAll(AnimationNum As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SUpdateAnimation)
        Buffer.WriteInt32(AnimationNum)

        AddDebug("Sent SMSG: SUpdateAnimation To All")

        For i = 0 To UBound(Animation(AnimationNum).Frames)
            Buffer.WriteInt32(Animation(AnimationNum).Frames(i))
        Next

        For i = 0 To UBound(Animation(AnimationNum).LoopCount)
            Buffer.WriteInt32(Animation(AnimationNum).LoopCount(i))
        Next

        For i = 0 To UBound(Animation(AnimationNum).LoopTime)
            Buffer.WriteInt32(Animation(AnimationNum).LoopTime(i))
        Next

        Buffer.WriteString(Animation(AnimationNum).Name)
        Buffer.WriteString(Animation(AnimationNum).Sound)

        For i = 0 To UBound(Animation(AnimationNum).Sprite)
            Buffer.WriteInt32(Animation(AnimationNum).Sprite(i))
        Next

        SendDataToAll(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendVitals(index as integer)
        For i = 1 To VitalType.Count - 1
            SendVital(Index, i)
        Next
    End Sub

    Sub SendVital(index as integer, Vital As VitalType)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        ' Get our packet type.
        Select Case Vital
            Case VitalType.HP
                Buffer.WriteInt32(ServerPackets.SPlayerHp)
                AddDebug("Sent SMSG: SPlayerHp")
            Case VitalType.MP
                Buffer.WriteInt32(ServerPackets.SPlayerMp)
                AddDebug("Sent SMSG: SPlayerMp")
            Case VitalType.SP
                Buffer.WriteInt32(ServerPackets.SPlayerSp)
                AddDebug("Sent SMSG: SPlayerSp")
        End Select

        ' Set and send related data.
        Buffer.WriteInt32(GetPlayerMaxVital(Index, Vital))
        Buffer.WriteInt32(GetPlayerVital(Index, Vital))
        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendWelcome(index as integer)

        ' Send them MOTD
        If Len(Options.Motd) > 0 Then
            PlayerMsg(Index, Options.Motd, ColorType.BrightCyan)
        End If

        ' Send whos online
        SendWhosOnline(Index)
    End Sub

    Sub SendWhosOnline(index as integer)
        Dim s As String
        Dim n As Integer
        Dim i As Integer
        s = ""
        For i = 1 To GetPlayersOnline()

            If IsPlaying(i) Then
                If i <> Index Then
                    s = s & GetPlayerName(i) & ", "
                    n = n + 1
                End If
            End If

        Next

        If n = 0 Then
            s = "There are no other players online."
        Else
            s = Mid$(s, 1, Len(s) - 2)
            s = "There are " & n & " other players online: " & s & "."
        End If

        PlayerMsg(Index, s, ColorType.White)
    End Sub

    Sub SendWornEquipment(index as integer)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SPlayerWornEq)

        AddDebug("Sent SMSG: SPlayerWornEq")

        For i = 1 To EquipmentType.Count - 1
            Buffer.WriteInt32(GetPlayerEquipment(Index, i))
        Next

        For i = 1 To EquipmentType.Count - 1
            Buffer.WriteString(Player(Index).Character(TempPlayer(Index).CurChar).RandEquip(i).Prefix)
            Buffer.WriteString(Player(Index).Character(TempPlayer(Index).CurChar).RandEquip(i).Suffix)
            Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).RandEquip(i).Damage)
            Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).RandEquip(i).Speed)
            Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).RandEquip(i).Rarity)
            For n = 1 To StatType.Count - 1
                Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).RandEquip(i).Stat(n))
            Next
        Next

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendMapData(index as integer, mapNum as Integer, SendMap As Boolean)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)
        Dim data() As Byte

        If SendMap Then
            Buffer.WriteInt32(1)
            Buffer.WriteInt32(MapNum)
            Buffer.WriteString(Map(MapNum).Name)
            Buffer.WriteString(Map(MapNum).Music)
            Buffer.WriteInt32(Map(MapNum).Revision)
            Buffer.WriteInt32(Map(MapNum).Moral)
            Buffer.WriteInt32(Map(MapNum).Tileset)
            Buffer.WriteInt32(Map(MapNum).Up)
            Buffer.WriteInt32(Map(MapNum).Down)
            Buffer.WriteInt32(Map(MapNum).Left)
            Buffer.WriteInt32(Map(MapNum).Right)
            Buffer.WriteInt32(Map(MapNum).BootMap)
            Buffer.WriteInt32(Map(MapNum).BootX)
            Buffer.WriteInt32(Map(MapNum).BootY)
            Buffer.WriteInt32(Map(MapNum).MaxX)
            Buffer.WriteInt32(Map(MapNum).MaxY)
            Buffer.WriteInt32(Map(MapNum).WeatherType)
            Buffer.WriteInt32(Map(MapNum).FogIndex)
            Buffer.WriteInt32(Map(MapNum).WeatherIntensity)
            Buffer.WriteInt32(Map(MapNum).FogAlpha)
            Buffer.WriteInt32(Map(MapNum).FogSpeed)
            Buffer.WriteInt32(Map(MapNum).HasMapTint)
            Buffer.WriteInt32(Map(MapNum).MapTintR)
            Buffer.WriteInt32(Map(MapNum).MapTintG)
            Buffer.WriteInt32(Map(MapNum).MapTintB)
            Buffer.WriteInt32(Map(MapNum).MapTintA)
            Buffer.WriteInt32(Map(MapNum).Instanced)
            Buffer.WriteInt32(Map(MapNum).Panorama)
            Buffer.WriteInt32(Map(MapNum).Parallax)

            For i = 1 To MAX_MAP_NPCS
                Buffer.WriteInt32(Map(MapNum).Npc(i))
            Next

            For x = 0 To Map(MapNum).MaxX
                For y = 0 To Map(MapNum).MaxY
                    Buffer.WriteInt32(Map(MapNum).Tile(x, y).Data1)
                    Buffer.WriteInt32(Map(MapNum).Tile(x, y).Data2)
                    Buffer.WriteInt32(Map(MapNum).Tile(x, y).Data3)
                    Buffer.WriteInt32(Map(MapNum).Tile(x, y).DirBlock)
                    For i = 0 To LayerType.Count - 1
                        Buffer.WriteInt32(Map(MapNum).Tile(x, y).Layer(i).Tileset)
                        Buffer.WriteInt32(Map(MapNum).Tile(x, y).Layer(i).X)
                        Buffer.WriteInt32(Map(MapNum).Tile(x, y).Layer(i).Y)
                        Buffer.WriteInt32(Map(MapNum).Tile(x, y).Layer(i).AutoTile)
                    Next
                    Buffer.WriteInt32(Map(MapNum).Tile(x, y).Type)

                Next
            Next

            'Event Data
            Buffer.WriteInt32(Map(MapNum).EventCount)
            If Map(MapNum).EventCount > 0 Then
                For i = 1 To Map(MapNum).EventCount
                    With Map(MapNum).Events(i)
                        Buffer.WriteString(Trim$(.Name))
                        Buffer.WriteInt32(.Globals)
                        Buffer.WriteInt32(.X)
                        Buffer.WriteInt32(.Y)
                        Buffer.WriteInt32(.PageCount)
                    End With
                    If Map(MapNum).Events(i).PageCount > 0 Then
                        For X = 1 To Map(MapNum).Events(i).PageCount
                            With Map(MapNum).Events(i).Pages(X)
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
                                Buffer.WriteInt32(.MoveRouteCount)
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
                                Buffer.WriteInt32(.QuestNum)

                                Buffer.WriteInt32(.chkPlayerGender)
                            End With
                            If Map(MapNum).Events(i).Pages(X).CommandListCount > 0 Then
                                For Y = 1 To Map(MapNum).Events(i).Pages(X).CommandListCount
                                    Buffer.WriteInt32(Map(MapNum).Events(i).Pages(X).CommandList(Y).CommandCount)
                                    Buffer.WriteInt32(Map(MapNum).Events(i).Pages(X).CommandList(Y).ParentList)
                                    If Map(MapNum).Events(i).Pages(X).CommandList(Y).CommandCount > 0 Then
                                        For z = 1 To Map(MapNum).Events(i).Pages(X).CommandList(Y).CommandCount
                                            With Map(MapNum).Events(i).Pages(X).CommandList(Y).Commands(z)
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
        Else
            Buffer.WriteInt32(0)
        End If

        For i = 1 To MAX_MAP_ITEMS
            Buffer.WriteInt32(MapItem(MapNum, i).Num)
            Buffer.WriteInt32(MapItem(MapNum, i).Value)
            Buffer.WriteInt32(MapItem(MapNum, i).X)
            Buffer.WriteInt32(MapItem(MapNum, i).Y)
        Next

        For i = 1 To MAX_MAP_NPCS
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).Num)
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).X)
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).Y)
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).Dir)
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).Vital(VitalType.HP))
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).Vital(VitalType.MP))
        Next

        'send Resource cache
        If ResourceCache(GetPlayerMap(Index)).ResourceCount > 0 Then
            Buffer.WriteInt32(1)
            Buffer.WriteInt32(ResourceCache(GetPlayerMap(Index)).ResourceCount)

            For i = 0 To ResourceCache(GetPlayerMap(Index)).ResourceCount
                Buffer.WriteInt32(ResourceCache(GetPlayerMap(Index)).ResourceData(i).ResourceState)
                Buffer.WriteInt32(ResourceCache(GetPlayerMap(Index)).ResourceData(i).X)
                Buffer.WriteInt32(ResourceCache(GetPlayerMap(Index)).ResourceData(i).Y)
            Next
        Else
            Buffer.WriteInt32(0)
        End If

        data = Compression.CompressBytes(Buffer.ToArray)
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SMapData)
        Buffer.WriteBlock(data)
        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        AddDebug("Sent SMSG: SMapData")

        buffer.Dispose()
    End Sub

    Sub SendJoinMap(index as integer)
        Dim i As Integer
        Dim data As Byte()
        ' Send all players on current map to index
        For i = 1 To GetPlayersOnline()
            If IsPlaying(i) Then
                If i <> Index Then
                    If GetPlayerMap(i) = GetPlayerMap(Index) Then
                        data = PlayerData(i)
                        Socket.SendDataTo(Index, data, data.Length)
                    End If
                End If
            End If
        Next

        ' Send index's player data to everyone on the map including himself
        data = PlayerData(Index)
        SendDataToMap(GetPlayerMap(Index), data, data.Length)
    End Sub

    Function PlayerData(index as integer) As Byte()
        dim buffer as ByteStream, i As Integer
        PlayerData = Nothing
        If Index > MAX_PLAYERS Then Exit Function
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SPlayerData)
        Buffer.WriteInt32(Index)
        Buffer.WriteString(GetPlayerName(Index))
        Buffer.WriteInt32(GetPlayerClass(Index))
        Buffer.WriteInt32(GetPlayerLevel(Index))
        Buffer.WriteInt32(GetPlayerPOINTS(Index))
        Buffer.WriteInt32(GetPlayerSprite(Index))
        Buffer.WriteInt32(GetPlayerMap(Index))
        Buffer.WriteInt32(GetPlayerX(Index))
        Buffer.WriteInt32(GetPlayerY(Index))
        Buffer.WriteInt32(GetPlayerDir(Index))
        Buffer.WriteInt32(GetPlayerAccess(Index))
        Buffer.WriteInt32(GetPlayerPK(Index))

        AddDebug("Sent SMSG: SPlayerData")

        For i = 1 To StatType.Count - 1
            Buffer.WriteInt32(GetPlayerStat(Index, i))
        Next

        Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).InHouse)

        For i = 0 To ResourceSkills.Count - 1
            Buffer.WriteInt32(GetPlayerGatherSkillLvl(Index, i))
            Buffer.WriteInt32(GetPlayerGatherSkillExp(Index, i))
            Buffer.WriteInt32(GetPlayerGatherSkillMaxExp(Index, i))
        Next

        For i = 1 To MAX_RECIPE
            Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).RecipeLearned(i))
        Next

        PlayerData = Buffer.ToArray()

        Buffer.Dispose()
    End Function

    Sub SendMapItemsTo(index as integer, mapNum as Integer)
        Dim i As Integer
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SMapItemData)

        AddDebug("Sent SMSG: SMapItemData")

        For i = 1 To MAX_MAP_ITEMS
            Buffer.WriteInt32(MapItem(MapNum, i).Num)
            Buffer.WriteInt32(MapItem(MapNum, i).Value)
            Buffer.WriteInt32(MapItem(MapNum, i).X)
            Buffer.WriteInt32(MapItem(MapNum, i).Y)
        Next

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendMapNpcsTo(index as integer, mapNum as Integer)
        Dim i As Integer
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SMapNpcData)

        AddDebug("Sent SMSG: SMapNpcData")

        For i = 1 To MAX_MAP_NPCS
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).Num)
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).X)
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).Y)
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).Dir)
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).Vital(VitalType.HP))
            Buffer.WriteInt32(MapNpc(MapNum).Npc(i).Vital(VitalType.MP))
        Next

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendMapNpcTo(mapNum as Integer, MapNpcNum As Integer)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SMapNpcUpdate)

        AddDebug("Sent SMSG: SMapNpcUpdate")

        buffer.WriteInt32(MapNpcNum)

        With MapNpc(MapNum).Npc(MapNpcNum)
            Buffer.WriteInt32(.Num)
            Buffer.WriteInt32(.X)
            Buffer.WriteInt32(.Y)
            Buffer.WriteInt32(.Dir)
            Buffer.WriteInt32(.Vital(VitalType.HP))
            Buffer.WriteInt32(.Vital(VitalType.MP))
        End With

        SendDataToMap(MapNum, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendPlayerXY(index as integer)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SPlayerXY)
        Buffer.WriteInt32(GetPlayerX(Index))
        Buffer.WriteInt32(GetPlayerY(Index))
        Buffer.WriteInt32(GetPlayerDir(Index))
        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        AddDebug("Sent SMSG: SPlayerXY")

        buffer.Dispose()
    End Sub

    Sub SendPlayerMove(index as integer, Movement As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SPlayerMove)
        Buffer.WriteInt32(Index)
        Buffer.WriteInt32(GetPlayerX(Index))
        Buffer.WriteInt32(GetPlayerY(Index))
        Buffer.WriteInt32(GetPlayerDir(Index))
        Buffer.WriteInt32(Movement)
        SendDataToMapBut(Index, GetPlayerMap(Index), Buffer.Data, Buffer.Head)

        AddDebug("Sent SMSG: SPlayerMove")

        buffer.Dispose()
    End Sub

    Sub SendDoorAnimation(mapNum as Integer, X As Integer, Y As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SDoorAnimation)
        Buffer.WriteInt32(X)
        Buffer.WriteInt32(Y)

        AddDebug("Sent SMSG: SDoorAnimation")

        SendDataToMap(MapNum, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendMapKey(index as integer, X As Integer, Y As Integer, Value As Byte)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SMapKey)
        Buffer.WriteInt32(X)
        Buffer.WriteInt32(Y)
        Buffer.WriteInt32(Value)

        AddDebug("Sent SMSG: SMapKey")

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub MapMsg(mapNum as Integer, Msg As String, Color As Byte)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SMapMsg)
        'Buffer.WriteString(Msg)
        Buffer.WriteBytes(WriteUnicodeString(Msg))

        AddDebug("Sent SMSG: SMapMsg")

        SendDataToMap(MapNum, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendActionMsg(mapNum as Integer, Message As String, Color As Integer, MsgType As Integer, X As Integer, Y As Integer, Optional PlayerOnlyNum As Integer = 0)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SActionMsg)
        'Buffer.WriteString(Message)
        Buffer.WriteBytes(WriteUnicodeString(Message))
        Buffer.WriteInt32(Color)
        Buffer.WriteInt32(MsgType)
        Buffer.WriteInt32(X)
        Buffer.WriteInt32(Y)

        AddDebug("Sent SMSG: SActionMsg")

        If PlayerOnlyNum > 0 Then
            Socket.SendDataTo(PlayerOnlyNum, Buffer.Data, Buffer.Head)
        Else
            SendDataToMap(MapNum, Buffer.Data, Buffer.Head)
        End If

        Buffer.Dispose()
    End Sub

    Sub SayMsg_Map(mapNum as Integer, index as integer, Message As String, SayColour As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SSayMsg)
        Buffer.WriteString(GetPlayerName(Index))
        Buffer.WriteInt32(GetPlayerAccess(Index))
        Buffer.WriteInt32(GetPlayerPK(Index))
        'Buffer.WriteString(Message)
        Buffer.WriteBytes(WriteUnicodeString(Message))
        Buffer.WriteString("[Map] ")
        Buffer.WriteInt32(SayColour)

        AddDebug("Sent SMSG: SSayMsg")

        SendDataToMap(MapNum, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendPlayerData(index as integer)
        Dim data = PlayerData(Index)
        SendDataToMap(GetPlayerMap(Index), data, data.Length)
    End Sub

    Sub SendUpdateResourceToAll(ResourceNum As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SUpdateResource)
        Buffer.WriteInt32(ResourceNum)

        AddDebug("Sent SMSG: SUpdateResource")

        buffer.WriteInt32(Resource(ResourceNum).Animation)
        Buffer.WriteString(Resource(ResourceNum).EmptyMessage)
        Buffer.WriteInt32(Resource(ResourceNum).ExhaustedImage)
        Buffer.WriteInt32(Resource(ResourceNum).Health)
        Buffer.WriteInt32(Resource(ResourceNum).ExpReward)
        Buffer.WriteInt32(Resource(ResourceNum).ItemReward)
        Buffer.WriteString(Resource(ResourceNum).Name)
        Buffer.WriteInt32(Resource(ResourceNum).ResourceImage)
        Buffer.WriteInt32(Resource(ResourceNum).ResourceType)
        Buffer.WriteInt32(Resource(ResourceNum).RespawnTime)
        Buffer.WriteString(Resource(ResourceNum).SuccessMessage)
        Buffer.WriteInt32(Resource(ResourceNum).LvlRequired)
        Buffer.WriteInt32(Resource(ResourceNum).ToolRequired)
        Buffer.WriteInt32(Resource(ResourceNum).Walkthrough)

        SendDataToAll(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendMapNpcVitals(mapNum as Integer, MapNpcNum As Byte)
        Dim i As Integer
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SMapNpcVitals)
        Buffer.WriteInt32(MapNpcNum)

        AddDebug("Sent SMSG: SMapNpcVitals")

        For i = 1 To VitalType.Count - 1
            Buffer.WriteInt32(MapNpc(MapNum).Npc(MapNpcNum).Vital(i))
        Next

        SendDataToMap(MapNum, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendMapKeyToMap(mapNum as Integer, X As Integer, Y As Integer, Value As Byte)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SMapKey)
        Buffer.WriteInt32(X)
        Buffer.WriteInt32(Y)
        Buffer.WriteInt32(Value)
        SendDataToMap(MapNum, Buffer.Data, Buffer.Head)

        AddDebug("Sent SMSG: SMapKey")

        buffer.Dispose()
    End Sub

    Sub SendResourceCacheToMap(mapNum as Integer, Resource_num As Integer)
        dim buffer as ByteStream
        Dim i As Integer
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SResourceCache)
        Buffer.WriteInt32(ResourceCache(MapNum).ResourceCount)

        AddDebug("Sent SMSG: SResourceCache")

        If ResourceCache(MapNum).ResourceCount > 0 Then

            For i = 0 To ResourceCache(MapNum).ResourceCount
                Buffer.WriteInt32(ResourceCache(MapNum).ResourceData(i).ResourceState)
                Buffer.WriteInt32(ResourceCache(MapNum).ResourceData(i).X)
                Buffer.WriteInt32(ResourceCache(MapNum).ResourceData(i).Y)
            Next

        End If

        SendDataToMap(MapNum, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendGameData(index as integer)
        dim buffer as ByteStream
        Dim i As Integer
        Dim data() As Byte
        Buffer = New ByteStream(4)

        Buffer.WriteBlock(ClassData)

        i = 0

        For x = 1 To MAX_ITEMS
            If Len(Trim$(Item(x).Name)) > 0 Then
                i = i + 1
            End If
        Next

        'Write Number of Items it is Sending and then The Item Data
        Buffer.WriteInt32(i)
        Buffer.WriteBlock(ItemsData)

        i = 0

        For x = 1 To MAX_ANIMATIONS
            If Len(Trim$(Animation(x).Name)) > 0 Then
                i = i + 1
            End If
        Next

        Buffer.WriteInt32(i)
        Buffer.WriteBlock(AnimationsData)

        i = 0

        For x = 1 To MAX_NPCS
            If Len(Trim$(Npc(x).Name)) > 0 Then
                i = i + 1
            End If
        Next

        Buffer.WriteInt32(i)
        Buffer.WriteBlock(NpcsData)

        i = 0

        For x = 1 To MAX_SHOPS
            If Len(Trim$(Shop(x).Name)) > 0 Then
                i = i + 1
            End If
        Next

        Buffer.WriteInt32(i)
        Buffer.WriteBlock(ShopsData)

        i = 0

        For x = 1 To MAX_SKILLS
            If Len(Trim$(Skill(x).Name)) > 0 Then
                i = i + 1
            End If
        Next

        Buffer.WriteInt32(i)
        Buffer.WriteBlock(SkillsData)

        i = 0

        For x = 1 To MAX_RESOURCES
            If Len(Trim$(Resource(x).Name)) > 0 Then
                i = i + 1
            End If
        Next

        Buffer.WriteInt32(i)
        Buffer.WriteBlock(ResourcesData)

        data = Compression.CompressBytes(Buffer.ToArray)

        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SGameData)

        AddDebug("Sent SMSG: SGameData")

        buffer.WriteBlock(data)

        Socket.SendDataTo(index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SayMsg_Global(index as integer, Message As String, SayColour As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SSayMsg)
        Buffer.WriteString(GetPlayerName(Index))
        Buffer.WriteInt32(GetPlayerAccess(Index))
        Buffer.WriteInt32(GetPlayerPK(Index))
        'Buffer.WriteString(Message)
        Buffer.WriteBytes(WriteUnicodeString(Message))
        Buffer.WriteString("[Global] ")
        Buffer.WriteInt32(SayColour)

        AddDebug("Sent SMSG: SSayMsg Global")

        SendDataToAll(Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendInventoryUpdate(index as integer, InvSlot As Integer)
        dim buffer as ByteStream, n As Integer
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SPlayerInvUpdate)
        Buffer.WriteInt32(InvSlot)
        Buffer.WriteInt32(GetPlayerInvItemNum(Index, InvSlot))
        Buffer.WriteInt32(GetPlayerInvItemValue(Index, InvSlot))

        AddDebug("Sent SMSG: SPlayerInvUpdate")

        buffer.WriteString(Player(Index).Character(TempPlayer(Index).CurChar).RandInv(InvSlot).Prefix)
        Buffer.WriteString(Player(Index).Character(TempPlayer(Index).CurChar).RandInv(InvSlot).Suffix)
        Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).RandInv(InvSlot).Rarity)
        For n = 1 To StatType.Count - 1
            Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).RandInv(InvSlot).Stat(n))
        Next n
        Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).RandInv(InvSlot).Damage)
        Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).RandInv(InvSlot).Speed)

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendAnimation(mapNum as Integer, Anim As Integer, X As Integer, Y As Integer, Optional LockType As Byte = 0, Optional Lockindex as integer = 0)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SAnimation)
        Buffer.WriteInt32(Anim)
        Buffer.WriteInt32(X)
        Buffer.WriteInt32(Y)
        Buffer.WriteInt32(LockType)
        Buffer.WriteInt32(LockIndex)

        AddDebug("Sent SMSG: SAnimation")

        SendDataToMap(MapNum, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendOpenShop(index as integer, ShopNum As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SOpenShop)
        Buffer.WriteInt32(ShopNum)
        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        AddDebug("Sent SMSG: SOpenShop")

        buffer.Dispose()
    End Sub

    Sub ResetShopAction(index as integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SResetShopAction)

        AddDebug("Sent SMSG: SResetShopAction")

        SendDataToAll(Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendBank(index as integer)
        dim buffer as ByteStream
        Dim i As Integer

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SBank)

        AddDebug("Sent SMSG: SBank")

        For i = 1 To MAX_BANK
            Buffer.WriteInt32(Bank(Index).Item(i).Num)
            Buffer.WriteInt32(Bank(Index).Item(i).Value)

            Buffer.WriteString(Bank(Index).ItemRand(i).Prefix)
            Buffer.WriteString(Bank(Index).ItemRand(i).Suffix)
            Buffer.WriteInt32(Bank(Index).ItemRand(i).Rarity)
            Buffer.WriteInt32(Bank(Index).ItemRand(i).Damage)
            Buffer.WriteInt32(Bank(Index).ItemRand(i).Speed)

            For x = 1 To StatType.Count - 1
                Buffer.WriteInt32(Bank(Index).ItemRand(i).Stat(x))
            Next
        Next

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendClearSkillBuffer(index as integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SClearSkillBuffer)

        AddDebug("Sent SMSG: SClearSkillBuffer")

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendClearTradeTimer(index as integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SClearTradeTimer)

        AddDebug("Sent SMSG: SClearTradeTimer")

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendTradeInvite(index as integer, Tradeindex as integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.STradeInvite)

        AddDebug("Sent SMSG: STradeInvite")

        buffer.WriteInt32(TradeIndex)

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendTrade(index as integer, TradeTarget As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.STrade)
        Buffer.WriteInt32(TradeTarget)
        Buffer.WriteString(Trim$(GetPlayerName(TradeTarget)))
        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        AddDebug("Sent SMSG: STrade")

        buffer.Dispose()
    End Sub

    Sub SendTradeUpdate(index as integer, DataType As Byte)
        dim buffer as ByteStream
        Dim i As Integer
        Dim tradeTarget As Integer
        Dim totalWorth As Integer

        tradeTarget = TempPlayer(Index).InTrade

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.STradeUpdate)
        Buffer.WriteInt32(DataType)

        AddDebug("Sent SMSG: STradeUpdate")

        If DataType = 0 Then ' own inventory

            For i = 1 To MAX_INV
                Buffer.WriteInt32(TempPlayer(Index).TradeOffer(i).Num)
                Buffer.WriteInt32(TempPlayer(Index).TradeOffer(i).Value)

                ' add total worth
                If TempPlayer(Index).TradeOffer(i).Num > 0 Then
                    ' currency?
                    If Item(TempPlayer(Index).TradeOffer(i).Num).Type = ItemType.Currency OrElse Item(TempPlayer(Index).TradeOffer(i).Num).Stackable = 1 Then
                        If TempPlayer(Index).TradeOffer(i).Value = 0 Then TempPlayer(Index).TradeOffer(i).Value = 1
                        totalWorth = totalWorth + (Item(GetPlayerInvItemNum(Index, TempPlayer(Index).TradeOffer(i).Num)).Price * TempPlayer(Index).TradeOffer(i).Value)
                    Else
                        totalWorth = totalWorth + Item(GetPlayerInvItemNum(Index, TempPlayer(Index).TradeOffer(i).Num)).Price
                    End If
                End If
            Next
        ElseIf DataType = 1 Then ' other inventory

            For i = 1 To MAX_INV
                Buffer.WriteInt32(GetPlayerInvItemNum(tradeTarget, TempPlayer(tradeTarget).TradeOffer(i).Num))
                Buffer.WriteInt32(TempPlayer(tradeTarget).TradeOffer(i).Value)

                ' add total worth
                If GetPlayerInvItemNum(tradeTarget, TempPlayer(tradeTarget).TradeOffer(i).Num) > 0 Then
                    ' currency?
                    If Item(GetPlayerInvItemNum(tradeTarget, TempPlayer(tradeTarget).TradeOffer(i).Num)).Type = ItemType.Currency OrElse Item(GetPlayerInvItemNum(tradeTarget, TempPlayer(tradeTarget).TradeOffer(i).Num)).Stackable = 1 Then
                        If TempPlayer(tradeTarget).TradeOffer(i).Value = 0 Then TempPlayer(tradeTarget).TradeOffer(i).Value = 1
                        totalWorth = totalWorth + (Item(GetPlayerInvItemNum(tradeTarget, TempPlayer(tradeTarget).TradeOffer(i).Num)).Price * TempPlayer(tradeTarget).TradeOffer(i).Value)
                    Else
                        totalWorth = totalWorth + Item(GetPlayerInvItemNum(tradeTarget, TempPlayer(tradeTarget).TradeOffer(i).Num)).Price
                    End If
                End If
            Next
        End If

        ' send total worth of trade
        Buffer.WriteInt32(totalWorth)

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendTradeStatus(index as integer, Status As Byte)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.STradeStatus)
        Buffer.WriteInt32(Status)
        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        AddDebug("Sent SMSG: STradeStatus")

        buffer.Dispose()
    End Sub

    Sub SendMapItemsToAll(mapNum as Integer)
        Dim i As Integer
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SMapItemData)

        AddDebug("Sent SMSG: SMapItemData To All")

        For i = 1 To MAX_MAP_ITEMS
            Buffer.WriteInt32(MapItem(MapNum, i).Num)
            Buffer.WriteInt32(MapItem(MapNum, i).Value)
            Buffer.WriteInt32(MapItem(MapNum, i).X)
            Buffer.WriteInt32(MapItem(MapNum, i).Y)
        Next

        SendDataToMap(MapNum, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendStunned(index as integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SStunned)
        Buffer.WriteInt32(TempPlayer(Index).StunDuration)

        AddDebug("Sent SMSG: SStunned")

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendBlood(mapNum as Integer, X As Integer, Y As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SBlood)
        Buffer.WriteInt32(X)
        Buffer.WriteInt32(Y)

        AddDebug("Sent SMSG: SBlood")

        SendDataToMap(MapNum, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendPlayerSkills(index as integer)
        Dim i As Integer
        dim buffer as ByteStream
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SSkills)

        AddDebug("Sent SMSG: SSkills")

        For i = 1 To MAX_PLAYER_SKILLS
            Buffer.WriteInt32(GetPlayerSkill(Index, i))
        Next

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendCooldown(index as integer, Slot As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SCooldown)
        Buffer.WriteInt32(Slot)

        AddDebug("Sent SMSG: SCooldown")

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendTarget(index as integer, Target As Integer, TargetType As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.STarget)
        Buffer.WriteInt32(Target)
        Buffer.WriteInt32(TargetType)

        AddDebug("Sent SMSG: STarget")

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    'Mapreport
    Sub SendMapReport(index as integer)
        dim buffer as ByteStream, I As Integer

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SMapReport)

        AddDebug("Sent SMSG: SMapReport")

        For I = 1 To MAX_MAPS
            Buffer.WriteString(Trim(Map(I).Name))
        Next

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendAdminPanel(index as integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SAdmin)

        AddDebug("Sent SMSG: SAdmin")

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendMapNames(index as integer)
        dim buffer as ByteStream, I As Integer

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SMapNames)

        AddDebug("Sent SMSG: SMapNames")

        For I = 1 To MAX_MAPS
            Buffer.WriteString(Trim(Map(I).Name))
        Next

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendHotbar(index as integer)
        dim buffer as ByteStream, i As Integer

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SHotbar)

        AddDebug("Sent SMSG: SHotbar")

        For i = 1 To MAX_HOTBAR
            Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).Hotbar(i).Slot)
            Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).Hotbar(i).SlotType)
        Next

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendCritical(index as integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SCritical)

        AddDebug("Sent SMSG: SCritical")

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendKeyPair(index as integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SKeyPair)
        Buffer.WriteString(EKeyPair.ExportKeyString(False))
        Socket.SendDataTo(index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendNews(index as integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SNews)

        AddDebug("Sent SMSG: SNews")

        buffer.WriteString(Trim(Options.GameName))
        Buffer.WriteString(Trim(GetFileContents(Application.StartupPath & "\data\news.txt")))

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendRightClick(index as integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SrClick)

        AddDebug("Sent SMSG: SrClick")

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendClassEditor(index as integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SClassEditor)

        AddDebug("Sent SMSG: SClassEditor")

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendAutoMapper(index as integer)
        dim buffer as ByteStream, Prefab As Integer
        Dim myXml As New XmlClass With {
            .Filename = Application.StartupPath & "\Data\AutoMapper.xml",
            .Root = "Options"
        }
        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SAutoMapper)

        AddDebug("Sent SMSG: SAutoMapper")

        buffer.WriteInt32(MapStart)
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

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendEmote(index as integer, Emote As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SEmote)

        AddDebug("Sent SMSG: SEmote")

        buffer.WriteInt32(Index)
        Buffer.WriteInt32(Emote)

        SendDataToMap(GetPlayerMap(Index), Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendChatBubble(mapNum as Integer, Target As Integer, TargetType As Integer, Message As String, Colour As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SChatBubble)

        AddDebug("Sent SMSG: SChatBubble")

        buffer.WriteInt32(Target)
        Buffer.WriteInt32(TargetType)
        'Buffer.WriteString(Message)
        Buffer.WriteBytes(WriteUnicodeString(Message))
        Buffer.WriteInt32(Colour)
        SendDataToMap(MapNum, Buffer.Data, Buffer.Head)

        Buffer.Dispose()

    End Sub

    Sub SendPlayerAttack(index as integer)
        Dim Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SAttack)

        AddDebug("Sent SMSG: SPlayerAttack")

        Buffer.WriteInt32(Index)
        SendDataToMapBut(Index, GetPlayerMap(Index), Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendNpcAttack(index as integer, NpcNum As Integer)
        Dim Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SAttack)

        AddDebug("Sent SMSG: SNpcAttack")

        Buffer.WriteInt32(NpcNum)
        SendDataToMap(GetPlayerMap(Index), Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendNpcDead(mapNum as Integer, index as integer)
        Dim Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SNpcDead)

        AddDebug("Sent SMSG: SNpcDead")

        Buffer.WriteInt32(Index)
        SendDataToMap(MapNum, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendTotalOnlineTo(index as integer)
        Dim Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.STotalOnline)

        AddDebug("Sent SMSG: STotalOnline")

        Buffer.WriteInt32(GetPlayersOnline)
        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub SendTotalOnlineToAll()
        Dim Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.STotalOnline)

        AddDebug("Sent SMSG: STotalOnline To All")

        Buffer.WriteInt32(GetPlayersOnline)
        SendDataToAll(Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub
End Module
