using DIBN.Areas.Admin.Data;
using DIBN.Areas.Admin.IRepository;
using DIBN.Areas.Admin.Repository;
using DIBN.IService;
using DIBN.Service;
using Microsoft.Extensions.DependencyInjection;

namespace DIBN.Models.Data
{
    public static class DIConfig
    {
        public static IServiceCollection RegisterConfiguration(this IServiceCollection services)
        {
            //Super Admin Repository Registration
            services.AddTransient<IUserRepository,UserRepository>();
            services.AddTransient<IEnquiryFormRepository, EnquiryFormRepository>();
            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IModuleRepository, ModuleRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();
            services.AddTransient<IBannerRepository, BannerRepository>();
            services.AddTransient<IPortalBalanceExpensesRepository, PortalBalanceExpensesRepository>();
            services.AddTransient<ICompanyDocumentTypeRepository, CompanyDocumentTypeRepository>();
            services.AddTransient<IShareholderRepository, ShareholderRepository>();
            services.AddTransient<IEmployeeServiceRepository, EmployeeServiceRepository>();
            services.AddTransient<ICompanyServiceRepository, CompanyServiceRepository>();
            services.AddTransient<ISupportTicketRepository, SupportTicketRepository>();
            services.AddTransient<IImportReminderNotificationRepository, ImportReminderNotificationRepository>();
            services.AddTransient<IServiceFormRepository, ServiceFormRepository>();
            services.AddTransient<ISalesPersonRepository, SalesPersonRepository>();
            services.AddTransient<ICompanyInvoiceRepository, CompanyInvoiceRepository>();
            services.AddTransient<IMessageTemplateRepository, MessageTemplateRepository>();
            services.AddTransient<ILogRepository, LogRepository>();
            services.AddTransient<IServiceRequestStatusRepository, ServiceRequestStatusRepository>();
            services.AddTransient<IGeneratePIRepository, GeneratePIRepository>();
            services.AddTransient<IReportManagementRepository, ReportManagementRepository>();
            services.AddTransient<IRMTeamManagementRepository, RMTeamManagementRepository>();
            services.AddTransient<ICompanySubTypeRepository, CompanySubTypeRepository>();


            // User/Admin Service Registartion
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IUserPermissionService, UserPermissionService>();
            services.AddTransient<IUserCompanyService, UserCompanyService>();
            services.AddTransient<IUserEmployeesService,UserEmployeesService>();
            services.AddTransient<IFileUploaderService,FileUploaderService>();
            services.AddTransient<IBannerImageService,BannerImageService>();
            services.AddTransient<IPortalBalanceService,PortalBalanceService>();
            services.AddTransient<IUserShareholderService, UserShareholderService>();
            services.AddTransient<IImportantReminderService, ImportantReminderService>();
            services.AddTransient<IEmployeeServiceList, EmployeeServiceList>();
            services.AddTransient<ISupportTicketService, SupportTicketService>();
            services.AddTransient<ICompanyServiceList, CompanyServiceList>();
            services.AddTransient<IServicesFormService, ServicesFormService>();
            services.AddTransient<IAccountManagementService, AccountManagementService>();
            services.AddTransient<IEnquiryFormService, EnquiryFormService>();


            services.AddSingleton<SecuritySettings>();
            services.AddSingleton<EncryptionService>();

            return services;
        }
    }
}
