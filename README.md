# portable-friend
Source code from aeon generation one - Motorola Moto-Q Smartphone

### Prequisites to using the sources

This code was written in Visual Studio 2005/2008 with the Windows Mobile 6 Standard SDK. In order to be able to see the code work in this original context you will need a virtual machine running Windows XP SP3 (one processor and 8GB of RAM works well) and need to install the following, in this order:

* Visual Studio 2008 (You only need the C# and Device support - see figure)
* Visual Studio 2008 SP1 (the VM will restart on each step of VS installation)
* Active Sync 4.5
* Windows Mobile 6 Standard SDK
* Microsoft Device Emulator 2 & 3
* Windows Mobile 6 Professional Images

Once this is setup, open the Cartheur.Animals.Managed.sln file and it will open the solution. You will need to preload the contents of the Debug directory onto the device emulator for it to work.

Currently the database is too large for the emulator. It will work properly on a real device, if you could find or recreate one. However, I will optimize the database into a smaller footprint so that the application functions.