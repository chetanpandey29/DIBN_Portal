#pragma checksum "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "238846b17ef6c51c87f3b5769e8cea603d662973aa1059c2160c8af47fa32e97"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_SalesPerson_SalesPersonDetails), @"mvc.1.0.view", @"/Areas/Admin/Views/SalesPerson/SalesPersonDetails.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"238846b17ef6c51c87f3b5769e8cea603d662973aa1059c2160c8af47fa32e97", @"/Areas/Admin/Views/SalesPerson/SalesPersonDetails.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"605a41b4408e5d1ff247f7264e35f4305eb9eeecf14a77cb4f30041778bbf31f", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Areas_Admin_Views_SalesPerson_SalesPersonDetails : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<DIBN.Areas.Admin.Models.SalesPersonViewModel>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
  
    ViewData["Title"] = "SalesPersonDetails";

#line default
#line hidden
#nullable disable

            WriteLiteral("\r\n");
#nullable restore
#line 6 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
  
    string[] companies = { };
    int count = 0;
    int index = 1;

#line default
#line hidden
#nullable disable

            WriteLiteral("<div id=\"loader\">\r\n    <div class=\"spinner-1\">\r\n        <div class=\"center-div-1\">\r\n            <div class=\"loader-circle-75\">\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n<div class=\"row\">\r\n    <div class=\"col-md-12\">\r\n        <a");
            BeginWriteAttribute("href", " href=\"", 429, "\"", 470, 1);
            WriteAttributeValue("", 436, 
#nullable restore
#line 21 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                  Url.Action("Index","SalesPerson")

#line default
#line hidden
#nullable disable
            , 436, 34, false);
            EndWriteAttribute();
            WriteLiteral(@" class=""btn btn-theme"" style=""float:right;margin-bottom:15px;"">Back to List</a>
    </div>
</div>
<div class=""row"">
    <div class=""col-12"">
        <div class=""card"">
            <div class=""card-body"">
                <div class=""row"">
                    <div class=""card-title"">
                        <h5 class=""text-dark float-start"">Sales Person Details</h5>
");
#nullable restore
#line 31 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         if (Model.IsActive && Model.IsLogin)
                        {

#line default
#line hidden
#nullable disable

            WriteLiteral("                            <span class=\"float-end fas fa-check text-success\" style=\"font-size:26px;\"></span>\r\n");
#nullable restore
#line 34 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                        }
                        else
                        {

#line default
#line hidden
#nullable disable

            WriteLiteral("                            <span class=\"float-end fas fa-times text-danger\" style=\"font-size:26px;\"></span>\r\n");
#nullable restore
#line 38 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                        }

#line default
#line hidden
#nullable disable

            WriteLiteral(@"                    </div>
                <hr/>
                </div>
                <dl class=""row"">
                    <dt class=""col-sm-4"">
                        Name
                    </dt>
                    <dd class=""col-sm-8"">
                        ");
            Write(
#nullable restore
#line 47 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayFor(model => model.FirstName)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(" ");
            Write(
#nullable restore
#line 47 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                                                                    Html.Raw(" ")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(" ");
            Write(
#nullable restore
#line 47 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                                                                                   Html.DisplayFor(model => model.LastName)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 50 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayNameFor(model => model.PassportNumber)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 53 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayFor(model => model.PassportNumber)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 56 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayNameFor(model => model.PassportExpiryDate)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 59 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayFor(model => model.PassportExpiryDate)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 62 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayNameFor(model => model.Designation)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 65 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.Raw("Sales Person")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 68 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayNameFor(model => model.VisaExpiryDate)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 71 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayFor(model => model.VisaExpiryDate)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 74 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayNameFor(model => model.InsuarnceCompany)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 77 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayFor(model => model.InsuarnceCompany)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 80 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayNameFor(model => model.InsuranceExpiryDate)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 83 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayFor(model => model.InsuranceExpiryDate)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 86 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayNameFor(model => model.EmailId)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 89 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayFor(model => model.EmailId)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 92 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayNameFor(model => model.PhoneNumber)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 95 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayFor(model => model.PhoneNumber)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 98 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayNameFor(model => model.CountryOfRecidence)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n                        ");
            Write(
#nullable restore
#line 101 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.DisplayFor(model => model.CountryOfRecidence)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 104 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.Raw("Is Login Allowed?")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n");
#nullable restore
#line 107 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         if (Model.IsLogin)
                        {
                            

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 109 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                             Html.Raw("Yes")

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 109 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                                            
                        }
                        else
                        {
                            

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 113 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                             Html.Raw("No")

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 113 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                                           
                        }

#line default
#line hidden
#nullable disable

            WriteLiteral("                    </dd>\r\n                    <dt class=\"col-sm-4\">\r\n                        ");
            Write(
#nullable restore
#line 117 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         Html.Raw("Is User Active?")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                    </dt>\r\n                    <dd class=\"col-sm-8\">\r\n");
#nullable restore
#line 120 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         if (Model.IsActive)
                        {
                            

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 122 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                             Html.Raw("Yes")

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 122 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                                            
                        }
                        else
                        {
                            

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 126 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                             Html.Raw("No")

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 126 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                                           
                        }

#line default
#line hidden
#nullable disable

            WriteLiteral(@"                    </dd>
                </dl>
            </div>
        </div>
    </div>
</div>
<div class=""row"">
    <div class=""col-12"">
        <div class=""card"">
            <div class=""card-body"">
                <div class=""row"">
                    <div class=""card-title"">
                    <h5 class=""text-dark"">Assigned Company(s)</h5>
                </div>
                <hr/>
                </div>
                <dl class=""row"">
                    <dt class=""col-sm-4"">
                        Company(s) : 
                    </dt>
                    <dd class=""col-sm-8"">
");
#nullable restore
#line 149 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                         if (@Model.Company.Contains(","))
                        {
                            companies = @Model.Company.Split(",");
                            count=companies.Count()-1;
 
                            for (var i=0;i<=count;i++){

                                

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 156 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                                 Html.Raw(""+@index+".")

#line default
#line hidden
#nullable disable
            );
            Write(
#nullable restore
#line 156 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                                                          Html.Raw(" ")

#line default
#line hidden
#nullable disable
            );
            Write(
#nullable restore
#line 156 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                                                                         Html.Raw(companies[i])

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 156 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                                                                                               
                                index++;

#line default
#line hidden
#nullable disable

            WriteLiteral("                                <br/>\r\n");
#nullable restore
#line 159 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                            }
                        }
                        else
                        {
                            

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 163 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                             Html.DisplayFor(model => model.Company)

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 163 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\SalesPersonDetails.cshtml"
                                                                    
                        }

#line default
#line hidden
#nullable disable

            WriteLiteral("                        \r\n                    </dd>\r\n                </dl>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n\r\n\r\n<script>\r\n    $(document).ready(function () {\r\n        $(\"#loader\").hide();\r\n    });\r\n</script>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<DIBN.Areas.Admin.Models.SalesPersonViewModel> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
