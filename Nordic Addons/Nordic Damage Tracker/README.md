# Nordic Damage Tracker
*This addon allows you to include a Damage Tracker for your player and all it's party members!*

**After importing** the addon we have three things we need to do. 
<br>
## **Installation Steps**

### **Step 1**: **Minor Core adjustment and ensure Partials**
**Make sure you have the Utils Folder for the Addons.** <br>
**Make sure Combat.cs is partial** ``public partial class Combat : NetworkBehaviour``<br>
Add the following ``UnityEvent<Entity, int>`` to Combat.cs:

```
            // Nordic Addon Event
            onServerEntityDealsDamage.Invoke(entity, damageDealt);
```

<img src="https://jokeoverflow.xyz/Install-Guides/tracker/s1.png" width="950" alt="Alt text for the image">

### **Step 2**: Drag in the included CANVAS prefab without parenting it:
locate the prefab at: ``..\Nordic Damage Tracker\Prefabs\[Canvas]DamageTracker.prefab``
We will be redrawing the damage tracker many times so it's suggested to have it's own Canvas so we don't have to redraw the whole UI.

### **Step 3**: **Locate and click the "Setup RPC Handler": **
Simply find the DamageTrackerManager inside the Prefab we just dragged and click the button "Setup RPC Handler.
It will scan your spawn list for Player classes and add it automatically, don't worry about duplicates.

<img src="https://jokeoverflow.xyz/Install-Guides/tracker/s2_3.png" width="950" alt="Alt text for the image">

### **Step 4**: **Test the addon: **
Go ahead and try to hit a couple of monsters or other players and your damage will be tracked!


### Potential incompatible 
if you already have a OnStartClient override for the Player.cs you have to move the lazy init from DamageTracker.Partials.