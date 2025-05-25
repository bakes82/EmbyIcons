using EmbyIcons.UIBaseClasses.Store;
using MediaBrowser.Common;
using MediaBrowser.Model.Logging;

namespace EmbyIcons.Storage;

public class PluginOptionsStore : SimpleFileStore<PluginUIOptions>
{
    public PluginOptionsStore(IApplicationHost applicationHost, ILogger logger, string pluginFullName) :
        base(applicationHost, logger, pluginFullName)
    {
    }
}