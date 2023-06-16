# Digital Twin of the tello drone

This project contains the the technical Implementation of a digital twin of a [Tello](https://www.ryzerobotics.com/de/tello).
This project is part of my bachelor thesis at the University of Applied Sciencs (HAW) in Hamburg, Germany. 
The main objective of my thesis is to design and develop a digital twin using the principles of a multi-agent system(MAS). A digital twin refers to the creation of a virtual counterpart that can represent various entities such as objects, individuals, production lines, or distributed systems. The fundamental idea behind this concept is to create an accurate and real-time representation of the internal state of the physical system. By doing so, we can gain valuable insights into the system's current status, enhance its efficiency, and anticipate potential failures by predicting its health condition. This approach not only enables effective system monitoring but also offers opportunities for continuous improvement.

This work consists of two main component. 
1. A user-friendly control interface allowing manual drone control through keyboard inputs for sending commands.
2. A digital twin serving as a virtual representation of the physical drone. The model actively receives flight information from the drone and accurately replicates its movements within a MAS simulation. This data is then recorded and stored within the system, accessible even after the termination of the simulation.

## Requierments
In order to use the application at it full potential the Tello is needed.

The application is developed .Net 7.0 and C# 10.

The following Nuget packages are needed:
- NLog 5.1.4 
- Mars.Life.Simulation 4.5.2 
- MathNet.Numers 5.0.0
- NUnit 3.13.3
- Colorful.Console

## Drone specification
Further information regarding the drone can be found [on this website](https://dl-cdn.ryzerobotics.com/downloads/tello/20180910/Tello%20SDK%20Documentation%20EN_1.3.pdf
).

Personally, I was using the specification for the [second version](https://dl-cdn.ryzerobotics.com/downloads/Tello/Tello%20SDK%202.0%20User%20Guide.pdf
) due to it's better readability but only the commands from the first Version are available.

## How to fly the drone manually? 

1. Press the power button on the drone and wait until the led blinks in an orange light
2. Start the application.
3. when the system is initilized press the C-Key to establish a connecton to the drone.
4. If connection is established, the drone led will blink green and the system is set up.
5. Fly the drone by using the commands listed [down below][## Keyboard Control]

## How to use the digital twin?
After following the first 3 step from the guide on how to manually fly the drone the digital twin will start receiving and mirroring the state of the drone, but it does not interfer at any of the command sent from the drone control. The digital twin can be used for autonomous operation. One simple operation is developed in this system and will be presented next.

## How to use the digital twin?

After following the first three steps in the guide on how to manually fly the drone, the digital twin will begin receiving and mirroring the drone's state without interfering with any commands sent from the drone controller. The digital twin can also be used for autonomous operations. One such operation, the Record-Repeat Navigation, is developed in this system and will be presented below.

## Record-Repeat Navigation
 
The Record-Repeat Navigation operation allows the user to record a trajectory by flying the drone and use the digital twin to replicate the recorded commands within the desired timeframe.
### How to use the operation:

1. Launch the application to start the operation.
2. Every keyboard input will be recorded during this phase, allowing the trajectory to be flown.
3. Press the "Stop recording" button to save all inputs to an external file in .csv format.
4. Locate the recorded file in the following directory: DtTelloDrone/OutputResources/RnrRecord.{current date}/Session.{system start time}/KeyboardInput.csv
5. Drag the file to the TestingResources/PlaybackNavigationDemos folder, which is located at the same path level as /RnrRecord.{current date}.
6. Navigate to the class DtTelloDrone/Model/Agent/TelloAgent and replace the string value of the variable DemoResourcesPath with the path to your recorded file.
7. Start the system and connect by pressing 'C'.
8. Press 'U' to begin the operation and execute the recorded commands.

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
