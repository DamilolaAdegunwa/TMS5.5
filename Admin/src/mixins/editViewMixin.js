const editViewMixin = {
  props: {
    value: {
      type: Boolean,
      default: false
    },
    isAdd: {
      type: Boolean,
      default: true
    },
    id: [Number, String]
  },
  data() {
    return {
      showDialog: this.value,
      saving: false
    };
  },
  computed: {
    editText() {
      return this.isAdd ? "添加" : "修改";
    }
  },
  watch: {
    value(val) {
      this.showDialog = val;

      if (val && !this.isAdd) {
        this.getDataForEdit();
      }
    }
  },
  methods: {
    getDataForEdit() {},
    save() {
      this.$refs.form.validate(async valid => {
        if (!valid) {
          return false;
        }

        try {
          this.saving = true;
          const result = await this.edit();
          if (result.success) {
            this.close();
            this.$message.success("保存成功");
            this.$emit("saved");
          }
        } catch (err) {
          return;
        } finally {
          this.saving = false;
        }
      });
    },
    async edit() {
      if (this.isAdd) {
        return await this.create();
      } else {
        return await this.update();
      }
    },
    async create() {},
    async update() {},
    close() {
      this.showDialog = false;
    },
    onClosed() {
      this.clear();
      this.$refs.form.resetFields();
      this.$emit("input", this.showDialog);
    },
    clear() {}
  }
};

export { editViewMixin };
