<template>
  <editor
    ref="editor"
    :key="editorKey"
    :value="content"
    :init="initObj"
    @input="onInput"
  />
</template>

<script>
import Editor from "@tinymce/tinymce-vue";
import { uuid } from "@tinymce/tinymce-vue/lib/es2015/Utils";
import commonService from "@/services/commonService.js";

export default {
  name: "TinyMce",
  components: { Editor },
  props: {
    value: String,
    height: {
      type: Number,
      default: 300
    }
  },
  data() {
    return {
      content: this.value,
      initObj: {
        height: this.height,
        toolbar:
          "undo redo | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
        plugins: [
          "lists advlist autolink link image preview hr anchor pagebreak",
          "searchreplace wordcount visualblocks visualchars code fullscreen insertdatetime nonbreaking",
          "save table directionality paste"
        ],
        font_formats: "宋体=宋体;微软雅黑=微软雅黑;楷体=楷体;黑体=黑体;隶书=隶书;",
        language: "zh_CN",
        image_dimensions: false,
        image_description: false,
        images_upload_handler: this.imagesUploadHandler
      },
      editorKey: uuid()
    };
  },
  watch: {
    value(val) {
      this.content = val;
    }
  },
  activated() {
    this.editorKey = uuid();
  },
  methods: {
    onInput(val) {
      this.content = val;
      this.$emit("input", val);
    },
    imagesUploadHandler(blobInfo, success, failure) {
      let formData = new FormData();
      formData.append("file", blobInfo.blob(), blobInfo.filename());

      commonService
        .uploadImageAsync(formData)
        .then(url => {
          success(url);
        })
        .catch(error => {
          failure(error.message);
        });
    }
  }
};
</script>

<style lang="scss">
.tox-silver-sink {
  z-index: 10000 !important;
}
</style>
