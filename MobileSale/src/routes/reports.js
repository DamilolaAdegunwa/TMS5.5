const ComprehensiveReport = () => import("@/views/reports/ComprehensiveReport.vue");
const PassengerFlowStat = () => import("@/views/reports/PassengerFlowStat.vue");
const TouristStat = () => import("@/views/reports/TouristStat.vue");
const TicketSaleStat = () => import("@/views/reports/TicketSaleStat.vue");
const TradeStat = () => import("@/views/reports/TradeStat.vue");
const TicketCheckStat = () => import("@/views/reports/TicketCheckStat.vue");
import Permissions from "./../permissions.js";

const routes = [
  {
    path: "/ComprehensiveReport",
    name: "ComprehensiveReport",
    component: ComprehensiveReport,
    meta: {
      title: "综合报表",
      permission: Permissions.TMSWeChat_ComprehensiveReport
    }
  },
  {
    path: "/PassengerFlowStat",
    name: "PassengerFlowStat",
    component: PassengerFlowStat,
    meta: {
      title: "客流量分析",
      permission: Permissions.TMSWeChat_PassengerFlowStat
    }
  },
  {
    path: "/touriststat",
    name: "touriststat",
    component: TouristStat,
    meta: {
      title: "游客统计",
      permission: Permissions.TMSWeChat_TouristStat
    }
  },
  {
    path: "/TicketSaleStat",
    name: "TicketSaleStat",
    component: TicketSaleStat,
    meta: {
      title: "售票统计",
      permission: Permissions.TMSWeChat_TicketSaleStat
    }
  },
  {
    path: "/TradeStat",
    name: "TradeStat",
    component: TradeStat,
    meta: {
      title: "收入汇总",
      permission: Permissions.TMSWeChat_TradeStat
    }
  },
  {
    path: "/TicketCheckStat",
    name: "TicketCheckStat",
    component: TicketCheckStat,
    meta: {
      title: "检票统计",
      permission: Permissions.TMSWeChat_TicketCheckStat
    }
  }
];

export default routes;
