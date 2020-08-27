<template>
  <div class="EmailConfirmed">
    <div class="top" v-t="'loginRegister_emailconfirm_title_reset'"></div>
    <div>
      <div class="bottom">
        <span v-t="'loginRegister_emailconfirm_bottom_reset'"></span>
        <span v-t="'loginRegister_emailconfirm_bottom_reset_or'"></span>
        <div>
          <span class="link" @click="resend()" v-t="'loginRegister_emailconfirm_resend'" />&nbsp;
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { mapActions } from "vuex";

export default {
  methods: {
    ...mapActions(["resetState", "resendEmailPassword"]),
    goToRegister() {
      this.resetState();
    },
    resend() {
      this.resendEmailPassword().then(() => {
        let text = this.$t("login_email_sent");
        this.$store.dispatch("updateToasterParams", {
          toasterText: text,
          showToaster: true
        });
      });
    }
  }
};
</script>

<style lang='less'>
@import "../../../../../styles/mixin.less";
@import "../../../../../styles/colors.less";
.EmailConfirmed {
  display: flex;
  flex-direction: column;
  align-items: center;
  .top {
    .responsive-property(font-size, 28px, null, 22px);
    .responsive-property(letter-spacing, -0.51px, null, -0.4px);
    .responsive-property(margin-bottom, 28px, null, 38px);
    .responsive-property(margin-top, null, null, 42px);
    text-align: center;
    color: @color-login-text-title;
  }
  .bottom {
    @media (max-width: @screen-xs) {
      padding: 0 40px;
      line-height: inherit;
    }
    font-size: 14px;
    letter-spacing: -0.37px;
    text-align: center;
    color: #4d4b69;
    line-height: 25px;
    .link {
      cursor: pointer;
      color: #5e68ff;
    }
    div {
      .responsive-property(margin-top, inherit, null, 36px);
    }
  }
}
</style>
