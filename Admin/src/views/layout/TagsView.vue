<template>
  <div class="tags-view-container">
    <scroll-panel ref="scrollPane" class="tags-view-wrapper">
      <router-link
        v-for="tag in visitedViews"
        ref="tag"
        :class="{'active':isActive(tag)}"
        :to="{ path: tag.path, query: tag.query, fullPath: tag.fullPath }"
        :key="tag.path"
        tag="span"
        class="tags-view-item"
        @click.middle.native="closeSelectedTag(tag)"
        @contextmenu.prevent.native="openMenu(tag,$event)"
      >
        {{ tag.title }}
        <span class="el-icon-close" @click.prevent.stop="closeSelectedTag(tag)"/>
      </router-link>
    </scroll-panel>
    <ul v-if="showContextMenu" :style="{left:left+'px',top:top+'px'}" class="contextmenu">
      <li @click="refreshSelectedTag(selectedTag)">刷新</li>
      <li @click="closeSelectedTag(selectedTag)">关闭</li>
      <li @click="closeOthersTags">关闭其他</li>
      <li @click="closeAllTags">关闭全部</li>
    </ul>
  </div>
</template>

<script>
import { mapState, mapMutations, mapActions } from "vuex";
import { modules, tagsView } from "./../../store/types.js";
import ScrollPanel from "./../../components/ScrollPanel.vue";

export default {
  name: "tags-view",
  components: { ScrollPanel },
  data() {
    return {
      showContextMenu: false,
      top: 0,
      left: 0,
      selectedTag: {}
    };
  },
  computed: {
    ...mapState(modules.tagsView, [tagsView.visitedViews])
  },
  watch: {
    $route() {
      this.addViewTags();
      this.moveToCurrentTag();
    },
    showContextMenu(value) {
      if (value) {
        document.body.addEventListener("click", this.closeMenu);
      } else {
        document.body.removeEventListener("click", this.closeMenu);
      }
    }
  },
  mounted() {
    this.addViewTags();
  },
  methods: {
    isActive(route) {
      return route.path === this.$route.path;
    },
    addViewTags() {
      const { name } = this.$route;
      if (name) {
        this.addView(this.$route);
      }
    },
    moveToCurrentTag() {
      const tags = this.$refs.tag;
      this.$nextTick(() => {
        for (const tag of tags) {
          if (tag.to.path === this.$route.path) {
            this.$refs.scrollPane.moveToTarget(tag);

            // when query is different then update
            if (tag.to.fullPath !== this.$route.fullPath) {
              this.updateVisitedView(this.$route);
            }

            break;
          }
        }
      });
    },
    refreshSelectedTag(view) {
      this.deleteCachedView(view);
      const { fullPath } = view;
      this.$nextTick(() => {
        this.$router.replace({
          path: fullPath
        });
      });
    },
    closeSelectedTag(view) {
      this.deleteView(view);
      if (this.isActive(view)) {
        const latestView = this.visitedViews.slice(-1)[0];
        if (latestView) {
          this.$router.push(latestView);
        } else {
          this.$router.push("/");
        }
      }
    },
    closeOthersTags() {
      this.$router.push(this.selectedTag);
      this.deleteOthersView(this.selectedTag);
      this.moveToCurrentTag();
    },
    closeAllTags() {
      this.deleteAllView();
      this.$router.push("/");
    },
    openMenu(tag, e) {
      const menuMinWidth = 105;
      const offsetLeft = this.$el.getBoundingClientRect().left; // container margin left
      const offsetWidth = this.$el.offsetWidth; // container width
      const maxLeft = offsetWidth - menuMinWidth; // left boundary
      const left = e.clientX - offsetLeft + 15; // 15: margin right

      if (left > maxLeft) {
        this.left = maxLeft;
      } else {
        this.left = left;
      }
      this.top = e.clientY;

      this.showContextMenu = true;
      this.selectedTag = tag;
    },
    closeMenu() {
      this.showContextMenu = false;
    },
    ...mapMutations(modules.tagsView, [tagsView.deleteCachedView, tagsView.updateVisitedView]),
    ...mapActions(modules.tagsView, [
      tagsView.addView,
      tagsView.deleteView,
      tagsView.deleteOthersView,
      tagsView.deleteAllView
    ])
  }
};
</script>

<style lang="scss" scoped>
@import "./../../styles/variables.scss";

.tags-view-container {
  height: $tags-height - 1px;
  width: 100%;
  background: #fff;
  border-bottom: 1px solid #d8dce5;
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.12), 0 0 3px 0 rgba(0, 0, 0, 0.04);
  .tags-view-wrapper {
    .tags-view-item {
      display: inline-block;
      position: relative;
      cursor: pointer;
      height: 26px;
      line-height: 26px;
      border: 1px solid #d8dce5;
      color: #495060;
      background: #fff;
      padding: 0 8px;
      font-size: 12px;
      margin-left: 5px;
      margin-top: 4px;
      &:first-of-type {
        margin-left: 15px;
      }
      &:last-of-type {
        margin-right: 15px;
      }
      &.active {
        background-color: $blue;
        color: #fff;
        border-color: $blue;
        &::before {
          content: "";
          background: #fff;
          display: inline-block;
          width: 8px;
          height: 8px;
          border-radius: 50%;
          position: relative;
          margin-right: 2px;
        }
      }
    }
  }
  .contextmenu {
    margin: 0;
    background: #fff;
    z-index: 100;
    position: absolute;
    list-style-type: none;
    padding: 5px 0;
    border-radius: 4px;
    font-size: 12px;
    font-weight: 400;
    color: #333;
    box-shadow: 2px 2px 3px 0 rgba(0, 0, 0, 0.3);
    li {
      margin: 0;
      padding: 7px 16px;
      cursor: pointer;
      &:hover {
        background: #eee;
      }
    }
  }
}
</style>

<style rel="stylesheet/scss" lang="scss">
//reset element css of el-icon-close
.tags-view-wrapper {
  .tags-view-item {
    .el-icon-close {
      width: 16px;
      height: 16px;
      vertical-align: 2px;
      border-radius: 50%;
      text-align: center;
      transition: all 0.3s cubic-bezier(0.645, 0.045, 0.355, 1);
      transform-origin: 100% 50%;
      &:before {
        transform: scale(0.6);
        display: inline-block;
        vertical-align: -3px;
      }
      &:hover {
        background-color: #b4bccc;
        color: #fff;
      }
    }
  }
}
</style>
