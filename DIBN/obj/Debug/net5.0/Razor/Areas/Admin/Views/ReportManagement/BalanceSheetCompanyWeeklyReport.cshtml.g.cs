#pragma checksum "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "d52cbc8c7d19e53db25d10f68071bcffb18d9b52160c1905df92381966f68c4c"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_ReportManagement_BalanceSheetCompanyWeeklyReport), @"mvc.1.0.view", @"/Areas/Admin/Views/ReportManagement/BalanceSheetCompanyWeeklyReport.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"d52cbc8c7d19e53db25d10f68071bcffb18d9b52160c1905df92381966f68c4c", @"/Areas/Admin/Views/ReportManagement/BalanceSheetCompanyWeeklyReport.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"605a41b4408e5d1ff247f7264e35f4305eb9eeecf14a77cb4f30041778bbf31f", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Areas_Admin_Views_ReportManagement_BalanceSheetCompanyWeeklyReport : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<DIBN.Areas.Admin.Models.BalanceSheetWeeklyReportByCompanyIdModel>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/jquery/dist/jquery.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/datatables.net-responsive/js/dataTables.responsive.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/datatables.net-responsive-bs4/js/responsive.bootstrap4.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
  
    ViewData["Title"] = "Balance Sheet Weekly Report of Company";

