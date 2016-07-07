﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Snowflake.Ajax;
using Snowflake.Platform;
using Snowflake.Extensibility;
using Snowflake.Romfile;
using Snowflake.Scraper;
using Snowflake.Service.HttpServer;
using Snowflake.Service.JSWebSocketServer;
using Snowflake.Service.Manager;
using NLog;

namespace Snowflake.Service
{
   
    [Export(typeof(ICoreService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CoreService : ICoreService
    {
        #region Loaded Objects
        public IDictionary<string, IPlatformInfo> Platforms { get; }
        public string AppDataDirectory { get; }
        public dynamic InfoBlob { get; }
        private readonly IDictionary<Type, dynamic> serviceContainer;
        private ILogger logger;

        #endregion

        // Flag: Has Dispose already been called? 
        bool disposed;

        // Instantiate a SafeHandle instance.
    
        public CoreService(string appDataDirectory)
        {
            this.logger = LogManager.GetLogger("~CORESERVICE");
            this.serviceContainer = new ConcurrentDictionary<Type, object>();
            this.RegisterService<IStoneProvider>(new StoneProvider());
            this.AppDataDirectory = appDataDirectory;
            this.InfoBlob = JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(this.AppDataDirectory, "info.json")));

            this.RegisterService<IServerManager>(new ServerManager());
         
            this.RegisterService<IPluginManager>(new PluginManager(this.AppDataDirectory, this));
            this.RegisterService<IAjaxManager>(new AjaxManager(this));
            var serverManager = this.Get<IServerManager>();
            serverManager.RegisterServer("AjaxApiServer", new ApiServer(this));
            serverManager.RegisterServer("WebSocketApiServer", new JsonApiWebSocketServer(30003, this));
            
        }

        public void RegisterService<T>(T serviceObject)
        {
            if (this.serviceContainer.ContainsKey(typeof (T))) return;
            this.serviceContainer.Add(typeof(T), serviceObject);
        }

        public IEnumerable<string> AvailableServices()
        {
            return this.serviceContainer.Keys.Select(service => service.Name);
        } 

        public T Get<T>()
        {
            return this.serviceContainer.ContainsKey(typeof (T)) ? this.serviceContainer[typeof (T)] : default(T);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if (disposing)
            {
              this.Get<IPluginManager>().Dispose();
              this.Get<IServerManager>().Dispose();
              GC.Collect();

            }

            // Free any unmanaged objects here. 
            //
            this.disposed = true;
        }
    }
}
