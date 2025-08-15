using MudBlazor;

namespace HomeAutomation.Web.Services;

public sealed class ThemeService
{
    public MudTheme CurrentTheme { get; private set; }
    public Palette CurrentPalette => IsDarkMode ? CurrentTheme.PaletteDark : CurrentTheme.PaletteLight;
    public bool IsDarkMode { get; private set; }

    private readonly MudTheme _lightTheme = new()
    {
        PaletteLight = new()
        {
            Black = "#353333ff",
            White = "rgba(255,255,255,1)",
            Primary = "#9ac2f9ff",
            PrimaryContrastText = "#ffffffff",
            Secondary = "#30751fff",
            SecondaryContrastText = "rgba(255,255,255,1)",
            Tertiary = "#b53e3eff",
            TertiaryContrastText = "rgba(255,255,255,1)",
            Info = "rgba(33,150,243,1)",
            InfoContrastText = "rgba(255,255,255,1)",
            Success = "rgba(0,200,83,1)",
            SuccessContrastText = "rgba(255,255,255,1)",
            Warning = "rgba(255,152,0,1)",
            WarningContrastText = "rgba(255,255,255,1)",
            Error = "rgba(244,67,54,1)",
            ErrorContrastText = "rgba(255,255,255,1)",
            Dark = "rgba(66,66,66,1)",
            DarkContrastText = "rgba(255,255,255,1)",
            TextPrimary = "#424242ff",
            TextSecondary = "rgba(0,0,0,0.5372549019607843)",
            TextDisabled = "rgba(0,0,0,0.3764705882352941)",
            ActionDefault = "#30751fff",
            ActionDisabled = "#00000042",
            ActionDisabledBackground = "#0000001e",
            Background = "#ebeaeaff",
            BackgroundGray = "#e9e6e6ff",
            Surface = "#ffffffff",
            DrawerBackground = "#9ac2f9ff",
            DrawerText = "rgba(66,66,66,1)",
            DrawerIcon = "#30751fff",
            AppbarBackground = "#9ac2f9ff",
            AppbarText = "rgba(255,255,255,1)",
            LinesDefault = "#78777791",
            LinesInputs = "#78777791",
            TableLines = "#78777791",
            TableStriped = "rgba(0,0,0,0.0196078431372549)",
            TableHover = "rgba(0,0,0,0.0392156862745098)",
            Divider = "#78777791",
            DividerLight = "rgba(0,0,0,0.8)",
            PrimaryDarken = "#448dfbff",
            PrimaryLighten = "#deebf9ff",
            SecondaryDarken = "#104902ff",
            SecondaryLighten = "#48b02eff",
            TertiaryDarken = "#873030ff",
            TertiaryLighten = "#e75353ff",
            InfoDarken = "rgb(12,128,223)",
            InfoLighten = "rgb(71,167,245)",
            SuccessDarken = "rgb(0,163,68)",
            SuccessLighten = "rgb(0,235,98)",
            WarningDarken = "rgb(214,129,0)",
            WarningLighten = "rgb(255,167,36)",
            ErrorDarken = "rgb(242,28,13)",
            ErrorLighten = "rgb(246,96,85)",
            DarkDarken = "rgb(46,46,46)",
            DarkLighten = "rgb(87,87,87)",
            HoverOpacity = 0.25,
            RippleOpacity = 0.7,
            RippleOpacitySecondary = 0.2,
            GrayDefault = "#9E9E9E",
            GrayLight = "#BDBDBD",
            GrayLighter = "#E0E0E0",
            GrayDark = "#757575",
            GrayDarker = "#616161",
            OverlayDark = "rgba(33,33,33,0.4980392156862745)",
            OverlayLight = "#ffffff7f",
        }
    };

    private readonly MudTheme _darkTheme = new()
    {
        PaletteDark = new()
        {
            Black = "rgba(39,39,47,1)",
            Primary = "#448dfbff",
            Secondary = "#20680eff",
            Tertiary = "#e75353ff",
            Info = "rgba(50,153,255,1)",
            Success = "rgba(11,186,131,1)",
            Warning = "rgba(255,168,0,1)",
            Error = "rgba(246,78,98,1)",
            Dark = "rgba(39,39,47,1)",
            TextPrimary = "rgba(255,255,255,0.6980392156862745)",
            TextSecondary = "rgba(255,255,255,0.4980392156862745)",
            TextDisabled = "rgba(255,255,255,0.2)",
            ActionDefault = "rgba(173,173,177,1)",
            ActionDisabled = "rgba(255,255,255,0.25882352941176473)",
            ActionDisabledBackground = "rgba(255,255,255,0.11764705882352941)",
            Background = "#595454ff",
            BackgroundGray = "rgba(39,39,47,1)",
            Surface = "rgba(55,55,64,1)",
            DrawerBackground = "rgba(39,39,47,1)",
            DrawerText = "#ffffff7f",
            DrawerIcon = "#ffffff7f",
            AppbarBackground = "rgba(39,39,47,1)",
            AppbarText = "#ffffff7f",
            LinesDefault = "rgba(255,255,255,0.11764705882352941)",
            LinesInputs = "rgba(255,255,255,0.2980392156862745)",
            TableLines = "rgba(255,255,255,0.11764705882352941)",
            TableStriped = "rgba(255,255,255,0.2)",
            TableHover = "#0000006d",
            Divider = "rgba(255,255,255,0.11764705882352941)",
            DividerLight = "rgba(255,255,255,0.058823529411764705)",
            PrimaryDarken = "#0d6effff",
            PrimaryLighten = "#9ac2f9ff",
            SecondaryDarken = "#0b2d04ff",
            SecondaryLighten = "#104902ff",
            TertiaryLighten = "#ff5d5dff",
            InfoDarken = "rgb(10,133,255)",
            InfoLighten = "rgb(92,173,255)",
            SuccessDarken = "rgb(9,154,108)",
            SuccessLighten = "rgb(13,222,156)",
            WarningDarken = "rgb(214,143,0)",
            WarningLighten = "rgb(255,182,36)",
            ErrorDarken = "rgb(244,47,70)",
            ErrorLighten = "rgb(248,119,134)",
            DarkDarken = "rgb(23,23,28)",
            DarkLighten = "rgb(56,56,67)",
            HoverOpacity = 0.25,
            RippleOpacity = 0.7,
        }
    };

    public ThemeService()
    {
        CurrentTheme = _darkTheme;
        IsDarkMode = false;
    }

    public void ToggleTheme(bool isDarkMode)
    {
        if (isDarkMode)
        {
            CurrentTheme = _darkTheme;
            IsDarkMode = true;
        }
        else
        {
            CurrentTheme = _lightTheme;
            IsDarkMode = false;
        }
    }

    public void ToggleTheme() => ToggleTheme(!IsDarkMode);
}