#line default
#line hidden
#nullable disable

            WriteLiteral(@"<style>
    .wrapper {
        display: flex;
        justify-content: center;
        align-items: center;
    }

    button {
        display: flex;
        justify-content: center;
        align-items: center;
        position: relative;
        color: #333333;
        border: none;
        background: transparent;
        font-size: 1rem;
        line-height: 1.5rem;
        padding: 1rem 2rem;
    }

        button > svg {
            position: absolute;
            width: 100%;
            height: 100%;
        }

            button > svg > rect {
                fill: none;
                stroke: #333333;
                stroke-width: 2px;
                stroke-dasharray: 240 160 240 160;
                stroke-dashoffset: 0;
                animation: pathRect 2s linear infinite;
                width: calc(100% - 2px);
                height: calc(100% - 2px);
            }

    ");
            WriteLiteral(@"@keyframes pathRect {
        25% {
            stroke-dashoffset: 100;
        }

        50% {
            stroke-dashoffset: 200;
        }

        75% {
            stroke-dashoffset: 300;
        }

        100% {
            stroke-dashoffset: 400;
        }
    }
</style>

<div class=""row"">
    <div class=""col-lg-12"">
        <a");
            BeginWriteAttribute("href", " href=\"", 1447, "\"", 1544, 1);
            WriteAttributeValue("", 1454, 
#nullable restore
#line 63 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                  Url.Action("CompanyBalanceSheetReport","ReportManagement",new{companyId=Model.CompanyId})

#line default
#line hidden
#nullable disable
            , 1454, 90, false);
            EndWriteAttribute();
            WriteLiteral(" class=\"btn btn-theme float-end me-2 mb-2\">Back</a>\r\n        <a role=\"button\" class=\"btn btn-theme float-end me-2 mb-2 Details\"");
            BeginWriteAttribute("onclick", " onclick=\"", 1672, "\"", 1750, 7);
            WriteAttributeValue("", 1682, "ExportAsExcel(\'", 1682, 15, true);
            WriteAttributeValue("", 1697, 
#nullable restore
#line 64 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                                                                                    Model.FromDates

#line default
#line hidden
#nullable disable
            , 1697, 16, false);
            WriteAttributeValue("", 1713, "\',\'", 1713, 3, true);
            WriteAttributeValue("", 1716, 
#nullable restore
#line 64 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                                                                                                       Model.ToDates

#line default
#line hidden
#nullable disable
            , 1716, 14, false);
            WriteAttributeValue("", 1730, "\',", 1730, 2, true);
            WriteAttributeValue("", 1732, 
#nullable restore
#line 64 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                                                                                                                       Model.CompanyId

#line default
#line hidden
#nullable disable
            , 1732, 16, false);
            WriteAttributeValue("", 1748, ");", 1748, 2, true);
            EndWriteAttribute();
            WriteLiteral(">Export Excel</a>\r\n        <a role=\"button\" class=\"btn btn-theme float-end me-2 mb-2 Details\"");
            BeginWriteAttribute("onclick", " onclick=\"", 1844, "\"", 1920, 7);
            WriteAttributeValue("", 1854, "ExportAsPdf(\'", 1854, 13, true);
            WriteAttributeValue("", 1867, 
#nullable restore
#line 65 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                                                                                  Model.FromDates

#line default
#line hidden
#nullable disable
            , 1867, 16, false);
            WriteAttributeValue("", 1883, "\',\'", 1883, 3, true);
            WriteAttributeValue("", 1886, 
#nullable restore
#line 65 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                                                                                                     Model.ToDates

#line default
#line hidden
#nullable disable
            , 1886, 14, false);
            WriteAttributeValue("", 1900, "\',", 1900, 2, true);
            WriteAttributeValue("", 1902, 
#nullable restore
#line 65 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                                                                                                                     Model.CompanyId

#line default
#line hidden
#nullable disable
            , 1902, 16, false);
            WriteAttributeValue("", 1918, ");", 1918, 2, true);
            EndWriteAttribute();
            WriteLiteral(@">Export PDF</a>
    </div>
</div>

<div class=""row Details"">
    <div class=""col-12"">
        <div class=""card"">
            <div class=""card-header"">
                <h4 class=""text-center front-text-theme fw-bold"">Balance Sheet Weekly Report From ");
            Write(
#nullable restore
#line 73 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                                                                                   Model.FromDate

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(" till ");
            Write(
#nullable restore
#line 73 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                                                                                                        Model.ToDate

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(" of ");
            Write(
#nullable restore
#line 73 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                                                                                                                         Model.CompanyName

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("</h4>\r\n                <input type=\"hidden\" name=\"fromDate\" id=\"fromDate\"");
            BeginWriteAttribute("value", " value=\"", 2307, "\"", 2331, 1);
            WriteAttributeValue("", 2315, 
#nullable restore
#line 74 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                                                           Model.FromDates

#line default
#line hidden
#nullable disable
            , 2315, 16, false);
            EndWriteAttribute();
            WriteLiteral(" />\r\n                <input type=\"hidden\" name=\"toDate\" id=\"toDate\"");
            BeginWriteAttribute("value", " value=\"", 2399, "\"", 2421, 1);
            WriteAttributeValue("", 2407, 
#nullable restore
#line 75 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                                                       Model.ToDates

#line default
#line hidden
#nullable disable
            , 2407, 14, false);
            EndWriteAttribute();
            WriteLiteral(" />\r\n                <input type=\"hidden\" name=\"companyId\" id=\"companyId\"");
            BeginWriteAttribute("value", " value=\"", 2495, "\"", 2519, 1);
            WriteAttributeValue("", 2503, 
#nullable restore
#line 76 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                                                             Model.CompanyId

#line default
#line hidden
#nullable disable
            , 2503, 16, false);
            EndWriteAttribute();
            WriteLiteral(@" />

                <div class=""row col-md-12 mt-4 mb-3"">
                    <div class=""float-left text-center"">
                        <span class=""text-center float-left front-text-theme h6 fw-bold"">Total Credit :</span><span class=""text-center float-left text-success h6 fw-bold""> ");
            Write(
#nullable restore
#line 80 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                                                                                                                                                             Model.TotalCredit

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(@" AED</span>
                        <span class=""text-center float-left front-text-theme h6 fw-bold"">||</span>
                        <span class=""text-center float-left front-text-theme h6 fw-bold"">Total Debit : </span><span class=""text-center float-left text-danger h6 fw-bold"">-");
            Write(
#nullable restore
#line 82 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                                                                                                                                                            Model.TotalDebit

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(" AED</span>\r\n                    </div>\r\n                </div>\r\n\r\n                <div class=\"row col-md-12 mt-4 mb-3\">\r\n                    <div class=\"float-left text-center\">\r\n");
#nullable restore
#line 88 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                         if (!Model.TotalBalance.StartsWith("-"))
                        {

#line default
#line hidden
#nullable disable

            WriteLiteral("                            <span class=\"text-center float-left front-text-theme h6 fw-bold\">Total Balance :</span>\r\n");
            WriteLiteral("                            <span class=\"text-center float-left text-success h6 fw-bold\"> ");
            Write(
#nullable restore
#line 92 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                                                                           Model.TotalBalance

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(" AED </span>\r\n");
#nullable restore
#line 93 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                        }
                        else
                        {

#line default
#line hidden
#nullable disable

            WriteLiteral("                            <span class=\"text-center float-left front-text-theme h6 fw-bold\">Total Balance :</span>\r\n");
            WriteLiteral("                            <span class=\"text-center float-left text-danger h6 fw-bold\"> ");
            Write(
#nullable restore
#line 98 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                                                                          Model.TotalBalance

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(" AED </span>\r\n");
#nullable restore
#line 99 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                        }

#line default
#line hidden
#nullable disable

            WriteLiteral(@"                    </div>
                </div>
            </div>
            <div class=""card-body"">
                <div class=""table-rep-plugin"">
                    <div class=""table-responsive mb-0"" data-pattern=""priority-columns"">
                        <table id=""datatable"" class=""table table-bordered table-striped dt-responsive nowrap"" style=""border-collapse:collapse; border-spacing:0; width:100%;"">
                            <thead>
                                <tr>
                                    <th>
                                        Transaction Id
                                    </th>
                                    <th data-sort='YYYYMMDD'>
                                        Transaction Date
                                    </th>
                                    <th>
                                        Company
                                    </th>
                                    <th>
                                        Descri");
            WriteLiteral(@"ption
                                    </th>
                                    <th>
                                        Credit(AED)
                                    </th>
                                    <th>
                                        Debit(AED)
                                    </th>
                                    <th>
                                        Balance(AED)
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div class=""modal fade bd-example-modal"" id=""multipleDownloadErrorModal"" role=""dialog"" data-bs-keyboard=""false"" data-bs-backdrop=""static"" style=""z-index:10000 !important;"">
    <div class=""modal-dialog modal-dialog-centered modal"" role=""document"">
        <div c");
            WriteLiteral(@"lass=""modal-content"">
            <div class=""modal-body pt-0"">
                <div class=""d-flex justify-content-center align-items-center"">
                    <div class=""mt-5"">
                        <div class='wrapper'>
                            <button>
                                <span class=""fw-bolder fs-5"">Downloading...</span>
                                <svg>
                                    <rect x=""1"" y=""1""></rect>
                                </svg>
                            </button>
                        </div>
                    </div>
                </div>
                <div class=""col-md-12 mt-5"">
                    <p class=""text-center fw-bolder fs-5"">Please wait for while , your report (PDF/Excel) file is currently downloading..</p>
                </div>
            </div>
        </div>
    </div>
</div>

");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "d52cbc8c7d19e53db25d10f68071bcffb18d9b52160c1905df92381966f68c4c20975", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "d52cbc8c7d19e53db25d10f68071bcffb18d9b52160c1905df92381966f68c4c22043", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "d52cbc8c7d19e53db25d10f68071bcffb18d9b52160c1905df92381966f68c4c23107", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"

<script>
    $(document).ready(function () {
        $.fn.dataTable.ext.errMode = 'none';

        var fromDate = $(""#fromDate"").val();
        var toDate = $(""#toDate"").val();
        var companyId = $(""#companyId"").val();

        var url = """);
            Write(
#nullable restore
#line 179 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                    Url.Action("BalanceSheetCompanyWeeklyPaginationReport","ReportManagement")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(@""";
        url = url + ""?fromDate="" + fromDate + ""&toDate="" + toDate + ""&companyId="" + companyId;

        $(""#datatable"").DataTable({
            ""processing"": true,
            ""serverSide"": true,
            ""filter"": false,
            ""ajax"": {
                ""type"": ""post"",
                ""datatype"": ""json"",
                ""data"": function () {
                    var info = $('#datatable').DataTable().page.info();
                    $('#datatable').DataTable().ajax.url(
                        url
                    );
                }
            },
            ""columns"": [
                { ""data"": ""transactionId"", ""name"": ""Transaction Id"" },
                { ""data"": ""date"", ""name"": ""Transaction Date"" },
                { ""data"": ""companyName"", ""name"": ""Company"" },
                { ""data"": ""description"", ""name"": ""Description"" },
                {
                    ""data"": function (data, full, meta) {
                        if (data.type == ""Credit"") {
            ");
            WriteLiteral(@"                return '<span class=""text-success"">' + data.grandTotal + '</span>';
                        }
                        else {
                            return '---';
                        }
                    },
                    ""name"": ""Credit(AED)""
                },
                {
                    ""data"": function (data, full, meta) {
                        if (data.type == ""Debit"") {
                            return '<span class=""text-danger"">-' + data.grandTotal + '</span>';
                        }
                        else {
                            return '---';
                        }
                    },
                    ""name"": ""Debit(AED)""
                },
                { ""data"": ""balanceTotal"", ""name"": ""Balance(AED)"" },
            ],
            ""language"": {
                'processing': '<div class=""spinner-2""><div class=""center-div-2""><div class=""loader-circle-1""></div></div></div>'
            },
            responsive");
            WriteLiteral(@": true,
            paging: true,
            pagingType: 'simple_numbers',
            ""bInfo"": true,
            ordering: true,
            ""aLengthMenu"": [20, 30, 50],
            searching: true,
            'columnDefs': [{
                'targets': [6],
                'orderable': false,
            }],
            dom: ""<'row'<'col-sm-3'l><'col-sm-3'f><'col-sm-6'p>>"" +
                ""<'row'<'col-sm-12'tr>>"" +
                ""<'row'<'col-sm-5'i><'col-sm-7'p>>"",
        });
    });
</script>
<script>
    function ExportAsExcel(fromDate, toDate, companyId) {
        $mymodal = $(""#multipleDownloadErrorModal"");
        $mymodal.find(""div.modal-body"");
        $mymodal.modal(""show"");

        $.ajax({
            url: '");
            Write(
#nullable restore
#line 252 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                   Url.Action("BalanceSheetCompanyWeeklyReportExcel", "ReportManagement")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(@"',
            type: 'GET',
            data: { 'fromDate': fromDate, 'toDate': toDate, 'companyId': companyId },
            success: function (result) {
                $mymodal = $(""#multipleDownloadErrorModal"");
                $mymodal.find(""div.modal-body"");
                $mymodal.modal(""hide"");

                if (result == ""Success"") {
                    var url = """);
            Write(
#nullable restore
#line 261 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                Url.Action("DownloadBalanceSheetCompanyWeeklyReportExcel","ReportManagement")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(@""";
                    url = url + ""?fromDate="" + fromDate + ""&toDate="" + toDate + ""&companyId="" + companyId;
                    window.location.href = url;

                    setTimeout(function () {
                        window.location.reload();
                    }, 2000);
                }
            },
            error: function () {
                alert(""Something went wrong please try again after sometimes."");
                setTimeout(function () {
                    window.location.reload();
                }, 2000);
            }
        });
    };

    function ExportAsPdf(fromDate, toDate, companyId) {
        $mymodal = $(""#multipleDownloadErrorModal"");
        $mymodal.find(""div.modal-body"");
        $mymodal.modal(""show"");

        $.ajax({
            url: '");
            Write(
#nullable restore
#line 285 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                   Url.Action("BalanceSheetCompanyWeeklyReportPdf", "ReportManagement")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(@"',
            type: 'GET',
            data: { 'fromDate': fromDate, 'toDate': toDate, 'companyId': companyId },
            success: function (result) {
                $mymodal = $(""#multipleDownloadErrorModal"");
                $mymodal.find(""div.modal-body"");
                $mymodal.modal(""hide"");

                if (result == ""Success"") {
                    var url = """);
            Write(
#nullable restore
#line 294 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                                Url.Action("DownloadBalanceSheetCompanyWeeklyReportPdf","ReportManagement")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(@""";
                    url = url + ""?fromDate="" + fromDate + ""&toDate="" + toDate + ""&companyId="" + companyId;
                    window.location.href = url;

                    setTimeout(function () {
                        window.location.reload();
                    }, 2000);
                }
            },
            error: function () {
                alert(""Something went wrong please try again after sometimes."");
                setTimeout(function () {
                    window.location.reload();
                }, 2000);
            }
        });
    };
</script>
<script>
    $(function () {
        $(""#Insert"").hide();
        $(""#View"").hide();
        $("".Update"").hide();
        $("".Delete"").hide();
        $("".Details"").hide();
    });
</script>
<script>
    $(function () {
        $(""#loader"").show();
        var module = $(""#Module"").val();
        $.ajax({
            url: """);
            Write(
#nullable restore
#line 326 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                   Url.Action("GetRolePermissionsName", "Permission")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(@""",
            method: ""GET"",
            data: { ""Module"": ""ReportManagement"" },
            async: false,
            contentType: ""application/json; charset=utf-8"",
            dataType: ""json"",
            success: function (RoleResponse) {
                $(""#loader"").show();
                $.ajax({
                    url: """);
            Write(
#nullable restore
#line 335 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyWeeklyReport.cshtml"
                           Url.Action("GetUserPermissionsName", "Permission")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(@""",
                    method: ""GET"",
                    data: { ""Module"": ""ReportManagement"" },
                    async: false,
                    contentType: ""application/json; charset=utf-8"",
                    dataType: ""json"",
                    success: function (UserResponse) {
                        $(""#loader"").show();
                        if (UserResponse.length > 0) {
                            for (let i = 0; i < UserResponse.length; i++) {
                                $(""#"" + UserResponse[i]).show();
                                if (UserResponse[i] == ""View"") {
                                    $("".Details"").show();
                                }
                                if (UserResponse[i] == ""Update"") {
                                    $(""."" + UserResponse[i]).show();
                                }
                                if (UserResponse[i] == ""Delete"") {
                                    $(""."" + UserResponse[i]).show();
        ");
            WriteLiteral(@"                        }
                            }
                            $(""#loader"").hide();
                        }
                        else if (RoleResponse.length > 0) {
                            for (let i = 0; i < RoleResponse.length; i++) {
                                $(""#"" + RoleResponse[i]).show();
                                if (RoleResponse[i] == ""View"") {
                                    $("".Details"").show();
                                }
                                if (RoleResponse[i] == ""Update"") {
                                    $(""."" + RoleResponse[i]).show();
                                }
                                if (RoleResponse[i] == ""Delete"") {
                                    $(""."" + RoleResponse[i]).show();
                                }
                            }
                            $(""#loader"").hide();

                        } else {
                            alert(""You don't have any permissi");
            WriteLiteral("on for this module.\");\r\n                            $(\"#loader\").hide();\r\n                        }\r\n                    }\r\n                })\r\n            }\r\n        });\r\n        $(\"#loader\").hide();\r\n    });\r\n</script>\r\n\r\n\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<DIBN.Areas.Admin.Models.BalanceSheetWeeklyReportByCompanyIdModel> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
