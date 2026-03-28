using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace VRCFTStarVRModule
{
    class StarVRNativeInterface
    {
        private bool isInit;
        protected StarVREyePose leftEyeData;
        protected StarVREyePose rightEyeData;
        protected ILogger Logger;

        #region Lifetime methods (Init, Update, Teardown)
        public bool Initialize(ILogger loggerInstance)
        {
            Logger = loggerInstance;

            if (LoadLibrary())
            {
                // check if ET is initialized
                var error = StarVR_EyeTracking_IsInit(out isInit);

                if (error != StarVRError.StarVRError_None)
                {
                    Logger.LogWarning($"API returned Error {error} ; isInit value is {isInit}");
                }
                else if (!isInit)
                {
                    Logger.LogWarning("API returned no error, but ET is not initialized");
                }

                // we can continue if no SVR Error is reported, and ET is initialized
                return error == StarVRError.StarVRError_None && isInit;
            }
            return false;
        }

        public void Teardown()
        {
        }

        /// <summary>
        /// Get's the newest Data from the SDK and stores it internally
        /// </summary>
        /// <returns>true when the data fetching was successful</returns>
        public bool Update()
        {
            if (!isInit)
                return false;

            // Get's Gaze Data from the StarVR API
            var leftError = StarVR_EyeTracking_GetLeftEyePose(out leftEyeData);
            var rightError = StarVR_EyeTracking_GetRightEyePose(out rightEyeData);

            // we have data if no error was reported
            bool hasLeftData = leftError == StarVRError.StarVRError_None;
            bool hasRightData = rightError == StarVRError.StarVRError_None;

            if (!hasLeftData)
                Logger.LogWarning("Error while getting Left Gaze Data; Error: " + leftError.ToString());

            if (!hasRightData)
                Logger.LogWarning("Error while getting Right Gaze Data; Error: " + rightError.ToString());

            return hasLeftData && hasRightData;
        }
        #endregion

        #region Public Getters
        public StarVREyePose GetLeftEye()
        {
            return leftEyeData;
        }
        public StarVREyePose GetRightEye()
        {
            return rightEyeData;
        }
        #endregion

        #region Internal helper methods
        private bool LoadLibrary()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\TrackingLibs\\starvr_api.dll";
            if (path == null)
            {
                Logger.LogError(string.Concat("Lib not found ", path));
                return false;
            }
            if (LoadLibrary(path) == IntPtr.Zero)
            {
                Logger.LogError(string.Concat("Unable to load library ", path));
                return false;
            }
            Logger.LogInformation(string.Concat("Loaded library ", path));
            return true;
        }

        private bool StarVRAvailable()
        {
            bool _isInit;
            var error = StarVR_EyeTracking_IsInit(out _isInit);

            return error == StarVRError.StarVRError_None && _isInit;
        }
        #endregion

        #region DllImports
        [DllImport("kernel32", CharSet = CharSet.Unicode, ExactSpelling = false, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);


        [DllImport("starvr_api", CharSet = CharSet.Auto)]
        private static extern StarVRError StarVR_Driver_IsInit();

        [DllImport("starvr_api", CharSet = CharSet.Auto)]
        private static extern StarVRError StarVR_EyeTracking_IsInit(out bool isInit);

        [DllImport("starvr_api", CharSet = CharSet.Auto)]
        private static extern StarVRError StarVR_EyeTracking_GetLeftEyePose(out StarVREyePose eyePose);

        [DllImport("starvr_api", CharSet = CharSet.Auto)]
        private static extern StarVRError StarVR_EyeTracking_GetRightEyePose(out StarVREyePose eyePose);
        #endregion

    }
}
