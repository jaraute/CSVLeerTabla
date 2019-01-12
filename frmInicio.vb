Imports System.Windows.Forms
Imports System.Data

Public Class frmInicio
    Dim fiCSV As String = ""
    Public dTabla As DataTable
    '
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        fiCSV = IO.Path.Combine(_appFull(appParte.Directorio), "DICTIONARY.txt")
        If IO.File.Exists(fiCSV) Then
            txtCSV.Text = fiCSV
            CargarDAtos()
        Else
            txtCSV.Text = ""
        End If
    End Sub
    Private Sub btnCSV_Click(sender As Object, e As EventArgs) Handles btnCSV.Click
        Dim oFd As New OpenFileDialog With {
            .InitialDirectory = _appFull(appParte.Directorio),
            .Filter = "Fichero CSV (*.csv)|*.csv|Fichero TXT (*.txt)|*.txt",
            .FilterIndex = 1
        }
        If oFd.ShowDialog = DialogResult.OK Then
            txtCSV.Text = oFd.FileName
        End If
        CargarDAtos()
    End Sub

    Public Sub CargarDAtos()
        If IO.File.Exists(fiCSV) = False Then Exit Sub
        dTabla = New DataTable("DATOSCSV")
        Using MyReader As New Microsoft.VisualBasic.FileIO.TextFieldParser(fiCSV, System.Text.Encoding.UTF8)
            MyReader.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited
            MyReader.Delimiters = New String() {vbTab, ";"}
            MyReader.CommentTokens = New String() {""}
            'MyReader.HasFieldsEnclosedInQuotes = True
            Dim currentRow As String()
            'Loop through all of the fields in the file. 
            'If any lines are corrupt, report an error and continue parsing. 
            While Not MyReader.EndOfData
                Try
                    currentRow = MyReader.ReadFields()
                    If MyReader.LineNumber = 2 Then
                        '' Crear las cabeceras
                        For Each tCol As String In currentRow
                            Call dTabla.Columns.Add(tCol, GetType(String))
                        Next
                    Else
                        '' Insertar los datos
                        Dim newRow = dTabla.Rows.Add()
                        newRow.ItemArray = currentRow
                    End If
                Catch ex As Microsoft.VisualBasic.FileIO.MalformedLineException
                    MsgBox("Line " & ex.Message &
                    " is invalid.  Skipping")
                End Try
            End While
            dgv1.DataSource = dTabla
        End Using
    End Sub

    Public Function _appFull(Optional parte As appParte = appParte.Full) As String
        Select Case parte
            Case appParte.Full
                Return System.Reflection.Assembly.GetEntryAssembly.Location
            Case appParte.Directorio
                Return IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly.Location)
            Case appParte.NameSin
                Return IO.Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly.Location)
            Case appParte.Directorio
                Return IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly.Location)
            Case appParte.Extension
                Return IO.Path.GetExtension(System.Reflection.Assembly.GetEntryAssembly.Location)
            Case Else
                Return System.Reflection.Assembly.GetEntryAssembly.Location
        End Select
    End Function
    Public Enum appParte
        Full
        NameSin
        Name
        Extension
        Directorio
    End Enum
End Class