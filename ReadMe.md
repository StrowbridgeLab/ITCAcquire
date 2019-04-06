# ITCAcquire Data Acquisition Program

## Description

This is a program written by Ben Strowbridge (bens@case.edu) in Visual Basic using Visual Studio 2010 on a PC running Windows 7 x64. This program interfaces with the ITC-18 data acquisition device originally manufactured by Instrutech (then Heka which is now (as of 4/2019) part of Harvard Biosciences). While the original ITC-18 interface is obsolete, it is possible that related devices are still being used inside other Heka-designed devices. 

The goal of ITCAcquire program is to operate one or more intracellular amplifiers while performing electrophysiological neuroscience experiments. The calibration factors included within the source code refer primarily to the AxoPatch 1D amplifier originally manufactured by Axon Instruments but now also obsolete. The ITCAcquire program encapsulates the hardware-specific operations required to run these electrophysiological experiments from other, software-only programs that setup stimulus parameters and display the acquired data. (Those ancillary programs are not included within the source code folder.) The ITCAcquire program "listens" for Windows messages from these other programs via the XDMessaging library created by TheCodeKing (https://thecodeking.co.uk/project/xdmessaging/). The interface for communicating acquisition parameters to the ITCAquire program and the format of the binary data files are defined in the class libraries in the source code folder.

While the source code provided is functional, it is not recommended for general use. Besides the target ITC-18 device being obsolete, the program requires several libraries that were available for Windows 7 but may not work under newer versions of Windows. Also many critical parameters (including file locations) are hard-coded within the program for our standardized computer configuration.

## Dependencies

XDMessaging.dll (Inter-process communication library)
ITCMM64.dll (low-level ITC18 library provided by Instrutech)
ZedGraph (used to create graphs during seal test runs)

## Contact Information

Ben W. Strowbridge 
Dept. of Neurosciences 
Case Western Reserve University 
Cleveland, Ohio 44106 
bens@case.edu

## License

This work is released under the MIT License.
