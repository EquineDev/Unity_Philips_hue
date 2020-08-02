# Unity_Philips_hue
Philips Hue Lighting Integration for Unity 2019.4.X

Require Philips Hue Hub

# Setup
Configuration Setup

* Open the the Example Scene Or Drag the Hue Bridge prefab in the prefab folder into your scene
* In the HostName enter your IP address of your Hue Hub.
* Press the link button on the  Hue Hub then click on create new user (Note: Hue Hub Bridge API 1.31 removed API calls to remove users from whitelist, you need to go to the  https://account.meethue.com/apps to remove users now)
* Click on Discovery Lights to find all lights connect to  Hue Hub
* Click on Save Config to save  Hue Hub configuration to a Json file in StreamingAssets folder.

# Hue Bridge Properties

* HostName - IP address of Hue Hub
* Username - User Name for Hue Hub
* Instance - Get Singleton instance of  Hue Hub bridge

# Hue Bridge Methods

* GetLight - Takes in string of the name of the hue light in the Hue Hub bridge.
* DeleteLight - Takes in as tring of the name of the hue light in the Hue Hub bridge to be deleted
* SetupNewUser - Setups new user for Hue Hub bridge
* Config -  Takes in string of user and host to be configured
* DiscoverLights - Finds all lights connected to Hue Hub bridge

# Hue Lamp Properties

* UseLight - Use this light or not
* Username - User Name for Hue Hub
* Instance - Get Singleton instance of  Hue Hub bridge

# Hue Lamp Methods

* Setup - Takes in string of the path and key to setup hue light.
* FlashToAColor - Takes in a color, speed in float of flash and in of number of flashes
* SetBrightness - Takes in a int of brightness to set the brightness of the hue light
* SetColor -  Takes in a color to be set to the color of the hue light
* TurnOnOffLight - Takes in a bool to toggle the the hue light

# 3rd Party 
The following third party licenses are used 

* MiniJson Copyright (c) 2013 Calvin Rien, MIT license, see(https://gist.github.com/darktable/1411710)
* unity-hue Copyright (c) 2015 Marc Teyssier, MIT license, see(https://github.com/marcteys/unity-hue)
* All product names, logos, and brands are property of their respective owners. All company, product and service names used are for identification purposes only. 
