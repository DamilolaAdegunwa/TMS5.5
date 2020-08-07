const mobileMixin = {
  data() {
    return {
      isLoading: false,
      pageLoaded: false
    };
  },
  beforeRouteLeave(to, from, next) {
    this.$toast.clear();
    next();
  },
  methods: {
    isIphone() {
      return /iphone|ipod|ipad/i.test(navigator.appVersion);
    },
    loading() {
      this.$toast.loading({
        duration: 0,
        message: "加载中..."
      });
      this.isLoading = true;
    },
    loaded() {
      this.$toast.clear();
      this.isLoading = false;
    },
    normalizeImage(selectors) {
      let images = document.querySelectorAll(`${selectors} img`);
      for (let image of images) {
        image.removeAttribute("width");
        image.removeAttribute("height");
      }
    }
  }
};

export { mobileMixin };
