string sorting_by_count_prefix = "{COUNT}";

IMyTerminalBlock sys_sorting_cache;
List<IMyTerminalBlock> Containers;

public class ItemWithCount {
    public double Amount { get; set; }
    public MyInventoryItem Item { get; set; }

    public ItemWithCount(double amount, MyInventoryItem item) {
        Amount = amount;
        Item = item;
    }
}


public Program() {
  Containers = new List<IMyTerminalBlock>();
  sys_sorting_cache = GridTerminalSystem.GetBlockWithName("SYS_SORTING_CACHE");
  if (sys_sorting_cache == null) {
    throw new Exception("SYS_SORTING_CACHE not found");
  }
  // Runtime.UpdateFrequency = UpdateFrequency.Update100; // Update every tick
}

public void Save() {}

public void Main(string argument, UpdateType updateSource) {
  Containers = new List<IMyTerminalBlock>();
  GridTerminalSystem.SearchBlocksOfName(sorting_by_count_prefix, Containers);
  sortItemsByCount();
}

public void sortItemsByCount() {
  foreach (IMyCargoContainer container in Containers) {
    if (!container.CustomName.Contains(sorting_by_count_prefix)) {
      continue;
    }
    if (!(container.GetInventory(0).ItemCount > 0)){
      continue;
    }
    
    List<MyInventoryItem> raw_container_items = new List<MyInventoryItem>();
    List<ItemWithCount> itemsWithCount = new List<ItemWithCount>();
    container.GetInventory(0).GetItems(raw_container_items);

    foreach (MyInventoryItem raw_container_item in raw_container_items) {
      double item_count = double.Parse(raw_container_item.ToString().Split(new char[]{'x'}, 2)[0]);
      itemsWithCount.Add(new ItemWithCount(item_count, raw_container_item));
    }
    itemsWithCount.Sort((x, y) => y.Amount.CompareTo(x.Amount));
    foreach (ItemWithCount item in itemsWithCount) {
      container.GetInventory(0).TransferItemTo(sys_sorting_cache.GetInventory(0), item.Item);
    }
    List<MyInventoryItem> sys_items = new List<MyInventoryItem>();
    sys_sorting_cache.GetInventory(0).GetItems(sys_items);
    if (!(sys_items.Count > 0)) {
      continue;
    }
    foreach (MyInventoryItem sys_item in sys_items) {
      sys_sorting_cache.GetInventory(0).TransferItemTo(container.GetInventory(0), sys_item);
    }
  }
}
