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

namespace inv_edi_system
{
    [WebService(Namespace = "http://lcitedi.lcit.com")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]



    public class inv_edi_control_system : System.Web.Services.WebService
    {
        sqlcon.Connection_String constr = new sqlcon.Connection_String();
        convert_json.jsoncv convertFormat = new convert_json.jsoncv();

        [WebMethod]
        public string linerProfile()
        {
            SqlConnection connectEDISrv = new SqlConnection();
            connectEDISrv.ConnectionString = constr.connectionLCIT_INVOICE_EDI;
            connectEDISrv.Open();
            SqlDataAdapter sdaLineProfile = new SqlDataAdapter("SELECT LINE FROM LCIT_INVOICE_EDI.dbo.CUST_PROFILE; ", connectEDISrv);
            DataTable dtLinerProfile = new DataTable();

            dtLinerProfile.TableName = "LinerProfile";
            sdaLineProfile.Fill(dtLinerProfile);
            connectEDISrv.Close();

            string result = convertFormat.convertToJson(dtLinerProfile);

            return result.ToString();

        }


        [WebMethod] // search invoice
        public string searchInv(String invoice, String Lineoper)
        {
            string result = "";
            OleDbConnection con = new OleDbConnection();
            con.ConnectionString = constr.connectionEbs;
            con.Open();
             OleDbCommand comd = new OleDbCommand("SELECT ROWNUM NO, INV.INVOICE_AN, NVL(VS.VOYAGE_TERMINAL,(SELECT voyage_terminal FROM INVOICE_V WHERE INVOICE_AN = inv.invoice_an)) VOYAGE_TERMINAL, NVL(VS.VESSEL_NM,(SELECT vessel_nm FROM INVOICE_V WHERE INVOICE_AN = inv.invoice_an))VESSEL_NM, OCL.EQUIPMENT_AN, OCL.EQUIPMENT_LENGTH_QT, OCL.EQUIPMENT_TYPE_AN, OCL.EQUIPMENT_LADEN_STATUS , OCL.EQUIPMENT_STATUS, OCL.SERVICE_TYPE_DS, OCL.CHARGE_LINE_DS FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '" + invoice.ToString() + "'AND INV.CONTRACT_COMPANY_NM LIKE '%" + Lineoper.ToString() + "%'AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND", con);
          // OleDbCommand comd = new OleDbCommand("SELECT 'T7' AgentCode, '' AgentName, 'THLCIT' VendoeCode, 'LEAM CHABANG INTERNATIONAL TERMINAL CO.,LTD' VendorName, INV.INVOICE_AN InvoiceID, to_char(INV.INVOICE_DATE, 'YYYY/MM/DD') InvoideDate, '' DueDate, OCL.BILL_CURRENCY_DS InvoiceCurrency, OCL.BILL_UNIT_RATE TOTAL_AMOUNT, to_char(ocl.company_service_group_id) CostCode, ocl.charge_line_ds CostDescription, 'THLCBP09' PortTerminalDepot, '' PlaceTo, '' TransportMode, '' EmptyPickUp, '' EmptyReturn, '' WorkingDate_GateIn, '' GateOut, '' Freeday, OCL.TOTAL_UNIT Quantity, OCL.CONTRACT_UNIT_RATE UnitPrice, OCL.EQUIPMENT_AN ContainerNo_ChassisNo_BLNo, OCL.EQUIPMENT_LENGTH_QT SizeCNTR, OCL.EQUIPMENT_TYPE_AN TypeCNTR, OCL.EQUIPMENT_LADEN_STATUS Full_Empty, '' OW, '' DG, '' InvoiceVoyage, '' MainVesseVoy, '' port, '' L_D_T, '1' InvoiceConversionRate, '1' SGAConversionRate, OCL.BILL_CURRENCY_DS SGACurrency, OCL.CONTRACT_UNIT_RATE SGAAmount FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '" + invoice + "'AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND", con);
            OleDbDataAdapter sda = new OleDbDataAdapter(comd);
            DataTable dt = new DataTable();

            dt.TableName = "Invoice810";
            sda.Fill(dt);
            con.Close();

            result = convertFormat.convertToJson(dt);

            return result.ToString();
        }

