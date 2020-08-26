#LUIS Configuration

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
1. Click "New app for conversation"
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_05.png)
1. Create new app
	1. Give it a name
	1. Select English (this is what we have tested in, I dont know how well this will behave in other languages. We could use some assistance in globalization.)
	1. Give it a descrption.
	1. Select the LUIS resource created in setup.
	1. Click Done
1. Close the splash popup.
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_06.png)
1. Your screen should resemble this image:
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_07.png)
1. Click Create
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_08.png)
1. Create new intent
	1. Enter GreetUser
	1. Click Done
1. You will see a screen similar to this image:
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_09.png)
1. Use the textbox to enter these GreetUser examples:
	1. how are you doing?
	1. good afternoon
	1. good evening
	1. good morning
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_10.png)
1. Create new intent
	1. Enter FarewellUser
	1. Click Done
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_11.png)
1. Use the textbox to enter these FarewellUSer examples:
	1. hasta la vista
	1. good night
	1. farewall
	1. good by
	1. adios
	1. cao
	1. see ya
	1. goodbye
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_12.png)
1. Click the Train button.
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_13.png)
1. Click the Test button.
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_14.png)
1. Type in a few things and see if LUIS provides the expected intent.
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_15.png)
1. Click the Publish button.
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_14.png)
	1. Select Production Slot
	1. Click Done
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_16.png)
1. Wait for end points to be published.
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_17.png)
1. Click on Manage
1. Click on "Azure Resources" in the left menu.
1. Verify that you have something similar to this image:
![Resource Group Image 1](https://github.com/Clover-Imaging-Group/AI4GoodVoicePicking/blob/master/media/images/Luis_Ai/Step_18.png)