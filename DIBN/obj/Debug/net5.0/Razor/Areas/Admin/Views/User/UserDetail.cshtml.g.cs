#pragma checksum "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "f4dda26ef422e38d479934b568a526fcdd492d634d7f0ad91e810fc076388201"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_User_UserDetail), @"mvc.1.0.view", @"/Areas/Admin/Views/User/UserDetail.cshtml")]
namespace AspNetCore
{
    #line default
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;
    using global::Microsoft.AspNetCore.Mvc;
    using global::Microsoft.AspNetCore.Mvc.Rendering;
    using global::Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\_ViewImports.cshtml"
using DIBN

#nullable disable
    ;
#nullable restore
#line 2 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\_ViewImports.cshtml"
using DIBN.Areas.Admin.Models

#nullable disable
    ;
#nullable restore
#line 3 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\_ViewImports.cshtml"
using DIBN.Areas.Admin.Data

#nullable disable
    ;
#nullable restore
#line 4 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\_ViewImports.cshtml"
using DIBN.Areas.Admin.IRepository

#nullable disable
    ;
#nullable restore
#line 5 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\_ViewImports.cshtml"
using DIBN.Areas.Admin.Repository

#line default
#line hidden
#nullable disable
    ;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"f4dda26ef422e38d479934b568a526fcdd492d634d7f0ad91e810fc076388201", @"/Areas/Admin/Views/User/UserDetail.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"605a41b4408e5d1ff247f7264e35f4305eb9eeecf14a77cb4f30041778bbf31f", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Areas_Admin_Views_User_UserDetail : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<DIBN.Areas.Admin.Models.UserViewModel>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
  
    ViewData["Title"] = "UserDetail";

#line default
#line hidden
#nullable disable

            WriteLiteral("<div id=\"loader\">\r\n    <div class=\"spinner-1\">\r\n        <div class=\"center-div-1\">\r\n            <div class=\"loader-circle-75\">\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n<input type=\"hidden\" name=\"Module\" id=\"Module\"");
            BeginWriteAttribute("value", " value=\"", 324, "\"", 345, 1);
            WriteAttributeValue("", 332, 
#nullable restore
#line 14 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                                                       Model.Module

#line default
#line hidden
#nullable disable
            , 332, 13, false);
            EndWriteAttribute();
            WriteLiteral(@" />

<div class=""row"">
    <div class=""col-12"">
        <div class=""card"">
            <div class=""card-body"">
                <div class=""row"">
                    <div class=""card-title"">
                        <h5 class=""text-dark float-start"">");
            Write(
#nullable restore
#line 22 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                                                           Html.DisplayFor(model => model.FirstName)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(" ");
            Write(
#nullable restore
#line 22 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                                                                                                      Html.Raw(" ")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(" ");
            Write(
#nullable restore
#line 22 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                                                                                                                     Html.DisplayFor(model => model.LastName)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\'s Detail</h5>\r\n");
#nullable restore
#line 23 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         if(Model.IsActive && Model.IsLogin){

#line default
#line hidden
#nullable disable

            WriteLiteral("                            <span class=\"float-end fas fa-check text-success\" style=\"font-size:26px;\"></span>\r\n");
#nullable restore
#line 25 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                        }
                        else{

#line default
#line hidden
#nullable disable

            WriteLiteral("                            <span class=\"float-end fas fa-times text-danger\" style=\"font-size:26px;\"></span>\r\n");
#nullable restore
#line 28 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                        }

#line default
#line hidden
#nullable disable

            WriteLiteral("                    </div>\r\n                <hr/>\r\n                </div>\r\n                <dl class=\"row\">\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 34 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayNameFor(model => model.AccountNumber)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 37 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayFor(model => model.AccountNumber)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        Employee Name\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 43 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayFor(model => model.FirstName)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(" ");
            Write(
#nullable restore
#line 43 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                                                                    Html.Raw(" ")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(" ");
            Write(
#nullable restore
#line 43 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                                                                                   Html.DisplayFor(model => model.LastName)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 46 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayNameFor(model => model.Nationality)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 49 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayFor(model => model.Nationality)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 52 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayNameFor(model => model.PassportNumber)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 55 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayFor(model => model.PassportNumber)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 58 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayNameFor(model => model.PassportExpiryDate)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 61 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayFor(model => model.PassportExpiryDate)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 64 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayNameFor(model => model.Designation)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n");
            WriteLiteral("                            ");
            Write(
#nullable restore
#line 73 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                             Html.DisplayFor(model => model.Designation)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n");
            WriteLiteral("                        \r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 78 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayNameFor(model => model.VisaExpiryDate)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 81 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayFor(model => model.VisaExpiryDate)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 84 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayNameFor(model => model.InsuranceCompany)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 87 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayFor(model => model.InsuranceCompany)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 90 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayNameFor(model => model.InsuranceExpiryDate)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 93 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayFor(model => model.InsuranceExpiryDate)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 96 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayNameFor(model => model.EmailID)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 99 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayFor(model => model.EmailID)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 102 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayNameFor(model => model.PhoneNumber)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 105 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.DisplayFor(model => model.PhoneNumber)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 108 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.Raw("Is Login Allowed?")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n");
#nullable restore
#line 111 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         if (Model.IsLogin)
                        {
                            

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 113 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                             Html.Raw("Yes")

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 113 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                                            
                        }
                        else
                        {
                            

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 117 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                             Html.Raw("No")

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 117 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                                           
                        }

#line default
#line hidden
#nullable disable

            WriteLiteral("                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 121 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         Html.Raw("Is User Active?")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n");
#nullable restore
#line 124 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                         if (Model.IsActive)
                        {
                            

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 126 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                             Html.Raw("Yes")

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 126 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                                            
                        }
                        else
                        {
                            

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 130 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                             Html.Raw("No")

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 130 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\User\UserDetail.cshtml"
                                           
                        }

#line default
#line hidden
#nullable disable

            WriteLiteral("                    </dd>\r\n");
            WriteLiteral(@"                </dl>
                <div>
                    <a href=""javascript:history.go(-1);"" class=""btn btn-theme text-white"">Back to List</a>
                </div>

            </div>
        </div>
    </div>
</div>

<script>
    $(document).ready(function () {
        $(""#loader"").hide();
    });
</script>");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<DIBN.Areas.Admin.Models.UserViewModel> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
