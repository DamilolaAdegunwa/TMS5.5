import dayjs from "dayjs";

const ticketTypeMixin = {
  methods: {
    getTravelDateText(ticketType) {
      const today = dayjs();
      const tomorrow = dayjs().addDays(1);
      const startTravelDate = dayjs(ticketType.startTravelDate);
      let travelDateText = "";
      if (startTravelDate.isSameOrBefore(today)) {
        travelDateText = "今日";
      } else if (startTravelDate.isSameOrBefore(tomorrow)) {
        travelDateText = "明日";
      } else {
        travelDateText = startTravelDate.format("MM月DD日");
      }

      return `最早可订${travelDateText}票`;
    },
    getRefundText(ticketType) {
      if (ticketType.allowRefund === false) {
        return "不可退";
      }

      return ticketType.refundLimited ? "有条件退" : "随时退";
    }
  }
};

export { ticketTypeMixin };
