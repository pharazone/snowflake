﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Castle.DynamicProxy;
using Snowflake.Configuration.Attributes;
using Snowflake.Configuration.Input;
using Snowflake.Configuration.Interceptors;
using Snowflake.Input.Controller;
using Snowflake.Input.Controller.Extensions;
using Snowflake.Input.Controller.Mapped;
using Snowflake.Input.Device;

namespace Snowflake.Configuration.Input
{
    public class InputTemplate<T> : IInputTemplate<T>
        where T : class, IInputTemplate<T>
    {
        /// <inheritdoc/>
        public int PlayerIndex { get; }

        /// <inheritdoc/>
        public T Template { get; }

        /// <inheritdoc/>
        public IDictionary<string, ControllerElement> Values
            => ImmutableDictionary.CreateRange(this.inputTemplateInterceptor.InputValues);

        /// <inheritdoc/>
        IEnumerable<IInputOption> IInputTemplate.Options => ImmutableList.CreateRange(this._Options.Select(p => p.Value));

        private IConfigurationSection<T> Configuration { get; }

        private IDictionary<string, IInputOption> _Options { get; }

        private readonly InputTemplateInterceptor<T> inputTemplateInterceptor;
        private readonly IList<IConfigurationOptionDescriptor> configurationOptions;

        /// <inheritdoc/>
        public ControllerElement this[ControllerElement virtualElement]
        {
            get
            {
                string optionKey = (from option in this._Options
                                    where option.Value.TargetElement == virtualElement
                                    select option.Key).FirstOrDefault();
                if (optionKey == null)
                {
                    throw new KeyNotFoundException("This template does not support the target element or element type.");
                }

                return this.inputTemplateInterceptor.InputValues[optionKey];
            }

            set
            {
                string optionKey = (from option in this._Options
                                    where option.Value.TargetElement == virtualElement
                                    where option.Value.InputOptionType.HasFlag(InputOptionType.Keyboard) == value.IsKeyboardKey()
                                    where option.Value.InputOptionType.HasFlag(InputOptionType.ControllerAxes) == value.IsAxis()
                                    select option.Key).FirstOrDefault();
                if (optionKey == null)
                {
                    throw new KeyNotFoundException("This template does not support the target element or element type.");
                }

                this.inputTemplateInterceptor.InputValues[optionKey] = value;
            }
        }

        public InputTemplate(IMappedControllerElementCollection mappedElements, int playerIndex = 0)
        {
            this.PlayerIndex = playerIndex;
            ProxyGenerator generator = new ProxyGenerator();
            this._Options = (from prop in typeof(T).GetProperties()
                let inputOptionAttribute = prop.GetCustomAttribute<InputOptionAttribute>()
                where inputOptionAttribute != null
                let name = prop.Name
                select new KeyValuePair<string, IInputOption>(name, new InputOption(inputOptionAttribute, name))).ToDictionary(o => o.Key,
                    o => o.Value);
            var overrides = (from element in mappedElements
                from key in this._Options.Keys
                let option = this._Options[key]
                let target = option.TargetElement
                where element.LayoutElement == target
                where option.InputOptionType.HasFlag(InputOptionType.Keyboard) == element.DeviceElement.IsKeyboardKey()
                where option.InputOptionType.HasFlag(InputOptionType.ControllerAxes) == element.DeviceElement.IsAxis()
                select new { key, element.DeviceElement }).ToDictionary(d => d.key, d => d.DeviceElement);
            var map = from key in this._Options.Keys
                let value = overrides.ContainsKey(key) ? overrides[key] : ControllerElement.NoElement
                select new KeyValuePair<string, ControllerElement>(key, value);
            this.configurationOptions = (from prop in typeof(T).GetProperties()
                          let configAttribute = prop.GetCustomAttribute<ConfigurationOptionAttribute>()
                          where configAttribute != null
                          let name = prop.Name
                          let metadata = prop.GetCustomAttributes<CustomMetadataAttribute>()
                          select new ConfigurationOptionDescriptor(configAttribute, metadata, name) as IConfigurationOptionDescriptor).ToList();

            var configOptionValues = new Dictionary<string, IConfigurationValue>();
            foreach (var custom in this.configurationOptions)
            {
                configOptionValues[custom.OptionKey] = new ConfigurationValue(custom.Default);
            }

            var attr = typeof(T).GetTypeInfo().GetCustomAttribute<InputTemplateAttribute>();

            this.inputTemplateInterceptor = new InputTemplateInterceptor<T>(map.ToDictionary(m => m.Key, m => m.Value), configOptionValues);
            var circular = new InputTemplateCircularInterceptor<T>(this);
            this.Configuration = new InputConfigurationSection<T>(circular, this.inputTemplateInterceptor);
            this.Template = generator.CreateInterfaceProxyWithoutTarget<T>(circular, this.inputTemplateInterceptor);
        }

        /// <inheritdoc/>
        IDictionary<string, IConfigurationValue> IConfigurationSection.Values => this.Configuration.Values;

        /// <inheritdoc/>
        IConfigurationSectionDescriptor IConfigurationSection.Descriptor => this.Configuration.Descriptor;

        /// <inheritdoc/>
        object IConfigurationSection.this[string key]
        {
            get { return this.Configuration[key]; }
            set { this.Configuration[key] = value; }
        }

        /// <inheritdoc/>
        T IConfigurationSection<T>.Configuration => this.Configuration.Configuration;

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<IConfigurationOptionDescriptor, IConfigurationValue>> GetEnumerator()
        {
            return this.Configuration.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
