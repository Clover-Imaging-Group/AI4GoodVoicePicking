# LUIS Dispatcher
## Powershell script
``` powershell
$dispatherDirectory = "C:\LuisDispatcher"

$Ai4GoodLuisDispatcherName = "Ai4Good-LUIS-Dispacther"                           # This can be whatever you want, keeping it consistant with the other names is highly recommended.

$Ai4GoodLuisExampleAppId                = "f506091f-58d4-48d4-93ac-c158075725e6" # 
$Ai4GoodLuisExampleName                 = "Ai4Good-LUIS-Example-1"               # 
$Ai4GoodLuisExampleVersion              = 0.1
$Ai4GoodLuisAuthoringKey1               = "a8d47e3a932244cba27b615cc3dda9d9"     # Found under Ai4Good-LUIS-Authoring -> Keys and End Point -> Show Keys -> copy and paste KEY 1
$Ai4GoodLuisLocation                    = "westus"                               # Found under Ai4Good-LUIS-Authoring -> Keys and End Point -> Show Keys -> copy and paste LOCATION

$Ai4GoodQnaExampleAppId = "172503cc-b834-4d75-ac8f-9df8948198a4"                 # qnamaker.ai - Settings - 
$Ai4GoodQnaExampleName  = "Ai4Good-KB-Example-1"                                 # qnamaker.ai - My Knowledge Bases - Knowledge base name, NOTE: Look for the azure service name as you'll need to look it up in the azure portal for the next field.
$Ai4GoodQnaKey1         = "e55199257a97485b9e4c569fbfc20896"                     # portal.azure.com - Ai4Good-QnaMaker -> Keys and Endpoint -> KEY1


New-Item -ItemType Directory -Force -Path $dispatherDirectory
Set-Location $dispatherDirectory
dir

#npm i -g npm
#npm i -g botdispatch

#dispatch init -n "oosAi4Good-LUIS-Dispatcher" --luisAuthoringKey "a8d47e3a932244cba27b615cc3dda9d9" --luisAuthoringRegion westus
dispatch init -n $Ai4GoodLuisDispatcherName --luisAuthoringKey $Ai4GoodLuisAuthoringKey1 --luisAuthoringRegion $Ai4GoodLuisLocation

#dispatch add -t luis -i "f506091f-58d4-48d4-93ac-c158075725e6" -n "oosAi4Good-LUIS-Example-1" -v 0.1 -k "a8d47e3a932244cba27b615cc3dda9d9" --intentName "oosAi4Good-Luis-Example-1"
dispatch add -t luis -i $Ai4GoodLuisExampleAppId -n $Ai4GoodLuisExampleName -v $Ai4GoodLuisExampleVersion -k $Ai4GoodLuisAuthoringKey1 --intentName $Ai4GoodLuisExampleName

#dispatch add -t qna -i "172503cc-b834-4d75-ac8f-9df8948198a4" -n "oosAi4Good-KB-Example-1" -k "e55199257a97485b9e4c569fbfc20896" --intentName "oosAi4Good-KB-Example-1"
dispatch add -t qna -i $Ai4GoodQnaExampleAppId -n $Ai4GoodQnaExampleName -k $Ai4GoodQnaKey1 --intentName $Ai4GoodQnaExampleName

dispatch create
```