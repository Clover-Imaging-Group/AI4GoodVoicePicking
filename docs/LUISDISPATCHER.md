# LUIS Dispatcher

1. Login to [luis.ai](https://www.luis.ai/) it should use the same credentials as the Azure portal
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_01.png)
1. In the upper right hand corner click on the circle to open your profile.
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_02.png)
1. Select the Azure tenant that you want to use. This must be the same tenant that the LUIS resource was setup in.
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_03.png)
1. Click on My Apps
1. Select the subscription you want to use.
1. Select the Authoring Resource that was created.
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_04.png)
1. Click on the app name of the LUIS conversation that you want to include in the dispatcher.
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Dispatcher/Step_01.png)
1. Click on MANAGE
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Dispatcher/Step_02.png)
1. Copy the following pieces of information into the powershell script below: They have been numbered for simpler what-goes-where.
	1. App name       (#1)
	1. App Id         (#2)
	1. Version Number (#3)
1. Click on Azure Resources
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Dispatcher/Step_03.png)
1. Copy the following to the script below:
	1. Primary Key (#4)
	1. Location    (#5)
1. Log in to [www.qnamaker.ai](https://www.qnamaker.ai/) it should use the same credentials as the Azure portal.
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Qna_Maker/Step_01.png)
1. Click on My knowledge bases
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Dispatcher/Step_04.png)
1. Copy the following to the script below:
	1. Knowledge base name (#7)
	1. Azure service name (#8) NOTE: just make a note of this as we will be searching for it in the azure portal.
1. Click on the knowledge base name
1. Click on SETTINGS
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Dispatcher/Step_05.png)
1. Scroll down to the bottom.
1. Copy the following to the script below:
	1. 


## Powershell script
``` powershell
$dispatherDirectory = "C:\LuisDispatcher"

# This can be whatever you want, keeping it consistant with the other names is highly recommended.
$Ai4GoodLuisDispatcherName = "Ai4Good-LUIS-Dispacther"                           


# (#1) luis.ai - Copied from the "Manage" section for the LUIS conversation  - App Name.
$Ai4GoodLuisExampleName                 = "Ai4Good-LUIS-Example-1"   

# (#2) luis.ai - Copied from the "Manage" section for the LUIS conversation  - App Id.
$Ai4GoodLuisExampleAppId                = "cf0b8898-894e-433e-8862-2a3483898942" # 

# (#3) luis.ai - Copied from the "Manage" section for the LUIS conversation  - use the version number example 0.1
$Ai4GoodLuisExampleVersion              = 0.1

# (#4) luis.ai - Copied from the "Manage / Azure Resources / Authoring Resource" section for the LUIS conversation  - copy the "Primary Key"
$Ai4GoodLuisAuthoringKey1               = "cec2f491a4474a23964edfe808f5c2bd"     

# (#5) luis.ai - Copied from the "Manage / Azure Resources / Authoring Resource" section for the LUIS conversation  - copy the "Location"
$Ai4GoodLuisLocation                    = "westus"                               


# (#6) qnamaker.ai - Settings - POST /knowledgebases/{ copy guid from here }/generateAnswer
$Ai4GoodQnaExampleAppId = "bc5ebf3d-ed96-4256-9d23-5f70fae2a795"                 

# (#7) qnamaker.ai - My Knowledge Bases - Knowledge base name
# (#8) NOTE: Look for the azure service name as you'll need to look it up in the azure portal for the next field.
$Ai4GoodQnaExampleName  = "Ai4Good-KB-Example-1"                                 

# (#9) portal.azure.com - Ai4Good-QnaMaker -> Keys and Endpoint -> KEY1
$Ai4GoodQnaKey1         = "e55199257a97485b9e4c569fbfc20896"                     

# Create a temporary directory to hold our dispatcher creation information.
# this can be deleted and remade as many times as needed.
New-Item -ItemType Directory -Force -Path $dispatherDirectory
Set-Location $dispatherDirectory

# Only need to be ran once. May show errors
npm i -g npm
npm i -g botdispatch

# Initializing a new dispatcher file
dispatch init -n $Ai4GoodLuisDispatcherName --luisAuthoringKey $Ai4GoodLuisAuthoringKey1 --luisAuthoringRegion $Ai4GoodLuisLocation

# Adding LUIS example to dispatcher.
# Multiple LUIS examples can be added to the dispatcher (just follow the same format)
dispatch add -t luis -i $Ai4GoodLuisExampleAppId -n $Ai4GoodLuisExampleName -v $Ai4GoodLuisExampleVersion -k $Ai4GoodLuisAuthoringKey1 --intentName $Ai4GoodLuisExampleName

# Adding QnA Maker to dispatcher
# Mutiple QnA Maker projects can be added to the dispatcher.
dispatch add -t qna -i $Ai4GoodQnaExampleAppId -n $Ai4GoodQnaExampleName -k $Ai4GoodQnaKey1 --intentName $Ai4GoodQnaExampleName

# Create the dispatcher
dispatch create
```