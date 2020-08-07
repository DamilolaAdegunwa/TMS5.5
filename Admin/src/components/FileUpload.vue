<template>
  <el-upload
    v-bind="$attrs"
    :action="uploadUrl"
    :headers="headers"
    :limit="limit"
    :on-exceed="onExceed"
    :before-upload="beforeUpload"
    :on-success="onSuccess"
    :on-remove="onRemove"
    :file-list="files"
    :list-type="listType"
    class="file-upload"
  >
    <slot></slot>
    <slot name="tip"></slot>
  </el-upload>
</template>

<script>
import combineURLs from "axios/lib/helpers/combineURLs.js";
import { baseURL } from "./../utils/ajax.js";
import tokenService from "./../services/tokenService.js";

const urls = {
  image: "common/UploadImageAsync"
};

const fileTypes = {
  image: ["image/jpeg", "image/png"]
};

export default {
  name: "FileUpload",
  inheritAttrs: false,
  props: {
    value: {
      type: Array,
      default() {
        return [];
      }
    },
    limit: {
      type: Number,
      default: 5
    },
    listType: {
      type: String,
      default: "picture"
    },
    fileType: {
      type: String,
      default: "image"
    },
    maxSize: {
      type: Number,
      default: 1024
    }
  },
  data() {
    return {
      files: this.value
    };
  },
  computed: {
    headers() {
      return {
        Authorization: `Bearer ${tokenService.getToken()}`
      };
    },
    uploadUrl() {
      return combineURLs(baseURL + "/api", urls[this.fileType]);
    }
  },
  watch: {
    value(val) {
      this.files = val;
    }
  },
  methods: {
    onExceed() {
      this.$message.error(`最多上传${this.limit}个文件`);
    },
    beforeUpload(file) {
      const isValidType = fileTypes[this.fileType].includes(file.type);
      if (!isValidType) {
        this.$message.error("文件格式不正确");
      }

      const isValidSize = file.size / 1024 <= this.maxSize;
      if (!isValidSize) {
        this.$message.error(`文件大小不能超过 ${this.maxSize}kb`);
      }

      return isValidType && isValidSize;
    },
    onSuccess(response, file) {
      this.files.push({
        name: file.name,
        uid: file.uid,
        url: response.result
      });
      this.$emit("input", this.files);
    },
    onRemove(file) {
      const index = this.files.findIndex(f => f.uid == file.uid);
      this.files.splice(index, 1);
      this.$emit("input", this.files);
    }
  }
};
</script>

<style lang="scss" scoped>
.file-upload {
  /deep/ .el-upload {
    text-align: left;
  }
}
</style>
