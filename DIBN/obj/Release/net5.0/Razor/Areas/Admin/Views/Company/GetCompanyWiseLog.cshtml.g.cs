#pragma checksum "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "de8be8741f88f23ae2cfefecb6cac0a5d7b64d774bc63c93524dceb9ca4c9b5f"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Company_GetCompanyWiseLog), @"mvc.1.0.view", @"/Areas/Admin/Views/Company/GetCompanyWiseLog.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"de8be8741f88f23ae2cfefecb6cac0a5d7b64d774bc63c93524dceb9ca4c9b5f", @"/Areas/Admin/Views/Company/GetCompanyWiseLog.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"605a41b4408e5d1ff247f7264e35f4305eb9eeecf14a77cb4f30041778bbf31f", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Areas_Admin_Views_Company_GetCompanyWiseLog : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<DIBN.Areas.Admin.Models.GetCompanyLogDetails>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/jquery/dist/jquery.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/datatables.net/js/jquery.dataTables.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/datatables.net-bs4/js/dataTables.bootstrap4.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/datatables.net-responsive/js/dataTables.responsive.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/datatables.net-responsive-bs4/js/responsive.bootstrap4.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
#line 2 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
  
    ViewData["Title"] = "Get Company Wise Log";

#line default
#line hidden
#nullable disable

            WriteLiteral(@"<div class=""row"">
    <div class=""col-md-12"">
        <h4 class=""card-title float-start"">Company Activity Log(s)</h4>
        <a href=""javascript:history.go(-1);"" class=""btn btn-theme float-end"" style=""margin-bottom:15px;"">Back to List</a>
    </div>
</div>
<div class=""row"" id=""View"">
    <div class=""col-12"">
        <div class=""card"">
            <div class=""card-body"">
                <div class=""col-md-12"">
                    <div class=""table-rep-plugin"">
                        <div class=""table-responsive mb-0"" data-pattern=""priority-columns"">
                            <table id=""datatable"" class=""table table-bordered table-striped dt-responsive nowrap"" style=""border-collapse: collapse; border-spacing: 0; width: 100%;"">
                                <thead>
                                    <tr>
                                        <th>
                                            #
                                        </th>
                                        <th>
  ");
            WriteLiteral(@"                                          Message
                                        </th>
                                        <th>
                                            Created By
                                        </th>
                                        <th>
                                            Created Date
                                        </th>
                                        <th>
                                            Amendment By
                                        </th>
                                        <th>
                                            Amendment Date
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
");
#nullable restore
#line 42 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                      
                                        var i = 0;
                                    

#line default
#line hidden
#nullable disable

#nullable restore
#line 45 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                     foreach (var item in Model.logs)
                                    {

#line default
#line hidden
#nullable disable

            WriteLiteral("                                        <tr>\r\n                                            <td>");
            Write(
#nullable restore
#line 48 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                  i + 1

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("</td>\r\n                                            <td class=\"text-wrap\">\r\n                                                ");
            Write(
#nullable restore
#line 50 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                 Html.Raw(@item.LogMessage)

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\r\n                                            </td>\r\n                                            <td class=\"text-wrap\">\r\n");
#nullable restore
#line 53 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                 if (item.CreatedBy != "N/A" && item.CreatedBy != null)
                                                {
                                                    

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 55 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                     item.CreatedBy

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 55 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                                   
                                                }
                                                else
                                                {
                                                    

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 59 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                     Html.Raw("---")

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 59 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                                    
                                                }

#line default
#line hidden
#nullable disable

            WriteLiteral("                                            </td>\r\n                                            <td class=\"text-wrap\">\r\n");
#nullable restore
#line 63 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                 if (item.CreatedOnDate != "N/A" && item.CreatedOnDate != null)
                                                {
                                                    

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 65 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                     item.CreatedOnDate

#line default
#line hidden
#nullable disable
            );
            Write(
#nullable restore
#line 65 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                                         Html.Raw(" ")

#line default
#line hidden
#nullable disable
            );
            Write(
#nullable restore
#line 65 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                                                        item.CreatedOnTime

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 65 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                                                                          
                                                }
                                                else
                                                {
                                                    

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 69 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                     Html.Raw("---")

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 69 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                                    
                                                }

#line default
#line hidden
#nullable disable

            WriteLiteral("                                            </td>\r\n                                            <td class=\"text-wrap\">\r\n");
#nullable restore
#line 73 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                 if (item.ModifyBy != "N/A" && item.ModifyBy != null)
                                                {
                                                    

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 75 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                     item.ModifyBy

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 75 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                                  
                                                }
                                                else
                                                {
                                                    

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 79 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                     Html.Raw("---")

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 79 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                                    
                                                }

#line default
#line hidden
#nullable disable

            WriteLiteral("                                            </td>\r\n                                            <td class=\"text-wrap\">\r\n");
#nullable restore
#line 83 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                 if (item.ModifyOnDate != "N/A" && item.ModifyOnDate != null)
                                                {
                                                    

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 85 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                     item.ModifyOnDate

#line default
#line hidden
#nullable disable
            );
            Write(
#nullable restore
#line 85 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                                        Html.Raw(" ")

#line default
#line hidden
#nullable disable
            );
            Write(
#nullable restore
#line 85 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                                                       item.ModifyOnTime

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 85 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                                                                        
                                                }
                                                else
                                                {
                                                    

#line default
#line hidden
#nullable disable

            Write(
#nullable restore
#line 89 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                     Html.Raw("---")

#line default
#line hidden
#nullable disable
            );
#nullable restore
#line 89 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                                                    
                                                }

#line default
#line hidden
#nullable disable

            WriteLiteral("                                            </td>\r\n                                        </tr>\r\n");
#nullable restore
#line 93 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\Company\GetCompanyWiseLog.cshtml"
                                        i++;
                                    }

#line default
#line hidden
#nullable disable

            WriteLiteral("                                </tbody>\r\n                            </table>\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "de8be8741f88f23ae2cfefecb6cac0a5d7b64d774bc63c93524dceb9ca4c9b5f18805", async() => {
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
            WriteLiteral("\r\n<!-- Required datatable js -->\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "de8be8741f88f23ae2cfefecb6cac0a5d7b64d774bc63c93524dceb9ca4c9b5f19903", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "de8be8741f88f23ae2cfefecb6cac0a5d7b64d774bc63c93524dceb9ca4c9b5f20967", async() => {
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
            WriteLiteral("\r\n\r\n<!-- Responsive examples -->\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "de8be8741f88f23ae2cfefecb6cac0a5d7b64d774bc63c93524dceb9ca4c9b5f22067", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "de8be8741f88f23ae2cfefecb6cac0a5d7b64d774bc63c93524dceb9ca4c9b5f23131", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_4);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"

<script>
    $('#datatable').dataTable({
        paging: true,
        ordering: true,
        searching: true,
        ""aLengthMenu"": [20, 30, 50],
        dom: ""<'row'<'col-sm-3'l><'col-sm-3'f><'col-sm-6'p>>"" +
            ""<'row'<'col-sm-12'tr>>"" +
            ""<'row'<'col-sm-5'i><'col-sm-7'p>>"",
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<DIBN.Areas.Admin.Models.GetCompanyLogDetails> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591