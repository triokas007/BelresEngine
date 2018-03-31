Friend Class frmResource

    Private Sub ScrlNormalPic_Scroll(sender As Object, e As EventArgs) Handles nudNormalPic.ValueChanged
        EditorResource_DrawSprite()
        Resource(EditorIndex).ResourceImage = nudNormalPic.Value
    End Sub

    Private Sub CmbType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbType.SelectedIndexChanged
        Resource(EditorIndex).ResourceType = cmbType.SelectedIndex
    End Sub

    Private Sub ScrlExhaustedPic_Scroll(sender As Object, e As EventArgs) Handles nudExhaustedPic.ValueChanged
        EditorResource_DrawSprite()
        Resource(EditorIndex).ExhaustedImage = nudExhaustedPic.Value
    End Sub

    Private Sub ScrlRewardItem_Scroll(sender As Object, e As EventArgs) Handles cmbRewardItem.SelectedIndexChanged
        Resource(EditorIndex).ItemReward = cmbRewardItem.SelectedIndex
    End Sub

    Private Sub ScrlRewardExp_Scroll(sender As Object, e As EventArgs) Handles nudRewardExp.ValueChanged
        Resource(EditorIndex).ExpReward = nudRewardExp.Value
    End Sub

    Private Sub ScrlLvlReq_Scroll(sender As Object, e As EventArgs) Handles nudLvlReq.ValueChanged
        Resource(EditorIndex).LvlRequired = nudLvlReq.Value
    End Sub

    Private Sub CmbTool_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbTool.SelectedIndexChanged

        Resource(EditorIndex).ToolRequired = cmbTool.SelectedIndex
    End Sub

    Private Sub ScrlHealth_Scroll(sender As Object, e As EventArgs) Handles nudHealth.ValueChanged
        Resource(EditorIndex).Health = nudHealth.Value
    End Sub

    Private Sub ScrlRespawn_Scroll(sender As Object, e As EventArgs) Handles nudRespawn.ValueChanged
        Resource(EditorIndex).RespawnTime = nudRespawn.Value
    End Sub

    Private Sub ScrlAnim_Scroll(sender As Object, e As EventArgs) Handles cmbAnimation.SelectedIndexChanged
        Resource(EditorIndex).Animation = cmbAnimation.SelectedIndex
    End Sub

    Private Sub LstIndex_Click(sender As Object, e As EventArgs) Handles lstIndex.Click
        ResourceEditorInit()
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        ResourceEditorOk()
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim tmpindex as integer

        ClearResource(EditorIndex)

        tmpIndex = lstIndex.SelectedIndex
        lstIndex.Items.RemoveAt(EditorIndex - 1)
        lstIndex.Items.Insert(EditorIndex - 1, EditorIndex & ": " & Resource(EditorIndex).Name)
        lstIndex.SelectedIndex = tmpIndex

        ResourceEditorInit()
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Call ResourceEditorCancel()
    End Sub

    Private Sub FrmEditor_Resource_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub TxtName_TextChanged(sender As Object, e As EventArgs) Handles txtName.TextChanged
        Dim tmpindex as integer

        If EditorIndex = 0 Then Exit Sub
        tmpIndex = lstIndex.SelectedIndex
        Resource(EditorIndex).Name = Trim$(txtName.Text)
        lstIndex.Items.RemoveAt(EditorIndex - 1)
        lstIndex.Items.Insert(EditorIndex - 1, EditorIndex & ": " & Resource(EditorIndex).Name)
        lstIndex.SelectedIndex = tmpIndex
    End Sub

    Private Sub TxtMessage_TextChanged(sender As Object, e As EventArgs) Handles txtMessage.TextChanged
        Resource(EditorIndex).SuccessMessage = Trim$(txtMessage.Text)
    End Sub

    Private Sub TxtMessage2_TextChanged(sender As Object, e As EventArgs) Handles txtMessage2.TextChanged
        Resource(EditorIndex).EmptyMessage = Trim$(txtMessage2.Text)
    End Sub
End Class