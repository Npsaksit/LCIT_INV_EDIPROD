using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Web.Services;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services.Protocols;
using System.Reflection;

public class YML {

  static sqlcon.Connection_String constr = new sqlcon.Connection_String();
  static convert_json.jsoncv convertFormat = new convert_json.jsoncv();
  static string statuscheck = "";

  public static string viewMode(string invoiceNo, string operatorCode) {
    string result = "";
    OleDbConnection con = new OleDbConnection();
    con.ConnectionString = constr.connectionEbs;
    OleDbCommand comd = new OleDbCommand("SELECT ROWNUM NO, inv.invoice_an, TO_CHAR(invoice_date, 'yyyy/MM/dd') AS invoice_date, 'THB' InvoiceCurrency, ocl.contract_unit_rate InvoiceTotalAmount, cl.total_unit Quantity, ocl.equipment_an ContainerNo, ocl.charge_line_ds CostDescription FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '" + invoiceNo + "'AND INV.CONTRACT_COMPANY_NM LIKE '%" + operatorCode + "%'AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND", con);

    OleDbDataAdapter sda = new OleDbDataAdapter(comd);
    DataTable dt = new DataTable();
    dt.TableName = "InvoiceEDI";
    sda.Fill(dt);
    result = convertFormat.convertToJson(dt);
    return result;
  }

