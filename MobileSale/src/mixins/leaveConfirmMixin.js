const leaveConfirmMixin = {
  data() {
    return {
      allowLeave: false,
      confirmLeaveTitle: "确认离开？",
      confirmLeaveButtonText: "离开",
      cancelLeaveButtonText: "取消"
    };
  },
  async beforeRouteLeave(to, from, next) {
    if (this.allowLeave || to.meta.allowLeave) {
      next();
      return;
    }

    try {
      await this.$dialog.confirm({
        title: this.confirmLeaveTitle,
        showCancelButton: true,
        confirmButtonText: this.confirmLeaveButtonText,
        cancelButtonText: this.cancelLeaveButtonText
      });

      this.onLeave(next);
    } catch {
      this.onCancelLeave(next);
    }
  },
  methods: {
    onLeave(next) {
      next();
    },
    onCancelLeave(next) {
      next(false);
    }
  }
};

export { leaveConfirmMixin };
