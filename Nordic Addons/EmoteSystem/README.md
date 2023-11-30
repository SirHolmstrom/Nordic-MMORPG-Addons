# Nordic Emote System Addon
*This addon allows you play emotes and audio and will automatically setup everything for you with the manager. The main idea behind the way it works is that it's easy to use for everyone.*

<center> <img src="https://jokeoverflow.xyz/Install-Guides/EmoteSystem/emote02.png" width="300" alt="Alt text for the image"> </center>

**After importing** the addon everything should work, right out of the box code wise, make sure you have the **Utils** folder. 
<br>
## **Installation Steps**

### **Step 0**: **Add PlayerEmote to every player prefab.**
<br>
It does everything it need so just leave it.

![Alt text for the image](https://jokeoverflow.xyz/Install-Guides/EmoteSystem/manager.png)

### **Step 1**: **Select EmoteManager**
The EmoteManager Scriptable Object is located at:
*Assets\uMMORPG\Scripts\Addons\Nordic-MMORPG-Addons\Nordic Addons\EmoteSystem\***EmoteManager**

![Alt text for the image](https://jokeoverflow.xyz/Install-Guides/EmoteSystem/manager.png)

### **Step 2**: **Assign your players Animator Controller**
Drag the **Animator Controller** from your player classes, if they share the same controller you only need to assign it one time.**.

![Alt text for the image](https://jokeoverflow.xyz/Install-Guides/EmoteSystem/GetController.png)

### **Step 3**: **Click Update Animators**
Now the Emote Manager will create a new layer called Emote Fullbody and add all the animation clips that are inside your Emotes list.

![Alt text for the image](https://jokeoverflow.xyz/Install-Guides/EmoteSystem/clickUpdate.png)

### **Step 4**: **Test the Addon**
Play and type /<Identifier> the <Identifier> is what you named the emote, in the example setup there is Dance, Wave, Cheer & Clap so /dance, /wave, /cheer, /clap.

### **OPTIONAL**: **Add more Emotes**
It's super easy to add more emotes, simply make a new entry in the list, assign as many animation variants as you wish, 
give it a identifier "name", 
decide if you want it to be able to loop or not, 
then **CLICK UPDATE ANIMATOR** and it will work.

![Alt text for the image](https://jokeoverflow.xyz/Install-Guides/EmoteSystem/populate.png)

<br>
<br>

### Note:
*If you wish to call it from a button or similar you simply use*:
```
// for example.
Player.Localplayer.emote.TryEmote("Dance");
```
- we don't send the string to the server it's getting the id after that and finds the EmoteData using that integer.
- OneShot means it will stop playing after one loop while Loop will continue until you start moving or leaving players "IDLE" state.
