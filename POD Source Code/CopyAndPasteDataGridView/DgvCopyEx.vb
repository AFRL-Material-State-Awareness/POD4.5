Imports System.Windows.Forms

Public Class clsDgvCopyPasteEx
    Private _Gridview As DataGridView
    Private _DgvContextMenu As ContextMenuStrip
    Private _PasteToolstripItem As New ToolStripMenuItem("Paste Text Data")
    Private _InsertToolstripItem As New ToolStripMenuItem("Insert Text Data")
    Sub New(Gridview As DataGridView)
        Dim count As Integer = 0
        _Gridview = Gridview
        If _Gridview.ContextMenuStrip IsNot Nothing Then
            _DgvContextMenu = _Gridview.ContextMenuStrip

            For j = 0 To (_DgvContextMenu.Items.Count - 1)
                If _DgvContextMenu.Items(j).Text = "Paste Text Data" Then
                    count = 1
                End If
            Next

            If count = 0 Then
                _PasteToolstripItem.ShortcutKeys = Keys.Control Or Keys.V
                _DgvContextMenu.Items.Add(_PasteToolstripItem)
                '_DgvContextMenu.Items.Add(_InsertToolstripItem)
            End If
        End If
        AddHandler _PasteToolstripItem.Click, AddressOf PasteTextDataFromClipboard
        AddHandler _InsertToolstripItem.Click, AddressOf InsertTextDataFromClipboard
        AddHandler _Gridview.KeyDown, AddressOf PasteTextDataFromClipboard
        AddHandler _Gridview.CellContextMenuStripNeeded, AddressOf ShowPasteDataContextMenu
    End Sub

    Public Property Dgv As DataGridView
        Get
            Return _Gridview
        End Get
        Set(value As DataGridView)
            _Gridview = value
        End Set
    End Property

    'Handling  Mouse Event
    Private Sub PasteTextDataFromClipboard(sender As Object, e As EventArgs)
        PasteTextDataFromClipboard(_Gridview)
    End Sub


    Private Sub InsertTextDataFromClipboard(sender As Object, e As EventArgs)
        PasteTextDataFromClipboard(_Gridview, False)
    End Sub

    'Handling Keyboard Event
    Public Sub PasteTextDataFromClipboard(sender As System.Object, e As System.Windows.Forms.KeyEventArgs)
        'Pater the Data From clipboard
        If e.Control AndAlso e.KeyCode = Keys.V Then
            PasteTextDataFromClipboard(_Gridview)
        End If
    End Sub

    'InCase of CellContextMenu override then add the PasteToolStripitem
    Private Sub ShowPasteDataContextMenu(sender As Object, e As DataGridViewCellContextMenuStripNeededEventArgs)
        If e.ContextMenuStrip IsNot Nothing Then
            e.ContextMenuStrip.Items.Add(_PasteToolstripItem)
            e.ContextMenuStrip.Items.Add(_InsertToolstripItem)
        End If
    End Sub

    Sub PasteTextDataFromClipBoard(Dgv As DataGridView, Optional ShowMessage As Boolean = True)
        Try
            'Get the Data from the clipBoard as Text
            Dim ClipBoardData As String = Clipboard.GetText()
            If ClipBoardData.Trim.Length = 0 Then
                MessageBox.Show("No data to Paste")
            End If
            'This is where we do our data split logic(i.e) how the data will paste in to the  Grid
            'since we get the data from from clipboard as text  we consider each data is seperated by Tab and each row is seperated by new line charecter
            'Get seperate lines
            Dim lines As String() = ClipBoardData.Split(New Char(1) {ControlChars.Cr, ControlChars.Lf}, StringSplitOptions.RemoveEmptyEntries)

            Dim columnNames As String() = lines(0).Split(New Char(0) {ControlChars.Tab}, StringSplitOptions.RemoveEmptyEntries)
            Dim columnCount As Integer = columnNames.Length



            Dim bs As BindingSource
            If TypeOf Dgv.DataSource Is BindingSource Then
                bs = CType(Dgv.DataSource, BindingSource)
            Else
                bs = New BindingSource
                bs.DataSource = Dgv.DataSource
            End If

            Dim T As Type
            Dim Typename As String
            'Get the typename from the bining source
            Try
                Typename = bs.DataSource.FullName()
            Catch ex As Exception
                Try
                    Typename = bs.DataSource(0).GetType.FullName
                Catch ex1 As Exception
                    Try
                        Typename = bs.DataSource.GetType.FullName
                    Catch ex2 As Exception
                        Typename = ""
                    End Try
                End Try
            End Try
            'Create a Type from the TypeName
            If Typename = "" Then
                MessageBox.Show("problem in Data type ")
                Exit Sub
            End If
            T = Type.GetType(Typename)

            Dim RowIndex As Integer
            Dim ColIndex As Integer

            'Get the current row index where we need to paste the  data
            'The data is pasted from the selected row not from selected column 
            If Not (Dgv.CurrentCell Is Nothing) Then
                RowIndex = Dgv.CurrentCell.RowIndex
                ColIndex = Dgv.CurrentCell.ColumnIndex
            Else
                RowIndex = 0
                ColIndex = 0
            End If
            Dim Lcount As Integer = lines.Length
            Dim overwrite As DialogResult
            'Ask the user to overwirte the Existing Data or just paste in as new row
            'you may extend or remove this logic here?

            overwrite = Windows.Forms.DialogResult.No

            If Not (Dgv.CurrentCell Is Nothing) Then
                If Not (Dgv.CurrentCell.Value Is Nothing Or Dgv.CurrentCell.Value.ToString = String.Empty) And ShowMessage Then
                    overwrite = MessageBox.Show("Do you want to overwrite the current table? Selecting ""No"" will append the new data to the end of the table.", Dgv.Parent.Text, MessageBoxButtons.YesNo)
                End If
            End If

            Dim counter As Integer = 1
            'Loop through all the pasted lines
            While counter < Lcount
                'Check whether a new row is needed to insert inside a gridview
                If overwrite = Windows.Forms.DialogResult.No OrElse (Dgv.Rows.Count - 1) <= RowIndex Then
                    'Check whether the data is binded by Datatable or Not able to find the base type
                    If T Is Nothing OrElse Typename.Contains("System.Data") OrElse T.BaseType.FullName.Contains("System.Data") Then
                        Dim obj As Object
                        With bs
                            Dim tbl As DataTable
                            If Typename.Contains("View") Then                   'For DataView
                                tbl = CType(.DataSource, DataView).Table
                            ElseIf Typename.Contains("DataSet") Then            'For DataSet
                                tbl = CType(.DataSource, DataSet).Tables(Dgv.DataMember)
                            ElseIf Typename.Contains("Table") OrElse _
                                   T.BaseType.FullName.Contains("DataRow") Then 'For DataTable or TypeDataset
                                tbl = CType(.DataSource, DataTable)
                            Else
                                If bs.AllowNew = False Then
                                    MessageBox.Show("Not able to insert new records")
                                    Exit Sub
                                End If
                                obj = .AddNew
                                RowIndex = .Count - 1
                            End If
                            'Tbl is nothing then it is not possible to be add a Datarow to underlying Datasource
                            If tbl IsNot Nothing Then

                                While tbl.Columns.Count < columnCount
                                    Dim dataCol As DataColumn = tbl.Columns.Add(columnNames(tbl.Columns.Count))
                                End While

                                obj = tbl.NewRow
                                tbl.Rows.InsertAt(obj, RowIndex)
                            End If

                        End With

                    Else
                        'if it is an object source
                        'since we are uing BindingSource if we insert a object in the binding source it will
                        'Automatically insert a row in the Grid
                        With CType(Dgv.DataSource, BindingSource)
                            'Activator.CreateInstance create a new object for the given type
                            .Insert(RowIndex, Activator.CreateInstance(T))
                        End With
                    End If
                End If
                'choose the  Relative line
                Dim line As String = lines(counter)
                'split Data in a single line and iterate it
                Dim Datas As String() = line.Split(ControlChars.Tab)
                'Fill the data by looping through the values
                Dim colCount As Integer = Dgv.Columns.GetColumnCount(DataGridViewElementStates.Visible) - 1
                For j = 0 To Math.Min(Datas.Length - 1, ColCount)
                    Dgv.Item(j, RowIndex).Value = Datas(j)
                Next
                RowIndex += 1
                counter += 1
            End While

            For Each column As DataGridViewColumn In Dgv.Columns
                column.SortMode = DataGridViewColumnSortMode.NotSortable
            Next
            Dgv.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect
        Catch Pasteex As Exception
            MessageBox.Show(Pasteex.Message)
        End Try
    End Sub

End Class
