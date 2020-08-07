import MemberCard from "@/views/members/MemberCard.vue";
import MemberCenter from "@/views/members/MemberCenter.vue";
import RegistMemberCard from "@/views/members/RegistMemberCard.vue";

const routes = [
  {
    path: "/membercard",
    name: "membercard",
    component: MemberCard,
    meta: {
      title: "电子会员码"
    }
  },
  {
    path: "/my",
    name: "my",
    component: MemberCenter,
    meta: {
      title: "我的",
      navbarColor: "blue",
      showTabbar: true
    }
  },
  {
    path: "/registmembercard",
    name: "registmembercard",
    component: RegistMemberCard,
    meta: {
      title: "注册散客会员"
    }
  }
];

export default routes;
