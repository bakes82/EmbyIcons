using System;
using System.Collections.Generic;
using System.IO;
using EmbyIcons;
using EmbyIcons.Storage;
using EmbyIcons.UI;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Activity;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Plugins.UI;
using MediaBrowser.Model.Serialization;

namespace Emby.MiscUtils;

public class EmbyIcons : BasePlugin, IHasThumbImage, IHasUIPages, IHasPluginConfiguration
{
    public static readonly string                        PluginName = "EmbyIcons";
    private readonly       IActivityManager              _activityManager;
    private readonly       IServerApplicationHost        _applicationHost;
    private readonly       IJsonSerializer               _jsonSerializer;
    private readonly       ILibraryManager               _libraryManager;
    private readonly       EnhancedLogger                _logger;
    private readonly       PluginOptionsStore            _pluginOptionsStore;
    private readonly       IUserManager                  _userManager;
    private                List<IPluginUIPageController> _pages;

    /// <summary>Initializes a new instance of the <see cref="MiscUtils" /> class.</summary>
    /// <param name="applicationHost">The application host.</param>
    /// <param name="logManager">The log manager.</param>
    /// <param name="libraryManager"></param>
    /// <param name="jsonSerializer"></param>
    /// <param name="activityManager"></param>
    /// <param name="userManager"></param>
    public EmbyIcons(ILogManager logManager,
                     IServerApplicationHost applicationHost,
                     ILibraryManager libraryManager,
                     IJsonSerializer jsonSerializer,
                     IActivityManager activityManager,
                     IUserManager userManager)
    {
        _libraryManager     = libraryManager;
        _jsonSerializer     = jsonSerializer;
        _activityManager    = activityManager;
        _userManager        = userManager;
        _applicationHost    = applicationHost;
        _logger             = new EnhancedLogger(logManager.GetLogger(PluginName));
        _pluginOptionsStore = new PluginOptionsStore(applicationHost, _logger, Name);
    }

    public override        Guid   Id          => new Guid("b8d0f5a4-3e96-4c0f-a6e2-9f0c2ecb5c5f");
    public override        string Description => "Overlays language icons onto media posters.";
    public sealed override string Name        => PluginName;

    /// <summary>
    ///     Gets the type of configuration this plugin uses
    /// </summary>
    /// <value>The type of the configuration.</value>
    public Type ConfigurationType => typeof(PluginUIOptions);

    /// <summary>
    ///     Completely overwrites the current configuration with a new copy
    ///     Returns true or false indicating success or failure
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <exception cref="System.ArgumentNullException">configuration</exception>
    public void UpdateConfiguration(BasePluginConfiguration configuration)
    {
    }

    /// <summary>
    ///     Gets the plugin's configuration
    /// </summary>
    /// <value>The configuration.</value>
    public BasePluginConfiguration Configuration { get; } = new BasePluginConfiguration();

    public void SetStartupInfo(Action<string> directoryCreateFn)
    {
    }

    /// <summary>Gets the thumb image format.</summary>
    /// <value>The thumb image format.</value>
    public ImageFormat ThumbImageFormat => ImageFormat.Jpg;

    /// <summary>Gets the thumb image.</summary>
    /// <returns>An image stream.</returns>
    public Stream GetThumbImage()
    {
        var type = GetType();
        return type.Assembly.GetManifestResourceStream(type.Namespace + "Images.logo.png");
    }

    public IReadOnlyCollection<IPluginUIPageController> UIPageControllers
    {
        get
        {
            if (_pages == null)
                _pages = new List<IPluginUIPageController>
                         {
                             new MainPageController(GetPluginInfo(), _pluginOptionsStore, _logger, _libraryManager,
                                                    _activityManager, _userManager)
                         };

            return _pages.AsReadOnly();
        }
    }
}