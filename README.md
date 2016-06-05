# Real-Time Arousal Detection Using Galvanic Skin Response Asset

## Overview

Galvanic Skin Response (GSR), also referred to Electro-Dermal Activity (EDA), Skin Conductance Response (SCR), Psycho-Galvanic Reflex (PGR), or Skin Conductance Level (SCL), is related to the activity of the sweat glands, which are regulated by the sympathetic nervous system. When being open and functioning intensively, they emit water solution (sweat) which creates channels of higher conductivity toward the deeper skin layers. EDA represents the electrical conductivity of the skin, which is directly dependent on the activity of the sweat glands, and is often used to index the autonomic arousal. GSR offers a popular and affordable way for detection of player’s arousal in adaptive digital games and other affective computing applications.
The asset produces real-time features of GSR signal measured from particular player such as: mean tonic activity level, phasic activity represented by mean and maximum amplitude of skin conductance response (all in micro-siemens), rate of phasic activity (response peaks/sec), SCR rise time, SCR 1/2 recovery time, and slope of tonic activity (in micro-siemens/sec). The level of arousal may be useful for emotion detection and for adaptation purposes.
The asset will receive a filtered raw signal from a simple, low cost biofeedback device allowing sampling rate up to 2Khz. Measurements are carried out with two electrodes placed on two adjacent fingers. Recording, filtering and feature extraction might be executed on a computer (server) different than the game machine, in order to speed up all the required processing. The results will be communicated from the server-side to the client component in order to be used for game adaptation.

## Asset architecture and input/output

Before starting with GSR measuring and arousal recognition, asset setup parameters should be initialised (client side). Such parameters include:
- required sample rate [Hz] – the frequency of measuring the GSR signal
- time window [s] – the length of time for calculating the moving average of arousal
- calibration period [s] – time for baseline calibration

The required sample rate [Hz] will be less than 2KHz. Next, the client-side will expect receiving request for the indexed arousal level of the player and will reply to them in real time. As well, he asset will be able to return values of mean tonic activity level, phasic activity represented by mean and maximum amplitude of skin conductance response (all in micro-siemens), rate of phasic activity (response peaks/sec), SCR rise time, SCR 1/2 recovery time, and slope of tonic activity (in micro-siemens/sec).
Due to the intensive computing processing required for filtering and logging of the raw signal, as far as calculation of signal features, the asset is to be developed with a server and client side components communicating each other via sockets. Thus, the server component will be able to reside at a computer different than the one running the game.

## Code structure

- /RealTimeArousalDetectionUsingGSR - contains projects responsible for calculating emotional arousal, filtering and displaying GSR signal and socket communication with the asset
 - /DisplayGSRSignal - contains code for displaying GSR signal and the asset's configuration file;
 - /Logger - contains code for logging;
 - /ReInitializeEDAValues - console application for re-initializing current EDA parameters from the property file;
  - /RealTimeArousalDetection - contains code for filtering and calculating emotional arousal;
  - /SocketServer - contains code for socket communication with the asset.
- /SignalDevice - contains code for communication with the GSR asset.

## Deployment and usage instructions

### Instalation and usage

The asset can be used as library without graphical representation of the GSR signal or as Window application. In both cases for installation and usage the following steps are needed:

- download or clone the repository;
- open project in your .Net IDE (Visual Studio, SharpDevelop, MonoDevelop, etc.);
- check application settings in the project [DisplayGSRSignal](https://github.com/ddessy/RealTimeArousalDetectionUsingGSR/tree/master/RealTimeArousalDetectionUsingGSR/DisplayGSRSignal) and change if it is needed;
- plug the GSR device;
- build the solution and run DisplayGSRSignal.

###  Application settings

The application settings are following:

- LogFile - path to the log file;
- MinAbsoluteArousalArea - the minimum arousal area achieved by all users until the moment;
- MinAverageArousalArea - the average minimum arousal area achieved by all users until the moment;
- MaxAbsoluteArousalArea - the maximum arousal area achieved by all users until the moment;
- MaxAverageArousalArea - the average maximum arousal area achieved by all users until the moment;
- MinAbsoluteTonicAmplitude - the minimum SCL amplitude achieved by all users until the moment;
- MinAverageTonicAmplitude - the average minimum SCL amplitude achieved by all users until the moment;
- MaxAbsoluteTonicAmplitude - the maximum SCL amplitude achieved by all users until the moment;
- MaxAverageTonicAmplitude - the average maximum SCL amplitude achieved by all users until the moment;
- NumberParticipants - number of users until the moment;
- CalibrationMinArousalArea - the calibration minimum arousal area achieved by the current user;
- CalibrationMaxArousalArea - the calibration maximum arousal area achieved by the current user;
- CalibrationMinTonicAmplitude - the calibration minimum SCL amplitude achieved by the current user;
- CalibrationMaxTonicAmplitude - the calibration minimum SCL amplitude achieved by the current user;
- MinArousalArea - the minimum arousal area achieved by the current user;
- MaxArousalArea - the maximum arousal area achieved by the current user;
- MinTonicAmplitude - the minimum SCL amplitude achieved by the current user;
- MaxTonicAmplitude - the maximum SCL amplitude achieved by the current user;
- DefaultTimeWindow - default value for the time window;
- SamplerateLabel - sample rate of the GSR device;
- ArousalLevel - number of arousal levels.

### Socket communication

The measured and calculated from the asset emotional arousal status of the current gamer/user can be access by a socket client. For this purpose following messages are expected:

- EOCP - this is the message for end of calibration period. After this message the calibration settings (CalibrationMinArousalArea, CalibrationMaxArousalArea, CalibrationMinTonicAmplitude and CalibrationMaxTonicAmplitude) are calculated (for tha last time window) and saved.
- GET_EDA - when the asset receives "GET_EDA" it returns a json file with information for the emotional arousal level of the gamer/user (in the last time window);
- EOM - this is the command for end of measurement for the current gamer/user. After this message the statistical values for the SCR and SCL arousal (MinAbsoluteArousalArea, MinAverageArousalArea, MaxAbsoluteArousalArea, MaxAverageArousalArea, MinAbsoluteTonicAmplitude, MinAverageTonicAmplitude, MaxAbsoluteTonicAmplitude, MaxAverageTonicAmplitude and NumberParticipants) are updated.

### Example of a JSON object returned by the asset

```sh
{
   "SCRArousalArea":770.88437500000009,
   "SCRAmplitude":{
      "Minimum":0.0010000000000001119,
      "Maximum":1.283,
      "Mean":0.428,
      "StdDeviation":0.604576435752062,
      "Count":0.375,
      "Name":"Amplitude"
   },
   "SCRRise":{
      "Minimum":50,
      "Maximum":5300,
      "Mean":1350,
      "StdDeviation":6.25,
      "Count":0.5,
      "Name":"Rise time"
   },
   "SCRRecoveryTime":{
      "Minimum":25,
      "Maximum":25,
      "Mean":18.75,
      "StdDeviation":0,
      "Count":0.5,
      "Name":"Recovery time"
   },
   "SCRAchievedArousalLevel":2,
   "TonicStatistics":{
      "Slope":0,
      "MeanAmp":0,
      "MinAmp":0,
      "MaxAmp":2.266,
      "StdDeviation":1.133
   },
   "SCLAchievedArousalLevel":1,
   "MovingAverage: 0.76491874999999976
}
```
