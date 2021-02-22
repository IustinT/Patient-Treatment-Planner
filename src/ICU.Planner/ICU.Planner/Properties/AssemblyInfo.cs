using Prism.Magician;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: NameFormatProvider(NameFormatProviderStyle.TypeName)]

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

[assembly: ExportFont("FA5Pro-Light.otf", Alias = "FAL")]
[assembly: ExportFont("FA5Pro-Regular.otf", Alias = "FAR")]
[assembly: ExportFont("FA5Pro-Solid.otf", Alias = "FAS")]
[assembly: ExportFont("Roboto-Regular.ttf", Alias = "RR")]
[assembly: ExportFont("Roboto-Medium.ttf", Alias = "RM")]