import ajax from "./../utils/ajax.js";

export default {
  async getTicketTypesForLocalSaleAsync(input) {
    const response = await ajax.post(
      "/TicketType/GetTicketTypesForLocalSaleAsync",
      input
    );
    return response;
  }
};
