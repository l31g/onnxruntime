// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Runtime.InteropServices;

namespace Microsoft.ML.OnnxRuntime
{
    /// <summary>
    ///  Sets various runtime options. 
    /// </summary>
    public class RunOptions : SafeHandle
    {
        internal IntPtr Handle
        {
            get
            {
                return handle;
            }
        }

        /// <summary>
        /// Default __ctor. Creates default RuntimeOptions
        /// </summary>
        public RunOptions()
            : base(IntPtr.Zero, true)
        {
            NativeApiStatus.VerifySuccess(NativeMethods.OrtCreateRunOptions(out handle));
        }

        /// <summary>
        /// Overrides SafeHandle.IsInvalid
        /// </summary>
        /// <value>returns true if handle is equal to Zero</value>
        public override bool IsInvalid { get { return handle == IntPtr.Zero; } }

        /// <summary>
        /// Log Severity Level for the session logs. Default = ORT_LOGGING_LEVEL_WARNING
        /// </summary>
        public OrtLoggingLevel LogSeverityLevel
        {
            get
            {
                return _logSeverityLevel;
            }
            set
            {
                NativeApiStatus.VerifySuccess(NativeMethods.OrtRunOptionsSetRunLogSeverityLevel(handle, value));
                _logSeverityLevel = value;
            }
        }
        private OrtLoggingLevel _logSeverityLevel = OrtLoggingLevel.ORT_LOGGING_LEVEL_WARNING;

        /// <summary>
        /// Log Verbosity Level for the session logs. Default = 0. Valid values are >=0.
        /// This takes into effect only when the LogSeverityLevel is set to ORT_LOGGING_LEVEL_VERBOSE.
        /// </summary>
        public int LogVerbosityLevel
        {
            get
            {
                return _logVerbosityLevel;
            }
            set
            {
                NativeApiStatus.VerifySuccess(NativeMethods.OrtRunOptionsSetRunLogVerbosityLevel(handle, value));
                _logVerbosityLevel = value;
            }
        }
        private int _logVerbosityLevel = 0;

        /// <summary>
        /// Log tag to be used during the run. default = ""
        /// </summary>
        public string LogId
        {
            get
            {
                return _logId;
            }
            set
            {
                var utf8 = NativeOnnxValueHelper.StringToZeroTerminatedUtf8(value);
                NativeApiStatus.VerifySuccess(NativeMethods.OrtRunOptionsSetRunTag(handle, utf8));

                _logId = value;
            }
        }

        private string _logId = "";


        /// <summary>
        /// Sets a flag to terminate all Run() calls that are currently using this RunOptions object 
        /// Default = false
        /// </summary>
        /// <value>terminate flag value</value>
        public bool Terminate
        {
            get
            {
                return _terminate;
            }
            set
            {
                if (!_terminate && value)
                {
                    NativeApiStatus.VerifySuccess(NativeMethods.OrtRunOptionsSetTerminate(handle));
                    _terminate = true;
                }
                else if (_terminate && !value)
                {
                    NativeApiStatus.VerifySuccess(NativeMethods.OrtRunOptionsUnsetTerminate(handle));
                    _terminate = false;
                }
            }
        }
        private bool _terminate = false; //value set to default value of the C++ RunOptions

        /// <summary>
        /// Set a single run configuration entry as a pair of strings
        /// If a configuration with same key exists, this will overwrite the configuration with the given configValue.
        /// </summary>
        /// <param name="configKey">config key name</param>
        /// <param name="configValue">config key value</param>
        public void AddRunConfigEntry(string configKey, string configValue)
        {
            var utf8Key = NativeOnnxValueHelper.StringToZeroTerminatedUtf8(configKey);
            var utf8Value = NativeOnnxValueHelper.StringToZeroTerminatedUtf8(configValue);
            NativeApiStatus.VerifySuccess(NativeMethods.OrtAddRunConfigEntry(handle, utf8Key, utf8Value));
        }

        /// <summary>
        /// Appends the specified lora adapter to the list of active lora adapters
        /// for this RunOptions instance. All run calls with this instant will
        /// make use of the activated Lora Adapters. An adapter is considered active
        /// if it is added to RunOptions that are used during Run() calls.
        /// </summary>
        /// <param name="loraAdapter">Lora adapter instance</param>
        public void AddActiveLoraAdapter(OrtLoraAdapter loraAdapter)
        {
            NativeApiStatus.VerifySuccess(NativeMethods.OrtRunOptionsAddActiveLoraAdapter(handle, loraAdapter.Handle));
        }

        #region SafeHandle
        /// <summary>
        /// Overrides SafeHandle.ReleaseHandle() to properly dispose of
        /// the native instance of RunOptions
        /// </summary>
        /// <returns>always returns true</returns>
        protected override bool ReleaseHandle()
        {
            NativeMethods.OrtReleaseRunOptions(handle);
            handle = IntPtr.Zero;
            return true;
        }

        #endregion
    }
}