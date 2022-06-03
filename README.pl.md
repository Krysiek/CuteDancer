# CuteDancer

_Animacje, paczka, dokumentacja: Krysiek  
Konfiguracja Sender/Receiver, wsparcie i testy: Luc4r_

[🇬🇧 For English - click here](/docs/README.md)

## Opis CuteDancer

CuteDancer jest paczką .unitypackage dedykowaną dla avatarów VRChat. Zawiera animacje tańców, które mogą być odgrywane równocześnie z innymi graczami mającymi tę paczkę zainstalowaną na swoich avatarach.

![promo anim](/docs/images/cutedancer.gif)

### Jak to działa?

Dzięki komponentom "contacs" dodanym niedawno do VRChata możliwe jest wysyłanie sygnałów między avatarami. Gdy jeden avatar zaczyna tańczyć, aktywowany jest odpowiedni "sender", a "receiver" na avatarze innych graczy aktywuje daną animację. Istnieje możliwość wyłączenia "receivera" w razie potrzeby.

### Załączone animacje

Na ten moment paczka zawiera 3 tańce:
- SAR Dance - domyślny taniec z gry Super Animal Royale
- Coincidance - taniec potrząsania barkiem (z memicznego teledysku)
- Badger badger - taniec wzorowany klasyczną animacją flashową

Wszystkie powyższe animacje stworzył od podstaw Krysiek przy użyciu programu [Cascadeur](https://cascadeur.com/).  
Tworzyłem je na modelu Taiduma, ale będą również działać na innych avatarach.

## Pobieranie

-- TODO create release

## Instalacja

### 1. Zaimportuj paczkę do Unity

Przeciągnij plik CuteDancer-v2-0.unitypackage do Unity lub wybierz z menu Assets -> Import package -> Custom package...

Wszystkie potrzebne pliki będą znajdowały się w katalogu CuteDancer w assetach.

### 2. Prefab Music przenieś do Avatar -> Armature -> Hips

![step 2](/docs/images/step2.png)

### 3. Prefab CuteDancerContact przenieś na Avatar

![step 3a](/docs/images/step3a.png)

![step 3b](/docs/images/step3b.png)

Dla wszystkich receiverów i senderów ustaw Root Transform na Hips twojego avatara (nie jest to wymagane, ale powinno poprawić działanie z space moverem itp.)

### 4. Wzorując się na VRCParams_Example dodaj do VRCParams używanego przez Twój avatar:

- VRCEmote (jeśli nie istnieje)
- CuteDancerContactOff
- CuteDancerMusicOff

![step 4a](/docs/images/step4a.png)

![step 4b](/docs/images/step4b.png)

### 5. Użyj VRCMenu_CuteDancer jako submenu w swoim avatarze

![step 5a](/docs/images/step5a.png)

![step 5b](/docs/images/step5b.png)

### 6. Przenieś warstwy kontrolera Action i FX z przykładowych plików


Pliki z przykładem to Ctrl_Action_Example i Ctrl_FX_Example.

Warstwy możesz przenieść ręcznie używając Ctrl+C i Ctrl+V lub użyć do tego automatycznego skryptu.

🛑 **PAMIĘTAJ O ZROBIENIU KOPII ZAPASOWEJ SWOICH KONTROLERÓW ACTION I FX** 🛑

Wybierz z górnego menu Tools -> CuteDancer Setup

Do pola Action przeciągnij kontroler Action używany przez Twój avatar
Do pola FX przeciągnij kontroler FX używany przez Twój avatar

Kliknij Add layers to my avatar.

⚠️ Uwaga: Jeśli w przyszłości chcesz mieć możliwość usunięcia warstw - nie zmieniaj ich nazw w swoich kontrolerach! ⚠️

![step 6](/docs/images/step6.png)

### 7. Gotowe, zbuduj i wgraj avatar, tańce powinny działać :)
