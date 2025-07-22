using Dapper;
using HRM.Models;
using Microsoft.Data.SqlClient;
using HRM.Interfaces;
using System.Data;

namespace HRM.Services
{
    public class RoleService : IRoleService
    {
        private readonly string _connectionString;
        public RoleService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
        }

        public async Task<bool> AddRolePermissionAsync(RolePermission role)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "";
                    var success = 0;
                    queryString = "delete from RolePermission where RoleId=@RoleId";
                    var parameters = new DynamicParameters();
                    parameters.Add("RoleId", role.RoleId, DbType.String);
                    success = await connection.ExecuteAsync(queryString, parameters);
                    foreach (var item in role.Menus)
                    {
                        queryString = "insert into RolePermission (RoleId,MenuId) values ";
                        queryString += "( @RoleId,@MenuId)";
                        parameters = new DynamicParameters();
                        parameters.Add("RoleId", role.RoleId, DbType.String);
                        parameters.Add("MenuId", item, DbType.Int32);
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

        public async Task<bool> DeleteRoleAsync(Guid roleId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select count(*) from UserRole where roleId=@id";
                    var parameters = new DynamicParameters();
                    parameters.Add("id", roleId.ToString(), DbType.String);
                    var result = await connection.ExecuteAsync(queryString, parameters);
                    if (result > 0)
                        return false;
                    queryString = "delete from Roles where id=@id";
                    parameters = new DynamicParameters();
                    parameters.Add("id", roleId.ToString(), DbType.String);
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

        public async Task<List<Menu>> GetAllMenusAsync(Guid roleId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select * from Menus order by id  ";
                    var query = string.Format(queryString);
                    var menuList = await connection.QueryAsync<Menu>(query);
                    foreach (var item in menuList)
                    {
                        queryString = "select MenuId from RolePermission where roleId='{0}'";
                        queryString += " and menuId={1}";
                        query = string.Format(queryString, roleId.ToString(), item.Id);
                        var result = await connection.QueryFirstOrDefaultAsync<int>(query);
                        if (result > 0)
                            item.Status = true;
                        else
                            item.Status = false;
                    }
                    return menuList.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select id,name,(case when status=1 then 'Active' else 'InActive' end) StatusName from Roles where name<>'Super Admin' and name<> 'Admin' order by name  ";
                    var query = string.Format(queryString);
                    var roleList = await connection.QueryAsync<Role>(query);
                    return roleList.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Role> GetRoleByIdAsync(Guid roleId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select * from Roles where id='{0}' order by name  ";
                    var query = string.Format(queryString, roleId);
                    var role = await connection.QueryFirstOrDefaultAsync<Role>(query);
                    return role;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select * from Roles where name='{0}' order by name  ";
                    var query = string.Format(queryString, roleName);
                    var role = await connection.QueryFirstOrDefaultAsync<Role>(query);
                    return role;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> InsertRoleAsync(Role role)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var queryString = "select name from roles where lower(name)='{0}' ";
                    var query = string.Format(queryString, role.Name.ToLower());
                    var roleObj = await connection.QueryFirstOrDefaultAsync<string>(query);
                    if (roleObj != null && roleObj.Length > 0)
                        return false;
                    queryString = "insert into Roles (id,name,status) values ";
                    queryString += "( @id,@name,@status)";
                    var parameters = new DynamicParameters();
                    parameters.Add("id", Guid.NewGuid().ToString(), DbType.String);
                    parameters.Add("name", role.Name, DbType.String);
                    parameters.Add("status", 1, DbType.Boolean);
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

        public async Task<List<string>> RolePermissionAsync(string roleId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var queryString = "select (select permissionname from menus where id=r.menuid) from RolePermission";
                    queryString += " r where roleid='{0}'  ";
                    var query = string.Format(queryString, roleId);
                    var permissionList = await connection.QueryAsync<string>(query);
                    return permissionList.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateRoleAsync(Role role)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var queryString = "update roles set name=@name,status=@status where id=@id";
                    var parameters = new DynamicParameters();
                    parameters.Add("name", role.Name, DbType.String);
                    parameters.Add("status", role.Status, DbType.Boolean);
                    parameters.Add("id", role.Id.ToString(), DbType.String);
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
    }
}
