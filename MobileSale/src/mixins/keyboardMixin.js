import { mobileMixin } from "./mobileMixin.js";

const keyboardMixin = {
  mixins: [mobileMixin],
  data() {
    return {
      clientHeight: 0
    };
  },
  mounted() {
    document.addEventListener("focus", this.onFocus, true);
    document.addEventListener("blur", this.onBlur, true);
    this.clientHeight = document.documentElement.clientHeight || document.body.clientHeight;
    window.addEventListener("resize", this.onResize, true);
  },
  beforeDestroy() {
    document.removeEventListener("focus", this.onFocus, true);
    document.removeEventListener("blur", this.onBlur, true);
    window.removeEventListener("resize", this.onResize, true);
  },
  methods: {
    onFocus(e) {
      if (["input", "textarea"].includes(e.target.localName)) {
        this.keyboardPopup();
      }
    },
    onBlur(e) {
      if (["input", "textarea"].includes(e.target.localName)) {
        if (this.isIphone()) {
          this.keyboardPopdown();
        }
      }
    },
    onResize() {
      const resizeHeight = document.documentElement.clientHeight || document.body.clientHeight;
      if (resizeHeight >= this.clientHeight) {
        if (!this.isIphone()) {
          this.keyboardPopdown();
        }
      }
    },
    keyboardPopup() {
      document.activeElement.scrollIntoViewIfNeeded(true);
    },
    keyboardPopdown() {
      document.activeElement.scrollIntoViewIfNeeded(true);
      if (this.isIphone()) {
        document.body.scrollIntoView(false);
      }
    }
  }
};

export { keyboardMixin };
