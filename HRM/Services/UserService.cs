using Dapper;
using HRM.Models;
using Microsoft.Data.SqlClient;
using HRM.Interfaces;
using System.Data;
using System.Net;
using System.Net.Mail;

namespace HRM.Services
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;
        public UserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
        }
        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "delete from users where id=@id";
                    var parameters = new DynamicParameters();
                    parameters.Add("id", userId.ToString(), DbType.String);
                    var success = await connection.ExecuteAsync(queryString, parameters);
                    if (success > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<User>> GetAllUserAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select id,name,(case when status=1 then 'Active' else 'InActive' end) StatusName,";
                    queryString += "Email,MobileNo,Password,School,ClassId,Dob from Users where name<>'Super Admin' order  by name";
                    var query = string.Format(queryString);
                    var userList = await connection.QueryAsync<User>(query);
                    var users = userList.ToList();

                    foreach (var user in users)
                    {
                        queryString = "select (select name from roles where id=ur.roleid) from UserRole ur where userid='{0}'";
                        query = string.Format(queryString, user.Id);
                        var roles = await connection.QueryAsync<string>(query);
                        var rolesName = "";
                        foreach (var item in roles)
                        {
                            rolesName += item + ",";
                        }
                        user.Roles = rolesName;
                    }
                    return users;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<User> GetLoginUserAsync(string email, string password)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var queryString1 = "select t3.Name from Users t1 join UserRole t2 on t1.Id=t2.UserId join Roles t3 on t2.RoleId=t3.Id where t1.Email='{0}'";
                    var query1 = string.Format(queryString1, email);
                    var user1 = await connection.QueryFirstOrDefaultAsync<User>(query1);

                    if (user1?.Name == "Teacher")
                    {
                        var queryString = "select id,name,status,";
                        queryString += "Email,MobileNo from Users where email='{0}' and password='{1}' and status=1 and Aproval=1";
                        var query = string.Format(queryString, email, password);
                        var user = await connection.QueryFirstOrDefaultAsync<User>(query);
                        return user;
                    }
                    else
                    {
                        var queryString = "select id,name,status,";
                        queryString += "Email,MobileNo from Users where email='{0}' and password='{1}' and status=1";
                        var query = string.Format(queryString, email, password);
                        var user = await connection.QueryFirstOrDefaultAsync<User>(query);
                        return user;
                    }

                    
                    
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select id,name,status,";
                    queryString += "Email,MobileNo from Users where id='{0}'";
                    var query = string.Format(queryString, userId.ToString());
                    var user = await connection.QueryFirstOrDefaultAsync<User>(query);
                    queryString = "select roleid from UserRole where userid='{0}'";
                    query = string.Format(queryString, userId.ToString());
                    var roles = await connection.QueryAsync<Guid>(query);
                    List<string> roleList = new List<string>();
                    foreach (var item in roles)
                    {
                        roleList.Add(item.ToString());
                    }
                    user.Role = roleList;
                    return user;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select roleid";
                    queryString += " from UserRole where UserId='{0}'";
                    var query = string.Format(queryString, userId.ToString());
                    var userRoles = await connection.QueryAsync<Guid>(query);
                    var roles = new List<string>();
                    foreach (var item in userRoles)
                    {
                        roles.Add(item.ToString());
                    }
                    return roles;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<User> GetUserTypeCheckAsync(string email)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select t3.Name from Users t1 join UserRole t2 on t1.Id=t2.UserId join Roles t3 on t2.RoleId=t3.Id where t1.Email='{0}'";
                        var query = string.Format(queryString, email);
                        var user = await connection.QueryFirstOrDefaultAsync<User>(query);
                        return user;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public async Task<string> InsertUserAsync(User user)
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            connection.Open();



        //            var random = new Random();
        //            var otp = random.Next(100000, 999999).ToString();

        //            //var queryString = "select email from users where lower(email)='{0}' ";
        //            //var query = string.Format(queryString, user.Email.ToLower());
        //            //var userObj = await connection.QueryFirstOrDefaultAsync<string>(query);
        //            //if (userObj != null && userObj.Length > 0)
        //            //    return false;

        //            var query = "select Name from Roles where id='"+user.RName+"'";
        //            var type = await connection.QueryFirstOrDefaultAsync<string>(query);


        //            var queryString = "insert into Users (id,name,mobileno,email,status,password,School,Classid,Dob,GetDateby,Updateby,UserTypeid,otp,Aproval) values ";
        //            queryString += "(@id,@name,@mobileno,@email,@status,@password,@School,@Classid,@Dob,@GetDateby,@Updateby,@UserTypeid,@otp,@Aproval)";

        //            var parameters = new DynamicParameters();
        //            var userId = Guid.NewGuid();  // Use GUID directly without converting to string

        //            parameters.Add("id", userId, DbType.Guid); // Pass as GUID instead of string
        //            parameters.Add("name", user.Name, DbType.String);
        //            parameters.Add("mobileno", user.MobileNo, DbType.String);
        //            parameters.Add("email", user.Email, DbType.String);
        //            parameters.Add("status", 0, DbType.Boolean);
        //            parameters.Add("password", user.Password, DbType.String);
        //            parameters.Add("School", user.School, DbType.String);
        //            parameters.Add("Classid", user.Class, DbType.String);
        //            parameters.Add("Dob", user.Dob, DbType.String);
        //            parameters.Add("otp", otp, DbType.String); // Fix casing: 'otp' should match the SQL column name exactly
        //            parameters.Add("GetDateby", DateTime.Now.ToString("yyyy-MM-dd"), DbType.String);
        //            parameters.Add("Updateby", DateTime.Now.ToString("yyyy-MM-dd"), DbType.String);
        //            parameters.Add("UserTypeid", type, DbType.String);
        //            parameters.Add("Aproval", 0, DbType.Boolean);

        //            var success = await connection.ExecuteAsync(queryString, parameters);



        //            foreach (var item in user.Role)
        //            {
        //                queryString = "insert into UserRole (UserId,RoleId) values ";
        //                queryString += "( @UserId,@RoleId)";
        //                parameters = new DynamicParameters();
        //                parameters.Add("UserId", (userId), DbType.String);
        //                parameters.Add("RoleId", item, DbType.String);
        //                success = await connection.ExecuteAsync(queryString, parameters);
        //            }

        //            if (success > 0)
        //            {

        //                using (WebClient client = new WebClient())
        //                {
        //                    var domain = "https://";
        //                    var message = "Edex365 OTP Number is " + otp + " ";
        //                    string url = "" + domain + "api.infobuzzer.net/v3.1/TransmitSMS?username=nm@synergyinterface.com&password=info@Pass&to=88" + user.MobileNo + "&text=" + message + "";
        //                    //client.DownloadString(url);
        //                }
        //                return userId.ToString();
        //            }

        //            return string.Empty;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //without otp via mail

        //public async Task<string> InsertUserAsync(User user)
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            await connection.OpenAsync(); // Use async version of Open
        //            var query = "SELECT COUNT(1) FROM Users WHERE MobileNo = @MobileNo";
        //            var count = await connection.ExecuteScalarAsync<int>(query, new { MobileNo = user.MobileNo });

        //            //bool exists = count > 0;
        //            if (count <= 0)
        //            {
        //                var random = new Random();
        //                var otp = random.Next(100000, 999999).ToString();

        //                // Get the user type based on role name (from Roles table)

        //                //var query = "SELECT id FROM Roles WHERE id = @RoleId";
        //                //var type = await connection.QueryFirstOrDefaultAsync<Guid>(query, new { RoleId = user.RName });



        //                // Insert user into Users table
        //                var queryString = @"
        //        INSERT INTO Users (
        //            id, name, mobileno, email, status, password, Dob,
        //            GetDateby, Updateby, otp, Aproval,UserTypeId
        //        ) VALUES (
        //            @id, @name, @mobileno, @email, @status, @password,@Dob,
        //            @GetDateby, @Updateby, @otp, @Aproval,@UserTypeId
        //        )";

        //                var parameters = new DynamicParameters();
        //                var userId = Guid.NewGuid();  // Generate a new GUID for the userId

        //                parameters.Add("id", userId, DbType.Guid); // Pass as GUID
        //                parameters.Add("name", user.Name, DbType.String); // Fixed from DbType.Guid to DbType.String
        //                parameters.Add("mobileno", user.MobileNo, DbType.String);
        //                parameters.Add("email", user.Email, DbType.String);
        //                parameters.Add("status", 0, DbType.Boolean);
        //                parameters.Add("password", user.Password, DbType.String);
        //                parameters.Add("Dob", user.Dob, DbType.String);
        //                parameters.Add("otp", otp, DbType.String); // Ensure the casing matches
        //                parameters.Add("GetDateby", DateTime.Now.ToString("yyyy-MM-dd"), DbType.String);
        //                parameters.Add("Updateby", DateTime.Now.ToString("yyyy-MM-dd"), DbType.String);
        //                parameters.Add("UserTypeid", Guid.Parse(user.RName), DbType.Guid);
        //                parameters.Add("Aproval", 0, DbType.Boolean);

        //                var success = await connection.ExecuteAsync(queryString, parameters);

        //                // Insert roles into UserRole table
        //                foreach (var item in user.Role)
        //                {
        //                    queryString = "INSERT INTO UserRole (UserId, RoleId) VALUES (@UserId, @RoleId)";
        //                    parameters = new DynamicParameters();
        //                    parameters.Add("UserId", userId, DbType.Guid); // Ensure UserId is GUID
        //                    parameters.Add("RoleId", item, DbType.String);
        //                    success += await connection.ExecuteAsync(queryString, parameters); // Add success count
        //                }

        //                if (success > 0)
        //                {
        //                    // Send OTP via SMS
        //                    using (var client = new WebClient())
        //                    {
        //                        var domain = "https://";
        //                        var message = "Edex365 OTP Number is " + otp;
        //                        string url = $"{domain}api.infobuzzer.net/v3.1/TransmitSMS?username=nm@synergyinterface.com&password=info@Pass&to=88{user.MobileNo}&text={message}";
        //                        // Uncomment the following line to actually send the SMS
        //                        //await client.DownloadStringTaskAsync(url);
        //                    }
        //                    return userId.ToString();  // Return the generated userId as a string
        //                }
        //            }

                    

        //            return string.Empty;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}





public async Task<string> InsertUserAsync(User user)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT COUNT(1) FROM Users WHERE MobileNo = @MobileNo";
                var count = await connection.ExecuteScalarAsync<int>(query, new { MobileNo = user.MobileNo });

                if (count <= 0)
                {
                    var random = new Random();
                    var otp = random.Next(100000, 999999).ToString();

                    var queryString = @"
                    INSERT INTO Users (
                        id, name, mobileno, email, status, password, Dob,
                        GetDateby, Updateby, otp, Aproval, UserTypeId
                    ) VALUES (
                        @id, @name, @mobileno, @email, @status, @password, @Dob,
                        @GetDateby, @Updateby, @otp, @Aproval, @UserTypeId
                    )";

                    var parameters = new DynamicParameters();
                    var userId = Guid.NewGuid();

                    parameters.Add("id", userId, DbType.Guid);
                    parameters.Add("name", user.Name, DbType.String);
                    parameters.Add("mobileno", user.MobileNo, DbType.String);
                    parameters.Add("email", user.Email, DbType.String);
                    parameters.Add("status", 0, DbType.Boolean);
                    parameters.Add("password", user.Password, DbType.String);
                    parameters.Add("Dob", user.Dob, DbType.String);
                    parameters.Add("otp", otp, DbType.String);
                    parameters.Add("GetDateby", DateTime.Now.ToString("yyyy-MM-dd"), DbType.String);
                    parameters.Add("Updateby", DateTime.Now.ToString("yyyy-MM-dd"), DbType.String);
                    parameters.Add("UserTypeid", Guid.Parse(user.RName), DbType.Guid);
                    parameters.Add("Aproval", 0, DbType.Boolean);

                    var success = await connection.ExecuteAsync(queryString, parameters);

                    // Insert roles into UserRole table
                    foreach (var item in user.Role)
                    {
                        queryString = "INSERT INTO UserRole (UserId, RoleId) VALUES (@UserId, @RoleId)";
                        parameters = new DynamicParameters();
                        parameters.Add("UserId", userId, DbType.Guid);
                        parameters.Add("RoleId", item, DbType.String);
                        success += await connection.ExecuteAsync(queryString, parameters);
                    }

                    if (success > 0)
                    {
                        // Send OTP via Email
                        var smtpClient = new SmtpClient("smtp.your-email-provider.com") // Replace with your SMTP server
                        {
                            Port = 587, // Usually 587 for TLS, or 465 for SSL
                            Credentials = new NetworkCredential("your-email@example.com", "your-email-password"),
                            EnableSsl = true, // Use SSL if required by the email provider
                        };

                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress("your-email@example.com"),
                            Subject = "Your OTP Code",
                            Body = $"Your OTP code is: {otp}",
                            IsBodyHtml = false,
                        };
                        mailMessage.To.Add(user.Email);

                        await smtpClient.SendMailAsync(mailMessage);

                        return userId.ToString();
                    }
                }

                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }





    //public async Task<string> InsertUserAsync(User user)
    //{
    //    try
    //    {
    //        using (var connection = new SqlConnection(_connectionString))
    //        {
    //            await connection.OpenAsync();

    //            var random = new Random();
    //            var otp = random.Next(100000, 999999).ToString();

    //            // Retrieve the GUID of the role (ensuring Role ID is GUID type)
    //            var query = "SELECT Id FROM Roles WHERE Name = @RoleName";
    //            var roleId = await connection.QueryFirstOrDefaultAsync<Guid>(query, new { RoleName = user.RName });

    //            if (roleId == Guid.Empty)
    //                throw new Exception("Role not found or invalid RoleId.");

    //            // Insert user into Users table
    //            var queryString = @"
    //    INSERT INTO Users (
    //        id, name, mobileno, email, status, password, School, Classid, Dob,
    //        GetDateby, Updateby, UserTypeid, otp, Aproval
    //    ) VALUES (
    //        @id, @name, @mobileno, @email, @status, @password, @School, @Classid, @Dob,
    //        @GetDateby, @Updateby, @UserTypeid, @otp, @Aproval
    //    )";

    //            var parameters = new DynamicParameters();
    //            var userId = Guid.NewGuid();

    //            parameters.Add("id", userId, DbType.Guid);
    //            parameters.Add("name", user.Name, DbType.String);
    //            parameters.Add("mobileno", user.MobileNo, DbType.String);
    //            parameters.Add("email", user.Email, DbType.String);
    //            parameters.Add("status", 0, DbType.Boolean);
    //            parameters.Add("password", user.Password, DbType.String);
    //            parameters.Add("School", user.School, DbType.String);
    //            parameters.Add("Classid", user.Class, DbType.String);
    //            parameters.Add("Dob", user.Dob, DbType.String);
    //            parameters.Add("otp", otp, DbType.String);
    //            parameters.Add("GetDateby", DateTime.Now.ToString("yyyy-MM-dd"), DbType.String);
    //            parameters.Add("Updateby", DateTime.Now.ToString("yyyy-MM-dd"), DbType.String);
    //            parameters.Add("UserTypeid", roleId, DbType.Guid);  // Corrected: use GUID for UserTypeid
    //            parameters.Add("Aproval", 0, DbType.Boolean);

    //            var success = await connection.ExecuteAsync(queryString, parameters);

    //            // Insert roles into UserRole table (assuming RoleId is GUID)
    //            foreach (var item in user.Role)
    //            {
    //                queryString = "INSERT INTO UserRole (UserId, RoleId) VALUES (@UserId, @RoleId)";
    //                parameters = new DynamicParameters();
    //                parameters.Add("UserId", userId, DbType.Guid);
    //                parameters.Add("RoleId", item, DbType.Guid);  // Ensure RoleId is GUID
    //                success += await connection.ExecuteAsync(queryString, parameters);
    //            }

    //            if (success > 0)
    //            {
    //                // Send OTP via SMS
    //                using (var client = new WebClient())
    //                {
    //                    var domain = "https://";
    //                    var message = "Edex365 OTP Number is " + otp;
    //                    string url = $"{domain}api.infobuzzer.net/v3.1/TransmitSMS?username=nm@synergyinterface.com&password=info@Pass&to=88{user.MobileNo}&text={message}";
    //                    // Uncomment the following line to actually send the SMS
    //                    // await client.DownloadStringTaskAsync(url);
    //                }
    //                return userId.ToString();
    //            }

    //            return string.Empty;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // Consider logging the exception message here for debugging
    //        throw; // Rethrow the exception
    //    }
    //}





    public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var queryString = "update Users set name=@name,mobileno=@mobileno,email=@email,status=@status ";
                    queryString += " ,password=@password where id=@id";
                    var parameters = new DynamicParameters();
                    parameters.Add("name", user.Name, DbType.String);
                    parameters.Add("mobileno", user.MobileNo, DbType.String);
                    parameters.Add("email", user.Email, DbType.String);
                    parameters.Add("status", user.Status, DbType.Boolean);
                    parameters.Add("password", user.Password, DbType.String);
                    parameters.Add("id", user.Id.ToString(), DbType.String);
                    var success = await connection.ExecuteAsync(queryString, parameters);

                    

                    foreach (var item in user.Role)
                    {
                        queryString = "insert into UserRole (UserId,RoleId) values ";
                        queryString += "( @UserId,@RoleId)";
                        parameters = new DynamicParameters();
                        parameters.Add("UserId", user.Id.ToString(), DbType.String);
                        parameters.Add("RoleId", item, DbType.String);
                        success = await connection.ExecuteAsync(queryString, parameters);
                    }

                    if (success > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
