Imports System.IO
Imports ASFW
Imports ASFW.IO
Imports ASFW.IO.FileIO

Friend Module ServerQuest
#Region "Constants"
    'Constants
    Friend Const MAX_QUESTS As Byte = 250
    'Friend Const MAX_TASKS As Byte = 10
    'Friend Const MAX_REQUIREMENTS As Byte = 10
    Friend Const MAX_ACTIVEQUESTS = 10

    Friend Const EDITOR_TASKS As Byte = 7

    'Friend Const QUEST_TYPE_GOSLAY As Byte = 1
    'Friend Const QUEST_TYPE_GOCOLLECT As Byte = 2
    'Friend Const QUEST_TYPE_GOTALK As Byte = 3
    'Friend Const QUEST_TYPE_GOREACH As Byte = 4
    'Friend Const QUEST_TYPE_GOGIVE As Byte = 5
    'Friend Const QUEST_TYPE_GOKILL As Byte = 6
    'Friend Const QUEST_TYPE_GOGATHER As Byte = 7
    'Friend Const QUEST_TYPE_GOFETCH As Byte = 8
    'Friend Const QUEST_TYPE_TALKEVENT As Byte = 9

    'Friend Const QUEST_NOT_STARTED As Byte = 0
    'Friend Const QUEST_STARTED As Byte = 1
    'Friend Const QUEST_COMPLETED As Byte = 2
    'Friend Const QUEST_REPEATABLE As Byte = 3

    'Types
    Friend Quest(MAX_QUESTS) As QuestRec

    Friend Structure PlayerQuestRec
        Dim Status As Integer '0=not started, 1=started, 2=completed, 3=completed but repeatable
        Dim ActualTask As Integer
        Dim CurrentCount As Integer 'Used to handle the Amount property
    End Structure

    Friend Structure TaskRec
        Dim Order As Integer
        Dim NPC As Integer
        Dim Item As Integer
        Dim Map As Integer
        Dim Resource As Integer
        Dim Amount As Integer
        Dim Speech As String
        Dim TaskLog As String
        Dim QuestEnd As Byte
        Dim TaskType As Integer
    End Structure

    Friend Structure QuestRec
        Dim Name As String
        Dim QuestLog As String
        Dim Repeat As Byte
        Dim Cancelable As Byte

        Dim ReqCount As Integer
        Dim Requirement() As Integer '1=item, 2=quest, 3=class
        Dim RequirementIndex() As Integer

        Dim QuestGiveItem As Integer 'Todo: make this dynamic
        Dim QuestGiveItemValue As Integer
        Dim QuestRemoveItem As Integer
        Dim QuestRemoveItemValue As Integer

        Dim Chat() As String

        Dim RewardCount As Integer
        Dim RewardItem() As Integer
        Dim RewardItemAmount() As Integer
        Dim RewardExp As Integer

        Dim TaskCount As Integer
        Dim Task() As TaskRec

    End Structure
#End Region

