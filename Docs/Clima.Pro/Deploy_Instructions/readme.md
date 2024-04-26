## Clima.Pro Application Deployment

### Step 1. - Prepare Dev Environment

 * **Download and install [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)** - For either Windows or macOS. Community edition will work fine.

 * **Follow the [Meadow.OS Deployment Guide](http://developer.wildernesslabs.co/Meadow/Getting_Started/Deploying_Meadow/)**:
   * The above guide should show you how to install `Meadow.CLI`
   * Walk your through out to download the latest Meadow.OS and flash it to the Clima Meadow board.

 * **Follow the [`Hello, Meadow` Guide](http://developer.wildernesslabs.co/Meadow/Getting_Started/Hello_World/)**:
   * The above guid should show  you to Install the Visual Studio and VSCode Extension(s) for Meadow.

### Expected Development version stack (RC-1)
At this point, you should have the following versions on your development machine
* Meadow.CLI - `v2.x`
* Visual Studio
  * MacOS - `v1.9.x`
  * Windows - `v1.9.x`
* VS Code - `v1.9.x`

* Meadow

And if you run this command line: `meadow device info`, you should see something similar to:
  ```
  Meadow by Wilderness Labs
  Board Information 
      Model: F7Micro
      Hardware version: F7CoreComputeV2
      Device name: MeadowF7

  Hardware Information 
      Processor type: STM32F777IIK6
      ID: [Specific ID for your Meadow]
      Serial number: [Specific Serial No. for your Meadow]
      Coprocessor type: ESP32
      MAC Address - 
    WiFi: [Specific Mac Address for your Meadow]

  Firmware Versions 
      OS: 1.10.0.2
      Runtime: 1.10.0.2
      Coprocessor: 1.10.0.1
      Protocol: 8
  ```

### Step 2. - Deploy the Clima App

 **Clone the Clima Repo**:
 
 [Clone](https://docs.github.com/en/desktop/contributing-and-collaborating-using-github-desktop/adding-and-cloning-repositories/cloning-and-forking-repositories-from-github-desktop) the [Clima(this) repository](https://github.com/wildernesslabs/Clima) locally.

 **Open the Clima VS Solution**:

 With either Visual Studio or VSCode running, navigate to the `source` folder and open the `Meadow.Clima.sln`. 

 **Add WiFi Credentials**:
 
 Edit the `secrets.cs` file to have the correct credentials for the WiFi your Clima will connect to.

 **Deploy the MeadowClimaProKit App**
