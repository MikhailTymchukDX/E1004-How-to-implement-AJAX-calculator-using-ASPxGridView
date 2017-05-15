<%@ Page Language="vb" AutoEventWireup="true"  CodeFile="Default.aspx.vb" Inherits="_Default" %>

<%@ Register Assembly="DevExpress.Web.v8.3, Version=8.3.2.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
	Namespace="DevExpress.Web.ASPxCallback" TagPrefix="dxcb" %>

<%@ Register Assembly="DevExpress.Web.ASPxGridView.v8.3, Version=8.3.2.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
	Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>
<%@ Register Assembly="DevExpress.Web.ASPxEditors.v8.3, Version=8.3.2.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
	Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dxe" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title>Untitled Page</title>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<dxwgv:ASPxGridView ID="grid" runat="server" AutoGenerateColumns="False" Width="563px" KeyFieldName="ProductID" OnCustomUnboundColumnData="grid_CustomUnboundColumnData" OnDataBound="grid_DataBound" OnRowUpdating="grid_RowUpdating" OnHtmlRowCreated="grid_HtmlRowCreated">
			<TotalSummary>
				<dxwgv:ASPxSummaryItem FieldName="Sum" SummaryType="Sum" />
			</TotalSummary>
			<Settings ShowFooter="True" ShowGroupButtons="False" />
			<SettingsEditing Mode="Inline" />
			<Columns>
				<dxwgv:GridViewDataTextColumn FieldName="CategoryName" VisibleIndex="0">
				</dxwgv:GridViewDataTextColumn>
				<dxwgv:GridViewDataTextColumn FieldName="Product" VisibleIndex="1">
				<DataItemTemplate>
					<dxe:ASPxComboBox ID="ProductCombo" runat="server" Value='<%#Eval("ProductID")%>' ValueField="ProductID"  TextField="Description" DataSource='<%#GetProductDataSource(Eval("CategoryID"))%>' ValueType="System.Int32">
					<ClientSideEvents 
						ValueChanged="
						function(s, e) {
							grid.PerformCallback();
						}"
					/>
					</dxe:ASPxComboBox>
				</DataItemTemplate>
				</dxwgv:GridViewDataTextColumn>
				<dxwgv:GridViewDataTextColumn Caption="UnitQty" VisibleIndex="2">
					<DataItemTemplate>
					<dxe:ASPxSpinEdit ID="QtySpin" runat="server" MaxValue="500" Value='<%#Eval("UnitQty")%>'>
						<ClientSideEvents 
						ValueChanged="
						function(s, e) {
							var rowIndex = s.GetMainElement().attributes['rowIndex'].nodeValue;
							var qty = s.GetValue();
							var priceLbl = eval('lblPrice_' + rowIndex.toString());
							var price = parseInt(priceLbl.GetText());
							var sumLbl = eval('lblSum_' + rowIndex.toString());
							sumLbl.SetText(price * qty);
						}"                        
						/>
					</dxe:ASPxSpinEdit>    
					</DataItemTemplate>
				</dxwgv:GridViewDataTextColumn>
				<dxwgv:GridViewDataTextColumn FieldName="UnitPrice" VisibleIndex="3" UnboundType="Decimal">
					<DataItemTemplate>
						<dxe:ASPxLabel ID="lblPrice" runat="server" Text='<%#Eval("UnitPrice")%>'></dxe:ASPxLabel>
					</DataItemTemplate>                
				</dxwgv:GridViewDataTextColumn>


<dxwgv:GridViewDataTextColumn FieldName="Sum" ReadOnly="True" UnboundType="Decimal"
					VisibleIndex="4">
					<EditFormSettings Visible="False" />
					<DataItemTemplate>
						<dxe:ASPxLabel ID="lblSum" runat="server" Text='<%#Eval("Sum")%>'></dxe:ASPxLabel>
					</DataItemTemplate>
				</dxwgv:GridViewDataTextColumn>
			</Columns>
		</dxwgv:ASPxGridView>    
		<dxe:ASPxButton ID="ASPxButton1" runat="server" AutoPostBack="False" Text="Update sum">
			<ClientSideEvents Click="function(s, e) {
	grid.PerformCallback();
}" />
		</dxe:ASPxButton>
		</div>
	</form>
</body>
</html>
