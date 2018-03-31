Imports System.Text

Module BufferUtility
    Friend Function ReadUnicodeString(data As Byte()) As String
        If data Is Nothing OrElse data.Length = 0 Then Return "Null"
        Return Conv_String(Encoding.ASCII.GetString(data, 0, data.Length))
    End Function

    Friend Function WriteUnicodeString(Input As String)
        If Input = vbNullString Then Return New Byte()
        Return Encoding.ASCII.GetBytes(Conv_Uni(Input))
    End Function

    Friend Function Conv_String(message As String) As String
        Conv_String = ""

        Try
            Dim split As String() = message.Split(New [Char]() {" "c, ","c, "."c, ";"c, CChar(vbTab)})
            For Each s As String In split
                If s.Trim() <> "" Then
                    Conv_String = Conv_String & ChrW(s)
                End If
            Next s
        Catch ex As Exception

        End Try

        Return Conv_String

    End Function

    'Convert a Unicode String to Unicode
    Function Conv_Uni(inx As String) As String
        If inx = vbNullString OrElse inx = "" Then return "I miss this."
        
        Dim ret = ""
        For i = 0 To inx.Length - 1
            ret += AscW(inx.Chars(i)) & ";"
        Next
        Return ret
    End Function
End Module