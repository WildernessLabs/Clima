<img src="Image_Assets/clima-banner.jpg" style="margin-bottom:10px" />

Clima is a solar-powered, custom embedded-IoT solution that tracks climate from a suite of sensors, saves data locally for access via Bluetooth, uses a RESTful Web API, and synchronizes data to the cloud.

## Clima Options

We offer clima in two options, a full dedicated kit that it's fully solar powered build and ideal to measure weather outdoors, or a much simplified version that you can build with our Hack Kits.

Both versions are 100% open source, including all of the enclosure design files, and PCB design of the pro version.

<table width="100%">
    <tr>
        <td width="50%">
            <strong><a href="https://store.wildernesslabs.co/collections/frontpage/products/clima-weather-station-kit">Pro Version</a></strong>
        </td>
        <td width="50%">
            <strong><a href="https://store.wildernesslabs.co/collections/frontpage/products/meadow-f7-micro-development-board-w-hack-kit-pro">Hack Kit Version</a></strong></td>
    </tr>
    <tr>
        <td>
            <img src="Image_Assets/ClimaPro.jpg" />
        </td>
        <td>
            <img src="Image_Assets/Clima.jpg" /> 
        </td>
    </tr>
    <tr>
        <td>
            With this kit, the sensors included are:
            <ul>
                <li>Anemometer to measure wind speed</li>
                <li>WindVane to check wind direction</li>
                <li>Rain meter to measure precipitation</li>
                <li>BME680 to measure ambient temperature, pressure and humidity </li>
            </ul>
        </td>
        <td> 
            With the Hack Kit, you can build this project to measure indoor room temperature with an analog temperature sensor, use a 240x240 TFT Spi display and three push buttons to build a simple UI using MicroGraphics to do things like change temperature units, and more.
        </td>
    </tr>
</table>

# Clima.Pro Kit 

## Getting Started

1) [Buy](https://store.wildernesslabs.co/collections/frontpage/products/clima-weather-station-kit) or [Source](/Docs/Clima.Pro/Bill_of_Materials.md) a kit.
2) [Assemble it](/Docs/Clima.Pro/Assembly_Instructions/readme.md).
3) [Build and Deploy the Meadow Clima.Pro App to it].
4) Optionally, build and deploy the [companion mobile app].

## Sourcing

A complete kit of the Pro version of Clima can be found on the [Wilderness Labs Store](https://store.wildernesslabs.co/collections/frontpage/products/clima-weather-station-kit).

The store version is 100% kit complete, including the option to include the 3D printed enclosure, and a meadow.

You can also source all of the components yourself. For a list of components see the [Clima Pro Bill of Material (BoM)](/Docs/Clima.Pro/Bill_of_Materials.md)
 
## Assembly

Instructions for assembly can be found [here](/Docs/Clima.Pro/Assembly_Instructions/readme.md).

## Known Issues

The Meadow.OS Power and Sleep APIs haven't been released yet, so Clima can't go to sleep to conserve power. For that reason, it'll need to be plugged into USB. We're hoping to have the first sleep APIs available in b6.2.

# Clima Hack Kit Version

Instructions on how to assemble the Clima Hack Kit Version can be found [here](/Docs/Clima.HackKit/readme.md)

# Clima Application Source

The source code for the Clima applications can be found in the [source](/Source) folder.

In there is a `clima.sln` file with the following projects in it:

* **WildernessLabs.Clima.Contracts** - Shared project with the data models that are shared amongs the various projects.
* **WildernessLabs.Clima.Meadow.Pro** - A Meadow application to use with the Clima Pro climate station hardware.
* **WildernessLabs.Clima.Meadow.HackKit** - Meadow application for the Hack Kit version of Clima.
* **WildernessLabs.Clima.Server.Mini.Tests** - A console application used to test the Mini server.
* **WildernessLabs.Clima.Server.Mini** - A very basic local test Web API server written in ASP.NET Core.

For more information on the application source code, please see the [source code readme](/Source/readme.md).