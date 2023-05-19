# Digital Twin of the tello drone

This project contains the the technical Implementation of a digital twin of a tello drone.
This project is part of my bachelor thesis at the University of Applied Sciencs in Hamburg, Germany. 
The focus of my thesis is to design and develop a digital 
twin according to the paradigm of a multi-agent system.

## Requierments
In order to use the application at it full potential the Tello by Ryze is needed.

You need at least .Net 7.0 and C# 10.

NLog 5.1.4
Mars.Life.Simulation 4.5.2
MathNet.Numers 5.0.0
NUnit 3.13.3
SharpDX.DirectInput 4.2.0
Coorful.Console

## Step-By-Step Instruction
When you start the system eveything is already set up.

1. Start the drone and wait until the Led blinks an orange light
2. Start the application.
3. Press C to connect the application to the drone.
4. Fly the drone.

## Drone specification
Further information regarding the drone can be found [on this website](https://dl-cdn.ryzerobotics.com/downloads/tello/20180910/Tello%20SDK%20Documentation%20EN_1.3.pdf
).

Personally, I was using the specification for the [second version](https://dl-cdn.ryzerobotics.com/downloads/Tello/Tello%20SDK%202.0%20User%20Guide.pdf
) due to it's better readability but only the commands from the first Version are available.

## Keyboard Control

| Action                          | Key    |
|---------------------------------|--------|
| Forward                         | W      |
| Backward                        | S      |
| Left                            | A      |
| Right                           | D      |
| Stop                            | Space  |
| Rotate Clockwise                | E      |
| Rotate Counterclockwise         | Q      |
| Take off                        | T      |
| Land                            | L      |
| Emergency Stop                  | P      |
| Battery                         | B      |
| start Record-Repeat Navigation  | U      |
| Stop Record-Repeat Navigation   | I      |
| Stop Recording                  | Delete |
