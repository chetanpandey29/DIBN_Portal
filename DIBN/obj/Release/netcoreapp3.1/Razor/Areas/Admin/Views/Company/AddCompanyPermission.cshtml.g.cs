#pragma checksum "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "d8735c92b7a3c4baba9415087cf2097acfa06f6844a7eb3a7fe7fde2ead74137"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_Company_AddCompanyPermission), @"mvc.1.0.view", @"/Areas/Admin/Views/Company/AddCompanyPermission.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA256", @"d8735c92b7a3c4baba9415087cf2097acfa06f6844a7eb3a7fe7fde2ead74137", @"/Areas/Admin/Views/Company/AddCompanyPermission.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA256", @"605a41b4408e5d1ff247f7264e35f4305eb9eeecf14a77cb4f30041778bbf31f", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Areas_Admin_Views_Company_AddCompanyPermission : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<DIBN.Areas.Admin.Models.CompanyViewModel.CompanyPermissionList>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("type", "hidden", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("Module"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("CompanyId"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/jquery/dist/jquery.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/sweetalert2/sweetalert2.all.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/sweetalert2/sweetalert2.min.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
  
    ViewData["Title"] = "AddUserPermission";

#line default
#line hidden
#nullable disable
            WriteLiteral("<div class=\"col-md-12\">\r\n    <div class=\"col-md-2 float-end\">\r\n        <a href=\"javascript:history.go(-1);\" class=\"btn btn-blue float-end\" style=\"margin-bottom:15px;\">Back to List</a>\r\n    </div>\r\n</div>\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "d8735c92b7a3c4baba9415087cf2097acfa06f6844a7eb3a7fe7fde2ead741376680", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
#nullable restore
#line 11 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => Model.Module);

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.InputTypeName = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 11 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                                                    WriteLiteral(Model.Module);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.Value = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n\r\n<div class=\"row\" id=\"invalidPermission\">\r\n    <div class=\"col-12\">\r\n        <div class=\"card\">\r\n            <div class=\"card-body\">\r\n                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "d8735c92b7a3c4baba9415087cf2097acfa06f6844a7eb3a7fe7fde2ead741379285", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.InputTypeName = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
#nullable restore
#line 17 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => Model.CompanyId);

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                <div class=\"col-md-4 float-start\">\r\n                    <h4 class=\"card-title mb-2\">Modules</h4>\r\n\r\n");
#nullable restore
#line 21 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                     foreach (var item in Model.Modules)
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <p style=\"margin-top:15px;\">\r\n                            ");
#nullable restore
#line 24 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                       Write(item.ModuleKeyword);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </p>\r\n");
#nullable restore
#line 26 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </div>\r\n                <div class=\"col-md-8 float-end\">\r\n");
#nullable restore
#line 29 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                     foreach (var item in Model.Permissions)
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <span class=\"card-title ms-4 mb-2 text-center\">\r\n                            ");
#nullable restore
#line 32 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                       Write(item.PermissionName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </span>\r\n");
#nullable restore
#line 34 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"

                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <div>\r\n");
