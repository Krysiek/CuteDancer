CuteDancer Readme

Animacje, paczka: Krysiek
Konfiguracja Sender/Receiver, wsparcie i testy: Luc4r


Instrukcja z obrazkami:

https://docs.google.com/document/d/1M8_diZJ1aDf3xiJoETLlrEiemzylTZhXLehZLkIfck0/edit?usp=sharing


Instrukcja bez obrazków:

1. Prefab Music przenieś do Avatar -> Armature -> Hips

2. Prefab CuteDancerContact przenieś na Avatar

    Dla wszystkich receiverów i senderów ustaw Root Transform na Hips twojego avatara.

3. Wzorując się na VRCParams_Example dodaj do VRCParams używanego przez Twój avatar:
    - VRCEmote (jeśli nie istnieje)
    - CuteDancerContactOff
    - CuteDancerMusicOff

4. Użyj VRCMenu_CuteDancer jako submenu w swoim avatarze

5. Przenieś warstwy kontrolera Action i FX z przykładowych plików

    Pliki z przykładem to Ctrl_Action_Example i Ctrl_FX_Example.

    Warstwy możesz przenieść ręcznie lub użyć do tego automatycznego skryptu.

    ! PAMIĘTAJ O ZROBIENIU KOPII ZAPASOWEJ SWOICH KONTROLERÓW ACTION I FX !

    Wybierz z górnego menu Tools -> CuteDancer Setup

    Do pola Action przeciągnij kontroler Action używany przez Twój avatar
    Do pola FX przeciągnij kontroler FX używany przez Twój avatar

    Kliknij Add layers to my avatar.

    Uwaga: Jeśli w przyszłości chcesz mieć możliwość usunięcia warstw - nie zmieniaj ich nazw w swoich kontrolerach!

6. Gotowe, zbuduj i wgraj avatar, tańce powinny działać :)