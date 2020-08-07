<template>
  <div :class="{'div-select-list': true, 'select-list-many': groundChangCi.changCiDtos.length > 2}">
    <div class="div-select-title">{{groundChangCi.groundName}}</div>
    <div
      :class="{'div-select-item': true, 'select-item-active': vmodel == changCiDto.id, 'select-item-disabled': changCiDto.disabled}"
      v-for="changCiDto in groundChangCi.changCiDtos"
      :key="changCiDto.displayText"
      @click="onSelect(groundChangCi, changCiDto)"
    >
      <div>
        <div class="select-item-name">{{changCiDto.stime}} - {{changCiDto.etime}}</div>
        <div class="select-item-surplus">余{{changCiDto.surplusNum}}</div>
      </div>
      <div v-if="vmodel == changCiDto.id" class="triangle-right-bottom">
        <van-icon name="xuanzhong" />
      </div>
    </div>
    <div class="div-none-changci" v-if="groundChangCi.changCiDtos.length < 1">当前不可订！</div>
  </div>
</template>

<script>
export default {
  name: "SelectList",
  props: {
    value: {
      type: Number
    },
    groundChangCi: {
      type: Object
    }
  },
  data() {
    return {
      vmodel: 0
    };
  },
  watch: {
    value(val) {
      this.vmodel = val;
    }
  },
  created() {
    this.vmodel = this.value;
  },
  methods: {
    onSelect(groundChangCi, changCiDto) {
      if (!changCiDto.disabled) {
        this.vmodel = changCiDto.id;
        this.$emit("input", changCiDto.id);
        this.$emit("change", {});
      }
    }
  }
};
</script>

<style lang="scss">
.div-select-list {
  display: flex;
  flex-wrap: wrap;
  padding: 8px 10px 3px 10px;
  border-bottom: 1px solid #efefef;
  .div-select-title {
    width: 100%;
    padding: 5px 0px 0px 5px;
    line-height: 7px;
  }
  .div-select-item {
    border: 1px solid #999;
    padding: 3px 0px 3px 0px;
    margin: 8px 3px 8px 3px;
    border-radius: 5px;
    width: calc(33% - 8px);
    font-size: 12px;
    position: relative;
    text-align: center;
    .select-item-name {
      color: #19a0f0;
    }
    .select-item-surplus {
      color: #ff0000;
    }
  }
  .select-item-active {
    border-color: #19a0f0;
  }
  .select-item-disabled{
    border-color: #c8c9cc;
    .select-item-name{
      color: #c8c9cc;
    }
    .select-item-surplus{
      color: #c8c9cc;
    }
  }

  .div-none-changci{
    padding: 15px 10px 10px 10px;
    text-align: center;
    color: #c8c9cc;
  }
}

.select-list-many {
  // justify-content: space-between;
}
</style>