#pragma checksum "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\Index.cshtml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "01377f96a5c4335b43f1cc28fce0d1659d43506bc492867f9690f398bb4955cb"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_Admin_Views_SalesPerson_Index), @"mvc.1.0.view", @"/Areas/Admin/Views/SalesPerson/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"01377f96a5c4335b43f1cc28fce0d1659d43506bc492867f9690f398bb4955cb", @"/Areas/Admin/Views/SalesPerson/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"605a41b4408e5d1ff247f7264e35f4305eb9eeecf14a77cb4f30041778bbf31f", @"/Areas/Admin/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Areas_Admin_Views_SalesPerson_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<DIBN.Areas.Admin.Models.MainSalesPersonViewModel>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/jquery/dist/jquery.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/datatables.net/js/jquery.dataTables.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/datatables.net-bs4/js/dataTables.bootstrap4.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/datatables.net-responsive/js/dataTables.responsive.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/datatables.net-responsive-bs4/js/responsive.bootstrap4.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/sweetalert2/sweetalert2.all.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_6 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/libs/sweetalert2/sweetalert2.min.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
#line 2 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\Index.cshtml"
  
    ViewData["Title"] = "Index";

#line default
#line hidden
#nullable disable

            WriteLiteral("<div id=\"loader\">\r\n    <div class=\"spinner-1\">\r\n        <div class=\"center-div-1\">\r\n            <div class=\"loader-circle-75\">\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n");
#nullable restore
#line 13 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\Index.cshtml"
 if (Model.allowedModule.Contains("Insert"))
{

#line default
#line hidden
#nullable disable

            WriteLiteral("    <div class=\"row\">\r\n        <div class=\"col-lg-12\">\r\n            <button class=\"btn btn-theme mb-3 float-end\" onclick=\"AddNew();\" id=\"Insert\">Create</button>\r\n        </div>\r\n    </div>\r\n");
#nullable restore
#line 20 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\Index.cshtml"
}

#line default
#line hidden
#nullable disable

