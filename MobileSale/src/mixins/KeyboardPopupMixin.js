const keyboardPopupMixin = {
  mounted() {
    let clientHeight = document.documentElement.clientHeight;
    let vm = this;
    window.onresize = function() {
      let nowClientHeight = document.documentElement.clientHeight;
      vm.isKeyboardPopup = clientHeight > nowClientHeight;

      if (vm.isKeyboardPopup) {
        let top = 0;
        let element = document.activeElement;
        while (element) {
          top += element.offsetTop;

          element = element.offsetParent;
        }
        window.scrollTo(0, top - 100);
      }
    };
  },
  beforeDestroy() {
    window.onresize = null;
  },
  methods: {
    onSubPopupChange(show, height) {
      height -= 50;
      if (show) {
        this.$refs.grbody.style.height = `${this.$refs.grbody.clientHeight + height}px`;
        window.scrollTo(0, window.scrollY + height);
      } else {
        window.scrollTo(0, window.scrollY - height);
        this.$refs.grbody.style.height = `${this.$refs.grbody.clientHeight - height}px`;
      }
    }
  }
};

export { keyboardPopupMixin };
