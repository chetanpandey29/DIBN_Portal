#pragma checksum "D:\Devotion Business\DIBN\DIBN\Views\Shared\Theme\_topbar.cshtml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "9e82644fa6c0a794908d0c381647d50bb8e721d8284ab4e64da9128186b07ed2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared_Theme__topbar), @"mvc.1.0.view", @"/Views/Shared/Theme/_topbar.cshtml")]
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
#line 1 "D:\Devotion Business\DIBN\DIBN\Views\_ViewImports.cshtml"
using DIBN;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Devotion Business\DIBN\DIBN\Views\_ViewImports.cshtml"
using DIBN.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA256", @"9e82644fa6c0a794908d0c381647d50bb8e721d8284ab4e64da9128186b07ed2", @"/Views/Shared/Theme/_topbar.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA256", @"7d5ddeb28e6b11bd8a350250841d25befde6ee969eb2ffe12afe4b0408fac805", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Shared_Theme__topbar : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"<style>
    .bell-font {
        background: #F4F3EA !important;
        color: #b08834 !important;
        font-weight: bold;
        font-size: 11px !important;
    }
</style>
<header id=""page-topbar"">
    <div class=""navbar-header"">
        <div class=""d-flex"">
            <!-- LOGO -->
            <div class=""navbar-brand-box"" style=""padding:0;"">
                <a");
            BeginWriteAttribute("href", " href=", 384, "", 436, 1);
#nullable restore
#line 14 "D:\Devotion Business\DIBN\DIBN\Views\Shared\Theme\_topbar.cshtml"
WriteAttributeValue("", 390, Url.Action("Index", "Home",new{area="admin"}), 390, 46, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\"logo logo-dark\">\r\n                    <span class=\"logo-sm\">\r\n                        <img src=\"https://res.cloudinary.com/dhokafmcn/image/upload/v1703249997/wwwroot/devotion_business_image/CORPORATE_SERVICE_PROVIDER.png\"");
            BeginWriteAttribute("alt", " alt=\"", 665, "\"", 671, 0);
            EndWriteAttribute();
            WriteLiteral(" height=\"30\">\r\n                    </span>\r\n                    <span class=\"logo-lg\">\r\n                        <img src=\"https://res.cloudinary.com/dhokafmcn/image/upload/v1703249997/wwwroot/devotion_business_image/CORPORATE_SERVICE_PROVIDER.png\"");
            BeginWriteAttribute("alt", " alt=\"", 919, "\"", 925, 0);
            EndWriteAttribute();
            WriteLiteral(" height=\"20\">\r\n                    </span>\r\n                </a>\r\n\r\n                <a");
            BeginWriteAttribute("href", " href=", 1012, "", 1064, 1);
