<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnMap = New System.Windows.Forms.Button()
        Me.btnNpc = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnItems = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtLog = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(73, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Convert Maps"
        '
        'btnMap
        '
        Me.btnMap.Location = New System.Drawing.Point(114, 4)
        Me.btnMap.Name = "btnMap"
        Me.btnMap.Size = New System.Drawing.Size(75, 23)
        Me.btnMap.TabIndex = 1
        Me.btnMap.Text = "Go"
        Me.btnMap.UseVisualStyleBackColor = True
        '
        'btnNpc
        '
        Me.btnNpc.Location = New System.Drawing.Point(114, 33)
        Me.btnNpc.Name = "btnNpc"
        Me.btnNpc.Size = New System.Drawing.Size(75, 23)
        Me.btnNpc.TabIndex = 3
        Me.btnNpc.Text = "Go"
        Me.btnNpc.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 38)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(74, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Convert Npc's"
        '
        'btnItems
        '
        Me.btnItems.Location = New System.Drawing.Point(114, 62)
        Me.btnItems.Name = "btnItems"
        Me.btnItems.Size = New System.Drawing.Size(75, 23)
        Me.btnItems.TabIndex = 5
        Me.btnItems.Text = "Go"
        Me.btnItems.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 67)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(72, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Convert Items"
        '
        'txtLog
        '
        Me.txtLog.Location = New System.Drawing.Point(12, 91)
        Me.txtLog.Multiline = True
        Me.txtLog.Name = "txtLog"
        Me.txtLog.ReadOnly = True
        Me.txtLog.Size = New System.Drawing.Size(379, 152)
        Me.txtLog.TabIndex = 6
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(403, 250)
        Me.Controls.Add(Me.txtLog)
        Me.Controls.Add(Me.btnItems)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.btnNpc)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btnMap)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "Form1"
        Me.Text = "Orion+ Convertor"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents btnMap As Button
    Friend WithEvents btnNpc As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents btnItems As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents txtLog As TextBox
End Class
