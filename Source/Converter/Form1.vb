Public Class Form1
    Sub TextAdd(ByVal Msg As String)
        If txtLog.Text = "" Then
            txtLog.Text = txtLog.Text & Msg
        Else
            txtLog.Text = txtLog.Text & vbNewLine & Msg
        End If
    End Sub

    Private Sub BtnMap_Click(sender As Object, e As EventArgs) Handles btnMap.Click
        txtLog.Clear()
        TextAdd("Converting Maps...")
        ConvertMaps()
        TextAdd("Done!")
    End Sub

    Private Sub BtnNpc_Click(sender As Object, e As EventArgs) Handles btnNpc.Click
        txtLog.Clear()
        TextAdd("Converting Npcs...")
        ConvertNpcs()
        TextAdd("Done!")
    End Sub

    Private Sub BtnItems_Click(sender As Object, e As EventArgs) Handles btnItems.Click
        txtLog.Clear()
        TextAdd("Converting Items...")
        ConvertItems()
        TextAdd("Done!")
    End Sub
End Class