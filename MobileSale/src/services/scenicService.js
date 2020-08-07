import ajax from "./../utils/ajax.js";

export default {
  async getScenicAsync() {
    const response = await ajax.get("/scenic/GetScenicAsync");
    return response.result;
  },
  async getGroundComboboxItemsAsync() {
    const response = await ajax.get("/scenic/GetGroundComboboxItemsAsync");
    return response.result;
  },
  async getGateGroupComboboxItemsAsync(groundId) {
    const response = await ajax.get(`/scenic/GetGateGroupComboboxItemsAsync?groundId=${groundId}`);
    return response.result;
  },
  async getWxTouristNeedCertTypeFlag(){
    const response = await ajax.get(`/scenic/GetWxTouristNeedCertTypeFlag`);
    return response.result;
  }
};