#nullable restore
#line 21 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\Index.cshtml"
 if (Model.allowedModule.Contains("View"))
{

#line default
#line hidden
#nullable disable

            WriteLiteral(@"    <div id=""View"">
        <div class=""row"" id=""View"">
            <div class=""col-12"">
                <div class=""card"">
                    <div class=""card-body"">
                        <h4 class=""card-title text-center"">Sales Person(s)</h4>
                        <hr style=""height:1px;"" class=""text-theme"" />
                        <div class=""table-rep-plugin"">
                            <div class=""table-responsive mb-0"" data-pattern=""priority-columns"">
                                <table id=""datatable"" class=""table table-bordered table-striped dt-responsive nowrap"" style=""border-collapse: collapse; border-spacing: 0; width: 100%;"">
                                    <thead>
                                        <tr>
                                            <th>
                                                Name
                                            </th>
                                            <th>
                                                Assigned Compan");
            WriteLiteral(@"y
                                            </th>
                                            <th>
                                                Designation
                                            </th>
                                            <th>
                                                Country Of Residence
                                            </th>
                                            <th>
                                                Passport No.
                                            </th>
                                            <th>
                                                Passport Expiry Date.
                                            </th>
                                            <th>
                                                Visa Expiry Date
                                            </th>
                                            <th>
                                                Insurance Company
                  ");
            WriteLiteral(@"                          </th>
                                            <th>
                                                Insurance Expiry Date
                                            </th>
                                            <th>

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
    </div>
");
#nullable restore
#line 78 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\Index.cshtml"
}

#line default
#line hidden
#nullable disable

            WriteLiteral("\r\n<input type=\"hidden\" name=\"Module\"");
            BeginWriteAttribute("value", " value=\"", 3362, "\"", 3383, 1);
            WriteAttributeValue("", 3370, 
#nullable restore
#line 80 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\Index.cshtml"
                                           Model.Module

#line default
#line hidden
#nullable disable
            , 3370, 13, false);
            EndWriteAttribute();
            WriteLiteral(" id=\"Module\" />\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "01377f96a5c4335b43f1cc28fce0d1659d43506bc492867f9690f398bb4955cb11458", async() => {
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
            WriteLiteral("\r\n\r\n<!-- Required datatable js -->\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "01377f96a5c4335b43f1cc28fce0d1659d43506bc492867f9690f398bb4955cb12560", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "01377f96a5c4335b43f1cc28fce0d1659d43506bc492867f9690f398bb4955cb13624", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "01377f96a5c4335b43f1cc28fce0d1659d43506bc492867f9690f398bb4955cb14724", async() => {
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "01377f96a5c4335b43f1cc28fce0d1659d43506bc492867f9690f398bb4955cb15788", async() => {
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
            WriteLiteral("\r\n\r\n<!-- Sweet Alerts-->\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "01377f96a5c4335b43f1cc28fce0d1659d43506bc492867f9690f398bb4955cb16880", async() => {
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
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "01377f96a5c4335b43f1cc28fce0d1659d43506bc492867f9690f398bb4955cb17944", async() => {
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
            WriteLiteral("\r\n\r\n<script>\r\n    $(document).ready(function(){\r\n        var flag = false;\r\n        var url = \"");
            Write(
#nullable restore
#line 98 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\Index.cshtml"
                    Url.Action("GetAllSalesPersonListWithPagination","SalesPerson")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(@""";
        url = url + ""?page="";
        var table = $('#datatable').DataTable({
            ""processing"": true,
            ""serverSide"": true,
            ""filter"": true,
            ""ajax"": {
                ""type"": ""post"",
                ""datatype"": ""json"",
                ""data"": function () {
                    var info = $('#datatable').DataTable().page.info();
                    $('#datatable').DataTable().ajax.url(
                        url + (info.page + 1)
                    );
                }
            },
            ""columns"": [
                {
                    ""data"": function (data, full, meta) {
                        return data.firstName + ' ' + data.lastName;
                    },
                    ""name"": ""Name""
                },
                { ""data"": function (data, full, meta) {
                        if (data.company.length > 30)
                        {
                            var AssignedCompany = data.company.substring(0, 30);
");
            WriteLiteral(@"                            return '<span title=""'+data.company+'"">'+AssignedCompany + '...</span>';
                        }
                        else
                        {
                            return data.company;
                        }
                    },
                    ""name"": ""Assigned Company"" 
                },
                {""data"": function (data, full, meta) {
                    return data.role;
                }, ""name"": ""Designation"" },
                { ""data"": ""countryOfRecidence"", ""name"": ""Country Of Residence"" },
                { ""data"": ""passportNumber"", ""name"": ""Passport No."" },
                { ""data"": ""passportExpiryDate"", ""name"": ""Passport Expiry Date."" },
                { ""data"": ""visaExpiryDate"", ""name"": ""Visa Expiry Date"" },
                { ""data"": ""insuarnceCompany"", ""name"": ""Insurance Company"" },
                { ""data"": ""insuranceExpiryDate"", ""name"": ""Insuarnce Expiry Date"" },
                {
                    ""data"": funct");
            WriteLiteral(@"ion (data, full, meta) {
                        var getSalesPersonDetails = ""ViewSalesPersonDetails("" + data.id + "")"";
                        var updateSalesPerson = ""UpdateSalesPerson("" + data.id + "")"";
                        var removeSalesPerson = ""RemoveSalesPerson("" + data.id + "")"";

                        var returnKey = """";
                        if ('");
            Write(
#nullable restore
#line 150 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\Index.cshtml"
                              Model.allowedModule.Contains("View")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\' == \"True\") {\r\n                            returnKey = \'<span class=\"btn btn-theme Update\" onclick=\' + getSalesPersonDetails + \'><i class=\"fas fa-eye\"></i></span>\';\r\n                        }\r\n                        if (\'");
            Write(
#nullable restore
#line 153 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\Index.cshtml"
                              Model.allowedModule.Contains("Update")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\' == \"True\") {\r\n                            returnKey = returnKey + \'<span class=\"btn btn-theme Update ms-1\" onclick=\' + updateSalesPerson + \'><i class=\"fas fa-edit\"></i></span>\';\r\n                        }\r\n                        if (\'");
            Write(
#nullable restore
#line 156 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\Index.cshtml"
                              Model.allowedModule.Contains("Delete")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(@"' == ""True"") {
                            returnKey = returnKey + '<span class=""btn btn-theme Delete ms-1"" onclick=' + removeSalesPerson + '><i class=""fas fa-trash""></i></span>';
                        }

                        return returnKey;
                    },
                    ""name"": """"
                },
            ],
            ""responsive"": true,
            ""deferRender"": true,
            ""language"": {
                'processing': '<div class=""spinner-2""><div class=""center-div-2""><div class=""loader-circle-1""></div></div></div>'
            },
            'columnDefs': [{
                'targets': [8],
                'orderable': false, 
            }],
            paging: true,
            pagingType: 'simple_numbers',
            ordering: true,
            searching: true,
            ""aLengthMenu"": [20, 30, 50],
            dom: ""<'row'<'col-sm-3'l><'col-sm-3'f><'col-sm-6'p>>"" +
                ""<'row'<'col-sm-12'tr>>"" +
                ""<'row'<'col-sm-5'i>");
            WriteLiteral("<\'col-sm-7\'p>>\",\r\n        });\r\n    });\r\n</script>\r\n<script>\r\n    function AddNew() {\r\n        var url = \"");
            Write(
#nullable restore
#line 187 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\Index.cshtml"
                    Url.Action("Create","SalesPerson")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\";\r\n        window.location.href = url;\r\n    }\r\n    function UpdateSalesPerson(Id) {\r\n        var url = \"");
            Write(
#nullable restore
#line 191 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\Index.cshtml"
                    Url.Action("Edit","SalesPerson")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(@""";
        window.location.href = url + ""?Id="" + Id;
    }
    $(document).ready(function () {
        $(""#loader"").hide();
    });
</script>
<script>
    function RemoveSalesPerson(Id) {
        event.preventDefault();
        const swalWithBootstrapButtons = Swal.mixin({
            customClass: {
                confirmButton: 'ms-3 btn btn-success',
                cancelButton: 'btn btn-danger'
            },
            buttonsStyling: false
        })

        swalWithBootstrapButtons.fire({
            title: 'Are you sure?',
            text: ""You won't be able to revert this!"",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'No, cancel!',
            reverseButtons: true,
            showConfirmButton: true,
        }).then((result) => {
            if (result.isConfirmed) {
                $(""#loader"").show();
                $.ajax({
                    url: """);
            Write(
#nullable restore
#line 222 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\Index.cshtml"
                           Url.Action("Delete", "SalesPerson")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral(@""",
                    method: ""GET"",
                    data: { ""Id"": Id },
                    success: function (response) {
                        $(""#loader"").hide();
                        swalWithBootstrapButtons.fire({
                            title: 'Deleted.',
                            text: ""Sales Person Deleted Successfully..!!"",
                            icon: 'success'
                        }).then((result) => {
                            window.location.reload();
                        });
                    }
                });
            }
            else if (
                result.dismiss === Swal.DismissReason.cancel
            ) {
                $(""#loader"").hide();
                swalWithBootstrapButtons.fire(
                    'Cancelled',
                    'Cancled by User..!! :)',
                    'error'
                )
            }
        });
    }
</script>
<script>
    function ViewSalesPersonDetails(Id) {
        var u");
            WriteLiteral("rl = \"");
            Write(
#nullable restore
#line 252 "D:\DIBN PORTAL\DIBN PORTAL - LIVE\DIBN\Areas\Admin\Views\SalesPerson\Index.cshtml"
                    Url.Action("SalesPersonDetails","SalesPerson")

#line default
#line hidden
#nullable disable
            );
            WriteLiteral("\";\r\n        url = url + \"?Id=\" + Id;\r\n        window.location.href = url;\r\n    };\r\n</script>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<DIBN.Areas.Admin.Models.MainSalesPersonViewModel> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
