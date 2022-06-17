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

At the moment the package contains 3 dances:
- SAR Dance - default dance from [Super Animal Royale](https://animalroyale.com/) game
- Coincidance - shoulder shake meme dance
- Badger badger - simple badger dance

All above dance animations were created from scratch by [Krysiek](https://github.com/Krysiek) using [Cascadeur](https://cascadeur.com/).  
I created them on Taidum model, but they will work on all other avatars.

## Download

[Download the newest version here](https://github.com/Krysiek/CuteDancer/releases)

## Installation

https://user-images.githubusercontent.com/54168895/173168780-17e84099-d3df-47c0-89eb-c4ad40ba456c.mp4

### 1. **Import package to [Unity](https://unity.com/)**

Drag & drop `CuteDancer.unitypackage` file to Unity editor or select from Unity's top menu: `Assets` -> `Import package` -> `Custom package...`

All necessary files will be placed in the `CuteDancer` directory in your main `Assets` folder.

### 2. **Open CuteDancer Setup window**

Select (from Unity's top menu): `Tools` -> `CuteDancer Setup`.

The `CuteDancer Script` will help you automate some boring setup and will verify if everything is installed correctly.\*

___
_\* As long as you stick with default installation settings and prefabs placement in the hierarchy. Changes made to prefabs, animations, parameters or layers can fool the script and assumes that you are experienced Unity user aware of what you are doing. For installation description for advanced users [click here](docs/README.old.md)._
___

### 3. **Select your avatar in the `CuteDancer Script` window**

Drag & drop your avatar from Scene or click the circle button on the right side of the field and choose your avatar from the list.

### 4. **Drag & drop `CuteDancerContact` and `CuteDancerMusic` prefabs on your main `[Avatar]` object**

Check if prefabs are detected in the `CuteDancer Setup` window (if the window does not refresh, move the mouse cursor above it).

### 5. **Add new parameters to your `VRC Expressions Parameters`**

Once you loaded your avatar in the CuteDancer Script window, the `Expression Parameters` field should be filled automatically.
___
ℹ️ If your avatar does not have an expression parameters and menu, [follow official documentation to create them](https://docs.vrchat.com/docs/expression-menu-and-controls#creating-an-expression-menu). ℹ️
___

Click `Add expression parameters` button.

### 5. **Add `VRCMenu_CuteDancer` as a submenu to your `VRC Expressions Menu`**

Once you loaded your avatar in the CuteDancer Script window, the `Expression Menu` field should be filled automatically.

Click `Add expression submenu` button.

### 6. **Add layers to `Action` and `FX` controllers**

Once you loaded your avatar in the CuteDancer Script window, the `Action` and `FX` fields should be filled automatically.

___
ℹ️ If your avatar does not contain `Action` or `FX` controllers, create them. ℹ️  
Click right mouse buttom in `Project` window and select `Create -> Animator Controller`. Click your avatar on the scene. In the `Inspector` window in `VRC Avatar Descriptor` navigate to `Playable Layers` section and add missing `Action` or `FX` controllers. After that, reload your avatar in the `CuteDancer Setup` window. More information about controllers you will find [in the official documentation](https://docs.vrchat.com/docs/playable-layers).
___

Click: `Add animator layers` button.

### 7. **Finished!**

Upload the avatar and enjoy dancing with your friends :)

## Updating package

If you are updating `CuteDancer` from version 1.1 and earlier, remove the `Music` prefab from `[Avatar] -> Armature -> Hips`. Then install the new package and follow the steps below.

As long as you left the default installation settings, update should be simple:

- Select (from Unity's top menu): `Tools` -> `CuteDancer Setup`.

- Select your avatar in `Avatar` field.

- Verify if Music and Contacts prefabs are detected correctly.

- In every section click `Remove` button and then an `Add` button.
