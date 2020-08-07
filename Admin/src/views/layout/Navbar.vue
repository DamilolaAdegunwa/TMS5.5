<template>
  <div class="navbar-container">
    <div class="navbar-left">
      <i
        class="al-icon-hanbao hamburger"
        :class="{'is-collapse':isSidebarCollapse}"
        @click="toggleSidebar"
      />
      <breadcrumb/>
    </div>
    <div>
      <el-dropdown size="small" @command="handleCommand" class="avatar-container" trigger="click">
        <div class="avatar-wrapper">
          <div class="user-avatar">
            <img v-if="staff.profileUrl" :src="staff.profileUrl">
            <i v-else class="al-icon-yonghu"/>
          </div>
          <span class="info">{{staff.name}}</span>
        </div>
        <el-dropdown-menu slot="dropdown">
          <el-dropdown-item command="home">
            <div>
              <i class="al-icon-home"/>
              <span>首页</span>
            </div>
          </el-dropdown-item>
          <el-dropdown-item command="editPassword" divided>
            <div>
              <i class="al-icon-edit"/>
              <span>修改密码</span>
            </div>
          </el-dropdown-item>
          <el-dropdown-item command="logout" divided>
            <div>
              <i class="al-icon-tuichu"/>
              <span>退出登录</span>
            </div>
          </el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>
      <edit-password v-model="showEditPassword"/>
    </div>
  </div>
</template>

<script>
import { mapState, mapMutations } from "vuex";
import { modules, app } from "./../../store/types.js";
import Breadcrumb from "./../../components/Breadcrumb.vue";
import staffService from "./../../services/staffService.js";
import EditPassword from "./../staffs/EditPassword.vue";

export default {
  name: "navbar",
  components: { Breadcrumb, EditPassword },
  data() {
    return {
      staff: {},
      showEditPassword: false
    };
  },
  computed: {
    ...mapState(modules.app, [app.isSidebarCollapse])
  },
  created() {
    this.staff = staffService.getStaffInfo();
  },
  methods: {
    handleCommand(command) {
      switch (command) {
        case "home": {
          this.$router.push("/");
          break;
        }
        case "editPassword": {
          this.showEditPassword = true;
          break;
        }
        case "logout": {
          this.logout();
          break;
        }
      }
    },
    logout() {
      staffService.logout();
      this.$router.push({ name: "Login" });
    },
    ...mapMutations(modules.app, [app.toggleSidebar])
  }
};
</script>

<style lang="scss" scoped>
@import "./../../styles/variables.scss";
.navbar-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: $nav-height;
  box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
}
.navbar-left {
  display: flex;
  align-items: center;
}
.hamburger {
  padding: 0 10px;
  cursor: pointer;
  transform: rotate(0deg);
  transition: 0.38s;
  transform-origin: 50% 50%;
}
.hamburger.is-collapse {
  transform: rotate(90deg);
}
.avatar-container {
  margin-right: 20px;

  .avatar-wrapper {
    display: flex;
    align-items: center;
    cursor: pointer;

    .user-avatar {
      width: 24px;
      height: 24px;
      line-height: 24px;
      text-align: center;
      border-radius: 50%;
      color: #fff;
      background-color: rgb(166, 219, 255);

      img {
        width: 100%;
        height: 100%;
        display: block;
      }
    }

    .info {
      display: inline-block;
      margin-left: 8px;
    }
  }
}
.el-dropdown-menu__item {
  .link {
    color: #606266;
    text-decoration: none;
  }

  i {
    margin-right: 5px;
  }
}
</style>
