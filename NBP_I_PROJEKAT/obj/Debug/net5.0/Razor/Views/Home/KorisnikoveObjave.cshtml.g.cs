#pragma checksum "C:\Users\Ivan\Desktop\NBP_I_PROJEKAT\NBP_I_PROJEKAT\Views\Home\KorisnikoveObjave.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "33047d5f38581d0bee765bd0b380310bec15db3c"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(NBP_I_PROJEKAT.Home.Views_Home_KorisnikoveObjave), @"mvc.1.0.razor-page", @"/Views/Home/KorisnikoveObjave.cshtml")]
namespace NBP_I_PROJEKAT.Home
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\Ivan\Desktop\NBP_I_PROJEKAT\NBP_I_PROJEKAT\Views\_ViewImports.cshtml"
using NBP_I_PROJEKAT;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\Ivan\Desktop\NBP_I_PROJEKAT\NBP_I_PROJEKAT\Views\Home\KorisnikoveObjave.cshtml"
using NBP_I_PROJEKAT.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"33047d5f38581d0bee765bd0b380310bec15db3c", @"/Views/Home/KorisnikoveObjave.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"7ca934b199857851c97d596b0d1b4abca4a1d9f3", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_KorisnikoveObjave : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "Objava", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "PrikaziPojedinacneObjaveKorisnika", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("card-link"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 5 "C:\Users\Ivan\Desktop\NBP_I_PROJEKAT\NBP_I_PROJEKAT\Views\Home\KorisnikoveObjave.cshtml"
  
   // ViewData["Title"] = "Objave";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div class=\"container\">\r\n    <div class=\"row main-row d-flex justify-content-center\">\r\n");
#nullable restore
#line 11 "C:\Users\Ivan\Desktop\NBP_I_PROJEKAT\NBP_I_PROJEKAT\Views\Home\KorisnikoveObjave.cshtml"
         if (Model.Count > 0)
            foreach (var o in Model)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <div class=\"col-3 mt-2 d-flex justify-content-center\">\r\n                    <div class=\"card card-shadow\" style=\"width: 18rem;\">\r\n                        <div class=\"card-body\">\r\n                            <h5 class=\"card-title\">");
#nullable restore
#line 17 "C:\Users\Ivan\Desktop\NBP_I_PROJEKAT\NBP_I_PROJEKAT\Views\Home\KorisnikoveObjave.cshtml"
                                              Write(o.Naziv);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h5>\r\n                            <h6 class=\"card-subtitle mb-2 text-muted\">Tag: ");
#nullable restore
#line 18 "C:\Users\Ivan\Desktop\NBP_I_PROJEKAT\NBP_I_PROJEKAT\Views\Home\KorisnikoveObjave.cshtml"
                                                                      Write(o.Tag);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h6>\r\n                            <p class=\"card-text\">Sadrzaj: ");
#nullable restore
#line 19 "C:\Users\Ivan\Desktop\NBP_I_PROJEKAT\NBP_I_PROJEKAT\Views\Home\KorisnikoveObjave.cshtml"
                                                     Write(o.Sadrzaj);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\r\n                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "33047d5f38581d0bee765bd0b380310bec15db3c6026", async() => {
                WriteLiteral("Otvori");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-objavaId", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 20 "C:\Users\Ivan\Desktop\NBP_I_PROJEKAT\NBP_I_PROJEKAT\Views\Home\KorisnikoveObjave.cshtml"
                                                                                                              WriteLiteral(o.Id);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["objavaId"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-objavaId", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["objavaId"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n");
#nullable restore
#line 24 "C:\Users\Ivan\Desktop\NBP_I_PROJEKAT\NBP_I_PROJEKAT\Views\Home\KorisnikoveObjave.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </div>\r\n</div>\r\n\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<List<Objava>> Html { get; private set; }
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<List<Objava>> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<List<Objava>>)PageContext?.ViewData;
        public List<Objava> Model => ViewData.Model;
    }
}
#pragma warning restore 1591
