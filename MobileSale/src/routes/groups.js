import ChangePassword from "@/views/groups/ChangePassword.vue";
import GroupLogin from "@/views/groups/GroupLogin.vue";
import GroupRegist from "@/views/groups/GroupRegist.vue";
import GroupRegistSuccess from "@/views/groups/GroupRegistSuccess.vue";

const routes = [
  {
    path: "/changepassword",
    name: "changepassword",
    component: ChangePassword,
    meta: {
      title: "修改密码"
    }
  },
  {
    path: "/grouplogin/:shouldBind/:nextPath",
    name: "grouplogin",
    component: GroupLogin,
    props: true,
    meta: {
      title: "团队登录"
    }
  },
  {
    path: "/groupregist",
    name: "groupregist",
    component: GroupRegist,
    meta: {
      title: "团队注册"
    }
  },
  {
    path: "/groupregistsuccess",
    name: "groupregistsuccess",
    component: GroupRegistSuccess,
    meta: {
      title: "注册成功"
    }
  }
];

export default routes;
