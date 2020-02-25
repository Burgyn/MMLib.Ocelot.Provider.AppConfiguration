# MMLib.Ocelot.Provider.AppConfiguration

![logo](/src/MMLib.Ocelot.Provider.AppConfiguration/icon.png)

***AppConfiguration*** provider brings the possibility to divide the routing configuration from the service address definition.

# Get Started

1. Define routing configuration

```json
"ReRoutes": [
    {
      "DownstreamPathTemplate": "/api/users",
      "ServiceName": "authorization",
      "UpstreamPathTemplate": "/organizations/{organizationId}/users",
      "SwaggerKey": "UsersKey"
    },
    {
      "DownstreamPathTemplate": "/api/users/{everything}",
      "ServiceName": "authorization",
      "UpstreamPathTemplate": "/organizations/{organizationId}/users/{everything}",
      "SwaggerKey": "PermissionsKey"
    },
    {
      "DownstreamPathTemplate": "/api/permissions",
      "ServiceName": "authorization",
      "UpstreamPathTemplate": "/organizations/{organizationId}/permissions",
      "SwaggerKey": "PermissionsKey"
    },
    {
      "DownstreamPathTemplate": "/api/permissions/{everything}",
      "ServiceName": "authorization",
      "UpstreamPathTemplate": "/organizations/{organizationId}/permissions/{everything}",
      "SwaggerKey": "PermissionsKey"
    },
    {
      "DownstreamPathTemplate": "/api/todos",
      "ServiceName": "toDos",
      "UpstreamPathTemplate": "/organizations/{organizationId}/todos",
      "SwaggerKey": "TodosKey"
    },
    {
      "DownstreamPathTemplate": "/api/todos/{everything}",
      "ServiceName": "toDos",
      "UpstreamPathTemplate": "/organizations/{organizationId}/todos/{everything}",
      "SwaggerKey": "TodosKey"
    },
    {
      "DownstreamPathTemplate": "/api/organizations",
      "ServiceName": "organizations",
      "UpstreamPathTemplate": "/organizations",
      "SwaggerKey": "OrganizationsKey"
    },
    {
      "DownstreamPathTemplate": "/api/organizations/{everything}",
      "ServiceName": "organizations",
      "UpstreamPathTemplate": "/organizations/{everything}",
      "SwaggerKey": "OrganizationsKey"
    }
  ]
```

3. Define services addresses in `appsettings.json`

```json
"Services": {
  "organizations": {
    "DownstreamPath": "http://localhost:9003"
  },
  "authorization": {
    "DownstreamPath": "http://localhost:9002"
  },
  "toDos": {
    "DownstreamPath": "http://localhost:9001"
  }
}
```

4. Define Service Discovery provider

```json
"GlobalConfiguration": {
    "ServiceDiscoveryProvider": {
        "Type": "AppConfiguration",
        "PollingIntervalSeconds":  10000
    }
}
```

3. Add AppConfiguration provider in `Startup.cs`

> dotnet add package MMLib.Ocelot.Provider.AppConfiguration

```CSharp
services.AddOcelot()
    .AddAppConfiguration();
```

## Polling

This provider from optimization reason cache services 5 minutes by default. If you want change or turn off this caching, then set `PollingInterval` in provider definition (miliseconds).

## Change services definition section name

If you want change default section name, than you can set property `AppConfigurationSectionName` in provider definition.

```json
"GlobalConfiguration": {
    "ServiceDiscoveryProvider": {
        "Type": "AppConfiguration",
        "PollingIntervalSeconds":  10000,
        "AppConfigurationSectionName": "ApiServices"
    }
}
