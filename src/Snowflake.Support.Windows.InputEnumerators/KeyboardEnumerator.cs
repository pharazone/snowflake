﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Snowflake.Extensibility;
using Snowflake.Extensibility.Provisioning;
using Snowflake.Input;
using Snowflake.Input.Device;
using Snowflake.Services;

namespace Snowflake.Plugin.InputEnumerators
{
    [Plugin("InputEnumerator-Keyboard")]
    public class KeyboardEnumerator : InputEnumerator
    {
        private readonly IInputManager inputManager;

        public KeyboardEnumerator(IPluginProvision p, IInputManager inputManager)
            : base(p)
        {
            this.inputManager = inputManager;
        }

        /// <inheritdoc/>
        public override IEnumerable<IInputDevice> GetConnectedDevices()
        {
            return from device in this.inputManager.GetAllDevices()
                where device?.DI_DeviceType == DeviceType.Keyboard
                select new InputDevice(InputApi.DirectInput, device, this.ControllerLayout);
        }
    }
}
