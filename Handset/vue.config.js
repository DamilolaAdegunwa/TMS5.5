module.exports = {
  publicPath: "/handset/",
  productionSourceMap: false,
  css: {
    loaderOptions: {
      less: {
        modifyVars: {
          blue: "#009DDC",
          orange: "#FE5906",
          "button-primary-background-color": "@blue",
          "button-primary-border-color": "@blue"
        }
      }
    }
  },
  devServer: {
    open: true,
    openPage: "login"
  }
};
