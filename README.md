# **CuteDancer**

_Animations, package: [Krysiek](https://github.com/Krysiek)  
Sender/Receiver config, support and tests: [Luc4r](https://github.com/Luc4r)  
Animators optimization, tests: [Jack'lul](https://github.com/jacklul)_

[🇵🇱 Język Polski - kliknij tutaj](/README.pl.md)

## About CuteDancer

CuteDancer is a .unitypackage dedicated to use on [VRChat](https://hello.vrchat.com/) avatars. It contains dance animations that can be played in sync together with other players who have the package installed on their avatars. [You can try it on public avatars here.](https://vrchat.com/home/world/wrld_deb6ff93-c907-4d16-92d0-911758135c70)

![promo anim](docs/images/cutedancer.gif)

### How does it work?

It uses new contacts components added recently to VRChat. When one avatar starts dancing, the sender component is activated and receivers on other avatars play that animation as well. It is possible to disable contacts if needed.

### Included dances

At the moment the package contains 5 dances:
- SAR Dance - default dance from [Super Animal Royale](https://animalroyale.com/) game
- Coincidance - shoulder shake meme dance
- Badger badger - simple badger dance
- Zufolo Impazzito - dance based on [this meme](https://www.reddit.com/r/doodoofard/comments/w6lhnl/dance/)
- Distraction Dance - [stickman meme dance](https://www.youtube.com/watch?v=6XK4S8OQPuU) (animation by Spooki Boy)

All above dance animations were created from scratch by [Krysiek](https://github.com/Krysiek) or Spooki Boy using [Cascadeur](https://cascadeur.com/) or [Blender](https://www.blender.org/).

## Download

[Download the newest version here](https://github.com/Krysiek/CuteDancer/releases)

## Installation

The video shows installation process for 3 typical scenarios. Detailed instructions are below the video.

https://user-images.githubusercontent.com/54168895/182499824-b87969a1-47ed-4541-a98c-be268e594142.mp4

### 1. **Import package to [Unity](https://unity.com/)**

Drag & drop `CuteDancer.unitypackage` file to Unity editor or select from Unity's top menu: `Assets` -> `Import package` -> `Custom package...`.

### 2. **Open CuteDancer Setup window**

Select (from Unity's top menu): `Tools` -> `CuteDancer Setup`.

The `CuteDancer Script` will help you automate some boring setup and will verify if everything is installed correctly.\*

___
_\* If you don't want to use a "magic tool" you can still use it for checking installation status. For manual installation description for advanced users [click here](docs/README.old.md)._
___

### 3. **Select your avatar in the `CuteDancer Script` window**

Drag & drop your avatar from Scene or click the circle button on the right side of the field and choose your avatar from the list.

### 4. **Click `Add` button in each section**

Click accordingly:
- `Add prefabs`
- `Add expression parameters`
- `Add expression submenu`
- `Add animator layers`
___
ℹ️ If your avatar does not have an expression parameters, expression menu or playable layers configured, you will see a prompt to create the missing asset. In this case click `Create it` and remember to save the project. ℹ️
___
ℹ️ Backup note: each operation creates one backup file per day. You can find them next to the original modified files. ℹ️
___

### 5. **Finished!**

Upload the avatar and enjoy dancing with your friends :)

## Updating package

ONLY if you are updating `CuteDancer` **from version 1.1 and earlier**, remove the `CuteDancer` directory from `Assets`. Remove all missing prefabs from avatar. Then install the new package and proceed with installation instructions above.

If you are updating from version 1.2 or newer, just install the new unitypackage file and click `Update animator layers` in the installation window.
