using System.Collections.Generic;
using System.ComponentModel;
using Emby.Web.GenericEdit;
using Emby.Web.GenericEdit.Common;
using Emby.Web.GenericEdit.Elements;
using MediaBrowser.Model.Attributes;

namespace EmbyIcons;

public class PluginUIOptions : EditableOptionsBase
{
    //private string _iconsFolder = @"D:\icons";
    //private string _logFolder = @"C:\temp";

    public PluginUIOptions()
    {
        //_iconsFolder = Environment.ExpandEnvironmentVariables(_iconsFolder);
        //_logFolder = Environment.ExpandEnvironmentVariables(_logFolder);

        IconSize = 10;

        AudioIconAlignment    = IconAlignment.TopLeft;
        SubtitleIconAlignment = IconAlignment.BottomLeft;

        //EnableLogging = true;

        AudioLanguages    = "eng,dan,fre,ger,spa,pol,jpn";
        SubtitleLanguages = "eng,dan,fre,ger,spa,pol,jpn";

        ShowAudioIcons    = true;
        ShowSubtitleIcons = true;

        SelectedLibraries = string.Empty;

        //SupportedExtensions = ".mkv,.mp4,.avi,.mov";

        //SubtitleFileExtensions = ".srt,.ass,.vtt";

        ShowSeriesIconsIfAllEpisodesHaveLanguage = true;

        AudioIconVerticalOffset    = 0;
        SubtitleIconVerticalOffset = 0;

        // Default for force refresh counter
        //ForceOverlayRefreshCounter = 0;
    }

    public override string EditorTitle => "EmbyIcons Settings";

    public override string EditorDescription =>
        "<h2 style='color:red; font-weight:bold;'>Refreshing metadata or server reset might be needed when changing an icon for one with the same name!</h2><br/>" +
        "Best to test your settings with one video at a time but not required.";

    [DontSave]
    public CaptionItem GeneralSettings { get; set; } = new CaptionItem("General Settings");

    [DisplayName("Icons Folder Path")]
    [Description("Full path containing language icon files.")]
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Icons folder path is required.")]
    //[CustomValidation(typeof(PluginUIOptions), nameof(ValidateIconsFolder))]
    [EditFilePicker]
    public string IconsFolder { get; set; }

    [DisplayName("Icon Size (% of shorter side)")]
    [Description("Size of icons overlaid on posters as % of the poster's shorter dimension.")]
    public int IconSize { get; set; }

    [Browsable(false)]
    [DontSave]
    public IEnumerable<EditorSelectOption> Libraries { get; set; }

    [DisplayName("Restrict to Libraries (comma separated names)")]
    [Description("Comma-separated list of library names to restrict plugin operation. Leave empty to process all libraries.")]
    [SelectItemsSource(nameof(Libraries))]
    [EditMultilSelect]
    public string SelectedLibraries { get; set; }

    [DontSave]
    public CaptionItem AudioSettings { get; set; } = new CaptionItem("Audio Settings");

    [DisplayName("Audio Icon Alignment")]
    [Description("Corner of the poster where audio icons are overlayed.")]
    public IconAlignment AudioIconAlignment { get; set; }

    [DisplayName("Audio Icon Vertical Offset (%)")]
    [Description("Vertical offset as % of the poster's shorter dimension. Positive moves down, negative moves up.")]
    public int AudioIconVerticalOffset { get; set; }

    [Browsable(false)]
    [DontSave]
    public IEnumerable<EditorSelectOption> AudioLanguageOptions { get; set; }

    [DisplayName("Audio Languages to Detect")]
    [Description("Comma-separated audio language codes (e.g., eng,dan,jpn). Only these audio languages will have icons overlaid.")]
    [SelectItemsSource(nameof(AudioLanguageOptions))]
    [EditMultilSelect]
    public string AudioLanguages { get; set; }

    [DisplayName("Show Audio Icons")]
    [Description("Enable or disable overlaying audio language icons.")]
    public bool ShowAudioIcons { get; set; }

