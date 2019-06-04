<template>
    <div class="browserSupport">
        <v-flex pb-5>
            <v-layout row wrap align-center justify-space-between class="browserSupport-header">
              <h4>Unsupported Browser</h4>
              <v-icon class="close">sbf-close</v-icon>
            </v-layout>
        </v-flex>

        <v-flex pb-4>
            <v-layout row wrap align-center justify-center class="browserSupport-center-top">
                    <v-icon class="card" color="#000000">sbf-card</v-icon>
                    <h3>Update Your Browser</h3>
            </v-layout>
        </v-flex>

        <div class="browserSupport-center-text">
            <p>
              Your’e using a web browser that isn’t fully supported by Spitball.
              To get a better experience go to on of these sites and get the latest version of your preffered browser
            </p>
        </div>

        <v-flex pb-3>
            <v-layout align-center justify-center text-xs-center class="browserSupport-center-footer">
                <div class="">
                    <img src="../../images/Safari.png" alt="Safari">
                    <p>Safari</p>
                </div>
                <div class="">
                    <img src="../../images/Chrome.png" alt="Chrome">
                    <p>Google Chrome</p>
                </div>
                <div class="">
                    <img src="../../images/Firefox.png" alt="Firefox">
                    <p>Mozilla Fierfox</p>
                </div>
            </v-layout>
        </v-flex>

    </div>
</template>

<script>

export default {

  data() {
    return {

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
    }

    .browserSupport-center-footer {
      p {
        color: rgba(68, 82, 252, 0.87);
        font-weight: 600;
      }
    }
}
</style>
