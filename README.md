<img src="Image_Assets/clima-banner.jpg" style="margin-bottom:10px" />

Clima is a solar-powered, custom embedded-IoT solution that tracks climate from a suite of sensors, saves data locally for access via Bluetooth, uses a RESTful Web API, and synchronizes data to the cloud.

## Contents
* [Clima Pro](#clima)
* [Assembly Instructions](#assembly-instructions)
* [Getting Started](#getting-started)
* [Hardware Specifications](#hardware-specifications)
* [Support](#support)

## Clima

With this kit, it includes the complete package of sensors, PCB enclosure and mount to place this outdoors. You'll be able to measure wind speed/direction, rain volume, atmospheric conditions like temperature, pressure, humidity, CO2 levels and GPS Coordinates.

<img src="Image_Assets/ClimaPro.jpg" />

## Assembly Instructions

A complete kit of Clima.Pro can be found on the [Wilderness Labs Store](https://store.wildernesslabs.co/collections/frontpage/products/clima-weather-station-kit) and the Instructions for assembly can be found [here](/Docs/Clima.Pro/Assembly_Instructions/readme.md).

The store version is 100% kit complete, including the option to upgrade the PCB, Enclosure and Battery only, if you include a previous version of the kit.

You can also source all of the components yourself. For a list of components see the [Clima Pro Bill of Material (BoM)](/Docs/Clima.Pro/Bill_of_Materials.md)

## Getting Started

To simplify the way to use this Meadow-powered reference IoT product, we've created a NuGet package that instantiates and encapsulates the onboard hardware into a `Clima` class.

1. Add the ProjectLab Nuget package your project: 
    - `dotnet add package Meadow.Clima`, or
    - [Meadow.Clima Nuget Package](https://www.nuget.org/packages/Meadow.Clima/)

2. Change the App type on your MeadowApp class to `ClimaAppBase` and initialize Clima's `MainController` passing the `Hardware` and a `INetworkAdapter` such as your WiFi adapter onboard the Meadow Core Compute Module:  

```csharp
public class ClimaApp : ClimaAppBase
{
    public override Task Initialize()
    {
        Resolver.Log.Info($"Initialize...");

        var mainController = new MainController();

        var wifi = Hardware.ComputeModule.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

        mainController.Initialize(
            hardware: Hardware,
            networkAdapter: wifi);
            .
            .
            .
```

3. Run the [Clima_Demo](Source/Clima_Demo/) project that does periodic readings of all its sensors and sends them to [Meadow.Cloud](https://www.meadowcloud.co) if you have a Wilderness Labs account and have provisioned your device.

## Hardware Specifications

<img src="Image_Assets/wildernesslabs-clima-v3-specs.jpg" style="margin-top:10px;margin-bottom:10px" />

You can find the schematics and other design files in the [Hardware_Design folder](Hardware_Design/).

## Support

Having trouble building/running these projects? 
* File an [issue](https://github.com/WildernessLabs/Meadow.Desktop.Samples/issues) with a repro case to investigate, and/or
* Join our [public Slack](http://slackinvite.wildernesslabs.co/), where we have an awesome community helping, sharing and building amazing things using Meadow.