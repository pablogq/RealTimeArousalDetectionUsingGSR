# Real-Time Arousal Detection Using Galvanic Skin Response Asset

## Overview

Galvanic Skin Response (GSR), also referred to Electro-Dermal Activity (EDA), Skin Conductance Response (SCR), Psycho-Galvanic Reflex (PGR), or Skin Conductance Level (SCL), is related to the activity of the sweat glands, which are regulated by the sympathetic nervous system. When being open and functioning intensively, they emit water solution (sweat) which creates channels of higher conductivity toward the deeper skin layers. EDA represents the electrical conductivity of the skin, which is directly dependent on the activity of the sweat glands, and is often used to index the autonomic arousal. GSR offers a popular and affordable way for detection of player’s arousal in adaptive digital games and other affective computing applications.
The asset produces real-time features of GSR signal measured from particular player such as: mean tonic activity level, phasic activity represented by mean and maximum amplitude of skin conductance response (all in micro-siemens), rate of phasic activity (response peaks/sec), SCR rise time, SCR 1/2 recovery time, and slope of tonic activity (in micro-siemens/sec). The level of arousal may be useful for emotion detection and for adaptation purposes, therefore the asset can be combined with the T2.3 Real-time Emotion Detection Asset and the T3.4 Player-centric rule- and pattern-based adaptation asset.
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

The asset can be used as library without graphical representation of the GSR signal or as Window application. In both cases for installation and usage the following steps are needed:

- download or clone the repository;
- open project in your .Net IDE (Visual Studio, SharpDevelop, MonoDevelop, etc.);
- check application settings in project [DisplayGSRSignal](https://github.com/ddessy/RealTimeArousalDetectionUsingGSR/tree/master/RealTimeArousalDetectionUsingGSR/DisplayGSRSignal) and change if it is needed;
- plug the GSR device;
- build and run application.
