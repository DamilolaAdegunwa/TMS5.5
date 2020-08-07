import CreateGroupOrder from "@/views/book/CreateGroupOrder.vue";
import CreateOrder from "@/views/book/CreateOrder.vue";
import Book from "@/views/book/Book.vue";

const routes = [
  {
    path: "/creategrouporder",
    name: "creategrouporder",
    component: CreateGroupOrder,
    meta: {
      title: "团队预约"
    }
  },
  {
    path: "/createorder",
    name: "createorder",
    component: CreateOrder,
    meta: {
      title: "散客领票"
    }
  },
  {
    path: "/book",
    name: "book",
    component: Book,
    meta: {
      title: "预约",
      showTabbar: true
    }
  }
];

export default routes;
