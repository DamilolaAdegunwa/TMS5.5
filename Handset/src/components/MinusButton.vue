<template>
  <button>
    <font-awesome-icon icon="minus" size="sm" />
  </button>
</template>

<script>
export default {
  name: "MinusButton"
};
</script>

<style lang="scss" scoped>
@import "../variables";

button {
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
</style>
