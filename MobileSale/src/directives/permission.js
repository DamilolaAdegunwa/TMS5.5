import memberService from "./../services/memberService.js";

export default {
  inserted(el, binding) {
    const { value } = binding;
    const permissions = memberService.getMember().permissions;

    if (value && value instanceof Array && value.length > 0) {
      const hasPermission = value.some(p => {
        return permissions.some(permission => permission.toUpperCase() == p.toUpperCase());
      });

      if (!hasPermission) {
        el.parentNode && el.parentNode.removeChild(el);
      }
    } else {
      throw new Error(`need roles! Like v-permission="['admin','editor']"`);
    }
  }
};
