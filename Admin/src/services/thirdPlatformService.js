import ajax from "./../utils/ajax.js";

export default {
  async createAsync(input) {
    return await ajax.post("/ThirdPlatform/CreateAsync", input);
  },
  async updateAsync(input) {
    return await ajax.put("/ThirdPlatform/UpdateAsync", input);
  },
  async deleteAsync(id) {
    return await ajax.delete(`/ThirdPlatform/DeleteAsync?id=${id}`);
  },
  async getThirdPlatformsAsync(uid) {
    const response = await ajax.get(`/ThirdPlatform/GetThirdPlatformsAsync?uid=${uid}`);
    return response.result;
  },
  async getThirdPlatformForEditAsync(id) {
    const response = await ajax.get(`/ThirdPlatform/GetThirdPlatformForEditAsync?id=${id}`);
    return response.result;
  },
  async getPlatformTypeComboboxItemsAsync() {
    const response = await ajax.get("/ThirdPlatform/GetPlatformTypeComboboxItems");
    return response.result;
  }
};
