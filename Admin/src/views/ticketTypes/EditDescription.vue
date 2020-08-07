<template>
  <el-dialog
    :title="title"
    :visible.sync="showDialog"
    :close-on-click-modal="false"
    @closed="onClosed"
    width="40%"
  >
    <el-form
      ref="form"
      :model="input"
      :rules="rules"
      status-icon
      label-width="100px"
    >
      <el-form-item prop="ticketTypeId" label="票类">
        <el-select v-model="input.ticketTypeId" filterable placeholder="请选择">
          <el-option
            v-for="item in ticketTypes"
            :key="item.value"
            :label="item.displayText"
            :value="item.value"
          ></el-option>
        </el-select>
      </el-form-item>
      <el-form-item prop="bookDescription" label="预订说明">
        <tiny-mce v-model="input.bookDescription"></tiny-mce>
      </el-form-item>
      <el-form-item label="费用说明">
        <tiny-mce v-model="input.feeDescription"></tiny-mce>
      </el-form-item>
      <el-form-item label="使用说明">
        <tiny-mce v-model="input.usageDescription"></tiny-mce>
      </el-form-item>
      <el-form-item label="退改说明">
        <tiny-mce v-model="input.refundDescription"></tiny-mce>
      </el-form-item>
      <el-form-item label="其他说明">
        <tiny-mce v-model="input.otherDescription"></tiny-mce>
      </el-form-item>
    </el-form>
    <div slot="footer" class="dialog-footer">
      <el-button @click="close">取 消</el-button>
      <el-button type="primary" :loading="saving" @click="save"
        >确 定</el-button
      >
    </div>
  </el-dialog>
</template>

<script>
import { editViewMixin } from "./../../mixins/editViewMixin.js";
import ticketTypeService from "./../../services/ticketTypeService.js";

class EditInput {
  constructor() {
    this.ticketTypeId = "";
    this.bookDescription = "";
    this.feeDescription = "";
    this.usageDescription = "";
    this.refundDescription = "";
    this.otherDescription = "";
  }
}

export default {
  name: "EditTicketTypeDescription",
  mixins: [editViewMixin],
  props: {
    ticketTypeId: [Number, String]
  },
  data() {
    return {
      input: new EditInput(),
      ticketTypes: [],
      autosize: { minRows: 2 },
      rules: {
        ticketTypeId: [{ required: true, message: "请选择票类", trigger: "change" }],
        bookDescription: [{ required: true, message: "请输入预订说明", trigger: "blur" }]
      }
    };
  },
  computed: {
    title() {
      return `${this.editText}票类说明`;
    }
  },
  async created() {
    this.ticketTypes = await ticketTypeService.getNetSaleTicketTypeComboboxItemsAsync();
  },
  methods: {
    getDataForEdit() {
      ticketTypeService.getTicketTypeDescriptionAsync(this.ticketTypeId).then(data => {
        this.input = data;
      });
    },
    async create() {
      return await ticketTypeService.createDescriptionAsync(this.input);
    },
    async update() {
      return await ticketTypeService.updateDescriptionAsync(this.input);
    },
    clear() {
      this.input = new EditInput();
    }
  }
};
</script>