#Region "Database"
    Sub SaveQuests()
        Dim I As Integer
        For I = 1 To MAX_QUESTS
            SaveQuest(I)
            Application.DoEvents()
        Next
    End Sub

    Sub SaveQuest(QuestNum As Integer)
        Dim filename As String
        Dim I As Integer
        filename = Path.Combine(Application.StartupPath, "data", "quests", String.Format("quest{0}.dat", QuestNum))

        Dim writer As New ByteStream(100)

        writer.WriteString(Quest(QuestNum).Name)
        writer.WriteString(Quest(QuestNum).QuestLog)
        writer.WriteByte(Quest(QuestNum).Repeat)
        writer.WriteByte(Quest(QuestNum).Cancelable)

        writer.WriteInt32(Quest(QuestNum).ReqCount)
        For I = 1 To Quest(QuestNum).ReqCount
            writer.WriteInt32(Quest(QuestNum).Requirement(I))
            writer.WriteInt32(Quest(QuestNum).RequirementIndex(I))
        Next

        writer.WriteInt32(Quest(QuestNum).QuestGiveItem)
        writer.WriteInt32(Quest(QuestNum).QuestGiveItemValue)
        writer.WriteInt32(Quest(QuestNum).QuestRemoveItem)
        writer.WriteInt32(Quest(QuestNum).QuestRemoveItemValue)

        For I = 1 To 3
            writer.WriteString(Quest(QuestNum).Chat(I))
        Next

        writer.WriteInt32(Quest(QuestNum).RewardCount)
        For I = 1 To Quest(QuestNum).RewardCount
            writer.WriteInt32(Quest(QuestNum).RewardItem(I))
            writer.WriteInt32(Quest(QuestNum).RewardItemAmount(I))
        Next
        writer.WriteInt32(Quest(QuestNum).RewardExp)

        writer.WriteInt32(Quest(QuestNum).TaskCount)
        For I = 1 To Quest(QuestNum).TaskCount
            writer.WriteInt32(Quest(QuestNum).Task(I).Order)
            writer.WriteInt32(Quest(QuestNum).Task(I).NPC)
            writer.WriteInt32(Quest(QuestNum).Task(I).Item)
            writer.WriteInt32(Quest(QuestNum).Task(I).Map)
            writer.WriteInt32(Quest(QuestNum).Task(I).Resource)
            writer.WriteInt32(Quest(QuestNum).Task(I).Amount)
            writer.WriteString(Quest(QuestNum).Task(I).Speech)
            writer.WriteString(Quest(QuestNum).Task(I).TaskLog)
            writer.WriteByte(Quest(QuestNum).Task(I).QuestEnd)
            writer.WriteInt32(Quest(QuestNum).Task(I).TaskType)
        Next

        BinaryFile.Save(filename, writer)
    End Sub

    Sub LoadQuests()
        Dim I As Integer

        CheckQuests()

        For I = 1 To MAX_QUESTS
            LoadQuest(I)
            Application.DoEvents()
        Next
    End Sub

    Sub LoadQuest(QuestNum As Integer)
        Dim FileName As String
        Dim I As Integer

        FileName = Path.Combine(Application.StartupPath, "data", "quests", String.Format("quest{0}.dat", QuestNum))

        Dim reader As New ByteStream()
        BinaryFile.Load(FileName, reader)

        Quest(QuestNum).Name = reader.ReadString()
        Quest(QuestNum).QuestLog = reader.ReadString()
        Quest(QuestNum).Repeat = reader.ReadByte()
        Quest(QuestNum).Cancelable = reader.ReadByte()

        Quest(QuestNum).ReqCount = reader.ReadInt32()
        ReDim Quest(QuestNum).Requirement(Quest(QuestNum).ReqCount)
        ReDim Quest(QuestNum).RequirementIndex(Quest(QuestNum).ReqCount)
        For I = 1 To Quest(QuestNum).ReqCount
            Quest(QuestNum).Requirement(I) = reader.ReadInt32()
            Quest(QuestNum).RequirementIndex(I) = reader.ReadInt32()
        Next

        Quest(QuestNum).QuestGiveItem = reader.ReadInt32()
        Quest(QuestNum).QuestGiveItemValue = reader.ReadInt32()
        Quest(QuestNum).QuestRemoveItem = reader.ReadInt32()
        Quest(QuestNum).QuestRemoveItemValue = reader.ReadInt32()

        For I = 1 To 3
            Quest(QuestNum).Chat(I) = reader.ReadString()
        Next

        Quest(QuestNum).RewardCount = reader.ReadInt32()
        ReDim Quest(QuestNum).RewardItem(Quest(QuestNum).RewardCount)
        ReDim Quest(QuestNum).RewardItemAmount(Quest(QuestNum).RewardCount)
        For I = 1 To Quest(QuestNum).RewardCount
            Quest(QuestNum).RewardItem(I) = reader.ReadInt32()
            Quest(QuestNum).RewardItemAmount(I) = reader.ReadInt32()
        Next
        Quest(QuestNum).RewardExp = reader.ReadInt32()

        Quest(QuestNum).TaskCount = reader.ReadInt32()
        ReDim Quest(QuestNum).Task(Quest(QuestNum).TaskCount)
        For I = 1 To Quest(QuestNum).TaskCount
            Quest(QuestNum).Task(I).Order = reader.ReadInt32()
            Quest(QuestNum).Task(I).NPC = reader.ReadInt32()
            Quest(QuestNum).Task(I).Item = reader.ReadInt32()
            Quest(QuestNum).Task(I).Map = reader.ReadInt32()
            Quest(QuestNum).Task(I).Resource = reader.ReadInt32()
            Quest(QuestNum).Task(I).Amount = reader.ReadInt32()
            Quest(QuestNum).Task(I).Speech = reader.ReadString()
            Quest(QuestNum).Task(I).TaskLog = reader.ReadString()
            Quest(QuestNum).Task(I).QuestEnd = reader.ReadByte()
            Quest(QuestNum).Task(I).TaskType = reader.ReadInt32()
        Next
    End Sub

    Sub CheckQuests()
        Dim I As Integer
        For I = 1 To MAX_QUESTS
            If Not File.Exists(Path.Combine(Application.StartupPath, "data", "quests", String.Format("quest{0}.dat", I))) Then
                SaveQuest(I)
                Application.DoEvents()
            End If
        Next
    End Sub

    Sub ClearQuest(QuestNum As Integer)
        Dim I As Integer

        ' clear the Quest
        Quest(QuestNum).Name = ""
        Quest(QuestNum).QuestLog = ""
        Quest(QuestNum).Repeat = 0
        Quest(QuestNum).Cancelable = 0

        Quest(QuestNum).ReqCount = 0
        ReDim Quest(QuestNum).Requirement(Quest(QuestNum).ReqCount)
        ReDim Quest(QuestNum).RequirementIndex(Quest(QuestNum).ReqCount)
        For I = 1 To Quest(QuestNum).ReqCount
            Quest(QuestNum).Requirement(QuestNum) = 0
            Quest(QuestNum).RequirementIndex(QuestNum) = 0
        Next

        Quest(QuestNum).QuestGiveItem = 0
        Quest(QuestNum).QuestGiveItemValue = 0
        Quest(QuestNum).QuestRemoveItem = 0
        Quest(QuestNum).QuestRemoveItemValue = 0

        ReDim Quest(QuestNum).Chat(3)
        For I = 1 To 3
            Quest(QuestNum).Chat(I) = ""
        Next

        Quest(QuestNum).RewardCount = 0
        ReDim Quest(QuestNum).RewardItem(Quest(QuestNum).RewardCount)
        ReDim Quest(QuestNum).RewardItemAmount(Quest(QuestNum).RewardCount)
        For I = 1 To Quest(QuestNum).RewardCount
            Quest(QuestNum).RewardItem(I) = 0
            Quest(QuestNum).RewardItemAmount(I) = 0
        Next
        Quest(QuestNum).RewardExp = 0

        Quest(QuestNum).TaskCount = 0
        ReDim Quest(QuestNum).Task(Quest(QuestNum).TaskCount)
        For I = 1 To Quest(QuestNum).TaskCount
            Quest(QuestNum).Task(I).Order = 0
            Quest(QuestNum).Task(I).NPC = 0
            Quest(QuestNum).Task(I).Item = 0
            Quest(QuestNum).Task(I).Map = 0
            Quest(QuestNum).Task(I).Resource = 0
            Quest(QuestNum).Task(I).Amount = 0
            Quest(QuestNum).Task(I).Speech = ""
            Quest(QuestNum).Task(I).TaskLog = ""
            Quest(QuestNum).Task(I).QuestEnd = 0
            Quest(QuestNum).Task(I).TaskType = 0
        Next

    End Sub

    Sub ClearQuests()
        Dim I As Integer

        For I = 1 To MAX_QUESTS
            ClearQuest(I)
        Next
    End Sub
