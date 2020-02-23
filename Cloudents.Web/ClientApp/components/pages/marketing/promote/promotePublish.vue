<template>
    <div class="promotePublish mx-auto pa-4">
        <div class="wrap text-center" :key="template.id">
            <img class="img" :src="template.imageLarge" alt="">
            <div class="bottom">
                <div class="shareIt text-left">Share it</div>
                <div class="btnWrap">
                    <v-btn small medium class="elevation-0 ma-2 ml-0" color="#305d98">
                        <svg xmlns="http://www.w3.org/2000/svg" width="11" height="20" viewBox="0 0 11 20">
                            <path fill="#FEFEFE" fill-rule="evenodd" d="M6.867 5.823V4.307c0-.738.528-.91.898-.91h2.275V.15L6.903.138C3.42.138 2.63 2.558 2.63 4.11v1.714H.62v3.789h2.03v9.474h4.06V9.612h3.013l.147-1.49.223-2.299H6.867z"/>
                        </svg>
                    </v-btn>
                    <v-btn small class="elevation-0 ma-2" color="#2cb742">
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 21 20">
                            <g fill="#FEFEFE" fill-rule="evenodd">
                                <path d="M20.699 9.383C20.562 4.18 16.069 0 10.542 0 5.076 0 .622 4.083.388 9.203c-.006.14-.013.28-.013.424 0 1.818.534 3.519 1.457 4.97L0 19.764l5.627-1.705a10.607 10.607 0 004.915 1.198c5.613 0 10.164-4.313 10.164-9.63 0-.08-.004-.164-.007-.244m-10.157 8.34a8.831 8.831 0 01-4.704-1.341l-3.283.997 1.066-3.006a7.79 7.79 0 01-1.585-5.53c.418-4.1 4.073-7.312 8.506-7.312 4.487 0 8.174 3.296 8.515 7.462.017.21.03.42.03.634 0 4.463-3.834 8.096-8.545 8.096"/>
                                <path d="M15.196 11.568c-.248-.116-1.475-.687-1.702-.767-.23-.077-.394-.117-.561.12-.168.233-.646.767-.79.925-.147.156-.29.176-.538.059-.25-.12-1.053-.367-2.005-1.17-.74-.627-1.241-1.402-1.385-1.635-.147-.237-.017-.364.107-.484.114-.103.25-.273.374-.413.038-.04.064-.077.091-.11.063-.09.1-.17.16-.284.084-.157.04-.294-.02-.413-.064-.118-.562-1.278-.769-1.752-.207-.474-.415-.394-.562-.394-.147 0-.31-.02-.478-.02a.932.932 0 00-.662.297c-.23.237-.872.807-.872 1.969 0 .273.05.543.127.804.25.833.792 1.517.889 1.637.124.157 1.725 2.615 4.256 3.563 2.534.944 2.534.63 2.99.59.457-.04 1.474-.57 1.684-1.124.204-.55.204-1.024.144-1.12-.063-.101-.228-.158-.478-.278"/>
                            </g>
                        </svg>
                    </v-btn>
                    <v-btn small class="elevation-0 ma-2" color="#45ceff">
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 21 16">
                            <path fill="#FEFEFE" fill-rule="evenodd" d="M21.082 1.875a9.21 9.21 0 01-2.481.626c.891-.493 1.576-1.277 1.902-2.208a9.082 9.082 0 01-2.75.968C16.966.487 15.842 0 14.596 0c-2.386 0-4.323 1.788-4.323 3.993 0 .313.037.617.109.91C6.79 4.737 3.602 3.145 1.47.731a3.736 3.736 0 00-.59 2.008c0 1.384.767 2.605 1.927 3.322a4.57 4.57 0 01-1.96-.5v.05c0 .513.105 1 .296 1.447.528 1.254 1.719 2.198 3.176 2.469a4.748 4.748 0 01-1.957.07c.554 1.584 2.151 2.738 4.04 2.772-1.48 1.07-3.343 1.708-5.372 1.708-.347 0-.69-.021-1.031-.057a12.997 12.997 0 006.631 1.794c6.761 0 10.917-4.393 12.013-8.95a10.258 10.258 0 00.283-2.925 8.44 8.44 0 002.155-2.064"/>
                        </svg>
                    </v-btn>
                    <v-btn small class="elevation-0 ma-2 mr-0" color="#878693">
                        <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 22 14">
                            <defs>
                                <path id="prefix__a" d="M0 0L21.412 0 21.412 14 0 14z"/>
                            </defs>
                            <g fill="none" fill-rule="evenodd">
                                <mask id="prefix__b" fill="#fff">
                                    <use xlink:href="#prefix__a"/>
                                </mask>
                                <path fill="#FFF" d="M21.346 1.834L15.83 7.08l5.517 5.256.066.067V1.764c-.02.024-.044.045-.066.07zm-6.66 6.335l-1.11 1.059a4.032 4.032 0 01-2.784 1.099A4.018 4.018 0 018.01 9.223L6.727 8.001l-5.67 5.395c-.276.258-.422.485-.474.604h20.138a2.614 2.614 0 00-.363-.428L14.687 8.17zM6.727 5.82l1.145 1.092 1.131 1.08c.473.45 1.126.694 1.79.69a2.593 2.593 0 001.792-.69l.955-.912 1.148-1.09L20.353.6c.272-.257.418-.478.471-.6H.695c.079.12.19.258.362.42l5.67 5.401zM.065 1.654C.041 1.633.024 1.61 0 1.587v10.64l.065-.068 5.517-5.246L.064 1.654z" mask="url(#prefix__b)"/>
                            </g>
                        </svg>
                    </v-btn>
                </div>
                <div class="copyBtn mt-3">
                    <div class="wrap">
                        <input type="text" class="copy text-truncate" name="" :value="video.product" ref="copy">
                        <button type="button" class="buttonCopy px-5" @click="copyLink" name="button">Copy</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
export default {
  props: {
    template: {
      type: Object,
      default: () => ({})
    },
    video: {
      type: Object,
      default: () => ({})
    }
  },
  data() {
    return {

    }
  },
  methods: {
    copyLink() {
      const copyText = this.$refs.copy;
      copyText.select();
      copyText.setSelectionRange(0, 99999)
      document.execCommand("copy");
    }
  }
}
</script>
<style lang="less">
.promotePublish {
  max-width: 454px;
  border-radius: 8px;
  border: solid 1px #dddddd;

  .wrap {
    width: 100%;
    .img {
      width: 100%;
    }
  }
  .bottom {
    .shareIt {
      font-size: 16px;
    }
    .btnWrap {
      display: flex;
      justify-content: space-between;
      button {
        min-width: 90px !important;
        flex: 1;
        @media (max-width: 599px) {
          min-width: 46px !important;
          // width: 16%;
          // margin: 0 !important;
        }
        svg {
          width: 20px;
        }
      }
    }
    .copyBtn {
      .wrap {
        display: flex;
        justify-content: flex-end;
        border: solid 1px #dddddd;
        border-radius: 8px;
        height: 34px;
        .copy {
          flex-grow: 1;
          width: 100%;
          outline: none;
          padding: 0 10px;
          color: #43425d;
          opacity: 0.5
        }
        .buttonCopy {
          outline: none;
          background: rgba(189, 192, 209, 0.5);
        }
      }
    }
  }
}
</style>
