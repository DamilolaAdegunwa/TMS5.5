<template>
  <div>
    <van-row>
      <van-col>
        <div
          class="content"
          :style="{
            height: contentHeight + 'px',
            minWidth: contentWidth + 'px'
          }"
        >
          <div class="content-title">预约须知</div>
          <div v-if="notice.timeNotice" class="content-item-title">
            开馆时间：
          </div>
          <p v-if="notice.timeNotice" class="content-text" v-html="notice.timeNotice"></p>
          <div v-if="notice.orderNotice" class="content-item-title">
            预约须知
          </div>
          <p v-if="notice.orderNotice" class="content-text" v-html="notice.orderNotice"></p>
          <div v-if="notice.contactNotice" class="content-item-title">
            联系信息
          </div>
          <p v-if="notice.contactNotice" class="content-text" v-html="notice.contactNotice"></p>
        </div>
      </van-col>
    </van-row>

    <van-row type="flex" justify="space-between" class="button-box">
      <van-col span="7">
        <div class="button-box-button" @click="createOrder(false)">
          <div>个人</div>
          <div class="tips">(成人)</div>
        </div>
      </van-col>
      <van-col span="7">
        <div class="button-box-button" @click="createOrder(true)">
          <div>家庭</div>
          <div class="tips">(携带儿童)</div>
        </div>
      </van-col>
      <van-col span="7">
        <div class="button-box-button" style="line-height: 38px;" @click="createGroupOrder">
          <div>团队</div>
        </div>
      </van-col>
    </van-row>
  </div>
</template>

<script>
import { mapState, mapMutations, mapActions } from "vuex";
import appConsts from "@/store/consts.js";
import memberService from "./../../services/memberService.js";

export default {
  name: "home",
  data() {
    return {
      clientWidth: 375,
      clientHeight: 667
    };
  },
  computed: {
    contentWidth() {
      return this.clientWidth - 60;
    },
    contentHeight() {
      return this.clientHeight - 200;
    },
    member() {
      return memberService.getMember();
    },
    ...mapState({
      notice: state => state.homeModule.notice
    })
  },
  methods: {
    createOrder(addChildren) {
      this.$router.push({
        name: "createorder",
        query: { addChildren: addChildren }
      });
    },
    createGroupOrder() {
      if (this.member.customerId) {
        this.$router.push("/creategrouporder");
      } else {
        this.$router.push({
          name: "grouplogin",
          params: { shouldBind: "0", nextPath: "creategrouporder" }
        });
      }
    },
    resize() {
      this.clientWidth = document.documentElement.clientWidth;
      this.clientHeight = document.documentElement.clientHeight;
    },
    ...mapMutations(["setTitle", "setShowNavbarLeft", "setShowNavbarRight"]),
    ...mapActions("homeModule", [appConsts.loadOrderNoticeAsync])
  },
  async created() {
    await this.loadOrderNoticeAsync();

    this.setTitle(this.notice.scenicName);
  },
  mounted() {
    this.resize();
    let vm = this;
    window.onresize = function() {
      vm.resize();
    };
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      if (from.name == "login") {
        vm.setShowNavbarLeft(false);
      }
      vm.setShowNavbarRight(false);
    });
  },
  beforeDestroy() {
    this.setShowNavbarLeft(true);
    this.setShowNavbarRight(true);
    window.onresize = null;
  }
};
</script>

<style lang="scss" scoped>
.content {
  margin: 10px 10px 20px 10px;
  padding: 0 20px;
  background-color: white;
  border-radius: 5%;
  overflow: scroll;

  &-title {
    text-align: center;
    padding-top: 25px;
    margin-bottom: 25px;
    font-weight: bold;
  }

  &-item-title {
    margin-bottom: 10px;
    font-weight: bold;
  }

  &-text {
    font-size: 13px;
  }
}

.content::-webkit-scrollbar {
  display: none;
}

.button-box {
  text-align: center;
  margin: 0 20px;

  &-button {
    padding: 5px 0;
    color: white;
    background-color: #19a0f0;
    border: 1px solid #19a0f0;
    border-radius: 3px;
    height: 38px;

    .tips {
      font-size: 12px;
    }
  }
}
</style>
