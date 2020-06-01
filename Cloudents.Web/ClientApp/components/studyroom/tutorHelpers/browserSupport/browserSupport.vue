<template>
    <div class="browserSupport">
        <v-flex pb-6 only>
            <v-layout wrap align-center justify-space-between class="browserSupport-header">
              <h4 v-language:inner="'browserSupport_unsupported_browser'"></h4>
              <v-icon class="closeBtn" size="12" @click="closeDialog()">sbf-close</v-icon>
            </v-layout>
        </v-flex>

        <v-flex pb-6>
            <v-layout wrap align-center justify-center class="browserSupport-center-top">
                    <v-icon class="card" color="#000000">sbf-card</v-icon>
                    <h3 v-language:inner="'browserSupport_update_browser'"></h3>
            </v-layout>
        </v-flex>

        <div class="browserSupport-center-text">
            <p v-language:inner="'browserSupport_supported_text'"></p>
        </div>
    </div>
</template>
<script>
export default {
  methods: {
    closeDialog(){
      global.onbeforeunload = function() { };
      window.location = '/'
    }
  }
}
</script>

<style lang="less">
.browserSupport {
    border-radius: 4px;
    background-color: #fff;

   .browserSupport-header {
      position: relative;
      background-color: #f0f0f7;
      h4 {
        padding: 9px 0 11px 24px;
      }
      .closeBtn{
        position: absolute;
        right: 8px;
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
}

</style>
