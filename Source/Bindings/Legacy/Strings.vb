Imports System.IO
Imports System.Windows.Forms

Friend NotInheritable Class Strings
    Private Sub New()
    End Sub

    Friend Enum OrionComponent
        Client = 0
        Editor
        Server
    End Enum

    Private Shared DefaultLanguage As Language
    Private Shared SelectedLanguage As Language

    Friend Shared Sub Init(component As OrionComponent, language As String)
        Dim path As String = Application.StartupPath & "\Data\"
        If Not Directory.Exists(path) then Directory.CreateDirectory(path)
        path += "Languages\"
        If Not Directory.Exists(path) then Directory.CreateDirectory(path)
        
        DefaultLanguage = New Language(path & component.ToString() & "_English.xml")
        dim fPath as string =  path & component.ToString() & "_" & language & ".xml"
        if file.Exists(fPath) Then SelectedLanguage = New Language(fPath)
    End Sub

    Friend Shared Function [Get](section As String, id As String, ParamArray args As Object()) As String
        If SelectedLanguage IsNot Nothing AndAlso SelectedLanguage.Loaded() AndAlso SelectedLanguage.HasString(section, id) Then
            Return SelectedLanguage.GetString(section, id, args)
        End If
        If DefaultLanguage IsNot Nothing AndAlso DefaultLanguage.Loaded() AndAlso DefaultLanguage.HasString(section, id) Then
            Return DefaultLanguage.GetString(section, id, args)
        End If
        Return "Not Found"
    End Function
End Class