#End Region

#Region "Incoming Packets"
    Sub Packet_RequestEditQuest(index as integer, ByRef data() As Byte)
        ' Prevent hacking
        If GetPlayerAccess(Index) < AdminType.Developer Then Exit Sub

        Dim Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SQuestEditor)
        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub Packet_SaveQuest(index as integer, ByRef data() As Byte)
        Dim QuestNum As Integer
        dim buffer as New ByteStream(data)
        ' Prevent hacking
        If GetPlayerAccess(Index) < AdminType.Developer Then Exit Sub

        QuestNum = Buffer.ReadInt32
        If QuestNum < 0 OrElse QuestNum > MAX_QUESTS Then Exit Sub

        ' Update the Quest
        Quest(QuestNum).Name = Buffer.ReadString
        Quest(QuestNum).QuestLog = Buffer.ReadString
        Quest(QuestNum).Repeat = Buffer.ReadInt32
        Quest(QuestNum).Cancelable = Buffer.ReadInt32

        Quest(QuestNum).ReqCount = Buffer.ReadInt32
        ReDim Quest(QuestNum).Requirement(Quest(QuestNum).ReqCount)
        ReDim Quest(QuestNum).RequirementIndex(Quest(QuestNum).ReqCount)
        For I = 1 To Quest(QuestNum).ReqCount
            Quest(QuestNum).Requirement(I) = Buffer.ReadInt32
            Quest(QuestNum).RequirementIndex(I) = Buffer.ReadInt32
        Next

        Quest(QuestNum).QuestGiveItem = Buffer.ReadInt32
        Quest(QuestNum).QuestGiveItemValue = Buffer.ReadInt32
        Quest(QuestNum).QuestRemoveItem = Buffer.ReadInt32
        Quest(QuestNum).QuestRemoveItemValue = Buffer.ReadInt32

        For I = 1 To 3
            Quest(QuestNum).Chat(I) = Buffer.ReadString
        Next

        Quest(QuestNum).RewardCount = Buffer.ReadInt32
        ReDim Quest(QuestNum).RewardItem(Quest(QuestNum).RewardCount)
        ReDim Quest(QuestNum).RewardItemAmount(Quest(QuestNum).RewardCount)
        For i = 1 To Quest(QuestNum).RewardCount
            Quest(QuestNum).RewardItem(i) = Buffer.ReadInt32
            Quest(QuestNum).RewardItemAmount(i) = Buffer.ReadInt32
        Next

        Quest(QuestNum).RewardExp = Buffer.ReadInt32

        Quest(QuestNum).TaskCount = Buffer.ReadInt32
        ReDim Quest(QuestNum).Task(Quest(QuestNum).TaskCount)
        For I = 1 To Quest(QuestNum).TaskCount
            Quest(QuestNum).Task(I).Order = Buffer.ReadInt32
            Quest(QuestNum).Task(I).NPC = Buffer.ReadInt32
            Quest(QuestNum).Task(I).Item = Buffer.ReadInt32
            Quest(QuestNum).Task(I).Map = Buffer.ReadInt32
            Quest(QuestNum).Task(I).Resource = Buffer.ReadInt32
            Quest(QuestNum).Task(I).Amount = Buffer.ReadInt32
            Quest(QuestNum).Task(I).Speech = Buffer.ReadString
            Quest(QuestNum).Task(I).TaskLog = Buffer.ReadString
            Quest(QuestNum).Task(I).QuestEnd = Buffer.ReadInt32
            Quest(QuestNum).Task(I).TaskType = Buffer.ReadInt32
        Next

        Buffer.Dispose()

        ' Save it
        SendQuests(Index) ' editor
        SendUpdateQuestToAll(QuestNum) 'players
        SaveQuest(QuestNum)
        Addlog(GetPlayerLogin(Index) & " saved Quest #" & QuestNum & ".", ADMIN_LOG)
    End Sub

    Sub Packet_RequestQuests(index as integer, ByRef data() As Byte)
        SendQuests(Index)
    End Sub

    Sub Packet_PlayerHandleQuest(index as integer, ByRef data() As Byte)
        Dim QuestNum As Integer, Order As Integer ', I As Integer
        dim buffer as New ByteStream(data)
        QuestNum = Buffer.ReadInt32
        Order = Buffer.ReadInt32 '1 = accept, 2 = cancel

        If Order = 1 Then
            Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).Status = QuestStatusType.Started '1
            Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).ActualTask = 1
            Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).CurrentCount = 0
            PlayerMsg(Index, "New quest accepted: " & Trim$(Quest(QuestNum).Name) & "!", ColorType.BrightGreen)
        ElseIf Order = 2 Then
            Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).Status = QuestStatusType.NotStarted '2
            Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).ActualTask = 1
            Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).CurrentCount = 0

            PlayerMsg(Index, Trim$(Quest(QuestNum).Name) & " has been canceled!", ColorType.BrightRed)

            If GetPlayerAccess(Index) > 0 AndAlso QuestNum = 1 Then
                For I = 1 To MAX_QUESTS
                    Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(I).Status = QuestStatusType.NotStarted '2
                    Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(I).ActualTask = 1
                    Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(I).CurrentCount = 0
                Next
            End If
        End If

        SavePlayer(Index)
        SendPlayerData(Index)
        SendPlayerQuests(Index)
        Buffer.Dispose()
    End Sub

    Sub Packet_QuestLogUpdate(index as integer, ByRef data() As Byte)
        SendPlayerQuests(Index)
    End Sub

    Sub Packet_QuestReset(index as integer, ByRef data() As Byte)
        Dim QuestNum As Integer

        ' Prevent hacking
        If GetPlayerAccess(Index) < AdminType.Mapper Then Exit Sub
        dim buffer as New ByteStream(data)
        QuestNum = Buffer.ReadInt32

        ResetQuest(Index, QuestNum)

        Buffer.Dispose()
    End Sub