#nullable restore
#line 37 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                          
                            List<string> permission = new List<string>();
                            permission.Add("Insert");
                            permission.Add("Update");
                            permission.Add("View");
                            permission.Add("Delete");
                            List<int> permissionId = new List<int>();
                            permissionId.Add(1);
                            permissionId.Add(2);
                            permissionId.Add(3);
                            permissionId.Add(4);

                        

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 51 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                         foreach (var item in Model.Modules)
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <p style=\"margin-top:15px;\">\r\n");
#nullable restore
#line 54 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                                 for (var i = 0; i < Model.permissionCount; i++)
                                {
                                    var allowed = false;
                                    

#line default
#line hidden
#nullable disable
#nullable restore
#line 57 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                                     foreach (var checkPermission in Model.getCompanyPermissionByCompanyIds)
                                    {

                                        if (checkPermission.ModuleId == item.ModuleId && checkPermission.PermissionId == permissionId[i])
                                        {
                                            allowed = true;
                                        }
                                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                    <span");
            BeginWriteAttribute("class", " class=\"", 2814, "\"", 2859, 4);
            WriteAttributeValue("", 2822, "ms-5", 2822, 4, true);
            WriteAttributeValue(" ", 2826, "text-center", 2827, 12, true);
            WriteAttributeValue(" ", 2838, "allow_", 2839, 7, true);
#nullable restore
#line 65 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
WriteAttributeValue("", 2845, permission[i], 2845, 14, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n                                        <input class=\"form-check-input\" type=\"checkbox\"");
            BeginWriteAttribute("name", " name=\"", 2950, "\"", 2971, 1);
#nullable restore
#line 66 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
WriteAttributeValue("", 2957, item.ModuleId, 2957, 14, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            BeginWriteAttribute("id", " id=\"", 2972, "\"", 3013, 3);
            WriteAttributeValue("", 2977, "allow_", 2977, 6, true);
#nullable restore
#line 66 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
WriteAttributeValue("", 2983, item.ModuleId, 2983, 14, false);

#line default
#line hidden
#nullable disable
#nullable restore
#line 66 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
WriteAttributeValue("", 2997, permissionId[i], 2997, 16, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            BeginWriteAttribute("value", " value=\"", 3014, "\"", 3036, 1);
#nullable restore
#line 66 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
WriteAttributeValue("", 3022, item.ModuleId, 3022, 14, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" ");
#nullable restore
#line 66 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                                                                                                                                                                           Write(allowed?"checked=checked":null);

#line default
#line hidden
#nullable disable
            WriteLiteral(" onclick=\"removePermission(");
#nullable restore
#line 66 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                                                                                                                                                                                                                                      Write(item.ModuleId);

#line default
#line hidden
#nullable disable
            WriteLiteral(",\'");
#nullable restore
#line 66 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                                                                                                                                                                                                                                                      Write(permission[i]);

#line default
#line hidden
#nullable disable
            WriteLiteral("\',");
#nullable restore
#line 66 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                                                                                                                                                                                                                                                                      Write(permissionId[i]);

#line default
#line hidden
#nullable disable
            WriteLiteral(");\" />\r\n                                    </span>\r\n");
#nullable restore
#line 68 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                                }

#line default
#line hidden
#nullable disable
            WriteLiteral("                            </p>\r\n");
#nullable restore
#line 70 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                        }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                    </div>
                </div>
            </div>
            <div class=""mt-2 float-start"">
                <input type=""button"" class=""btn btn-blue ms-4 mb-2"" value=""Save"" name=""Save"" id=""savePermissions"" />
            </div>
        </div>
    </div>
</div>


");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "d8735c92b7a3c4baba9415087cf2097acfa06f6844a7eb3a7fe7fde2ead7413720025", async() => {
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
            WriteLiteral("\r\n<!-- Sweet Alerts-->\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "d8735c92b7a3c4baba9415087cf2097acfa06f6844a7eb3a7fe7fde2ead7413721113", async() => {
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
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "d8735c92b7a3c4baba9415087cf2097acfa06f6844a7eb3a7fe7fde2ead7413722177", async() => {
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
            WriteLiteral(@"

<script>
    $(""#savePermissions"").click(function(){
        event.preventDefault();
        const swalWithBootstrapButtons = Swal.mixin({
          customClass: {
            confirmButton: 'ms-3 btn btn-success',
            cancelButton: 'btn btn-danger'
          },
          buttonsStyling: false
        })
        
        var CompanyId = $(""#CompanyId"").val();
        var insertPermission = $('.allow_Insert input:checkbox:checked').map(function () {
                return $(this).val();
            }).get();
        var updatePermission = $('.allow_Update input:checkbox:checked').map(function () {
                return $(this).val();
            }).get();
        var viewPermission = $('.allow_View input:checkbox:checked').map(function () {
                return $(this).val();
            }).get();
        var deletePermission = $('.allow_Delete input:checkbox:checked').map(function () {
                return $(this).val();
            }).get();
            
        if((");
            WriteLiteral(@"insertPermission !=null || updatePermission != null || viewPermission !=null || deletePermission !=null) && CompanyId != null
            && (insertPermission.length>0 || updatePermission.length>0 || viewPermission.length >0 || deletePermission.length >0)){
            $.ajax({
                url: """);
#nullable restore
#line 115 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                 Write(Url.Action("SaveCompanyPermission","Company"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@""",
                method: ""post"",
                data:{""InsertPermission"":insertPermission,""UpdatePermission"":updatePermission,""ViewPermission"":viewPermission,'DeletePermission':deletePermission,""CompanyId"":CompanyId},
                success: function (response) {
                    if(response>0){
                        swalWithBootstrapButtons.fire({
                          title: 'Success.',
                          text: ""Permission Added Successfully..!!"",
                          icon: 'success'
                        }).then((result) => {
                            window.location.reload();
                        });
                    }else{
                        swalWithBootstrapButtons.fire({
                          title: 'Faild.',
                          text: ""Due to Some Problem Selected Permission(s) were not allocated to this Company..!!"",
                          icon: 'error'
                        }).then((result) => {
                            windo");
            WriteLiteral(@"w.location.reload();
                        });
                    }
                }
            });
        }

    });
</script>
<script>
    function removePermission(moduleId,permission,permissionId){
        
       var checked= $(""#allow_""+moduleId+permissionId);
       var CompanyId = $(""#CompanyId"").val();
       if(!$(checked).is(':checked')){
           $.ajax({
               url: """);
#nullable restore
#line 149 "D:\Devotion Business\DIBN\DIBN\Areas\Admin\Views\Company\AddCompanyPermission.cshtml"
                Write(Url.Action("RemoveCompanyPermission","Company"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@""",
               method:""get"",
               data:{""CompanyId"":CompanyId,""ModuleId"":moduleId,""PermissionId"":permissionId},
               success: function (response) {
                    if(response>0){
                        
                        window.location.reload();
                    }
                }
           });
       }
    }
</script>


");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<DIBN.Areas.Admin.Models.CompanyViewModel.CompanyPermissionList> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591