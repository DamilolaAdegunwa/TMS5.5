import ajax from "@/utils/ajax.js";

export default {
  async logError(message) {
    if (typeof message == "object") {
      message = JSON.stringify(message);
    }

    await ajax.post(`/common/LogError?message=${message}`);
  },
  async getEducationComboboxItems() {
    let response = await ajax.get("/common/GetEducationComboboxItems");
    return response.result;
  },
  async getNationComboboxItems() {
    let response = await ajax.get("/common/GetNationComboboxItems");
    return response.result;
  },
  async getAgeRangeComboboxItemsAsync() {
    let response = await ajax.get("/common/GetAgeRangeComboboxItemsAsync");
    return response.result;
  },
  async getTouristOriginTreeAsync() {
    let response = await ajax.get("/common/GetTouristOriginTreeAsync");
    return response.result;
  },
  async getCertTypeComboBoxItemsAsync(){
    let response = await ajax.get(`/common/GetCertTypeComboboxItemsAsync`);
    return response.result;
  }
};
