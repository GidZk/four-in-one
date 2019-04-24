# Bachelors's Thesis - Four in one
This is a bachelor thesis project made by students at Chalmers univerity of technology and University of Gothenburg.
This game was made in joint with the department of Information technologies and Interaction design.
## Submarine adventure / Ubåtsäventyret
The goal of this project is to design and implement a game where communication and cooperation is encouraged and trained.
The game consists of four tablets put together into one gaming field, the players will interact with a submarine in order to collect valuables.

The players have  control over different mechanisms of the submarine.
* Player 1: Steer the submarine along the **y-axis (UP,DOWN)**
* Player 2: Steer the submarine aling the **x-axis (LEFT,RIGHT)**
* Player 3: Aim a hook in order to shoot collecatbles.
* Player 4: Trigger shots and reel in the hook.

#### Current status of the project
Here are some screenshots of the current status of the project
<p align="center">
  <img src="https://github.com/GidZk/four-in-one/blob/master/media/puzzle%20screenshot%201.jpg" width="350" heigth ="450" title="Puzzle scene">
  <img src="https://github.com/GidZk/four-in-one/blob/master/media/gameplay%20screenshot%201.jpg" width="340" heigth ="400" title="Gameplay scene">
</p>

We are to test this with users one more time (friday 26/04), and some modifications will be made for the final prototype/ product.


## Built with
* [Unity](https://unity.com/) **v.2018.3.x**.
* [Unet Networking API](https://docs.unity3d.com/Manual/UNet.html):
* [Unet High Level API(HLAPI)](https://docs.unity3d.com/Manual/UNetUsingHLAPI.html)
* [Low Level API(LLAPI)](https://docs.unity3d.com/Manual/UnityWebRequest-LLAPI.html)
* [C#](https://docs.microsoft.com/en-us/dotnet/csharp/) as programming language.

#### IMPORTANT: UNET Networking information

In order to connect the tablets we have set up the networking communication with networking API's built inot UNet. Although  **the HLAPI and LLAPI _is-to-be-removed_ in future versions of Unity** so if you want to do further development of this game, the latest version of Unity to use is _2018.4.x_. 
The other option is to rewrite the network communication into the networking API being launched by unity.

More information on deprecation of Unity on [UNet deprecation page](https://support.unity3d.com/hc/en-us/articles/360001252086-UNet-Deprecation-FAQ)

