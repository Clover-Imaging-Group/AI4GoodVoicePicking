# Web App Setup
1. Login to your Azure Tenant [portal.azure.com](https://portal.azure.com)
1. Click on "Create a resource"
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Add_WebApp/Step_01.png)
1. Search for and click on "Web App"
![Resource Group Image 2](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Add_WebApp/Step_02.png)
1. Click Create
![Resource Group Image 3](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Add_WebApp/Step_03.png)
1. Project Details
	1. Select the subscription that you will be using to host the Web App.
	1. Select the resource group to house the face resource.
1. Instance Details
	1. Name the web app resource. We chose Ai4Good-WebApp
	1. Select "Code" for publish
	1. Select ".NET Core 3.1 (LTS)" for the runtime stack.
	1. Select an approate region reletave to your location.
1. App Service Plan
	1. We allowed the web app to create a new app service plan and kept the defaults.
1. Click on Review + create
![Resource Group Image 4](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Add_WebApp/Step_04.png)
1. Click on Create
![Resource Group Image 5](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Add_WebApp/Step_05.png)
1. Setup completed