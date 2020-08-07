import ajax from "./../utils/ajax.js";

export default {
  async getMemberComboboxItemsAsync() {
    const response = await ajax.get("/member/GetMemberComboboxItemsAsync");
    return response.result;
  }
};
