#pragma checksum "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\CompanyProfitLoss.cshtml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "a8b4b1731104c3dc725bc93a6cdadbea7bfd03c3be1efff767e8dc22ed4c8e2f"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_ReportManagement_CompanyProfitLoss), @"mvc.1.0.view", @"/Areas/Admin/Views/ReportManagement/CompanyProfitLoss.cshtml")]
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
#line 1 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\_ViewImports.cshtml"
using DIBN

#nullable disable
    ;
#nullable restore
#line 2 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\_ViewImports.cshtml"
using DIBN.Areas.Admin.Models

#nullable disable
    ;
#nullable restore
#line 3 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\_ViewImports.cshtml"
using DIBN.Areas.Admin.Data

#nullable disable
    ;
#nullable restore
#line 4 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\_ViewImports.cshtml"
using DIBN.Areas.Admin.IRepository

#nullable disable
    ;
#nullable restore
#line 5 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\_ViewImports.cshtml"
using DIBN.Areas.Admin.Repository

#line default
#line hidden
#nullable disable
    ;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"a8b4b1731104c3dc725bc93a6cdadbea7bfd03c3be1efff767e8dc22ed4c8e2f", @"/Areas/Admin/Views/ReportManagement/CompanyProfitLoss.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"605a41b4408e5d1ff247f7264e35f4305eb9eeecf14a77cb4f30041778bbf31f", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Areas_Admin_Views_ReportManagement_CompanyProfitLoss : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/dibn_business_image/growth.png"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("img-fluid text-center front-text-theme mt-2 p-3"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/dibn_business_image/growth-1.png"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("img-fluid text-center mt-2 front-text-theme p-3"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
#line 1 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\CompanyProfitLoss.cshtml"
  
    ViewData["Title"] = "Company Report";

#line default
#line hidden
#nullable disable

            WriteLiteral(@"<div id=""loader"">
    <div class=""spinner-1"">
        <div class=""center-div-1"">
            <div class=""loader-circle-75"">
            </div>
        </div>
    </div>
</div>
<div class=""row"" id=""View"">
    <div class=""col-12"">
        <div class=""card"">
            <div class=""card-body"">
                <h3 class=""text-center front-text-theme fw-bold"">Company Report</h3>
                <hr class=""text-theme"" style=""height:1px;"" />

                <div class=""col-md-12 mt-4 mb-3"">
                    <div class=""row text-center justify-content-center"">
                        <div class=""col-md-2 col-sm-4 p-1 mx-3 mt-1 float-left border rounded-3 bg-white border-theme"" style=""cursor:pointer;"" onclick=""SelectRangeType('Cleared Company');"" onmouseover=""addeffect1();"" onmouseout=""removeeffect1();"" id=""effect1"">
                            <p class=""front-text-theme text-center h5 text-truncate text-wrap mt-2"">Cleared Company(s) Excel</p>
                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "a8b4b1731104c3dc725bc93a6cdadbea7bfd03c3be1efff767e8dc22ed4c8e2f6652", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
                        </div>
                        <div class=""col-md-2 col-sm-4 p-1 mx-3 mt-1 float-left border rounded-3 bg-white border-theme"" style=""cursor:pointer;"" onclick=""SelectRangeType('Overdue Company');"" onmouseover=""addeffect2();"" onmouseout=""removeeffect2();"" id=""effect2"">
                            <p class=""front-text-theme text-center h5 text-truncate text-wrap mt-2"">Overdue Company(s) Excel</p>
                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "a8b4b1731104c3dc725bc93a6cdadbea7bfd03c3be1efff767e8dc22ed4c8e2f8254", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<script>
    function SelectRangeType(selectedType){
        
        if (selectedType == ""Cleared Company"") {
            var url = """);
            Write(
#nullable restore
#line 40 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\CompanyProfitLoss.cshtml"
                        Url.Action("ClearedCompanyListForExcel","ReportManagement")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\";\r\n            window.location.href = url;\r\n        }\r\n        else{\r\n            var url = \"");
            Write(
#nullable restore
#line 44 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\CompanyProfitLoss.cshtml"
                        Url.Action("OverdueCompanyListForExcel","ReportManagement")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(@""";
            window.location.href = url;
        }
    }
</script>

<script>
    $(document).ready(function () {
        $(""#loader"").hide();
    });

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
#line 68 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\CompanyProfitLoss.cshtml"
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
#line 77 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\ReportManagement\CompanyProfitLoss.cshtml"
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
            WriteLiteral("on for this module.\");\r\n                            $(\"#loader\").hide();\r\n                        }\r\n                    }\r\n                })\r\n            }\r\n        });\r\n        $(\"#loader\").hide();\r\n    });\r\n</script>\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591