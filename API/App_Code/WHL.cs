using System;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Configuration;
using System.Text;  
using System.Collections.Generic;
using System.Web.Script.Serialization;

//using System.Web.Helpers;
	public class WHL 
	{

       static string connectionString = "Provider=OraOLEDB.Oracle.1;Password=lcit001$lcit;Persist Security Info=True;User ID=ops$lcit001;Data Source=MISPROD";
       static string connectionCustoms = "Provider=SQLOLEDB;Data Source=lcitdbsrv;Database=Customs_Interchange;User ID=sa;Password=p@ssw0rd;";
       static string connectionEbs = "Provider=OraOLEDB.Oracle.1;Password=lcitprodebsreport;Persist Security Info=True;User ID=OPS$EBS_REPORT;Data Source=TMSPROD";
       static string connectionChargeCode = "Data Source=lcitedisrv.lcit.com;Initial Catalog=INVOICE_CHARGE_CODE;User ID=sa;Password=p@ssw0rd";
       static DirectoryInfo LLS = new DirectoryInfo(@"D:\Invoice_EDI\");
       static int CountCon = 0;
       static int LineCount=0;
       static char[] cut_space = {' ','\t'};


              public static string createMode(string Invoice, string Lineoper)
              {
                string invoice_whl ="";
                   
                     OleDbConnection con = new OleDbConnection();
                     con.ConnectionString = connectionEbs;
                     OleDbCommand comd = new OleDbCommand("SELECT (SUBSTR(RPAD((CASE WHEN INV.COMPANY_CODE = 'C3' THEN 'LCH14' ELSE 'LCH06' END), 5, ' '), 1, 5) || SUBSTR(RPAD('RF', 6, ' '), 1, 6)|| SUBSTR(RPAD(TO_CHAR(OCL.CALCULATE_START_DATE, 'YYYYMMDD'), 8, ' '), 1, 8)|| SUBSTR(RPAD(TO_CHAR(OCL.CALCULATE_END_DATE, 'YYYYMMDD'), 8, ' '), 1, 8) || SUBSTR(RPAD(INV.INVOICE_AN , 25, '       '), 1, 25) || (SUBSTR(RPAD((CASE WHEN INV.COMPANY_CODE = 'C3' THEN 'L004' ELSE 'L025' END), 12, ' '), 1, 12))) WHL FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, TMV_VESSEL_VISIT VS , TMV_CNTR_GRP1 CG WHERE INV.INVOICE_AN = '"+Invoice+"' AND OCL.SERVICE_TYPE_CODE_AN LIKE 'RF'AND INV.invoice_id = CL.invoice_id AND CL.CHARGE_LINE_ID = OCL.CHARGE_LINE_ID AND INV.COMPANY_ID = com.COMPANY_ID AND VS.VESSEL_VISIT_C(+) = CL.VOYAGE_TERMINAL_OUTBOUND AND CG.CNTR_AN = CL.EQUIPMENT_AN AND CG.DEPARTURE_TM IS NOT NULL AND ROWNUM <= 1 UNION SELECT (SUBSTR(RPAD(CL.EQUIPMENT_AN, 12, ' '), 1, 12) || SUBSTR(RPAD((CASE WHEN OCL.EQUIPMENT_LENGTH_QT = '20' THEN '2' WHEN OCL.EQUIPMENT_LENGTH_QT = '40' THEN '4' WHEN OCL.EQUIPMENT_LENGTH_QT = '45' THEN 'L' END), 1, ' '), 1, 1) || SUBSTR(RPAD((CASE WHEN CG.SIZETYPE_HEIGHT_AN = '86' THEN '2' WHEN CG.SIZETYPE_HEIGHT_AN = '96' THEN '5' END), 1, ' '), 1, 1) || SUBSTR(RPAD((CASE WHEN CG.CNTR_LOAD_STATUS_CODE = 'F' THEN 'F' WHEN CG.CNTR_LOAD_STATUS_CODE = 'E' THEN 'E' END), 1, ' '), 1, 1) || SUBSTR(RPAD((CASE WHEN CG.SHIPPING_STATUS_CODE = 'EX' THEN 'E' WHEN CG.SHIPPING_STATUS_CODE = 'IM' THEN 'I' WHEN CG.SHIPPING_STATUS_CODE = 'TS' THEN 'T' END), 2, ' '), 1, 2) || SUBSTR(RPAD(TO_CHAR(CG.ARRIVAL_TM, 'YYYYMMDDHH24MI'), 12, ' '), 1, 12)|| SUBSTR(RPAD(TO_CHAR(CG.DEPARTURE_TM, 'YYYYMMDDHH24MI'), 12, ' '), 1, 12)|| SUBSTR(RPAD(INV.BILL_CURRENCY_DS , 3, '       '), 1, 3) || SUBSTR(LPAD('0' , 8, '       '), 1, 8) || SUBSTR(LPAD('0' , 8, '       '), 1, 8) || SUBSTR(LPAD('0' , 8, '       '), 1, 8) || SUBSTR(LPAD('0' , 8, '       '), 1, 8) || SUBSTR(LPAD('0' , 8, '       '), 1, 8) || SUBSTR(LPAD((OCL.TOTAL_UNIT*OCL.CONTRACT_UNIT_RATE) , 8, '       '), 1, 8) || SUBSTR(LPAD((OCL.TOTAL_UNIT*OCL.CONTRACT_UNIT_RATE) , 9, '       '), 1, 9)) WHL FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, TMV_VESSEL_VISIT VS , TMV_CNTR_GRP1 CG WHERE INV.INVOICE_AN = '"+Invoice+"' AND OCL.SERVICE_TYPE_CODE_AN LIKE 'RF'AND INV.invoice_id = CL.invoice_id AND CL.CHARGE_LINE_ID = OCL.CHARGE_LINE_ID AND INV.COMPANY_ID = com.COMPANY_ID AND VS.VESSEL_VISIT_C(+) = CL.VOYAGE_TERMINAL_OUTBOUND AND CG.CNTR_AN = CL.EQUIPMENT_AN AND CG.DEPARTURE_TM IS NOT NULL", con);

                      OleDbDataAdapter sda = new OleDbDataAdapter(comd);
                       DataTable dt = new DataTable();
                       dt.TableName = "EDI_WHL";
                       sda.Fill(dt);

                       for(int i =0 ; i<dt.Rows.Count;i++)
                       {
                           invoice_whl += dt.Rows[i]["WHL"].ToString()+"\n";
                       }
                       



                     // return dt.Row[0]["WHL"].ToString();
                     // return dt.Rows[0]["WHL"].ToString()

                       // string EDI_INV = dt.Rows[0][0].ToString();
                       return invoice_whl;
              }

       }