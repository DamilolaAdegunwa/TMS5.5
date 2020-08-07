<template>
  <div class="page">
    <img class="banner" src="../assets/imgs/banner.png" />

    <div class="content">
      <h1>请选择项目</h1>
      <h5>PLEASE SELECT ITEM</h5>

      <van-row gutter="24">
        <van-col span="9" offset="3">
          <img
            class="img-button"
            src="../assets/imgs/check-button.png"
            @click="gotoCheck"
          />
        </van-col>
        <van-col span="9">
          <img
            class="img-button"
            src="../assets/imgs/sale-button.png"
            @click="gotoSale"
          />
        </van-col>
      </van-row>
    </div>
    <TabBar />
  </div>
</template>

<script>
import TabBar from "../components/TabBar";

export default {
  name: "Home",
  components: { TabBar },
  data() {
    return {
      permissions: []
    };
  },
  async created() {
    this.loadData();
  },
  methods: {
    loadData() {
      let temp = sessionStorage.getItem("permissions");
      this.permissions = JSON.parse(temp);
    },
    gotoCheck() {
      if (
        this.permissions.includes(
          "4E31FD2B-78D1-4C61-AE8C-1C04514469F9".toLocaleLowerCase()
        )
      ) {
        this.$router.push("/check");
      } else {
        this.$toast("没有访问权限");
      }
    },
    gotoSale() {
      if (
        this.permissions.includes(
          "4C971497-69DA-41A8-9EC0-1A2AC7689BE5".toLocaleLowerCase()
        )
      ) {
        this.$router.push("/sale");
      } else {
        this.$toast("没有访问权限");
      }
    },
    gotoAccount() {
      this.$router.push("/account");
    }
  }
};
</script>

<style lang="scss" scoped>
.page {
  background-color: #eef9ff;
}

.banner {
  width: 100%;
}

.content {
  text-align: center;
  padding-top: 50px;

  h1 {
    color: #404040;
    letter-spacing: 3px;
    font-weight: normal;
    font-size: 30px;
    margin-bottom: 0;
  }

  h5 {
    color: #404040;
    letter-spacing: 0.5px;
    font-weight: normal;
    font-family: Roboto, sans-serif;
  }

  .van-row {
    padding-top: 30px;
    margin-left: 0 !important;
    margin-right: 0 !important;
  }

  .img-button {
    max-width: 100%;
  }
}
</style>