#nullable restore
#line 23 "D:\Devotion Business\DIBN\DIBN\Views\Shared\Theme\_topbar.cshtml"
WriteAttributeValue("", 1018, Url.Action("Index", "Home",new{area="admin"}), 1018, 46, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\"logo logo-light\">\r\n                    <span class=\"logo-sm\">\r\n                        <img src=\"https://res.cloudinary.com/dhokafmcn/image/upload/v1703249997/wwwroot/devotion_business_image/CORPORATE_SERVICE_PROVIDER.png\"");
            BeginWriteAttribute("alt", " alt=\"", 1294, "\"", 1300, 0);
            EndWriteAttribute();
            WriteLiteral(" height=\"30\">\r\n                    </span>\r\n                    <span class=\"logo-lg\">\r\n                        <img src=\"https://res.cloudinary.com/dhokafmcn/image/upload/v1703249997/wwwroot/devotion_business_image/CORPORATE_SERVICE_PROVIDER.png\"");
            BeginWriteAttribute("alt", " alt=\"", 1548, "\"", 1554, 0);
            EndWriteAttribute();
            WriteLiteral(@" height=""20"">
                    </span>
                </a>
            </div>

            <button type=""button"" class=""btn btn-sm px-3 font-size-16 header-item waves-effect vertical-menu-btn"">
                <i class=""fas fa-fw fa-bars""></i>
            </button>
            <div class=""dropdown d-inline-block ms-3"">
                <button type=""button"" class=""btn header-item noti-icon waves-effect"" id=""page-header-notifications-dropdown"" data-bs-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">
                    <i class=""uil-bell front-text-blue""></i>
                    <span class=""badge bell-font rounded-pill"" id=""notificationCount""></span>
                </button>
                <div class=""dropdown-menu dropdown-menu-right"" id=""notificationMessage"">
                </div>
            </div>
        </div>

        <div class=""d-flex"">
            <div class=""dropdown d-none d-lg-inline-block"">
                <span class=""btn header-item waves-effect align-mid");
            WriteLiteral(@"dle mt-4 text-wrap overflow-hidden text-truncate ""><i class=""fas fa-question-circle""></i> &nbsp;<span class=""front-text-blue"">Help Center</span></span>
            </div>
            <div class=""dropdown d-none d-lg-inline-block"" id=""dibnlivesupport"">
                <span class=""btn header-item waves-effect align-middle mt-4 text-wrap overflow-hidden text-truncate""><i class=""fas fa-comments""></i> &nbsp;<span class=""front-text-blue"">DEVOTION Live Support</span></span>
            </div>
            <div class=""dropdown d-none d-lg-inline-block"">
                <span class=""btn header-item waves-effect align-middle mt-4 text-wrap overflow-hidden text-truncate ""><i class=""fas fa-phone-alt""></i>&nbsp; <span class=""front-text-blue"">+97143421947</span></span>
            </div>
            <div class=""dropdown d-inline-block"">
                <button type=""button"" class=""btn header-item waves-effect mt-2 text-truncate"" id=""page-header-user-dropdown""
                        data-bs-toggle=""dropdown"" aria");
            WriteLiteral(@"-haspopup=""true"" aria-expanded=""false"">
                    <span class=""d-xl-inline-block d-none ms-1 fw-medium font-size-15 align-middle text-truncate "">
                        <i class=""fas fa-globe""></i>&nbsp;<span class=""front-text-blue"">DEVOTION CORPORATE SERVICES L.L.C</span>
                    </span>
                    <i class=""uil-angle-down d-none d-xl-inline-block font-size-15 ""></i>
                </button>
                <div class=""dropdown-menu dropdown-menu-end"">
                    <!-- item-->
                    <a class=""dropdown-item CompanyServices15""");
            BeginWriteAttribute("href", " href=\"", 4197, "\"", 4270, 1);
#nullable restore
#line 66 "D:\Devotion Business\DIBN\DIBN\Views\Shared\Theme\_topbar.cshtml"
WriteAttributeValue("", 4204, Url.Action("Index", "CompanyService",new{name="CompanyServices"}), 4204, 66, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral("><i class=\"uil uil-user-circle font-size-18 align-middle me-1\" style=\"color:#B08434;\"></i> <span class=\"align-middle front-text-blue\">Access Company Services</span></a>\r\n                    <a class=\"dropdown-item EmployeeServices15\"");
            BeginWriteAttribute("href", " href=\"", 4504, "\"", 4579, 1);
#nullable restore
#line 67 "D:\Devotion Business\DIBN\DIBN\Views\Shared\Theme\_topbar.cshtml"
WriteAttributeValue("", 4511, Url.Action("Index", "EmployeeService",new{name="EmployeeServices"}), 4511, 68, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral("><i class=\"uil uil-wallet font-size-18 align-middle me-1\" style=\"color:#B08434;\"></i> <span class=\"align-middle front-text-blue\">Access Employees Services</span></a>\r\n                    <a class=\"dropdown-item MyRequests15\"");
            BeginWriteAttribute("href", " href=\"", 4804, "\"", 4873, 1);
#nullable restore
#line 68 "D:\Devotion Business\DIBN\DIBN\Views\Shared\Theme\_topbar.cshtml"
WriteAttributeValue("", 4811, Url.Action("Index", "ServiceRequests",new{name="MyRequests"}), 4811, 62, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@"><i class=""uil uil-wallet font-size-18 align-middle me-1"" style=""color:#B08434;""></i> <span class=""align-middle front-text-blue"">Track Your Request</span></a>
                </div>
            </div>
            <div class=""dropdown d-inline-block"">
                <button type=""button"" class=""btn header-item waves-effect mt-2 text-truncate"" id=""page-header-user-dropdown""
                        data-bs-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">
                    <span class=""d-xl-inline-block ms-1 fw-medium font-size-15 align-middle text-truncate ""><i class=""fas fa-cog""></i>&nbsp;<span class=""front-text-blue"">Settings</span></span>
                    <i class=""uil-angle-down d-xl-inline-block font-size-15 ""></i>
                </button>
                <div class=""dropdown-menu dropdown-menu-end"">
                    <!-- item-->
");
            WriteLiteral("                    <a class=\"dropdown-item\"");
            BeginWriteAttribute("href", " href=\"", 6165, "\"", 6220, 1);
#nullable restore
#line 83 "D:\Devotion Business\DIBN\DIBN\Views\Shared\Theme\_topbar.cshtml"
WriteAttributeValue("", 6172, Url.Action("LockScreen","Account",new{area=""}), 6172, 48, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral("><i class=\"uil uil-lock-alt font-size-18 align-middle overflow-hidden\" style=\"color:#B08434;\"></i> <span class=\"align-middle overflow-hidden text-truncate front-text-blue\">Lock screen</span></a>\r\n                    <a class=\"dropdown-item\"");
            BeginWriteAttribute("href", " href=\"", 6461, "\"", 6512, 1);
#nullable restore
#line 84 "D:\Devotion Business\DIBN\DIBN\Views\Shared\Theme\_topbar.cshtml"
WriteAttributeValue("", 6468, Url.Action("Logout","Account",new{area=""}), 6468, 44, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@"><i class=""uil uil-sign-out-alt font-size-18 align-middle overflow-hidden"" style=""color:#B08434;""></i> <span class=""align-middle overflow-hidden text-truncate front-text-blue"">Sign out</span></a>
                </div>
            </div>
            <input type=""hidden"" name=""support"" id=""supportid"" />
        </div>
    </div>
</header>
<script");
            BeginWriteAttribute("src", " src=\"", 6867, "\"", 6920, 1);
#nullable restore
#line 91 "D:\Devotion Business\DIBN\DIBN\Views\Shared\Theme\_topbar.cshtml"
WriteAttributeValue("", 6873, Url.Content("~/lib/jquery/dist/jquery.min.js"), 6873, 47, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral("></script>\r\n\r\n<script>\r\n    $(document).ready(function () {\r\n        $.ajax({\r\n            url: \"");
#nullable restore
#line 96 "D:\Devotion Business\DIBN\DIBN\Views\Shared\Theme\_topbar.cshtml"
             Write(Url.Action("GetLoggedInCompany","Home"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\",\r\n            method: \"POST\",\r\n            success: function (response) {\r\n                if (response != null && response == 1) {\r\n                    $.ajax({\r\n                        url: \"");
#nullable restore
#line 101 "D:\Devotion Business\DIBN\DIBN\Views\Shared\Theme\_topbar.cshtml"
                         Write(Url.Action("GetCurrentNotificationCount","Home"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\",\r\n                        method: \"POST\",\r\n                        success: function (service) {\r\n                            if (service != null) {\r\n                                var _url = \"");
#nullable restore
#line 105 "D:\Devotion Business\DIBN\DIBN\Views\Shared\Theme\_topbar.cshtml"
                                       Write(Url.Action("GetRequestNotifications","Home"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@""";
                                var _count = parseInt($(""#notificationCount"").text());
                                if (isNaN(_count)) {
                                    _count = 0;
                                }
                                _count = parseInt(_count) + parseInt(service);
                                var _message = ""<a class='dropdown-item' href="" + _url + "" style='color:#333D51;'>""
                                    + ""<span class='align-middle'><strong> You have "" + service + "" New Notification.</strong></span></a>"";

                                $(""#notificationCount"").text(_count);
                                $(""#notificationMessage"").append(_message);

                                $("".CompanyServices15"").hide();
                                $("".EmployeeServices15"").hide();
                            }

                        }
                    });
                }
                else {
                    $.ajax({
               ");
            WriteLiteral("         url: \"");
#nullable restore
#line 126 "D:\Devotion Business\DIBN\DIBN\Views\Shared\Theme\_topbar.cshtml"
                         Write(Url.Action("GetLoggedInRole","Home"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\",\r\n                        method: \"GET\",\r\n                        success: function (role) {\r\n                            if (!role.startsWith(\"Sales\")){\r\n                                $.ajax({\r\n                                    url: \"");
#nullable restore
#line 131 "D:\Devotion Business\DIBN\DIBN\Views\Shared\Theme\_topbar.cshtml"
                                     Write(Url.Action("GetNoticeMessage","Home"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@""",
                                    method: ""POST"",
                                    success: function (response) {
                                        if (response != null) {
                                            if (response.message != null) {
                                                var _url = """);
#nullable restore
#line 136 "D:\Devotion Business\DIBN\DIBN\Views\Shared\Theme\_topbar.cshtml"
                                                       Write(Url.Action("GetAllNotification","Home"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@""";
                                                var _message = ""<a class='dropdown-item' href="" + _url + "" style='color:#333D51;'>""
                                                    + ""<span class='align-middle'><strong> You have "" + response.notificationCount + "" New Notifications.</strong></span></a>"";
                                                $(""#notificationCount"").text(response.notificationCount);
                                                $(""#notificationMessage"").append(_message);
                                            }
                                            else {
                                                $(""#notificationCount"").text(response.notificationCount);
                                                $(""#notificationMessage"").html(""<strong>No Notification Found.</strong>"");
                                            }

                                        }

                                    }
                                });
          ");
            WriteLiteral(@"                  }
                            else{
                                $(""#notificationCount"").text(""0"");
                                $(""#notificationMessage"").html(""<strong>No Notification Found.</strong>"");
                                $("".CompanyServices15"").hide();
                                $("".EmployeeServices15"").hide();    
                            }
                        }
                    });
                }
            }
        });

    });
</script>
<script>
    $(""#dibnlivesupport"").on('click', function () {
        var supprort = $(""#supportid"").val();
        if (supprort == """") {
            $(""#supportid"").val(""true"");
            var Tawk_API = Tawk_API || {}, Tawk_LoadStart = new Date();
            (function () {
                var s1 = document.createElement(""script""), s0 = document.getElementsByTagName(""script"")[0];
                s1.async = true;
                s1.src = 'https://embed.tawk.to/62848be4b0d10b6f3e72c2c5/1g3as");
            WriteLiteral(@"4tf4';
                s1.charset = 'UTF-8';
                s1.setAttribute('crossorigin', '*');
                s0.parentNode.insertBefore(s1, s0);
            })();
        }
        else {
            window.location.reload();
        }
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591