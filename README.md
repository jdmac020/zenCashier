# zenCashier
Implementation of a kata provided by Pillar Technology

To Run Unit Tests in Windows:
------
1. Click on the file labeled "UnitTestPackage.zip"
2. Download this file using the button on the top right
3. Go to whatever location your downloaded files end up via the File Explorer, right-click on the UnitTestPackage.zip folder, and select "Extract All" to a directory of your choice

   (For instance, "C:\Users\Cindy\Desktop\UnitTestPackage")

4. Open the destination folder when the files have finished extracting
5. Using the File menu in the File Explorer Window, open Powershell

   (If Powershell is grayed out, double check that files have finished extracting and that you have the destination folder open. If you need to open Powershell via the Start menu, you can just use the "CD" command to switch to the correct folder with all the extracted files)

6. Verify that Powershell's directory matches the one where all the dll files extracted to

   (If we're in keeping with the example above, you would see "PS C:\Users\Cindy\Desktop\UnitTestPackage>")
   
7. Copy the command below. (It assumes your Powershell directory has all DLL files and the xunit.runner.console folder). 

   xunit.runner.console.2.4.1\tools\net461\xunit.console ZenCashier.dll

8. Paste into the Powershell window
9. Hit Enter and watch the magic!
