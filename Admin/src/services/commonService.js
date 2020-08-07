import ajax from "./../utils/ajax.js";

export default {
  async uploadImageAsync(input) {
    const response = await ajax.post("/common/UploadImageAsync", input);
    return response.result;
  }
};
