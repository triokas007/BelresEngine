Imports ASFW

Friend Module modTime
    Sub InitTime()
        ' Add handlers to time events
        AddHandler Time.Instance.OnTimeChanged, AddressOf HandleTimeChanged
        AddHandler Time.Instance.OnTimeOfDayChanged, AddressOf HandleTimeOfDayChanged
        AddHandler Time.Instance.OnTimeSync, AddressOf HandleTimeSync

        ' Prepare the time instance
        Time.Instance.Time = Date.Now
        Time.Instance.GameSpeed = 1
    End Sub

    Sub HandleTimeChanged(ByRef source As Time)
        UpdateCaption()
    End Sub

    Sub HandleTimeOfDayChanged(ByRef source As Time)
        SendTimeToAll()
        ClearAllMapNpcs()
        SpawnAllMapNpcs()
    End Sub

    Sub HandleTimeSync(ByRef source As Time)
        SendGameClockToAll()
    End Sub

    Sub SendGameClockTo(index as integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.SClock)
        Buffer.WriteInt32(Time.Instance.GameSpeed)
        Buffer.WriteBytes(BitConverter.GetBytes(Time.Instance.Time.Ticks))
        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Addlog("Sent SMSG: SClock", PACKET_LOG)
        Console.WriteLine("Sent SMSG: SClock")

        Addlog(" Player: " & GetPlayerName(Index) & " : " & " GameSpeed: " & Time.Instance.GameSpeed & " Instance Time Ticks: " & Time.Instance.Time.Ticks, PLAYER_LOG)
        Console.WriteLine(" Player: " & GetPlayerName(Index) & " : " & " GameSpeed: " & Time.Instance.GameSpeed & " Instance Time Ticks: " & Time.Instance.Time.Ticks)

        Buffer.Dispose()
    End Sub

    Sub SendGameClockToAll()
        Dim I As Integer

        For I = 1 To GetPlayersOnline()
            If IsPlaying(I) Then
                SendGameClockTo(I)
            End If
        Next
    End Sub

    Sub SendTimeTo(index as integer)
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ServerPackets.STime)
        Buffer.WriteByte(Time.Instance.TimeOfDay)
        Socket.SendDataTo(Index, Buffer.Data, Buffer.Head)

        Addlog("Sent SMSG: STime", PACKET_LOG)
        Console.WriteLine("Sent SMSG: STime")

        Addlog(" Player: " & GetPlayerName(Index) & " : " & " Time Of Day: " & Time.Instance.TimeOfDay, PLAYER_LOG)
        Console.WriteLine(" Player: " & GetPlayerName(Index) & " : " & " Time Of Day: " & Time.Instance.TimeOfDay)

        Buffer.Dispose()
    End Sub

    Sub SendTimeToAll()
        Dim I As Integer

        For I = 1 To GetPlayersOnline()
            If IsPlaying(I) Then
                SendTimeTo(I)
            End If
        Next

    End Sub
End Module