  public static string createMode(string invoice, string Lineoper) {

    string result = "";
    OleDbConnection con = new OleDbConnection();
    con.ConnectionString = constr.connectionEbs;
    con.Open();
    // OleDbCommand comd = new OleDbCommand("(SELECT 'T7' AgentCode, '' AgentName, 'THLCIT' VendoeCode, 'LEAM CHABANG INTERNATIONAL TERMINAL CO.,LTD' VendorName, INV.INVOICE_AN InvoiceID, to_char(INV.INVOICE_DATE, 'YYYY/MM/DD') InvoideDate, '' DueDate, OCL.BILL_CURRENCY_DS InvoiceCurrency, INV.INVOICE_SUB_TOTAL + inv.INVOICE_TAX_AMOUNT + ROUND((inv.INVOICE_SUB_TOTAL * 0.03 *-1), 2) TOTAL_AMOUNT, to_char(ocl.company_service_group_id) CostCode, to_char(ocl.charge_line_ds) CostDescription, 'THLCBP09' PortTerminalDepot, '' PlaceTo, '' TransportMode, '' EmptyPickUp, '' EmptyReturn, (SELECT TO_CHAR(TCT.DEPARTURE_TM, 'YYYY/MM/DD') FROM TMV_CHARGE_TRANSACTION TCT WHERE TCT.CNTR_CYCLE_ID = NVL((SELECT TO_CHAR(TCH.CNTR_CYCLE_ID) FROM TMS_OWNER.TMS_CNTR_HIST TCH, TMS_CNTR_GRP1 TCG, TMV_VESSEL_VISIT TVV WHERE TCH.CNTR_SEQ = TCG.CNTR_SEQ AND TCG.CNTR_AN = OCL.EQUIPMENT_AN AND TCH.OUT_VESSEL_VISIT_C = TVV.VESSEL_VISIT_C AND ROWNUM <= 1 ), (SELECT TO_CHAR(TCH.CNTR_CYCLE_ID) FROM TMS_OWNER.TMS_CNTR_HIST TCH, TMS_CNTR_GRP1 TCG, TMV_VESSEL_VISIT TVV WHERE TCH.CNTR_SEQ = TCG.CNTR_SEQ AND TCG.CNTR_AN = OCL.EQUIPMENT_AN AND TCH.IN_VESSEL_VISIT_C = TVV.VESSEL_VISIT_C AND ROWNUM <= 1 ) ) AND ROWNUM <= 1) WorkingDate_GateIn, (SELECT TO_CHAR(TCT.DEPARTURE_TM, 'YYYY/MM/DD') FROM TMV_CHARGE_TRANSACTION TCT WHERE TCT.CNTR_CYCLE_ID = NVL((SELECT TO_CHAR(TCH.CNTR_CYCLE_ID) FROM TMS_OWNER.TMS_CNTR_HIST TCH, TMS_CNTR_GRP1 TCG, TMV_VESSEL_VISIT TVV WHERE TCH.CNTR_SEQ = TCG.CNTR_SEQ AND TCG.CNTR_AN = OCL.EQUIPMENT_AN AND TCH.OUT_VESSEL_VISIT_C = TVV.VESSEL_VISIT_C AND ROWNUM <= 1 ), (SELECT TO_CHAR(TCH.CNTR_CYCLE_ID) FROM TMS_OWNER.TMS_CNTR_HIST TCH, TMS_CNTR_GRP1 TCG, TMV_VESSEL_VISIT TVV WHERE TCH.CNTR_SEQ = TCG.CNTR_SEQ AND TCG.CNTR_AN = OCL.EQUIPMENT_AN AND TCH.IN_VESSEL_VISIT_C = TVV.VESSEL_VISIT_C AND ROWNUM <= 1 ) ) AND ROWNUM <= 1) GateOut, '' Freeday, OCL.TOTAL_UNIT Quantity, CASE WHEN OCL.TOTAL_UNIT2 IS NULL THEN OCL.CONTRACT_UNIT_RATE*OCL.TOTAL_UNIT ELSE OCL.CONTRACT_UNIT_RATE*OCL.TOTAL_UNIT * OCL.TOTAL_UNIT2 END UnitPrice, OCL.EQUIPMENT_AN ContainerNo_ChassisNo_BLNo, '' SizeCNTR, '' TypeCNTR, '' Full_Empty, '' OW, '' DG, '' InvoiceVoyage, '' MainVesseVoy, '' port, '' L_D_T, '1' InvoiceConversionRate, '1' SGAConversionRate, OCL.BILL_CURRENCY_DS SGACurrency, CASE WHEN OCL.TOTAL_UNIT2 IS NULL THEN OCL.CONTRACT_UNIT_RATE*OCL.TOTAL_UNIT ELSE OCL.CONTRACT_UNIT_RATE*OCL.TOTAL_UNIT * OCL.TOTAL_UNIT2 END SGAAmount FROM INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '" + invoice + "'AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND ) UNION ALL (SELECT 'T7' AgentCode, '' AgentName, 'THLCIT' VendoeCode, 'LEAM CHABANG INTERNATIONAL TERMINAL CO.,LTD' VendorName, INV.INVOICE_AN InvoiceID, to_char(INV.INVOICE_DATE, 'YYYY/MM/DD') InvoideDate, '' DueDate, inv.BILL_CURRENCY_DS InvoiceCurrency, INV.INVOICE_SUB_TOTAL + inv.INVOICE_TAX_AMOUNT + ROUND((inv.INVOICE_SUB_TOTAL * 0.03 *-1), 2) TOTAL_AMOUNT, 'S012O5' CostCode, 'VAT' CostDescription, 'THLCBP09' PortTerminalDepot, '' PlaceTo, '' TransportMode, '' EmptyPickUp, '' EmptyReturn, '' WorkingDate_GateIn, '' GateOut, '' Freeday, 1 Quantity, inv.INVOICE_TAX_AMOUNT UnitPrice, inv.INVOICE_AN ContainerNo_ChassisNo_BLNo, '' SizeCNTR, '' TypeCNTR, '' Full_Empty, '' OW, '' DG, '' InvoiceVoyage, '' MainVesseVoy, '' port, '' L_D_T, '1' InvoiceConversionRate, '1' SGAConversionRate, inv.BILL_CURRENCY_DS SGACurrency, inv.INVOICE_TAX_AMOUNT SGAAmount FROM INVOICE_V INV WHERE INV.INVOICE_AN = '" + invoice + "') UNION ALL (SELECT 'T7' AgentCode, '' AgentName, 'THLCIT' VendoeCode, 'LEAM CHABANG INTERNATIONAL TERMINAL CO.,LTD' VendorName, INV.INVOICE_AN InvoiceID, to_char(INV.INVOICE_DATE, 'YYYY/MM/DD') InvoideDate, '' DueDate, inv.BILL_CURRENCY_DS InvoiceCurrency, INV.INVOICE_SUB_TOTAL + inv.INVOICE_TAX_AMOUNT + ROUND((inv.INVOICE_SUB_TOTAL * 0.03 *-1), 2) TOTAL_AMOUNT, 'M6B0S13' CostCode, '' CostDescription, 'THLCBP09' PortTerminalDepot, '' PlaceTo, '' TransportMode, '' EmptyPickUp, '' EmptyReturn, '' WorkingDate_GateIn, '' GateOut, '' Freeday, 1 Quantity, ROUND((inv.INVOICE_SUB_TOTAL * 0.03 *-1), 2) UnitPrice, '' ContainerNo_ChassisNo_BLNo, '' SizeCNTR, '' TypeCNTR, '' Full_Empty, '' OW, '' DG, '' InvoiceVoyage, '' MainVesseVoy, '' port, '' L_D_T, '1' InvoiceConversionRate, '1' SGAConversionRate, inv.BILL_CURRENCY_DS SGACurrency, ROUND((inv.INVOICE_SUB_TOTAL * 0.03 *-1), 2) SGAAmount FROM INVOICE_V INV WHERE INV.INVOICE_AN = '" + invoice + "')", con);
    
     OleDbCommand comd = new OleDbCommand("(SELECT 'T7' AgentCode, '' AgentName, 'THLCIT' VendoeCode, 'LEAM CHABANG INTERNATIONAL TERMINAL CO.,LTD' VendorName, INV.INVOICE_AN InvoiceID, to_char(INV.INVOICE_DATE, 'YYYY/MM/DD') InvoideDate, '' DueDate, OCL.BILL_CURRENCY_DS InvoiceCurrency, INV.INVOICE_SUB_TOTAL + inv.INVOICE_TAX_AMOUNT + ROUND((inv.INVOICE_SUB_TOTAL * 0.03 *-1), 2) TOTAL_AMOUNT, to_char(ocl.company_service_group_id) CostCode, to_char(ocl.charge_line_ds) CostDescription, 'THLCBP09' PortTerminalDepot, '' PlaceTo, '' TransportMode, '' EmptyPickUp, '' EmptyReturn, (SELECT TO_CHAR(TCT.DEPARTURE_TM, 'YYYY/MM/DD') FROM TMV_CHARGE_TRANSACTION TCT WHERE TCT.CNTR_CYCLE_ID = NVL((SELECT TO_CHAR(TCH.CNTR_CYCLE_ID) FROM TMS_OWNER.TMS_CNTR_HIST TCH, TMS_CNTR_GRP1 TCG, TMV_VESSEL_VISIT TVV WHERE TCH.CNTR_SEQ = TCG.CNTR_SEQ AND TCG.CNTR_AN = OCL.EQUIPMENT_AN AND TCH.OUT_VESSEL_VISIT_C = TVV.VESSEL_VISIT_C AND ROWNUM <= 1 ), (SELECT TO_CHAR(TCH.CNTR_CYCLE_ID) FROM TMS_OWNER.TMS_CNTR_HIST TCH, TMS_CNTR_GRP1 TCG, TMV_VESSEL_VISIT TVV WHERE TCH.CNTR_SEQ = TCG.CNTR_SEQ AND TCG.CNTR_AN = OCL.EQUIPMENT_AN AND TCH.IN_VESSEL_VISIT_C = TVV.VESSEL_VISIT_C AND ROWNUM <= 1 ) ) AND ROWNUM <= 1) WorkingDate_GateIn, (SELECT TO_CHAR(TCT.DEPARTURE_TM, 'YYYY/MM/DD') FROM TMV_CHARGE_TRANSACTION TCT WHERE TCT.CNTR_CYCLE_ID = NVL((SELECT TO_CHAR(TCH.CNTR_CYCLE_ID) FROM TMS_OWNER.TMS_CNTR_HIST TCH, TMS_CNTR_GRP1 TCG, TMV_VESSEL_VISIT TVV WHERE TCH.CNTR_SEQ = TCG.CNTR_SEQ AND TCG.CNTR_AN = OCL.EQUIPMENT_AN AND TCH.OUT_VESSEL_VISIT_C = TVV.VESSEL_VISIT_C AND ROWNUM <= 1 ), (SELECT TO_CHAR(TCH.CNTR_CYCLE_ID) FROM TMS_OWNER.TMS_CNTR_HIST TCH, TMS_CNTR_GRP1 TCG, TMV_VESSEL_VISIT TVV WHERE TCH.CNTR_SEQ = TCG.CNTR_SEQ AND TCG.CNTR_AN = OCL.EQUIPMENT_AN AND TCH.IN_VESSEL_VISIT_C = TVV.VESSEL_VISIT_C AND ROWNUM <= 1 ) ) AND ROWNUM <= 1) GateOut, '' Freeday, OCL.TOTAL_UNIT Quantity, OCL.CONTRACT_UNIT_RATE UnitPrice, OCL.EQUIPMENT_AN ContainerNo_ChassisNo_BLNo, '' SizeCNTR, '' TypeCNTR, '' Full_Empty, '' OW, '' DG, '' InvoiceVoyage, '' MainVesseVoy, '' port, '' L_D_T, '1' InvoiceConversionRate, '1' SGAConversionRate, OCL.BILL_CURRENCY_DS SGACurrency, CASE WHEN OCL.TOTAL_UNIT2 IS NULL THEN OCL.CONTRACT_UNIT_RATE * OCL.TOTAL_UNIT ELSE OCL.CONTRACT_UNIT_RATE * OCL.TOTAL_UNIT * OCL.TOTAL_UNIT2 END SGAAmount FROM INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '" + invoice + "'AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND ) UNION ALL (SELECT 'T7' AgentCode, '' AgentName, 'THLCIT' VendoeCode, 'LEAM CHABANG INTERNATIONAL TERMINAL CO.,LTD' VendorName, INV.INVOICE_AN InvoiceID, to_char(INV.INVOICE_DATE, 'YYYY/MM/DD') InvoideDate, '' DueDate, inv.BILL_CURRENCY_DS InvoiceCurrency, INV.INVOICE_SUB_TOTAL + inv.INVOICE_TAX_AMOUNT + ROUND((inv.INVOICE_SUB_TOTAL * 0.03 *-1), 2) TOTAL_AMOUNT, 'S012O5' CostCode, 'VAT' CostDescription, 'THLCBP09' PortTerminalDepot, '' PlaceTo, '' TransportMode, '' EmptyPickUp, '' EmptyReturn, '' WorkingDate_GateIn, '' GateOut, '' Freeday, 1 Quantity, inv.INVOICE_TAX_AMOUNT UnitPrice, inv.INVOICE_AN ContainerNo_ChassisNo_BLNo, '' SizeCNTR, '' TypeCNTR, '' Full_Empty, '' OW, '' DG, '' InvoiceVoyage, '' MainVesseVoy, '' port, '' L_D_T, '1' InvoiceConversionRate, '1' SGAConversionRate, inv.BILL_CURRENCY_DS SGACurrency, inv.INVOICE_TAX_AMOUNT SGAAmount FROM INVOICE_V INV WHERE INV.INVOICE_AN = '" + invoice + "') UNION ALL (SELECT 'T7' AgentCode, '' AgentName, 'THLCIT' VendoeCode, 'LEAM CHABANG INTERNATIONAL TERMINAL CO.,LTD' VendorName, INV.INVOICE_AN InvoiceID, to_char(INV.INVOICE_DATE, 'YYYY/MM/DD') InvoideDate, '' DueDate, inv.BILL_CURRENCY_DS InvoiceCurrency, INV.INVOICE_SUB_TOTAL + inv.INVOICE_TAX_AMOUNT + ROUND((inv.INVOICE_SUB_TOTAL * 0.03 *-1), 2) TOTAL_AMOUNT, 'M6B0S13' CostCode, '' CostDescription, 'THLCBP09' PortTerminalDepot, '' PlaceTo, '' TransportMode, '' EmptyPickUp, '' EmptyReturn, '' WorkingDate_GateIn, '' GateOut, '' Freeday, 1 Quantity, ROUND((inv.INVOICE_SUB_TOTAL * 0.03 *-1), 2) UnitPrice, '' ContainerNo_ChassisNo_BLNo, '' SizeCNTR, '' TypeCNTR, '' Full_Empty, '' OW, '' DG, '' InvoiceVoyage, '' MainVesseVoy, '' port, '' L_D_T, '1' InvoiceConversionRate, '1' SGAConversionRate, inv.BILL_CURRENCY_DS SGACurrency, ROUND((inv.INVOICE_SUB_TOTAL * 0.03 *-1), 2) SGAAmount FROM INVOICE_V INV WHERE INV.INVOICE_AN = '" + invoice + "')", con);


    OleDbDataAdapter sda = new OleDbDataAdapter(comd);
    DataTable dt = new DataTable();
    dt.TableName = "Invoice810";
    sda.Fill(dt);
    con.Close();

    string YMCode = "";
    string YMCodeDS = "";

    if (dt.Rows.Count > 0) {
      for (int i = 0; i < dt.Rows.Count - 2; i++) {
        YMCode = MappingCode(dt.Rows[i]["CostCode"].ToString(), dt.Rows[i]["ContainerNo_ChassisNo_BLNo"].ToString(), dt.Rows[i]["InvoiceID"].ToString());
        YMCodeDS = MappingCodeDS(YMCode, statuscheck);
        statuscheck = "";

        dt.Rows[i][9] = YMCode.ToString();
        dt.Rows[i][10] = YMCodeDS.ToString();
      }

    }

    

    dt = dt.AsEnumerable()
      .GroupBy(r => new {
        AGENTCODE = r["AGENTCODE"], AGENTNAME = r["AGENTNAME"], VENDOECODE = r["VENDOECODE"], VENDORNAME = r["VENDORNAME"], INVOICEID = r["INVOICEID"], INVOIDEDATE = r["INVOIDEDATE"], DUEDATE = r["DUEDATE"], INVOICECURRENCY = r["INVOICECURRENCY"], TOTAL_AMOUNT = r["TOTAL_AMOUNT"], COSTCODE = r["COSTCODE"], COSTDESCRIPTION = r["COSTDESCRIPTION"], PORTTERMINALDEPOT = r["PORTTERMINALDEPOT"], PLACETO = r["PLACETO"], TRANSPORTMODE = r["TRANSPORTMODE"], EMPTYPICKUP = r["EMPTYPICKUP"], EMPTYRETURN = r["EMPTYRETURN"], WORKINGDATE_GATEIN = r["WORKINGDATE_GATEIN"], GATEOUT = r["GATEOUT"], FREEDAY = r["FREEDAY"], QUANTITY = r["QUANTITY"], CONTAINERNO_CHASSISNO_BLNO = r["CONTAINERNO_CHASSISNO_BLNO"], SIZECNTR = r["SIZECNTR"], TYPECNTR = r["TYPECNTR"], FULL_EMPTY = r["FULL_EMPTY"], OW = r["OW"], DG = r["DG"], INVOICEVOYAGE = r["INVOICEVOYAGE"], MAINVESSEVOY = r["MAINVESSEVOY"], PORT = r["PORT"], L_D_T = r["L_D_T"], INVOICECONVERSIONRATE = r["INVOICECONVERSIONRATE"], SGACONVERSIONRATE = r["SGACONVERSIONRATE"], SGACURRENCY = r["SGACURRENCY"]
      })
      .Select(g => {

        var row = dt.NewRow();

        row["AGENTCODE"] = g.Key.AGENTCODE;
        row["AGENTNAME"] = g.Key.AGENTNAME;
        row["VENDOECODE"] = g.Key.VENDOECODE;
        row["VENDORNAME"] = g.Key.VENDORNAME;
        row["INVOICEID"] = g.Key.INVOICEID;
        row["INVOIDEDATE"] = g.Key.INVOIDEDATE;
        row["DUEDATE"] = g.Key.DUEDATE;
        row["INVOICECURRENCY"] = g.Key.INVOICECURRENCY;
        row["TOTAL_AMOUNT"] = g.Key.TOTAL_AMOUNT;
        row["COSTCODE"] = g.Key.COSTCODE;
        row["COSTDESCRIPTION"] = g.Key.COSTDESCRIPTION;
        row["PORTTERMINALDEPOT"] = g.Key.PORTTERMINALDEPOT;
        row["PLACETO"] = g.Key.PLACETO;
        row["TRANSPORTMODE"] = g.Key.TRANSPORTMODE;
        row["EMPTYPICKUP"] = g.Key.EMPTYPICKUP;
        row["EMPTYRETURN"] = g.Key.EMPTYRETURN;
        row["WORKINGDATE_GATEIN"] = g.Key.WORKINGDATE_GATEIN;
        row["GATEOUT"] = g.Key.GATEOUT;
        row["FREEDAY"] = g.Key.FREEDAY;
        row["QUANTITY"] = g.Key.QUANTITY;
        row["UNITPRICE"] = g.Sum(r => r.Field < decimal > ("UNITPRICE"));
        row["CONTAINERNO_CHASSISNO_BLNO"] = g.Key.CONTAINERNO_CHASSISNO_BLNO;
        row["SIZECNTR"] = g.Key.SIZECNTR;
        row["TYPECNTR"] = g.Key.TYPECNTR;
        row["FULL_EMPTY"] = g.Key.FULL_EMPTY;
        row["OW"] = g.Key.OW;
        row["DG"] = g.Key.DG;
        row["INVOICEVOYAGE"] = g.Key.INVOICEVOYAGE;
        row["MAINVESSEVOY"] = g.Key.MAINVESSEVOY;
        row["PORT"] = g.Key.PORT;
        row["L_D_T"] = g.Key.L_D_T;
        row["INVOICECONVERSIONRATE"] = g.Key.INVOICECONVERSIONRATE;
        row["SGACONVERSIONRATE"] = g.Key.SGACONVERSIONRATE;
        row["SGACURRENCY"] = g.Key.SGACURRENCY;
        row["SGAAMOUNT"] = g.Sum(r => r.Field < decimal > ("SGAAMOUNT"));

        return row;
      })
      .CopyToDataTable();

    result = convertFormat.convertToJson(dt);

    return result.ToString();
    // return dt;
  }

