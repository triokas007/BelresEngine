﻿Imports System.IO
Imports SFML.Audio

Module modSound
    'Music + Sound Players
    Friend SoundPlayer As Sound
    Friend ExtraSoundPlayer As Sound
    Friend MusicPlayer As Music
    Friend PreviewPlayer As Music
    Friend MusicCache(100) As String
    Friend SoundCache(100) As String

    Friend FadeInSwitch As Boolean
    Friend FadeOutSwitch As Boolean
    Friend CurMusic As String
    Friend MaxVolume As Single

    Sub PlayMusic(FileName As String)
        If Not Options.Music = 1 OrElse Not File.Exists(Application.StartupPath & MUSIC_PATH & FileName) Then Exit Sub
        If FileName = CurMusic Then Exit Sub

        If MusicPlayer Is Nothing Then
            Try
                MusicPlayer = New Music(Application.StartupPath & MUSIC_PATH & FileName)
                MusicPlayer.Loop() = True
                MusicPlayer.Volume() = 0
                MusicPlayer.Play()
                CurMusic = FileName
                FadeInSwitch = True
            Catch ex As Exception

            End Try
        Else
            Try
                CurMusic = FileName
                FadeOutSwitch = True

            Catch ex As Exception

            End Try
        End If
    End Sub

    Sub StopMusic()
        If MusicPlayer Is Nothing Then Exit Sub
        MusicPlayer.Stop()
        MusicPlayer.Dispose()
        MusicPlayer = Nothing
        CurMusic = ""
    End Sub

    Sub PlayPreview(FileName As String)
        If Not Options.Music = 1 OrElse Not File.Exists(Application.StartupPath & MUSIC_PATH & FileName) Then Exit Sub

        If PreviewPlayer Is Nothing Then
            Try
                PreviewPlayer = New Music(Application.StartupPath & MUSIC_PATH & FileName)
                PreviewPlayer.Loop() = True
                PreviewPlayer.Volume() = 75
                PreviewPlayer.Play()

            Catch ex As Exception

            End Try
        Else
            Try
                StopPreview()
                PreviewPlayer = New Music(Application.StartupPath & MUSIC_PATH & FileName)
                PreviewPlayer.Loop() = True
                PreviewPlayer.Volume() = 75
                PreviewPlayer.Play()
            Catch ex As Exception

            End Try
        End If
    End Sub

    Sub StopPreview()
        If PreviewPlayer Is Nothing Then Exit Sub
        PreviewPlayer.Stop()
        PreviewPlayer.Dispose()
        PreviewPlayer = Nothing
    End Sub

    Sub PlaySound(FileName As String, Optional Looped As Boolean = False)
        If Not Options.Sound = 1 OrElse Not File.Exists(Application.StartupPath & SOUND_PATH & FileName) Then Exit Sub

        dim buffer as SoundBuffer
        If SoundPlayer Is Nothing Then
            SoundPlayer = New Sound()
            buffer = New SoundBuffer(Application.StartupPath & SOUND_PATH & FileName)
            SoundPlayer.SoundBuffer = buffer
            If Looped = True Then
                SoundPlayer.Loop() = True
            Else
                SoundPlayer.Loop() = False
            End If
            SoundPlayer.Volume() = MaxVolume
            SoundPlayer.Play()
        Else
            SoundPlayer.Stop()
            buffer = New SoundBuffer(Application.StartupPath & SOUND_PATH & FileName)
            SoundPlayer.SoundBuffer = buffer
            If Looped = True Then
                SoundPlayer.Loop() = True
            Else
                SoundPlayer.Loop() = False
            End If
            SoundPlayer.Volume() = MaxVolume
            SoundPlayer.Play()
        End If
    End Sub

    Sub StopSound()
        If SoundPlayer Is Nothing Then Exit Sub
        SoundPlayer.Dispose()
        SoundPlayer = Nothing
    End Sub

    Sub PlayExtraSound(FileName As String, Optional Looped As Boolean = False)
        If Not Options.Sound = 1 OrElse Not File.Exists(Application.StartupPath & SOUND_PATH & FileName) Then Exit Sub
        'If FileName = CurExtraSound Then Exit Sub

        dim buffer as SoundBuffer
        If ExtraSoundPlayer Is Nothing Then
            ExtraSoundPlayer = New Sound()
            buffer = New SoundBuffer(Application.StartupPath & SOUND_PATH & FileName)
            ExtraSoundPlayer.SoundBuffer = buffer
            If Looped = True Then
                ExtraSoundPlayer.Loop() = True
            Else
                ExtraSoundPlayer.Loop() = False
            End If
            ExtraSoundPlayer.Volume() = MaxVolume
            ExtraSoundPlayer.Play()
        Else
            ExtraSoundPlayer.Stop()
            buffer = New SoundBuffer(Application.StartupPath & SOUND_PATH & FileName)
            ExtraSoundPlayer.SoundBuffer = buffer
            If Looped = True Then
                ExtraSoundPlayer.Loop() = True
            Else
                ExtraSoundPlayer.Loop() = False
            End If
            ExtraSoundPlayer.Volume() = MaxVolume
            ExtraSoundPlayer.Play()
        End If
    End Sub

    Sub StopExtraSound()
        If ExtraSoundPlayer Is Nothing Then Exit Sub
        ExtraSoundPlayer.Dispose()
        ExtraSoundPlayer = Nothing
    End Sub

    Sub FadeIn()

        If MusicPlayer Is Nothing Then Exit Sub

        If MusicPlayer.Volume() >= MaxVolume Then FadeInSwitch = False
        MusicPlayer.Volume() = MusicPlayer.Volume() + 3

    End Sub

    Sub FadeOut()
        Dim tmpmusic As String
        If MusicPlayer Is Nothing Then Exit Sub

        If MusicPlayer.Volume() = 0 OrElse MusicPlayer.Volume() < 3 Then
            FadeOutSwitch = False
            If CurMusic = "" Then
                StopMusic()
            Else
                tmpmusic = CurMusic
                StopMusic()
                PlayMusic(tmpmusic)
            End If
        End If
        If MusicPlayer Is Nothing Then Exit Sub

        MusicPlayer.Volume() = MusicPlayer.Volume() - 3

    End Sub
End Module