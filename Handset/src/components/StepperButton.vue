<template>
  <div class="button-group">
    <button class="minus-button" @click="onMinusClick">
      <font-awesome-icon icon="minus" size="sm" />
    </button>
    <span class="quantity">{{ currentValue }}</span>
    <button class="plus-button" @click="onPlusClick">
      <font-awesome-icon icon="plus" size="sm" />
    </button>
  </div>
</template>

<script>
export default {
  name: "StepperButton",
  props: {
    minValue: {
      type: Number,
      default: -99
    },
    maxValue: {
      type: Number,
      default: 99
    },
    defaultValue: {
      type: Number,
      default: 0
    },
    value: {
      type: Number
    }
  },

  computed: {
    currentValue() {
      if (this.value) {
        return this.value;
      }
      return this.defaultValue;
    }
  },
  data() {
    return {
      num: 0
    };
  },
  methods: {
    onFocus() {
      document.activeElement.blur();
    },
    onMinusClick() {
      this.num = this.currentValue;
      if (this.num <= this.minValue) {
        this.num = this.minValue;
      } else {
        this.num--;
      }
      this.$emit("input", this.num);
    },
    onPlusClick() {
      this.num = this.currentValue;
      if (this.num >= this.maxValue) {
        this.num = this.maxValue;
      } else {
        this.num++;
      }
      this.$emit("input", this.num);
    }
  }
};
</script>

<style lang="scss" scoped>
@import "../variables";

.minus-button {
  display: flex;
  justify-content: center;
  align-items: center;
  width: $round-button-size;
  height: $round-button-size;
  border-radius: $round-button-size / 2;
  padding: 0;

  color: #666666;
  background: white;
  border: 1px solid #666666;
}
.plus-button {
  display: flex;
  justify-content: center;
  align-items: center;
  width: $round-button-size;
  height: $round-button-size;
  border-radius: $round-button-size / 2;
  padding: 0;

  color: white;
  background: $blue;
  border: 1px solid $blue;
}
.field-button {
  max-width: 50px;
  max-height: 30px;
  text-align: center;
  text-align-last: center;
  font-size: 24;
}
.button-group {
  $button-size: 24px;

  display: flex;
  align-items: center;
  justify-content: flex-end;

  // 当前数量
  .quantity {
    display: inline-block;
    text-align: center;
    min-width: 30px;
    vertical-align: center;
  }
}
</style>
