import ajax from "./../utils/ajax.js";
import qs from "qs";

export default {
  async checkTicketAsync(input) {
    const response = await ajax.post(
      "/Ticket/CheckTicketFromHandsetAsync",
      input
    );
    return response;
  },
  async printAsync(input) {
    const response = await ajax.post("/Ticket/PrintAsync", input);
    return response;
  },
  async RefundFromHandsetAsync(input) {
    const response = await ajax.post("/Ticket/RefundFromHandsetAsync", input);
    return response;
  },
  async GetTicketFullInfoAsync(input) {
    const response = await ajax.post("/Ticket/GetTicketFullInfoAsync", input);
    return response;
  },
  async statTicketCheckInByTicketTypeAsync(input) {
    const response = await ajax.post(
      "/ticket/StatTicketCheckInByTicketTypeAsync",
      qs.stringify(input)
    );
    return response.result;
  },
  async statCashierSaleAsync(input) {
    const response = await ajax.post("/Ticket/StatCashierSaleAsync", input);
    return response.result;
  },
  async statTicketSaleByTradeSourceAsync(input) {
    const response = await ajax.post(
      "/Ticket/StatTicketSaleByTradeSourceAsync",
      qs.stringify(input)
    );
    return response.result;
  },
  async getLastCheckTicketInfoAsync(input) {
    const response = await ajax.post(
      "/Ticket/GetLastCheckTicketInfoAsync",
      input
    );
    return response.result;
  },
  async rePrintAsync(input) {
    const response = await ajax.post("/Ticket/RePrintAsync", input);
    return response.result;
  }
};
