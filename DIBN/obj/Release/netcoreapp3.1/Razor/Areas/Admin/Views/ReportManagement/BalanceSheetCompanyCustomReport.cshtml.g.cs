#pragma checksum "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "b3ba08ee4b1aab87fcf27465eebe609dab4ca0979d706a10587993e17e8540b7"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_ReportManagement_BalanceSheetCompanyCustomReport), @"mvc.1.0.view", @"/Areas/Admin/Views/ReportManagement/BalanceSheetCompanyCustomReport.cshtml")]
namespace AspNetCore
{
    #line hidden
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;
    using global::Microsoft.AspNetCore.Mvc;
    using global::Microsoft.AspNetCore.Mvc.Rendering;
    using global::Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\_ViewImports.cshtml"
using DIBN;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\_ViewImports.cshtml"
using DIBN.Areas.Admin.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\_ViewImports.cshtml"
using DIBN.Areas.Admin.Data;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\_ViewImports.cshtml"
using DIBN.Areas.Admin.IRepository;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\_ViewImports.cshtml"
using DIBN.Areas.Admin.Repository;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA256", @"b3ba08ee4b1aab87fcf27465eebe609dab4ca0979d706a10587993e17e8540b7", @"/Areas/Admin/Views/ReportManagement/BalanceSheetCompanyCustomReport.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA256", @"605a41b4408e5d1ff247f7264e35f4305eb9eeecf14a77cb4f30041778bbf31f", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Areas_Admin_Views_ReportManagement_BalanceSheetCompanyCustomReport : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<DIBN.Areas.Admin.Models.BalanceSheetCustomReportByCompanyIdModel>
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
#line 2 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
  
