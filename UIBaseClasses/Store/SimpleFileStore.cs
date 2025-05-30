﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Emby.Web.GenericEdit;
using MediaBrowser.Common;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;

namespace EmbyIcons.UIBaseClasses.Store;

public class SimpleFileStore<TOptionType> : SimpleContentStore<TOptionType>
    where TOptionType : EditableOptionsBase, new()
{
    private readonly IFileSystem     _fileSystem;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly object          _lockObj = new object();
    private readonly ILogger         _logger;
    private readonly string          _pluginConfigPath;
    private readonly string          _pluginFullName;

    private TOptionType _options;

    public SimpleFileStore(IApplicationHost applicationHost, ILogger logger, string pluginFullName)
    {
        _logger         = logger;
        _pluginFullName = pluginFullName;
        _jsonSerializer = applicationHost.Resolve<IJsonSerializer>();
        _fileSystem     = applicationHost.Resolve<IFileSystem>();

        var applicationPaths = applicationHost.Resolve<IApplicationPaths>();
        _pluginConfigPath = applicationPaths.PluginConfigurationsPath;

        if (!_fileSystem.DirectoryExists(_pluginConfigPath)) _fileSystem.CreateDirectory(_pluginConfigPath);

        OptionsFileName = $"{pluginFullName}.json";
    }

    public virtual string OptionsFileName { get; }

    public string OptionsFilePath => Path.Combine(_pluginConfigPath, OptionsFileName);

    public event EventHandler<FileSavingEventArgs> FileSaving;

    public event EventHandler<FileSavedEventArgs> FileSaved;

    public override TOptionType GetOptions()
    {
        lock (_lockObj)
        {
            if (_options == null) return ReloadOptions();

            return _options;
        }
    }

    public TOptionType ReloadOptions()
    {
        lock (_lockObj)
        {
            var tempOptions = _options ?? new TOptionType();

            try
            {
                if (!_fileSystem.FileExists(OptionsFilePath)) return tempOptions;

                using (var stream = _fileSystem.OpenRead(OptionsFilePath))
                {
                    var deserialized = tempOptions.DeserializeFromJsonStream(stream, _jsonSerializer);

                    _options = deserialized as TOptionType;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error loading plugin _options for {0} from {1}", ex, _pluginFullName,
                                       OptionsFilePath);
                _options = tempOptions;
            }

            return _options ?? new TOptionType();
        }
    }

    public override void SetOptions(TOptionType newOptions)
    {
        if (newOptions == null) throw new ArgumentNullException(nameof(newOptions));

        var savingArgs = new FileSavingEventArgs(newOptions);
        FileSaving?.Invoke(this, savingArgs);

        if (savingArgs.Cancel) return;

        lock (_lockObj)
        {
            // Create a dictionary to store only properties that are not marked with DontSave
            var filteredOptions = new Dictionary<string, object>();

            foreach (var property in typeof(TOptionType).GetProperties())
            {
                if (property.GetCustomAttributes(typeof(DontSaveAttribute), false)
                            .Any())
                    continue;

                if (property.CanRead)
                {
                    var value = property.GetValue(newOptions);
                    filteredOptions[property.Name] = value;
                }
            }

            using (var stream = _fileSystem.GetFileStream(OptionsFilePath, FileOpenMode.Create, FileAccessMode.Write))
            {
                // Serialize the filtered dictionary instead of the full object
                _jsonSerializer.SerializeToStream(filteredOptions, stream, new JsonSerializerOptions
                                                                           {
                                                                               Indent = true
                                                                           });
            }
        }

        lock (_lockObj)
        {
            _options = newOptions;
        }

        var savedArgs = new FileSavedEventArgs(newOptions);
        FileSaved?.Invoke(this, savedArgs);
    }
}