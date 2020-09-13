
## What gets deployed?

- Custom Vision authoring resource
- Custom Vision prediction resource
- Web app
- App service plan

## How?

1. Open the Infrastructure directory in a Powershell 6 window
2. Run the following commands:

Login to Azure:

`az login`


Set the subscription to work against:

`az account set --subscription "<SUBSCRIPTION_NAME>"`


Run the deployment script:

`./deploy.ps1 -resourceGroup "<RESOURCE_GROUP_NAME>" -location "<RESOURCE_GROUP_LOCATION>" -templateFile "<FULL_PATH_TO>\video-analytics\src\VideoAnalytics\VideoAnalytics.Infrastructure\templates\arm-template.json" -parametersFile "<FULL_PATH_TO>\video-analytics\src\VideoAnalytics\VideoAnalytics.Infrastructure\parameters\parameters.DEV.json"`


For example, from the 'D:\Dev\video-analytics\src\VideoAnalytics\VideoAnalytics.Infrastructure' directory:

`./deploy.ps1 -resourceGroup "video-analytics-test" -location "australiaeast" -templateFile "D:\Dev\video-analytics\src\VideoAnalytics\VideoAnalytics.Infrastructure\templates\arm-template.json" -parametersFile "D:\Dev\video-analytics\src\VideoAnalytics\VideoAnalytics.Infrastructure\parameters\parameters.DEV.json"`


