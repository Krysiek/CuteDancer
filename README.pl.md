# **CuteDancer**

_Animacje, paczka: [Krysiek](https://github.com/Krysiek)  
Konfiguracja Sender/Receiver, wsparcie i testy: [Luc4r](https://github.com/Luc4r)  
Optymalizacja animator贸w, testy: [Jack'lul](https://github.com/jacklul)_

[馃嚞馃嚙 For English - click here](/README.md)

## Opis CuteDancer

CuteDancer jest paczk膮 .unitypackage dedykowan膮 dla avatar贸w [VRChat](https://hello.vrchat.com/). Zawiera animacje ta艅c贸w, kt贸re mog膮 by膰 odgrywane r贸wnocze艣nie z innymi graczami maj膮cymi t臋 paczk臋 zainstalowan膮 na swoich avatarach.

![promo anim](docs/images/cutedancer.gif)

### Jak to dzia艂a?

Dzi臋ki komponentom `contacts` mo偶liwe jest wysy艂anie sygna艂贸w mi臋dzy avatarami. Gdy jeden avatar zaczyna ta艅czy膰, aktywowany jest odpowiedni `sender`, a `receiver` na avatarze innych graczy aktywuje dan膮 animacj臋. Istnieje mo偶liwo艣膰 wy艂膮czenia `receiver`a w razie potrzeby.

### Za艂膮czone animacje

Na ten moment paczka zawiera 3 ta艅ce:
- SAR Dance - domy艣lny taniec z gry [Super Animal Royale](https://animalroyale.com/)
- Coincidance - taniec potrz膮sania barkiem (z memicznego teledysku)
- Badger badger - taniec wzorowany klasyczn膮 animacj膮 flashow膮

Wszystkie powy偶sze animacje stworzy艂 od podstaw [Krysiek](https://github.com/Krysiek) przy u偶yciu programu [Cascadeur](https://cascadeur.com/).  
Stworzone na bazie modelu Taiduma, ale b臋d膮 r贸wnie偶 dzia艂a膰 na innych avatarach.

## Pobieranie

[Pobierz najnowsz膮 wersj臋 tutaj](https://github.com/Krysiek/CuteDancer/releases)

## Instalacja

https://user-images.githubusercontent.com/54168895/173168780-17e84099-d3df-47c0-89eb-c4ad40ba456c.mp4

### 1. **Zaimportuj paczk臋 do [Unity](https://unity.com/)**

Przeci膮gnij i upu艣膰 plik `CuteDancer.unitypackage` do Unity albo wybierz z g贸rnego menu: `Assets` -> `Import package` -> `Custom package...`

Wszystkie potrzebne pliki znajdziesz w katalogu `CuteDancer` w g艂贸wnym folderze `Assets`.

### 2. **Otw贸rz okno `CuteDancer Setup`**

Wybierz z g贸rnego menu Unity: `Tools` -> `CuteDancer Setup`.

Okno `CuteDancer Script` pomo偶e Ci zautomatyzowa膰 pewne nudne czynno艣ci oraz pozwoli zweryfikowa膰, czy paczka zosta艂a zainstalowana poprawnie.\*

___
_\* Pod warunkiem, 偶e pozostawisz prefaby na swoim miejscu i nie b臋dziesz modyfikowa艂 innych ustawie艅. Zmiany wprowadzone do prefab贸w, animacji, parametr贸w, menu czy warstw animatora mog膮 zmyli膰 skrypt. Je艣li jeste艣 do艣wiadczonym u偶ytkownikiem Unity i jeste艣 艣wiadomy tego co robisz, mo偶esz zajrze膰 do [instrukcji dla zaawansowanych](docs/README.old.md)._
___

### 3. **Wybierz tw贸j avatar w oknie `CuteDancer Script`**

Przeci膮gnij i upu艣膰 avatar ze sceny lub kliknij k贸艂ko po prawej stronie pola i wybierz avatar z listy.

### 4. **Przeci膮gnij i upu艣膰 prefaby `CuteDancerContact` i `CuteDancerMusic` na g艂贸wny obiekt Twojego avatara**

Sprawd藕 czy prefaby zosta艂y wykryte w oknie `CuteDancer Setup` (je艣li okno nie chce si臋 od艣wie偶y膰, poruszaj nad nim myszk膮).

### 5. **Dodaj nowe parametry do `VRC Expressions Parameters`**

Po wybraniu avatara, pole `Expression Parameters` powinno zosta膰 wype艂nione automatycznie.
___
鈩癸笍 Je艣li Tw贸j avatar nie posiada parametr贸w i menu expresji [sprawd藕 jak je utworzy膰](https://docs.vrchat.com/docs/expression-menu-and-controls#creating-an-expression-menu). 鈩癸笍
___

Kliknij przycisk `Add expression parameters`.

### 5. **U偶yj `VRCMenu_CuteDancer` jako submenu w swoim menu ekpresji**

Pole `Expression Menu` r贸wnie偶 powinno zosta膰 wype艂nione automatycznie.

Kliknij przycik `Add expression submenu`.

### 6. **Dodaj warstwy kontroler贸w `Action` i `FX`**

Pola `Action` i `FX` powinny r贸wnie偶 zosta膰 uzupe艂nione automatycznie.

___
鈩癸笍 Je艣li Tw贸j avatar nie posiada kontroler贸w `Action` lub `FX`, stw贸rz je. 鈩癸笍  
Kliknij prawym przyciskiem w oknie `Project` i wybierz `Create -> Animator Controller`. Kliknij Tw贸j avatar na scenie. W inspektorze w `VRC Avatar Descriptor` w sekcji `Playable Layers` podepnij brakuj膮ce kontrolery. Nast臋pnie wczytaj ponownie avatar w oknie `CuteDancer Setup`. Wi臋cej informacji o kontrolerach znajdziesz [w oficjalnej dokumentacji](https://docs.vrchat.com/docs/playable-layers).
___

Kliknij przycik `Add animator layers`.

### 7. **Gotowe!**

Wgraj sw贸j avatar i ta艅cz razem z przyjaci贸艂mi :)

## Aktualizowanie paczki

Je偶eli aktualizujesz `CuteDancer` z wersji 1.1 lub starszej, usu艅 prefab `Music` z `[Avatar] -> Armature -> Hips`. Nast臋pnie zainstaluj paczk臋 i wykonaj poni偶sze kroki.

Je偶eli pozosta艂e艣 przy domy艣lnych ustawieniach paczki, aktualizacja b臋dzie banalnie prosta:

- Z g贸rnego menu Unity wybierz: `Tools` -> `CuteDancer Setup`.

- Wybierz Tw贸j avatar w polu `Avatar`.

- Upewnij si臋, 偶e prefaby CuteDancerMusic i CuteDancerContacts s膮 wykryte poprawnie.

- W ka偶dej z sekcji kliknij przycisk `Remove` a nast臋pnie `Add`.