using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Standalone
{

    public partial class adas_api : System.Web.UI.Page
    {
        Functionality fn = new Functionality();
        protected void Page_Load(object sender, EventArgs e)
        {
            string type = string.Empty;
            string ShowID = string.Empty;
            string actionID = string.Empty;
            string submittedBy = string.Empty;
            string designation = string.Empty;
            string email = string.Empty;
            string details = string.Empty;
            Hashtable htt = new Hashtable();
            DataTable data = new DataTable();
            ArrayList rtnlist = new ArrayList();


            #region Data
            if (Request.QueryString["type"] != null)
            {
                type = Request.QueryString["type"].ToString();
                if (type == "Save")
                {
                    if (Request.QueryString["ShowID"] != null && Request.QueryString["actionID"] != null && Request.QueryString["submittedBy"] != null && Request.QueryString["designation"] != null && Request.QueryString["email"] != null)
                    {
                        ShowID = Request.QueryString["ShowID"].ToString();
                        actionID = Request.QueryString["actionID"].ToString();
                        submittedBy = Request.QueryString["submittedBy"].ToString();
                        designation = Request.QueryString["designation"].ToString();
                        email = Request.QueryString["email"].ToString();
                        details = Request.QueryString["details"].ToString();
                    }
                }
                else
                {
                    if (Request.QueryString["ShowID"] != null && Request.QueryString["actionID"] != null)
                    {
                        ShowID = Request.QueryString["ShowID"].ToString();
                        actionID = Request.QueryString["actionID"].ToString();
                    }
                }
            }

            if (Request.Form["type"] != null)
            {
                type = Request.Form["type"].ToString();
                if (type == "Save")
                {
                    if (Request.Form["ShowID"] != null && Request.Form["actionID"] != null && Request.Form["submittedBy"] != null && Request.Form["designation"] != null && Request.Form["email"] != null)
                    {
                        ShowID = Request.Form["ShowID"].ToString();
                        actionID = Request.Form["actionID"].ToString();
                        submittedBy = Request.Form["submittedBy"].ToString();
                        designation = Request.Form["designation"].ToString();
                        email = Request.Form["email"].ToString();
                        details = Request.Form["details"].ToString();
                    }
                }
                else
                {
                    if (Request.Form["ShowID"] != null && Request.Form["actionID"] != null)
                    {
                        ShowID = Request.Form["ShowID"].ToString();
                        actionID = Request.Form["actionID"].ToString();
                    }
                }
            }
            #endregion

            #region Save SubmitInfo
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(ShowID) && !string.IsNullOrEmpty(actionID) && !string.IsNullOrEmpty(submittedBy) && !string.IsNullOrEmpty(designation) && !string.IsNullOrEmpty(email) && type == "Save")
            {
                try
                {
                    SaveSubmitInfo(ShowID, actionID, submittedBy, designation, email, details);
                }
                catch (Exception ex)
                {

                }
            }
            #endregion

            #region Request SubmitInfo
            else if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(ShowID) && type == "Request")
            {
                data = GetSubmitInfoByID(ShowID, actionID);
               if(data.Rows.Count>0)
                {
                    foreach (DataRow dr in data.Rows)
                    {
                        Hashtable ht= new Hashtable();
                        ht.Add("GUID", dr["GUID"].ToString());
                        ht.Add("ShowID", dr["ShowID"].ToString());
                        ht.Add("ActionID", dr["ActionID"].ToString());
                        ht.Add("SubmittedBy", dr["SubmittedBy"].ToString());
                        ht.Add("Designation", dr["Designation"].ToString());
                        ht.Add("Email", dr["Email"].ToString());
                        ht.Add("Details", dr["Details"].ToString());
                        ht.Add("deleteFlag", dr["deleteFlag"].ToString());
                        ht.Add("createdDate", dr["createdDate"].ToString());
                        ht.Add("updatedDate", dr["updatedDate"].ToString());

                        rtnlist.Add(ht);
                    }
                    htt.Add("data", rtnlist);
                    htt.Add("Status", "200");
                    htt.Add("message", "success");
                }
                else
                {
                    htt.Add("Status", "404");
                    htt.Add("message", "Not Found");
                }
            }
            else
            {
                htt.Add("Status", "417");
                htt.Add("message", "Expectation Failed");
            }
            #endregion
            JavaScriptSerializer ser = new JavaScriptSerializer();
            String jsonStr = ser.Serialize(htt);
            Response.Write(jsonStr);

        }
        #region SaveMethod
        public void SaveSubmitInfo(string ShowID, string actionID, string submittedBy, string designation, string email, string details)
        {

            try
            {
                StringBuilder str = new StringBuilder();
                str.Append("insert into tblSubmitterInfo ");
                str.Append("(ShowID,ActionID,SubmittedBy,Designation,Email,Details,deleteFlag,createdDate,updatedDate)");
                str.Append(" values");
                str.Append(" (@ShowID,@ActionID,@SubmittedBy,@Designation,@Email,@Details,@deleteFlag,@createdDate,@updatedDate)");

                SqlParameter[] parms ={
                                    new SqlParameter("@ShowID",SqlDbType.NVarChar),
                                    new SqlParameter("@ActionID",SqlDbType.NVarChar),
                                    new SqlParameter("@SubmittedBy",SqlDbType.NVarChar),
                                    new SqlParameter("@Designation",SqlDbType.NVarChar),
                                    new SqlParameter("@Email",SqlDbType.NVarChar),
                                    new SqlParameter("@Details",SqlDbType.NVarChar),
                                    new SqlParameter("@deleteFlag",SqlDbType.Bit),
                                    new SqlParameter("@createdDate",SqlDbType.DateTime),
                                    new SqlParameter("@updatedDate",SqlDbType.DateTime)
                                  };

                parms[0].Value = ShowID;
                parms[1].Value = actionID;
                parms[2].Value = submittedBy;
                parms[3].Value = designation;
                parms[4].Value = email;
                parms[5].Value = details;
                parms[6].Value = false;
                parms[7].Value = DateTime.Now;
                parms[8].Value = DateTime.Now;

                fn.ExecuteNonQuery(str.ToString(), parms);

            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region RequestMethod
        public DataTable GetSubmitInfoByID(string ShowID, string actionID)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = "select * from tblSubmitterInfo where ShowID='" + ShowID + "' and ActionID='" + actionID + "' and deleteFlag=0";
                dt = fn.GetDatasetByCommand(sql, "sdt").Tables[0];
            }
            catch (Exception ex)
            { }
            return dt;
        }
        #endregion
    }
}