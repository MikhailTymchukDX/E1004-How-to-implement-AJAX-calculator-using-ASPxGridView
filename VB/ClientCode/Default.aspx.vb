Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Collections.Generic
Imports System.Collections
Imports DevExpress.Web.ASPxGridView
Imports DevExpress.Web.ASPxEditors

Partial Public Class _Default
	Inherits System.Web.UI.Page

	#region "DataSources"
	Protected Function GetProductDataSource(ByVal categoryId As Object) As DataView
		Dim fdataView As New DataView(CType(Session("lookupTable"), DataTable))
		fdataView.RowFilter = "[CategoryID] = " & categoryId.ToString() & "or [CategoryID] = -1"
		Return fdataView
	End Function
	Private Sub PrepareTables()
		Dim dataTable As New DataTable()
		dataTable.Columns.Add("CategoryID", GetType(Integer)) ' key
		dataTable.Columns.Add("CategoryName", GetType(String))
		dataTable.Columns.Add("ProductID", GetType(Integer))
		dataTable.Columns.Add("UnitQty", GetType(Integer))
		dataTable.PrimaryKey = New DataColumn() { dataTable.Columns("CategoryID") }
		dataTable.Rows.Add(New Object() { 0, "Windows Forms" })
		dataTable.Rows.Add(New Object() { 1, "ASP" })
		dataTable.Rows.Add(New Object() { 2, "VCL" })
		Session("mainTable") = dataTable

		dataTable = New DataTable()
		dataTable.Columns.Add("CategoryID", GetType(Integer))
		dataTable.Columns.Add("ProductID", GetType(Integer)) ' ' key
		dataTable.Columns.Add("Description", GetType(String))
		dataTable.Columns.Add("UnitPrice", GetType(Double))
		dataTable.PrimaryKey = New DataColumn() { dataTable.Columns("ProductID") }

		dataTable.Rows.Add(New Object() { 0, 1, "XtraGrid", 1000.0 })
		dataTable.Rows.Add(New Object() { 0, 2, "XtraBars", 750.0 })
		dataTable.Rows.Add(New Object() { 1, 3, "ASPxGridView", 500.0 })
		dataTable.Rows.Add(New Object() { 1, 4, "ASPxPerience", 300.0 })
		dataTable.Rows.Add(New Object() { 2, 5, "ExpressQuantumGrid", 825.0 })
		dataTable.Rows.Add(New Object() { -1, -1, "", 0.0 }) '' empty

		Session("lookupTable") = dataTable
	End Sub

	Protected Function GetGridDataSource() As DataTable
		Return CType(Session("mainTable"), DataTable)
	End Function
	Protected Function GetLookupDataSource() As DataTable
		Return CType(Session("lookupTable"), DataTable)
	End Function
	#End Region

	Protected Sub grid_CustomUnboundColumnData(ByVal sender As Object, ByVal e As ASPxGridViewColumnDataEventArgs)
		If e.Column.FieldName = "UnitPrice" Then
			e.Value = CalcUnitPrice(e.GetListSourceFieldValue("ProductID"))
		End If
		If e.Column.FieldName = "Sum" Then
			e.Value = CalcSum(e.GetListSourceFieldValue("UnitPrice"), e.GetListSourceFieldValue("UnitQty"))
		End If
	End Sub
	Protected Function CalcUnitPrice(ByVal projectID As Object) As Object
		If projectID Is DBNull.Value Then
			Return 0
		End If
		Dim fdataTable As DataTable = GetLookupDataSource()
		Return fdataTable.Rows.Find(projectID)("UnitPrice")
	End Function

	Private Function CalcSum(ByVal unitPrice As Object, ByVal unitQty As Object) As Object
		If unitPrice Is DBNull.Value OrElse unitQty Is DBNull.Value Then
			Return 0
		End If
		Return Convert.ToDecimal(unitPrice) * Convert.ToDecimal(unitQty)
	End Function


	Protected Sub grid_DataBound(ByVal sender As Object, ByVal e As EventArgs)

	End Sub
	Protected Sub grid_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs)

	End Sub
	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		If Session("mainTable") Is Nothing Then
			PrepareTables()
		End If
		grid.DataSource = GetGridDataSource()
		grid.KeyFieldName = "CategoryID"
'        grid.DataBind();
		SynchronizeData()
		grid.DataBind()
	End Sub

	Private Sub SynchronizeData()
		Dim columnQty As GridViewDataColumn = CType(grid.Columns("UnitQty"), GridViewDataColumn)
		Dim columnProduct As GridViewDataColumn = CType(grid.Columns("Product"), GridViewDataColumn)
		For i As Integer = 0 To grid.VisibleRowCount - 1

			Dim spin As ASPxSpinEdit = CType(grid.FindRowCellTemplateControl(i, columnQty, "QtySpin"), ASPxSpinEdit)
			If spin IsNot Nothing AndAlso spin.Value IsNot Nothing Then
				SetFieldValue(i, "UnitQty", spin.Value)
			End If
			Dim combo As ASPxComboBox = CType(grid.FindRowCellTemplateControl(i, columnProduct, "ProductCombo"), ASPxComboBox)
			If combo IsNot Nothing AndAlso combo.Value IsNot Nothing Then
				SetFieldValue(i, "ProductID", combo.Value)
			End If
		Next i
	End Sub
	Protected Sub grid_HtmlRowCreated(ByVal sender As Object, ByVal e As ASPxGridViewTableRowEventArgs)
		If e.RowType = GridViewRowType.Data Then
			Dim gridView As ASPxGridView = CType(sender, ASPxGridView)

			Dim spin As ASPxSpinEdit = CType(gridView.FindRowCellTemplateControl(e.VisibleIndex, CType(gridView.Columns("UnitQty"), GridViewDataColumn), "QtySpin"), ASPxSpinEdit)
			spin.Attributes.Add("rowIndex", e.VisibleIndex.ToString())

			Dim lblPrice As ASPxLabel = CType(gridView.FindRowCellTemplateControl(e.VisibleIndex, CType(gridView.Columns("UnitPrice"), GridViewDataColumn), "lblPrice"), ASPxLabel)
			lblPrice.ClientInstanceName = "lblPrice_" & e.VisibleIndex.ToString()

			Dim lbl As ASPxLabel = CType(gridView.FindRowCellTemplateControl(e.VisibleIndex, CType(gridView.Columns("Sum"), GridViewDataColumn), "lblSum"), ASPxLabel)
			lbl.ClientInstanceName = "lblSum_" & e.VisibleIndex.ToString()

		End If
	End Sub

	Private Sub SetFieldValue(ByVal rowIndex As Integer, ByVal fieldName As String, ByVal value As Object)
		Dim dataTable As DataTable = GetGridDataSource()
		Dim keyValue As Object = grid.GetRowValues(rowIndex, New String() {grid.KeyFieldName})
		Dim row As DataRow = dataTable.Rows.Find(keyValue)
		row(fieldName) = value
	End Sub
End Class

