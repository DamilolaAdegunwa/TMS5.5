import ajax from "./../utils/ajax.js";
import file from "./../utils/file.js";
import qs from "qs";

export default {
  async getMerchantComboBoxItemsAsync() {
    const response = await ajax.get("/ware/GetMerchantComboBoxItemsAsync");
    return response.result;
  },
  async getShopComboBoxItemsAsync() {
    const response = await ajax.get("/ware/GetShopComboBoxItemsAsync");
    return response.result;
  },
  async getShopTypeComboBoxItemsAsync() {
    const response = await ajax.get("/ware/GetShopTypeComboBoxItemsAsync");
    return response.result;
  },
  async getSupplierComboBoxItemsAsync() {
    const response = await ajax.get("/ware/GetSupplierComboBoxItemsAsync");
    return response.result;
  },
  async getWareTypeComboBoxItemsAsync() {
    const response = await ajax.get("/ware/GetWareTypeComboBoxItemsAsync");
    return response.result;
  },
  async getWareTypeTypeComboBoxItemsAsync() {
    const response = await ajax.get("/ware/GetWareTypeTypeComboBoxItemsAsync");
    return response.result;
  },
  async getPayDetailStatTypeComboBoxItems() {
    const response = await ajax.get("/ware/GetPayDetailStatTypeComboBoxItems");
    return response.result;
  },
  async queryWareAsync(input) {
    const response = await ajax.post("/ware/QueryWareAsync", input);
    return response.result;
  },
  async queryWareIODetailAsync(input) {
    const response = await ajax.post("/ware/QueryWareIODetailAsync", qs.stringify(input));
    return response.result;
  },
  async queryWareIODetailToExcelAsync(input) {
    await file.downloadExcelAsync("/ware/QueryWareIODetailToExcelAsync", qs.stringify(input));
  },
  async queryWareTradeAsync(input) {
    const response = await ajax.post("/ware/QueryWareTradeAsync", qs.stringify(input));
    return response.result;
  },
  async queryWareTradeToExcelAsync(input) {
    await file.downloadExcelAsync("/ware/QueryWareTradeToExcelAsync", qs.stringify(input));
  },
  async statWareTradeAsync(input) {
    const response = await ajax.post("/ware/StatWareTradeAsync", qs.stringify(input));
    return response.result;
  },
  async statWareTradeToExcelAsync(input) {
    await file.downloadExcelAsync("/ware/StatWareTradeToExcelAsync", qs.stringify(input));
  },
  async statWareSaleByWareTypeAsync(input) {
    const response = await ajax.post("/ware/StatWareSaleByWareTypeAsync", qs.stringify(input));
    return response.result;
  },
  async statWareSaleByWareTypeToExcelAsync(input) {
    await file.downloadExcelAsync("/ware/StatWareSaleByWareTypeToExcelAsync", qs.stringify(input));
  },
  async queryWareToExcelAsync(input) {
    await file.downloadExcelAsync("/ware/QueryWareToExcelAsync", input);
  },
  async statWareRentSaleAsync(input) {
    const response = await ajax.post("/Ware/StatWareRentSaleAsync", input);
    return response.result;
  },
  async statWareTradeTotalAsync(input) {
    const response = await ajax.post("/Ware/StatWareTradeTotalAsync", input);
    return response.result;
  },
  async statWareSaleAsync(input) {
    const response = await ajax.post("/ware/StatWareSaleAsync", input);
    return response.result;
  },
  async statWareSaleShiftAsync(input) {
    const response = await ajax.post("/ware/StatWareSaleShiftAsync", input);
    return response.result;
  }
};
