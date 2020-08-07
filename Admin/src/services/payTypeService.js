import ajax from "./../utils/ajax.js";

export default {
  async getPayTypeComboboxItemsAsync() {
    const response = await ajax.get("/payType/GetPayTypeComboboxItemsAsync");
    return response.result;
  }
};
