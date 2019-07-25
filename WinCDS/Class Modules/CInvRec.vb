﻿Public Class CInvRec
    Public PoSold As Double
    Public Available As Double
    Public Desc As String
    Public OnHand As Double
    Public Style As String
    Public Landed As Decimal
    Public OnSale As Decimal
    Public List As Decimal
    Public DeptNo As String
    Public VendorNo As String
    Public Vendor As String
    Public Comments As String
    Public Sales1 As Double
    Public Sales2 As Double
    Public Sales3 As Double
    Public Sales4 As Double
    Private WithEvents mDataAccess As CDataAccess
    'Implements CDataAccess
    Private LocBal(0 To Setup_MaxStores_DB - 1)
    Public Cost As Decimal
    Public RN As String
    Private OO(0 To Setup_MaxStores_DB - 1)

    Public MinStk As Double
    Public Freight As Double
    Public FreightType as integer  ' 0 == percentage, 1 == $$ amount
    Public GM As String
    Public MarkUp As Double
    Public Spiff As Decimal
    Public RDate As String
    Public SKU As String
    Public Cubes As Double
    Public Psales1 As Double
    Public Psales2 As Double
    Public Psales3 As Double
    Public Psales4 As Double


    Public Function Load(ByVal KeyVal As String, Optional ByRef KeyName As String = "") As Boolean
        ' Checks the database for a matching Style ID.
        ' Returns True if the load was successful, false otherwise.
        ' If a record was found, also loads the data into this object.

        ' Search for the Style
        Load = False
        If KeyName = "" Then
            DataAccess.Records_OpenIndexAt(KeyVal)
        ElseIf Left(KeyName, 1) = "#" Then
            ' This allows searching by AutoNumber - specialized to query by number
            ' since Access is exceptionally picky about quotation marks.
            DataAccess.Records_OpenFieldIndexAtNumber(Mid(KeyName, 2), KeyVal)
        Else
            DataAccess.Records_OpenFieldIndexAt(KeyName, KeyVal)
        End If

        If DataAccess.Records_Available Then Load = True
    End Function
    Public Sub ItemsSold(ByRef Quan As Double, ByRef DateSold As Date)
        If Year(DateSold) <> Year(Date.Today) Then Exit Sub
        Select Case DatePart("q", DateSold)
            Case 1
                Sales1 = Sales1 + Quan
            Case 2
                Sales2 = Sales2 + Quan
            Case 3
                Sales3 = Sales3 + Quan
            Case 4
                Sales4 = Sales4 + Quan
            Case Else
                MsgBox("Error: Can't determine which quarter " & DateSold & " is in.")
        End Select
    End Sub
    Public Sub AddLocationQuantity(ByVal Location as integer, ByVal Quantity As Double)
        SetStock(Location, QueryStock(Location) + Quantity)
    End Sub
    Public Sub Save()
        ' This instructs the class (in one simple call) to save its data members to the database.
        If DataAccess.Record_EOF Then
            DataAccess.Records_Add()
        End If
        ' Then load our data into the recordset.
        DataAccess.Record_Update
        ' And finally, tell the class to save the recordset.
        DataAccess.Records_Update
    End Sub
    Public Function DataAccess() As CDataAccess
        DataAccess = mDataAccess
    End Function
    Public Function SetStock(ByVal StoreNum as integer, ByVal Val As Double)
        SetStock = Nothing
        If Not InvRecValidLocation(StoreNum) Then Exit Function
        LocBal(StoreNum - 1) = Val
    End Function
    Public Function QueryStock(ByVal StoreNum as integer) As Double
        QueryStock = 0
        If StoreNum = 0 Then QueryStock = QueryTotalStock()
        If Not InvRecValidLocation(StoreNum) Then Exit Function
        QueryStock = LocBal(StoreNum - 1)
    End Function
    Private Function InvRecValidLocation(ByRef Loc as integer) As Boolean
        InvRecValidLocation = (Loc >= 1 And Loc <= Setup_MaxStores_DB)
    End Function
    Public Function QueryTotalStock() As Double
        Dim I as integer
        QueryTotalStock = 0
        For I = 1 To Setup_MaxStores_DB
            QueryTotalStock = QueryTotalStock + QueryStock(I)
        Next
    End Function
    Public Function QueryOnOrder(ByVal StoreNum as integer) As Double
        If StoreNum = 0 Then QueryOnOrder = QueryTotalOnOrder
        If Not InvRecValidLocation(StoreNum) Then Exit Function
        QueryOnOrder = OO(StoreNum)
    End Function
    Public Function QueryTotalOnOrder() As Double
        Dim I as integer
        QueryTotalOnOrder = 0
        For I = 1 To Setup_MaxStores_DB
            QueryTotalOnOrder = QueryTotalOnOrder + QueryOnOrder(I)
        Next
    End Function

End Class