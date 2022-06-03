# **CuteDancer**

_Animations, package, docs: [Krysiek](https://github.com/Krysiek)  
Sender/Receiver config, support and tests: [Luc4r](https://github.com/Luc4r)_

[🇵🇱 Język Polski - kliknij tutaj](/README.pl.md)

## About CuteDancer

CuteDancer is a unitypackage dedicated to use on [VRChat](https://hello.vrchat.com/) avatars. It contains dance animations that can be played in sync together with other players who have the package installed on their avatars.

![promo anim](/docs/images/cutedancer.gif)

### How does it work?

It uses new contacts components added recently to VRChat. When one avatar starts dancing, the sender component is activated and receivers on other avatars play that animation as well. It is possible to disable contacts if needed.

### Included dances

At the moment the package contains 3 dances:
- SAR Dance - default dance from Super Animal Royale game
- Coincidance - shoulder shake meme dance
- Badger badger - simple badger dance

All above dance animations were created from scratch by [Krysiek](https://github.com/Krysiek) using [Cascadeur](https://cascadeur.com/).  
I created them on Taidum model, but they will work on all other avatars.

## Download

-- TODO create release

## Installation

### 1. Import package to [Unity](https://unity.com/)

Drag & drop `CuteDancer-v2-0.unitypackage` file to Unity editor or select from Unity's top menu: `Assets` -> `Import package` -> `Custom package...`

All necessary files will be placed in the `CuteDancer` directory in your main `Assets` folder.

### 2. Drag & drop `Music` prefab to Hierarchy: `[Avatar]` -> `Armature` -> `Hips`
- If your avatar doesn't have `Hips` bone just drop `Music` prefab on the first bone under `Armature` (this will require modyfing animations a bit, more on step [6. a.](#6-a-updating-animations))

![step 2](/docs/images/step2.png)

### 3. Drag & drop `CuteDancerContact` prefab on your `[Avatar]`

![step 3a](/docs/images/step3a.png)

- For all receivers and senders, set `Root Transform` to `Hips` (not necessary, but it should work better with space movers etc.)  
   - Again - if your avatar doesn't have `Hips` bone just select the first bone under `Armature`

![step 3b](/docs/images/step3b.png)

### 4. Add new parameters to your `VRC Expressions Parameters`:

- `VRCEmote` (if doesn't exist) with `Type` set to `Int`
- `CuteDancerContactOff`, with `Type` set to `Bool`
- `CuteDancerMusicOff`, with `Type` set to `Bool`

You can check out the `VRCParams_Example` file as an example.

![step 4a](/docs/images/step4a.png)

![step 4b](/docs/images/step4b.png)

### 5. Use `VRCMenu_CuteDancer` as a submenu in your `VRC Expressions Menu`

![step 5a](/docs/images/step5a.png)
![step 5b](/docs/images/step5b.png)

### 6. Transfer layers to `Action` and `FX` controllers from example files

Example controllers are named accordingly: `Ctrl_Action_Example` and `Ctrl_FX_Example`.

You can transfer layers manually by selecting all blocks and using `Ctrl+C` and `Ctrl+V`, or you can use an automatic script included in the package.

_________________

🛑 **REMEMBER TO BACKUP YOUR ACTION AND FX CONTROLLERS BEFORE USING THE SCRIPT** 🛑

_________________

Select (from Unity's top menu): `Tools` -> `CuteDancer Setup`

In first field, select `Action controller` used by your avatar.   
In second field, select `FX controller` used by your avatar.

You can either select them from the list (click the circle button on the right side of controller select field) or drag & drop them from `Project` tab.

Click: `Add layers to my avatar`.

![step 6](/docs/images/step6.png)

_________________

⚠️ Warning: If you want to remove layers later (or have an easier time updating the package in the future) - don't change CuteDance's layer names in your controllers! ⚠️

_________________

### 6. a. Updating animations

As mentioned before - your avatar may not have the `Hips` bone in the correct spot (or using a different name for it, f.e. `Pelvis`). In that case you will **need** to update some animations for the package to work.   
- Open `CuteDancer/AnimsToggle` folder (from your main `Assets` folder)
- All animations but `Contact_ON`/`Contact_OFF` need to be updated.
    - Let's start with `MusicAll_OFF` animation - click on it
    -  Open the `Animation` tab (in case you don't see it select `Window` -> `Animation` -> `Animation` from Unity's top menu)
    - Click on the `Music : Game Object.Is Active` label and then click again - after a second, it should switch to the text field which contains path to the missing object (`Armature/Hips/Music`). Change `Hips` to whatever your first bone under `Armature` is named
    - Done, this animation should work properly! Now repeat these steps for other animations from this folder (`MusicAll_ON` will be the exact same steps, for other animations there are two fields - `...Music` and `...Sender` - you only need to update `...Music` one since it contains `Hips` part which you need to replace)

![step 6a - 1](/docs/images/step6a1.png)
![step 6a - 2](/docs/images/step6a2.png)

### 7. **Finished!**

Upload the avatar and enjoy dancing with your friends :)