        [WebMethod]
        public string createInvoice(String invoice, String Lineoper, String createMode)
        {

             string result = "";

            if (createMode.ToString() == "view")
            {

                Type type = Type.GetType(Lineoper);
                MethodInfo method = type.GetMethod("viewMode");
                result = (string)method.Invoke(this, new object[] { invoice, Lineoper });
            }

            if (createMode.ToString() == "generate")
            {

                Type type = Type.GetType(Lineoper);
                MethodInfo method = type.GetMethod("createMode");
                result = (string)method.Invoke(this, new object[] { invoice, Lineoper });



            }

            return result.ToString();

            // OleDbConnection con = new OleDbConnection();
            // con.ConnectionString = constr.connectionEbs;
            // con.Open();
            // // OleDbCommand comd = new OleDbCommand("SELECT ROWNUM NO, INV.INVOICE_AN, NVL(VS.VOYAGE_TERMINAL,(SELECT voyage_terminal FROM INVOICE_V WHERE INVOICE_AN = inv.invoice_an)) VOYAGE_TERMINAL, NVL(VS.VESSEL_NM,(SELECT vessel_nm FROM INVOICE_V WHERE INVOICE_AN = inv.invoice_an))VESSEL_NM, OCL.EQUIPMENT_AN, OCL.EQUIPMENT_LENGTH_QT, OCL.EQUIPMENT_TYPE_AN, OCL.EQUIPMENT_LADEN_STATUS , OCL.EQUIPMENT_STATUS, OCL.SERVICE_TYPE_DS, OCL.CHARGE_LINE_DS FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '" + invoice.ToString() + "'AND INV.CONTRACT_COMPANY_NM LIKE '%" + Lineoper.ToString() + "%'AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND", con);
            // OleDbCommand comd = new OleDbCommand("SELECT 'T7' AgentCode, '' AgentName, 'THLCIT' VendoeCode, 'LEAM CHABANG INTERNATIONAL TERMINAL CO.,LTD' VendorName, INV.INVOICE_AN InvoiceID, to_char(INV.INVOICE_DATE, 'YYYY/MM/DD') InvoideDate, '' DueDate, OCL.BILL_CURRENCY_DS InvoiceCurrency, OCL.BILL_UNIT_RATE TOTAL_AMOUNT, to_char(ocl.company_service_group_id) CostCode, ocl.charge_line_ds CostDescription, 'THLCBP09' PortTerminalDepot, '' PlaceTo, '' TransportMode, '' EmptyPickUp, '' EmptyReturn, '' WorkingDate_GateIn, '' GateOut, '' Freeday, OCL.TOTAL_UNIT Quantity, OCL.CONTRACT_UNIT_RATE UnitPrice, OCL.EQUIPMENT_AN ContainerNo_ChassisNo_BLNo, OCL.EQUIPMENT_LENGTH_QT SizeCNTR, OCL.EQUIPMENT_TYPE_AN TypeCNTR, OCL.EQUIPMENT_LADEN_STATUS Full_Empty, '' OW, '' DG, '' InvoiceVoyage, '' MainVesseVoy, '' port, '' L_D_T, '1' InvoiceConversionRate, '1' SGAConversionRate, OCL.BILL_CURRENCY_DS SGACurrency, OCL.CONTRACT_UNIT_RATE SGAAmount FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '" + invoice + "'AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND", con);
            // OleDbDataAdapter sda = new OleDbDataAdapter(comd);
            // DataTable dt = new DataTable();

            // dt.TableName = "Invoice810";
            // sda.Fill(dt);
            // con.Close();

            // return convertFormat.convertToJson(dt).ToString();

            
        

        }

         [WebMethod] // gemerate edi file
        public DataTable GenEDIHead(String invoice,String Lineoper)
        { 

             Type type = Type.GetType(Lineoper);
                    MethodInfo method = type.GetMethod("create_Headfile");
              var InvoiceInfo = method.Invoke(this ,new object[] {invoice, Lineoper});

                      using(DataTable dataInvoice = new DataTable())
                    {
                        dataInvoice.TableName = "HEADINVOICE";
                        dataInvoice.Columns.Add("EDI", typeof(string));
                        dataInvoice.Rows.Add();
                        dataInvoice.Rows[0][0]  = InvoiceInfo.ToString();

                        return dataInvoice;
                    }
        }

         [WebMethod] // gemerate edi file
        public DataTable GenEDIDetail(String invoice,String Lineoper)
        { 

             Type type = Type.GetType(Lineoper);
                    MethodInfo method = type.GetMethod("create_Detailfile");
              var InvoiceInfo = method.Invoke(this ,new object[] {invoice, Lineoper});

                      using(DataTable dataInvoice = new DataTable())
                    {
                        dataInvoice.TableName = "DETAILINVOICE";
                        dataInvoice.Columns.Add("EDI", typeof(string));
                        dataInvoice.Rows.Add();
                        dataInvoice.Rows[0][0]  = InvoiceInfo.ToString();

                        return dataInvoice;
                    }
        }  

    }


}
