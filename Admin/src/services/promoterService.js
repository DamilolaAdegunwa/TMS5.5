import ajax from "./../utils/ajax.js";

export default {
  async getPromoterComboboxItemsAsync() {
    const response = await ajax.get("/promoter/GetPromoterComboboxItemsAsync");
    return response.result;
  }
};
