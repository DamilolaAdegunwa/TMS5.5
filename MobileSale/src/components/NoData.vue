<template>
  <div class="no-data">
    <van-row type="flex" justify="center" align="center" :style="{ height: contentHeight + 'px' }">
      <van-col>
        <van-icon name="wushuju" class="no-data-icon" />
        <div class="no-data-text">暂无数据</div>
      </van-col>
    </van-row>
  </div>
</template>

<script>
export default {
  name: "no-data",
  props: {
    contentHeight: {
      type: Number,
      default: 500
    }
  }
};
</script>

<style lang="scss">
.no-data {
  color: dimgrey;
  text-align: center;

  &-icon {
    font-size: 40px;
  }

  &-text {
    font-size: 13px;
    margin-top: 5px;
  }
}
</style>
