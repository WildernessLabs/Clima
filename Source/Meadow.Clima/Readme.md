# Meadow.Clima

**Library for the Meadow Clima IoT weather station accelerator**

The **Clima** library is included in the **Meadow.Clima** nuget package and is designed for the [Wilderness Labs](www.wildernesslabs.co) Meadow .NET IoT platform.

Clima is a solar-powered, custom embedded-IoT solution that tracks climate from a suite of sensors, saves data locally, and synchronizes telemetry to Meadow.Cloud.

For more information on developing for Meadow, visit [developer.wildernesslabs.co](http://developer.wildernesslabs.co/).

To view all Wilderness Labs open-source projects, including samples, visit [github.com/wildernesslabs](https://github.com/wildernesslabs/).

## Onboard Hardware

| Peripheral | Description |
|---|---|
| NEO-M8 | GNSS/GPS module |
| BME688 | Temperature, pressure, humidity, and gas resistance sensor |
| SCD40 | CO2, humidity, and temperature sensor |
| SwitchingAnemometer | Wind speed sensor |
| WindVane | Wind direction sensor |
| SwitchingRainGauge | Rain accumulation sensor |
| RGB LED | Onboard status indicator |
| Solar input | Solar charging with battery voltage monitoring |

## Installation

You can install the library from within Visual Studio using the NuGet Package Manager or from the command line using the .NET CLI:

`dotnet add package Meadow.Clima`

## Usage

```csharp
public class ClimaApp : ClimaAppBase
{
    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        var mainController = new MainController();

        var wifi = Hardware.ComputeModule.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

        mainController.Initialize(
            hardware: Hardware,
            networkAdapter: wifi);

        return base.Initialize();
    }

    public override Task Run()
    {
        return base.Run();
    }
}
```

For lower-level hardware access, you can use `ClimaHardwareProvider` directly:

```csharp
public class MeadowApp : App<F7CoreComputeV2>
{
    IClimaHardware clima;

    public override Task Initialize()
    {
        clima = ClimaHardwareProvider.Create();

        if (clima.TemperatureSensor is { } tempSensor)
        {
            tempSensor.Updated += (s, e) =>
                Resolver.Log.Info($"Temperature: {e.New.Celsius:N1}C");
        }

        if (clima.Anemometer is { } anemometer)
        {
            anemometer.Updated += (s, e) =>
                Resolver.Log.Info($"Wind speed: {e.New.KilometersPerHour:N1} km/h");
        }

        if (clima.Gnss is { } gnss)
        {
            gnss.GnssDataReceived += (s, e) =>
                Resolver.Log.Info($"GNSS data received");
        }

        return Task.CompletedTask;
    }

    public override async Task Run()
    {
        if (clima.TemperatureSensor is { } tempSensor)
            tempSensor.StartUpdating(TimeSpan.FromMinutes(1));

        if (clima.Anemometer is { } anemometer)
            anemometer.StartUpdating(TimeSpan.FromSeconds(15));

        if (clima.Gnss is { } gnss)
            gnss.StartUpdating();

        await Task.Delay(Timeout.Infinite);
    }
}
```

## How to Contribute

- **Found a bug?** [Report an issue](https://github.com/WildernessLabs/Meadow_Issues/issues)
- Have a **feature idea or driver request?** [Open a new feature request](https://github.com/WildernessLabs/Meadow_Issues/issues)
- Want to **contribute code?** Fork the [Clima](https://github.com/WildernessLabs/Clima) repository and submit a pull request against the `develop` branch

## Need Help?

If you have questions or need assistance, please join the Wilderness Labs [community on Slack](http://slackinvite.wildernesslabs.co/).

## About Meadow

Meadow is a complete, IoT platform with defense-grade security that runs full .NET applications on embeddable microcontrollers and Linux single-board computers including Raspberry Pi and NVIDIA Jetson.

### Build

Use the full .NET platform and tooling such as Visual Studio and plug-and-play hardware drivers to painlessly build IoT solutions.

### Connect

Utilize native support for WiFi, Ethernet, and Cellular connectivity to send sensor data to the Cloud and remotely control your peripherals.

### Deploy

Instantly deploy and manage your fleet in the cloud for OtA, health-monitoring, logs, command + control, and enterprise backend integrations.
