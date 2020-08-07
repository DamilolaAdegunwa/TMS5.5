import ajax from "./../utils/ajax.js";
import file from "./../utils/file.js";
import qs from "qs";

export default {
  async getTicketStatusComboboxItems() {
    const response = await ajax.get("/ticket/GetTicketStatusComboboxItems");
    return response.result;
  },
  async getConsumeTypeComboboxItemsAsync() {
    const response = await ajax.get("/ticket/GetConsumeTypeComboboxItems");
    return response.result;
  },
  async queryTicketSalesToExcelAsync(input) {
    await file.downloadExcelAsync("/ticket/QueryTicketSalesToExcelAsync", qs.stringify(input));
  },
  async queryTicketSalesAsync(input) {
    const response = await ajax.post("/ticket/QueryTicketSalesAsync", qs.stringify(input));
    return response.result;
  },
  async statTicketSaleByPayTypeToExcelAsync(input) {
    await file.downloadExcelAsync(
      "/ticket/StatTicketSaleByPayTypeToExcelAsync",
      qs.stringify(input)
    );
  },
  async statTicketSaleByPayTypeAsync(input) {
    const response = await ajax.post("/ticket/StatTicketSaleByPayTypeAsync", qs.stringify(input));
    return response.result;
  },
  async queryTicketChecksAsync(input) {
    const response = await ajax.post("/ticket/QueryTicketChecksAsync", qs.stringify(input));
    return response.result;
  },
  async statTicketCheckInAsync(input) {
    const response = await ajax.post("/ticket/StatTicketCheckInAsync", qs.stringify(input));
    return response.result;
  },
  async statTicketCheckInToExcelAsync(input) {
    await file.downloadExcelAsync("/ticket/StatTicketCheckInToExcelAsync", qs.stringify(input));
  },
  async statTicketCheckInByDateAndParkAsync(input) {
    const response = await ajax.post(
      "/ticket/StatTicketCheckInByDateAndParkAsync",
      qs.stringify(input)
    );
    return response.result;
  },
  async queryReprintLogAsync(input) {
    const response = await ajax.post("/ticket/QueryReprintLogAsync", qs.stringify(input));
    return response.result;
  },
  async queryExchangeHistoryAsync(input) {
    const response = await ajax.post("/ticket/QueryExchangeHistoryAsync", qs.stringify(input));
    return response.result;
  },
  async statGroundChangCiSaleToExcelAsync(input) {
    await file.downloadExcelAsync("/ticket/StatGroundChangCiSaleToExcelAsync", input);
  },
  async statGroundChangCiSaleAsync(input) {
    const response = await ajax.post("/ticket/StatGroundChangCiSaleAsync", input);
    return response.result;
  },
  async queryTicketConsumesAsync(input) {
    const response = await ajax.post("/ticket/QueryTicketConsumesAsync", qs.stringify(input));
    return response.result;
  },
  async queryTicketConsumesToExcelAsync(input) {
    await file.downloadExcelAsync("/ticket/QueryTicketConsumesToExcelAsync", qs.stringify(input));
  },
  async statTicketConsumeAsync(input) {
    const response = await ajax.post("/ticket/StatTicketConsumeAsync", qs.stringify(input));
    return response.result;
  },
  async statTicketConsumeToExcelAsync(input) {
    await file.downloadExcelAsync("/ticket/StatTicketConsumeToExcelAsync", qs.stringify(input));
  },
  async statTicketSaleAsync(input) {
    const response = await ajax.post("/ticket/StatTicketSaleAsync", qs.stringify(input));
    return response.result;
  },
  async statTicketSaleToExcelAsync(input) {
    await file.downloadExcelAsync("/ticket/StatTicketSaleToExcelAsync", qs.stringify(input));
  },
  async getTicketFullInfoAsync(input) {
    const response = await ajax.post("/ticket/GetTicketFullInfoAsync", input);
    return response.result;
  },
  async getTicketSaleStatTypeComboboxItems() {
    const response = await ajax.get("/ticket/GetTicketSaleStatTypeComboboxItems");
    return response.result;
  },
  // ---------------------------------------------------------------
  async statTicketCheckInAverageAsync(input){
    const response = await ajax.post('/ticket/StatTicketCheckInAverageAsync',input);
    return response.result;
  },
  async statStadiumTicketCheckInAsync(input){
    const response = await ajax.post('/ticket/StatStadiumTicketCheckInAsync',input);
    return response.result;
  },
  async  statTicketCheckByTradeSourceAsync(input){
    const response = await ajax.post('/ticket/StatTicketCheckByTradeSourceAsync',qs.stringify(input));
    return response.result;
  },
  async statTicketCheckInYearOverYearComparisonAsync(input){
    const response = await ajax.post('/ticket/StatTicketCheckInYearOverYearComparisonAsync',input);
    return response.result;
  },
  async statTouristByAgeRangeAsync(input){
    const response = await ajax.post('/ticket/StatTouristByAgeRangeAsync',qs.stringify(input));
    return response.result;
  },
  async statTouristByAreaAsync(input){
    const response = await ajax.post('/ticket/StatTouristByAreaAsync',qs.stringify(input));
    return response.result;
  },
  async statTouristBySexAsync(input){
    const response = await ajax.post('/ticket/StatTouristBySexAsync',qs.stringify(input));
    return response.result;
  },
  //--------------------ToExcel---------------
  async statTicketCheckInAverageToExcelAsync(input){
    await file.downloadExcelAsync('/ticket/StatTicketCheckInAverageToExcelAsync',input);
  },
  async statStadiumTicketCheckInToExcelAsync(input){
    await file.downloadExcelAsync('/ticket/StatStadiumTicketCheckInToExcelAsync',input);
  },
  async  statTicketCheckByTradeSourceToExcelAsync(input){
    await file.downloadExcelAsync('/ticket/StatTicketCheckByTradeSourceToExcelAsync',input);
  },
  async statTicketCheckInYearOverYearComparisonToExcelAsync(input){
    await file.downloadExcelAsync('/ticket/StatTicketCheckInYearOverYearComparisonToExcelAsync',input);
  },
  async statTouristByAgeRangeToExcelAsync(input){
    await file.downloadExcelAsync('/ticket/StatTouristByAgeRangeToExcelAsync',input);
  },
  async statTouristByAreaToExcelAsync(input){
    await file.downloadExcelAsync('/ticket/StatTouristByAreaToExcelAsync',input);
  },
  async statTouristBySexToExcelAsync(input){
    await file.downloadExcelAsync('/ticket/StatTouristBySexToExcelAsync',input);
  },

};