  public static string MappingCode(String LCITCode, String cntr_no, String invoiceNo) {

    string resultstatus ="";
    SqlConnection conn = new SqlConnection();
    conn.ConnectionString = constr.connectionLCIT_INVOICE_EDI;
    conn.Open();
    SqlDataAdapter sda = new SqlDataAdapter("SELECT LCMP.EXPENSE_ID, LCMP.STATUS FROM LCIT_INVOICE_EDI.dbo.LCIT_CODE_MAPPING AS LCMP WHERE LCMP.LCIT_GROUP_ID = '" + LCITCode + "'", conn);
    DataTable dtLcitCode = new DataTable();

    sda.Fill(dtLcitCode);

    conn.Close();

    if (dtLcitCode.Rows.Count > 1) {

      string result = "";
      OleDbConnection con = new OleDbConnection();
      con.ConnectionString = constr.connectionEbs;
      con.Open();
      OleDbCommand comd = new OleDbCommand("SELECT OCL.EQUIPMENT_LADEN_STATUS || DECODE(OCL.EQUIPMENT_STATUS,'EX','OB','IM','IB','BO','OB','TS','OB','TI','IB','TE','IB') STATUS FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '" + invoiceNo + "'AND OCL.EQUIPMENT_AN LIKE '" + cntr_no + "'AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND", con);

      OleDbDataAdapter sdastatus = new OleDbDataAdapter(comd);

      con.Close();

      DataTable dtstatus = new DataTable();
      sdastatus.Fill(dtstatus);

      conn.Open();
      SqlDataAdapter sdaFINAL = new SqlDataAdapter("SELECT LCMP.EXPENSE_ID, LCMP.STATUS FROM LCIT_INVOICE_EDI.dbo.LCIT_CODE_MAPPING AS LCMP WHERE LCMP.LCIT_GROUP_ID = '" + LCITCode + "' AND LCMP.STATUS LIKE '"+dtstatus.Rows[0]["STATUS"].ToString()+"' ", conn);
      DataTable dtLcitCodeFINAL = new DataTable();

      sdaFINAL.Fill(dtLcitCodeFINAL);

      conn.Close();

      statuscheck = dtLcitCodeFINAL.Rows[0]["STATUS"].ToString();

      resultstatus = dtLcitCodeFINAL.Rows[0]["EXPENSE_ID"].ToString();
    } else {

      statuscheck = dtLcitCode.Rows[0]["STATUS"].ToString();
      resultstatus = dtLcitCode.Rows[0]["EXPENSE_ID"].ToString();

    }

     return resultstatus;


  }

