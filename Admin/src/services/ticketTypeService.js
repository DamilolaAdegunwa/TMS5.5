import ajax from "./../utils/ajax.js";

export default {
  async getTicketTypeDescriptionsAsync(input) {
    const response = await ajax.post("/ticketType/GetTicketTypeDescriptionsAsync", input);
    return response.result;
  },
  async getTicketTypeDescriptionAsync(ticketTypeId) {
    const response = await ajax.get(
      `/ticketType/GetTicketTypeDescriptionAsync?ticketTypeId=${ticketTypeId}`
    );
    return response.result;
  },
  async createDescriptionAsync(input) {
    return await ajax.post("/ticketType/CreateDescriptionAsync", input);
  },
  async updateDescriptionAsync(input) {
    return await ajax.post("/ticketType/UpdateDescriptionAsync", input);
  },
  async deleteDescriptionAsync(ticketTypeId) {
    return await ajax.delete(`/ticketType/deleteDescriptionAsync?ticketTypeId=${ticketTypeId}`);
  },
  async getTicketTypeTypeComboboxItemsAsync() {
    const response = await ajax.get("/ticketType/GetTicketTypeTypeComboboxItemsAsync");
    return response.result;
  },
  async getTicketTypeComboboxItemsAsync(ticketTypeTypeId = "") {
    const response = await ajax.get(
      `/ticketType/GetTicketTypeComboboxItemsAsync?ticketTypeTypeId=${ticketTypeTypeId}`
    );
    return response.result;
  },
  async getNetSaleTicketTypeComboboxItemsAsync() {
    const response = await ajax.get("/ticketType/GetNetSaleTicketTypeComboboxItemsAsync");
    return response.result;
  },
  async getTicketTypeClassComboboxItemsAsync() {
    const response = await ajax.get("/ticketType/GetTicketTypeClassComboboxItemsAsync");
    return response.result;
  }
};
