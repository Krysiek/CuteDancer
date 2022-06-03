# CuteDancer

_Animations, package, doc: Krysiek  
Sender/Receiver config, support and tests: Luc4r_

[🇵🇱 Język Polski - kliknij tutaj](/README.pl.md)

## About CuteDancer

CuteDancer is a unitypackage dedicated for VRChat avatars. It contains dance animations that can be played in sync together with other players who has the package installed on their avatars.

![promo anim](/docs/images/cutedancer.gif)

### How does it work?

It uses new contacts components added reccently to VRChat. When one avatar starts dancing, the sender component is activated and receivers on others avatars plays animation as well. It is possible to disable receivers if needed.

### Included dances

At the moment package contains 3 dances:
- SAR Dance - default dance from Super Animal Royale game
- Coincidance - shoulder shake dance meme
- Badger badger - simple badger dance

All above dance animations are created from scratch by Krysiek using [Cascadeur](https://cascadeur.com/).  
I created them on Taidum model, but they will work on all other avatars.

## Download

-- TODO create release

## Installation

### 1. Import package to Unity

Drag & drop file CuteDancer-v2-0.unitypackage to Unity editor or select from menu: Assets -> Import package -> Custom package...

All necessary files will be placed in CuteDancer directory in your assets.

### 2. Drag & drop Music prefab to Hierarchy: Avatar -> Armature -> Hips

![step 2](/docs/images/step2.png)

### 3. Drag & drop CuteDancerContact prefab on Avatar

![step 3a](/docs/images/step3a.png)

![step 3b](/docs/images/step3b.png)

For all receivers and senders set Root Transform to Hips (not necessary, but it should will work better with space movers etc.)

### 4. Add new params to your VRCParams:

- VRCEmote (if not exists)
- CuteDancerContactOff
- CuteDancerMusicOff

You can see VRCParams_Example file as an example.

![step 4a](/docs/images/step4a.png)

![step 4b](/docs/images/step4b.png)

### 5. Use VRCMenu_CuteDancer as a submenu

![step 5a](/docs/images/step5a.png)

![step 5b](/docs/images/step5b.png)

### 6. Transfer layers to Action and FX controllers from example files

Example controllers are: Ctrl_Action_Example and Ctrl_FX_Example.

You can transfer layers manually by selecting all blocks and using Ctrl+C and Ctrl+V or you can use an automatic script included in the package.

🛑 **REMEMBER TO BACKUP YOUR ACTION AND FX CONTROLLERS BEFORE USING THE SCRIPT** 🛑

Select from top menu: Tools -> CuteDancer Setup

In first field select Action controller used by your avatar.
In second field select FX controller used by your avatar.

You can drag&drop them from assets.

Click: Add layers to my avatar.

⚠️ Warning: If you would want to remove layers later - don't change CuteDance's layer names in your controllers! ⚠️

![step 6](/docs/images/step6.png)

### 7. Finished! Upload the avatar - dances should work :)
