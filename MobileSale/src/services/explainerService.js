import ajax from "@/utils/ajax.js";

export default {
  async getSchedulingsAsync(input) {
    let response = await ajax.post("/explainer/GetSchedulingsAsync", input);
    return response.result;
  },
  async beginExplainAsync(input) {
    return await ajax.post("/explainer/BeginExplainAsync", input);
  },
  async completeExplainAsync(input) {
    return await ajax.post("explainer/CompleteExplainAsync", input);
  }
};
