Imports System.Windows.Forms
Imports ASFW
Imports ASFW.IO

Module ClientNetworkSend
    Friend Sub SendNewAccount(name As String, password As String)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CNewAccount)
        Buffer.WriteString(EKeyPair.EncryptString(Name))
        Buffer.WriteString(EKeyPair.EncryptString(Password))
        Socket.SendData(Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Friend Sub SendAddChar(slot As Integer, name As String, sex As Integer, classNum As Integer, sprite As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CAddChar)
        Buffer.WriteInt32(Slot)
        Buffer.WriteString(Name)
        Buffer.WriteInt32(Sex)
        Buffer.WriteInt32(ClassNum)
        Buffer.WriteInt32(Sprite)
        Socket.SendData(Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Friend Sub SendLogin(name As String, password As String)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CLogin)
        Buffer.WriteString(EKeyPair.EncryptString(Name))
        Buffer.WriteString(EKeyPair.EncryptString(Password))
        Buffer.WriteString(EKeyPair.EncryptString(Application.ProductVersion))
        Socket.SendData(Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Sub GetPing()
        dim buffer as New ByteStream(4)
        PingStart = GetTickCount()

        Buffer.WriteInt32(ClientPackets.CCheckPing)
        Socket.SendData(Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Friend Sub SendRequestEditMap()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestEditMap)
        Socket.SendData(Buffer.Data, Buffer.Head)

        Buffer.Dispose()
    End Sub

    Friend Sub SendMap()
        Dim x As Integer, y As Integer, i As Integer
        Dim data() As Byte
        dim buffer as New ByteStream(4)
        CanMoveNow = False

        Buffer.WriteString(Trim$(Map.Name))
        Buffer.WriteString(Trim$(Map.Music))
        Buffer.WriteInt32(Map.Moral)
        Buffer.WriteInt32(Map.Tileset)
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
        Buffer.WriteInt32(ClientPackets.CSaveMap)
        Buffer.WriteBlock(Compression.CompressBytes(data))

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendPlayerMove()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CPlayerMove)
        Buffer.WriteInt32(GetPlayerDir(MyIndex))
        Buffer.WriteInt32(Player(MyIndex).Moving)
        Buffer.WriteInt32(Player(MyIndex).X)
        Buffer.WriteInt32(Player(MyIndex).Y)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SayMsg(text As String)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CSayMsg)
        'Buffer.WriteString(text)
        Buffer.WriteBytes(WriteUnicodeString(text))

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendKick(name As String)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CKickPlayer)
        Buffer.WriteString(Name)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendBan(name As String)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CBanPlayer)
        Buffer.WriteString(Name)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub WarpMeTo(name As String)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CWarpMeTo)
        Buffer.WriteString(Name)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub WarpToMe(name As String)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CWarpToMe)
        Buffer.WriteString(Name)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub WarpTo(mapNum as Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CWarpTo)
        Buffer.WriteInt32(MapNum)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendRequestLevelUp()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestLevelUp)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendSpawnItem(tmpItem As Integer, tmpAmount As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CSpawnItem)
        Buffer.WriteInt32(tmpItem)
        Buffer.WriteInt32(tmpAmount)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendSetSprite(spriteNum As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CSetSprite)
        Buffer.WriteInt32(SpriteNum)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendSetAccess(name As String, access As Byte)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CSetAccess)
        Buffer.WriteString(Name)
        Buffer.WriteInt32(Access)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendAttack()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CAttack)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendRequestItems()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestItems)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendPlayerDir()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CPlayerDir)
        Buffer.WriteInt32(GetPlayerDir(MyIndex))

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendPlayerRequestNewMap()
        If GettingMap Then Exit Sub

        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestNewMap)
        Buffer.WriteInt32(GetPlayerDir(MyIndex))

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Sub SendRequestResources()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestResources)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendRequestNpcs()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestNPCS)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendRequestSkills()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestSkills)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendRequestShops()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestShops)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendRequestAnimations()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestAnimations)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendMapRespawn()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CMapRespawn)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendTrainStat(statNum As Byte)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CTrainStat)
        Buffer.WriteInt32(StatNum)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendRequestPlayerData()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestPlayerData)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub BroadcastMsg(text As String)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CBroadcastMsg)
        'Buffer.WriteString(text)
        Buffer.WriteBytes(WriteUnicodeString(text))

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub PlayerMsg(text As String, msgTo As String)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CPlayerMsg)
        Buffer.WriteString(MsgTo)
        'Buffer.WriteString(text)
        Buffer.WriteBytes(WriteUnicodeString(text))

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendWhosOnline()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CWhosOnline)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendMotdChange(motd As String)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CSetMotd)
        Buffer.WriteString(MOTD)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendBanList()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CBanList)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendChangeInvSlots(oldSlot As Integer, newSlot As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CSwapInvSlots)
        Buffer.WriteInt32(OldSlot)
        Buffer.WriteInt32(NewSlot)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendUseItem(invNum As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CUseItem)
        Buffer.WriteInt32(InvNum)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendDropItem(invNum As Integer, amount As Integer)
        dim buffer as New ByteStream(4)

        If InBank OrElse InShop Then Exit Sub

        ' do basic checks
        If InvNum < 1 OrElse InvNum > MAX_INV Then Exit Sub
        If PlayerInv(InvNum).Num < 1 OrElse PlayerInv(InvNum).Num > MAX_ITEMS Then Exit Sub
        If Item(GetPlayerInvItemNum(MyIndex, InvNum)).Type = ItemType.Currency OrElse Item(GetPlayerInvItemNum(MyIndex, InvNum)).Stackable = 1 Then
            If Amount < 1 OrElse Amount > PlayerInv(InvNum).Value Then Exit Sub
        End If

        Buffer.WriteInt32(ClientPackets.CMapDropItem)
        Buffer.WriteInt32(InvNum)
        Buffer.WriteInt32(Amount)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub BuyItem(shopslot As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CBuyItem)
        Buffer.WriteInt32(shopslot)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SellItem(invslot As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CSellItem)
        Buffer.WriteInt32(invslot)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub DepositItem(invslot As Integer, amount As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CDepositItem)
        Buffer.WriteInt32(invslot)
        Buffer.WriteInt32(Amount)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub WithdrawItem(bankslot As Integer, amount As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CWithdrawItem)
        Buffer.WriteInt32(bankslot)
        Buffer.WriteInt32(Amount)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub ChangeBankSlots(oldSlot As Integer, newSlot As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CChangeBankSlots)
        Buffer.WriteInt32(OldSlot)
        Buffer.WriteInt32(NewSlot)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub CloseBank()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CCloseBank)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

        InBank = False
        pnlBankVisible = False
    End Sub

    Sub PlayerSearch(curX As Integer, curY As Integer, rClick As Byte)
        dim buffer as New ByteStream(4)

        If IsInBounds() Then
            Buffer.WriteInt32(ClientPackets.CSearch)
            Buffer.WriteInt32(CurX)
            Buffer.WriteInt32(CurY)
            Buffer.WriteInt32(RClick)
            Socket.SendData(Buffer.Data, Buffer.Head)
        End If

        Buffer.Dispose()
    End Sub

    Friend Sub AdminWarp(x As Integer, y As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CAdminWarp)
        Buffer.WriteInt32(X)
        Buffer.WriteInt32(Y)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendTradeRequest(name As String)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CTradeInvite)

        Buffer.WriteString(Name)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Sub SendTradeInviteAccept(awnser As Byte)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CTradeInviteAccept)

        Buffer.WriteInt32(Awnser)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Friend Sub TradeItem(invslot As Integer, amount As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CTradeItem)
        Buffer.WriteInt32(invslot)
        Buffer.WriteInt32(Amount)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub UntradeItem(invslot As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CUntradeItem)
        Buffer.WriteInt32(invslot)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub AcceptTrade()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CAcceptTrade)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub DeclineTrade()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CDeclineTrade)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendLeaveGame()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CQuit)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendUnequip(eqNum As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CUnequip)
        Buffer.WriteInt32(EqNum)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub ForgetSkill(skillslot As Integer)
        dim buffer as New ByteStream(4)

        ' Check for subscript out of range
        If Skillslot < 1 OrElse Skillslot > MAX_PLAYER_SKILLS Then Exit Sub

        ' dont let them forget a skill which is in CD
        If SkillCD(Skillslot) > 0 Then
            AddText("Cannot forget a skill which is cooling down!", QColorType.AlertColor)
            Exit Sub
        End If

        ' dont let them forget a skill which is buffered
        If SkillBuffer = Skillslot Then
            AddText("Cannot forget a skill which you are casting!", QColorType.AlertColor)
            Exit Sub
        End If

        If PlayerSkills(Skillslot) > 0 Then
            Buffer.WriteInt32(ClientPackets.CForgetSkill)
            Buffer.WriteInt32(Skillslot)
            Socket.SendData(Buffer.Data, Buffer.Head)
        Else
            AddText("No skill found.", QColorType.AlertColor)
        End If

        Buffer.Dispose()
    End Sub

    Friend Sub SendRequestMapreport()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CMapReport)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendRequestAdmin()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CAdmin)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendRequestClasses()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestClasses)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendUseEmote(emote As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CEmote)
        Buffer.WriteInt32(Emote)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub
End Module
