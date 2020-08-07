import { pagedViewMixin } from "./pagedViewMixin.js";

const pagedAndEditViewMixin = {
  mixins: [pagedViewMixin],
  data() {
    return {
      currentId: "",
      isAdd: true,
      showEditDialog: false
    };
  },
  methods: {
    add() {
      this.isAdd = true;
      this.showEditDialog = true;
    },
    update(id) {
      this.currentId = id;
      this.isAdd = false;
      this.showEditDialog = true;
    },
    async remove(id) {
      try {
        await this.$confirm("确定删除此项?", "提示", {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning"
        });
        await this.delete(id);
        this.query();
      } catch (error) {
        return;
      }
    },
    async delete() {}
  }
};

export { pagedAndEditViewMixin };
