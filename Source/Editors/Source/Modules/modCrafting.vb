
Imports ASFW

Friend Module modCrafting
#Region "Globals"

    Friend Recipe_Changed(MAX_RECIPE) As Boolean
    Friend Recipe(MAX_RECIPE) As RecipeRec
    Friend InitRecipeEditor As Boolean
    Friend InitCrafting As Boolean
    Friend InCraft As Boolean
    Friend pnlCraftVisible As Boolean

    Friend Const RecipeType_Herb As Byte = 0
    Friend Const RecipeType_Wood As Byte = 1
    Friend Const RecipeType_Metal As Byte = 2

    Friend RecipeNames(MAX_RECIPE) As String

    Friend chkKnownOnlyChecked As Boolean
    Friend chkKnownOnlyEnabled As Boolean
    Friend btnCraftEnabled As Boolean
    Friend btnCraftStopEnabled As Boolean
    Friend nudCraftAmountEnabled As Boolean
    Friend lstRecipeEnabled As Boolean

    Friend CraftAmountValue As Byte
    Friend CraftProgressValue As Integer
    Friend picProductindex as integer
    Friend lblProductNameText As String
    Friend lblProductAmountText As String

    Friend picMaterialIndex(MAX_INGREDIENT) As Integer
    Friend lblMaterialName(MAX_INGREDIENT) As String
    Friend lblMaterialAmount(MAX_INGREDIENT) As String

    Friend SelectedRecipe As Integer = 0

    Friend Structure RecipeRec
        Dim Name As String
        Dim RecipeType As Byte
        Dim MakeItemNum As Integer
        Dim MakeItemAmount As Integer
        Dim Ingredients() As IngredientsRec
        Dim CreateTime As Byte
    End Structure

    Friend Structure IngredientsRec
        Dim ItemNum As Integer
        Dim Value As Integer
    End Structure

#End Region

#Region "Database"
    Sub ClearRecipes()
        Dim i As Integer

        For i = 1 To MAX_RECIPE
            ClearRecipe(i)
        Next

    End Sub

    Sub ClearRecipe(Num As Integer)
        Recipe(Num).Name = ""
        Recipe(Num).RecipeType = 0
        Recipe(Num).MakeItemNum = 0
        ReDim Recipe(Num).Ingredients(MAX_INGREDIENT)
    End Sub

    Friend Sub ClearChanged_Recipe()
        Dim i As Integer

        For i = 1 To MAX_RECIPE
            Recipe_Changed(i) = Nothing
        Next

        ReDim Recipe_Changed(MAX_RECIPE)
    End Sub
#End Region

#Region "Editor"
    Friend Sub RecipeEditorPreInit()
        Dim i As Integer

        With frmRecipe
            Editor = EDITOR_RECIPE
            .lstIndex.Items.Clear()

            ' Add the names
            For i = 1 To MAX_RECIPE
                .lstIndex.Items.Add(i & ": " & Trim$(Recipe(i).Name))
            Next

            'fill comboboxes
            .cmbMakeItem.Items.Clear()
            .cmbIngredient.Items.Clear()

            .cmbMakeItem.Items.Add("None")
            .cmbIngredient.Items.Add("None")
            For i = 1 To MAX_ITEMS
                .cmbMakeItem.Items.Add(Trim$(Item(i).Name))
                .cmbIngredient.Items.Add(Trim$(Item(i).Name))
            Next

            .Show()
            .lstIndex.SelectedIndex = 0
            RecipeEditorInit()
        End With
    End Sub

    Friend Sub RecipeEditorInit()

        If frmRecipe.Visible = False Then Exit Sub
        EditorIndex = frmRecipe.lstIndex.SelectedIndex + 1

        With Recipe(EditorIndex)
            frmRecipe.txtName.Text = Trim$(.Name)

            frmRecipe.lstIngredients.Items.Clear()

            frmRecipe.cmbType.SelectedIndex = .RecipeType
            frmRecipe.cmbMakeItem.SelectedIndex = .MakeItemNum

            If .MakeItemAmount < 1 Then .MakeItemAmount = 1
            frmRecipe.nudAmount.Value = .MakeItemAmount

            If .CreateTime < 1 Then .CreateTime = 1
            frmRecipe.nudCreateTime.Value = .CreateTime

            UpdateIngredient()
        End With

        Recipe_Changed(EditorIndex) = True

    End Sub

    Friend Sub RecipeEditorCancel()
        Editor = 0
        frmRecipe.Visible = False
        ClearChanged_Recipe()
        ClearRecipes()
        SendRequestRecipes()
    End Sub

    Friend Sub RecipeEditorOk()
        Dim i As Integer

        For i = 1 To MAX_RECIPE
            If Recipe_Changed(i) Then
                SendSaveRecipe(i)
            End If
        Next

        frmRecipe.Visible = False
        Editor = 0
        ClearChanged_Recipe()
    End Sub

    Friend Sub UpdateIngredient()
        Dim i As Integer
        frmRecipe.lstIngredients.Items.Clear()

        For i = 1 To MAX_INGREDIENT
            With Recipe(EditorIndex).Ingredients(i)
                ' if none, show as none
                If .ItemNum <= 0 AndAlso .Value = 0 Then
                    frmRecipe.lstIngredients.Items.Add("Empty")
                Else
                    frmRecipe.lstIngredients.Items.Add(Trim$(Item(.ItemNum).Name) & " X " & .Value)
                End If

            End With
        Next

        frmRecipe.lstIngredients.SelectedIndex = 0
    End Sub
