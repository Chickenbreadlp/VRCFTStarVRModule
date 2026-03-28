using System.Runtime.InteropServices;

namespace VRCFTStarVRModule
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VRVector3
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public float[] v;  // <- x, y, z in that order
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct VRVector2
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)] public float[] v;  // <- x, y in that order
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct StarVREyePose
    {
        public VRVector3 gazeOrigin;                // <- start of gaze ray
        public VRVector3 gazeDirectionNormalized;   // <- end of gaze ray
        public VRVector2 pupilCenter;               // <- x/y of eye relative to lens center?
        public float pupilDiameter;                 // <- likely passed through Tobii pupil diameter; could be mm?
        public bool isOpen;                         // <- boolean blink
        public bool isGazeOriginValid;              // <- Probably always false?
        public bool isGazeDirectionValid;           // <- Seemingly always false
    }

    public enum StarVRError
    {
        StarVRError_Failed = -1,
        StarVRError_None = 0,
        StarVRError_Unknown = 1,

        StarVRError_DriverNotInit = 2,
        StarVRError_OutOfBounds = 3,

        StarVRError_TobiiServiceNotFound = 4,
        StarVRError_TobiiVersionMismatch = 5,

        StarVRError_EyeTrackingNotInit = 6,
        StarVRError_EyeTrackerDeviceNotFound = 7,

        StarVRError_SubprocessAlreadyRunning = 8,

        StarVRError_TextureTypeNotSupported = 9,
        StarVRError_TextureAlreadySubmitted = 10
    };

    public enum EVREye // <- defined in headers, but seemingly not used?
    {
        Eye_Left = 0,
        Eye_Right = 1,
        Eye_Left_Side = 2,
        Eye_Right_Side = 3
    };
}
