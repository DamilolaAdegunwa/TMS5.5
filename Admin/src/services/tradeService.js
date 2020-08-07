import ajax from "./../utils/ajax.js";
import file from "./../utils/file.js";
import qs from "qs";

export default {
  async getTradeSourceComboboxItems() {
    const response = await ajax.get("/trade/GetTradeSourceComboboxItems");
    return response.result;
  },
  async getTradeTypeTypeComboboxItemsAsync() {
    const response = await ajax.get("/trade/GetTradeTypeTypeComboboxItemsAsync");
    return response.result;
  },
  async getTradeTypeComboboxItemsAsync(tradeTypeTypeId = "") {
    const response = await ajax.get(
      `/trade/GetTradeTypeComboboxItemsAsync?tradeTypeTypeId=${tradeTypeTypeId}`
    );
    return response.result;
  },
  async queryTradesAsync(input) {
    const response = await ajax.post("/trade/QueryTradesAsync", qs.stringify(input));
    return response.result;
  },
  async getPayDetailStatTypeComboboxItems() {
    const response = await ajax.get("/trade/GetPayDetailStatTypeComboboxItems");
    return response.result;
  },
  async statPayDetailToExcelAsync(input) {
    await file.downloadExcelAsync("/trade/StatPayDetailToExcelAsync", qs.stringify(input));
  },
  async statPayDetailAsync(input) {
    const response = await ajax.post("/trade/StatPayDetailAsync", qs.stringify(input));
    return response.result;
  },
  async statTradeByPayTypeToExcelAsync(input) {
    await file.downloadExcelAsync("/trade/StatTradeByPayTypeToExcelAsync", qs.stringify(input));
  },
  async statTradeByPayTypeAsync(input) {
    const response = await ajax.post("/trade/StatTradeByPayTypeAsync", qs.stringify(input));
    return response.result;
  },
  async statPayDetailByCustomerToExcelAsync(input) {
    await file.downloadExcelAsync(
      "/trade/StatPayDetailByCustomerToExcelAsync",
      qs.stringify(input)
    );
  },
  async statPayDetailByCustomerAsync(input) {
    const response = await ajax.post("/trade/StatPayDetailByCustomerAsync", qs.stringify(input));
    return response.result;
  }
};
