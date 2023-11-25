# Nordic Buyback Addon
*This addon allows you to buy back items that you have previously sold. It becomes available if there are any items to buy back.*

## **Installation Steps**
### **Step 1**: **Ensure Partial Classes**

**Make sure** that *UINpcTrading* is declared as **partial**. 
<br>
**Make sure** that *PlayerNpcTrading* is declared as **partial**.

### **Step 2**: **Setup the Buyback Panel**
Drag the **BuybackPanel** from ``Assets\uMMORPG\Scripts\Addons\Nordic Addons\Buyback\Prefabs`` to the **NpcTrading panel**.


### **Step 3**: **Add Required Fields**
The Buyback Prefab is located at:
*Assets\uMMORPG\Scripts\Addons\Nordic Addons\Buyback\Prefabs\Slot\* **UIBuybackSlot.prefab**



### **Step 4**: **Test the Addon**
Play and sell an item. The panel will only be visible if there is something to buy back by default.

### Note:
*If you don’t desire this default behavior, you can simply comment out or remove the following line inside* **UINpcTrading.Buyback.cs**:

```
buybackPanel.SetActive(buybackSlots.Count > 0);