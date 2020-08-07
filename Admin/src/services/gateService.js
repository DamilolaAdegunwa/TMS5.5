import ajax from "./../utils/ajax.js";

export default {
  async getGateComboboxItemsAsync(gateGroupId = "") {
    const response = await ajax.get(`/gate/GetGateComboboxItemsAsync?gateGroupId=${gateGroupId}`);
    return response.result;
  }
};
