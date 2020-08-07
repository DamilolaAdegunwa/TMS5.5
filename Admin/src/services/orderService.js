import ajax from "./../utils/ajax.js";
import file from "./../utils/file.js";
import qs from "qs";

export default {
  async getOrderStatusComboboxItems() {
    const response = await ajax.get("/order/GetOrderStatusComboboxItems");
    return response.result;
  },
  async getConsumeStatusComboboxItems() {
    const response = await ajax.get("/order/GetConsumeStatusComboboxItems");
    return response.result;
  },
  async getRefundStatusComboboxItems() {
    const response = await ajax.get("/order/GetRefundStatusComboboxItems");
    return response.result;
  },
  async getOrderTypeComboboxItems() {
    const response = await ajax.get("/order/GetOrderTypeComboboxItems");
    return response.result;
  },
  async getOrdersToExcelAsync(input) {
    await file.downloadExcelAsync("/order/GetOrdersToExcelAsync", qs.stringify(input));
  },
  async getOrdersAsync(input) {
    const response = await ajax.post("/order/GetOrdersAsync", qs.stringify(input));
    return response.result;
  },
  async getOrderFullInfoAsync(input) {
    const response = await ajax.post("/order/GetOrderFullInfoAsync", input);
    return response.result;
  }
};
