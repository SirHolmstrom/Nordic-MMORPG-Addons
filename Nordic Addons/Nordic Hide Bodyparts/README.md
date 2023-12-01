# Nordic Hide Bodyparts
*This addon allows you to simply define using an enum array what body parts to hide (Gameobjects).*

**After importing** the addon we have two things we need to do. 
<br>
## **Installation Steps**

### **Step 1**: **Minor Core adjustment & Ensure partials**
First make EquipmentItem partial.

Add the following UnityEvent to PlayerEquipment in the Equipments slots callback:
```
            // Addon Event
            OnEquipmentChangedEvent.Invoke(oldItem as EquipmentItem, newItem as EquipmentItem);
```

<img src="https://jokeoverflow.xyz/Install-Guides/hide/s0.png" width="950" alt="Alt text for the image">

### **Step 2**: **Add method to new unity event: **

<img src="https://jokeoverflow.xyz/Install-Guides/hide/s1.png" width="950" alt="Alt text for the image">

### **Step 3**: **Setup Bodypart Mapping in PlayerEquipment: **
Simply assign a enum and the gameobjects (parts) you want to hide.

<img src="https://jokeoverflow.xyz/Install-Guides/hide/s2.png" width="950" alt="Alt text for the image">

### **Step 4**: **Setup Equipment items: **
Simply assign one or multiple enums and they will hide as we equip.

<img src="https://jokeoverflow.xyz/Install-Guides/hide/s3.png" width="950" alt="Alt text for the image">
