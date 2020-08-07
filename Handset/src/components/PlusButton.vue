<template>
  <button>
    <font-awesome-icon icon="plus" size="sm" />
  </button>
</template>

<script>
export default {
  name: "PlusButton"
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

  color: white;
  background: $blue;
  border: 1px solid $blue;
}
</style>
