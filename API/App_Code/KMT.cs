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
	public class KMT 
	{

       static string connectionString = "Provider=OraOLEDB.Oracle.1;Password=lcit001$lcit;Persist Security Info=True;User ID=ops$lcit001;Data Source=MISPROD";
       static string connectionCustoms = "Provider=SQLOLEDB;Data Source=lcitdbsrv;Database=Customs_Interchange;User ID=sa;Password=p@ssw0rd;";
       static string connectionEbs = "Provider=OraOLEDB.Oracle.1;Password=lcitprodebsreport;Persist Security Info=True;User ID=OPS$EBS_REPORT;Data Source=TMSPROD";
       static string connectionInvProfile = "Data Source=lcitedisrv.lcit.com;Initial Catalog=INVOICE_CHARGE_CODE;User ID=sa;Password=p@ssw0rd";
       static DirectoryInfo LLS = new DirectoryInfo(@"D:\Invoice_EDI_TEST\");
       static int FileRunNum;
       static char[] cut_space = {' ','\t','	'};



	   public static string create_Headfile(string Invoice, string Lineoper){

// File LRunning number
	   	SqlConnection connectInv = new  SqlConnection();
    	connectInv.ConnectionString = connectionInvProfile;
    	connectInv.Open();
    	SqlDataAdapter Invsda = new SqlDataAdapter("SELECT RUNNING_NUMBER FROM INVOICE_CHARGE_CODE.DBO.TEST_PROFILE_RUNNING WHERE LINE_ID = 'KMT'",connectInv);
    	DataTable dtInv = new DataTable();
    	Invsda.Fill(dtInv);

    	FileRunNum = int.Parse(dtInv.Rows[0]["RUNNING_NUMBER"].ToString());

    	connectInv.Close();
// Query EDI Header file		
		string Invoice_data_final="";
		OleDbConnection con = new OleDbConnection();
        con.ConnectionString = connectionEbs;
        OleDbCommand comd = new OleDbCommand("SELECT 'IBH' AS MESSAGE_HEADER, '00001' AS MESSAGE_SEQUENCE, SUBSTR(RPAD(INV.INVOICE_AN,15,' '),1,15) AS INVOICE_NUMBER, SUBSTR(RPAD(to_char(INV.INVOICE_DATE,'YYYYMMDD'),8,' '),1,8) AS INVOICE_DATE, SUBSTR(RPAD('VSLCHG',12,' '),1,12) AS MESSAGE_ID, 'O' AS DOCUMENT_TYPE, 'Laem Chabang International Terminal Co.Ltd Port '||INV.COMPANY_CODE AS SENDER_NAME, SUBSTR(RPAD('Thungsukla Sriracha Chonburi 20231',50,' '),1,50) AS TERMIANL_ADDRESS1, SUBSTR(RPAD('KMTC',50,' '),1,50) AS RECEIPIENT_NAME, SUBSTR(RPAD(INV.VOYAGE_TERMINAL,40,' '),1,40) AS LINE_VESSEL_CODE, SUBSTR(RPAD(INV.VOYAGE_EXTERNAL,40,' '),1,40) AS LINE_VOYAGE_CODE, SUBSTR(RPAD(INV.VESSEL_NM,30,' '),1,30) AS VESSEL_NAME, 'THB' AS CURRENCY_CODE, SUBSTR(RPAD((INV.INVOICE_SUB_TOTAL+INV.INVOICE_TAX_AMOUNT),13,' '),1,13) AS TOTAL_AMOUNT, SUBSTR(RPAD((SELECT  COUNT(DISTINCT OCL.SERVICE_TYPE_ID) FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '"+Invoice+"'AND INV.CONTRACT_COMPANY_NM LIKE '%KMT%'AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND),4,' '),1,4) AS TOTAL_NUMBER_OF_ITEMS,	SUBSTR(RPAD((SELECT 	DECODE(ocl.equipment_status, 'BI', 'EX', 'BO', 'IM', 'BT', 'EX','', DECODE(inv.invoice_type_ds, 'BACK TO TOWN', 'EX'),ocl.equipment_status) AS equipment_status FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '"+Invoice+"'AND INV.CONTRACT_COMPANY_NM LIKE '%KMT%'AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND GROUP BY ocl.EQUIPMENT_STATUS,inv.invoice_type_ds ),2,' '),1,2) AS  INOUT_INDICATOR  FROM	EBS_OWNER.INVOICE_V INV WHERE INV.INVOICE_AN = '"+Invoice+"'", con); 
 // Query EDI bidy file
        OleDbCommand comddetail = new OleDbCommand("SELECT 'IBH' AS MESSAGE_HEADER, SUBSTR(LPAD(ROW_NUMBER() OVER (PARTITION BY INV.INVOICE_AN ORDER BY OCL.SERVICE_TYPE_ID),5,'0'),1,5) AS MESSAGE_SEQUENCE, SUBSTR(RPAD(INV.INVOICE_AN,15,' '),1,15) AS INVOICE_NUMBER, SUBSTR(RPAD(to_char(INV.INVOICE_DATE,'YYYYMMDD'),8,' '),1,8) AS INVOICE_DATE, SUBSTR(RPAD('VSLCHG',12,' '),1,12) AS MESSAGE_ID, SUBSTR(RPAD(ocl.company_service_group_id,33,' '),1,33) AS TARIFF_CODE, SUBSTR(RPAD(ocl.charge_line_ds,80,' '),1,80) AS TARIFF_1ST_DESCRIPTION, SUBSTR(RPAD(COUNT(ocl.company_service_group_id),8,' '),1,8) AS QUANTITY, SUBSTR(RPAD(ocl.contract_unit_rate,13,' '),1,13) AS RATE FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '"+Invoice+"' AND INV.CONTRACT_COMPANY_NM LIKE '%KMT%'AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND GROUP BY 'IBH',INV.INVOICE_AN,OCL.SERVICE_TYPE_ID,INV.INVOICE_DATE,'VSLCHG',ocl.company_service_group_id,ocl.charge_line_ds,ocl.contract_unit_rate", con); 
        OleDbDataAdapter sda = new OleDbDataAdapter(comd);
        OleDbDataAdapter sdadetail = new OleDbDataAdapter(comddetail);

        	DataTable EdiHeader = new DataTable();
            EdiHeader.TableName = "Invoice810Header";
            sda.Fill(EdiHeader);
			
			if(EdiHeader.Rows.Count >0)
			{			          
	            DataTable EdiHeaderDetail = new DataTable();
	            EdiHeader.TableName = "Invoice810Detail";
	            sdadetail.Fill(EdiHeaderDetail);
	   	
	             string SenderId = "LCIT";
	             string ReceiveId = "KMTC";
	             string RunningNo = "";
	            
	             string OPS ="";
	             string VesselName ="";
	       
	             

	             string eqst = "";

// Generage Header Invoice EDI file
				if(EdiHeader.Rows[0]["INOUT_INDICATOR"].ToString() == "EX")
				{
					eqst = "O";
				}
				else if(EdiHeader.Rows[0]["INOUT_INDICATOR"].ToString() == "IM")
				{
					eqst = "I";
				}  	 

			     string Invoice_data_header =	EdiHeader.Rows[0]["MESSAGE_HEADER"].ToString()+
			     								EdiHeader.Rows[0]["MESSAGE_SEQUENCE"].ToString()+
			     								EdiHeader.Rows[0]["INVOICE_NUMBER"].ToString()+
			     								EdiHeader.Rows[0]["INVOICE_DATE"].ToString()+
			     								EdiHeader.Rows[0]["MESSAGE_ID"].ToString()+
			     								EdiHeader.Rows[0]["DOCUMENT_TYPE"].ToString()+
			     								FileRunNum.ToString("00000")+
			     								SenderId+
			     								ReceiveId+
			     								DateTime.Now.ToString("yyyyMMdd")+
			     								DateTime.Now.ToString("HHmmss")+
			     								EdiHeader.Rows[0]["SENDER_NAME"].ToString()+
			     								EdiHeader.Rows[0]["TERMIANL_ADDRESS1"].ToString()+
			     								"                                                  "+
			     								EdiHeader.Rows[0]["RECEIPIENT_NAME"].ToString()+
			     								"                                                  "+
			     								"                                                  "+
			     								"        "+
			     								EdiHeader.Rows[0]["LINE_VESSEL_CODE"].ToString()+
			     								EdiHeader.Rows[0]["LINE_VOYAGE_CODE"].ToString()+
			     								EdiHeader.Rows[0]["VESSEL_NAME"].ToString()+
			     								EdiHeader.Rows[0]["CURRENCY_CODE"].ToString()+
			     								eqst+
			     								EdiHeader.Rows[0]["TOTAL_AMOUNT"].ToString()+
			     								EdiHeader.Rows[0]["TOTAL_NUMBER_OF_ITEMS"].ToString()+"\n";
// Call Method for generate content detail 
			     
		         string Invoice_data_Hdetail="";  
		         		for(int i=0 ; i < EdiHeaderDetail.Rows.Count;i++)
		         		{
	         		 	Invoice_data_Hdetail+= 	EdiHeaderDetail.Rows[i]["MESSAGE_HEADER"].ToString()+					 
	         		 							EdiHeaderDetail.Rows[i]["MESSAGE_SEQUENCE"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["INVOICE_NUMBER"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["INVOICE_DATE"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["MESSAGE_ID"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["TARIFF_CODE"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["TARIFF_1ST_DESCRIPTION"].ToString()+
	         							 		"        "+
	         							 		EdiHeaderDetail.Rows[i]["QUANTITY"].ToString()+
	         							 		"        "+
	         							 		"        "+
	         							 		EdiHeaderDetail.Rows[i]["RATE"].ToString()+
	         							 		"             "+
	         							 		"             "+
	         							 		"        "+
	         							 		"      "+
	         							 		"        "+
	         							 		"      "+
	         							 		double.Parse(EdiHeaderDetail.Rows[i]["QUANTITY"].ToString())*double.Parse(EdiHeaderDetail.Rows[i]["RATE"].ToString())+"\n";

				 		}
				 Invoice_data_final = Invoice_data_header+Invoice_data_Hdetail;
			}

			return Invoice_data_final; // Return EDI data to web service

		}


 public static string create_Detailfile(string Invoice, string Lineoper){
				string Invoice_data_final="";

// Query EDI Header file

		OleDbConnection con = new OleDbConnection();
        con.ConnectionString = connectionEbs;
        	OleDbCommand comd = new OleDbCommand("SELECT 'IAH' AS MESSAGE_HEADER, '00001' AS MESSAGE_SEQUENCE, SUBSTR(RPAD(INV.INVOICE_AN, 15, ' '), 1, 15) AS INVOICE_NUMBER, SUBSTR(RPAD(to_char(INV.INVOICE_DATE, 'YYYYMMDD'), 8, ' '), 1, 8) AS INVOICE_DATE, SUBSTR(RPAD('VSLCHG', 6, ' '), 1, 6) AS MESSAGE_ID, 'LCIT' AS SENDER_ID, 'KMTC' AS RECEIPIENT_ID, SUBSTR(RPAD('XXXXXX', 6, ' '), 1, 6) AS MESSAGE_TYPE, SUBSTR(RPAD((SELECT COUNT(OCL.SERVICE_TYPE_ID) FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '"+Invoice+"'AND INV.CONTRACT_COMPANY_NM LIKE '%KMT%'AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND), 4, ' '), 1, 4) AS TOTAL_NUMBER_OF_RECORD FROM EBS_OWNER.INVOICE_V INV WHERE INV.INVOICE_AN = '"+Invoice+"'", con);
         	OleDbDataAdapter sda = new OleDbDataAdapter(comd);

        	DataTable EdiHeader = new DataTable();
            EdiHeader.TableName = "Invoice810Header";
            sda.Fill(EdiHeader);
 // Query EDI bidy file


       		OleDbCommand comddetail = new OleDbCommand("SELECT 'IAD' AS MESSAGE_HEADER, SUBSTR(LPAD(ROW_NUMBER() OVER (PARTITION BY INV.INVOICE_AN ORDER BY OCL.SERVICE_TYPE_ID), 5, '0'), 1, 5) AS MESSAGE_SEQUENCE, SUBSTR(RPAD(INV.INVOICE_AN, 15, ' '), 1, 15) AS INVOICE_NUMBER, SUBSTR(RPAD(to_char(INV.INVOICE_DATE, 'YYYYMMDD'), 8, ' '), 1, 8) AS INVOICE_DATE, SUBSTR(RPAD('VSLCHG', 6, ' '), 1, 6) AS MESSAGE_ID, SUBSTR(RPAD(ocl.company_service_group_id, 33, ' '), 1, 33) AS TARIFF_CODE, SUBSTR(RPAD(ocl.charge_line_ds, 80, ' '), 1, 80) AS TARIFF_1ST_DESCRIPTION, SUBSTR(RPAD(ocl.equipment_an , 13, ' '), 1, 13) AS CONTAINER_NUMBER, SUBSTR(RPAD(NVL(to_char(OCL.ACTUAL_START_DATE, 'YYYYMMDD'), ' '), 8, ' '), 1, 8) AS MOVEMENT_DATE , SUBSTR(RPAD(NVL(to_char(OCL.ACTUAL_START_DATE, 'HH24MISS'), ' '), 6, ' '), 1, 6) AS MOVEMENT_TIME , SUBSTR(RPAD(INV.VESSEL_NM, 30, ' '), 1, 30) AS VESSEL_NAME, SUBSTR(RPAD(INV.VOYAGE_TERMINAL, 40, ' '), 1, 40) AS LINE_VESSEL_CODE, SUBSTR(RPAD(INV.VOYAGE_EXTERNAL, 40, ' '), 1, 40) AS LINE_VOYAGE_CODE, SUBSTR(RPAD(NVL(to_char(OCL.CALCULATE_START_DATE, 'YYYYMMDD'), ' '), 8, ' '), 1, 8) AS STORAGE_FROM_DATE , SUBSTR(RPAD(NVL(to_char(OCL.CALCULATE_START_DATE, 'HH24MISS'), ' '), 6, ' '), 1, 6) AS STORAGE_FROM_TIME , SUBSTR(RPAD(NVL(to_char(OCL.CALCULATE_END_DATE, 'YYYYMMDD'), ' '), 8, ' '), 1, 8) AS STORAGE_TO_DATE , SUBSTR(RPAD(NVL(to_char(OCL.CALCULATE_END_DATE, 'HH24MISS'), ' '), 6, ' '), 1, 6) AS STORAGE_TO_TIME, SUBSTR(RPAD(decode(OCL.EQUIPMENT_TYPE_AN, '2210', '20GP', '2200', '20GP', '2250', '20GP', '22G1', '20GP', '2230', '20RF', '2251', '20OT', '2263', '20FR', '22T6', '20TK', '2500', '20HQ', '4300', '40GP', '4310', '40GP', '4351', '40OT', '4250', '40OT', '4363', '40FR', '4364', '40FR', '4351', '40OT', '4500', '40HQ', '4530', '40RH', '4532', '40RH', '45R0', '40RH', '45R1', '40RH', '4550', '45OT', OCL.EQUIPMENT_TYPE_AN) , 4, ' '), 1, 4) AS CONTAINER_INDICATOR FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '"+Invoice+"'AND INV.CONTRACT_COMPANY_NM LIKE '%KMT%'AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND", con);

        OleDbDataAdapter sdadetail = new OleDbDataAdapter(comddetail);
// Generage Header Invoice EDI file
				if(EdiHeader.Rows.Count >0)
			{

				DataTable EdiHeaderDetail = new DataTable();
	            EdiHeader.TableName = "Invoice810Detail";
	            sdadetail.Fill(EdiHeaderDetail);

			     string Invoice_data_header =	EdiHeader.Rows[0]["MESSAGE_HEADER"].ToString()+
			     								EdiHeader.Rows[0]["MESSAGE_SEQUENCE"].ToString()+
			     								EdiHeader.Rows[0]["INVOICE_NUMBER"].ToString()+
			     								EdiHeader.Rows[0]["INVOICE_DATE"].ToString()+
			     								EdiHeader.Rows[0]["MESSAGE_ID"].ToString()+
			     								EdiHeader.Rows[0]["SENDER_ID"].ToString()+
			     								EdiHeader.Rows[0]["RECEIPIENT_ID"].ToString()+
			     								DateTime.Now.ToString("yyyyMMdd")+
			     								DateTime.Now.ToString("HHmmss")+
			     								EdiHeader.Rows[0]["MESSAGE_TYPE"].ToString()+
			     								EdiHeader.Rows[0]["TOTAL_NUMBER_OF_RECORD"].ToString()+
			     								" \n";
// Call Method for generate content detail 
			     
		         string Invoice_data_Hdetail = ""; 

		         		for(int i=0 ; i < EdiHeaderDetail.Rows.Count;i++)
		         		{
	         		 	Invoice_data_Hdetail+= 	EdiHeaderDetail.Rows[i]["MESSAGE_HEADER"].ToString()+					 
	         		 							EdiHeaderDetail.Rows[i]["MESSAGE_SEQUENCE"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["INVOICE_NUMBER"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["INVOICE_DATE"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["MESSAGE_ID"].ToString()+
	         							 		"      "+
	         							 		EdiHeaderDetail.Rows[i]["TARIFF_CODE"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["TARIFF_1ST_DESCRIPTION"].ToString()+
	         							 		" "+
	         							 		"    "+
	         							 		EdiHeaderDetail.Rows[i]["CONTAINER_NUMBER"].ToString()+
	         							 		" "+
	         							 		EdiHeaderDetail.Rows[i]["MOVEMENT_DATE"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["MOVEMENT_TIME"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["VESSEL_NAME"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["LINE_VESSEL_CODE"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["LINE_VOYAGE_CODE"].ToString()+
	         							 		" "+
	         							 		EdiHeaderDetail.Rows[i]["STORAGE_FROM_DATE"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["STORAGE_FROM_TIME"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["STORAGE_TO_DATE"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["STORAGE_TO_TIME"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["CONTAINER_INDICATOR"].ToString()+
	         							 		"     "+
	         							 		"     "+
	         							 		"     "+
	         							 		"     "+
	         							 		"   "+
	         							 		"     "+
	         							 		"        "+
	         							 		"      "+
	         							 		"                    "+
	         							 		"                    "+
	         							 		"          "+
	         							 		"                    "+
	         							 		"     "+
	         							 		"                              "+
	         							 		"\n";
				 		}
		         		
				 	Invoice_data_final = Invoice_data_header+Invoice_data_Hdetail;
			 }

			return Invoice_data_final; // Return EDI data to web service

		}
		public static DataTable iso_check(string a)
		{
// Method for check ISO code for show container size and type			
			OleDbConnection con = new OleDbConnection(connectionString);
         	OleDbDataAdapter sda = new OleDbDataAdapter("SELECT (SIZETYPE_SIZE_AN||SIZETYPE_TYPE_AN) SITY FROM MIS_OWNER.TMS_CONTAINER_TYPE_SNAP WHERE CONTAINER_TYPE_C ='"+a+"'",con);
         	DataTable dt = new DataTable();
         	dt.TableName = "TestISO";
         	sda.Fill(dt); 	
			return dt;
		}


			public static string create_file_manual_header(string Invoice, string Lineoper, String CNTR,String[] KMT_CODE)
    		{
    	
// File LRunning number
	   	SqlConnection connectInv = new  SqlConnection();
    	connectInv.ConnectionString = connectionInvProfile;
    	connectInv.Open();
    	SqlDataAdapter Invsda = new SqlDataAdapter("SELECT RUNNING_NUMBER FROM INVOICE_CHARGE_CODE.DBO.TEST_PROFILE_RUNNING WHERE LINE_ID = 'KMT'",connectInv);
    	DataTable dtInv = new DataTable();
    	Invsda.Fill(dtInv);

    	FileRunNum = int.Parse(dtInv.Rows[0]["RUNNING_NUMBER"].ToString());

    	connectInv.Close();

    	string container_no ="";
    	char[] cut_string = {'[',']'};

    	CNTR = CNTR.Trim(cut_string).Replace("\"","'").Replace("-","");
 				container_no = CNTR;
// Query EDI Header file		
		string Invoice_data_final="";
		OleDbConnection con = new OleDbConnection();
        con.ConnectionString = connectionEbs;
        OleDbCommand comd = new OleDbCommand("SELECT 'IBH' AS MESSAGE_HEADER, '00001' AS MESSAGE_SEQUENCE, SUBSTR(RPAD(INV.INVOICE_AN, 15, ' '), 1, 15) AS INVOICE_NUMBER, SUBSTR(RPAD(to_char(INV.INVOICE_DATE, 'YYYYMMDD'), 8, ' '), 1, 8) AS INVOICE_DATE, SUBSTR(RPAD('VSLCHG', 12, ' '), 1, 12) AS MESSAGE_ID, 'O' AS DOCUMENT_TYPE, 'Laem Chabang International Terminal Co.Ltd Port ' || INV.COMPANY_CODE AS SENDER_NAME, SUBSTR(RPAD('Thungsukla Sriracha Chonburi 20231', 50, ' '), 1, 50) AS TERMIANL_ADDRESS1, SUBSTR(RPAD('KMTC', 50, ' '), 1, 50) AS RECEIPIENT_NAME, SUBSTR(RPAD(INV.VOYAGE_TERMINAL, 40, ' '), 1, 40) AS LINE_VESSEL_CODE, SUBSTR(RPAD(INV.VOYAGE_EXTERNAL, 40, ' '), 1, 40) AS LINE_VOYAGE_CODE, SUBSTR(RPAD(INV.VESSEL_NM, 30, ' '), 1, 30) AS VESSEL_NAME, 'THB' AS CURRENCY_CODE, SUBSTR(RPAD((INV.INVOICE_SUB_TOTAL + INV.INVOICE_TAX_AMOUNT), 13, ' '), 1, 13) AS TOTAL_AMOUNT, SUBSTR(RPAD((SELECT COUNT(DISTINCT OCL.SERVICE_TYPE_ID) FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '"+Invoice+"' AND INV.CONTRACT_COMPANY_NM LIKE '%KMT%' AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND), 4, ' '), 1, 4) AS TOTAL_NUMBER_OF_ITEMS, SUBSTR(RPAD((SELECT NVL(DECODE(ocl.equipment_status, 'BI', 'EX', 'BO', 'IM', 'BT', 'EX', '', DECODE(inv.invoice_type_ds, 'BACK TO TOWN', 'EX'), ocl.equipment_status),(SELECT CATEGORY FROM TMS_OWNER.TMS_CNTR_GRP1 tcg WHERE CNTR_AN IN ("+CNTR+") GROUP BY CATEGORY)) AS equipment_status FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '"+Invoice+"' AND INV.CONTRACT_COMPANY_NM LIKE '%KMT%' AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND GROUP BY ocl.EQUIPMENT_STATUS, inv.invoice_type_ds ), 2, ' '), 1, 2) AS INOUT_INDICATOR FROM EBS_OWNER.INVOICE_V INV WHERE INV.INVOICE_AN = '"+Invoice+"'", con); 
 // Query EDI bidy file
        OleDbCommand comddetail = new OleDbCommand("SELECT 'IBH' AS MESSAGE_HEADER, SUBSTR(LPAD(ROW_NUMBER() OVER (PARTITION BY INV.INVOICE_AN ORDER BY OCL.SERVICE_TYPE_ID),5,'0'),1,5) AS MESSAGE_SEQUENCE, SUBSTR(RPAD(INV.INVOICE_AN,15,' '),1,15) AS INVOICE_NUMBER, SUBSTR(RPAD(to_char(INV.INVOICE_DATE,'YYYYMMDD'),8,' '),1,8) AS INVOICE_DATE, SUBSTR(RPAD('VSLCHG',12,' '),1,12) AS MESSAGE_ID, SUBSTR(RPAD(ocl.company_service_group_id,33,' '),1,33) AS TARIFF_CODE, SUBSTR(RPAD(ocl.charge_line_ds,80,' '),1,80) AS TARIFF_1ST_DESCRIPTION, SUBSTR(RPAD(COUNT(ocl.company_service_group_id),8,' '),1,8) AS QUANTITY, SUBSTR(RPAD(ocl.contract_unit_rate,13,' '),1,13) AS RATE FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '"+Invoice+"' AND INV.CONTRACT_COMPANY_NM LIKE '%KMT%'AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND GROUP BY 'IBH',INV.INVOICE_AN,OCL.SERVICE_TYPE_ID,INV.INVOICE_DATE,'VSLCHG',ocl.company_service_group_id,ocl.charge_line_ds,ocl.contract_unit_rate", con); 
        OleDbDataAdapter sda = new OleDbDataAdapter(comd);
        OleDbDataAdapter sdadetail = new OleDbDataAdapter(comddetail);

        	DataTable EdiHeader = new DataTable();
            EdiHeader.TableName = "Invoice810Header";
            sda.Fill(EdiHeader);
			
			if(EdiHeader.Rows.Count >0)
			{			          
	            DataTable EdiHeaderDetail = new DataTable();
	            EdiHeader.TableName = "Invoice810Detail";
	            sdadetail.Fill(EdiHeaderDetail);
	   	
	             string SenderId = "LCIT";
	             string ReceiveId = "KMTC";
	             string RunningNo = "";
	            
	             string OPS ="";
	             string VesselName ="";


			     string Invoice_data_header =	EdiHeader.Rows[0]["MESSAGE_HEADER"].ToString()+
			     								EdiHeader.Rows[0]["MESSAGE_SEQUENCE"].ToString()+
			     								EdiHeader.Rows[0]["INVOICE_NUMBER"].ToString()+
			     								EdiHeader.Rows[0]["INVOICE_DATE"].ToString()+
			     								EdiHeader.Rows[0]["MESSAGE_ID"].ToString()+
			     								EdiHeader.Rows[0]["DOCUMENT_TYPE"].ToString()+
			     								FileRunNum.ToString("00000")+
			     								SenderId+
			     								ReceiveId+
			     								DateTime.Now.ToString("yyyyMMdd")+
			     								DateTime.Now.ToString("HHmmss")+
			     								EdiHeader.Rows[0]["SENDER_NAME"].ToString()+
			     								EdiHeader.Rows[0]["TERMIANL_ADDRESS1"].ToString()+
			     								"                                                  "+
			     								EdiHeader.Rows[0]["RECEIPIENT_NAME"].ToString()+
			     								"                                                  "+
			     								"                                                  "+
			     								"        "+
			     								EdiHeader.Rows[0]["LINE_VESSEL_CODE"].ToString()+
			     								EdiHeader.Rows[0]["LINE_VOYAGE_CODE"].ToString()+
			     								EdiHeader.Rows[0]["VESSEL_NAME"].ToString()+
			     								EdiHeader.Rows[0]["CURRENCY_CODE"].ToString()+
			     								EdiHeader.Rows[0]["INOUT_INDICATOR"].ToString().Substring(0,1)+
			     								EdiHeader.Rows[0]["TOTAL_AMOUNT"].ToString()+
			     								EdiHeader.Rows[0]["TOTAL_NUMBER_OF_ITEMS"].ToString()+"\n";
// Call Method for generate content detail 
			     
		         string Invoice_data_Hdetail="";  
		         		for(int i=0 ; i < EdiHeaderDetail.Rows.Count;i++)
		         		{
	         		 	Invoice_data_Hdetail+= 	EdiHeaderDetail.Rows[i]["MESSAGE_HEADER"].ToString()+					 
	         		 							EdiHeaderDetail.Rows[i]["MESSAGE_SEQUENCE"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["INVOICE_NUMBER"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["INVOICE_DATE"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["MESSAGE_ID"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["TARIFF_CODE"].ToString()+
	         							 		EdiHeaderDetail.Rows[i]["TARIFF_1ST_DESCRIPTION"].ToString()+
	         							 		"        "+
	         							 		EdiHeaderDetail.Rows[i]["QUANTITY"].ToString()+
	         							 		"        "+
	         							 		"        "+
	         							 		EdiHeaderDetail.Rows[i]["RATE"].ToString()+
	         							 		"             "+
	         							 		"             "+
	         							 		"        "+
	         							 		"      "+
	         							 		"        "+
	         							 		"      "+
	         							 		double.Parse(EdiHeaderDetail.Rows[i]["QUANTITY"].ToString())*double.Parse(EdiHeaderDetail.Rows[i]["RATE"].ToString())+"\n";

				 		}
				 Invoice_data_final = Invoice_data_header+Invoice_data_Hdetail;
			}

			return Invoice_data_final; // Return EDI data to web service

    		}


    		public static string create_file_manual_detail(string Invoice, string Lineoper, String[] CNTR,String[] KMT_CODE)
    		{
    	
// File LRunning number
	  

    	var stringArray = CNTR[0].Split('"');
    	var stringArray2 = KMT_CODE[0].Split('"');
// Query EDI Header file		
		string Invoice_data_final="";
		OleDbConnection con = new OleDbConnection();
        con.ConnectionString = connectionEbs;
        	OleDbCommand comd = new OleDbCommand("SELECT 'IAH' AS MESSAGE_HEADER, '00001' AS MESSAGE_SEQUENCE, SUBSTR(RPAD(INV.INVOICE_AN, 15, ' '), 1, 15) AS INVOICE_NUMBER, SUBSTR(RPAD(to_char(INV.INVOICE_DATE, 'YYYYMMDD'), 8, ' '), 1, 8) AS INVOICE_DATE, SUBSTR(RPAD('VSLCHG', 6, ' '), 1, 6) AS MESSAGE_ID, 'LCIT' AS SENDER_ID, 'KMTC' AS RECEIPIENT_ID, SUBSTR(RPAD('XXXXXX', 6, ' '), 1, 6) AS MESSAGE_TYPE, SUBSTR(RPAD((SELECT COUNT(OCL.SERVICE_TYPE_ID) FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '"+Invoice+"'AND INV.CONTRACT_COMPANY_NM LIKE '%KMT%'AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND), 4, ' '), 1, 4) AS TOTAL_NUMBER_OF_RECORD FROM EBS_OWNER.INVOICE_V INV WHERE INV.INVOICE_AN = '"+Invoice+"'", con);
 // Query EDI bidy file

        OleDbDataAdapter sda = new OleDbDataAdapter(comd);
        

        	DataTable EdiHeader = new DataTable();
            EdiHeader.TableName = "Invoice810Header";
            sda.Fill(EdiHeader);
			
			if(EdiHeader.Rows.Count >0)
			{			          
	            
	   	
	             string SenderId = "LCIT";
	             string ReceiveId = "KMTC";
	             string RunningNo = "";
	            
	             string OPS ="";
	             string VesselName ="";
	       
			   
// Call Method for generate content detail 
			     
		         string Invoice_data_Hdetail=""; 
		         int count =0; 

		         for(int i=0 ; i< stringArray.Length;i++)
					        	{

					        		if((i%2) != 0)
					        		{
					        			count++;

       										OleDbCommand comddetail = new OleDbCommand("SELECT 'IAD' AS MESSAGE_HEADER, SUBSTR(LPAD(ROW_NUMBER() OVER (PARTITION BY INV.INVOICE_AN ORDER BY OCL.SERVICE_TYPE_ID), 5, '0'), 1, 5) AS MESSAGE_SEQUENCE, SUBSTR(RPAD(INV.INVOICE_AN, 15, ' '), 1, 15) AS INVOICE_NUMBER, SUBSTR(RPAD(to_char(INV.INVOICE_DATE, 'YYYYMMDD'), 8, ' '), 1, 8) AS INVOICE_DATE, SUBSTR(RPAD('VSLCHG', 6, ' '), 1, 6) AS MESSAGE_ID, SUBSTR(RPAD(ocl.company_service_group_id, 33, ' '), 1, 33) AS TARIFF_CODE, SUBSTR(RPAD(ocl.charge_line_ds, 80, ' '), 1, 80) AS TARIFF_1ST_DESCRIPTION, '"+stringArray[i].ToString()+"  ' AS CONTAINER_NUMBER, SUBSTR(RPAD(NVL(to_char((SELECT CN_DCHG_D  FROM TMS_OWNER.TMS_CNTR_GRP1 tcg WHERE CNTR_AN LIKE '"+stringArray[i].ToString()+"'), 'YYYYMMDD'), ' '), 8, ' '), 1, 8) AS MOVEMENT_DATE , SUBSTR(RPAD(NVL(to_char((SELECT CN_DCHG_D  FROM TMS_OWNER.TMS_CNTR_GRP1 tcg WHERE CNTR_AN LIKE '"+stringArray[i].ToString()+"'), 'HH24MISS'), ' '), 6, ' '), 1, 6) AS MOVEMENT_TIME , SUBSTR(RPAD(INV.VESSEL_NM, 30, ' '), 1, 30) AS VESSEL_NAME, SUBSTR(RPAD(INV.VOYAGE_TERMINAL, 40, ' '), 1, 40) AS LINE_VESSEL_CODE, SUBSTR(RPAD(INV.VOYAGE_EXTERNAL, 40, ' '), 1, 40) AS LINE_VOYAGE_CODE, SUBSTR(RPAD(NVL(to_char(OCL.CALCULATE_START_DATE, 'YYYYMMDD'), ' '), 8, ' '), 1, 8) AS STORAGE_FROM_DATE , SUBSTR(RPAD(NVL(to_char(OCL.CALCULATE_START_DATE, 'HH24MISS'), ' '), 6, ' '), 1, 6) AS STORAGE_FROM_TIME , SUBSTR(RPAD(NVL(to_char(OCL.CALCULATE_END_DATE, 'YYYYMMDD'), ' '), 8, ' '), 1, 8) AS STORAGE_TO_DATE , SUBSTR(RPAD(NVL(to_char(OCL.CALCULATE_END_DATE, 'HH24MISS'), ' '), 6, ' '), 1, 6) AS STORAGE_TO_TIME, SUBSTR(RPAD(decode((SELECT CONTAINER_TYPE_C   FROM TMV_CNTR_GRP1 WHERE CNTR_AN = '"+stringArray[i].ToString()+"'), '2210', '20GP', '2200', '20GP', '2250', '20GP', '22G1', '20GP', '2230', '20RF', '2251', '20OT', '2263', '20FR', '22T6', '20TK', '2500', '20HQ', '4300', '40GP', '4310', '40GP', '4351', '40OT', '4250', '40OT', '4363', '40FR', '4364', '40FR', '4351', '40OT', '4500', '40HQ', '4530', '40RH', '4532', '40RH', '45R0', '40RH', '45R1', '40RH', '4550', '45OT', OCL.EQUIPMENT_TYPE_AN) , 4, ' '), 1, 4) AS CONTAINER_INDICATOR FROM EBS_OWNER.INVOICE_V INV, ORDER_CHARGE_LINE_V OCL, CHARGE_LINE_T CL , COMPANY_T COM, VESSEL_VOYAGE_T VS WHERE INV.INVOICE_AN = '"+Invoice+"'AND INV.CONTRACT_COMPANY_NM LIKE '%KMT%'AND inv.invoice_id = cl.invoice_id AND cl.CHARGE_LINE_ID = ocl.CHARGE_LINE_ID AND inv.COMPANY_ID = com.COMPANY_ID AND vs.VOYAGE_TERMINAL(+) = CL.VOYAGE_TERMINAL_OUTBOUND", con);

       										OleDbDataAdapter sdadetail = new OleDbDataAdapter(comddetail);
       										DataTable EdiHeaderDetail = new DataTable();
								            EdiHeader.TableName = "Invoice810Detail";
								            sdadetail.Fill(EdiHeaderDetail);





					        			//Invoice_data_Hdetail += stringArray[i].ToString()+" "+stringArray2[i].ToString()+"\n";
					        			Invoice_data_Hdetail +=EdiHeaderDetail.Rows[0]["MESSAGE_HEADER"].ToString()+					 
	         		 							count.ToString("00000")+
	         							 		EdiHeaderDetail.Rows[0]["INVOICE_NUMBER"].ToString()+
	         							 		EdiHeaderDetail.Rows[0]["INVOICE_DATE"].ToString()+
	         							 		EdiHeaderDetail.Rows[0]["MESSAGE_ID"].ToString()+
	         							 		"      "+
	         							 		EdiHeaderDetail.Rows[0]["TARIFF_CODE"].ToString()+
	         							 		EdiHeaderDetail.Rows[0]["TARIFF_1ST_DESCRIPTION"].ToString()+
	         							 		" "+
	         							 		"    "+
	         							 		EdiHeaderDetail.Rows[0]["CONTAINER_NUMBER"].ToString()+
	         							 		" "+
	         							 		EdiHeaderDetail.Rows[0]["MOVEMENT_DATE"].ToString()+
	         							 		EdiHeaderDetail.Rows[0]["MOVEMENT_TIME"].ToString()+
	         							 		EdiHeaderDetail.Rows[0]["VESSEL_NAME"].ToString()+
	         							 		EdiHeaderDetail.Rows[0]["LINE_VESSEL_CODE"].ToString()+
	         							 		EdiHeaderDetail.Rows[0]["LINE_VOYAGE_CODE"].ToString()+
	         							 		" "+
	         							 		EdiHeaderDetail.Rows[0]["STORAGE_FROM_DATE"].ToString()+
	         							 		EdiHeaderDetail.Rows[0]["STORAGE_FROM_TIME"].ToString()+
	         							 		EdiHeaderDetail.Rows[0]["STORAGE_TO_DATE"].ToString()+
	         							 		EdiHeaderDetail.Rows[0]["STORAGE_TO_TIME"].ToString()+
	         							 		EdiHeaderDetail.Rows[0]["CONTAINER_INDICATOR"].ToString()+
	         							 		"     "+
	         							 		"     "+
	         							 		"     "+
	         							 		"     "+
	         							 		"   "+
	         							 		"     "+
	         							 		"        "+
	         							 		"      "+
	         							 		"                    "+
	         							 		"                    "+
	         							 		"          "+
	         							 		"                    "+
	         							 		"     "+
	         							 		"                              "+
	         							 		"\n";
					        		}							
		         					
		         				}

		         				  string Invoice_data_header =	EdiHeader.Rows[0]["MESSAGE_HEADER"].ToString()+
			     								EdiHeader.Rows[0]["MESSAGE_SEQUENCE"].ToString()+
			     								EdiHeader.Rows[0]["INVOICE_NUMBER"].ToString()+
			     								EdiHeader.Rows[0]["INVOICE_DATE"].ToString()+
			     								EdiHeader.Rows[0]["MESSAGE_ID"].ToString()+
			     								EdiHeader.Rows[0]["SENDER_ID"].ToString()+
			     								EdiHeader.Rows[0]["RECEIPIENT_ID"].ToString()+
			     								DateTime.Now.ToString("yyyyMMdd")+
			     								DateTime.Now.ToString("HHmmss")+
			     								EdiHeader.Rows[0]["MESSAGE_TYPE"].ToString()+
			     								count.ToString()+
			     								" \n";
				 		
				 Invoice_data_final = Invoice_data_header+Invoice_data_Hdetail;
			}

			return Invoice_data_final; // Return EDI data to web service

    		}
	}