# Monkey Speak Extended

## Description
MonkeySpeakEx is a dragonspeak-like interpreter written in C# 4.6. MonkeySpeakEx has a light footprint, less than 100kb size. Runtime memory usage is small making it very friendly in mobile environments.

## Basics
MonkeySpeakEx is a functional language. The Engine loads the script into a new Page that contains the Triggers. Triggers can be compared to functions since they are used to perform a function in MonkeySpeakEx. Triggers are assigned specific TriggerHandlers (delegates) that are executed whenever the engine detects that trigger. Only one TriggerHandler delegate is assigned per Trigger.

## Features

* Native supported data types (string, double) 
* Local variable scope per Page 
* Multiple conditional statements using (1:##) triggers 
* Default libraries Sys, IO, Math, Debug, Timers 
* Easy to extend by extending AbstractBaseLibrary or attaching TriggerHandlerAttribute to a method 
* Compilable scripts for faster loading or sending over the network 
* Thread safe, can execute multiple pages asynchronously. (beta) 
* EXE compiler (beta)

## Apache License
Version 2.0, January 2004
http://www.apache.org/licenses/

## Disclaimer
The project "MonkeySpeakEx" and it's development team is in no way affiliated with Dragon's Eye Productions or Catnip Studios. MonkeySpeakEx is free for life. Free as in FREE BEER!!
Important: MonkeySpeakEx's compiled file format is not compatible with Dragonspeak's compiled file format.
