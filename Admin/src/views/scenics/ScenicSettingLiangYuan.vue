<template>
  <el-form ref="form" :model="input" :rules="rules" status-icon label-width="100px" class="form">
    <el-form-item prop="language" label="语言">
      <el-select v-model="input.language" placeholder="请选择语言">
        <el-option
          v-for="item in languages"
          :key="item.value"
          :label="item.displayText"
          :value="item.value"
        ></el-option>
      </el-select>
    </el-form-item>
    <div class="setting-title">微信购票参数</div>
    <el-form-item prop="scenicName" :label="pageLabelMainText + '名称'">
      <el-input v-model="input.scenicName" :maxlength="50"></el-input>
    </el-form-item>
    <el-form-item :label="pageLabelMainText + '照片'">
      <file-upload v-model="input.photoList" :max-size="500">
        <el-button type="primary">点击上传</el-button>
        <div slot="tip" class="el-upload__tip">只能上传jpg、png文件，且不超过500kb，宽高比为640*360</div>
      </file-upload>
    </el-form-item>
    <!-- <el-form-item label="开放时间">
      <tiny-mce v-model="input.openingHours" :key="input.id"></tiny-mce>
    </el-form-item> -->
    <el-form-item prop="bookNotes" label="来馆须知">
      <tiny-mce v-model="input.bookNotes" :key="input.id"></tiny-mce>
    </el-form-item>
    <el-form-item prop="scenicIntro" label="参观须知">
      <tiny-mce v-model="input.scenicIntro" :key="input.id"></tiny-mce>
    </el-form-item>
    <!-- <el-form-item label="联系信息">
      <tiny-mce v-model="input.contactInfo" :key="input.id"></tiny-mce>
    </el-form-item> -->
    <el-form-item>
      <el-button type="primary" :loading="saving" @click="save">保存</el-button>
    </el-form-item>
  </el-form>
</template>

<script>
import scenicService from "./../../services/scenicService.js";

class EditInput {
  constructor() {
    this.language = "zh-CN";
    this.scenicName = "";
    this.openTime = "";
    this.closeTime = "";
    this.openingHours = "";
    this.photoList = [];
    this.scenicIntro = "";
    this.scenicFeature = "";
    this.bookNotes = "";
    this.noticeTitle = "";
    this.noticeContent = "";
    this.longitude = "";
    this.latitude = "";
    this.address = "";
    this.contactInfo = "";
    this.indexLink = "";
    this.freeTicketPolicy = "";
    this.favouredPolicy = "";
    this.invoicing = "";
    this.importantNote = "";
    this.safetyInstructions = "";
  }
}

export default {
  name: "ScenicSettingLiangYuan",
  data() {
    return {
      input: new EditInput(),
      saving: false,
      languages: [
        { displayText: "中文", value: "zh-CN" },
        { displayText: "英文", value: "en-US" },
      ],
      timeOptions: {
        format: "HH:mm",
      },
      autosize: { minRows: 5 },
      rules: {
        language: [{ required: true, message: "请选择语言", trigger: "change" }],
        scenicName: [
          { required: true, message: "请输入景区名称", trigger: "blur" },
          { max: 50, message: "长度不能超过50个字符", trigger: "blur" },
        ],
        bookNotes: [{ required: true, message: "请输入来馆须知", trigger: "blur" }],
        scenicIntro: [{ required: true, message: "请输入参观须知", trigger: "blur" }],
        noticeTitle: [{ max: 50, message: "长度不能超过50个字符", trigger: "blur" }],
        address: [{ max: 200, message: "长度不能超过200个字符", trigger: "blur" }],
      },
      pageLabelMainText: "景区",
    };
  },
  async created() {
    await this.getScenics();
    this.pageLabelMainText = scenicService.getPageLabelMainText();
    this.rules.scenicName[0].message = "请输入" + this.pageLabelMainText + "名称";
    // this.rules.scenicIntro[0].message = "请输入" + this.pageLabelMainText + "简介";
  },
  watch: {
    "input.language": async function () {
      await this.getScenics();
    },
  },
  methods: {
    async getScenics() {
      const scenic = await scenicService.getScenicAsync(this.input.language);
      scenic.language = scenic.language || this.input.language;

      this.input = scenic;
    },
    save() {
      this.$refs.form.validate(async (valid) => {
        if (!valid) {
          return false;
        }
        if (this.input.photoList.length < 1) {
          this.$message.error(`请选择${this.pageLabelMainText}照片`);
        } else {
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
        }
      });
    },
  },
};
</script>

<style lang="scss" scoped>
.form {
  width: 50%;
  margin-top: 20px;
}
.setting-title {
  padding: 0px 0px 10px 30px;
}
</style>
