<template>
  <div class="headlineSection">
    <div class="headlineSearch">
      <div class="headlineTitles">
        <h1
          class="hd-title"
          v-language:inner="isMobile?'homePage_hd_title_mobile':'homePage_hd_title'"
        />
        <h2
          :class="['hd-subtitle',isMobile?'mt-1':'mt-3']"
          v-language:inner="'homePage_hd_subtitle'"
        />
        <div class="hd-search mt-3">
          <div class="search-input-cont">
            <v-text-field
              class="search-input"
              v-model="search"
              @keyup.enter="searchQuery"
              solo
              prepend-inner-icon="sbf-search"
              :placeholder="phSearch"
              autocomplete="off"
              hide-details
            ></v-text-field>
          </div>
          <div @click="searchQuery" class="search-btn" v-language:inner="'homePage_hd_search'" />
        </div>
      </div>
      <div class="headline-img">
        <div class="handImg"></div>
      </div>
    </div>
    <headlineStatus></headlineStatus>
  </div>
</template>

<script>
import headlineStatus from "./headlineStatus.vue";
import { LanguageService } from "../../../services/language/languageService.js";

export default {
  components: { headlineStatus },
  data() {
    return {
      search: "",
      phSearch: LanguageService.getValueByKey("homePage_hd_search_ph")
    };
  },
  methods: {
    searchQuery() {
      if (this.search) {
        this.$router.push({ path: "/feed", query: { term: this.search } });
      }
    }
  },
  computed: {
    isMobile() {
      return this.$vuetify.breakpoint.xsOnly;
    }
  }
};
</script>

<style lang="less">
@import "../../../styles/mixin.less";

.headlineSection {
  width: 100%;
  height: 562px;
  @media (max-width: @sbScreen-smallDesktop) {
    height: 550px;
  }
  @media (max-width: @screen-xs) {
    height: 370px;
    background-size: 100% 50%;
    background-position-y: top;
  }
  background-image: url("../images/Hero_background.jpg");
  background-repeat: no-repeat;
  background-size: cover;
  background-position: center;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  position: relative;
  .headlineSearch {
    .responsiveLandingPage(1354px, 80px);
    @media (max-width: @screen-xs) {
      width: calc(~"100% - 20px");
      color: #43425d;
      flex-direction: column-reverse;
      justify-content: flex-end;
    }
    color: #ffffff;
    font-stretch: normal;
    font-style: normal;
    letter-spacing: normal;
    height: 100%;
    display: flex;
    align-items: center;
    margin: 0 auto;
    .headline-img {
      width: 50%;
      height: 100%;
      @media (max-width: @screen-xs) {
        height: 50%;
        width: 100%;
        overflow: hidden;
      }
      .handImg {
        width: 100%;
        height: 100%;
        background-image: url("../images/Hand_screen2.png");
        background-repeat: no-repeat;
        background-size: cover;
      }
    }
    .headlineTitles {
      @media (max-width: @screen-xs) {
        width: 100%;
      }
      width: 50%;
      .hd-title {
        @media (max-width: @screen-xs) {
          font-size: 28px;
          margin-top: 10px;
        }
        font-size: 36px;
        font-weight: bold;
      }
      .hd-subtitle {
        @media (max-width: @screen-xs) {
          padding-right: 0;
        }
        font-size: 16px;
        font-weight: normal;
        padding-right: 60px;
      }
      .hd-search {
        background: white;
        width: 470px;
        @media (max-width: @screen-xs) {
          width: 100%;
          height: 40px;
          border: solid 1px #c1c3d2;
          max-width: 490px;
        }
        height: 50px;
        border-radius: 12px;
        display: flex;
        .search-input-cont {
          flex: 2;
          height: 100%;
          .search-input {
            height: 100%;
            .v-input__control {
              height: 100%;
              min-height: initial;
              .v-input__slot {
                background: none;
                box-shadow: none;
                height: 100%;
                .v-input__icon {
                  i {
                    color: #c3c3d0;
                  }
                }
                .v-text-field__slot {
                  @media (max-width: @screen-xs) {
                    font-size: 16px;
                  }

                  font-size: 18px;
                  font-weight: normal;
                  font-stretch: normal;
                  font-style: normal;
                  line-height: normal;
                  letter-spacing: normal;
                  color: #a1a3b0;
                }
              }
            }
          }
        }
        .search-btn {
          cursor: pointer;
          @media (max-width: @screen-xs) {
            font-size: 14px;
            color: white;
          }
          font-size: 20px;
          border-top-right-radius: 12px;
          border-bottom-right-radius: 12px;
          display: flex;
          align-items: center;
          justify-content: center;
          height: 100%;
          background-color: #4c59ff;
          padding: 0 20px;
        }
      }
    }
  }
}
</style>