#End Region

#Region "Outgoing packets"
    Sub SendQuests(index as integer)
        Dim I As Integer

        For I = 1 To MAX_QUESTS
            If Len(Trim$(Quest(I).Name)) > 0 Then
                SendUpdateQuestTo(Index, I)
            End If
        Next
    End Sub

    Sub SendUpdateQuestToAll(QuestNum As Integer)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SUpdateQuest)
        Buffer.WriteInt32(QuestNum)

        Buffer.WriteString(Trim(Quest(QuestNum).Name))
        Buffer.WriteString(Trim(Quest(QuestNum).QuestLog))
        Buffer.WriteInt32(Quest(QuestNum).Repeat)
        Buffer.WriteInt32(Quest(QuestNum).Cancelable)

        Buffer.WriteInt32(Quest(QuestNum).ReqCount)
        For I = 1 To Quest(QuestNum).ReqCount
            Buffer.WriteInt32(Quest(QuestNum).Requirement(I))
            Buffer.WriteInt32(Quest(QuestNum).RequirementIndex(I))
        Next

        Buffer.WriteInt32(Quest(QuestNum).QuestGiveItem)
        Buffer.WriteInt32(Quest(QuestNum).QuestGiveItemValue)
        Buffer.WriteInt32(Quest(QuestNum).QuestRemoveItem)
        Buffer.WriteInt32(Quest(QuestNum).QuestRemoveItemValue)

        For I = 1 To 3
            Buffer.WriteString(Trim(Quest(QuestNum).Chat(I)))
        Next

        Buffer.WriteInt32(Quest(QuestNum).RewardCount)
        For i = 1 To Quest(QuestNum).RewardCount
            Buffer.WriteInt32(Quest(QuestNum).RewardItem(i))
            Buffer.WriteInt32(Quest(QuestNum).RewardItemAmount(i))
        Next

        Buffer.WriteInt32(Quest(QuestNum).RewardExp)

        Buffer.WriteInt32(Quest(QuestNum).TaskCount)
        For I = 1 To Quest(QuestNum).TaskCount
            Buffer.WriteInt32(Quest(QuestNum).Task(I).Order)
            Buffer.WriteInt32(Quest(QuestNum).Task(I).NPC)
            Buffer.WriteInt32(Quest(QuestNum).Task(I).Item)
            Buffer.WriteInt32(Quest(QuestNum).Task(I).Map)
            Buffer.WriteInt32(Quest(QuestNum).Task(I).Resource)
            Buffer.WriteInt32(Quest(QuestNum).Task(I).Amount)
            Buffer.WriteString(Trim(Quest(QuestNum).Task(I).Speech))
            Buffer.WriteString(Trim(Quest(QuestNum).Task(I).TaskLog))
            Buffer.WriteInt32(Quest(QuestNum).Task(I).QuestEnd)
            Buffer.WriteInt32(Quest(QuestNum).Task(I).TaskType)
        Next

        SendDataToAll(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendUpdateQuestTo(index as integer, QuestNum As Integer)
        dim buffer as ByteStream, I As Integer
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SUpdateQuest)
        Buffer.WriteInt32(QuestNum)

        Buffer.WriteString(Trim(Quest(QuestNum).Name))
        Buffer.WriteString(Trim(Quest(QuestNum).QuestLog))
        Buffer.WriteInt32(Quest(QuestNum).Repeat)
        Buffer.WriteInt32(Quest(QuestNum).Cancelable)

        Buffer.WriteInt32(Quest(QuestNum).ReqCount)
        For I = 1 To Quest(QuestNum).ReqCount
            Buffer.WriteInt32(Quest(QuestNum).Requirement(I))
            Buffer.WriteInt32(Quest(QuestNum).RequirementIndex(I))
        Next

        Buffer.WriteInt32(Quest(QuestNum).QuestGiveItem)
        Buffer.WriteInt32(Quest(QuestNum).QuestGiveItemValue)
        Buffer.WriteInt32(Quest(QuestNum).QuestRemoveItem)
        Buffer.WriteInt32(Quest(QuestNum).QuestRemoveItemValue)

        For I = 1 To 3
            Buffer.WriteString(Trim(Quest(QuestNum).Chat(I)))
        Next

        Buffer.WriteInt32(Quest(QuestNum).RewardCount)
        For I = 1 To Quest(QuestNum).RewardCount
            Buffer.WriteInt32(Quest(QuestNum).RewardItem(I))
            Buffer.WriteInt32(Quest(QuestNum).RewardItemAmount(I))
        Next

        Buffer.WriteInt32(Quest(QuestNum).RewardExp)

        Buffer.WriteInt32(Quest(QuestNum).TaskCount)
        For I = 1 To Quest(QuestNum).TaskCount
            Buffer.WriteInt32(Quest(QuestNum).Task(I).Order)
            Buffer.WriteInt32(Quest(QuestNum).Task(I).NPC)
            Buffer.WriteInt32(Quest(QuestNum).Task(I).Item)
            Buffer.WriteInt32(Quest(QuestNum).Task(I).Map)
            Buffer.WriteInt32(Quest(QuestNum).Task(I).Resource)
            Buffer.WriteInt32(Quest(QuestNum).Task(I).Amount)
            Buffer.WriteString(Trim(Quest(QuestNum).Task(I).Speech))
            Buffer.WriteString(Trim(Quest(QuestNum).Task(I).TaskLog))
            Buffer.WriteInt32(Quest(QuestNum).Task(I).QuestEnd)
            Buffer.WriteInt32(Quest(QuestNum).Task(I).TaskType)
        Next

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub SendPlayerQuests(index as integer)
        Dim I As Integer
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SPlayerQuests)

        For I = 1 To MAX_QUESTS
            Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(I).Status)
            Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(I).ActualTask)
            Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(I).CurrentCount)
        Next

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Friend Sub SendPlayerQuest(index as integer, QuestNum As Integer)
        dim buffer as ByteStream

        Buffer = New ByteStream(4)
        Buffer.WriteInt32(ServerPackets.SPlayerQuest)

        Buffer.WriteInt32(QuestNum)
        Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).Status)
        Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).ActualTask)
        Buffer.WriteInt32(Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).CurrentCount)

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    'sends a message to the client that is shown on the screen
    Friend Sub QuestMessage(index as integer, QuestNum As Integer, message As String, QuestNumForStart As Integer)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SQuestMessage)

        Buffer.WriteInt32(QuestNum)
        Buffer.WriteString(Trim$(message))
        Buffer.WriteInt32(QuestNumForStart)

        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub
