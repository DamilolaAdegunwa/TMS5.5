import WxJsPay from "./../views/payment/WxJsPay.vue";

const routes = [
  {
    path: "/WxJsPay/:listNo",
    name: "wxjspay",
    component: WxJsPay,
    props: true,
    meta: {
      title: "支付订单"
    }
  }
];

export default routes;
