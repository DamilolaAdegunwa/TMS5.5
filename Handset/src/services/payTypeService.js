import ajax from "./../utils/ajax.js";
import { App } from "../handset-sdk.js";

export default {
  async getPayTypeComboboxItemsAsync() {
    const response = await ajax.get("/PayType/GetPayTypeComboboxItemsAsync");
    if (App) {
      App.setValue("payTypeItems", JSON.stringify(response.result));
    } else {
      window.localStorage.setItem(
        "payTypeItems",
        JSON.stringify(response.result)
      );
    }
  },
  getPayTypeName(value) {
    let payTypeItemsStr = "";
    if (App) {
      payTypeItemsStr = App.getValue("payTypeItems");
    } else {
      payTypeItemsStr = localStorage.getItem("payTypeItems");
    }
    let payTypeItems = JSON.parse(payTypeItemsStr);
    let payTypeItem = payTypeItems.filter(c => c["value"] == value)[0];
    return payTypeItem.displayText;
  }
};
