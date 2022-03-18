using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;  
using System.IO;
using System.Web.Script.Serialization;   
using System.Reflection; 

 namespace convert_json{

	public class jsoncv{

		public string convertToJson(DataTable dt)
		{

		JavaScriptSerializer jsSerializer = new JavaScriptSerializer();  
        List < Dictionary < string, object >> parentRow = new List < Dictionary < string, object >> ();  
        Dictionary < string, object > childRow;  
        foreach(DataRow row in dt.Rows) 
        {  
            childRow = new Dictionary < string, object > ();  
            foreach(DataColumn col in dt.Columns) 
            {  
                childRow.Add(col.ColumnName, row[col]);  
            }  
            parentRow.Add(childRow);  
        }

         return jsSerializer.Serialize(parentRow); 
		}

	}
}