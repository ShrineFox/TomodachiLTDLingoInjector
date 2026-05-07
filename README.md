![](https://i.imgur.com/fBTUXhw.png)
# Tomodachi Life: Living The Dream - LINGO INJECTOR
This is a fork of [living-the-dream-save-editor](https://github.com/tlmodding/living-the-dream-save-editor) by [Rafacasari](https://github.com/Rafacasari).  
I added features related to managing island lingo. You can **import, export, and edit lingo in batch**. 
This is an easy way to freshen up dialogue in your game, since you don't have to wait for an opportunity to add lingo one by one while playing.  

Of course, you'll need to use either a modded switch with a homebrew like JKSV to edit and replace your save file, or a Switch emulator.

# Features
- ``Lingo > Inject... > From File``
  - Choose a text file to import lingo from. On each line, lingo is organized like this: ``word|type`` (i.e. ``dying after eating Arby's|Action``)
- ``Lingo > Inject... > From TomoLingo``
  - This option will import random lingo from the website https://tomolingo.neocities.org/ by [ryder_flyder](https://x.com/ryder_flyder).  
  - **NOTE: lingo from this site is user-submitted, and may be NSFW or political in nature. Discretion is advised.**
- ``Lingo > Replace Existing Lingo``
  - When this box is checked, all existing lingo will be overwritten when using one of the above "inject" options.
- ``Lingo > SFW Only TomoLingo text``
  - When this box is checked, only **family friendly** lingo from TomoLingo will be imported using the above "inject" option.
- ``Lingo > Set Grammar for "a" and "an"``
  - When this box is checked, injected lingo that starts with "a" or "an" will have that removed from the lingo text itself.  
  - The program will attempt to set this as an attribute instead, to avoid repeats of the words "a" or "an" in-game when spoken.
- ``Lingo > Import Lingo List``
  - This option will import all lingo from a previously exported ``lingo.tsv`` file, overwriting all existing lingo.
- ``Lingo > Export Lingo List``
  - This option will export all current lingo as a ``lingo.tsv`` file so you can easily restore it later.
- ``Lingo > Clear Lingo List``
  - This option will remove all lingo currently in the lingo list. This is useful if you want to start clean.
 
# Limitations
- Currently, this only supports the English language (at least as far as injecting from TomoLingo or setting grammar attributes automatically).  
  You can still import/export and bulk edit your lingo if you're playing in a different language.
- It's recommended to proofread TomoLingo imports, since some may be worded as suggestions or "(insert thing here)".  
   If you're streaming the game, you may want to remove references to anything that violates TOS ahead of time.

# Topic Types
Use these when editing the exported ``lingo.tsv`` file, or creating a ``.txt`` file of lingo to randomly import.  
These types determine how the lingo will be used in conversation.  
  
Person  
Object  
Action  
Topic  
Phrase  
