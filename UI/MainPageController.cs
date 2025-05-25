using System.Threading.Tasks;
using EmbyIcons.Storage;
using EmbyIcons.UIBaseClasses;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Activity;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Plugins.UI.Views;

namespace EmbyIcons.UI;

internal class MainPageController : ControllerBase
{
    private readonly IActivityManager   _activityManager;
    private readonly ILibraryManager    _libraryManager;
    private readonly EnhancedLogger     _logger;
    private readonly PluginInfo         _pluginInfo;
    private readonly PluginOptionsStore _pluginOptionsStore;
    private readonly IUserManager       _userManager;

    /// <summary>Initializes a new instance of the <see cref="ControllerBase" /> class.</summary>
    /// <param name="pluginInfo">The plugin information.</param>
    /// <param name="pluginOptionsStore"></param>
    /// <param name="logger"></param>
    /// <param name="libraryManager"></param>
    /// <param name="activityManager"></param>
    /// <param name="userManager"></param>
    public MainPageController(PluginInfo pluginInfo,
                              PluginOptionsStore pluginOptionsStore,
                              EnhancedLogger logger,
                              ILibraryManager libraryManager,
                              IActivityManager activityManager,
                              IUserManager userManager) : base(pluginInfo.Id)
    {
        _pluginInfo         = pluginInfo;
        _pluginOptionsStore = pluginOptionsStore;
        _logger             = logger;
        _libraryManager     = libraryManager;
        _activityManager    = activityManager;
        _userManager        = userManager;

        PageInfo = new PluginPageInfo
                   {
                       Name             = "EmbyIcons",
                       EnableInMainMenu = true,
                       DisplayName      = "Emby Icons",
                       MenuIcon         = "list_alt",
                       IsMainConfigPage = true
                   };
    }

    public override PluginPageInfo PageInfo { get; }

    public override Task<IPluginUIView> CreateDefaultPageView()
    {
        IPluginUIView view = new EmbyIconsConfigPluginPageView(_pluginInfo, _pluginOptionsStore, _logger,
                                                               _libraryManager, _activityManager, _userManager);
        return Task.FromResult(view);
    }
}