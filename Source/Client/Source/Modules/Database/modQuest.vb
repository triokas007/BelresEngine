Imports ASFW

Friend Module ModQuest
#Region "Global Info"
    'Constants
    Friend Const MaxQuests As Byte = 250
    'Friend Const MAX_TASKS As Byte = 10
    'Friend Const MAX_REQUIREMENTS As Byte = 10
    Friend Const EditorTasks As Byte = 7

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

    Friend QuestLogPage As Integer
    Friend QuestNames(MaxActivequests) As String

    Friend QuestChanged(MaxQuests) As Boolean

    Friend QuestEditorShow As Boolean

    'questlog variables

    Friend Const MaxActivequests = 10

    Friend QuestLogX As Integer = 150
    Friend QuestLogY As Integer = 100

    Friend PnlQuestLogVisible As Boolean
    Friend SelectedQuest As Integer
    Friend QuestTaskLogText As String = ""
    Friend ActualTaskText As String = ""
    Friend QuestDialogText As String = ""
    Friend QuestStatus2Text As String = ""
    Friend AbandonQuestText As String = ""
    Friend AbandonQuestVisible As Boolean
    Friend QuestRequirementsText As String = ""
    Friend QuestRewardsText() As String

    'here we store temp info because off UpdateUI >.<
    Friend UpdateQuestWindow As Boolean
    Friend UpdateQuestChat As Boolean
    Friend QuestNum As Integer
    Friend QuestNumForStart As Integer
    Friend QuestMessage As String
    Friend QuestAcceptTag As Integer

    'Types
    Friend Quest(MaxQuests) As QuestRec

    Friend Structure PlayerQuestRec
        Dim Status As Integer '0=not started, 1=started, 2=completed, 3=completed but repeatable
        Dim ActualTask As Integer
        Dim CurrentCount As Integer 'Used to handle the Amount property
    End Structure

    Friend Structure TaskRec
        Dim Order As Integer
        Dim Npc As Integer
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

#Region "DataBase"
    Sub ClearQuest(questNum As Integer)
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
            Quest(QuestNum).Requirement(I) = 0
            Quest(QuestNum).RequirementIndex(I) = 0
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
            Quest(QuestNum).Task(I).Npc = 0
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

        ReDim Quest(MaxQuests)

        For I = 1 To MaxQuests
            ClearQuest(I)
        Next
    End Sub
#End Region

#Region "Incoming Packets"
    Friend Sub Packet_QuestEditor(ByRef data() As Byte)
        QuestEditorShow = True
    End Sub

    Friend Sub Packet_UpdateQuest(ByRef data() As Byte)
        Dim questNum As Integer
        dim buffer as New ByteStream(Data)
        QuestNum = Buffer.ReadInt32

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
            Quest(QuestNum).Task(I).Npc = Buffer.ReadInt32
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
    End Sub

    Friend Sub Packet_PlayerQuest(ByRef data() As Byte)
        Dim questNum As Integer
        dim buffer as New ByteStream(Data)
        QuestNum = Buffer.ReadInt32

        Player(MyIndex).PlayerQuest(QuestNum).Status = Buffer.ReadInt32
        Player(MyIndex).PlayerQuest(QuestNum).ActualTask = Buffer.ReadInt32
        Player(MyIndex).PlayerQuest(QuestNum).CurrentCount = Buffer.ReadInt32

        RefreshQuestLog()

        Buffer.Dispose()
    End Sub

    Friend Sub Packet_PlayerQuests(ByRef data() As Byte)
        Dim I As Integer
        dim buffer as New ByteStream(Data)
        For I = 1 To MaxQuests
            Player(MyIndex).PlayerQuest(I).Status = Buffer.ReadInt32
            Player(MyIndex).PlayerQuest(I).ActualTask = Buffer.ReadInt32
            Player(MyIndex).PlayerQuest(I).CurrentCount = Buffer.ReadInt32
        Next

        RefreshQuestLog()

        Buffer.Dispose()
    End Sub

    Friend Sub Packet_QuestMessage(ByRef data() As Byte)
        dim buffer as New ByteStream(Data)
        QuestNum = Buffer.ReadInt32
        QuestMessage = Trim$(Buffer.ReadString)
        QuestMessage = QuestMessage.Replace("$playername$", GetPlayerName(MyIndex))
        QuestNumForStart = Buffer.ReadInt32

        UpdateQuestChat = True

        Buffer.Dispose()
    End Sub
#End Region

#Region "Outgoing Packets"

    Sub SendRequestQuests()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestQuests)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Friend Sub UpdateQuestLog()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CQuestLogUpdate)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub

    Friend Sub PlayerHandleQuest(questNum As Integer, order As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CPlayerHandleQuest)
        Buffer.WriteInt32(QuestNum)
        Buffer.WriteInt32(Order) '1=accept quest, 2=cancel quest

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Friend Sub QuestReset(questNum As Integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CQuestReset)
        Buffer.WriteInt32(QuestNum)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()

    End Sub
