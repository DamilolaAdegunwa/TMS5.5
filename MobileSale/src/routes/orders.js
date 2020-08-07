import MyTicket from "@/views/orders/MyTicket.vue";
import Order from "@/views/orders/Order.vue";
import OrderInfo from "@/views/orders/OrderInfo.vue";
import OrderDetail from "./../views/orders/OrderDetail.vue";
import RefundTicket from "./../views/orders/RefundTicket.vue";
import RefundDetail from "./../views/orders/RefundDetail.vue";
import EnrollFace from "./../views/orders/EnrollFace.vue";
import MyFace from "./../views/orders/MyFace.vue";

const routes = [
  {
    path: "/myticket",
    name: "myticket",
    component: MyTicket,
    meta: {
      title: "我的门票"
    }
  },
  {
    path: "/order",
    name: "order",
    component: Order,
    meta: {
      title: "订单",
      showTabbar: true,
      navbarColor: "blue"
    }
  },
  {
    path: "/orderinfo/:listNo",
    name: "orderinfo",
    component: OrderInfo,
    props: true,
    meta: {
      title: "订单详情",
      navbarColor: "blue"
    }
  },
  {
    path: "/orderdetail/:listNo",
    name: "orderdetail",
    component: OrderDetail,
    props: true,
    meta: {
      title: "订单详情",
      navbarColor: "blue"
    }
  },
  {
    path: "/refundticket/:listNo",
    name: "refundticket",
    component: RefundTicket,
    props: true,
    meta: {
      title: "取消订单"
    }
  },
  {
    path: "/refunddetail/:listNo",
    name: "refunddetail",
    component: RefundDetail,
    props: true,
    meta: {
      title: "退款详情"
    }
  },
  {
    path: "/EnrollFace/:listNo",
    name: "EnrollFace",
    component: EnrollFace,
    props: true,
    meta: {
      title: "登记人脸"
    }
  },
  {
    path: "/MyFace",
    name: "myFace",
    component: MyFace,
    props: true,
    meta: {
      title: "登记人脸"
    }
  }
];

export default routes;
