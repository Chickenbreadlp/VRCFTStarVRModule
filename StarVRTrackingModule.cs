using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using VRCFaceTracking;
using VRCFaceTracking.Core.Library;
using VRCFaceTracking.Core.Params.Data;
using VRCFaceTracking.Core.Types;

namespace VRCFTStarVRModule
{
   
    // This class contains the overrides for any VRCFT Tracking Data struct functions
    public static class TrackingData
    {
        private static SmoothFloat leftLid = new SmoothFloat();
        private static SmoothFloat rightLid = new SmoothFloat();

        /// <summary>
        /// Function to map a StarVR Vectror3 Gaze Ray to a Vector2 Ray for use in VRCFT
        /// </summary>
        /// <param name="starVRGaze"></param>
        /// <returns></returns>
        private static Vector2 GetGazeVector(VRVector3 starVRGaze)
        {
            return new Vector2((float)starVRGaze.v[0], (float)starVRGaze.v[1]);
        }

        /// <summary>
        /// Main Update function
        /// Mapps StarVR Eye Data to VRCFT Parameters
        /// </summary>
        /// <param name="data">Primary VRCFT gaza data object</param>
        /// <param name="expressionData">VRCFT expression data object</param>
        /// <param name="leftEye">Left Eye data</param>
        /// <param name="rightEye">Right Eye data</param>
        public static void Update(ref UnifiedEyeData data, ref UnifiedExpressionShape[] expressionData, StarVREyePose leftEye, StarVREyePose rightEye)
        {
            // Set the Gaze for each eye
            // not sure if pupil diameter is in mm or generic 0-1, but it's not gonna matter on most avis
            data.Left.Gaze = GetGazeVector(leftEye.gazeDirectionNormalized);
            data.Left.PupilDiameter_MM = leftEye.pupilDiameter;
            data.Right.Gaze = GetGazeVector(rightEye.gazeDirectionNormalized);
            data.Right.PupilDiameter_MM = rightEye.pupilDiameter;

            // Set eye openness
            leftLid.Value = leftEye.isOpen ? 1f : 0f;
            rightLid.Value = rightEye.isOpen ? 1f : 0f;
            data.Left.Openness = leftLid.Value;
            data.Right.Openness = rightLid.Value;
        }
    }
    
    public class StarVRTrackingModule : ExtTrackingModule 
    {
        private static StarVRNativeInterface tracker;

        // Mark this module as only supporting Eye Tracking
        public override (bool SupportsEye, bool SupportsExpression) Supported => (true, false);

        // Sets module display name and icon and loads the StarVR API
        public override (bool eyeSuccess, bool expressionSuccess) Initialize(bool eye, bool lip)
        {
            // Define Module details
            ModuleInformation.Name = "StarVR One Eye Tracking";
            var hmdIcon = GetType().Assembly.GetManifestResourceStream("VRCFTStarVRModule.Assets.starvrone.png");
            ModuleInformation.StaticImages = hmdIcon != null ? new List<Stream> { hmdIcon } : ModuleInformation.StaticImages;

            // Init the tracker class
            tracker = new StarVRNativeInterface();
            bool svrActive = tracker.Initialize(Logger);

            if (svrActive)
            {
                // only init float smoother when SVR is running!
                SmoothFloatWorkers.Init();
            }

            // Tell the lib manager our result with the init and that we (again) do not support Lip Tracking
            return (svrActive, false);
        }

        // Update function to be called in a while(true) loop. Keeping a delay is necessary to not fully load an entire CPU thread!
        public override void Update()
        {
            if (Status == ModuleState.Active)
            {
                // try and update data; log an error on failure and wait some extra time
                try
                {
                    if (tracker.Update())
                    {
                        TrackingData.Update(ref UnifiedTracking.Data.Eye, ref UnifiedTracking.Data.Shapes, tracker.GetLeftEye(), tracker.GetRightEye());
                    }
                    else
                    {
                        Logger.LogWarning("There seems to be an issue with getting Tracking data. Will try again in 1 second.");
                        Thread.Sleep(990); // wait a second before trying another update
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError("An exception was thrown while trying to load the next tracking values. Exception: " + e.Message);
                    Thread.Sleep(9990); // wait 10 seconds before trying another update
                }
            }

            // Sleep the thread for 10ms (= 100Hz tracking)
            Thread.Sleep(10);
        }

        // Function to be called when the module is torn down
        public override void Teardown()
        {
            tracker.Teardown();
        }
    }
}