    ViewData["Title"] = "Balance Sheet Custom Report of Company";

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
            BeginWriteAttribute("href", " href=\"", 1265, "\"", 1362, 1);
#nullable restore
#line 62 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
WriteAttributeValue("", 1272, Url.Action("CompanyBalanceSheetReport","ReportManagement",new{companyId=Model.CompanyId}), 1272, 90, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\"btn btn-blue float-end me-2 mb-2\">Back</a>\r\n        <a role=\"button\" class=\"btn btn-blue float-end me-2 mb-2 Details\"");
            BeginWriteAttribute("onclick", " onclick=\"", 1488, "\"", 1566, 7);
            WriteAttributeValue("", 1498, "ExportAsExcel(\'", 1498, 15, true);
#nullable restore
#line 63 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
WriteAttributeValue("", 1513, Model.FromDates, 1513, 16, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 1529, "\',\'", 1529, 3, true);
#nullable restore
#line 63 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
WriteAttributeValue("", 1532, Model.ToDates, 1532, 14, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 1546, "\',", 1546, 2, true);
#nullable restore
#line 63 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
WriteAttributeValue("", 1548, Model.CompanyId, 1548, 16, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 1564, ");", 1564, 2, true);
            EndWriteAttribute();
            WriteLiteral(">Export Excel</a>\r\n        <a role=\"button\" class=\"btn btn-blue float-end me-2 mb-2 Details\"");
            BeginWriteAttribute("onclick", " onclick=\"", 1659, "\"", 1735, 7);
            WriteAttributeValue("", 1669, "ExportAsPdf(\'", 1669, 13, true);
#nullable restore
#line 64 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
WriteAttributeValue("", 1682, Model.FromDates, 1682, 16, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 1698, "\',\'", 1698, 3, true);
#nullable restore
#line 64 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
WriteAttributeValue("", 1701, Model.ToDates, 1701, 14, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 1715, "\',", 1715, 2, true);
#nullable restore
#line 64 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
WriteAttributeValue("", 1717, Model.CompanyId, 1717, 16, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 1733, ");", 1733, 2, true);
            EndWriteAttribute();
            WriteLiteral(@">Export PDF</a>
    </div>
</div>

<div class=""row Details"">
    <div class=""col-12"">
        <div class=""card"">
            <div class=""card-header"">
                <h4 class=""text-center front-text-blue fw-bold"">Balance Sheet Custom Report From ");
#nullable restore
#line 72 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
                                                                                            Write(Model.FromDate);

#line default
#line hidden
#nullable disable
            WriteLiteral(" till ");
#nullable restore
#line 72 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
                                                                                                                 Write(Model.ToDate);

#line default
#line hidden
#nullable disable
            WriteLiteral(" of ");
#nullable restore
#line 72 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
                                                                                                                                  Write(Model.CompanyName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h4>\r\n                <input type=\"hidden\" name=\"fromDate\" id=\"fromDate\"");
            BeginWriteAttribute("value", " value=\"", 2121, "\"", 2145, 1);
#nullable restore
#line 73 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
WriteAttributeValue("", 2129, Model.FromDates, 2129, 16, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" />\r\n                <input type=\"hidden\" name=\"toDate\" id=\"toDate\"");
            BeginWriteAttribute("value", " value=\"", 2213, "\"", 2235, 1);
#nullable restore
#line 74 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
WriteAttributeValue("", 2221, Model.ToDates, 2221, 14, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" />\r\n                <input type=\"hidden\" name=\"companyId\" id=\"companyId\"");
            BeginWriteAttribute("value", " value=\"", 2309, "\"", 2333, 1);
#nullable restore
#line 75 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
WriteAttributeValue("", 2317, Model.CompanyId, 2317, 16, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" />
               
                <div class=""row col-md-12 mt-4 mb-3"">
                    <div class=""float-left text-center"">
                        <span class=""text-center float-left front-text-blue h6 fw-bold"">Total Credit :</span><span class=""text-center float-left text-success h6 fw-bold""> ");
#nullable restore
#line 79 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
                                                                                                                                                                      Write(Model.TotalCredit);

#line default
#line hidden
#nullable disable
            WriteLiteral(@" AED</span>
                        <span class=""text-center float-left front-text-blue h6 fw-bold"">||</span>
                        <span class=""text-center float-left front-text-blue h6 fw-bold"">Total Debit : </span><span class=""text-center float-left text-danger h6 fw-bold"">-");
#nullable restore
#line 81 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
                                                                                                                                                                     Write(Model.TotalDebit);

#line default
#line hidden
#nullable disable
            WriteLiteral(" AED</span>\r\n                    </div>\r\n                </div>\r\n\r\n                <div class=\"row col-md-12 mt-4 mb-3\">\r\n                    <div class=\"float-left text-center\">\r\n");
#nullable restore
#line 87 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
                         if (!Model.TotalBalance.StartsWith("-"))
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <span class=\"text-center float-left front-text-blue h6 fw-bold\">Total Balance :</span>\r\n");
            WriteLiteral("                            <span class=\"text-center float-left text-success h6 fw-bold\"> ");
#nullable restore
#line 91 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
                                                                                     Write(Model.TotalBalance);

#line default
#line hidden
#nullable disable
            WriteLiteral(" AED </span>\r\n");
#nullable restore
#line 92 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
                        }
                        else
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <span class=\"text-center float-left front-text-blue h6 fw-bold\">Total Balance :</span>\r\n");
            WriteLiteral("                            <span class=\"text-center float-left text-danger h6 fw-bold\"> ");
#nullable restore
#line 97 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
                                                                                    Write(Model.TotalBalance);

#line default
#line hidden
#nullable disable
            WriteLiteral(" AED </span>\r\n");
#nullable restore
#line 98 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "b3ba08ee4b1aab87fcf27465eebe609dab4ca0979d706a10587993e17e8540b719194", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "b3ba08ee4b1aab87fcf27465eebe609dab4ca0979d706a10587993e17e8540b720262", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "b3ba08ee4b1aab87fcf27465eebe609dab4ca0979d706a10587993e17e8540b721326", async() => {
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
#nullable restore
#line 178 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
              Write(Url.Action("BalanceSheetCompanyCustomPaginationReport","ReportManagement"));

#line default
#line hidden
#nullable disable
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
#nullable restore
#line 251 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
             Write(Url.Action("BalanceSheetCompanyCustomReportExcel", "ReportManagement"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
            type: 'GET',
            data: { 'fromDate': fromDate, 'toDate': toDate, 'companyId': companyId },
            success: function (result) {
                $mymodal = $(""#multipleDownloadErrorModal"");
                $mymodal.find(""div.modal-body"");
                $mymodal.modal(""hide"");

                if (result == ""Success"") {
                    var url = """);
#nullable restore
#line 260 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
                          Write(Url.Action("DownloadBalanceSheetCompanyCustomReportExcel","ReportManagement"));

#line default
#line hidden
#nullable disable
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
#nullable restore
#line 284 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
             Write(Url.Action("BalanceSheetCompanyCustomReportPdf", "ReportManagement"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
            type: 'GET',
            data: { 'fromDate': fromDate, 'toDate': toDate, 'companyId': companyId },
            success: function (result) {
                $mymodal = $(""#multipleDownloadErrorModal"");
                $mymodal.find(""div.modal-body"");
                $mymodal.modal(""hide"");

                if (result == ""Success"") {
                    var url = """);
#nullable restore
#line 293 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
                          Write(Url.Action("DownloadBalanceSheetCompanyCustomReportPdf","ReportManagement"));

#line default
#line hidden
#nullable disable
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
#nullable restore
#line 325 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
             Write(Url.Action("GetRolePermissionsName", "Permission"));

#line default
#line hidden
#nullable disable
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
#nullable restore
#line 334 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\BalanceSheetCompanyCustomReport.cshtml"
                     Write(Url.Action("GetUserPermissionsName", "Permission"));

#line default
#line hidden
#nullable disable
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
            WriteLiteral("on for this module.\");\r\n                            $(\"#loader\").hide();\r\n                        }\r\n                    }\r\n                })\r\n            }\r\n        });\r\n        $(\"#loader\").hide();\r\n    });\r\n</script>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<DIBN.Areas.Admin.Models.BalanceSheetCustomReportByCompanyIdModel> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591