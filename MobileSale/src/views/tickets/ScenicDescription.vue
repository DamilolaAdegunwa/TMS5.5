<template>
  <van-popup v-model="show" position="bottom">
    <div class="description">
      <div class="description-title">
        <div>{{ scenic.scenicName }}</div>
        <div class="description-close-btn" @click="onClose">
          <van-icon name="cuowu" />
        </div>
      </div>
      <div style="height: 100%; overflow-y: auto;">
        <div class="description-content">
          <div style="height:40px;">&nbsp;</div>
          <div v-if="scenic.openTime && scenic.closeTime" class="description-content-item">
            <h3>开放时间</h3>
            <div class="description-content-item-text">
              {{ scenic.openTime }}至{{ scenic.closeTime }}
            </div>
          </div>
          <div v-if="scenic.scenicIntro" class="description-content-item">
            <h3>{{pageLabelMainText}}简介</h3>
            <div class="description-content-item-text" v-html="scenic.scenicIntro"></div>
          </div>
          <div v-if="scenic.scenicFeature" class="description-content-item">
            <h3>{{pageLabelMainText}}特色</h3>
            <div class="description-content-item-text" v-html="scenic.scenicFeature"></div>
          </div>
        </div>
      </div>
    </div>
  </van-popup>
</template>

<script>
import { mobileMixin } from "./../../mixins/mobileMixin.js";
import { mapState } from "vuex";

export default {
  name: "ScenicDescription",
  mixins: [mobileMixin],
  props: {
    scenic: {},
    value: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      show: this.value,
      imageNormalized: false
    };
  },
  watch: {
    value(val) {
      this.show = val;
      if (val && !this.imageNormalized) {
        this.$nextTick(() => {
          this.normalizeImage(".description-content-item");
          this.imageNormalized = true;
        });
      }
    },
    show(val) {
      if (!val) {
        this.$emit("input", val);
      }
    }
  },
  computed: {
    ...mapState(["pageLabelMainText"])
  },
  methods: {
    onClose() {
      this.show = false;
    }
  }
};
</script>

<style lang="scss" scoped>
.description {
  background-color: #fff;
  transition: all 0.3s ease;
  transform: translateY(0);

  &-title {
    left: 0;
    top: 0;
    width: 100%;
    z-index: 100;
    position: fixed;
    height: 40px;
    line-height: 40px;
    font-size: 18px;
    text-align: center;
    background-color: #efefef;
  }

  &-close-btn {
    right: 0;
    top: 0;
    position: absolute;
    color: #999;
    width: 40px;
    height: 40px;
  }

  &-content {
    position: relative;
    font-size: 15px;
    height: 84vh;
    overflow-y: auto;
    box-sizing: border-box;

    &-item {
      margin-left: 15px;
      padding: 15px 15px 15px 0;
      word-wrap: break-word;
      word-break: normal;
      border-bottom: 1px solid #dbdbdb;

      h3 {
        color: #333;
        font-size: 100%;
        font-weight: 700;
        padding-bottom: 5px;
      }

      &-text {
        margin-left: 15px;
      }

      /deep/ img {
        max-width: 100% !important;
      }
    }
  }
}
</style>
