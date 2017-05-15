using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxEditors;

public partial class _Default : System.Web.UI.Page 
{

    #region "DataSources"
    protected DataView GetProductDataSource(object categoryId ) {
        DataView fdataView = new DataView((DataTable)Session["lookupTable"]);
        fdataView.RowFilter = "[CategoryID] = " + categoryId.ToString() + "or [CategoryID] = -1";
        return fdataView;
    }
    void PrepareTables() {
        DataTable dataTable = new DataTable();
        dataTable.Columns.Add("CategoryID", typeof(int)); // key
        dataTable.Columns.Add("CategoryName", typeof(string));
        dataTable.Columns.Add("ProductID", typeof(int));
        dataTable.Columns.Add("UnitQty", typeof(int));
        dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["CategoryID"] };
        dataTable.Rows.Add(new object[] { 0, "Windows Forms" });
        dataTable.Rows.Add(new object[] { 1, "ASP" });
        dataTable.Rows.Add(new object[] { 2, "VCL" });
        Session["mainTable"] = dataTable;

        dataTable = new DataTable();
        dataTable.Columns.Add("CategoryID", typeof(int));
        dataTable.Columns.Add("ProductID", typeof(int));// ' key
        dataTable.Columns.Add("Description", typeof(string));
        dataTable.Columns.Add("UnitPrice", typeof(double));
        dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["ProductID"] };

        dataTable.Rows.Add(new object[] { 0, 1, "XtraGrid", 1000.0 });
        dataTable.Rows.Add(new object[] { 0, 2, "XtraBars", 750.0 });
        dataTable.Rows.Add(new object[] { 1, 3, "ASPxGridView", 500.0 });
        dataTable.Rows.Add(new object[] { 1, 4, "ASPxPerience", 300.0 });
        dataTable.Rows.Add(new object[] { 2, 5, "ExpressQuantumGrid", 825.0 });
        dataTable.Rows.Add(new object[] { -1, -1, "", 0.0 }); //' empty

        Session["lookupTable"] = dataTable;
    }

    protected DataTable GetGridDataSource() {
        return (DataTable)Session["mainTable"];
    }
    protected DataTable GetLookupDataSource() {
        return (DataTable)Session["lookupTable"];
    }
    #endregion

    protected void grid_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e) {
        if (e.Column.FieldName == "UnitPrice") {
            e.Value = CalcUnitPrice(e.GetListSourceFieldValue("ProductID"));
        }
        if (e.Column.FieldName == "Sum") {
            e.Value = CalcSum(e.GetListSourceFieldValue("UnitPrice"), e.GetListSourceFieldValue("UnitQty"));
        }
    }
    protected object CalcUnitPrice(object projectID){
        if (projectID == DBNull.Value)
            return 0;
        DataTable fdataTable = GetLookupDataSource();
        return fdataTable.Rows.Find(projectID)["UnitPrice"];
    }

    object CalcSum(object unitPrice , object unitQty) {
        if (unitPrice == DBNull.Value || unitQty == DBNull.Value)
            return 0;
        return Convert.ToDecimal(unitPrice) * Convert.ToDecimal(unitQty);
    }


    protected void grid_DataBound(object sender, EventArgs e) {

    }
    protected void grid_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e) {

    }
    protected void Page_Load(object sender, EventArgs e) {
        if(Session["mainTable"] == null)
            PrepareTables();
        grid.DataSource = GetGridDataSource();
        grid.KeyFieldName = "CategoryID";
//        grid.DataBind();
        SynchronizeData();
        grid.DataBind();
    }

    private void SynchronizeData() {
        GridViewDataColumn columnQty = (GridViewDataColumn)grid.Columns["UnitQty"];
        GridViewDataColumn columnProduct = (GridViewDataColumn)grid.Columns["Product"];
        for (int i = 0; i < grid.VisibleRowCount; i++) {

            ASPxSpinEdit spin = (ASPxSpinEdit)grid.FindRowCellTemplateControl(i, columnQty, "QtySpin");
            if (spin != null && spin.Value != null) 
                SetFieldValue(i, "UnitQty", spin.Value);
            ASPxComboBox combo = (ASPxComboBox)grid.FindRowCellTemplateControl(i, columnProduct, "ProductCombo");
            if (combo != null && combo.Value != null)
                SetFieldValue(i, "ProductID", combo.Value);
        }
    }
    protected void grid_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e) {
        if(e.RowType == GridViewRowType.Data) {
            ASPxGridView gridView = (ASPxGridView)sender;

            ASPxSpinEdit spin = (ASPxSpinEdit)gridView.FindRowCellTemplateControl(e.VisibleIndex, (GridViewDataColumn)gridView.Columns["UnitQty"], "QtySpin");
            spin.Attributes.Add("rowIndex", e.VisibleIndex.ToString());

            ASPxLabel lblPrice = (ASPxLabel)gridView.FindRowCellTemplateControl(e.VisibleIndex, (GridViewDataColumn)gridView.Columns["UnitPrice"], "lblPrice");
            lblPrice.ClientInstanceName = "lblPrice_" + e.VisibleIndex.ToString();

            ASPxLabel lbl = (ASPxLabel)gridView.FindRowCellTemplateControl(e.VisibleIndex, (GridViewDataColumn)gridView.Columns["Sum"], "lblSum");
            lbl.ClientInstanceName = "lblSum_" + e.VisibleIndex.ToString();

        }
    }

    void SetFieldValue(int rowIndex, string fieldName, object value) {
        DataTable dataTable = GetGridDataSource();
        object keyValue = grid.GetRowValues(rowIndex, new string[] {grid.KeyFieldName});
        DataRow row = dataTable.Rows.Find(keyValue);
        row[fieldName] = value;
    }
}