    /*[DisplayName("Supported Media Extensions")]
    [Description("Comma-separated list of supported media file extensions for language detection.")]
    public string SupportedExtensions { get; set; }*/

    [DisplayName("Show Series Icons If All Episodes Have Language")]
    [Description("Show icons on series posters if all episodes have the specified audio/subtitle languages.")]
    public bool ShowSeriesIconsIfAllEpisodesHaveLanguage { get; set; }

    /// <summary>
    ///     ///////////////////////////////////////////////////////////////////
    /// </summary>

    [DontSave]
    public CaptionItem SubtitleSettings { get; set; } = new CaptionItem("Subtitle Settings");

    [DisplayName("Subtitle Icon Alignment")]
    [Description("Corner of the poster where subtitle icons are overlayed.")]
    public IconAlignment SubtitleIconAlignment { get; set; }

    [DisplayName("Subtitle Icon Vertical Offset (%)")]
    [Description("Vertical offset as % of the poster's shorter dimension. Positive moves down, negative moves up.")]
    public int SubtitleIconVerticalOffset { get; set; }

    [Browsable(false)]
    [DontSave]
    public IEnumerable<EditorSelectOption> SubtitleLanguageOptions { get; set; }

    [DisplayName("Subtitle Languages to Detect")]
    [Description("Comma-separated subtitle language codes (e.g., eng,dan,jpn). Only these subtitle languages will have icons overlaid.")]
    [SelectItemsSource(nameof(SubtitleLanguageOptions))]
    [EditMultilSelect]
    public string SubtitleLanguages { get; set; }

    [DisplayName("Show Subtitle Icons")]
    [Description("Enable or disable overlaying subtitle language icons.")]
    public bool ShowSubtitleIcons { get; set; }

    /*[DisplayName("External Subtitle File Extensions")]
    [Description("Comma-separated list of external subtitle file extensions to scan (e.g., .srt,.ass,.vtt).")]
    public string SubtitleFileExtensions { get; set; }*/

    [DontSave]
    public ButtonItem SaveConfig =>
        new ButtonItem
        {
            Icon    = IconNames.save,
            Caption = "Save Config Options",
            Data1   = "SaveConfig"
        };

    [DontSave]
    public ButtonItem RunJob =>
        new ButtonItem
        {
            Icon    = IconNames.run_circle,
            Caption = "Make Overlays",
            Data1   = "RunJob"
        };

    /*[DisplayName("Enable Logging")]
    [Description("Enable or disable plugin logging.")]
    [Display(Order = 1000)]
    public bool EnableLogging { get; set; }

    [DisplayName("Log Folder Path")]
    [Description("Folder path where plugin logs will be saved.")]
    [Display(Order = 1001)]
    public string LogFolder
    {
        get => _logFolder;
        set => _logFolder = Environment.ExpandEnvironmentVariables(value ?? @"C:\temp");
    }

    // -------------- DUMMY COUNTER ADDED HERE ------------------
    [DisplayName("Force Overlay Refresh (increment & save)")]
    [Description("Increase this number and press Save to force all overlays/icons to regenerate. No other effect.")]
    [Display(Order = 2002)]
    public int ForceOverlayRefreshCounter { get; set; }
    // ----------------------------------------------------------*/

    /*public static ValidationResult? ValidateIconsFolder(string? folderPath, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            return new ValidationResult("Icons folder path cannot be empty.");

        if (!Directory.Exists(folderPath))
            return new ValidationResult($"Icons folder path '{folderPath}' does not exist.");

        try
        {
            var pngFiles = Directory.GetFiles(folderPath, "*.png");
            if (pngFiles.Length == 0)
                return new ValidationResult($"No PNG icon files found in '{folderPath}'.");
        }
        catch (Exception ex)
        {
            return new ValidationResult($"Error accessing icons folder: {ex.Message}");
        }

        return ValidationResult.Success;
    }*/
}