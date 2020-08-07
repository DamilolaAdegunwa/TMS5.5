import ajax from "./../utils/ajax.js";

export default {
  async getGroundComboboxItemsAsync() {
    const response = await ajax.get("/Scenic/GetGroundComboboxItemsAsync");
    return response.result;
  },
  async getSalePointComboboxItemsAsync() {
    const response = await ajax.get("/Scenic/GetSalePointComboboxItemsAsync");
    return response.result;
  },
  async getGateGroupComboboxItemsAsync(groundId) {
    const response = await ajax.get(
      `/Scenic/GetGateGroupComboboxItemsAsync?groundId=${groundId}`
    );
    return response.result;
  }
};
