import ajax from "./../utils/ajax.js";

export default {
  async getScenicAsync(language) {
    const response = await ajax.get(`/scenic/GetScenicAsync?language=${language}`);
    return response.result;
  },
  async editScenicAsync(input) {
    return await ajax.post("/scenic/EditScenicAsync", input);
  },
  async getScenicOptions() {
    const response = await ajax.get("/scenic/GetScenicOptions");
    return response.result;
  },
  async getParkComboboxItemsAsync() {
    const response = await ajax.get("/scenic/GetParkComboboxItemsAsync");
    return response.result;
  },
  async getGroundComboboxItemsAsync() {
    const response = await ajax.get("/scenic/GetGroundComboboxItemsAsync");
    return response.result;
  },
  async getGateGroupComboboxItemsAsync(groundId = "") {
    const response = await ajax.get(`/scenic/GetGateGroupComboboxItemsAsync?groundId=${groundId}`);
    return response.result;
  },
  async getSalePointComboboxItemsAsync() {
    const response = await ajax.get("/scenic/GetSalePointComboboxItemsAsync");
    return response.result;
  },
  async getGateComboBoxItemsAsync() {
    const response = await ajax.get(`/scenic/GetGateComboBoxItemsAsync`);
    return response.result;
  },
  async setScenicObject() {
    let response = await ajax.get("/scenic/GetScenicObject");
    let scenicObjectDto = response.result;
    if(!scenicObjectDto){
      scenicObjectDto = {};
    }
    if(!scenicObjectDto.pageLabelMainText){
      scenicObjectDto.pageLabelMainText = "景区";
    }
    sessionStorage.setItem("pageLabelMainText", scenicObjectDto.pageLabelMainText);
    sessionStorage.setItem("scenicName", scenicObjectDto.scenicName);
    sessionStorage.setItem("scenicObject", scenicObjectDto.scenicObject);
    sessionStorage.setItem("copyright", scenicObjectDto.copyright);
  },
  getPageLabelMainText() {
    return sessionStorage.getItem("pageLabelMainText");
  },
  getScenicName(){
    return sessionStorage.getItem("scenicName");
  },
  getScenicObject(){
    return sessionStorage.getItem("scenicObject");
  },
  getCopyright(){
    return sessionStorage.getItem("copyright");
  }
};
