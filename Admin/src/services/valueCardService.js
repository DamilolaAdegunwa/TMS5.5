import ajax from "./../utils/ajax.js";
import file from "./../utils/file.js";
import qs from "qs";

export default {
  async queryCzkDetailsToExcelAsync(input) {
    await file.downloadExcelAsync("/valueCard/QueryCzkDetailsToExcelAsync", qs.stringify(input));
  },
  async queryCzkDetailsAsync(input) {
    const response = await ajax.post("/valueCard/QueryCzkDetailsAsync", qs.stringify(input));
    return response.result;
  },
  async getCzkOpTypeComboboxItems() {
    const response = await ajax.get("/valueCard/GetCzkOpTypeComboboxItems");
    return response.result;
  },
  async getCzkRechargeTypeComboboxItems() {
    const response = await ajax.get("/valueCard/GetCzkRechargeTypeComboboxItems");
    return response.result;
  },
  async getCzkConsumeTypeComboboxItems() {
    const response = await ajax.get("/valueCard/GetCzkConsumeTypeComboboxItems");
    return response.result;
  },
  async getCzkCztcComboboxItemsAsync() {
    const response = await ajax.get("/valueCard/GetCzkCztcComboboxItemsAsync");
    return response.result;
  }
};
