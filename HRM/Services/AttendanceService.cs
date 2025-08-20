using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Data;

namespace HRM.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public AttendanceService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public Task<bool> DeleteAttendance(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Attendance>> GetAllAttendance()
        {
            throw new NotImplementedException();
        }


        public async Task<bool> InsertAttendance(Attendance attendance)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);
                    var companyId = await _baseService.GetCompanyId(subscriptionId);

                    // check if employee already logged in today
                    var loginQuery = @"SELECT Id, logindate FROM attendance WHERE employeeid = @EmpId AND DATEDIFF(DAY, logindate, @NowDate) = 0";

                    var existingLogin = await connection.QueryFirstOrDefaultAsync<(int TxNo, DateTime LoginDate)?>(
                        loginQuery,
                        new { EmpId = attendance.EmployeeId, NowDate = DateTime.Now }
                    );

                    if (existingLogin == null) // no login today → INSERT
                    {
                        var insertQuery = @" INSERT INTO Attendance (EmployeeId, LoginDate, BranchId, SubscriptionId, CompanyId) VALUES (@EmployeeId, @LoginDate, @BranchId, @SubscriptionId, @CompanyId); SELECT SCOPE_IDENTITY();";

                        var parameters = new
                        {
                            EmployeeId = attendance.EmployeeId,
                            LoginDate = DateTime.Now,
                            BranchId = branchId,
                            SubscriptionId = subscriptionId,
                            CompanyId = companyId
                        };

                        var Id = await connection.ExecuteScalarAsync<int>(insertQuery, parameters);

                        return Id > 0; 
                    }
                    else
                    {
                        var Id = existingLogin.Value.TxNo;
                        var logindt = existingLogin.Value.LoginDate;

                        // If logged in less than 1 hour ago → block
                        if ((DateTime.Now - logindt).TotalHours < 1)
                        {
                            // already logged in recently
                            return false;
                        }

                        // otherwise update logout
                        var updateQuery = @"UPDATE attendance SET logoutdate = @LogoutDate WHERE Id = @Id";

                        var updated = await connection.ExecuteAsync(updateQuery,
                            new { LogoutDate = DateTime.Now, Id = Id });

                        return updated > 0;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }






        //public async Task<bool> InsertAttendance(Attendance attendance)
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            connection.Open();

        //            var subscriptionId = _baseService.GetSubscriptionId();
        //            var userId = _baseService.GetUserId();
        //            var branchId = await _baseService.GetBranchId(subscriptionId, userId);
        //            var companyId = await _baseService.GetCompanyId(subscriptionId);


        //            var login_data = @"SELECT tx_no FROM attendance WHERE employee_id = @EmpId AND DATEDIFF(DAY, login_date, @NowDate) = 0";

        //            double loginTX = await connection.QueryFirstOrDefaultAsync<int?>( login_data, new { EmpId = attendance.EmployeeId, NowDate = DateTime.Now } ) ?? 0;

        //            if (loginTX == 0) // insert only if not already logged in today
        //            {
        //                var queryString = @" INSERT INTO Attendance (EmployeeId, LoginDate, BranchId, SubscriptionId, CompanyId) VALUES (@EmployeeId, @LoginDate, @BranchId, @SubscriptionId, @CompanyId); SELECT SCOPE_IDENTITY();";

        //                var parameters = new DynamicParameters();
        //                parameters.Add("EmployeeId", attendance.EmployeeId, DbType.Int64);
        //                parameters.Add("LoginDate", DateTime.Now, DbType.DateTime);
        //                parameters.Add("BranchId", branchId);
        //                parameters.Add("SubscriptionId", subscriptionId);
        //                parameters.Add("CompanyId", companyId);

        //                // get inserted tx_id
        //                var tx_id = await connection.ExecuteScalarAsync<int>(queryString, parameters);

        //                if (tx_id > 0)
        //                {
        //                    var query = @"SELECT a.employee_id AS [Employee ID], e.EmployeeName AS [Employee Name], a.login_date AS [Login Date], a.logout_date AS [Logout Date] FROM attendance a INNER JOIN employee e ON a.employee_id = e.EmployeeId WHERE a.tx_no = @TxId WHERE SubscriptionId = @subscriptionId";

        //                    var result = await connection.QueryAsync<Attendance>(query, new { subscriptionId, TxId = tx_id });
        //                    return true;
        //                }
        //            }
        //            else
        //            {
        //                Dim tx_id
        //    tx_id = genf.returnvaluefromdv("select tx_no from attendance where employee_id='" & Val(TextBox1.Text) & "' and datediff(day,login_date,'" & DateTime.Now & "')=0", 0)

        //    Dim logindt
        //    If Not IsNothing(genf.returnvaluefromdvdtformat("select login_date from attendance where employee_id='" & Val(TextBox1.Text) & "' and datediff(day,login_date,'" & DateTime.Now & "')=0", 0)) Then
        //        logindt = genf.returnvaluefromdvdtformat("select login_date from attendance where employee_id='" & Val(TextBox1.Text) & "' and datediff(day,login_date,'" & DateTime.Now & "')=0", 0)

        //    Else
        //        logindt = DateTime.Now 'genf.returnvaluefromdvdtformat("select login_date from attendance where employee_id='" & Val(TextBox1.Text) & "' and datediff(day,login_date,'" & DateTime.Now & "')=0", 0)

        //    End If


        //    Label2.Text = DateDiff(DateInterval.Hour, logindt, DateTime.Now)
        //    'Dim sb2 As StringBuilder
        //    'sb2 = New StringBuilder()
        //    'sb2.Append("<script>")
        //    'sb2.Append("alert('" & DateDiff(DateInterval.Hour, logindt, DateTime.Now) & "');")
        //    'sb2.Append("</scri")
        //    'sb2.Append("pt>")
        //    'Page.RegisterStartupScript("test2", sb2.ToString())


        //    If DateDiff(DateInterval.Hour, logindt, DateTime.Now) < 1 Then

        //        Dim alertScript = "alert('You have already Logged in...');"
        //        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Key", alertScript, True)
        //          refresh()
        //    Else
        //        Dim insertnewrecord
        //        insertnewrecord = ""
        //        insertnewrecord = "update [attendance]"
        //        insertnewrecord = insertnewrecord & " set [logout_date]='" & DateTime.Now & "' where tx_no='" & tx_id & "'"

        //        Myconnection = New System.Data.SqlClient.SqlConnection(strconnection)
        //        Myconnection.Open()

        //        strnewrecord = insertnewrecord
        //        Mycommand = New SqlCommand(strnewrecord, Myconnection)
        //        Mycommand.ExecuteNonQuery()
        //        Myconnection.Close()
        //        refresh()
        //        Literal1.Text = genf.create_table("select employee_id as [Employee ID], [Employee Name], login_date as [Login Date], logout_date as [Logout Date] from attendance, employee where employee_id=[Employee ID] and tx_no='" & tx_id & "'")

        //        TextBox2.Text = "1"
        //        TextBox1.Text = ""
        //        TextBox1.Focus()

        //    End If
        //            }





        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}



        public Task<bool> UpdateAttendance(Attendance attendance)
        {
            throw new NotImplementedException();
        }
    }
}
