using Dapper;
using EmployeeEvaluationSystem.MVC.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using EmployeeEvaluationSystem.MVC.Models;
using ReportDetails = EmployeeEvaluationSystem.Entity.SharedObjects.Model.Reports.ReportDetails;

namespace EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6.Repositories
{
    public class ReportRepository
    {
        //To Handle connection related activities   
        SqlConnection con;

        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);
        }

        /// <summary>  
        /// Get Multiple Table details  
        /// </summary>  
        /// <returns></returns>  
        public IEnumerable<ReportDetails> GetReportDetails()
        {
            connection();
            con.Open();
            var objDetails = SqlMapper.QueryMultiple(con, "GetReportDetails", commandType: CommandType.StoredProcedure);
            ReportDetails ObjMaster = new ReportDetails();

            //Assigning each Multiple tables data to specific single model class              /
            ObjMaster.EmpAvgRatings = objDetails.Read<ReportGenerationViewModel>().ToList();

            List<ReportDetails> ReportObj = new List<ReportDetails>();
            //Add list of records into ReportDetails list  
            ReportObj.Add(ObjMaster);
            con.Close();

            return ReportObj;
        }
    }
}
