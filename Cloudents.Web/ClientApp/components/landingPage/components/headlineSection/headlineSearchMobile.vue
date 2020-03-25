<template>
    <div class="headlineSearchMobile">
      <div class="headlineTitlesMobile">
        <h1 class="hd-titleMobile" v-t="'homePage_hd_title_mobile'"/>
        <h2 :class="['hd-subtitleMobile','mb-4']" v-t="'homePage_hd_subtitle'"/>
        <div class="hd-searchMobile">
          <div class="search-input-cont">
            <v-text-field
              class="search-input-hp"
              v-model="search"
              @keyup.enter="searchQuery"
              solo
              prepend-inner-icon="sbf-search"
              :placeholder="phSearch"
              autocomplete="off"
              hide-details
              type="search"
            ></v-text-field>
          </div>
          <div @click="searchQuery" class="search-btn" v-t="'homePage_hd_search'" />
        </div>
      </div>
      <div class="headline-img" :style="{'backgroundImage': `url(${backgroundImg}`, 'background-position': '-900px -120px'}">
        <div class="handImg" :style="{'backgroundImage': `url(${handImg}`, 'background-size': '320px'}"/>
      </div>
    </div>
</template>

<script>
import { LanguageService } from "../../../../services/language/languageService.js";
import { mapGetters } from 'vuex';

export default {
  data() {
    return {
      search: "",
      phSearch: LanguageService.getValueByKey("homePage_hd_search_ph")
    };
  },
  methods: {
    searchQuery() {
      if (this.search) {
        this.$router.push({ name: "feed", query: { term: this.search } });
      }
    }
  },
  computed: {
    ...mapGetters(['isFrymo']),
    isMobile() {
      return this.$vuetify.breakpoint.xsOnly;
    },
    backgroundImg(){
      return global.isRtl? require('../../images/bg_he.png') : require('../../images/bg_en.jpg')
    },
    handImg(){
      if(this.isFrymo){
        return require('../../images/Hand_Frymo.png')
      }else{
        if(global.isRtl){
          return require('../../images/Hand_he.png')
        }else{
          return require('../../images/Hand_en.png')
        }
      }
    }
  }
};
</script>
<style lang="less" >
@import "../../../../styles/mixin.less";

.headlineSearchMobile {
    .responsiveLandingPage(1354px, 0px);
    width: 100%;
    @media (max-width: @screen-xs) {
      margin-bottom: 14px;
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
      @media (max-width: @screen-xs) {
        height: 163px;
        width: 100%;
        overflow: hidden;
        display: flex;
        justify-content: center;
      }
      .handImg {
        width: 105%;
        height: 100%;
        background-repeat: no-repeat;
        background-size: cover;
        background-position-x: left;
        @media (max-width: @screen-xs) {
          margin-top: -20px;
          height: initial;
          margin-left: calc(~"100% / 4");
        }
      }
    }
    .headlineTitlesMobile {
        margin-left: 50px;
        z-index: 1;
        padding-bottom: 70px;
      @media (max-width: @screen-xs) {
        width: calc(~"100% - 22px");
        padding-bottom: 0;
        margin-left: 0;
      }
      width: 50%;
      .hd-titleMobile {
        @media (max-width: @screen-xs) {
          font-size: 28px;
          margin-top: 10px;
        }
        font-size: 36px;
        font-weight: bold;
      }
      .hd-subtitleMobile {
        @media (max-width: @screen-xs) {
          padding-right: 0;
          margin-top: 14px;
        }
        font-size: 16px;
        font-weight: normal;
        padding-right: 60px;
      }
      .hd-searchMobile {
        overflow: hidden;
        width: 470px;
        @media (max-width: @screen-xs) {
          width: 100%;
          height: 40px;
          border: solid 1px #c1c3d2;
          max-width: 490px;
          margin-top:10px;
        }
        height: 50px;
        border-radius: 12px;
        display: flex;
        .search-input-cont {
          background: white !important;

          flex: 2;
          height: 100%;
          .search-input-hp {
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
</style>