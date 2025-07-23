using Dapper;
using HRM.Interfaces;
using HRM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace HRM.Services
{
    public class CompaniesService : ICompaniesService
    {
        private readonly string _connectionString;
        private readonly BaseService _baseService;

        public CompaniesService(IConfiguration configuration, BaseService baseService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
            _baseService = baseService;
        }
        public async Task<List<Companies>> GetAllCompanies()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);

                    var queryString = "select Name,TagLine,VatRegistrationNo,TinNo,WebsiteLink,Email,ContactNumber,Address,Remarks,LogoPath,SubscriptionId from Companies where SubscriptionId='"+ subscriptionId + "'";
                    var query = string.Format(queryString);
                    var List = await connection.QueryAsync<Companies>(query);
                    return List.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertCompanies(Companies companies)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var subscriptionId = _baseService.GetSubscriptionId();
                    var userId = _baseService.GetUserId();
                    var branchId = await _baseService.GetBranchId(subscriptionId, userId);


                    string logoPath = null;

                    if (companies.LogoFile != null && companies.LogoFile.Length > 0)
                    {
                        // Create a unique file name
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(companies.LogoFile.FileName);
                        var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/logo", fileName);

                        using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await companies.LogoFile.CopyToAsync(stream);
                        }

                        // Save relative path to DB
                        logoPath = "/logo/" + fileName;
                    }

                    var Id = "select SubscriptionId from Companies where SubscriptionId='"+ subscriptionId + "'";
                    int SubscriptionId = connection.ExecuteScalar<int>(Id);


                    if (SubscriptionId ==null)
                    {
                        var queryString = "insert into Companies (Name,TagLine,VatRegistrationNo,TinNo,WebsiteLink,Email,ContactNumber,Address,Remarks,LogoPath,SubscriptionId,CreatedAt) values ";
                        queryString += "( @Name,@TagLine,@VatRegistrationNo,@TinNo,@WebsiteLink,@Email,@ContactNumber,@Address,@Remarks,@LogoPath,@SubscriptionId,@CreatedAt)";
                        var parameters = new DynamicParameters();
                        parameters.Add("Name", companies.Name, DbType.String);
                        parameters.Add("TagLine", companies.TagLine, DbType.String);
                        parameters.Add("VatRegistrationNo", companies.VatRegistrationNo, DbType.String);
                        parameters.Add("TinNo", companies.TinNo, DbType.String);
                        parameters.Add("WebsiteLink", companies.WebsiteLink, DbType.String);
                        parameters.Add("Email", companies.Email, DbType.String);
                        parameters.Add("ContactNumber", companies.ContactNumber, DbType.String);
                        parameters.Add("Address", companies.Address, DbType.String);
                        parameters.Add("Remarks", companies.Remarks, DbType.String);
                        parameters.Add("LogoPath", logoPath);
                        parameters.Add("SubscriptionId", subscriptionId);
                        parameters.Add("CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
                        var success = await connection.ExecuteAsync(queryString, parameters);
                        if (success > 0)
                        {
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        var queryString = "Update Companies set(Name=@Name,TagLine=@TagLine,VatRegistrationNo=@VatRegistrationNo,TinNo=@TinNo,WebsiteLink=@WebsiteLink,Email=@Email,ContactNumber=@ContactNumber,Address=@Address,Remarks=@Remarks,LogoPath=@LogoPath,SubscriptionId=@SubscriptionId,CreatedAt=@CreatedAt) values ";
                        var parameters = new DynamicParameters();
                        parameters.Add("Name", companies.Name, DbType.String);
                        parameters.Add("TagLine", companies.TagLine, DbType.String);
                        parameters.Add("VatRegistrationNo", companies.VatRegistrationNo, DbType.String);
                        parameters.Add("TinNo", companies.TinNo, DbType.String);
                        parameters.Add("WebsiteLink", companies.WebsiteLink, DbType.String);
                        parameters.Add("Email", companies.Email, DbType.String);
                        parameters.Add("ContactNumber", companies.ContactNumber, DbType.String);
                        parameters.Add("Address", companies.Address, DbType.String);
                        parameters.Add("Remarks", companies.Remarks, DbType.String);
                        parameters.Add("LogoPath", logoPath);
                        parameters.Add("SubscriptionId", subscriptionId);
                        parameters.Add("CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
                        var success = await connection.ExecuteAsync(queryString, parameters);
                        if (success > 0)
                        {
                            return true;
                        }
                        return false;
                    }



                    
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<bool> UpdateCompanies(Companies companies)
        {
            throw new NotImplementedException();
        }
    }
}
