# Nordic Emote System Addon
*This addon allows you play emotes, dynamic emote text in chat, and audio to be played and it will automatically setup everything for you after the few steps. The main idea behind the way it works is that it's easy to use for everyone.*

<img src="https://jokeoverflow.xyz/Install-Guides/EmoteSystem/s8.png" width="200" alt="Alt text for the image"> <img src="https://jokeoverflow.xyz/Install-Guides/EmoteSystem/s9.png" width="200" alt="Alt text for the image"> <img src="https://jokeoverflow.xyz/Install-Guides/EmoteSystem/s10.png" width="200" alt="Alt text for the image">


**After importing** the addon everything should work, right out of the box code wise, there are no core edits, make sure you have the **Utils** folder. 
<br>
## **Installation Steps**

### **Step 0**: **Add PlayerEmote to every player prefab.**
It will add a listener automatically to PlayerChat component and everything else works out of the box, you only need to do a few button clicks from here:

<img src="https://jokeoverflow.xyz/Install-Guides/EmoteSystem/s1.png" width="300" alt="Alt text for the image">

### **Step 1**: **Press: "Find Animator Controller**
This will select the runtime controller you are using for the player.

<img src="https://jokeoverflow.xyz/Install-Guides/EmoteSystem/s2.png" width="300" alt="Alt text for the image">


### **Step 2**: **Assign a Scriptable Emote List**
Simply assign the included **Scriptable Emote List** or create your own right away.
*Assets\uMMORPG\Scripts\Addons\Nordic-MMORPG-Addons\Nordic Addons\Nordic Emote System\Resources\Player Human Generic Emote List*

<img src="https://jokeoverflow.xyz/Install-Guides/EmoteSystem/s3.png" width="300" alt="Alt text for the image">

### **Step 3**: **Click Update Animators**
Now the press Update Animators, it will create a new layer called Emote Fullbody and add all the animation clips that are inside your Emotes list.
<br>
<img src="https://jokeoverflow.xyz/Install-Guides/EmoteSystem/s4.png" width="300" alt="Alt text for the image"><img src="https://jokeoverflow.xyz/Install-Guides/EmoteSystem/s5.png" width="300" alt="Alt text for the image">

### **Step 4**: **Test the Addon**
Play and type /<Identifier> the <Identifier> is what you named the emote, in the example setup there is Dance, Wave, Cheer & Clap so /dance, /wave, /cheer, /clap.

### **OPTIONAL**: **Add more Emotes**
It's super easy to add more emotes, simply make a new entry in the list, assign as many animation variants as you wish, 
give it a identifier "name", 
decide if you want it to be able to loop or not, 
if you wish to not use the chat emote text, then just leave blank,
then **CLICK UPDATE ANIMATOR** and it will work.

<img src="https://jokeoverflow.xyz/Install-Guides/EmoteSystem/s6.png" width="400" alt="Alt text for the image">

<br>

### Note:
*If you wish to call it from a button or similar you simply use*:
```
// for example.
Player.Localplayer.emote.TryEmote("Dance");
```
- we don't send the string to the server it's getting the id after that and finds the EmoteData using that integer.
- OneShot means it will stop playing after one loop while Loop will continue until you start moving or leaving players "IDLE" state.

### UPDATE **Emote Text In Chat**
Introducing a very neat option to use an emote message in text, if you have a target it will display that emote in relation to it, if no target it's just a simple emote.
for example:
no target emote quote:
{EMOTER} clap excitedly.
target emote quote:
{EMOTER} clap excitedly for {TARGET}.
The neat thing is this never goes via the server but is synced accross, so no extra overhead.
EMOTER will be translated the following way:
are we the EMOTER? You : Name
are we the TARGET? You : Name
So while you see "You burst into Dance with OtherPlayerName", they see "Yourname burst into Dance with OtherPlayerName (or YOU if the target is you).