  public static string MappingCodeDS(String YMcode, String status) {

    if (status == "") {
      status = "%";
    }

    SqlConnection conn = new SqlConnection();
    conn.ConnectionString = constr.connectionLCIT_INVOICE_EDI;
    conn.Open();
    SqlDataAdapter sda = new SqlDataAdapter(" WITH CTE_TableName AS(SELECT CT.CUS_TARIFF_CODE, ct.CUS_TARIFF_DE FROM LCIT_INVOICE_EDI.dbo.CUST_TARIFF ct WHERE ct.CUS_TARIFF_CODE LIKE '" + YMcode + "' AND ct.STATUS LIKE '" + status + "' ) SELECT t0.CUS_TARIFF_CODE , STUFF((SELECT ',' + t1.CUS_TARIFF_DE FROM CTE_TableName t1 WHERE t1.CUS_TARIFF_CODE = t0.CUS_TARIFF_CODE ORDER BY t1.CUS_TARIFF_DE FOR XML PATH('')), 1, LEN(','), '') AS CUS_TARIFF_DE FROM CTE_TableName t0 GROUP BY t0.CUS_TARIFF_CODE ORDER BY CUS_TARIFF_CODE", conn);
    DataTable dtLcitCode = new DataTable();
    sda.Fill(dtLcitCode);
    conn.Close();
 
    string result = "";
    try {
      result = dtLcitCode.Rows[0]["CUS_TARIFF_DE"].ToString();
    } catch (Exception ex) {
      result = "Description not found";
    }
    return result;
  }
}