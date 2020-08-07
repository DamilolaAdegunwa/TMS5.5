import BindStaff from "@/views/staffs/BindStaff.vue";
const GroupCheckTicket = () => import("@/views/staffs/GroupCheckTicket.vue");
const GroupExplain = () => import("@/views/staffs/GroupExplain.vue");
import Permissions from "@/permissions.js";

const routes = [
  {
    path: "/bindstaff",
    name: "bindstaff",
    component: BindStaff,
    meta: {
      title: "员工绑定"
    }
  },
  {
    path: "/groupcheckticket",
    name: "groupcheckticket",
    component: GroupCheckTicket,
    meta: {
      title: "团队检票",
      permission: Permissions.TMSWeChat_GroupCheck
    }
  },
  {
    path: "/groupexplain",
    name: "groupexplain",
    component: GroupExplain,
    meta: {
      title: "团队讲解",
      permission: Permissions.TMSWeChat_GroupExplain
    }
  }
];

export default routes;
