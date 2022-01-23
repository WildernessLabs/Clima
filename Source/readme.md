# Clima Application Source

This folder contains all of the Clima application source code.

In the [Clima](Clima) folder is a `clima.sln` file with the following projects in it:

* **WildernessLabs.Clima.Contracts** - Shared project with the data models that are shared amongs the various projects.
* **WildernessLabs.Clima.Meadow.Pro** - A Meadow application to use with the Clima Pro climate station hardware.
* **WildernessLabs.Clima.Meadow.HackKit** - Meadow application for the Hack Kit version of Clima.
* **WildernessLabs.Clima.Server.Mini.Tests** - A console application used to test the Mini server.
* **WildernessLabs.Clima.Server.Mini** - A very basic local test Web API server written in ASP.NET Core.

## Local Server Web API

The mini Clima server stores `ClimateReading` objects in memory will populate itself with a sample reading on start so there is always some dummy data in there.

### Getting a List of Climate Readings

**Get** - http://localhost:[port]/ClimateData

### Posting a new Reading

**Post** - http://localhost:[port]/ClimateData

Sample body payload:

```json
{
    "timeOfReading": "2020-09-23T17:43:00-07:00",
    "tempC": 23,
    "barometricPressureMillibarHg": 250,
    "relativeHumdity": 0.7
}
```
