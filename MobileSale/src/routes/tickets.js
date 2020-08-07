import TicketType from "@/views/tickets/TicketType.vue";
import BuyTicket from "@/views/tickets/BuyTicket.vue";
import CheckTicket from "@/views/tickets/CheckTicket.vue";
import QueryTicket from "@/views/tickets/QueryTicket.vue";
import Permissions from "./../permissions.js";

const routes = [
  {
    path: "/tickettype/:publicSaleFlag",
    name: "tickettype",
    component: TicketType,
    props: true,
    meta: {
      title: "购票",
      showNavbar: false,
      showTabbar: true
    }
  },
  {
    path: "/buyticket/:ticketTypeId",
    name: "buyticket",
    component: BuyTicket,
    props: true,
    meta: {
      title: "提交订单"
    }
  },
  {
    path: "/CheckTicket",
    name: "CheckTicket",
    component: CheckTicket,
    meta: {
      title: "门票核销",
      permission: Permissions.TMSWeChat_CheckTicket
    }
  },
  {
    path: "/QueryTicket",
    name: "QueryTicket",
    component: QueryTicket,
    meta: {
      title: "门票查询",
      permission: Permissions.TMSWeChat_QueryTicket
    }
  }
];

export default routes;
