# VRCFaceTracking StarVR module

The module provides gaze tracking data for VRCFT while using a StarVR One.  
This module was created using my fork of the [VRCFT Varjo module](https://github.com/Chickenbreadlp/VRCFTVarjoModule) as a basis.

## Installation

0. Download [VRCFaceTracking](https://github.com/benaclejames/VRCFaceTracking)
1. Download the latest version from [Releases](https://github.com/Chickenbreadlp/VRCFTStarVRModule/releases)
2. Import the downloaded Zip in VRCFT
3. Enjoy ^^

## Q & A

### How do I calibrate the Eye Tracking?

As the StarVR software (both Compass and [ReStar](https://github.com/Nyabsi/ReStar)) don't have a button to trigger the Eye Tracking Calibration, you'll have to use [this helper tool](https://github.com/Chickenbreadlp/StarVR_ET_Calib_Helper).

### Why no Module Registry?

I haven't been in the VRCFT Discord for ages now, and I don't plan on changing this just for this one silly module.

The amount of people who'll find this module useful is also very likely in the single or lower double-digits, so a distribution through manual installation likely won't be major issue.

## Credits
- @m3gagluk  
	- for making the initial strides in the [VRCFT Varjo module](https://github.com/m3gagluk/VRCFTVarjoModule) (on which this project is based upon)
- @Nyabsi  
	- for creating the float smoother (liberated from their [VRCFT Broken Eye module](https://github.com/Nyabsi/VRCFTBrokenEyeModule))
	- as well as creating [ReStar](https://github.com/Nyabsi/ReStar), whithout it this module would have made 0 sense to create (StarVR Compass does not deliver the necessary tracking data to use their headset in VRChat)
