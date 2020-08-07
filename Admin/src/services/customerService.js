import ajax from "./../utils/ajax.js";

export default {
  async getCustomerComboboxItemsAsync() {
    const response = await ajax.get("/customer/GetCustomerComboboxItemsAsync");
    return response.result;
  },
  async getConsumeCustomerComboBoxItemsAsync(){
    const response = await ajax.get("/customer/GetConsumeCustomerComboBoxItemsAsync");
    return response.result; 
  }
};
