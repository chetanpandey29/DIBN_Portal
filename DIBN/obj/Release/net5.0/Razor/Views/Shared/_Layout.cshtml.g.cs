#pragma checksum "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "62e30f4a7f2d4a0fd96e92a8950ccec6ce52f687db0e54006a1c290fd8975821"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared__Layout), @"mvc.1.0.view", @"/Views/Shared/_Layout.cshtml")]
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
#line 1 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\_ViewImports.cshtml"
using DIBN

#nullable disable
    ;
#nullable restore
#line 2 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\_ViewImports.cshtml"
using DIBN.Models

#line default
#line hidden
#nullable disable
    ;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"62e30f4a7f2d4a0fd96e92a8950ccec6ce52f687db0e54006a1c290fd8975821", @"/Views/Shared/_Layout.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"7d5ddeb28e6b11bd8a350250841d25befde6ee969eb2ffe12afe4b0408fac805", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Shared__Layout : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/fontawesome-free-6.0.0-web/js/fontawesome.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/datatables.net/js/jquery.dataTables.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/datatables.net-bs4/js/dataTables.bootstrap4.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/autocomplete/jquery-ui.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/select2/select2.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/sweetalert2/sweetalert2.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_6 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/bootstrap-touchspin/jquery.bootstrap-touchspin.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("<!DOCTYPE html>\r\n<html lang=\"en\">\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "62e30f4a7f2d4a0fd96e92a8950ccec6ce52f687db0e54006a1c290fd89758216249", async() => {
                WriteLiteral("\r\n\r\n    ");
                Write(
#nullable restore
#line 6 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
     await Html.PartialAsync("/Areas/Admin/Views/Shared/Theme/_title_meta.cshtml")

#line default
#line hidden
#nullable disable
                );
                WriteLiteral("\r\n    ");
                Write(
#nullable restore
#line 7 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
     await Html.PartialAsync("/Areas/Admin/Views/Shared/Theme/_head_css.cshtml")

#line default
#line hidden
#nullable disable
                );
                WriteLiteral("\r\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "62e30f4a7f2d4a0fd96e92a8950ccec6ce52f687db0e54006a1c290fd89758217881", async() => {
                WriteLiteral("\r\n    <!-- Begin page -->\r\n    <div id=\"layout-wrapper\">\r\n        ");
                Write(
#nullable restore
#line 13 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
         await Html.PartialAsync("~/Views/Shared/Theme/_topbar.cshtml")

#line default
#line hidden
#nullable disable
                );
                WriteLiteral("\r\n        ");
                Write(
#nullable restore
#line 14 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
         await Html.PartialAsync("~/Views/Shared/Theme/_sidebar.cshtml")

#line default
#line hidden
#nullable disable
                );
                WriteLiteral(@"

        <!-- ============================================================== -->
        <!-- Start right Content here -->
        <!-- ============================================================== -->
        <div id=""main-loader"">
            <div class=""spinner-1"">
                <div class=""center-div-1"">
                    <div class=""loader-circle-75"">
                    </div>
                </div>
            </div>
        </div>

        <div class=""main-content"">
            <div class=""page-content"">
                <div class=""container-fluid"">
                    <div class=""row"">
                        <div class=""col-12 mb-2"">
                            <h3 class=""text-theme"" style=""font-size:18px"">
                                <strong>
                                    <span id=""welcomeText"">Welcome to </span><span id=""CompanyName""></span><span>.</span>
                                    <span class=""float-end text-theme"">
                                 ");
                WriteLiteral("       <span id=\"CompanyType\"></span>\r\n                                    </span>\r\n                                </strong>\r\n                            </h3>\r\n                        </div>\r\n                    </div>\r\n                    ");
                Write(
#nullable restore
#line 43 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
                     RenderBody()

#line default
#line hidden
#nullable disable
                );
                WriteLiteral("\r\n                </div> <!-- container-fluid -->\r\n            </div>\r\n            <!-- End Page-content -->\r\n            ");
                Write(
#nullable restore
#line 47 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
             await Html.PartialAsync("~/Views/Shared/Theme/_footer.cshtml")

#line default
#line hidden
#nullable disable
                );
                WriteLiteral("\r\n        </div>\r\n        <!-- end main content-->\r\n\r\n    </div>\r\n    <!-- END layout-wrapper -->\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "62e30f4a7f2d4a0fd96e92a8950ccec6ce52f687db0e54006a1c290fd897582111001", async() => {
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
                WriteLiteral("\r\n\r\n    <script");
                BeginWriteAttribute("src", " src=\"", 2096, "\"", 2149, 1);
                WriteAttributeValue("", 2102, 
#nullable restore
#line 55 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
                  Url.Content("~/lib/jquery/dist/jquery.min.js")

#line default
#line hidden
#nullable disable
                , 2102, 47, false);
                EndWriteAttribute();
                WriteLiteral("></script>\r\n    <script");
                BeginWriteAttribute("src", " src=\"", 2173, "\"", 2245, 1);
                WriteAttributeValue("", 2179, 
#nullable restore
#line 56 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
                  Url.Content("~/assets/libs/bootstrap/js/bootstrap.bundle.min.js")

#line default
#line hidden
#nullable disable
                , 2179, 66, false);
                EndWriteAttribute();
                WriteLiteral("></script>\r\n    <script");
                BeginWriteAttribute("src", " src=\"", 2269, "\"", 2331, 1);
                WriteAttributeValue("", 2275, 
#nullable restore
#line 57 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
                  Url.Content("~/assets/libs/metismenu/metisMenu.min.js")

#line default
#line hidden
#nullable disable
                , 2275, 56, false);
                EndWriteAttribute();
                WriteLiteral("></script>\r\n    <script");
                BeginWriteAttribute("src", " src=\"", 2355, "\"", 2417, 1);
                WriteAttributeValue("", 2361, 
#nullable restore
#line 58 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
                  Url.Content("~/assets/libs/simplebar/simplebar.min.js")

#line default
#line hidden
#nullable disable
                , 2361, 56, false);
                EndWriteAttribute();
                WriteLiteral("></script>\r\n    <script");
                BeginWriteAttribute("src", " src=\"", 2441, "\"", 2528, 1);
                WriteAttributeValue("", 2447, 
#nullable restore
#line 59 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
                  Url.Content("~/assets/libs/bootstrap-datepicker/js/bootstrap-datepicker.min.js")

#line default
#line hidden
#nullable disable
                , 2447, 81, false);
                EndWriteAttribute();
                WriteLiteral("></script>\r\n\r\n\r\n    <!-- Required datatable js -->\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "62e30f4a7f2d4a0fd96e92a8950ccec6ce52f687db0e54006a1c290fd897582114616", async() => {
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
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "62e30f4a7f2d4a0fd96e92a8950ccec6ce52f687db0e54006a1c290fd897582115740", async() => {
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
                WriteLiteral("\r\n\r\n    <!-- Responsive examples -->\r\n    <script");
                BeginWriteAttribute("src", " src=\"", 2808, "\"", 2901, 1);
                WriteAttributeValue("", 2814, 
#nullable restore
#line 67 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
                  Url.Content("~/assets/libs/datatables.net-responsive/js/dataTables.responsive.min.js")

#line default
#line hidden
#nullable disable
                , 2814, 87, false);
                EndWriteAttribute();
                WriteLiteral("></script>\r\n    <script");
                BeginWriteAttribute("src", " src=\"", 2925, "\"", 3022, 1);
                WriteAttributeValue("", 2931, 
#nullable restore
#line 68 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
                  Url.Content("~/assets/libs/datatables.net-responsive-bs4/js/responsive.bootstrap4.min.js")

#line default
#line hidden
#nullable disable
                , 2931, 91, false);
                EndWriteAttribute();
                WriteLiteral("></script>\r\n\r\n    <!-- App js -->\r\n    <script");
                BeginWriteAttribute("src", " src=\"", 3069, "\"", 3109, 1);
                WriteAttributeValue("", 3075, 
#nullable restore
#line 71 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
                  Url.Content("~/assets/js/app.js")

#line default
#line hidden
#nullable disable
                , 3075, 34, false);
                EndWriteAttribute();
                WriteLiteral("></script>\r\n\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "62e30f4a7f2d4a0fd96e92a8950ccec6ce52f687db0e54006a1c290fd897582118428", async() => {
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
                WriteLiteral("\r\n\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "62e30f4a7f2d4a0fd96e92a8950ccec6ce52f687db0e54006a1c290fd897582119556", async() => {
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
                WriteLiteral("\r\n\r\n    <!-- Sweet Alerts-->\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "62e30f4a7f2d4a0fd96e92a8950ccec6ce52f687db0e54006a1c290fd897582120712", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_5);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n    <!-- Bootrstrap touchspin -->\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "62e30f4a7f2d4a0fd96e92a8950ccec6ce52f687db0e54006a1c290fd897582121873", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_6);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"
    <script>
        document.onkeypress = function (event) {
            event = (event || window.event);
            if (event.keyCode == 123) {
                return false;
            }
        }
        document.onmousedown = function (event) {
            event = (event || window.event);
            if (event.keyCode == 123) {
                return false;
            }
        }
        document.onkeydown = function (event) {
            event = (event || window.event);
            if (event.keyCode == 123) {
                return false;
            }
        }
    </script>
    <script>
        document.addEventListener('contextmenu', event => event.preventDefault());  // To disable right click

        $(document).ready(function () {
            $(""#main-loader"").hide();
            $(document.querySelectorAll(""a"")).click(function () {
                if(!document.querySelectorAll(""a"")).contains(""href='#'"")){

                    if (!$(this).hasClass(""has-arrow"")) {
");
                WriteLiteral(@"                        $('.spinner').css('display', 'block');
                    }
                }
            });
        });

        $(window).on('load', function () {
            $('.spinner').css('display', 'block');
        });
    </script>
    <script>
        $(document).ajaxComplete(function (event, xhr, options) {
            
            $('.spinner').css('display', 'none');

        }).ajaxError(function (event, jqxhr, settings, exception) {

             $('.spinner').css('display', 'none');

        });
    </script>
    <script>
        $(function(){
            $.ajax({
                url: """);
                Write(
#nullable restore
#line 134 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
                       Url.Action("GetLoggedInRole", "Home")

#line default
#line hidden
#nullable disable
                );
                WriteLiteral(@""",
                method: ""GET"",
                async: false,
                success:function(response){
                    
                    if(response == ""Sales Person"" || response == ""RM Team""){
                        if (response == ""Sales Person"") {
                            $.ajax({
                                url: """);
                Write(
#nullable restore
#line 142 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
                                       Url.Action("GetUserNameSalesPerson", "Home")

#line default
#line hidden
#nullable disable
                );
                WriteLiteral(@""",
                                method: ""GET"",
                                async: false,
                                success: function (response1) {
                                    $(""#welcomeText"").text("""");
                                    $(""#welcomeText"").text(""Welcome "");
                                    $(""#CompanyName"").text(response1);
                                    $(""#CompanyType"").text(response);
                                }
                            });
                        }
                        else {
                            $.ajax({
                                url: """);
                Write(
#nullable restore
#line 155 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
                                       Url.Action("GetRMTeamName", "Home")

#line default
#line hidden
#nullable disable
                );
                WriteLiteral(@""",
                                method: ""GET"",
                                async: false,
                                success: function (response1) {
                                    $(""#welcomeText"").text("""");
                                    $(""#welcomeText"").text(""Welcome "");
                                    $(""#CompanyName"").text(response1);
                                    $(""#CompanyType"").text(response);
                                }
                            });
                        }
                    }
                    else{
                        
                        $.ajax({
                            url: """);
                Write(
#nullable restore
#line 170 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
                                   Url.Action("GetCompanyName", "Home")

#line default
#line hidden
#nullable disable
                );
                WriteLiteral(@""",
                            method: ""GET"",
                            async: false,
                            success:function(response){
                                $(""#CompanyName"").text(response.companyName);
                                
                                if(response.companyType!=""N/A""){
                                    if (response.companyType == ""Dubai Mainland"") {
                                        $(""#CompanyType"").text(""Mainland"");
                                    }
                                    else {
                                        $(""#CompanyType"").text(response.companyType);
                                    }
                                    
                                }
                                else{
                                     $.ajax({
                                        url: """);
                Write(
#nullable restore
#line 187 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Views\Shared\_Layout.cshtml"
                                               Url.Action("GetUserName", "Home")

#line default
#line hidden
#nullable disable
                );
                WriteLiteral(@""",
                                        method: ""GET"",
                                        async: false,
                                        success:function(response1){
                                            $(""#CompanyType"").text(response1);
                                        }
                                     });
                                }
                            }
                        });
                    }
                    
                }
            });
        });
    </script>
");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</html>");
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