#End Region

#Region "Functions"

    Friend Sub ResetQuest(index as integer, QuestNum As Integer)
        If GetPlayerAccess(Index) > 0 Then
            Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).Status = QuestStatusType.NotStarted
            Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).ActualTask = 1
            Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).CurrentCount = 0

            SendPlayerQuests(Index)
            PlayerMsg(Index, "Quest " & QuestNum & " reset!", ColorType.BrightRed)
        End If
    End Sub

    Friend Function CanStartQuest(index as integer, QuestNum As Integer) As Boolean
        CanStartQuest = False
        If QuestNum < 1 OrElse QuestNum > MAX_QUESTS Then Exit Function
        If QuestInProgress(Index, QuestNum) Then Exit Function

        'Check if player has the quest 0 (not started) or 3 (completed but it can be started again)
        If Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).Status = QuestStatusType.NotStarted OrElse Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).Status = QuestStatusType.Repeatable Then
            For i = 1 To Quest(QuestNum).ReqCount
                'Check if item is needed
                If Quest(QuestNum).Requirement(i) = 1 Then
                    If Quest(QuestNum).RequirementIndex(i) > 0 AndAlso Quest(QuestNum).RequirementIndex(i) <= MAX_ITEMS Then
                        If HasItem(Index, Quest(QuestNum).RequirementIndex(i)) = 0 Then
                            PlayerMsg(Index, "You need " & Item(Quest(QuestNum).Requirement(2)).Name & " to take this quest!", ColorType.Yellow)
                            Exit Function
                        End If
                    End If
                End If

                'Check if previous quest is needed
                If Quest(QuestNum).Requirement(i) = 2 Then
                    If Quest(QuestNum).RequirementIndex(i) > 0 AndAlso Quest(QuestNum).RequirementIndex(i) <= MAX_QUESTS Then
                        If Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(Quest(QuestNum).Requirement(2)).Status = QuestStatusType.NotStarted OrElse Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(Quest(QuestNum).Requirement(2)).Status = QuestStatusType.Started Then
                            PlayerMsg(Index, "You need to complete the " & Trim$(Quest(Quest(QuestNum).Requirement(2)).Name) & " quest in order to take this quest!", ColorType.Yellow)
                            Exit Function
                        End If
                    End If
                End If

            Next

            'Go on :)
            CanStartQuest = True
        Else
            'PlayerMsg Index, "You can't start that quest again!", BrightRed
        End If
    End Function

    Friend Function CanEndQuest(index as integer, QuestNum As Integer) As Boolean
        CanEndQuest = False

        If Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).ActualTask >= Quest(QuestNum).Task.Length Then
            CanEndQuest = True
        End If
        If Quest(QuestNum).Task(Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).ActualTask).QuestEnd = 1 Then
            CanEndQuest = True
        End If
    End Function

    'Tells if the quest is in progress or not
    Friend Function QuestInProgress(index as integer, QuestNum As Integer) As Boolean
        QuestInProgress = False
        If QuestNum < 1 OrElse QuestNum > MAX_QUESTS Then Exit Function

        If Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).Status = QuestStatusType.Started Then 'Status=1 means started
            QuestInProgress = True
        End If
    End Function

    Friend Function QuestCompleted(index as integer, QuestNum As Integer) As Boolean
        QuestCompleted = False
        If QuestNum < 1 OrElse QuestNum > MAX_QUESTS Then Exit Function

        If Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).Status = 2 OrElse Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).Status = 3 Then
            QuestCompleted = True
        End If
    End Function

    'Gets the quest reference num (id) from the quest name (it shall be unique)
    Friend Function GetQuestNum(QuestName As String) As Integer
        Dim I As Integer
        GetQuestNum = 0

        For I = 1 To MAX_QUESTS
            If Trim$(Quest(I).Name) = Trim$(QuestName) Then
                GetQuestNum = I
                Exit For
            End If
        Next
    End Function

    Friend Function GetItemNum(ItemName As String) As Integer
        Dim I As Integer
        GetItemNum = 0

        For I = 1 To MAX_ITEMS
            If Trim$(Item(I).Name) = Trim$(ItemName) Then
                GetItemNum = I
                Exit For
            End If
        Next
    End Function

    ' /////////////////////
    ' // General Purpose //
    ' /////////////////////

    Friend Sub CheckTasks(index as integer, TaskType As Integer, Targetindex as integer)
        Dim I As Integer

        For I = 1 To MAX_QUESTS
            If QuestInProgress(Index, I) Then
                CheckTask(Index, I, TaskType, TargetIndex)
            End If
        Next
    End Sub

    Friend Sub CheckTask(index as integer, QuestNum As Integer, TaskType As Integer, Targetindex as integer)
        Dim ActualTask As Integer, I As Integer
        ActualTask = Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).ActualTask

        If ActualTask >= Quest(QuestNum).Task.Length Then Exit Sub

        Select Case TaskType
            Case QuestType.Slay 'defeat X amount of X npc's.

                'is npc defeated id same as the npc i have to defeat?
                If TargetIndex = Quest(QuestNum).Task(ActualTask).NPC Then
                    'Count +1
                    Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).CurrentCount = Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).CurrentCount + 1
                    'did i finish the work?
                    If Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).CurrentCount >= Quest(QuestNum).Task(ActualTask).Amount Then
                        QuestMessage(Index, QuestNum, Trim$(Quest(QuestNum).Task(ActualTask).TaskLog) & " - Task completed", 0)
                        'is the quest's end?
                        If CanEndQuest(Index, QuestNum) Then
                            EndQuest(Index, QuestNum)
                        Else
                            'otherwise continue to the next task
                            Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).ActualTask = ActualTask + 1
                        End If
                    End If
                End If

            Case QuestType.Collect 'Gather X amount of X item.
                If TargetIndex = Quest(QuestNum).Task(ActualTask).Item Then
                    Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).CurrentCount = Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).CurrentCount + 1
                    If Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).CurrentCount >= Quest(QuestNum).Task(ActualTask).Amount Then
                        QuestMessage(Index, QuestNum, Trim$(Quest(QuestNum).Task(ActualTask).TaskLog) & " - Task completed", 0)
                        If CanEndQuest(Index, QuestNum) Then
                            EndQuest(Index, QuestNum)
                        Else
                            Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).ActualTask = ActualTask + 1
                        End If
                    End If
                End If

            Case QuestType.Talk 'Interact with X npc.
                If TargetIndex = Quest(QuestNum).Task(ActualTask).NPC AndAlso Quest(QuestNum).Task(ActualTask).Amount = 0 Then
                    QuestMessage(Index, QuestNum, Quest(QuestNum).Task(ActualTask).Speech, 0)
                    If CanEndQuest(Index, QuestNum) Then
                        EndQuest(Index, QuestNum)
                    Else
                        Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).ActualTask = ActualTask + 1
                    End If
                End If

            Case QuestType.Reach 'Reach X map.
                If TargetIndex = Quest(QuestNum).Task(ActualTask).Map Then
                    QuestMessage(Index, QuestNum, Trim$(Quest(QuestNum).Task(ActualTask).TaskLog) & " - Task completed", 0)
                    If CanEndQuest(Index, QuestNum) Then
                        EndQuest(Index, QuestNum)
                    Else
                        Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).ActualTask = ActualTask + 1
                    End If
                End If

            Case QuestType.Give 'Give X amount of X item to X npc.
                If TargetIndex = Quest(QuestNum).Task(ActualTask).NPC Then
                    For I = 1 To MAX_INV
                        If GetPlayerInvItemNum(Index, I) = Quest(QuestNum).Task(ActualTask).Item Then
                            If GetPlayerInvItemValue(Index, I) >= Quest(QuestNum).Task(ActualTask).Amount Then
                                TakeInvItem(Index, GetPlayerInvItemNum(Index, I), Quest(QuestNum).Task(ActualTask).Amount)
                                QuestMessage(Index, QuestNum, Quest(QuestNum).Task(ActualTask).Speech, 0)
                                If CanEndQuest(Index, QuestNum) Then
                                    EndQuest(Index, QuestNum)
                                Else
                                    Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).ActualTask = ActualTask + 1
                                End If
                                Exit For
                            End If
                        End If
                    Next
                End If

            Case QuestType.Kill 'Kill X amount of players.
                Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).CurrentCount = Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).CurrentCount + 1
                If Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).CurrentCount >= Quest(QuestNum).Task(ActualTask).Amount Then
                    QuestMessage(Index, QuestNum, Trim$(Quest(QuestNum).Task(ActualTask).TaskLog) & " - Task completed", 0)
                    If CanEndQuest(Index, QuestNum) Then
                        EndQuest(Index, QuestNum)
                    Else
                        Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).ActualTask = ActualTask + 1
                    End If
                End If

            Case QuestType.Gather 'Hit X amount of times X resource.
                If TargetIndex = Quest(QuestNum).Task(ActualTask).Resource Then
                    Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).CurrentCount = Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).CurrentCount + 1
                    If Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).CurrentCount >= Quest(QuestNum).Task(ActualTask).Amount Then
                        QuestMessage(Index, QuestNum, Trim$(Quest(QuestNum).Task(ActualTask).TaskLog) & " - Task completed", 0)
                        If CanEndQuest(Index, QuestNum) Then
                            EndQuest(Index, QuestNum)
                        Else
                            Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).ActualTask = ActualTask + 1
                        End If
                    End If
                End If

            Case QuestType.Fetch 'Get X amount of X item from X npc.
                If TargetIndex = Quest(QuestNum).Task(ActualTask).NPC Then
                    GiveInvItem(Index, Quest(QuestNum).Task(ActualTask).Item, Quest(QuestNum).Task(ActualTask).Amount)
                    QuestMessage(Index, QuestNum, Quest(QuestNum).Task(ActualTask).Speech, 0)
                    If CanEndQuest(Index, QuestNum) Then
                        EndQuest(Index, QuestNum)
                    Else
                        Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).ActualTask = ActualTask + 1
                    End If
                End If
        End Select

        SendPlayerQuest(Index, QuestNum)
    End Sub

    Friend Sub ShowQuest(index as integer, QuestNum As Integer)
        If QuestInProgress(Index, QuestNum) Then
            QuestMessage(Index, QuestNum, Trim$(Quest(QuestNum).Chat(2)), 0) 'show meanwhile message
            Exit Sub
        End If
        If Not CanStartQuest(Index, QuestNum) Then Exit Sub

        QuestMessage(Index, QuestNum, Trim$(Quest(QuestNum).Chat(1)), QuestNum) 'chat 1 = request message
    End Sub

    Friend Sub EndQuest(index as integer, QuestNum As Integer)
        Dim I As Integer

        QuestMessage(Index, QuestNum, Trim$(Quest(QuestNum).Chat(3)), 0)

        For I = 1 To Quest(QuestNum).RewardCount
            If Quest(QuestNum).RewardItem(I) > 0 Then
                PlayerMsg(Index, "You recieved " & Quest(QuestNum).RewardItemAmount(I) & " " & Trim(Item(Quest(QuestNum).RewardItem(I)).Name), ColorType.BrightGreen)
            End If
            GiveInvItem(Index, Quest(QuestNum).RewardItem(I), Quest(QuestNum).RewardItemAmount(I))
        Next

        If Quest(QuestNum).RewardExp > 0 Then
            SetPlayerExp(Index, GetPlayerExp(Index) + Quest(QuestNum).RewardExp)
            SendExp(Index)
            ' Check for level up
            CheckPlayerLevelUp(Index)
        End If

        'Check if quest is repeatable, set it as completed
        If Quest(QuestNum).Repeat = True Then
            Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).Status = QuestStatusType.Repeatable
        Else
            Player(Index).Character(TempPlayer(Index).CurChar).PlayerQuest(QuestNum).Status = QuestStatusType.Completed
        End If
        PlayerMsg(Index, Trim$(Quest(QuestNum).Name) & ": quest completed", ColorType.BrightGreen)

        SavePlayer(Index)
        SendPlayerData(Index)
        SendPlayerQuest(Index, QuestNum)
    End Sub
#End Region

End Module