using HRM.Authorization;
using HRM.Interfaces;
using HRM.Repository;
using HRM.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using ServiceManagementSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<IRoleService,RoleService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<BaseService>();
builder.Services.AddScoped<ICompaniesService,CompaniesService>();
builder.Services.AddScoped<IBranchService,BranchService>();
builder.Services.AddScoped<IDepartmentService,DepartmentService>();
builder.Services.AddScoped<IAddUserService,AddUserService>();
builder.Services.AddScoped<IEmployeeService,EmployeeService>();
builder.Services.AddScoped<IDesignationService,DesignationService>();
builder.Services.AddScoped<IDutySlotService,DutySlotService>();
builder.Services.AddScoped<ILeaveTypeService,LeaveTypeService>();
builder.Services.AddScoped<ILeaveAllotmentService,LeaveAllotmentService>();
builder.Services.AddScoped<ILateAttendanceService,LateAttendanceService>();
builder.Services.AddScoped<IOffDaysService,OffDaysService>();
builder.Services.AddScoped<IHolidayCalendarService,HolidayCalendarService>();
builder.Services.AddScoped<ISalaryHeadsService,SalaryHeadsService>();
//builder.Services.AddScoped<ISalaryService,SalaryService>();
builder.Services.AddScoped<ISalaryHeadsService, SalaryHeadsService>();
builder.Services.AddScoped<IOvertimeService, OvertimeService>();
builder.Services.AddScoped<IBonusCalculateService, BonusCalculateService>();
builder.Services.AddScoped<IBonusTypeService, BonusTypeService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<ILoanApplicationService, LoanApplicationService>();
builder.Services.AddScoped<IRecommendLoanApplicationService, RecommendLoanApplicationService>();
builder.Services.AddScoped<ILoanApprovalService,LoanApprovalService>();
builder.Services.AddScoped<IEmployeeLoanService,EmployeeLoanService>();
builder.Services.AddScoped<IShowInstalmentListService,ShowInstalmentListService>();
builder.Services.AddScoped<ILoanInstallmentService,LoanInstallmentService>();
builder.Services.AddScoped<IAdditionalInfoService,AdditionalInfoService>();
builder.Services.AddScoped<ITaxtSlabSetupService,TaxtSlabSetupService>();
builder.Services.AddScoped<IExcludeTaxService,ExcludeTaxService>();
builder.Services.AddScoped<IPfContributionService,PfContributionService>();
builder.Services.AddScoped<IPFInterestService,PFInterestService>();
builder.Services.AddScoped<IGratuityCalculateSetupService,GratuityCalculateSetupService>();
builder.Services.AddScoped<ISeparationReasonsService,SeparationReasonsService>();
builder.Services.AddScoped<IEmployeeSeparationService,EmployeeSeparationService>();
builder.Services.AddScoped<IGratuityCalculateService,GratuityCalculateService>();
builder.Services.AddScoped<ISalaryCalculationService, SalaryCalculationService>();
builder.Services.AddScoped<ISalaryCreateService, SalaryCreateService>();
builder.Services.AddScoped<ILeaveApplicationService, LeaveApplicationService>();



builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.  
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddPermissionAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}




app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
