# Bachelors's Thesis - Four in one
This is a bachelor thesis project made by students at Chalmers univerity of technology and University of Gothenburg.
This game was made in joint with the department of Information technologies and Interaction design.
## Submarine adventure / Ubåtsäventyret
The goal of this project is to design and implement a game where communication and cooperation is encouraged and trained.
The game consists of four tablets put together into one gaming field, the players will interact with a submarine in order to collect valuables.

The players have  control over different mechanisms of the submarine.
* **Player 1**: Steer the submarine along the **y-axis (UP,DOWN)**
* **Player 2**: Steer the submarine aling the **x-axis (LEFT,RIGHT)**
* **Player 3**: Aim a hook in order to shoot collecatbles.
* **Player 4**: Trigger shots and reel in the hook.

#### Current status of the project
Here are some screenshots of the current status of the project
<p align="center">
  <img src="https://github.com/GidZk/four-in-one/blob/master/media/puzzle%20screenshot%201.jpg" width="350" heigth ="450" title="Puzzle scene">
  <img src="https://github.com/GidZk/four-in-one/blob/master/media/gameplay%20screenshot%201.jpg" width="340" heigth ="400" title="Gameplay scene">
</p>

We are to test this with users one more time **(friday 26/04-19)**, and some modifications will be made for the final prototype/ product.


## Built with
* [Unity](https://unity.com/) **v.2018.3.x**.
* [Unet Networking API](https://docs.unity3d.com/Manual/UNet.html):
* [Unet High Level API(HLAPI)](https://docs.unity3d.com/Manual/UNetUsingHLAPI.html)
* [Low Level API(LLAPI)](https://docs.unity3d.com/Manual/UnityWebRequest-LLAPI.html)
* [C#](https://docs.microsoft.com/en-us/dotnet/csharp/) as programming language.

#### IMPORTANT: UNET Networking information

In order to connect the tablets we have set up the networking communication with networking API's built inot UNet. Although  **the HLAPI and LLAPI _is-to-be-removed_ in future versions of Unity** so for further development of this game, the latest version of Unity that is usable is **_v.2018.4.x_**. 

If further development is made with versions higher than v.2018.4.x, you will need to **migrate the networking code** to suit the to-be-launched HLAPI and LLAPI.

More information on deprecation of Unity on [UNet deprecation page](https://support.unity3d.com/hc/en-us/articles/360001252086-UNet-Deprecation-FAQ)
## Authors
* [Alice Gunnarsson](https://github.com/blackfisken)
* [Erik Lundin](https://github.com/erilundi)
* [John Lindström Gidskehaug](https://github.com/GidZk)
* [Linnea Elman](https://github.com/linneaelm)
* [Magnus Wamby](https://github.com/EmElw)



## Acknowledgements
Special thanks to our bachelors thesis supervisor **Olof Torgerssor** for helping us with litterature and support through out the project.


Other thanks to :
* **Peter Börjesson** - doctorial student (24/04-19) : For helping us with interview questions and support with designing games for children.
* **Förskolan Lokomotivet (Nursery school Lokomotivet)** : For letting us use their pupils as user testers.
