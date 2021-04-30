# Frends.Community.TCP

frends Community Task for TCPTasks

[![Actions Status](https://github.com/CommunityHiQ/Frends.Community.TCP/workflows/PackAndPushAfterMerge/badge.svg)](https://github.com/CommunityHiQ/Frends.Community.TCP/actions) ![MyGet](https://img.shields.io/myget/frends-community/v/Frends.Community.TCP) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) 

- [Installing](#installing)
- [Tasks](#tasks)
     - [ASCIIRequest](#ASCIIRequest)
- [Building](#building)
- [Contributing](#contributing)
- [Change Log](#change-log)

# Installing

You can install the Task via frends UI Task View or you can find the NuGet package from the following NuGet feed
https://www.myget.org/F/frends-community/api/v3/index.json and in Gallery view in MyGet https://www.myget.org/feed/frends-community/package/nuget/Frends.Community.TCP

# Tasks

## ASCIIRequest

Send one or more TCP/IP Ascii requests

### Parameters

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Commands | `string` | A command to send | `< GET MODEL >` |
| IpAddress | `string` | Host IP address | `127.0.0.1` |
| Port | `int` | Host port number | `13000` |

### Options

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Timeout | `int` | Reader timeout in milliseconds | `1000` |

### Returns

Responses in JArray

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Responses | `JArray` | Command response data | `{["< MODEL... >"]}` |

# Building

Clone a copy of the repository

`git clone https://github.com/CommunityHiQ/Frends.Community.TCP.git`

Rebuild the project

`dotnet build`

Run tests

`dotnet test`

Create a NuGet package

`dotnet pack --configuration Release`

# Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repository on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

# Change Log

| Version | Changes |
| ------- | ------- |
| 0.0.10   | Initial version |
| 0.0.11   | Return type changed to dynamic |