#End Region

#Region "Support Functions"
    'Tells if the quest is in progress or not
    Friend Function QuestInProgress(questNum As Integer) As Boolean
        QuestInProgress = False
        If QuestNum < 1 OrElse QuestNum > MaxQuests Then Exit Function

        If Player(MyIndex).PlayerQuest(QuestNum).Status = QuestStatusType.Started Then 'Status=1 means started
            QuestInProgress = True
        End If
    End Function

    Friend Function QuestCompleted(questNum As Integer) As Boolean
        QuestCompleted = False
        If QuestNum < 1 OrElse QuestNum > MaxQuests Then Exit Function

        If Player(MyIndex).PlayerQuest(QuestNum).Status = QuestStatusType.Completed OrElse Player(MyIndex).PlayerQuest(QuestNum).Status = QuestStatusType.Repeatable Then
            QuestCompleted = True
        End If
    End Function

    Friend Function GetQuestNum(questName As String) As Integer
        Dim I As Integer
        GetQuestNum = 0

        For I = 1 To MaxQuests
            If Trim$(Quest(I).Name) = Trim$(QuestName) Then
                GetQuestNum = I
                Exit For
            End If
        Next
    End Function
#End Region

#Region "Misc Functions"
    Friend Function CanStartQuest(questNum As Integer) As Boolean
        Dim i As Integer

        CanStartQuest = False

        If QuestNum < 1 OrElse QuestNum > MaxQuests Then Exit Function
        If QuestInProgress(QuestNum) Then Exit Function

        'Check if player has the quest 0 (not started) or 3 (completed but it can be started again)
        If Player(MyIndex).PlayerQuest(QuestNum).Status = QuestStatusType.NotStarted OrElse Player(MyIndex).PlayerQuest(QuestNum).Status = QuestStatusType.Repeatable Then

            For i = 1 To Quest(QuestNum).ReqCount
                'Check if item is needed
                If Quest(QuestNum).Requirement(i) = 1 Then
                    If Quest(QuestNum).RequirementIndex(i) > 0 AndAlso Quest(QuestNum).RequirementIndex(i) <= MAX_ITEMS Then
                        If HasItem(MyIndex, Quest(QuestNum).RequirementIndex(i)) = 0 Then
                            Exit Function
                        End If
                    End If
                End If

                'Check if previous quest is needed
                If Quest(QuestNum).Requirement(i) = 2 Then
                    If Quest(QuestNum).RequirementIndex(i) > 0 AndAlso Quest(QuestNum).RequirementIndex(i) <= MaxQuests Then
                        If Player(MyIndex).PlayerQuest(Quest(QuestNum).RequirementIndex(i)).Status = QuestStatusType.NotStarted OrElse Player(MyIndex).PlayerQuest(Quest(QuestNum).RequirementIndex(i)).Status = QuestStatusType.Started Then
                            Exit Function
                        End If
                    End If
                End If

            Next

            'Go on :)
            CanStartQuest = True
        Else
            CanStartQuest = False
        End If
    End Function

    Friend Function CanEndQuest(index as integer, questNum As Integer) As Boolean
        CanEndQuest = False

        If Player(Index).PlayerQuest(QuestNum).ActualTask >= Quest(QuestNum).Task.Length Then
            CanEndQuest = True
        End If
        If Quest(QuestNum).Task(Player(Index).PlayerQuest(QuestNum).ActualTask).QuestEnd = 1 Then
            CanEndQuest = True
        End If
    End Function

    Function HasItem(index as integer, itemNum As Integer) As Integer
        Dim i As Integer

        ' Check for subscript out of range
        If IsPlaying(Index) = False OrElse itemNum <= 0 OrElse itemNum > MAX_ITEMS Then
            Exit Function
        End If

        For i = 1 To MAX_INV

            ' Check to see if the player has the item
            If GetPlayerInvItemNum(Index, i) = itemNum Then
                If Item(itemNum).Type = ItemType.Currency OrElse Item(itemNum).Stackable = 1 Then
                    HasItem = GetPlayerInvItemValue(Index, i)
                Else
                    HasItem = 1
                End If

                Exit Function
            End If

        Next

    End Function

    Friend Sub RefreshQuestLog()
        Dim I As Integer, x As Integer

        For I = 1 To MaxActivequests
            QuestNames(I) = ""
        Next

        x = 1

        For I = 1 To MaxQuests
            If QuestInProgress(I) AndAlso x < MaxActivequests Then
                QuestNames(x) = Trim$(Quest(I).Name)
                x = x + 1
            End If
        Next

    End Sub

    ' ////////////////////////
    ' // Visual Interaction //
    ' ////////////////////////

    Friend Sub LoadQuestlogBox()
        Dim questNum As Integer, curTask As Integer, I As Integer

        If SelectedQuest = 0 Then Exit Sub

        For I = 1 To MaxQuests
            If Trim$(QuestNames(SelectedQuest)) = Trim$(Quest(I).Name) Then
                QuestNum = I
            End If
        Next

        If QuestNum = 0 Then Exit Sub

        CurTask = Player(MyIndex).PlayerQuest(QuestNum).ActualTask

        If CurTask >= Quest(QuestNum).Task.Length Then Exit Sub

        'Quest Log (Main Task)
        QuestTaskLogText = Trim$(Quest(QuestNum).QuestLog)

        'Actual Task
        QuestTaskLogText = Trim$(Quest(QuestNum).Task(CurTask).TaskLog)

        'Last dialog
        If Player(MyIndex).PlayerQuest(QuestNum).ActualTask > 1 Then
            If Len(Trim$(Quest(QuestNum).Task(CurTask - 1).Speech)) > 0 Then
                QuestDialogText = Trim$(Quest(QuestNum).Task(CurTask - 1).Speech).Replace("$playername$", GetPlayerName(MyIndex))
            Else
                QuestDialogText = Trim$(Quest(QuestNum).Chat(1).Replace("$playername$", GetPlayerName(MyIndex)))
            End If
        Else
            QuestDialogText = Trim$(Quest(QuestNum).Chat(1).Replace("$playername$", GetPlayerName(MyIndex)))
        End If

        'Quest Status
        If Player(MyIndex).PlayerQuest(QuestNum).Status = QuestStatusType.Started Then
            QuestStatus2Text = Strings.Get("quests", "queststarted")
            AbandonQuestText = Strings.Get("quests", "questcancel")
            AbandonQuestVisible = True
        ElseIf Player(MyIndex).PlayerQuest(QuestNum).Status = QuestStatusType.Completed Then
            QuestStatus2Text = Strings.Get("quests", "questcomplete")
            AbandonQuestVisible = False
        Else
            QuestStatus2Text = "???"
            AbandonQuestVisible = False
        End If

        Select Case Quest(QuestNum).Task(CurTask).TaskType
                'defeat x amount of Npc
            Case QuestType.Slay
                Dim curCount As Integer = Player(MyIndex).PlayerQuest(QuestNum).CurrentCount
                Dim maxAmount As Integer = Quest(QuestNum).Task(CurTask).Amount
                Dim npcName As String = Npc(Quest(QuestNum).Task(CurTask).Npc).Name
                ActualTaskText = Strings.Get("quests", "questgoslay", CurCount, MaxAmount, NpcName)'"Defeat " & CurCount & "/" & MaxAmount & " " & NpcName
                'gather x amount of items
            Case QuestType.Collect
                Dim curCount As Integer = Player(MyIndex).PlayerQuest(QuestNum).CurrentCount
                Dim maxAmount As Integer = Quest(QuestNum).Task(CurTask).Amount
                Dim itemName As String = Item(Quest(QuestNum).Task(CurTask).Item).Name
                ActualTaskText = Strings.Get("quests", "questgocollect", CurCount, MaxAmount, ItemName)'"Collect " & CurCount & "/" & MaxAmount & " " & ItemName
                'go talk to npc
            Case QuestType.Talk
                Dim npcName As String = Npc(Quest(QuestNum).Task(CurTask).Npc).Name
                ActualTaskText = Strings.Get("quests", "questtalkto", NpcName)'"Go talk to  " & NpcName
                'reach certain map
            Case QuestType.Reach
                Dim mapName As String = MapNames(Quest(QuestNum).Task(CurTask).Map)
                ActualTaskText = Strings.Get("quests", "questgoto", MapName)'"Go to " & MapName
            Case QuestType.Give
                'give x amount of items to npc
                Dim npcName As String = Npc(Quest(QuestNum).Task(CurTask).Npc).Name
                Dim curCount As Integer = Player(MyIndex).PlayerQuest(QuestNum).CurrentCount
                Dim maxAmount As Integer = Quest(QuestNum).Task(CurTask).Amount
                Dim itemName As String = Item(Quest(QuestNum).Task(CurTask).Item).Name
                ActualTaskText = Strings.Get("quests", "questgive", NpcName, ItemName, CurCount, MaxAmount)'"Give " & NpcName & " the " & ItemName & CurCount & "/" & MaxAmount & " they requested"
                'defeat certain amount of players
            Case QuestType.Kill
                Dim curCount As Integer = Player(MyIndex).PlayerQuest(QuestNum).CurrentCount
                Dim maxAmount As Integer = Quest(QuestNum).Task(CurTask).Amount
                ActualTaskText = Strings.Get("quests", "questkill", CurCount, MaxAmount)'"Defeat " & CurCount & "/" & MaxAmount & " Players in Battle"
                'go collect resources
            Case QuestType.Gather
                Dim curCount As Integer = Player(MyIndex).PlayerQuest(QuestNum).CurrentCount
                Dim maxAmount As Integer = Quest(QuestNum).Task(CurTask).Amount
                Dim resourceName As String = Resource(Quest(QuestNum).Task(CurTask).Resource).Name
                ActualTaskText = Strings.Get("quests", "questgather", CurCount, MaxAmount, ResourceName)'"Gather " & CurCount & "/" & MaxAmount & " " & ResourceName
                'fetch x amount of items from npc
            Case QuestType.Fetch
                Dim npcName As String = Item(Quest(QuestNum).Task(CurTask).Npc).Name
                Dim maxAmount As Integer = Quest(QuestNum).Task(CurTask).Amount
                Dim itemName As String = Item(Quest(QuestNum).Task(CurTask).Item).Name
                ActualTaskText = Strings.Get("quests", "questfetch", ItemName, MaxAmount, NpcName) '"Fetch " & ItemName & "X" & MaxAmount & " from " & NpcName
            Case Else
                'ToDo
                ActualTaskText = "errr..."
        End Select

        'Rewards
        ReDim QuestRewardsText(Quest(QuestNum).RewardCount + 1)
        For I = 1 To Quest(QuestNum).RewardCount
            QuestRewardsText(I) = Item(Quest(QuestNum).RewardItem(I)).Name & " X" & Str(Quest(QuestNum).RewardItemAmount(I))
        Next
        QuestRewardsText(I) = Str(Quest(QuestNum).RewardExp) & " EXP"
    End Sub

    Friend Sub DrawQuestLog()
        Dim i As Integer, y As Integer

        y = 10

        'first render panel
        RenderSprite(QuestSprite, GameWindow, QuestLogX, QuestLogY, 0, 0, QuestGFXInfo.Width, QuestGFXInfo.Height)

        'draw quest names
        For i = 1 To MaxActivequests
            If Len(Trim$(QuestNames(i))) > 0 Then
                DrawText(QuestLogX + 7, QuestLogY + y, Trim$(QuestNames(i)), SFML.Graphics.Color.White, SFML.Graphics.Color.Black, GameWindow)
                y = y + 20
            End If
        Next

        If SelectedQuest <= 0 Then Exit Sub

        'quest log text
        y = 0
        For Each str As String In WordWrap(Trim$(QuestTaskLogText), 35, WrapMode.Characters, WrapType.BreakWord)
            'description
            DrawText(QuestLogX + 204, QuestLogY + 30 + y, str, SFML.Graphics.Color.White, SFML.Graphics.Color.Black, GameWindow)
            y = y + 15
        Next

        y = 0
        For Each str As String In WordWrap(Trim$(ActualTaskText), 40, WrapMode.Characters, WrapType.BreakWord)
            'description
            DrawText(QuestLogX + 204, QuestLogY + 147 + y, str, SFML.Graphics.Color.White, SFML.Graphics.Color.Black, GameWindow)
            y = y + 15
        Next

        y = 0
        For Each str As String In WordWrap(Trim$(QuestDialogText), 40, WrapMode.Characters, WrapType.BreakWord)
            'description
            DrawText(QuestLogX + 204, QuestLogY + 218 + y, str, SFML.Graphics.Color.White, SFML.Graphics.Color.Black, GameWindow)
            y = y + 15
        Next
        DrawText(QuestLogX + 280, QuestLogY + 263, Trim$(QuestStatus2Text), SFML.Graphics.Color.White, SFML.Graphics.Color.Black, GameWindow)

        'DrawText(QuestLogX + 285, QuestLogY + 288, Trim$(QuestRequirementsText), SFML.Graphics.Color.White, SFML.Graphics.Color.Black, GameWindow)

        y = 0
        For i = 1 To QuestRewardsText.Length - 1
            'description
            DrawText(QuestLogX + 255, QuestLogY + 292 + y, Trim$(QuestRewardsText(i)), SFML.Graphics.Color.White, SFML.Graphics.Color.Black, GameWindow)
            y = y + 15
        Next

    End Sub

    Friend Sub ResetQuestLog()

        QuestTaskLogText = ""
        ActualTaskText = ""
        QuestDialogText = ""
        QuestStatus2Text = ""
        AbandonQuestText = ""
        AbandonQuestVisible = False
        QuestRequirementsText = ""
        ReDim QuestRewardsText(0)
        pnlQuestLogVisible = False

        SelectedQuest = 0
    End Sub
#End Region

End Module