#End Region

#Region "Incoming Packets"
    Sub Packet_UpdateRecipe(ByRef data() As Byte)
        Dim n As Integer, i As Integer
        dim buffer as New ByteStream(Data)
        'recipe index
        n = Buffer.ReadInt32

        ' Update the Recipe
        Recipe(n).Name = Trim$(Buffer.ReadString)
        Recipe(n).RecipeType = Buffer.ReadInt32
        Recipe(n).MakeItemNum = Buffer.ReadInt32
        Recipe(n).MakeItemAmount = Buffer.ReadInt32

        For i = 1 To MAX_INGREDIENT
            Recipe(n).Ingredients(i).ItemNum = Buffer.ReadInt32()
            Recipe(n).Ingredients(i).Value = Buffer.ReadInt32()
        Next

        Recipe(n).CreateTime = Buffer.ReadInt32

        Buffer.Dispose()

    End Sub

    Sub Packet_RecipeEditor(ByRef data() As Byte)
        InitRecipeEditor = True
    End Sub

#End Region

#Region "OutGoing Packets"
    Sub SendRequestRecipes()
        dim buffer as New ByteStream(4)

        Buffer.WriteInt32(ClientPackets.CRequestRecipes)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendRequestEditRecipes()
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.RequestEditRecipes)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

    Sub SendSaveRecipe(RecipeNum As Integer)
        dim buffer as ByteStream
        Buffer = New ByteStream(4)

        Buffer.WriteInt32(EditorPackets.SaveRecipe)

        Buffer.WriteInt32(RecipeNum)

        Buffer.WriteString(Trim$(Recipe(RecipeNum).Name))
        Buffer.WriteInt32(Recipe(RecipeNum).RecipeType)
        Buffer.WriteInt32(Recipe(RecipeNum).MakeItemNum)
        Buffer.WriteInt32(Recipe(RecipeNum).MakeItemAmount)

        For i = 1 To MAX_INGREDIENT
            Buffer.WriteInt32(Recipe(RecipeNum).Ingredients(i).ItemNum)
            Buffer.WriteInt32(Recipe(RecipeNum).Ingredients(i).Value)
        Next

        Buffer.WriteInt32(Recipe(RecipeNum).CreateTime)

        Socket.SendData(Buffer.Data, Buffer.Head)
        Buffer.Dispose()
    End Sub

#End Region

#Region "Functions"

    Function GetRecipeIndex(RecipeName As String) As Integer
        Dim i As Integer

        GetRecipeIndex = 0

        For i = 1 To MAX_RECIPE
            If Trim$(Recipe(i).Name) = Trim$(RecipeName) Then
                GetRecipeIndex = i
                Exit For
            End If
        Next

    End Function
#End Region

End Module
