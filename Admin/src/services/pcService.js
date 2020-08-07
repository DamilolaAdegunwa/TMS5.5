import ajax from "./../utils/ajax.js";

export default {
  async getCashPcComboboxItemsAsync() {
    const response = await ajax.get("/pc/GetCashPcComboboxItemsAsync");
    return response.result;
  }
};
