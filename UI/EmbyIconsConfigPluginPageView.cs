using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emby.Web.GenericEdit.Common;
using EmbyIcons.Storage;
using EmbyIcons.UIBaseClasses.Views;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Activity;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Plugins.UI.Views;
using MediaBrowser.Model.Querying;

namespace EmbyIcons.UI;

internal class EmbyIconsConfigPluginPageView : PluginPageView
{
    private readonly IActivityManager   _activityManager;
    private readonly ILibraryManager    _libraryManager;
    private readonly EnhancedLogger     _logger;
    private readonly PluginOptionsStore _store;
    private readonly IUserManager       _userManager;

    public EmbyIconsConfigPluginPageView(PluginInfo pluginInfo,
                                         PluginOptionsStore store,
                                         EnhancedLogger logger,
                                         ILibraryManager libraryManager,
                                         IActivityManager activityManager,
                                         IUserManager userManager) : base(pluginInfo.Id)
    {
        ShowSave = false;

        _store           = store;
        _logger          = logger;
        _libraryManager  = libraryManager;
        _activityManager = activityManager;
        _userManager     = userManager;

        var data = store.GetOptions();
        data.AudioLanguageOptions    = GetLanguageOptions();
        data.SubtitleLanguageOptions = GetLanguageOptions(MediaStreamType.Subtitle);
        data.Libraries               = GetLibrariesOptions();

        ContentData = data;
    }

    public PluginUIOptions PluginUiOptions => ContentData as PluginUIOptions;

    public override Task<IPluginUIView> OnSaveCommand(string itemId, string commandId, string data)
    {
        //This is used when ShowSave = True, otherwise use a button to trigger
        _store.SetOptions(PluginUiOptions);
        return base.OnSaveCommand(itemId, commandId, data);
    }

    public override bool IsCommandAllowed(string commandKey)
    {
        if (commandKey == "SaveConfig") return true;

        if (commandKey == "RunJob") return true;

        return base.IsCommandAllowed(commandKey);
    }

    public override Task<IPluginUIView> RunCommand(string itemId, string commandId, string data)
    {
        if (commandId == "SaveConfig")
        {
            _store.SetOptions(PluginUiOptions);
            return Task.FromResult((IPluginUIView)this);
        }

        if (commandId == "RunJob")
        {
            _logger.Info("Starting job", true);

            _activityManager.Create(new ActivityLogEntry
                                    {
                                        Name          = "EmbyIcons Job Results",
                                        Overview      = $" <pre><code>{_logger.GetAllLogsAsString()}</code></pre>",
                                        ShortOverview = null,
                                        Type          = "EmbyIconsJobResults",
                                        ItemId        = null,
                                        Date          = DateTimeOffset.Now,
                                        UserId = GetAdminUser()
                                                 .InternalId.ToString(),
                                        Severity = LogSeverity.Info
                                    });
            return Task.FromResult((IPluginUIView)this);
        }

        return base.RunCommand(itemId, commandId, data);
    }

    public List<EditorSelectOption> GetLanguageOptions(MediaStreamType type = MediaStreamType.Audio)
    {
        var result = new List<EditorSelectOption>();

        var items = _libraryManager.GetItemList(new InternalItemsQuery
                                                {
                                                    IsVirtualItem = false,
                                                    Recursive     = true,
                                                    HasSubtitles  = true,
                                                    IncludeItemTypes = new[]
                                                                       {
                                                                           nameof(Episode),
                                                                           nameof(Movie)
                                                                       }
                                                });

        var languages = items.SelectMany(e => e.GetMediaStreams()
                                               .Where(x => x.Type == type)
                                               .Select(x => x.Language))
                             .Distinct()
                             .ToList();

        foreach (var language in languages)
        {
            var option = new EditorSelectOption
                         {
                             Value     = language,
                             Name      = language,
                             IsEnabled = true
                         };
            result.Add(option);
        }

        return result.OrderBy(x => x.Name)
                     .ToList();
    }

    private List<EditorSelectOption> GetLibrariesOptions(string collectionType = null)
    {
        var list = new List<EditorSelectOption>();
        var libraries = _libraryManager.GetVirtualFolders()
                                       .Where(x => !x.CollectionType.Equals(CollectionType.BoxSets.ToString(),
                                                                            StringComparison
                                                                                .InvariantCultureIgnoreCase) &&
                                                   (collectionType == null ||
                                                    x.CollectionType.Equals(collectionType,
                                                                            StringComparison
                                                                                .InvariantCultureIgnoreCase)))
                                       .ToList();
        foreach (var virtualFolderInfo in libraries)
            list.Add(new EditorSelectOption
                     {
                         Value     = virtualFolderInfo.Id,
                         Name      = virtualFolderInfo.Name,
                         IsEnabled = true
                     });
        return list;
    }

    private User GetAdminUser()
    {
        return _userManager.GetUsers(new UserQuery
                                     {
                                         IsAdministrator = true
                                     })
                           .Items.First();
    }
}