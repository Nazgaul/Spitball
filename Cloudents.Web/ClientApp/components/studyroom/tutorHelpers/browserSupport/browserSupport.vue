<template>
    <div class="browserSupport">
        <v-flex pb-5 only>
            <v-layout row wrap align-center justify-space-between class="browserSupport-header">
              <h4 v-language:inner="'browserSupport_unsupported_browser'"></h4>
              <!-- <v-icon class="close" @click="closeDialog()">sbf-close</v-icon> -->
            </v-layout>
        </v-flex>

        <v-flex pb-4>
            <v-layout row wrap align-center justify-center class="browserSupport-center-top">
                    <v-icon class="card" color="#000000">sbf-card</v-icon>
                    <h3 v-language:inner="'browserSupport_update_browser'"></h3>
            </v-layout>
        </v-flex>

        <div class="browserSupport-center-text">
            <p v-language:inner="'browserSupport_supported_text'"></p>
        </div>

        <v-flex pb-3>
            <v-layout :class="{'column': isMobile}" align-center justify-center text-xs-center class="browserSupport-center-footer">
                <div>
                    <img src="../../images/Safari.png" alt="Safari">
                    <p v-language:inner="'browserSupport_supported_safari'"></p>
                </div>
                <div>
                    <img src="../../images/Chrome.png" alt="Chrome">
                    <p v-language:inner="'browserSupport_supported_chrome'"></p>
                </div>
                <div>
                    <img src="../../images/Firefox.png" alt="Firefox">
                    <p v-language:inner="'browserSupport_supported_firefox'"></p>
                </div>
            </v-layout>
        </v-flex>

    </div>
</template>

<script>
export default {
  computed:{
    isMobile(){
      return this.$vuetify.breakpoint.xsOnly;
    }
  }

}
</script>

<style lang="less">
.browserSupport {
    border-radius: 4px;
    background-color: #fff;

   .browserSupport-header {
      background-color: #f0f0f7;
      h4 {
        padding: 9px 0 11px 24px;
      }
      .close {
        font-size: 12px;
        padding: 14px;
        opacity: .4;
        cursor: pointer;
      }
    }

    .browserSupport-center-top {
      h3 {
        margin-left: 10px;
      }
      .card {
        font-size: 16px;
      }
    }

    .browserSupport-center-text {
        text-align: center;
        padding-bottom: 20px;
        p {
         margin: 0 10px 0 0px;
         padding: 0 72px;
       }
       @media screen and (min-width: 325px) and (max-width: 425px) {
         .browserSupport-center-text {
           padding: 0 20px;
         }
       }
    }

    .browserSupport-center-footer {
      p {
        color: rgba(68, 82, 252, 0.87);
        font-weight: 600;
      }

      }
}

</style>
