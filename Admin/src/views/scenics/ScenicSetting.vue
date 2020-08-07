<template>
  <el-form
    ref="form"
    :model="input"
    :rules="rules"
    status-icon
    label-width="100px"
    class="form"
  >
    <el-form-item prop="scenicName" :label="pageLabelMainText + '名称'">
      <el-input v-model="input.scenicName" :maxlength="50"></el-input>
    </el-form-item>
    <el-form-item label="开放时间">
      <el-time-picker
        v-model="input.openTime"
        :picker-options="timeOptions"
        value-format="HH:mm"
      ></el-time-picker>
    </el-form-item>
    <el-form-item label="至">
      <el-time-picker
        v-model="input.closeTime"
        :picker-options="timeOptions"
        value-format="HH:mm"
      ></el-time-picker>
    </el-form-item>
    <el-form-item :label="pageLabelMainText + '照片'">
      <file-upload v-model="input.photoList" :max-size="500">
        <el-button type="primary">点击上传</el-button>
        <div slot="tip" class="el-upload__tip">
          只能上传jpg、png文件，且不超过500kb，宽高比为640*360
        </div>
      </file-upload>
    </el-form-item>
    <el-form-item prop="scenicIntro" :label="pageLabelMainText + '简介'">
      <tiny-mce v-model="input.scenicIntro"></tiny-mce>
    </el-form-item>
    <el-form-item :label="pageLabelMainText + '特色'">
      <tiny-mce v-model="input.scenicFeature"></tiny-mce>
    </el-form-item>
    <el-form-item prop="noticeTitle" label="公告标题">
      <el-input v-model="input.noticeTitle" :maxlength="50"></el-input>
    </el-form-item>
    <el-form-item label="公告内容">
      <tiny-mce v-model="input.noticeContent"></tiny-mce>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" :loading="saving" @click="save">保存</el-button>
    </el-form-item>
  </el-form>
</template>

<script>
import scenicService from "./../../services/scenicService.js";

class EditInput {
  constructor() {
    this.scenicName = "";
    this.openTime = "";
    this.closeTime = "";
    this.photoList = [];
    this.scenicIntro = "";
    this.scenicFeature = "";
    this.noticeTitle = "";
    this.noticeContent = "";
    this.longitude = "";
    this.latitude = "";
    this.address = "";
  }
}

export default {
  name: "ScenicSetting",
  data() {
    return {
      input: new EditInput(),
      saving: false,
      timeOptions: {
        format: "HH:mm"
      },
      autosize: { minRows: 5 },
      rules: {
        scenicName: [
          { required: true, message: "请输入景区名称", trigger: "blur" },
          { max: 50, message: "长度不能超过50个字符", trigger: "blur" }
        ],
        scenicIntro: [{ required: true, message: "请输入景区简介", trigger: "blur" }],
        noticeTitle: [{ max: 50, message: "长度不能超过50个字符", trigger: "blur" }],
        address: [{ max: 200, message: "长度不能超过200个字符", trigger: "blur" }]
      },
      pageLabelMainText: '景区'
    };
  },
  async created() {
    this.input = await scenicService.getScenicAsync();
    this.pageLabelMainText = scenicService.getPageLabelMainText();
    this.rules.scenicName[0].message = "请输入" + this.pageLabelMainText + "名称";
    this.rules.scenicIntro[0].message = "请输入" + this.pageLabelMainText + "简介";
  },
  methods: {
    save() {
      this.$refs.form.validate(async valid => {
        if (!valid) {
          return false;
        }

        try {
          this.saving = true;
          const result = await scenicService.editScenicAsync(this.input);
          if (result.success) {
            this.$message.success("保存成功");
          }
        } catch (err) {
          return;
        } finally {
          this.saving = false;
        }
      });
    }
  }
};
</script>

<style lang="scss" scoped>
.form {
  width: 50%;
  margin-top: 20px;
}
</style>
