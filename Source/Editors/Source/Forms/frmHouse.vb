Friend Class frmHouse

    Private Sub LstIndex_Click(sender As Object, e As EventArgs) Handles lstIndex.Click
        HouseEditorInit()
    End Sub

    Private Sub TxtName_TextChanged(sender As Object, e As EventArgs) Handles txtName.TextChanged
        Dim tmpindex as integer

        If EditorIndex <= 0 Then Exit Sub

        tmpIndex = lstIndex.SelectedIndex
        House(EditorIndex).ConfigName = Trim$(txtName.Text)
        lstIndex.Items.RemoveAt(EditorIndex - 1)
        lstIndex.Items.Insert(EditorIndex - 1, EditorIndex & ": " & House(EditorIndex).ConfigName)
        lstIndex.SelectedIndex = tmpIndex

    End Sub

    Private Sub NudBaseMap_ValueChanged(sender As Object, e As EventArgs) Handles nudBaseMap.Click
        If EditorIndex <= 0 Then Exit Sub

        If nudBaseMap.Value < 1 OrElse nudBaseMap.Value > MAX_MAPS Then Exit Sub
        House(EditorIndex).BaseMap = nudBaseMap.Value
    End Sub

    Private Sub NudX_ValueChanged(sender As Object, e As EventArgs) Handles nudX.Click
        If EditorIndex <= 0 Then Exit Sub

        If nudX.Value < 0 OrElse nudX.Value > 255 Then Exit Sub
        House(EditorIndex).X = nudX.Value

    End Sub

    Private Sub NudY_ValueChanged(sender As Object, e As EventArgs) Handles nudY.Click
        If EditorIndex <= 0 Then Exit Sub

        If nudY.Value < 0 OrElse nudY.Value > 255 Then Exit Sub
        House(EditorIndex).Y = nudY.Value

    End Sub

    Private Sub NudPrice_ValueChanged(sender As Object, e As EventArgs) Handles nudPrice.Click
        If EditorIndex <= 0 Then Exit Sub

        House(EditorIndex).Price = nudPrice.Value

    End Sub

    Private Sub NudFurniture_ValueChanged(sender As Object, e As EventArgs) Handles nudFurniture.Click
        If EditorIndex <= 0 Then Exit Sub

        If nudFurniture.Value < 0 Then Exit Sub
        House(EditorIndex).MaxFurniture = nudFurniture.Value
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If Len(Trim$(txtName.Text)) = 0 Then
            MsgBox("Name required.")
            Exit Sub
        End If

        If nudBaseMap.Value = 0 Then
            MsgBox("Base map required.")
            Exit Sub
        End If

        HouseEditorOk()
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        HouseEditorCancel()
    End Sub
End Class