# **CuteDancer**

_Animacje, paczka: [Krysiek](https://github.com/Krysiek)  
Konfiguracja Sender/Receiver, wsparcie i testy: [Luc4r](https://github.com/Luc4r)  
Optymalizacja animatorów, testy: [Jack'lul](https://github.com/jacklul)_

[🇬🇧 For English - click here](/README.md)

## Opis CuteDancer

CuteDancer jest paczką .unitypackage dedykowaną dla avatarów [VRChat](https://hello.vrchat.com/). Zawiera animacje tańców, które mogą być odgrywane równocześnie z innymi graczami mającymi tę paczkę zainstalowaną na swoich avatarach.

![promo anim](docs/images/cutedancer.gif)

### Jak to działa?

Dzięki komponentom `contacts` możliwe jest wysyłanie sygnałów między avatarami. Gdy jeden avatar zaczyna tańczyć, aktywowany jest odpowiedni `sender`, a `receiver` na avatarze innych graczy aktywuje daną animację. Istnieje możliwość wyłączenia `receiver`a w razie potrzeby.

### Załączone animacje

Na ten moment paczka zawiera 3 tańce:
- SAR Dance - domyślny taniec z gry [Super Animal Royale](https://animalroyale.com/)
- Coincidance - taniec potrząsania barkiem (z memicznego teledysku)
- Badger badger - taniec wzorowany klasyczną animacją flashową

Wszystkie powyższe animacje stworzył od podstaw [Krysiek](https://github.com/Krysiek) przy użyciu programu [Cascadeur](https://cascadeur.com/).  
Stworzone na bazie modelu Taiduma, ale będą również działać na innych avatarach.

## Pobieranie

[Pobierz najnowszą wersję tutaj](https://github.com/Krysiek/CuteDancer/releases)

## Instalacja

https://user-images.githubusercontent.com/54168895/173168780-17e84099-d3df-47c0-89eb-c4ad40ba456c.mp4

### 1. **Zaimportuj paczkę do [Unity](https://unity.com/)**

Przeciągnij i upuść plik `CuteDancer.unitypackage` do Unity albo wybierz z górnego menu: `Assets` -> `Import package` -> `Custom package...`

Wszystkie potrzebne pliki znajdziesz w katalogu `CuteDancer` w głównym folderze `Assets`.

### 2. **Otwórz okno `CuteDancer Setup`**

Wybierz z górnego menu Unity: `Tools` -> `CuteDancer Setup`.

Okno `CuteDancer Script` pomoże Ci zautomatyzować pewne nudne czynności oraz pozwoli zweryfikować, czy paczka została zainstalowana poprawnie.\*

___
_\* Pod warunkiem, że pozostawisz prefaby na swoim miejscu i nie będziesz modyfikował innych ustawień. Zmiany wprowadzone do prefabów, animacji, parametrów, menu czy warstw animatora mogą zmylić skrypt. Jeśli jesteś doświadczonym użytkownikiem Unity i jesteś świadomy tego co robisz, możesz zajrzeć do [instrukcji dla zaawansowanych](docs/ADVANCED.pl.md)._
___

### 3. **Wybierz twój avatar w oknie `CuteDancer Script`**

Przeciągnij i upuść avatar ze sceny lub kliknij kółko po prawej stronie pola i wybierz avatar z listy.

### 4. **Przeciągnij i upuść prefaby `CuteDancerContact` i `CuteDancerMusic` na główny obiekt Twojego avatara**

Sprawdź czy prefaby zostały wykryte w oknie `CuteDancer Setup` (jeśli okno nie chce się odświeżyć, poruszaj nad nim myszką).

### 5. **Dodaj nowe parametry do `VRC Expressions Parameters`**

Po wybraniu avatara, pole `Expression Parameters` powinno zostać wypełnione automatycznie.
___
ℹ️ Jeśli Twój avatar nie posiada parametrów i menu expresji [sprawdź jak je utworzyć](https://docs.vrchat.com/docs/expression-menu-and-controls#creating-an-expression-menu). ℹ️
___

Kliknij przycisk `Add expression parameters`.

### 5. **Użyj `VRCMenu_CuteDancer` jako submenu w swoim menu ekpresji**

Pole `Expression Menu` również powinno zostać wypełnione automatycznie.

Kliknij przycik `Add expression submenu`.

### 6. **Dodaj warstwy kontrolerów `Action` i `FX`**

Pola `Action` i `FX` powinny również zostać uzupełnione automatycznie.

___
ℹ️ Jeśli Twój avatar nie posiada kontrolerów `Action` lub `FX`, stwórz je. ℹ️  
Kliknij prawym przyciskiem w oknie `Project` i wybierz `Create -> Animator Controller`. Kliknij Twój avatar na scenie. W inspektorze w `VRC Avatar Descriptor` w sekcji `Playable Layers` podepnij brakujące kontrolery. Następnie wczytaj ponownie avatar w oknie `CuteDancer Setup`. Więcej informacji o kontrolerach znajdziesz [w oficjalnej dokumentacji](https://docs.vrchat.com/docs/playable-layers).
___

Kliknij przycik `Add animator layers`.

### 7. **Gotowe!**

Wgraj swój avatar i tańcz razem z przyjaciółmi :)

## Aktualizowanie paczki

Jeżeli aktualizujesz `CuteDancer` z wersji 1.1 lub starszej, usuń prefab `Music` z `[Avatar] -> Armature -> Hips`. Następnie zainstaluj paczkę i wykonaj poniższe kroki.

Jeżeli pozostałeś przy domyślnych ustawieniach paczki, aktualizacja będzie banalnie prosta:

- Z górnego menu Unity wybierz: `Tools` -> `CuteDancer Setup`.

- Wybierz Twój avatar w polu `Avatar`.

- Upewnij się, że prefaby CuteDancerMusic i CuteDancerContacts są wykryte poprawnie.

- W każdej z sekcji kliknij przycisk `Remove` a następnie `Add`.